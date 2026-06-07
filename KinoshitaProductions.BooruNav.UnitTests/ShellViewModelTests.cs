using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Localization;
using KinoshitaProductions.BooruNav.Presentation;
using KinoshitaProductions.BooruNav.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

public class ShellViewModelTests
{
    private static (ShellViewModel shell, IMessenger messenger, AppState state) Build(bool firstRun)
    {
        var state = new AppState { IsFirstRun = firstRun };
        var messenger = new WeakReferenceMessenger();

        var services = new ServiceCollection();
        services.AddSingleton<IMessenger>(messenger);
        services.AddSingleton<IAppState>(state);
        services.AddSingleton(Substitute.For<ILocalizationService>());
        services.AddTransient<SplashViewModel>();
        services.AddTransient<WelcomeViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddSingleton<ShellViewModel>();

        var shell = services.BuildServiceProvider().GetRequiredService<ShellViewModel>();
        shell.MinimumSplashDuration = TimeSpan.Zero; // keep the test instant
        return (shell, messenger, state);
    }

    [Fact]
    public void Starts_on_the_splash_page()
    {
        var (shell, _, _) = Build(firstRun: true);

        shell.Current.ShouldBeOfType<SplashViewModel>();
    }

    [Fact]
    public async Task First_run_routes_to_welcome()
    {
        var (shell, _, _) = Build(firstRun: true);

        await shell.InitializeAsync();

        shell.Current.ShouldBeOfType<WelcomeViewModel>();
    }

    [Fact]
    public async Task Returning_user_routes_straight_to_main()
    {
        var (shell, _, _) = Build(firstRun: false);

        await shell.InitializeAsync();

        shell.Current.ShouldBeOfType<MainViewModel>();
    }

    [Fact]
    public async Task Completing_welcome_navigates_to_main_and_clears_first_run()
    {
        var (shell, messenger, state) = Build(firstRun: true);
        await shell.InitializeAsync();

        messenger.Send(new WelcomeCompleted());

        shell.Current.ShouldBeOfType<MainViewModel>();
        state.IsFirstRun.ShouldBeFalse();
    }
}
