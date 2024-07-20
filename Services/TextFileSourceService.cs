using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Exceptions;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Update.Internal;


namespace GenGui_CrystalStar.Services;

public interface ITextFileSourceService
{
    void Init();
    Task<Response<ResultCode>> RefreshAllTags();
    Task<Response<ResultCode>> RefreshTagBlock (string blockName);
}

public class TextFileSourceService : ITextFileSourceService
{
    private static List<TextFile> _textFiles { get; set; } = [];
    private static readonly string rx_BlockStart = @"(?i)^\s*\{\s*\w*:.*$";
    private static readonly string rx_FlagMatch = @"(?i)\W*\w*:\s*(\w*)$";
    private static readonly string rx_BlockName = @"(?i)^\W*(\w*)";
    private static readonly string rx_BlockStartOrEnd = @"^\s*\}\s*$|^\s*\{";
    private static readonly string rx_BlockEnd = @"^\s*\}\s*$";

    // private GenGuiContext _database { get; set; }
    private ITextFileSourceSettings _dataPath { get; set; }
    private IGenGuiDataService _dataService { get; set; }

    public TextFileSourceService(/* GenGuiContext database, */ ITextFileSourceSettings dataPath, IGenGuiDataService dataService)
    {
        // _database = database;
        _dataPath = dataPath;
        _dataService = dataService;
    }

    public void Init()
    {
        Task setFilePath = Task.Run(() => SetFilePath(_dataPath.Path));
        Task.WaitAll(setFilePath);

        if(setFilePath.IsCompletedSuccessfully == false)
        {
            throw new Exception("Error setting file path");
        }

        else
        {

        // Task loadAllTextFiles = Task.Run(LoadAllTextFiles);
        Task loadAllTextFiles = Task.Run(LoadAllTextFiles);
        Task.WaitAll(loadAllTextFiles);


        if(loadAllTextFiles.IsCompletedSuccessfully == true)
        {
            WatchFiles();
        }

        else
        {
            throw new Exception("Error loading text files");
        }
        }
    }

    public async Task<Response<ResultCode>> RefreshTagBlock (string blockName)
    {
        var matchBlockFile = await _dataService.GetBlockFile(blockName);
        if (matchBlockFile.Success == false)
            return new Response<ResultCode>(matchBlockFile.Exception, ResultCode.Error);

        var textFile = _textFiles.FirstOrDefault(x => x.FilePath == matchBlockFile.Data.FilePath);

        var deleteTagBlock = await _dataService.DeleteTagBlock(blockName);
        if (deleteTagBlock.Success == false)
            return new Response<ResultCode>(deleteTagBlock.Exception, ResultCode.Error);

        var loadTagBlock = LoadTagBlock(textFile!, blockName);
        if (loadTagBlock.Success == false)
            return new Response<ResultCode>(loadTagBlock.Exception, ResultCode.Error);

        var insertTagBlock = await _dataService.InsertTags(loadTagBlock.Data);
        if (insertTagBlock.Success == false)
            return new Response<ResultCode>(insertTagBlock.Exception, ResultCode.Error);



        await _dataService.UpdateSingleBlockTagCount(blockName, loadTagBlock.Data.Count);

        return new Response<ResultCode>(ResultCode.Okay);
    }

    private Response<List<Tags>> LoadTagBlock(TextFile textFile, string blockName)
    {
        var tagList = new List<Tags>();
        var lines = new List<string> { Capacity = 20000 };
        var text =  File.ReadAllLines(textFile.FilePath);
        lines.AddRange(text);

        for (int lineNum = 0; lineNum < lines.Count; lineNum++)
        {
            if (Regex.IsMatch(lines[lineNum], rx_BlockStart))
            {
                var foundBlockName = Regex.Match(lines[lineNum], rx_BlockName).Groups[1].ToString();
                if (foundBlockName == blockName)
                {
                    var flagmatch = Regex.Match(lines[lineNum], rx_FlagMatch);
                    var blockFlag  = new FlagResponse<string>(flagmatch.Groups[1].ToString()).Code;

                    lineNum++;

                    int blocklineNum = 1;
                    while (lineNum < lines.Count && Regex.IsMatch(lines[lineNum], rx_BlockStartOrEnd) == false)
                    {
                        var newTag = new TagMaker().MakeTag(lines[lineNum], blocklineNum);
                        if( newTag is null)
                        {
                            Console.WriteLine($"{lines[lineNum]} is an invalid line");
                        }

                        else
                        {
                            newTag.BlockName = blockName;
                            newTag.BlockFlag = (BlockFlag)blockFlag; // hope that works
                            tagList.Add(newTag);
                        }
                        blocklineNum++;
                        lineNum++;
                    }

                    return new Response<List<Tags>>(tagList);
                }
            }
        }
        return new Response<List<Tags>>("TagBlock not found...?", ResultCode.NotFound);

    }

    // filewatcher file OnChanged, but not renamed or created/deleted
    public async Task<Response<ResultCode>> WatcherFileChanged(TextFile textFile)
    {

        bool DoesFlagBlockPairExist(TextFile tFile, BlockFiles blkFile)
        {
            var foundPair = tFile.ContainedBlocks.FirstOrDefault(x => x.Key == blkFile.BlockName);
            if (!foundPair.Equals(default(KeyValuePair<string, int>)))
                return foundPair.Value == (int)blkFile.BlockFlag;
            else
                return false;
        }

        // Try not to change the _textFiles List unless we have to
        var blockStates = LoadBlockStates(textFile);
        var matchedTextFile = _textFiles.FirstOrDefault(x => x.FilePath == textFile.FilePath); // will always exist if only used for watcher OnChanged
        int flagBlockChanges = 0;  // if goes above 0, need to change the _textFiles list as ContainedBlocks will change

        while (flagBlockChanges == 0)
        {
            foreach (var blockState in blockStates.Data)
            {
                if (matchedTextFile is not null) // path didn't change
                {
                    if (DoesFlagBlockPairExist(matchedTextFile, blockState) == false) // blockname and blockflag changed :(
                        flagBlockChanges++; // will break while loop if there is a change
                }
            }
            break; // no blockStates changed :), exit the while loop
        }

        if (flagBlockChanges == 0) // no blockStates changed, just reload the tags
        {
            foreach (var blockState in blockStates.Data)
            {
                    var deleteTagBlock = await _dataService.DeleteTagBlock(blockState.BlockName!);
                    if (deleteTagBlock.Success == false)
                        return new Response<ResultCode>(deleteTagBlock.Exception!, ResultCode.Error);

                    var tagList = LoadTextFile(textFile);

                    var insertTagBlock = await _dataService.InsertTags(tagList.Data);
                    if (insertTagBlock.Success == false)
                        return new Response<ResultCode>(insertTagBlock.Exception!, ResultCode.Error);

                    await _dataService.UpdateSingleBlockTagCount(blockState.BlockName!, tagList.Data.Count);
            }

            return new Response<ResultCode>(ResultCode.Okay);
        }

        else // path, blockname, or blockflag changed, gotta reload :(
        {
            var removeTextFileState = await RemoveTextFileState(textFile);
            if (removeTextFileState.Success == false)
                return new Response<ResultCode>(removeTextFileState.Exception!, ResultCode.Error);

            var addTextFileState = await AddTextFileState(textFile);
            if (addTextFileState.Success == false)
                return new Response<ResultCode>(addTextFileState.Exception!, ResultCode.Error);

            return new Response<ResultCode>(ResultCode.Okay);
        }
    }

    public async Task<Response<ResultCode>> RefreshTextFile(TextFile oldTextFile, TextFile newTextFile)
    {
        var removeTextFileState = await RemoveTextFileState(oldTextFile);
        if (removeTextFileState.Success == false)
            return new Response<ResultCode>(removeTextFileState.Exception!, ResultCode.Error);

        var addTextFileState = await AddTextFileState(newTextFile);
        if (addTextFileState.Success == false)
            return new Response<ResultCode>(addTextFileState.Exception!, ResultCode.Error);

        return new Response<ResultCode>(ResultCode.Okay);
    }

    // Only for adding new text file or following a RemoveTextFileState after a rename
    public async Task<Response<ResultCode>> AddTextFileState(TextFile textFile)
    {
        var tagsList = LoadTextFile(textFile);
        if (tagsList.Data.Any())
        {
            var insertTags = await _dataService.InsertTags(tagsList.Data);

            if (insertTags.Success == false){
                return new Response<ResultCode>(insertTags.Exception!, ResultCode.Error);}
        }

        var blockFiles = LoadBlockStates(textFile);
        if (blockFiles.Data.Any())
        {
            var insertBlockFiles = await _dataService.InsertBlockFiles(blockFiles.Data);

            if (insertBlockFiles.Success == false){
                return new Response<ResultCode>(insertBlockFiles.Exception!, ResultCode.Error);}
        }

        var blocks = LoadBlocks(textFile);
        if (blocks.Data.Any())
        {
            var insertBlocks = await _dataService.InsertBlocks(blocks.Data);

            if (insertBlocks.Success == false){
                return new Response<ResultCode>(insertBlocks.Exception!, ResultCode.Error);}
        }

        foreach (var block in blocks.Data)
        {
            await _dataService.UpdateSingleBlockTagCount(block.BlockName);
        }

        _textFiles.Add(textFile);

        return new Response<ResultCode>(ResultCode.Okay);
    }

    // Only if textfile name or path changes
    public async Task<Response<ResultCode>> RemoveTextFileState(TextFile textFile)
    {
        foreach (var block in textFile.ContainedBlocks)
        {
            var deleteBlock = await _dataService.DeleteBlockState(block.Key);

            if(deleteBlock.Success == false){
                return new Response<ResultCode>(deleteBlock.Exception!, ResultCode.Error);}
        }

        _textFiles.Remove(textFile);

        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> RefreshAllTags()
    {
        // var timer = Stopwatch.StartNew();
        var resetTags = _dataService.ResetTagsData();
        var resetBlockFiles = _dataService.ResetBlockFilesData();
        var resetBlocks = _dataService.ResetBlockData();

        var loadFileTasks = new List<Task<Response<List<Tags>>>>(); // lol <<<<>>>>
        foreach (var file in _textFiles)
        {
            loadFileTasks.Add(Task.Run(() => LoadTextFile(file)));
        }

        var results = await Task.WhenAll(loadFileTasks);
        await resetTags;

        if(resetTags.Result.Success == false)
            return new Response<ResultCode>(resetTags.Result.Exception!, ResultCode.Error);

        foreach (var tagsList in results)
        {
            if (tagsList.Data.Any())
            {
                var insertTags = await _dataService.InsertTags(tagsList.Data);
                if (insertTags.Success == false)
                    return new Response<ResultCode>(insertTags.Exception!, ResultCode.Error);
            }
        }

        await resetBlockFiles;
        if(resetBlockFiles.Result.Success == false)
        {
            return new Response<ResultCode>(resetBlockFiles.Result.Exception!, ResultCode.Error);
        }

        foreach (var file in _textFiles)
        {
            var blockFiles = LoadBlockStates(file);
            if (blockFiles.Data.Any())
            {
                var insertBlockFiles = await _dataService.InsertBlockFiles(blockFiles.Data);
                if (insertBlockFiles.Success == false)
                    return new Response<ResultCode>(insertBlockFiles.Exception!, ResultCode.Error);

            }
        }

        await resetBlocks;
        if(resetBlocks.Result.Success == false)
        {
            return new Response<ResultCode>(resetBlocks.Result.Exception!, ResultCode.Error);
        }

        foreach (var file in _textFiles)
        {
            var blocks = LoadBlocks(file);
            if (blocks.Data.Any())
            {
                var insertBlocks = await _dataService.InsertBlocks(blocks.Data);
                if (insertBlocks.Success == false)
                    return new Response<ResultCode>(insertBlocks.Exception!, ResultCode.Error);
            }

            foreach (var block in blocks.Data)
            {
                await _dataService.UpdateSingleBlockTagCount(block.BlockName);
            }
        }

        // timer.Stop();
        // Console.WriteLine($"Time to refresh all text files: {timer.ElapsedMilliseconds}ms");
        return new Response<ResultCode>(ResultCode.Okay);

    }

    // lets fucking gooo
    private async Task<Response<ResultCode>> LoadAllTextFiles()
    {
        try
        {
            // var timer = Stopwatch.StartNew();
            foreach (var file in _textFiles)
            {
                var tagsList = LoadTextFile(file);
                if (tagsList.Data.Any())
                {
                    var insertTags = await _dataService.InsertTags(tagsList.Data);
                    if (insertTags.Success == false)
                        return new Response<ResultCode>(insertTags.Exception!, ResultCode.Error);
                }

                var blockFiles = LoadBlockStates(file);
                if (blockFiles.Data.Any())
                {
                    var insertBlockFiles = await _dataService.InsertBlockFiles(blockFiles.Data);
                    if (insertBlockFiles.Success == false)
                        return new Response<ResultCode>(insertBlockFiles.Exception!, ResultCode.Error);
                }

                var blocks = LoadBlocks(file);
                if (blocks.Data.Any())
                {
                    var insertBlocks = await _dataService.InsertBlocks(blocks.Data);
                    if (insertBlocks.Success == false)
                        return new Response<ResultCode>(insertBlocks.Exception!, ResultCode.Error);
                }

                foreach (var block in blocks.Data)
                {
                    await _dataService.UpdateSingleBlockTagCount(block.BlockName);
                }
            }

            // timer.Stop();
            // Console.WriteLine($"Time to load all text files: {timer.ElapsedMilliseconds}ms");
            return new Response<ResultCode>(ResultCode.Okay);

        }
        catch (Exception e)
        {
            return new Response<ResultCode>(e.Message, ResultCode.Error);
        }
    }

    private Response<List<Blocks>> LoadBlocks(TextFile textFile)
    {
        var blocks = new List<Blocks>();

        foreach (var pair in textFile.ContainedBlocks)
        {
            var block = new Blocks()
            {
                BlockName = pair.Key,
                BlockFlag = (BlockFlag)pair.Value,
            };
            blocks.Add(block);
        }

        return new Response<List<Blocks>>(blocks);
    }

    private Response<List<BlockFiles>> LoadBlockStates(TextFile textFile)
    {
        var blockFiles = new List<BlockFiles>();

            if (textFile.ContainedBlocks.Any())
            {
                foreach (var block in textFile.ContainedBlocks)
                {
                    var blockFile = new BlockFiles
                    {
                        FilePath = textFile.FilePath,
                        FileName = Path.GetFileName(textFile.FilePath),
                        BlockName = block.Key,
                        BlockFlag = (BlockFlag)block.Value
                    };
                    blockFiles.Add(blockFile);
                }
            }

        return new Response<List<BlockFiles>>(blockFiles);
    }

    private Response<List<Tags>> LoadTextFile(TextFile textfile)
    {
        var tagList = new List<Tags>();
        var path = textfile.FilePath;

        if(string.IsNullOrEmpty(path))
        {
            return new Response<List<Tags>>("Filepath is invalid", ResultCode.InvalidArgument);
        }

        else
        {
            var lines = new List<string> { Capacity = 20000 };
            var text =  File.ReadAllLines(path);
            lines.AddRange(text); // this is actually slightly fast than File.ReadAllLines(path).ToList() for some reason...

            for (int lineNum = 0; lineNum < lines.Count; lineNum++)
            {
                try
                {
                    if (Regex.IsMatch(lines[lineNum], rx_BlockStart))
                    {
                        var flagmatch = Regex.Match(lines[lineNum], rx_FlagMatch);
                        if (flagmatch.Success == false)
                        {
                            var e = new InvalidFlag($"{DataContext(lines, lineNum)}");
                            throw e;
                        }

                        else if (TryMatchFlag(flagmatch.Groups[1].ToString()) == false)
                        {
                            var e = new InvalidFlag($"{flagmatch.Groups[1]} is not a valid flag.{DataContext(lines, lineNum)}");
                            throw e;
                        }

                        else
                        {
                            var flagResponse = new FlagResponse<string>(flagmatch.Groups[1].ToString());
                            var blockFlag = flagResponse.Code;

                            var blockName = Regex.Match(lines[lineNum], rx_BlockName).Groups[1].ToString();

                            if (textfile.ContainedBlocks.Any(b => b.Key == blockName))
                            {
                                textfile.ContainedBlocks.Remove(blockName);
                            }
                            textfile.ContainedBlocks.Add(blockName, blockFlag);

                            lineNum++;

                            int blocklineNum = 1;
                            while (lineNum < lines.Count && Regex.IsMatch(lines[lineNum], rx_BlockStartOrEnd) == false)
                            {
                                var newTag = new TagMaker().MakeTag(lines[lineNum], blocklineNum);
                                if( newTag is null)
                                {
                                    Console.WriteLine($"{lines[lineNum]} is an invalid line");
                                }

                                else
                                {
                                    newTag.BlockName = blockName;
                                    newTag.BlockFlag = (BlockFlag)blockFlag; // hope that works
                                    tagList.Add(newTag);
                                }
                                blocklineNum++;
                                lineNum++;
                            }
                            if (Regex.IsMatch(lines[lineNum], rx_BlockEnd) == false || lineNum == lines.Count)
                            {
                                lineNum--; //negate last skip
                                var e = new UnclosedBlock($"{DataContext(lines, lineNum)}");
                                throw e;
                            }
                        }
                    }
                }
                catch (InvalidFlag e)
                {
                    Console.WriteLine($"{e}");
                }
                catch (UnclosedBlock e)
                {
                    Console.WriteLine($"{e}");
                }
            }
        }

        textfile.IsLoaded = true;
        return new Response<List<Tags>>(tagList);
    }

    private async Task<Response<ResultCode>> SetFilePath(string path)
    {
        var setFilePath = await Task.Run(() =>
        {
            if(string.IsNullOrEmpty(path))
                return new Response<ResultCode>("Invalid File Path",ResultCode.NullItemInput);

            else if (Directory.Exists(path))
            {
                _dataPath.Path = path;
                var files = Directory.GetFiles(path, "*.txt");

                foreach (var file in files)
                {
                    _textFiles.Add(new TextFile() { FilePath = file });
                }

                return new Response<ResultCode>(ResultCode.Okay);
            }

            else
                return new Response<ResultCode>("Filepath not Found",ResultCode.NotFound);

        });

        return setFilePath;
    }

    private static bool TryMatchFlag(string flag)
    {
        var flagmatch = new FlagResponse<string>(flag);
        if(flagmatch.Success == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static string DataContext(List<string> list, int position)
    {
        if ( position > 0 && position < list.Count)
            return $"\nContext:\n Previous Line: {list[position-1]}\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";

        else if ( position == 0)
            return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";

        else
            return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: (EOF)";

    }

    public void WatchFiles()
    {
        var watcher = new FileSystemWatcher()
        {
            Path = _dataPath.Path,
            Filter = ".txt",
            NotifyFilter = NotifyFilters.FileName
                         | NotifyFilters.LastWrite,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
    }

    private static DateTime _lastTimeFileChanged { get; set; }
    private static DateTime _lastTimeFileCreated { get; set; }
    private static DateTime _lastTimeFileRenamed { get; set; }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        //double changed events :)
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        if( e.ChangeType == WatcherChangeTypes.Changed )
        {
            if( DateTime.Now.Subtract (_lastTimeFileChanged).TotalMilliseconds < 100
              | DateTime.Now.Subtract (_lastTimeFileCreated).TotalMilliseconds < 100
              | DateTime.Now.Subtract (_lastTimeFileRenamed).TotalMilliseconds < 100 )
            {
                return;
            }
        }

        _lastTimeFileChanged = DateTime.Now;

        _textFiles.FirstOrDefault(t => t.FilePath == e.FullPath)!.IsLoaded = false;

    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _lastTimeFileCreated = DateTime.Now;

        _textFiles.Add(new TextFile() { FilePath = e.FullPath, IsLoaded = false });
        throw new NotImplementedException();
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if(_textFiles.FirstOrDefault(t => t.FilePath == e.FullPath) is not null)
        {
            _textFiles.RemoveAt(_textFiles.IndexOf(_textFiles.FirstOrDefault(t => t.FilePath == e.FullPath)!));
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if(_textFiles.FirstOrDefault(t => t.FilePath == e.OldFullPath) is not null)
        {
            _textFiles.FirstOrDefault(t => t.FilePath == e.FullPath)!.FilePath = e.FullPath;
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        throw new NotImplementedException();
    }

}