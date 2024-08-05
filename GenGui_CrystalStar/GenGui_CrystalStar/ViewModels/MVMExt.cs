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
using GenGui_CrystalStar.Code.DatabaseModels;
using System.Linq;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Avalonia.Styling;

namespace GenGui_CrystalStar.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    public IList<OutputType> PromptOutputType { get; } = new List<OutputType>
    {
        OutputType.Positive,
        OutputType.Api
    };

    [ObservableProperty]
    private OutputType _selectedPromptOutputType = OutputType.Positive;

    [ObservableProperty]
    private bool _trimLastCommaToggle = false;

    partial void OnTrimLastCommaToggleChanged(bool value)
    {
        Debug.WriteLine($"TrimLastCommaToggle changed to {value}");
    }


    public IList<Enabled> EnabledOptionsList { get; } = new List<Enabled>
    {
        Enabled.Enabled,
        Enabled.Disabled
    };

    public IList<SelectionScope> SelectionScopeOptionsList { get; } = new List<SelectionScope>
    {
        SelectionScope.Global,
        SelectionScope.IndividualBlocks
    };



    [ObservableProperty]
    private GlobalShuffleSetting _selectedGlobalShuffleOption = GlobalShuffleSetting.None;

    public IList<GlobalShuffleSetting> GlobalShuffleOptionsList { get; } = new List<GlobalShuffleSetting>
    {
        GlobalShuffleSetting.Full,
        GlobalShuffleSetting.None,
        GlobalShuffleSetting.WithinBlocks,
        GlobalShuffleSetting.WholeBlocks
    };

    partial void OnSelectedGlobalShuffleOptionChanged(GlobalShuffleSetting value)
    {
        Debug.WriteLine($"ShuffleOption changed to {value}");
    }


    [ObservableProperty]
    private TrimLastComma _selectedTrimLastCommaOption = TrimLastComma.True;

    public IList<TrimLastComma> TrimLastCommaOptionsList { get; } = new List<TrimLastComma>
    {
        TrimLastComma.True,
        TrimLastComma.False
    };



    [ObservableProperty]
    private bool _globalTagStyleEnabledOption; // has to be bool :( cant be Enabled enum

    [ObservableProperty]
    private SelectionScope _globalTagStyleSelectionScopeOption = SelectionScope.Global;

    [ObservableProperty]
    private TagStyle _selectedGlobalTagStyleOption = TagStyle.Clean;

    public IList<TagStyle> GlobalTagStyleOptionsList { get; } = new List<TagStyle>
    {
        TagStyle.Clean,
        TagStyle.Underscore,
        TagStyle.Random
    };


    partial void OnGlobalTagStyleEnabledOptionChanged(bool value)
    {
        Debug.WriteLine($"GlobalTagStyleEnabledOption changed to {value}");
    }

    partial void OnGlobalTagStyleSelectionScopeOptionChanged(SelectionScope value)
    {
        Debug.WriteLine($"GlobalTagStyleSelectionScopeOption changed to {value}");
    }

    partial void OnSelectedGlobalTagStyleOptionChanged(TagStyle value)
    {
        Debug.WriteLine($"GlobalSelectedTagStyleOption changed to {value}");
    }



    [ObservableProperty]
    private bool _globalRandomDropEnabledOption;

    [ObservableProperty]
    private SelectionScope _globalRandomDropSelectionScopeOption = SelectionScope.Global;

    [ObservableProperty]
    private double _globalRandomDropChance = 0;

    partial void OnGlobalRandomDropEnabledOptionChanged(bool value)
    {
        Debug.WriteLine($"GlobalRandomDropEnabledOption changed to {value}");
    }

    partial void OnGlobalRandomDropSelectionScopeOptionChanged(SelectionScope value)
    {
        Debug.WriteLine($"GlobalRandomDropSelection changed to {value}");
    }

    partial void OnGlobalRandomDropChanceChanging(double value)
    {
        Debug.WriteLine($"GlobalRandomDropChance changed to {value}");
    }


    [ObservableProperty]
    private bool _globalAddAdjTypeEnabledOption;

    [ObservableProperty]
    private SelectionScope _globalAddAdjTypeSelectionScopeOption = SelectionScope.Global;

    [ObservableProperty]
    private AdjType _selectedGlobalAddAdjOption = AdjType.All;

    public IList<AdjType> GlobalAddAdjectivesOptionsList { get; } = new List<AdjType>
    {
        AdjType.All,
        AdjType.Color,
        AdjType.Size,
        AdjType.Other
    };

    [ObservableProperty]
    private int _globalAddAdjChance;



    //private IList<BlockFlag> _blockFlagsList { get; } = new List<BlockFlag>
    //{
    //    BlockFlag.positive,
    //    BlockFlag.negative,
    //    BlockFlag.width,
    //    BlockFlag.height,
    //    BlockFlag.steps,
    //    BlockFlag.cfg_scale,
    //    BlockFlag.batch_size,
    //    BlockFlag.sd_model,
    //    BlockFlag.sampler_name,
    //    BlockFlag.sampler_index,
    //    BlockFlag.seed,
    //    BlockFlag.subseed,
    //    BlockFlag.subseed_strength,
    //    BlockFlag.outpath_samples,
    //    BlockFlag.outpath_grids,
    //    BlockFlag.prompt_for_display,
    //    BlockFlag.styles,
    //    BlockFlag.seed_resize_from_w,
    //    BlockFlag.seed_resize_from_h,
    //    BlockFlag.n_iter,
    //    BlockFlag.restore_faces,
    //    BlockFlag.tiling,
    //    BlockFlag.do_not_save_samples,
    //    BlockFlag.do_not_save_grid
    //};

    //[ObservableProperty]
    //private List<GuiBlock> _positive;
    //[ObservableProperty]
    //private List<GuiBlock> _negative;
    //[ObservableProperty]
    //private List<GuiBlock> _width;
    //[ObservableProperty]
    //private List<GuiBlock> _height;
    //[ObservableProperty]
    //private List<GuiBlock> _steps;
    //[ObservableProperty]
    //private List<GuiBlock> _cfg_scale;
    //[ObservableProperty]
    //private List<GuiBlock> _batch_size;
    //[ObservableProperty]
    //private List<GuiBlock> _sd_model;
    //[ObservableProperty]
    //private List<GuiBlock> _sampler_name;
    //[ObservableProperty]
    //private List<GuiBlock> _sampler_index;
    //[ObservableProperty]
    //private List<GuiBlock> _seed;
    //[ObservableProperty]
    //private List<GuiBlock> _subseed;
    //[ObservableProperty]
    //private List<GuiBlock> _subseed_strength;
    //[ObservableProperty]
    //private List<GuiBlock> _outpath_samples;
    //[ObservableProperty]
    //private List<GuiBlock> _outpath_grids;
    //[ObservableProperty]
    //private List<GuiBlock> _prompt_for_display;
    //[ObservableProperty]
    //private List<GuiBlock> _styles;
    //[ObservableProperty]
    //private List<GuiBlock> _seed_resize_from_w;
    //[ObservableProperty]
    //private List<GuiBlock> _seed_resize_from_h;
    //[ObservableProperty]
    //private List<GuiBlock> _n_iter;
    //[ObservableProperty]
    //private List<GuiBlock> _restore_faces;
    //[ObservableProperty]
    //private List<GuiBlock> _tiling;
    //[ObservableProperty]
    //private List<GuiBlock> _do_not_save_samples;
    //[ObservableProperty]
    //private List<GuiBlock> _do_not_save_grid;



    public GuiBlock ConvertBlocksToGui(Blocks blocks)
    {
        return new GuiBlock
        {
            BlockName = blocks.BlockName,
            BlockFlag = blocks.BlockFlag,
            SelectCount = blocks.SelectCount,
            ShuffleEnabled = blocks.ShuffleEnabled == Enabled.Enabled ? true : false,
            TagStyleEnabled = blocks.TagStyleEnabled == Enabled.Enabled ? true : false,
            SelectedTagStyleOption = blocks.TagStyleOption,
            RandomDropEnabled = blocks.RandomDropEnabled == Enabled.Enabled ? true : false,
            RandomDropChance = blocks.RandomDropChance,
            AddAdjEnabled = blocks.AddAdjEnabled == Enabled.Enabled ? true : false,
            SelectedAddAdjTypeOption = blocks.AddAdjTypeOption,
            AddAdjChance = blocks.AddAdjChance
        };
    }    
    
    public List<GuiBlock> ConvertBlocksToGui(List<Blocks> blocks)
    {
        
        if (blocks.Any())
        {
            var guiBlockList = new List<GuiBlock>();
            foreach (var block in blocks)
            {
                var guiBlock = new GuiBlock
                {
                    BlockName = block.BlockName,
                    BlockFlag = block.BlockFlag,
                    SelectCount = block.SelectCount,
                    ShuffleEnabled = block.ShuffleEnabled == Enabled.Enabled ? true : false,
                    TagStyleEnabled = block.TagStyleEnabled == Enabled.Enabled ? true : false,
                    SelectedTagStyleOption = block.TagStyleOption,
                    RandomDropEnabled = block.RandomDropEnabled == Enabled.Enabled ? true : false,
                    RandomDropChance = block.RandomDropChance,
                    AddAdjEnabled = block.AddAdjEnabled == Enabled.Enabled ? true : false,
                    SelectedAddAdjTypeOption = block.AddAdjTypeOption,
                    AddAdjChance = block.AddAdjChance
                };
                guiBlockList.Add(guiBlock);
            }

            return guiBlockList;
        }

        else
            return null;
    }


    [ObservableProperty]
    private List<BlocksContainer> _blocksContainers = [];

    public async void LoadBlockContainers()
    {
         // lol smelly shit, idgaf tbh
         // lmaoooooooo it works
        var blocks = await _dataService.GetAllBlocks();

           
        BlocksContainers.Add(new BlocksContainer { 
            BlockFlagName = "positive", 
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.positive).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.positive).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer {
            BlockFlagName = "negative",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.negative).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.negative).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "width",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.width).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.width).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "height",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.height).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.height).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "steps",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.steps).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.steps).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "cfg_scale",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.cfg_scale).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.cfg_scale).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "batch_size",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.batch_size).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.batch_size).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "sd_model",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sd_model).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sd_model).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "sampler_name",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sampler_name).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sampler_name).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "sampler_index",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sampler_index).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.sampler_index).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "seed",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "subseed",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.subseed).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.subseed).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "subseed_strength",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.subseed_strength).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.subseed_strength).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "outpath_samples",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.outpath_samples).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.outpath_samples).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "outpath_grids",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.outpath_grids).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.outpath_grids).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "prompt_for_display",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.prompt_for_display).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.prompt_for_display).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "styles",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.styles).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.styles).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "seed_resize_from_w",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed_resize_from_w).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed_resize_from_w).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "seed_resize_from_h",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed_resize_from_h).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.seed_resize_from_h).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "n_iter",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.n_iter).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.n_iter).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "restore_faces",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.restore_faces).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.restore_faces).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "tiling",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.tiling).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.tiling).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "do_not_save_samples",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.do_not_save_samples).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.do_not_save_samples).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

        BlocksContainers.Add(new BlocksContainer{
            BlockFlagName = "do_not_save_grid",
            GuiBlocks = new List<GuiBlock>(blocks.Data.Where(x => x.BlockFlag == BlockFlag.do_not_save_grid).Count() > 0 ?
                ConvertBlocksToGui(blocks.Data.Where(x => x.BlockFlag == BlockFlag.do_not_save_grid).OrderBy(x => x.BlockName).ToList()) : new List<GuiBlock>())});

    }
}
