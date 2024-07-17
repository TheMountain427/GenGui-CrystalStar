

using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using SQLite;

namespace GenGui_CrystalStar.Services;

public interface IGenGuiDatabaseService
{
    void Init();
    Task ClearTransientTables();
    Task ClearStatefulTables();


    Task RefreshTransientTables(List<Tags> Tags);
    // void RefreshBlocksTable();
    // void RefreshGenerationSettingsTable();
}

public class GenGuiDataBaseService : IGenGuiDatabaseService
{
    private GenGuiContext _dbContext;
    private SQLiteAsyncConnection _db;

    public GenGuiDataBaseService()
    {
        _dbContext = new GenGuiContext();
        _db = new SQLiteAsyncConnection(_dbContext.DatabasePath);
        Init();
    }

    public void Init()
    {
        Task.Run( async () =>
        {
            await CreateNewTransientTables();
            await CreateNewStatefulTables();
        });
    }

    private async Task CreateNewTransientTables()
    {
        var dropTransientTables = new List<Task>
        {
            _db.DropTableAsync<Tags>(),
            _db.DropTableAsync<Blocks>()
        };

        await Task.WhenAll(dropTransientTables);

        var createTransientTables = new List<Task>
        {
            _db.CreateTableAsync<Tags>(),
            _db.CreateTableAsync<Blocks>()
        };

        await Task.WhenAll(createTransientTables);
    }

    private async Task CreateNewStatefulTables()
    {
        var createStatefulTables = new List<Task>
        {
            _db.CreateTableAsync<GenerationSettings>()
        };

        await Task.WhenAll(createStatefulTables);
    }

    public async Task ClearTransientTables()
    {
        await _db.DeleteAllAsync<Tags>();
        await _db.DeleteAllAsync<Blocks>();
    }

    public async Task ClearStatefulTables()
    {
        await _db.DeleteAllAsync<GenerationSettings>();
    }

    // ????
    public async Task RefreshTransientTables(List<Tags> Tags)
    {
        await ClearTransientTables();
        throw new NotImplementedException();
    }

}