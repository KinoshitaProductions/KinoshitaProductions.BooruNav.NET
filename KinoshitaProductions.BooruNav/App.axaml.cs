using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Localization;
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

    /// <summary>
    /// Whether the runtime device-switcher overlay is shown. A plain switch for now — replace this
    /// with a persisted user setting when one exists.
    /// </summary>
    public static bool ShowDeviceSwitcher { get; set; } = true;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        var shell = Services.GetRequiredService<ShellViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = shell };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new ShellView { DataContext = shell };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new ShellView { DataContext = shell };
        }

        // Splash is already shown (set in the shell ctor); kick off warmup, then it routes itself.
        _ = shell.InitializeAsync();

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // CommunityToolkit's messenger is the broadcast channel for ViewportChanged.
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<IViewportService, ViewportService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton<ILocalizationService>(LocalizationService.Instance);

        services.AddSingleton<ShellViewModel>();
        services.AddTransient<SplashViewModel>();
        services.AddTransient<WelcomeViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DeviceSwitcherViewModel>();
        services.AddTransient<LanguageSelectorViewModel>();
    }
}
