using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code;
using System.Diagnostics;

namespace GenGui_CrystalStar.Services;

public interface IGenGuiDataService
{
    void Init();
    Task<Response<ResultCode>> InsertTags(List<Tags> tags);
    Task<Response<ResultCode>> InsertBlockFiles(List<BlockFiles> blockFiles);
    Task<Response<ResultCode>> InsertBlocks(List<Blocks> blocks);
    Task<Response<ResultCode>> ResetTagsData();
    Task<Response<ResultCode>> ResetBlockFilesData();
    Task<Response<ResultCode>> ResetBlockData();
    Task<Response<ResultCode>> DeleteBlockState(string blockName);
    Task<Response<ResultCode>> DeleteTagBlock(string blockName);
    Task<Response<BlockFiles>> GetBlockFile(string blockName);
    Task<Response<ResultCode>> UpdateSingleBlockTagCount(string blockName);
    Task<Response<ResultCode>> UpdateSingleBlockTagCount(string blockName, int count);
}

public class GenGuiDataService : IGenGuiDataService
{
    private readonly GenGuiContext _database;
    private readonly IGenGuiDatabaseService _databaseService;

    public GenGuiDataService(GenGuiContext database, IGenGuiDatabaseService databaseService)
    {
        _database = database;
        _databaseService = databaseService;
    }

    public void Init()
    {

    }

    public async Task<Response<ResultCode>> UpdateSingleBlockTagCount(string blockName)
    {
        var tagCount = await _database.Tags.CountAsync(x => x.BlockName == blockName);
        if (tagCount == 0)
            return new Response<ResultCode>("Zero tags found, won't update", ResultCode.DataError);

        var block = await _database.Blocks.FirstOrDefaultAsync(x => x.BlockName == blockName) as Blocks;
        if (block is null)
            return new Response<ResultCode>("BlockName found in Tags table but not Blocks table", ResultCode.DataError);

        block.TagCount = tagCount;
        _database.Update(block);

        _database.SaveChanges();
        _database.ChangeTracker.Clear();

        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> UpdateSingleBlockTagCount(string blockName, int count)
    {
        var block = await _database.Blocks.FirstOrDefaultAsync(x => x.BlockName == blockName);
        if (block is null)
            return new Response<ResultCode>("BlockName not found", ResultCode.DataError);

        block.TagCount = count;
        _database.Update(block);

        await _database.SaveChangesAsync();
        _database.ChangeTracker.Clear();

        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<BlockFiles>> GetBlockFile(string blockName)
    {
        var blockFile = await _database.BlockFiles.Where(x => x.BlockName == blockName).FirstOrDefaultAsync();

        if (blockFile == null)
            return new Response<BlockFiles>(ResultCode.NotFound);
        else
            return new Response<BlockFiles>(blockFile);
    }

    public async Task<Response<ResultCode>> DeleteTagBlock(string blockName)
    {
        var errors = new List<string>();

        if (await _database.Tags.Where(x => x.BlockName == blockName).FirstOrDefaultAsync() == null)
            errors.Add($"BlockName {blockName} not found in Tags table");

        // if (await _database.Blocks.Where(x => x.BlockName == blockName).FirstOrDefaultAsync() == null)
        //     errors.Add($"BlockName {blockName} not found in Blocks table");

        if (errors.Any())
            return new Response<ResultCode>(errors, ResultCode.NotFound);

        var tags = await _database.Tags.Where(t => t.BlockName == blockName).ToListAsync();
        _database.Tags.RemoveRange(tags);

        // var block = await _database.Blocks.Where(b => b.BlockName == blockName).FirstOrDefaultAsync();
        // _database.Blocks.Remove(block!);

        await _database.SaveChangesAsync();
        _database.ChangeTracker.Clear();

        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> DeleteBlockState(string blockName)
    {
        var tags = await _database.Tags.Where(t => t.BlockName == blockName).ToListAsync();
        _database.Tags.RemoveRange(tags);

        var block = await _database.Blocks.Where(b => b.BlockName == blockName).FirstOrDefaultAsync();
        _database.Blocks.Remove(block!);

        var blockFiles = await _database.BlockFiles.Where(bf => bf.BlockName == blockName).ToListAsync();
        _database.BlockFiles.RemoveRange(blockFiles);

        await _database.SaveChangesAsync();
        _database.ChangeTracker.Clear();

        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> ResetTagsData()
    {
        await _databaseService.ResetTagsTable();
        // stupid fucking tracker
        _database.ChangeTracker.Clear();
        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> ResetBlockFilesData()
    {
        await _databaseService.ResetBlockFilesTable();
        _database.ChangeTracker.Clear();
        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> ResetBlockData()
    {
        await _databaseService.ResetBlocksTable();
        _database.ChangeTracker.Clear();
        return new Response<ResultCode>(ResultCode.Okay);
    }

    public async Task<Response<ResultCode>> InsertTags(List<Tags> tags)
    {
        if (tags.Any())
        {
            await _database.Tags.AddRangeAsync(tags);
            await _database.SaveChangesAsync();
            _database.ChangeTracker.Clear();
            return new Response<ResultCode>(ResultCode.Okay);
        }

        else
        {
            return new Response<ResultCode>("Failed to insert tags into database", ResultCode.Error);
        }
    }

    public async Task<Response<ResultCode>> InsertBlockFiles(List<BlockFiles> blockFiles)
    {
        if (blockFiles.Any())
        {
            await _database.BlockFiles.AddRangeAsync(blockFiles);
            await _database.SaveChangesAsync();
            _database.ChangeTracker.Clear();

            return new Response<ResultCode>(ResultCode.Okay);
        }

        else
        {
            return new Response<ResultCode>("Failed to insert block files into database", ResultCode.Error);
        }
    }

    public async Task<Response<ResultCode>> InsertBlocks(List<Blocks> blocks)
    {
        if (blocks.Any())
        {
            await _database.Blocks.AddRangeAsync(blocks);
            await _database.SaveChangesAsync();
            _database.ChangeTracker.Clear();

            return new Response<ResultCode>(ResultCode.Okay);
        }

        else
        {
            return new Response<ResultCode>("Failed to insert blocks into database", ResultCode.Error);
        }
    }
}