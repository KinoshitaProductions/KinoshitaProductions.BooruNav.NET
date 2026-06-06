using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Backs the runtime device-switcher overlay. Picking a preset or toggling orientation issues a
/// request to <see cref="IViewportService"/>; changes made elsewhere are reflected back here (via
/// <see cref="ViewportChanged"/>) without echoing a new request.
/// </summary>
public partial class DeviceSwitcherViewModel : ViewModelBase, IRecipient<ViewportChanged>
{
    private readonly IViewportService _viewport;
    private readonly IThemeService _theme;
    private bool _suppress;

    public IReadOnlyList<DevicePreset> Presets => DevicePreset.All;

    [ObservableProperty]
    private DevicePreset _selectedPreset;

    [ObservableProperty]
    private bool _isLandscape;

    [ObservableProperty]
    private string _summary = string.Empty;

    [ObservableProperty]
    private ThemeVariant _selectedTheme;

    public DeviceSwitcherViewModel(IViewportService viewport, IThemeService theme, IMessenger messenger)
    {
        _viewport = viewport;
        _theme = theme;
        _selectedPreset = Match(viewport.Current) ?? Presets[0];
        _isLandscape = viewport.Current.Orientation == ScreenOrientation.Landscape;
        _summary = Describe(viewport.Current);
        _selectedTheme = theme.Current;
        messenger.Register<DeviceSwitcherViewModel, ViewportChanged>(this, static (vm, m) => vm.Receive(m));
    }

    partial void OnSelectedThemeChanged(ThemeVariant value) => _theme.Request(value);

    partial void OnSelectedPresetChanged(DevicePreset value)
    {
        if (_suppress || value is null)
            return;

        _viewport.Request(value.Viewport with
        {
            Orientation = IsLandscape ? ScreenOrientation.Landscape : ScreenOrientation.Portrait,
        });
    }

    partial void OnIsLandscapeChanged(bool value)
    {
        if (_suppress)
            return;

        _viewport.Request(v => v with
        {
            Orientation = value ? ScreenOrientation.Landscape : ScreenOrientation.Portrait,
        });
    }

    public void Receive(ViewportChanged message)
    {
        _suppress = true;
        try
        {
            IsLandscape = message.Viewport.Orientation == ScreenOrientation.Landscape;
            if (Match(message.Viewport) is { } preset)
                SelectedPreset = preset;
            Summary = Describe(message.Viewport);
        }
        finally
        {
            _suppress = false;
        }
    }

    // Match on everything except orientation, which the toggle drives independently.
    private static DevicePreset? Match(Viewport vp) =>
        DevicePreset.All.FirstOrDefault(p =>
            p.Viewport.Mode == vp.Mode &&
            p.Viewport.Aspect == vp.Aspect &&
            p.Viewport.Size == vp.Size);

    private static string Describe(Viewport vp)
    {
        if (vp.Mode == FormFactor.Desktop)
            return "Desktop — fills window";

        var size = vp.Size is { } s ? $"{s.Width:0}×{s.Height:0}" : vp.Aspect.ToString();
        return $"{size} · {vp.Orientation}";
    }
}
