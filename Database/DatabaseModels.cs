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

    public static async void CreateTagTable(SqliteConnection db, string item)
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

    public static async Task InsertTagsAsync(SqliteConnection db, string table, List<string> Tags)
    {
        // single transaction so its not slow
        using (var trans = await db.BeginTransactionAsync())
        {
            using (var command = db.CreateCommand())
            {
                int i = 1; 
                foreach (var tag in Tags)
                {
                    command.CommandText = @$"
                           INSERT INTO {table} 
                           VALUES ({i}, '{tag}');";
                    await command.ExecuteNonQueryAsync();
                    i++;
                }
            }
            await trans.CommitAsync();
        }
    }

    public static void InsertBlockData(SqliteConnection db, TagBlocks blocks)
    {
        using (var trans = db.BeginTransaction())
        {
            // check if table BlockData exists
            var command = db.CreateCommand();
            command.CommandText = @"
                                SELECT name 
                                    FROM sqlite_master 
                                    WHERE type='table' 
                                        AND name='BlockData';";
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                // create table if it doesn't exist
                command.CommandText = @"
                                CREATE TABLE BlockData (
                                    BlockName TEXT NOT NULL PRIMARY KEY,
                                    BlockFlag TEXT NOT NULL,
                                    SelectCount INTEGER NOT NULL
                                );";
                command.ExecuteNonQuery();
            }
            reader.Close();

            // insert data
            foreach (var block in blocks)
            {
                command.CommandText = @$"
                                INSERT INTO BlockData
                                VALUES ('{block.Value.BlockName}', '{block.Value.BlockFlag}', {block.Value.SelectCount});";
                command.ExecuteNonQuery();
            }

            trans.Commit();
        }
    }

}