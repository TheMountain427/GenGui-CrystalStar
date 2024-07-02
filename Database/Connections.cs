using System.Data.SqlTypes;
using Microsoft.Data.Sqlite;
using GenGui_CrystalStar.Services;
using SQLitePCL;

namespace GenGui_CrystalStar;

public static class DB
{
    public static async Task<SqliteConnection> NewSqliteConnection()
    {
        var connection = new SqliteConnection(@"Data Source = C:\Users\sbker\OneDrive\Desktop\(WS)-GenGui-CrystalStar\GenGui-CrystalStar\GenGui.db");
        await connection.OpenAsync();
        return connection;
    }

    public static async void CreateTable(SqliteConnection db, string item)
    {
        var command = db.CreateCommand();
        command.CommandText = @$"
                                CREATE TABLE {item} (
                                    line INTEGER NOT NULL PRIMARY KEY,
                                    tag TEXT NOT NULL
                                );";
        await command.ExecuteNonQueryAsync();
    }

    public static void ClearDatabase(SqliteConnection db)
    {
        var command = db.CreateCommand();
        command.CommandText = @"
                        PRAGMA writable_schema = 1;
                        DELETE FROM sqlite_master;
                        PRAGMA writable_schema = 0;
                        VACUUM;
                        PRAGMA integrity_check;";
        command.ExecuteNonQuery();
    }

    public static void InsertTags(SqliteConnection db, string table, List<string> Tags)
    {
        // single transaction so its not slow as balls
        using (var trans = db.BeginTransaction())
        {
            using (var command = db.CreateCommand())
            {
                int i = 1; foreach (var tag in Tags)
            {
                command.CommandText = @$"
                        INSERT INTO {table} 
                        VALUES ({i}, '{tag}');";
                command.ExecuteNonQuery();
                i++;
            }
        }
        trans.Commit();
    }
}
}