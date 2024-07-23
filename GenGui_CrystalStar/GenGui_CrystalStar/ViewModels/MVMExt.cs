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
