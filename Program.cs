using GenGui_CrystalStar.Services;
using Microsoft.Extensions.Configuration;
using GenGui_CrystalStar.Code.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace GenGui_CrystalStar;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder();
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
        string datapath = config.GetValue<string>("DataSource:Path")!;


        var serviceProvider = new ServiceCollection()
            .AddDbContext<GenGuiContext>()
            .AddScoped<IGenGuiDatabaseService, GenGuiDataBaseService>()
            // .AddScoped<ITextFileSourceService, TextFileSourceService>()
            .AddScoped<IGenGuiDataService, GenGuiDataService>();

        var srv = serviceProvider.BuildServiceProvider();


        var a = new List<Tags>()
        {
            new Tags()
            {
                LineNumber = 1,
                Line = "Line",
                CommaTag = "CommaTag",
                CleanTag = "CleanTag",
                CleanTagUnderscore = "CleanTagUnderscore",
                CommaTagUnderscore = "CommaTagUnderscore",
                IsMultiTag = true,
                BlockName = "BlockName",
                BlockFlag = BlockFlag.cfg_scale
            }
        };
        srv.GetService<IGenGuiDatabaseService>().InitAsync();
        await srv.GetService<IGenGuiDataService>().InsertTags(a);
        Console.WriteLine("Hello World!");
    }
}

