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

        Task.Run(async () =>
        {
            Positive = (await _dataService.GetBlocksByFlag(BlockFlag.positive)).Data ?? [];
            Negative = (await _dataService.GetBlocksByFlag(BlockFlag.negative)).Data ?? [];
            Width = (await _dataService.GetBlocksByFlag(BlockFlag.width)).Data ?? [];
            Height = (await _dataService.GetBlocksByFlag(BlockFlag.height)).Data ?? [];
            Steps = (await _dataService.GetBlocksByFlag(BlockFlag.steps)).Data ?? [];
            Cfg_scale = (await _dataService.GetBlocksByFlag(BlockFlag.cfg_scale)).Data ?? [];
            Batch_size = (await _dataService.GetBlocksByFlag(BlockFlag.batch_size)).Data ?? [];
            Sd_model = (await _dataService.GetBlocksByFlag(BlockFlag.sd_model)).Data ?? [];
            Sampler_name = (await _dataService.GetBlocksByFlag(BlockFlag.sampler_name)).Data ?? [];
            Sampler_index = (await _dataService.GetBlocksByFlag(BlockFlag.sampler_index)).Data ?? [];
            Seed = (await _dataService.GetBlocksByFlag(BlockFlag.seed)).Data ?? [];
            Subseed = (await _dataService.GetBlocksByFlag(BlockFlag.subseed)).Data ?? [];
            Subseed_strength = (await _dataService.GetBlocksByFlag(BlockFlag.subseed_strength)).Data ?? [];
            Outpath_samples = (await _dataService.GetBlocksByFlag(BlockFlag.outpath_samples)).Data ?? [];
            Outpath_grids = (await _dataService.GetBlocksByFlag(BlockFlag.outpath_grids)).Data ?? [];
            Prompt_for_display = (await _dataService.GetBlocksByFlag(BlockFlag.prompt_for_display)).Data ?? [];
            Styles = (await _dataService.GetBlocksByFlag(BlockFlag.styles)).Data ?? [];
            Seed_resize_from_w = (await _dataService.GetBlocksByFlag(BlockFlag.seed_resize_from_w)).Data ?? [];
            Seed_resize_from_h = (await _dataService.GetBlocksByFlag(BlockFlag.seed_resize_from_h)).Data ?? [];
            N_iter = (await _dataService.GetBlocksByFlag(BlockFlag.n_iter)).Data ?? [];
            Restore_faces = (await _dataService.GetBlocksByFlag(BlockFlag.restore_faces)).Data ?? [];
            Tiling = (await _dataService.GetBlocksByFlag(BlockFlag.tiling)).Data ?? [];
            Do_not_save_samples = (await _dataService.GetBlocksByFlag(BlockFlag.do_not_save_samples)).Data ?? [];
            Do_not_save_grid = (await _dataService.GetBlocksByFlag(BlockFlag.do_not_save_grid)).Data ?? [];
        });
        Task.WhenAll();


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

    public void RefreshBlocks()
    {

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

        if (Positive.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Positive));
        if (Negative.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Negative));
        if (Width.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Width));
        if (Height.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Height));
        if (Steps.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Steps));
        if (Cfg_scale.Any())
            blockSettingsList.AddRange(GenerateBlockSettings(Cfg_scale));
        // add the rest later

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

    private List<BlockGenerationSettings> GenerateBlockSettings(List<Blocks> blocks)
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
                    BlockShuffleSetting = block.ShuffleEnabled,             // V this shit don't work :( V
                    BlockTagStyleSettings = new BlockTagStyleSettings
                    {
                        IsEnabled = block.TagStyleEnabled,
                        BlockTagStyle = block.TagStyleOption
                    },
                    BlockRandomDropSettings = new BlockRandomDropSettings
                    {
                        IsEnabled = block.RandomDropEnabled,
                        BlockRandomDropChance = block.RandomDropChance
                    },
                    BlockAddAdjSettings = new BlockAddAdjectivesSettings
                    {
                        IsEnabled = block.AddAdjEnabled,
                        BlockAdjType = block.AddAdjTypeOption,
                        BlockAddAdjChance = block.AddAdjChance
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

    //partial void OnPromptHistoryChanged(List<PromptHistory> value)
    //{
    //    throw new NotImplementedException();
    //}
}

