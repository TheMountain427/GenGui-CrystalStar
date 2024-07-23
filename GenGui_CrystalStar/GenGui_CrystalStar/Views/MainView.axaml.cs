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
using System.Linq;
using System.ComponentModel;

namespace GenGui_CrystalStar.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }



    // use the changing sidepane rectangle bounds to cleanly set main view width.
    // trying to set it to MainWindow width - OpenPaneLegnth makes it do an ugly jump
    public void BlocksSidePaneOpened(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e is Avalonia.AvaloniaPropertyChangedEventArgs<Avalonia.Rect>)
        {
            if (DataContext is MainViewModel vm)
            {
                string[] newBounds = e.NewValue.ToString().Split(',');
                bool newWidthToDouble = double.TryParse(newBounds[2].Trim(), out double newWidth);
                vm.BlocksPaneRectWidth = newWidth;
            }

        }
    }

    public void HistSidePaneOpened(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e is Avalonia.AvaloniaPropertyChangedEventArgs<Avalonia.Rect>)
        {
            if (DataContext is MainViewModel vm)
            {
                //Debug.WriteLine($"{e.NewValue}");
                string[] newBounds = e.NewValue.ToString().Split(',');
                bool newWidthToDouble = double.TryParse(newBounds[2].Trim(), out double newWidth);
                vm.HistPaneRectWidth = newWidth;
            }

        }
    }

    public void ResetSelectCountBox(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {

            Debug.WriteLine($"SelectCount reset requested by {btn} for {btn.Tag}");
            if (DataContext is MainViewModel vm)
            {
                //var block = vm.Blocks.FirstOrDefault(b => b.BlockName == btn.Tag.ToString());
                //Debug.WriteLine($"{btn.Tag} Old SelectCount: {block.SelectCount} ");

                var parent = btn.Parent;
                if (parent is Grid grid)
                {
                    //grid.Children.Where(c => c is NumericUpDown).ToList().ForEach(n => (n as NumericUpDown).Value = 0);
                }
                //Debug.WriteLine($"{btn.Tag} New SelectCount: {block.SelectCount} ");
            }
        }
    }

    // allow click background to deselect boxes, grid must be focusable and transparent
    public void MainViewGrid_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        (sender as Grid).Focus();
    }
}


