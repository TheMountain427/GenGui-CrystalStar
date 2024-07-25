using GenGui_CrystalStar;
using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Exceptions;
using GenGui_CrystalStar.Code.DatabaseModels;
using GenGui_CrystalStar.Code.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GenGui_CrystalStar.Code.DatabaseModels.Commands;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenGui_CrystalStar.Code.Services;

public interface IGeneratorService
{
    void Init();
    Task<Response<List<PromptOutput>>> GeneratePrompt(string globalGenSettings, string blockGenSettingsList);
}

public class GeneratorService : IGeneratorService
{
    private readonly IGenGuiDataService _dataService;

    public GeneratorService(IGenGuiDataService dataService)
    {
        _dataService = dataService;
    }

    public void Init()
    {

    }

    public async Task<Response<List<PromptOutput>>> GeneratePrompt(string globalGenSettings, string blockGenSettingsList)
    {
        try
        {
            var blockSettingsList = JsonSerializer.Deserialize<BlockGenSettingsList>(blockGenSettingsList)!;
            var globalSettings = JsonSerializer.Deserialize<GlobalGenerationSettings>(globalGenSettings)!;


            var PromptOutputs = new List<PromptOutput>();

            for (int num = 0; num < globalSettings.OutputCount; num++)
            {
                var blocksToProcess = new List<GeneratorSettingFramework>();
                if (globalSettings.OutputType == OutputType.Positive)
                {
                    foreach (var blkset in blockSettingsList.Where(x => x.BlockFlag == BlockFlag.positive && x.SelectCount > 0))
                    {

                        var blockSettingFramework = await ConstructSettingsFramework(globalSettings, blkset);
                        if (blockSettingFramework.Success == false)
                        {
                            throw new Exception($"Generator could not construct GeneratorSettingFramework for {blkset.BlockName}");
                        }


                        blocksToProcess.Add(blockSettingFramework.Data);
                    }
                }

                else if (globalSettings.OutputType == OutputType.Api)
                {
                    foreach (var blkset in blockSettingsList.Where(x => x.SelectCount > 0))
                    {

                        var blockSettingFramework = await ConstructSettingsFramework(globalSettings, blkset);
                        if (blockSettingFramework.Success == false)
                        {
                            throw new Exception($"Generator could not construct GeneratorSettingFramework for {blkset.BlockName}");
                        }


                        blocksToProcess.Add(blockSettingFramework.Data);
                    }

                }


                var selectTags = await _dataService.GetTags(blocksToProcess);
                if (selectTags.Success == false)
                    throw new Exception("dead af");

                blocksToProcess = selectTags.Data;

                foreach (var blkSet in blocksToProcess)
                {

                    var Random = new Random();
                    if (blkSet.RandomDrop == Enabled.Enabled)
                    { // go backwards so collection does not shift
                        for (int i = blkSet.SelectedLines.Count - 1; i >= 0; i--)
                        {
                            if (Random.Next(0, 101) < blkSet.RandomDropChance)
                            {
                                blkSet.SelectedLines.RemoveAt(i);
                            }
                        }
                    }

                    if (blkSet.SelectedLines.Count == 0)
                        continue;

                    var styledTags = new List<string>();
                    switch (blkSet.TagStyle)
                    {
                        case TagStyle.Clean:
                            styledTags = blkSet.SelectedLines.Select(x => x.CommaTag).ToList()!;
                            break;
                        case TagStyle.Underscore:
                            styledTags = blkSet.SelectedLines.Select(x => x.CommaTagUnderscore).ToList()!;
                            break;

                        case TagStyle.Random:
                            styledTags = blkSet.SelectedLines.Select(x => Random.Next(0, 2) == 0 ? x.CommaTag : x.CommaTagUnderscore).ToList()!;
                            break;
                    }

                    if (globalSettings.ShuffleSetting == GlobalShuffleSetting.WithinBlocks)
                    {
                        styledTags = styledTags.OrderBy(x => Random.Next()).ToList();
                    }

                    if (blkSet.AddAdj == Enabled.Enabled)
                    {
                        throw new NotImplementedException();
                        // for (int i = styledTags.Count - 1; i >= 0; i--)
                        // {
                        //     if (Random.Next(0, 100) < blkSet.AddAdjChance)
                        //     {
                        //         var AdjType = blkSet.AddAdjType;
                        //         blkSet.SelectedLines[i] = string.Concat(GetAdjective(AdjType)," ", styledTags[i]);
                        //     }
                        // }
                    }

                    blkSet.FinalLines.AddRange(styledTags);
                }

                var prompt = "";

                if (globalSettings.OutputType == OutputType.Positive)
                {
                    var Random = new Random();
                    if (globalSettings.ShuffleSetting == GlobalShuffleSetting.WholeBlocks)
                    {
                        blocksToProcess = blocksToProcess.OrderBy(x => Random.Next()).ToList();
                    }

                    var almostDone = new List<string>();
                    foreach (var blkSet in blocksToProcess)
                    {
                        almostDone.AddRange(blkSet.FinalLines);
                    }

                    if (globalSettings.ShuffleSetting == GlobalShuffleSetting.Full)
                    {
                        almostDone = almostDone.OrderBy(x => Random.Next()).ToList();
                    }

                    prompt = string.Join(" ", almostDone);

                    if (globalSettings.TrimLastComma == TrimLastComma.True)
                    {
                        if (prompt.Last() == ',' )
                            prompt = prompt.TrimEnd(',');
                    }
                }

                if (globalSettings.OutputType == OutputType.Api)
                {
                    prompt = new ApiPromptMaker().MakeApiPrompt(blocksToProcess, globalSettings);
                }


                var promptOutput = new PromptOutput
                {
                    NewPrompt= prompt,
                    BlockAttributes = blockGenSettingsList,
                    GenSettings = globalGenSettings
                };


                PromptOutputs.Add(promptOutput);
            }
            await _dataService.InsertPromptHistory(PromptOutputs);
            return new Response<List<PromptOutput>>(PromptOutputs);
        }
        catch (Exception e)
        {
            //throw e;
            Console.WriteLine($"{e.Message}");
            return new Response<List<PromptOutput>>(e.Message, ResultCode.Error);
        }

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

            blockSettingFramework.SelectedLineNumbers = SelectRandomNumbers(blockSettingFramework.TotalTags, blockSettingFramework.SelectCount);

            return new Response<GeneratorSettingFramework>(blockSettingFramework);
        }

        catch (Exception e)
        {
            return new Response<GeneratorSettingFramework>(e.Message, ResultCode.Error);
        }

    }

    private List<int> SelectRandomNumbers(int limitValue, int numToSelect)
    {


        Random random = new Random();
        HashSet<int> selectedIndices = new HashSet<int>();

        if (numToSelect > limitValue)
            numToSelect = limitValue;

        while (selectedIndices.Count < numToSelect)
        {
            int randomIndex = random.Next(1, limitValue + 1);
            selectedIndices.Add(randomIndex);
        }

        var a = selectedIndices.ToList();


        return a;
    }
}


