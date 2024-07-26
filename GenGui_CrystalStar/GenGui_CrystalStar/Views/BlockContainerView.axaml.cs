using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Linq;
using GenGui_CrystalStar.ViewModels;
using System.Diagnostics;

namespace GenGui_CrystalStar.Views;

public partial class BlockContainerView : UserControl
{
    public BlockContainerView()
    {
        InitializeComponent();
    }

   

    public void ResetSelectCountBox(object sender, RoutedEventArgs e)
    {
        if ( sender is Button btn)
        {

            Debug.WriteLine($"SelectCount reset requested by {btn} for {btn.Tag}");
            if ( DataContext is MainViewModel vm)
            {
                
                var parent = btn.Parent;
                if (parent is Grid grid)
                {
                    grid.Children.Where(c => c is NumericUpDown).ToList().ForEach(n => (n as NumericUpDown).Value = 0);
                }
            }

            


        }
    }
}


