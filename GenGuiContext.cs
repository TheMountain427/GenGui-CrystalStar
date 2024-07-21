using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code.DatabaseModels.Configurations;
using Microsoft.EntityFrameworkCore.Internal;

namespace GenGui_CrystalStar;


public class GenGuiContext : DbContext
{
    public DbSet<sqlite_schema> sqlite_schema { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Blocks> Blocks { get; set; }
    public DbSet<PastGenerationSettings> PastGenerationSettings { get; set; }
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
        var path = Environment.CurrentDirectory;
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
            modelBuilder.ApplyConfiguration(new PastGenerationSettingsConfig());
            modelBuilder.ApplyConfiguration(new BlockFilesConfig());
            modelBuilder.ApplyConfiguration(new PromptHistoryConfig());

            base.OnModelCreating(modelBuilder);
        }
}