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

namespace GenGui_CrystalStar.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public void CloseApplication(object msg)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Debug.WriteLine($"App close requested by {msg}");
            lifetime.Shutdown(0);
        }
    }


    private string _maximizeAppBtnContent = char.ConvertFromUtf32(0x0001F5D6);
    public string MaximizeAppBtnContent
    {
        get => _maximizeAppBtnContent;
        set
        {
            SetProperty(ref _maximizeAppBtnContent, value);
        }
    }
    public void MaximizeWindow(object msg)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Debug.WriteLine($"App maximize requested by {msg}");
            bool isMaximized = lifetime.MainWindow.WindowState == WindowState.Maximized;
            lifetime.MainWindow.WindowState = isMaximized ? WindowState.Normal : WindowState.Maximized;
            MaximizeAppBtnContent = isMaximized ? char.ConvertFromUtf32(0x0001F5D6) : char.ConvertFromUtf32(0x0001F5D7); // Update the icon based on the new state
        }
    }


    public void MinimizeWindow(object msg)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Debug.WriteLine($"App minimize requested by {msg}");
            lifetime.MainWindow.WindowState = WindowState.Minimized;
        }
    }

    public void DragWindow(object sender, PointerPressedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Debug.WriteLine($"App drag requested by {sender}");
            lifetime.MainWindow.BeginMoveDrag(e);
            
        }
    }


    
}




