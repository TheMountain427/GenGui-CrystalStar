using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code.DatabaseModels.Configurations;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GenGui_CrystalStar.Code;


public class GenGuiContext : DbContext
{
    public DbSet<sqlite_schema> sqlite_schema { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Blocks> Blocks { get; set; }
    public DbSet<BlockFiles> BlockFiles { get; set; }
    public DbSet<PromptHistory> PromptHistory { get; set; }

    public string DatabasePath { get; }

    public GenGuiContext()
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);

        var path = Environment.CurrentDirectory;
        DatabasePath = System.IO.Path.Join(path, "GenGui.db");
    }

    public GenGuiContext(DbContextOptions<GenGuiContext> options)
        : base(options)
    {
        //var path = Environment.CurrentDirectory;

        // fucking retarded visual studio bug with ava previewer, hard code path for testing
        var path = @"C:\Users\sbker\OneDrive\Desktop\(WS)-GenGui-CrystalStar\GenGui-CrystalStar\GenGui_CrystalStar\GenGui_CrystalStar.Desktop\bin\Debug\net8.0\";


        DatabasePath = System.IO.Path.Join(path, "GenGui.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        // optionsBuilder.LogTo(Console.WriteLine);
        // optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new sqlite_schemaConfig());
            modelBuilder.ApplyConfiguration(new TagConfig());
            modelBuilder.ApplyConfiguration(new BlocksConfig());
            modelBuilder.ApplyConfiguration(new BlockFilesConfig());
            modelBuilder.ApplyConfiguration(new PromptHistoryConfig());

            base.OnModelCreating(modelBuilder);
        }
}