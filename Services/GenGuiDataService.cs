using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code;

namespace GenGui_CrystalStar.Services;

public interface IGenGuiDataService
{
    Task<Response<ResultCode>> InsertTags(List<Tags> tags);
}

public class GenGuiDataService : IGenGuiDataService
{
    private readonly GenGuiContext _database;

    public GenGuiDataService(GenGuiContext database)
    {
        _database = database;
    }

    public static Task<Response<ResultCode>> InsertTag(Tags tag)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<ResultCode>> InsertTags(List<Tags> tags)
    {
        if (tags.Any())
        {
            await _database.Tags.AddRangeAsync(tags);
            await _database.SaveChangesAsync();
            return new Response<ResultCode>(ResultCode.Okay);
        }

        else
        {
            return new Response<ResultCode>(ResultCode.Error);
        }

    }

    public static Task<Response<ResultCode>> InsertBlock(Blocks block)
    {
        throw new NotImplementedException();
    }

    public static Task<Response<ResultCode>> InsertBlocks(List<Blocks> blocks)
    {
        throw new NotImplementedException();
    }

    public static Task<Response<ResultCode>> InsertGenerationSetting(GenerationSettings setting)
    {
        throw new NotImplementedException();
    }

    public static Task<Response<ResultCode>> InsertGenerationSettings(List<GenerationSettings> settings)
    {
        throw new NotImplementedException();
    }
}