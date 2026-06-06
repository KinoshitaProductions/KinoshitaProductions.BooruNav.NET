using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Root navigator. Hosts the current page in <see cref="Current"/> (a ContentControl in ShellView
/// binds it, and the ViewLocator turns the VM into its view). Flow: Splash → (Welcome | Main),
/// with Welcome → Main on completion.
/// </summary>
public partial class ShellViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;
    private readonly IAppState _state;

    [ObservableProperty]
    private ViewModelBase? _current;

    /// <summary>Floor on how long the in-app splash stays up, to avoid a flicker on fast warmups.</summary>
    public TimeSpan MinimumSplashDuration { get; set; } = TimeSpan.FromMilliseconds(800);

    public ShellViewModel(IServiceProvider services, IAppState state, IMessenger messenger)
    {
        _services = services;
        _state = state;
        messenger.Register<ShellViewModel, WelcomeCompleted>(this, static (shell, _) => shell.GoToMain());

        // Set synchronously so the very first frame shows the splash.
        _current = services.GetRequiredService<SplashViewModel>();
    }

    /// <summary>Run startup work behind the splash, then route to the first real page.</summary>
    public async Task InitializeAsync()
    {
        var warmup = WarmUpAsync();
        if (MinimumSplashDuration > TimeSpan.Zero)
            await Task.Delay(MinimumSplashDuration);
        await warmup;

        Current = _state.IsFirstRun
            ? _services.GetRequiredService<WelcomeViewModel>()
            : _services.GetRequiredService<MainViewModel>();
    }

    // Real startup work belongs here (load settings, auth, warm caches). Trivial for now.
    private static Task WarmUpAsync() => Task.CompletedTask;

    private void GoToMain()
    {
        _state.IsFirstRun = false; // persist once a settings store exists
        Current = _services.GetRequiredService<MainViewModel>();
    }
}
