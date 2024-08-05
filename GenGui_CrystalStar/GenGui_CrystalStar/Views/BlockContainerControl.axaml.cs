using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GenGui_CrystalStar.ViewModels;
using System.Diagnostics;

namespace GenGui_CrystalStar.Views;

public partial class BlockContainerControl : UserControl
{
    public BlockContainerControl()
    {
        InitializeComponent();
    }

    public void ResetSelectCountBox(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            Debug.WriteLine($"SelectCount reset requested by {btn} for {btn.Tag}");
            if (DataContext is MainViewModel vm)
            {

                var parent = btn.Parent;
                if (parent is Grid grid)
                {
                    grid.Children.Where(c => c is NumericUpDown).ToList().ForEach(n => (n as NumericUpDown).Value = 0);
                }
            }
        }
    }

    private void ResetAllSelectCounts(object sender, RoutedEventArgs e)
    {

        // Traverse the logical tree and find all NumericUpDown controls with the specific tag
        var numericUpDowns = FindControlsWithoutClass<NumericUpDown>(MainStackPanel, "TextBoxOnly");

        // Reset the value of each NumericUpDown control
        foreach (var numericUpDown in numericUpDowns)
        {
            numericUpDown.Value = 0;
        }
    }

    private IEnumerable<T> FindControlsWithoutClass<T>(Control root, string className) where T : Control
    {
        var controls = new List<T>();

        foreach (var child in root.GetLogicalChildren().OfType<Control>())
        {
            if (child is T control && child.Classes.Where(x => x == className).Count() <= 0)
            {
                controls.Add(control);
            }

            controls.AddRange(FindControlsWithoutClass<T>(child, className));
        }

        return controls;
    }

    

    private void ResetAllBlockSettings(object sender, RoutedEventArgs e)
    {

        // Traverse the logical tree and find all NumericUpDown controls with the specific tag
        var numericUpDowns = FindControlsWithClass<NumericUpDown>(MainStackPanel, "TextBoxOnly");

        // Reset the value of each NumericUpDown control
        foreach (var numericUpDown in numericUpDowns)
        {
            numericUpDown.Value = 0;
        }

        var toggleButtons = FindControlsWithTag<ToggleButton>(MainStackPanel, "MiniButton");

        foreach (var button in toggleButtons)
        {
            button.IsChecked = false;
        }

    }

    private IEnumerable<T> FindControlsWithClass<T>(Control root, string className) where T : Control
    {
        var controls = new List<T>();

        foreach (var child in root.GetLogicalChildren().OfType<Control>())
        {
            if (child is T control && child.Classes.Where(x => x == className).Count() > 0)
            {
                controls.Add(control);
            }

            controls.AddRange(FindControlsWithClass<T>(child, className));
        }

        return controls;
    }

    private IEnumerable<T> FindControlsWithTag<T>(Control root, string tag) where T : Control
    {
        var controls = new List<T>();

        foreach (var child in root.GetLogicalChildren().OfType<Control>())
        {
            if (child is T control && control.Tag?.ToString() == tag)
            {
                controls.Add(control);
            }

            controls.AddRange(FindControlsWithTag<T>(child, tag));
        }

        return controls;
    }
}

