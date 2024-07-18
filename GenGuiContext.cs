using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code.DatabaseModels.Configurations;

namespace GenGui_CrystalStar;

public class GenGuiContext : DbContext
{
    public DbSet<sqlite_schema> sqlite_schema { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Blocks> Blocks { get; set; }
    public DbSet<GenerationSettings> GenerationSettings { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DatabasePath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new sqlite_schemaConfig());
            modelBuilder.ApplyConfiguration(new TagConfig());
            modelBuilder.ApplyConfiguration(new BlocksConfig());
            modelBuilder.ApplyConfiguration(new GenerationSettingsConfig());
        }
}