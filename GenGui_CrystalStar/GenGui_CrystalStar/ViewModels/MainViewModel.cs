using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using GenGui_CrystalStar.ViewModels;
using GenGui_CrystalStar.Views;
using Avalonia;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Services;
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Avalonia.Styling;
using System.Threading.Tasks;
using GenGui_CrystalStar.Code.DatabaseModels;
using System.Data;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using GenGui_CrystalStar.Code;

namespace GenGui_CrystalStar.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    private readonly IGenGuiDatabaseService _databaseService;
    private readonly IGenGuiDataService _dataService;
    private readonly ITextFileSourceService _textFileSourceService;
    private readonly IGeneratorService _generatorService;

    public MainViewModel(IGenGuiDatabaseService databaseService, IGenGuiDataService dataService, ITextFileSourceService textFileSourceService, IGeneratorService generatorService)
    {
        _databaseService = databaseService;
        _dataService = dataService;
        _textFileSourceService = textFileSourceService;
        _generatorService = generatorService;


        _databaseService.Init();
        _databaseService.Init();
        _textFileSourceService.Init();

        LoadBlockContainers(); // this is hilarious that this works
        _ = Task.Run( () => GetInitialPromptHistory());
        _ = Task.Run( () => GetLastGenSettings());
        //LoadBlocks();
    }

    private async Task GetInitialPromptHistory()
    {
        PromptHistory = (await _dataService.GetPromptHistory()).Data;
    }

    [ObservableProperty]
    private int _promptCount = 1;

    partial void OnPromptCountChanged(int value)
    {
        Debug.WriteLine($"PromptCount changed to {value}");
    }

    [ObservableProperty]
    private bool _autoCopy;
    partial void OnAutoCopyChanged(bool value)
    {
        Debug.WriteLine($"AutoCopy changed to {value}");
    }

    public async void RefreshBlocks()
    {
        await _textFileSourceService.RefreshAllTags();
    }

    public void ResetDatabase()
    {

    }

    public async void CopyOutput()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            await window.Clipboard!.SetTextAsync(PromptOutput);

        }
    }

    public async Task GetLastGenSettings()
    {
        var jsonLastGen = await _dataService.GetLastGenSettings();

        var lastGen = JsonSerializer.Deserialize<GlobalGenerationSettings>(jsonLastGen.Exception); //Response<string> makes an exception lol

        PromptCount = lastGen.OutputCount;
        TrimLastCommaToggle = lastGen.TrimLastComma == TrimLastComma.True ? true : false;
        SelectedGlobalShuffleOption = lastGen.ShuffleSetting;
        SelectedPromptOutputType = lastGen.OutputType;
        GlobalTagStyleEnabledOption = lastGen.GlobalTagStyleSettings.IsEnabled ==  Enabled.Enabled ? true : false;
        GlobalTagStyleSelectionScopeOption = lastGen.GlobalTagStyleSettings.SelectionScope;
        SelectedGlobalTagStyleOption = lastGen.GlobalTagStyleSettings.GlobalTagStyle;
        GlobalRandomDropEnabledOption = lastGen.GlobalRandomDropSettings.IsEnabled ==  Enabled.Enabled ? true : false;
        GlobalRandomDropSelectionScopeOption = lastGen.GlobalRandomDropSettings.SelectionScope;
        GlobalRandomDropChance = lastGen.GlobalRandomDropSettings.GlobalRandomDropChance;
    }

    public async void Generate()
    {
        var globalSettings = new GlobalGenerationSettings
        {
            OutputCount = PromptCount,
            TrimLastComma = TrimLastCommaToggle ? TrimLastComma.True : TrimLastComma.False,
            ShuffleSetting = SelectedGlobalShuffleOption,
            OutputType = SelectedPromptOutputType,
            GlobalTagStyleSettings =                          
            {
                IsEnabled = GlobalTagStyleEnabledOption ? Enabled.Enabled : Enabled.Disabled,
                SelectionScope = GlobalTagStyleSelectionScopeOption,
                GlobalTagStyle = SelectedGlobalTagStyleOption
            },
            GlobalRandomDropSettings =
            {
                IsEnabled = GlobalRandomDropEnabledOption ? Enabled.Enabled : Enabled.Disabled,
                SelectionScope = GlobalRandomDropSelectionScopeOption,
                GlobalRandomDropChance = (int)GlobalRandomDropChance
            },
            GlobalAddAdjSettings =
            {
                IsEnabled = Enabled.Disabled,             //GlobalAddAdjTypeEnabledOption      // add in code below
                SelectionScope = SelectionScope.Global,   //GlobalAddAdjTypeSelectionScopeOption
                GlobalAdjType = AdjType.All,              //SelectedGlobalAddAdjOption
                GlobalAddAdjChance = 0                    //GlobalAddAdjChance              // To here
            }
        };

        var blockSettingsList = new BlockGenSettingsList();

        foreach (var shittyBlockThing in BlocksContainers)
        {   // it just works
            blockSettingsList.AddRange(GenerateBlockSettings(shittyBlockThing.GuiBlocks));
        }


        var jsonGlobalSettings = JsonSerializer.Serialize(globalSettings);
        var jsonBlockSettings = JsonSerializer.Serialize(blockSettingsList);

        var promptList = await _generatorService.GeneratePrompt(jsonGlobalSettings, jsonBlockSettings);

        PromptHistory = (await _dataService.GetPromptHistory()).Data;

        string prompt = "";
        if (promptList.Data is not null)
            prompt = string.Join("\n", promptList.Data.Select(x => x.NewPrompt).ToArray());
        
        PromptOutput = prompt;

        if (AutoCopy == true)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
            {
                await window.Clipboard.SetTextAsync(prompt);

            }
        }
    }

    private List<BlockGenerationSettings> GenerateBlockSettings(List<GuiBlock> blocks)
    {
        var blockSettings = new List<BlockGenerationSettings>();
        if (blocks.Any())
        {
            foreach (var block in blocks)
            {
                var blockSetting = new BlockGenerationSettings
                {
                    BlockName = block.BlockName,
                    BlockFlag = block.BlockFlag,
                    SelectCount = block.SelectCount,
                    BlockShuffleSetting = block.ShuffleEnabled ? Enabled.Enabled : Enabled.Disabled,             // V this shit don't work :( V
                    BlockTagStyleSettings = new BlockTagStyleSettings
                    {
                        IsEnabled = block.TagStyleEnabled ? Enabled.Enabled : Enabled.Disabled,
                        BlockTagStyle = block.SelectedTagStyleOption
                    },
                    BlockRandomDropSettings = new BlockRandomDropSettings
                    {
                        IsEnabled = block.RandomDropEnabled ? Enabled.Enabled : Enabled.Disabled,
                        BlockRandomDropChance = (int)block.RandomDropChance
                    },
                    BlockAddAdjSettings = new BlockAddAdjectivesSettings
                    {
                        IsEnabled = block.AddAdjEnabled ? Enabled.Enabled : Enabled.Disabled,
                        BlockAdjType = block.SelectedAddAdjTypeOption,
                        BlockAddAdjChance = (int)block.AddAdjChance
                    }

                    // add the rest of the settings
                };
                blockSettings.Add(blockSetting);
            }
        }
        return blockSettings;
    }

    public void PopOutWindow()
    {

    }


    [ObservableProperty]
    private string _promptOutput;

    [ObservableProperty]
    private List<PromptHistory> _promptHistory;

    public async void ReloadTagBlock(object args)
    {
        var blockName = args.ToString();
        await _textFileSourceService.RefreshTagBlock(blockName);
    }
}

