using Avalonia;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// An immutable snapshot of the requested UI presentation. Being a record struct it has cheap
/// value equality (so no-op requests can be dropped) and is safe to hand across threads.
/// </summary>
/// <param name="Mode">Desktop (fill) or Mobile (letterboxed phone viewport).</param>
/// <param name="Aspect">Aspect ratio used when <paramref name="Size"/> is not given.</param>
/// <param name="Size">Explicit device size in device-independent pixels; null = derive from <paramref name="Aspect"/>.</param>
/// <param name="Orientation">Portrait or landscape.</param>
/// <param name="Scaling">Extra multiplier applied to the resolved device size (1.0 = none).</param>
public readonly record struct Viewport(
    FormFactor Mode,
    AspectRatio Aspect,
    Size? Size,
    ScreenOrientation Orientation = ScreenOrientation.Portrait,
    double Scaling = 1.0)
{
    /// <summary>Normal desktop: fill the window.</summary>
    public static readonly Viewport Desktop =
        new(FormFactor.Desktop, AspectRatio.Ratio16__9, null, ScreenOrientation.Landscape);

    // A couple of handy device presets to drive emulation from code/tests.
    public static readonly Viewport Pixel8 =
        new(FormFactor.Mobile, AspectRatio.Ratio20__9, new Size(412, 915));

    public static readonly Viewport IPhone15 =
        new(FormFactor.Mobile, AspectRatio.Ratio19_5__9, new Size(393, 852));
}
