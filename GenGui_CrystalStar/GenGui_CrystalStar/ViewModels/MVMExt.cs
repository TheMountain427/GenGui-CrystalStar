﻿using Avalonia.Controls.ApplicationLifetimes;
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



    private IList<BlockFlag> _blockFlagsList { get; } = new List<BlockFlag>
    {
        BlockFlag.positive,
        BlockFlag.negative,
        BlockFlag.width,
        BlockFlag.height,
        BlockFlag.steps,
        BlockFlag.cfg_scale,
        BlockFlag.batch_size,
        BlockFlag.sd_model,
        BlockFlag.sampler_name,
        BlockFlag.sampler_index,
        BlockFlag.seed,
        BlockFlag.subseed,
        BlockFlag.subseed_strength,
        BlockFlag.outpath_samples,
        BlockFlag.outpath_grids,
        BlockFlag.prompt_for_display,
        BlockFlag.styles,
        BlockFlag.seed_resize_from_w,
        BlockFlag.seed_resize_from_h,
        BlockFlag.n_iter,
        BlockFlag.restore_faces,
        BlockFlag.tiling,
        BlockFlag.do_not_save_samples,
        BlockFlag.do_not_save_grid
    };

    [ObservableProperty]
    private List<Blocks> _positive;
    [ObservableProperty]
    private List<Blocks> _negative;
    [ObservableProperty]
    private List<Blocks> _width;
    [ObservableProperty]
    private List<Blocks> _height;
    [ObservableProperty]
    private List<Blocks> _steps;
    [ObservableProperty]
    private List<Blocks> _cfg_scale;
    [ObservableProperty]
    private List<Blocks> _batch_size;
    [ObservableProperty]
    private List<Blocks> _sd_model;
    [ObservableProperty]
    private List<Blocks> _sampler_name;
    [ObservableProperty]
    private List<Blocks> _sampler_index;
    [ObservableProperty]
    private List<Blocks> _seed;
    [ObservableProperty]
    private List<Blocks> _subseed;
    [ObservableProperty]
    private List<Blocks> _subseed_strength;
    [ObservableProperty]
    private List<Blocks> _outpath_samples;
    [ObservableProperty]
    private List<Blocks> _outpath_grids;
    [ObservableProperty]
    private List<Blocks> _prompt_for_display;
    [ObservableProperty]
    private List<Blocks> _styles;
    [ObservableProperty]
    private List<Blocks> _seed_resize_from_w;
    [ObservableProperty]
    private List<Blocks> _seed_resize_from_h;
    [ObservableProperty]
    private List<Blocks> _n_iter;
    [ObservableProperty]
    private List<Blocks> _restore_faces;
    [ObservableProperty]
    private List<Blocks> _tiling;
    [ObservableProperty]
    private List<Blocks> _do_not_save_samples;
    [ObservableProperty]
    private List<Blocks> _do_not_save_grid;



}
