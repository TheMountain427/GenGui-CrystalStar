using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using SQLite;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenGui_CrystalStar.Code.Services;

public interface IGenGuiDatabaseService
{
    void Init();
    Task ClearTransientTables();
    Task ClearStatefulTables();
    Task ResetTagsTable();
    Task ResetBlocksTable();
    Task ResetBlockFilesTable();

    // Task RefreshTransientTables(List<Tags> Tags);
    // void RefreshBlocksTable();
    // void RefreshGenerationSettingsTable();
}

public class GenGuiDataBaseService : IGenGuiDatabaseService
{
    private readonly GenGuiContext _database;
    private SQLiteAsyncConnection _db;


    public GenGuiDataBaseService(GenGuiContext database)
    {
        _database = database;
        _db = new SQLiteAsyncConnection(_database.DatabasePath);
        Init();

        // Console.WriteLine("Database Initialized");
    }


    public void Init()
    {
        CreateNewTransientTables();
        CreateNewStatefulTables();
    }

    private void CreateNewTransientTables()
    {
        var t = Task.Run(async () =>
            {
                await _db.DropTableAsync<Tags>();
                //await _db.DropTableAsync<Blocks>();
                await _db.DropTableAsync<BlockFiles>();
            });
        // Program finishes before db is created lol
        // Do this and ignore warning or do Thread.Sleep(1000); in main
        Task.WaitAll(t);

        t = Task.Run(async () =>
            {
                await _db.CreateTableAsync<Tags>();
                //await _db.CreateTableAsync<Blocks>();
                await _db.CreateTableAsync<BlockFiles>();

            });

        Task.WaitAll(t);
    }

    private void CreateNewStatefulTables()
    {
        var t = Task.Run(async () =>
        {
            await _db.CreateTableAsync<PromptHistory>();
            await _db.CreateTableAsync<Blocks>();
        });

        Task.WaitAll(t);
    }

    public async Task ClearTransientTables()
    {
        await _db.DeleteAllAsync<Tags>();
        await _db.DeleteAllAsync<Blocks>();
        await _db.DeleteAllAsync<BlockFiles>();
    }

    public async Task ClearStatefulTables()
    {
        await _db.DeleteAllAsync<PromptHistory>();
    }

    public async Task ResetTagsTable()
    {

        await _db.DropTableAsync<Tags>();
        await _db.CreateTableAsync<Tags>();

    }

    public async Task ResetBlocksTable()
    {
        await _db.DropTableAsync<Blocks>();
        await _db.CreateTableAsync<Blocks>();
    }

    public async Task ResetBlockFilesTable()
    {
        await _db.DropTableAsync<BlockFiles>();
        await _db.CreateTableAsync<BlockFiles>();
    }

}