using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Exceptions;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace GenGui_CrystalStar.Services;

public class TextFileSourceService
{
    private string _dataPath { get; set; } = "";
    public string DataPath { get => _dataPath; }
    private static List<TextFile> _textFiles { get; set; } = [];
    private static readonly string rx_BlockStart = @"(?i)^\s*\{\s*\w*:.*$";
    private static readonly string rx_FlagMatch = @"(?i)\W*\w*:\s*(\w*)$";
    private static readonly string rx_BlockName = @"(?i)^\W*(\w*)";
    private static readonly string rx_BlockStartOrEnd = @"^\s*\}\s*$|^\s*\{";
    private static readonly string rx_BlockEnd = @"^\s*\}\s*$";

    private GenGuiContext _database { get; set; }




    public TextFileSourceService(string dataPath)
    {
        _database = new GenGuiContext();
        if(Init(dataPath).Result.Success == false)
        {
            throw new Exception("Error initializing TextFileSourceService");
        }

    }

    private async Task<Response<ResultCode>> Init(string dataPath)
    {
        var t_SetFilePath = await SetFilePath(dataPath);

        if(t_SetFilePath.Success == false)
        {
            return new Response<ResultCode>(ResultCode.Error);
        }

        else
        {

            Task t_LoadAllTextFiles = Task.Run(LoadAllTextFiles);
            await t_LoadAllTextFiles;

            if(t_LoadAllTextFiles.IsCompletedSuccessfully == true)
            {
                WatchFiles();
                return new Response<ResultCode>(ResultCode.Okay);
            }

            else
            {
                return new Response<ResultCode>(ResultCode.Failed);
            }
        }
    }

    // lets fucking gooo
    private async Task<Response<ResultCode>> LoadAllTextFiles()
    {
        // Convert each file path into a task of List<Tag>
        var loadFileTasks = new List<Task<List<Tags>>>();
        foreach (var file in _textFiles)
        {
            loadFileTasks.Add(Task.Run(() => LoadTextFile(file)));
            System.Console.WriteLine("loadtextfile tasks");
        }

        // Await the completion of all tasks
        var results = await Task.WhenAll(loadFileTasks);


        // Now, results is an array of List<Tag>, where each List<Tag> corresponds to a file
        foreach (var tagsList in results)
        {
            if (tagsList.Any())
            {
                await _database.AddRangeAsync(tagsList);
            }
        }
        await _database.SaveChangesAsync();
        return new Response<ResultCode>(ResultCode.Okay);
    }

    private List<Tags> LoadTextFile(TextFile textfile)
    {
        System.Console.WriteLine("loadtextfile");
        var tagList = new List<Tags>();
        var path = textfile.FilePath;

        if(string.IsNullOrEmpty(path))
        {
            return null!;
        }

        else
        {
            var lines = new List<string> { Capacity = 20000 };
            var text =  File.ReadAllLines(path);

            foreach (var line in text)
            {
                lines.Add(line);
            }

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
                            lineNum++;

                            int blocklineNum = 1;
                            while (lineNum < lines.Count && Regex.IsMatch(lines[lineNum], rx_BlockStartOrEnd) == false)
                            {
                                var newTag = new TagMaker().MakeTag(lines[lineNum], blocklineNum);
                                if( newTag == null)
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
        return tagList;
    }

    private async Task<Response<ResultCode>> SetFilePath(string path)
    {
        var t_SetFilePath = await Task.Run(() =>
        {
            if(string.IsNullOrEmpty(path))
            {
                return new Response<ResultCode>(ResultCode.NullItemInput);
            }

            else if (Directory.Exists(path))
            {
                _dataPath = path;
                var files = Directory.GetFiles(path, "*.txt");

                foreach (var file in files)
                {
                    _textFiles.Add(new TextFile() { FilePath = file });
                }

                return new Response<ResultCode>(ResultCode.Okay);
            }

            else
            {
                return new Response<ResultCode>(ResultCode.NotFound);
            }
        });

        return t_SetFilePath;
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
        {
            return $"\nContext:\n Previous Line: {list[position-1]}\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";
        }
        else if ( position == 0)
        {
            return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";
        }
        else
        {
            return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: (EOF)";
        }
    }

    public void WatchFiles()
    {
        var watcher = new FileSystemWatcher()
        {
            Path = _dataPath,
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
        if(_textFiles.FirstOrDefault(t => t.FilePath == e.FullPath) != null)
        {
            _textFiles.RemoveAt(_textFiles.IndexOf(_textFiles.FirstOrDefault(t => t.FilePath == e.FullPath)!));
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if(_textFiles.FirstOrDefault(t => t.FilePath == e.OldFullPath) != null)
        {
            _textFiles.FirstOrDefault(t => t.FilePath == e.FullPath)!.FilePath = e.FullPath;
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        throw new NotImplementedException();
    }

}