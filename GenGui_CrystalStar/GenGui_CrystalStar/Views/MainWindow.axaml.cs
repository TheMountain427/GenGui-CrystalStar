using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia;
using GenGui_CrystalStar.ViewModels;
using System;
using System.Diagnostics;

namespace GenGui_CrystalStar.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.PropertyChanged += MainWindow_PropertyChanged;


    }

    // subscribe to window width property changing so we can do a clean transition with the side pane
    private void MainWindow_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == WidthProperty)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.MainWindowWidth = (double)e.NewValue;
                //Debug.WriteLine($"New MainWindow width: {e.NewValue}");
            }
        }
    }

    void SetupSide(string name, StandardCursorType cursor, WindowEdge edge)
    {
        var ctl = this.Get<Control>(name);
        ctl.Cursor = new Cursor(cursor);
        ctl.PointerPressed += (i, e) =>
        {
            BeginResizeDrag(edge, e);
        };
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        SetupSide("Left", StandardCursorType.LeftSide, WindowEdge.West);
        SetupSide("Right", StandardCursorType.RightSide, WindowEdge.East);
        SetupSide("Top", StandardCursorType.TopSide, WindowEdge.North);
        SetupSide("Bottom", StandardCursorType.BottomSide, WindowEdge.South);
        SetupSide("TopLeft1", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest);
        SetupSide("TopLeft2", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest);
        SetupSide("TopRight1", StandardCursorType.TopRightCorner, WindowEdge.NorthEast);
        SetupSide("TopRight2", StandardCursorType.TopRightCorner, WindowEdge.NorthEast);
        SetupSide("BottomLeft1", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest);
        SetupSide("BottomLeft2", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest);
        SetupSide("BottomRight1", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast);
        SetupSide("BottomRight2", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast);
    }
}
