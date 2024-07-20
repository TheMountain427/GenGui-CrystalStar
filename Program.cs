using GenGui_CrystalStar.Services;
using Microsoft.Extensions.Configuration;
using GenGui_CrystalStar.Code.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.Models;
using GenGui.CrystalStar.Code.Models;

namespace GenGui_CrystalStar;

class Program
{
    static async Task Main(string[] args)
    {
        // ------------------------
        var DataSourceSettings = new DataSourceSettings();
        var TextFileSourceSettings = new TextFileSourceSettings();

        // ------------------------
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

        // ------------------------
        config.GetSection("DataSourceSettings").Bind(DataSourceSettings);
        config.GetSection("TextFileSourceSettings").Bind(TextFileSourceSettings);


        // ------------------------
        var serviceProvider = new ServiceCollection();

        // ------------------------
        serviceProvider.AddDbContext<GenGuiContext>();

        // ------------------------
        serviceProvider.AddSingleton<IDataSourceSettings>(DataSourceSettings);
        serviceProvider.AddSingleton<ITextFileSourceSettings>(TextFileSourceSettings);

        // ------------------------
        serviceProvider.AddScoped<GenGuiContext>();
        serviceProvider.AddScoped<IGenGuiDatabaseService, GenGuiDataBaseService>();
        serviceProvider.AddScoped<ITextFileSourceService, TextFileSourceService>();
        serviceProvider.AddScoped<IGenGuiDataService, GenGuiDataService>();

        // ------------------------
        var srv = serviceProvider.BuildServiceProvider();




        srv.GetService<IGenGuiDatabaseService>()!.Init();
        srv.GetService<IGenGuiDataService>()!.Init();
        srv.GetService<ITextFileSourceService>()!.Init();
        // await srv.GetService<ITextFileSourceService>()!.RefreshAllTags();
        // await srv.GetService<IGenGuiDataService>()!.DeleteTagBlock("Cars_Pos");
        // var a = await srv.GetService<IGenGuiDataService>()!.GetBlockFile("Cars_Pos");
        var b = await srv.GetService<ITextFileSourceService>()!.RefreshTagBlock("Cars_Pos");
        Console.WriteLine("Hello World!");

    }
}

