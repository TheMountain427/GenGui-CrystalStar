using GenGui_CrystalStar;
using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Exceptions;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Services;
using System.Text.Json;

namespace GenGui_CrystalStar.Services;

public interface IGeneratorService
{
    void Init();
    void TestTheThing(string a);
}

public class GeneratorService : IGeneratorService
{
    private readonly IGenGuiDataService _dataService;
    private readonly GenGuiContext _db;

    public GeneratorService(IGenGuiDataService dataService, GenGuiContext db)
    {
        _dataService = dataService;
        _db = db;
    }

    public void Init()
    {

    }

    public void TestTheThing(string a)
    {
        // var tag = _db.Tags.FirstOrDefault(x => x.ID == 1);

        GlobalGenerationSettings? jsonString = JsonSerializer.Deserialize<GlobalGenerationSettings>(a);

        Console.WriteLine($"{jsonString}");
        Console.WriteLine("Deez Nutz");
    }



    public async void /* Task<List<PromptOutput> */ GeneratePrompt(string blockGenSettingsList, string globalGenSettings)
    {
        var blockSettingsList = JsonSerializer.Deserialize<BlockGenSettingsList>(blockGenSettingsList)!;

        var globalSettings = JsonSerializer.Deserialize<GlobalGenerationSettings>(globalGenSettings)!;

        var deeznutz = new List<PromptFacilitator>();
        if (globalSettings.OutputType == OutputType.Positive)
        {

            foreach (var blockSetting in blockSettingsList.BlockGenSettings.Where(x => x.BlockFlag == BlockFlag.positive && x.SelectCount > 0))
            {
                var blockThing = new PromptFacilitator{
                    BlockName = blockSetting.BlockName,
                    BlockFlag = blockSetting.BlockFlag,
                    SelectCount = blockSetting.SelectCount,
                    TotalTags = (await _dataService.GetTotalTagCount(blockSetting.BlockName)).Data,

                    TagStyle = globalSettings.GlobalTagStyleSettings.IsEnabled == Enabled.Enabled  // what the fuck is this, what have I done
                               && globalSettings.GlobalTagStyleSettings.SelectionScope == SelectionScope.Global ?
                                  globalSettings.GlobalTagStyleSettings.GlobalTagStyle :
                                  blockSetting.BlockTagStyleSettings.IsEnabled == Enabled.Enabled ?
                                      blockSetting.BlockTagStyleSettings.BlockTagStyle : globalSettings.GlobalTagStyleSettings.GlobalTagStyle,

                    BlockShuffle = globalSettings.ShuffleSetting != GlobalShuffleSetting.None ?
                                   Enabled.Disabled : blockSetting.BlockShuffleSetting,

                    RandomDrop = globalSettings.GlobalRandomDropSettings.IsEnabled == Enabled.Enabled
                               && globalSettings.GlobalTagStyleSettings.SelectionScope == SelectionScope.Global ?
                                    Enabled.Enabled : blockSetting.BlockRandomDropSettings.IsEnabled,

                    RandomDropChance = globalSettings.GlobalRandomDropSettings.IsEnabled == Enabled.Enabled
                                       && globalSettings.GlobalRandomDropSettings.SelectionScope == SelectionScope.Global ?
                                            globalSettings.GlobalRandomDropSettings.GlobalRandomDropChance : blockSetting.BlockRandomDropSettings.BlockRandomDropChance,

                    AddAdj = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled
                             && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                Enabled.Enabled : blockSetting.BlockAddAdjSettings.IsEnabled,

                    AddAdjChance = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled
                                       && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                            globalSettings.GlobalAddAdjSettings.GlobalAddAdjChance : blockSetting.BlockAddAdjSettings.BlockAddAdjChance,

                    AddAdjType = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled
                                 && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                    globalSettings.GlobalAddAdjSettings.GlobalAdjType : blockSetting.BlockAddAdjSettings.BlockAdjType
                };
                // I am sorry for what I just did, time for bed....
            }

            var RandomGenerator = new Random();
        }

        // parse selected blocks and select count
        // get block tag counts, choose random lines, start db search for those objects

        // parse inputed block settings json
        // parse inputed GenerationSettings
        // generate final block settings

        // await the returned tag objects
        // check global setting state
        // apply per block setting









        //

    }
}
public class PromptFacilitator
{
    public required string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
    public int SelectCount { get; set; }
    public int TotalTags { get; set; }
    public List<int> SelectedLineNumber { get; set; } = [];
    public List<string> SelectedLines { get; set; } = [];
    public TagStyle TagStyle { get; set; } = TagStyle.Clean;
    public Enabled BlockShuffle { get; set; } = Enabled.Disabled;
    public Enabled RandomDrop { get; set;} = Enabled.Disabled;
    public int RandomDropChance { get; set; } = 0;
    public Enabled AddAdj { get; set; } = Enabled.Disabled;
    public int AddAdjChance { get; set; } = 0;
    public AdjType AddAdjType { get; set; } = 0;
}