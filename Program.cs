using GenGui_CrystalStar.Services;
using Microsoft.Extensions.Configuration;
using GenGui_CrystalStar.Code.DatabaseModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.Models;
using GenGui.CrystalStar.Code.Models;
using System.Text.Json;

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
        serviceProvider.AddScoped<IGeneratorService, GeneratorService>();

        // ------------------------
        var srv = serviceProvider.BuildServiceProvider();




        srv.GetService<IGenGuiDatabaseService>()!.Init();
        srv.GetService<IGenGuiDataService>()!.Init();
        srv.GetService<ITextFileSourceService>()!.Init();
        srv.GetService<IGeneratorService>()!.Init();
        // await srv.GetService<ITextFileSourceService>()!.RefreshAllTags();
        // await srv.GetService<IGenGuiDataService>()!.DeleteTagBlock("Cars_Pos");
        // var a = await srv.GetService<IGenGuiDataService>()!.GetBlockFile("Cars_Pos");
        // var b = await srv.GetService<ITextFileSourceService>()!.RefreshTagBlock("Cars_Pos");
        // var a = new TextFile();
        // a.FilePath = @"C:\Users\sbker\OneDrive\Desktop\(WS)-GenGui-CrystalStar\Prompt Collections\Software-Computers-Monitor Resolutions.txt";
        // var b = srv.GetService<ITextFileSourceService>()!.LoadTextFile(a);
        // var h = """{"OutputCount":1,"TrimLastComma":1,"ShuffleSetting":4,"OutputType":1,"GlobalTagStyle":{"IsEnabled":0,"selectionScope":1,"GlobalChance":0},"GlobalRandomDrop":{"IsEnabled":0,"selectionScope":1,"GlobalChance":0},"GlobalAddAdjectives":{"IsEnabled":0,"selectionScope":1,"GlobalAdjtype":0,"GlobalChance":0}}""";
        // srv.GetService<IGeneratorService>()!.TestTheThing(h);

        // var deeznutz = new BlockGenSettingsList();
        // deeznutz.BlockGenSettings.Add(new BlockGenerationSettings());
        // deeznutz.BlockGenSettings.Add(new BlockGenerationSettings());
        // deeznutz.BlockGenSettings.Add(new BlockGenerationSettings());
        // var json = JsonSerializer.Serialize(deeznutz);

        var bbb = JsonSerializer.Deserialize<BlockGenSettingsList>(blockSettingString);
        var ggg = JsonSerializer.Deserialize<BlockGenSettingsList>(globalSettingsString);

        Console.WriteLine("Hello World!");

    }

    public static string globalSettingsString =
        """
        {
            "OutputCount": 1,
            "TrimLastComma": 1,
            "ShuffleSetting": 0,
            "OutputType": 1,
            "GlobalTagStyleSettings": {
                "IsEnabled": 1,
                "SelectionScope": 1,
                "GlobalTagStyle": 1
            },
            "GlobalRandomDropSettings": {
                "IsEnabled": 0,
                "SelectionScope": 1,
                "GlobalRandomDropChance": 0
            },
            "GlobalAddAdjSettings": {
                "IsEnabled": 0,
                "SelectionScope": 1,
                "GlobalAdjType": 0,
                "GlobalAddAdjChance": 0
            }
        }
        """;

    public static string blockSettingString =
        """
        {
            "BlockGenSettings": [
                {
                    "BlockName": "Cars_Pos",
                    "BlockFlag": 1,
                    "SelectCount": 2,
                    "BlockShuffleSetting": 0,
                    "BlockTagStyleSettings": {
                        "IsEnabled": 0,
                        "BlockTagStyle": 1
                    },
                    "BlockRandomDropSettings": {
                        "IsEnabled": 0,
                        "BlockRandomDropChance": 0
                    },
                    "BlockAddAdjSettings": {
                        "IsEnabled": 0,
                        "BlockAdjType": 0,
                        "BlockAddAdjChance": 0
                    }
                },
                {
                    "BlockName": "Dark_Colors_Neg",
                    "BlockFlag": 2,
                    "SelectCount": 3,
                    "BlockShuffleSetting": 0,
                    "BlockTagStyleSettings": {
                        "IsEnabled": 0,
                        "BlockTagStyle": 1
                    },
                    "BlockRandomDropSettings": {
                        "IsEnabled": 0,
                        "BlockRandomDropChance": 0
                    },
                    "BlockAddAdjSettings": {
                        "IsEnabled": 0,
                        "BlockAdjType": 0,
                        "BlockAddAdjChance": 0
                    }
                },
                {
                    "BlockName": "Computer_Models_pos",
                    "BlockFlag": 1,
                    "SelectCount": 7,
                    "BlockShuffleSetting": 0,
                    "BlockTagStyleSettings": {
                        "IsEnabled": 0,
                        "BlockTagStyle": 2
                    },
                    "BlockRandomDropSettings": {
                        "IsEnabled": 0,
                        "BlockRandomDropChance": 0
                    },
                    "BlockAddAdjSettings": {
                        "IsEnabled": 0,
                        "BlockAdjType": 0,
                        "BlockAddAdjChance": 0
                    }
                }
            ]
        }
        """;
}

