using Avalonia.Styling;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Single source of truth for the app theme. <see cref="ThemeVariant.Default"/> follows the OS;
/// <see cref="ThemeVariant.Light"/>/<see cref="ThemeVariant.Dark"/> force a variant. Thread-safe —
/// the change is applied to <c>Application.Current.RequestedThemeVariant</c> on the UI thread.
/// </summary>
public interface IThemeService
{
    ThemeVariant Current { get; }

    void Request(ThemeVariant variant);
}
