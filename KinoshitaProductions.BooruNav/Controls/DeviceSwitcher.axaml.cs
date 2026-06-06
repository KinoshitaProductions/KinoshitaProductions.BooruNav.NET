using Avalonia.Controls;
using KinoshitaProductions.BooruNav.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// Floating debug overlay for switching the emulated device at runtime. Resolves its view model
/// from the composition root (controls can't take constructor injection). Visibility is gated by
/// <see cref="App.ShowDeviceSwitcher"/> at the call site; see <c>MainWindow.axaml</c>.
/// </summary>
public partial class DeviceSwitcher : UserControl
{
    public DeviceSwitcher()
    {
        InitializeComponent();

        // Null in the previewer (no composition root); bindings simply stay empty there.
        if (App.Services is { } services)
            DataContext = services.GetRequiredService<DeviceSwitcherViewModel>();
    }
}
