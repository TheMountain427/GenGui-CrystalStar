using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GenGui_CrystalStar.Code.Models;
using GenGui_CrystalStar.Code.Services;
using GenGui_CrystalStar.Code;
using GenGui_CrystalStar.ViewModels;
using GenGui_CrystalStar.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenGui_CrystalStar;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        var DataSourceSettings = new DataSourceSettings();
        var TextFileSourceSettings = new TextFileSourceSettings();

        // ------------------------
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

        // ------------------------
        config.GetSection("DataSourceSettings").Bind(DataSourceSettings);
        config.GetSection("TextFileSourceSettings").Bind(TextFileSourceSettings);


        // ------------------------
        var serviceProvider = new ServiceCollection();

        // ------------------------
        serviceProvider.AddDbContext<GenGuiContext>();

        // ------------------------
        serviceProvider.AddSingleton<IDataSourceSettings>(DataSourceSettings);
        serviceProvider.AddSingleton<ITextFileSourceSettings>(TextFileSourceSettings);

        // ------------------------
        serviceProvider.AddScoped<GenGuiContext>();
        serviceProvider.AddScoped<IGenGuiDatabaseService, GenGuiDataBaseService>();
        serviceProvider.AddScoped<ITextFileSourceService, TextFileSourceService>();
        serviceProvider.AddScoped<IGenGuiDataService, GenGuiDataService>();
        serviceProvider.AddScoped<IGeneratorService, GeneratorService>();

        // ------------------------

        serviceProvider.AddTransient<MainViewModel>();

        // ------------------------
        var srv = serviceProvider.BuildServiceProvider();

        var vm = srv.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
