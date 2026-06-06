using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Default <see cref="IThemeService"/>. Initialises <see cref="Current"/> from the running app and
/// applies requests on the UI thread. A clean seam for later persisting the user's choice (see the
/// "dark by default but user-overridable" note in CLAUDE.md).
/// </summary>
public sealed class ThemeService : IThemeService
{
    public ThemeVariant Current { get; private set; } =
        Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;

    public void Request(ThemeVariant variant)
    {
        if (Dispatcher.UIThread.CheckAccess())
            Apply(variant);
        else
            Dispatcher.UIThread.Post(() => Apply(variant));
    }

    private void Apply(ThemeVariant variant)
    {
        if (Equals(variant, Current))
            return;

        Current = variant;
        if (Application.Current is { } app)
            app.RequestedThemeVariant = variant;
    }
}
