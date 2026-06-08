using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using KinoshitaProductions.BooruNav.Controls;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

// Exercised in isolation (not attached to the styled app tree), so the App.axaml style that sets
// the theme-aware Accent does not apply here — the control builds its four beams from its own
// properties.
public class BorealBackgroundTests
{
    [AvaloniaFact]
    public void Builds_four_beams_by_default()
    {
        var boreal = new BorealBackground();

        boreal.Children.Count.ShouldBe(4);
        boreal.Children.ShouldAllBe(c => c is Rectangle);
    }

    [AvaloniaFact]
    public void Beams_glow_from_the_bottom_edge()
    {
        var boreal = new BorealBackground();

        foreach (var rect in boreal.Children.OfType<Rectangle>())
        {
            var brush = rect.Fill.ShouldBeOfType<RadialGradientBrush>();
            brush.Center.Point.Y.ShouldBe(1.0);          // anchored at the bottom
            brush.Center.Unit.ShouldBe(Avalonia.RelativeUnit.Relative);
        }
    }

    [AvaloniaFact]
    public void Disabling_animation_strips_the_beam_classes()
    {
        var boreal = new BorealBackground { IsAnimated = false };

        boreal.Children.SelectMany(c => c.Classes).ShouldBeEmpty();
    }
}
