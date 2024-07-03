using Microsoft.Data.Sqlite;
using GenGui_CrystalStar.Services;
using SQLitePCL;

namespace GenGui_CrystalStar;

public class Program
{
    public static async Task Main()
    {
        Task<TagBlocks> GetLines = GetData.GetLinesFromTxtFiles();

        var dbConnection = await DB.NewSqliteConnection();

        DB.ClearDatabase(dbConnection);

        var blocks = await GetLines;
        
        var tasks = new List<Task>();
            foreach (var key in blocks.Keys)
            {
                DB.CreateTagTable(dbConnection, key); // Assuming this is synchronous and necessary before inserting tags
                var taglist = blocks[key].Tags; // Direct access to avoid FirstOrDefault
                var task = DB.InsertTagsAsync(dbConnection, key, taglist);
                tasks.Add(task);
            }
        await Task.WhenAll(tasks);

        DB.InsertBlockData(dbConnection, blocks);
    }
}
