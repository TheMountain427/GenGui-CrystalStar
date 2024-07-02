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
        foreach (var key in blocks.Keys)
        {
            DB.CreateTable(dbConnection, key);
            // oh god what have I done
            var taglist = blocks.FirstOrDefault(m => m.Key == key).Value.FirstOrDefault(m => m.Key == "Tags").Value as List<string>;
            DB.InsertTags(dbConnection, key, taglist);
            // I mean it works, but its slow as balls. like 1 min slow
            // nvm fixed it
        }
    }
}
