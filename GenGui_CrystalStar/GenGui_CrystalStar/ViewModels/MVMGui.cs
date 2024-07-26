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

namespace GenGui_CrystalStar.ViewModels;
public partial class MainViewModel : ViewModelBase
{


    [ObservableProperty]
    private double _blocksPaneRectWidth;

    // lets fucking go!!, transistion is super clean now
    partial void OnBlocksPaneRectWidthChanged(double value)
    {
        //Debug.WriteLine($"OnBlocksPaneRectWidth changing MainViewWidth by {value}");
        // give a smooth transition to MainView when closing SidePane
        MainViewWidth = MainWindowWidth - BlocksPaneRectWidth;
    }
    [ObservableProperty]
    private double _histPaneRectWidth;

    partial void OnHistPaneRectWidthChanged(double value)
    {
        //Debug.WriteLine($"OnHistPaneRectWidth changing MainViewWidth by {value}");
        // hide certain controls when window gets small enough
        bool willGeneratorControlsStackGroup1 = (GeneratorControlsWidth * 2) + 60 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup1 = !willGeneratorControlsStackGroup1;
        bool willGeneratorControlsStackGroup2 = (GeneratorControlsWidth * 3) + 80 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup2 = !willGeneratorControlsStackGroup2;
        bool willGeneratorControlsStackGroup3 = (GeneratorControlsWidth * 4) + 100 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup3 = !willGeneratorControlsStackGroup3;
    }

    // Main Window Width set here
    [ObservableProperty]
    private double _mainWindowWidth = 1000;

    partial void OnMainWindowWidthChanged(double value)
    {
        //Debug.WriteLine($"MainWindowWidth changed to {value}");

        // Change MainView width when resizing MainWindow
        // Keeps right side of MainView snug to MainWindow
        MainViewWidth = MainWindowWidth - BlocksPaneRectWidth;

        // Scale down BlocksSplitPane when MainView is resized below SidePane OpenPaneLength
        BlocksOpenPaneLength = _blocksPaneLength <= MainWindowWidth ? _blocksPaneLength : MainWindowWidth - 40;
        HistOpenPaneLength = _histPaneLength <= MainWindowWidth ? _histPaneLength : MainWindowWidth - 40;

    }

    [ObservableProperty]
    private double _mainWindowHeight = 600;

    partial void OnMainWindowHeightChanged(double value)
    {
        Debug.WriteLine($"MainWindowHeight changed to {value}");
        MainViewGridHeight = MainWindowHeight - 35;
    }

    [ObservableProperty] // fix for scroll viewer not going to end
    private double _mainViewGridHeight = 600-35;

    partial void OnMainViewGridHeightChanged(double value)
    {
        Debug.WriteLine($"MainGridWindowHeight changed to {value}");
    }

    [ObservableProperty]
    private double _mainViewWidth;

    partial void OnMainViewWidthChanged(double value)
    {
        //Debug.WriteLine($"MainViewWidth changed to {value}");

        // prevent MainView from going negative
        double _tempMainViewWidth = MainWindowWidth - BlocksPaneRectWidth;
        MainViewWidth = MainViewWidth <= 0 ? 0 : _tempMainViewWidth;

        // hide certain controls when window gets small enough
        bool willGeneratorControlsStackGroup1 = (GeneratorControlsWidth * 2) + 60 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup1 = !willGeneratorControlsStackGroup1;
        bool willGeneratorControlsStackGroup2 = (GeneratorControlsWidth * 3) + 80 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup2 = !willGeneratorControlsStackGroup2;
        bool willGeneratorControlsStackGroup3 = (GeneratorControlsWidth * 4) + 100 > MainViewWidth - HistPaneRectWidth;
        GeneratorControlVisibleGroup3 = !willGeneratorControlsStackGroup3;
    }

    [ObservableProperty]
    private double _blocksPaneWidth;

    partial void OnBlocksPaneWidthChanged(double value)
    {
        Debug.WriteLine($"BlocksPaneWidth changed to {value}");
    }


    private const double _blocksPaneLength = 500; // set BlocksPane OpenPaneLength here
    [ObservableProperty]
    private double _blocksOpenPaneLength = _blocksPaneLength;
    partial void OnBlocksOpenPaneLengthChanged(double value)
    {
        Debug.WriteLine($"BlocksOpenPaneLength changed to {value}");
    }

    [ObservableProperty]
    private string _blocksSidePaneIcon = "<";

    partial void OnBlocksSidePaneIconChanged(string value)
    {
        Debug.WriteLine($"BlocksPaneIcon changed to {value}");
    }

    [ObservableProperty]
    private bool _blocksSidePaneState = true;

    partial void OnBlocksSidePaneStateChanged(bool value)
    {
        Debug.WriteLine($"BlocksPaneState changed to {value}");
        bool isSidePaneOpen = BlocksSidePaneState == true;
        BlocksSidePaneIcon = isSidePaneOpen ? "<" : ">";
    }

    public void ChangeBlocksPaneState()
    {
        BlocksSidePaneState = !BlocksSidePaneState;
    }

    [ObservableProperty]
    private bool _histSidePaneState;
    partial void OnHistSidePaneStateChanged(bool value)
    {
        Debug.WriteLine($"HistPaneState changed to {value}");
    }

    public void ChangeHistSidePaneState()
    {
        HistSidePaneState = !HistSidePaneState;
        //if (HistSidePaneState == true)
        //{
        //    PromptHistory = (await _dataService.GetPromptHistory()).Data;
        //}
    }

    private const double _histPaneLength = 400; // set HistSidePane OpenPaneLength here
    [ObservableProperty]
    private double _histOpenPaneLength = _histPaneLength;
    partial void OnHistOpenPaneLengthChanged(double value)
    {
        Debug.WriteLine($"HistOpenPaneLength changed to {value}");
    }

    [ObservableProperty]
    private bool _generatorControlVisibleGroup1;

    partial void OnGeneratorControlVisibleGroup1Changed(bool value)
    {
        Debug.WriteLine($"GeneratorControlVisibleGroup1 changed to {value}");
    }

    [ObservableProperty]
    private bool _generatorControlVisibleGroup2;

    partial void OnGeneratorControlVisibleGroup2Changed(bool value)
    {
        Debug.WriteLine($"GeneratorControlVisibleGroup2 changed to {value}");
    }

    [ObservableProperty]
    private bool _generatorControlVisibleGroup3;

    partial void OnGeneratorControlVisibleGroup3Changed(bool value)
    {
        Debug.WriteLine($"GeneratorControlVisibleGroup3 changed to {value}");
    }

    [ObservableProperty]
    private double _generatorControlsWidth = 175; // set GeneratorControlsWidth here

    // Debug output for when a collection changes
    private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Log or handle the change
        Debug.WriteLine($"Collection changed. Action: {e.Action}");
    }
}