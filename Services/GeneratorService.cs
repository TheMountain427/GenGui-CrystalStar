using GenGui_CrystalStar;
using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Exceptions;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GenGui_CrystalStar.Code.DatabaseModels.Commands;

namespace GenGui_CrystalStar.Services;

public interface IGeneratorService
{
    void Init();
    void TestTheThing(string a);
    void GeneratePrompt(string blockGenSettingsList, string globalGenSettings);
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



    public async void /* Task<List<PromptOutput> */ GeneratePrompt(string globalGenSettings, string blockGenSettingsList)
    {
        try
        {
            var blockSettingsList = JsonSerializer.Deserialize<BlockGenSettingsList>(blockGenSettingsList)!;
            var globalSettings = JsonSerializer.Deserialize<GlobalGenerationSettings>(globalGenSettings)!;

            var blocksToProcess = new List<GeneratorSettingFramework>();
            if (globalSettings.OutputType == OutputType.Positive)
            {
                foreach (var blkset in blockSettingsList.BlockGenSettings.Where(x => x.BlockFlag == BlockFlag.positive && x.SelectCount > 0))
                {

                    var blockSettingFramework = await ConstructSettingsFramework(globalSettings, blkset);
                    if (blockSettingFramework.Success == false)
                    {
                        throw new Exception($"Generator could not construct GeneratorSettingFramework for {blkset.BlockName}");
                    }


                    blocksToProcess.Add(blockSettingFramework.Data);
                }
            }

            var selectTags = await SelectTagsSingleQuery.SelectTagsWithJoinAsync(_db, blocksToProcess);
            if (selectTags.Success == false)
                throw new Exception("dead af");

            blocksToProcess = selectTags.Data;




            Console.WriteLine("skinny penis");

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
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }

    }

    private double StandardDeviation(IEnumerable<double> values)
    {
        double avg = values.Average();
        return Math.Sqrt(values.Average(v=>Math.Pow(v-avg,2)));
    }

    private async Task<Response<GeneratorSettingFramework>> ConstructSettingsFramework(GlobalGenerationSettings globalSettings, BlockGenerationSettings blkset)
    {   // I am sorry for what I just did, time for bed....
        try
        {
            var blockSettingFramework = new GeneratorSettingFramework
            {
                BlockName = blkset.BlockName,
                BlockFlag = blkset.BlockFlag,
                SelectCount = blkset.SelectCount,
                TotalTags = (await _dataService.GetTotalTagCount(blkset.BlockName)).Data,

                TagStyle = globalSettings.GlobalTagStyleSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalTagStyleSettings.SelectionScope == SelectionScope.Global ?
                                    globalSettings.GlobalTagStyleSettings.GlobalTagStyle : blkset.BlockTagStyleSettings.IsEnabled == Enabled.Enabled ?
                                        blkset.BlockTagStyleSettings.BlockTagStyle : globalSettings.GlobalTagStyleSettings.GlobalTagStyle,

                BlockShuffle = globalSettings.ShuffleSetting != GlobalShuffleSetting.None ?
                                    Enabled.Disabled : blkset.BlockShuffleSetting,

                RandomDrop = globalSettings.GlobalRandomDropSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalTagStyleSettings.SelectionScope == SelectionScope.Global ?
                                    Enabled.Enabled : blkset.BlockRandomDropSettings.IsEnabled,

                RandomDropChance = globalSettings.GlobalRandomDropSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalRandomDropSettings.SelectionScope == SelectionScope.Global ?
                                    globalSettings.GlobalRandomDropSettings.GlobalRandomDropChance : blkset.BlockRandomDropSettings.BlockRandomDropChance,

                AddAdj = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                    Enabled.Enabled : blkset.BlockAddAdjSettings.IsEnabled,

                AddAdjChance = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                    globalSettings.GlobalAddAdjSettings.GlobalAddAdjChance : blkset.BlockAddAdjSettings.BlockAddAdjChance,

                AddAdjType = globalSettings.GlobalAddAdjSettings.IsEnabled == Enabled.Enabled && globalSettings.GlobalAddAdjSettings.SelectionScope == SelectionScope.Global ?
                                    globalSettings.GlobalAddAdjSettings.GlobalAdjType : blkset.BlockAddAdjSettings.BlockAdjType
            };

            blockSettingFramework.SelectedLineNumbers = SelectRandomLineNumbers(blockSettingFramework.TotalTags, blockSettingFramework.SelectCount);

            return new Response<GeneratorSettingFramework>(blockSettingFramework);
        }

        catch (Exception e)
        {
            return new Response<GeneratorSettingFramework>(e.Message, ResultCode.Error);
        }

    }

    private List<int> SelectRandomLineNumbers(int totalTags, int selectCount)
    {


        Random random = new Random();
        HashSet<int> selectedIndices = new HashSet<int>();

        if (selectCount > totalTags)
            selectCount = totalTags;

        while (selectedIndices.Count < selectCount)
        {
            int randomIndex = random.Next(1, totalTags + 1);
            selectedIndices.Add(randomIndex);
        }

        var a = selectedIndices.ToList();


        return a;
    }
}


