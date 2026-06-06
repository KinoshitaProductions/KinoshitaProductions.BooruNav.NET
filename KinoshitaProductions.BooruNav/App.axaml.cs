using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Microsoft.Extensions.DependencyInjection;

namespace KinoshitaProductions.BooruNav;

public partial class App : Application
{
    /// <summary>
    /// Composition root. Assigned once in <see cref="OnFrameworkInitializationCompleted"/>; controls
    /// that can't take constructor injection (created by XAML) resolve services from here.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>(),
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory =
                () => new MainView { DataContext = Services.GetRequiredService<MainViewModel>() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // CommunityToolkit's messenger is the broadcast channel for ViewportChanged.
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<IViewportService, ViewportService>();

        services.AddTransient<MainViewModel>();
    }
}
