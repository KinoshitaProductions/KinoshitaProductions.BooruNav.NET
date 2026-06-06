using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using KinoshitaProductions.BooruNav.Controls;
using KinoshitaProductions.BooruNav.Presentation;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

// ViewportHost is exercised detached from any window. There is no composition root under headless
// (OnFrameworkInitializationCompleted never runs), so the host falls back to its seed properties —
// letting these assert the pure letterbox math without DI in the way.
//
// When emulating, the child is *arranged* at full device size (so its own layout is unaware of the
// frame) and then scaled + centered by a render transform. We assert both: child.Bounds for the
// device box, and the transform matrix for the scale/offset (M11/M22 = scale, M31/M32 = translate).
public class ViewportHostTests
{
    private const double Tol = 0.01;

    private static Control LayOut(ViewportHost host, Size window)
    {
        var child = new Border();
        host.Child = child;
        host.Measure(window);
        host.Arrange(new Rect(window));
        return child;
    }

    [AvaloniaFact]
    public void Desktop_mode_is_a_passthrough()
    {
        var child = LayOut(new ViewportHost { Mode = FormFactor.Desktop }, new Size(800, 600));

        child.Bounds.ShouldBe(new Rect(0, 0, 800, 600));
        child.RenderTransform.ShouldBeNull();
    }

    [AvaloniaFact]
    public void Mobile_letterboxes_explicit_size_centered_and_scaled_to_fit()
    {
        // device 400x800 in an 800x600 window → scale = min(1, 800/400, 600/800) = 0.75
        var child = LayOut(
            new ViewportHost { Mode = FormFactor.Mobile, DeviceSize = new Size(400, 800) },
            new Size(800, 600));

        child.Bounds.ShouldBe(new Rect(0, 0, 400, 800));

        var m = ((TransformGroup)child.RenderTransform!).Value;
        m.M11.ShouldBe(0.75, Tol);
        m.M22.ShouldBe(0.75, Tol);
        m.M31.ShouldBe((800 - 400 * 0.75) / 2, Tol); // 250 — horizontal letterbox bars
        m.M32.ShouldBe(0, Tol);                       // fits exactly in height
    }

    [AvaloniaFact]
    public void Mobile_derives_size_from_aspect_ratio_when_no_explicit_size()
    {
        // 20:9 portrait, long edge 900 → 405x900; in 800x450 → scale = min(1, 800/405, 450/900) = 0.5
        var child = LayOut(
            new ViewportHost { Mode = FormFactor.Mobile, Aspect = AspectRatio.Ratio20__9 },
            new Size(800, 450));

        child.Bounds.ShouldBe(new Rect(0, 0, 405, 900));

        var m = ((TransformGroup)child.RenderTransform!).Value;
        m.M11.ShouldBe(0.5, Tol);
        m.M31.ShouldBe((800 - 405 * 0.5) / 2, Tol); // 298.75
        m.M32.ShouldBe(0, Tol);
    }

    [AvaloniaFact]
    public void Never_scales_above_one_to_one()
    {
        // device smaller than the window → shown at native size, just centered.
        var child = LayOut(
            new ViewportHost { Mode = FormFactor.Mobile, DeviceSize = new Size(200, 400) },
            new Size(1000, 1000));

        var m = ((TransformGroup)child.RenderTransform!).Value;
        m.M11.ShouldBe(1.0, Tol);
        m.M31.ShouldBe((1000 - 200) / 2, Tol); // 400
        m.M32.ShouldBe((1000 - 400) / 2, Tol); // 300
    }

    [AvaloniaFact]
    public void Landscape_swaps_the_device_dimensions()
    {
        var child = LayOut(
            new ViewportHost
            {
                Mode = FormFactor.Mobile,
                DeviceSize = new Size(400, 800),
                Orientation = ScreenOrientation.Landscape,
            },
            new Size(1000, 1000));

        child.Bounds.ShouldBe(new Rect(0, 0, 800, 400)); // width/height swapped for landscape
    }
}
