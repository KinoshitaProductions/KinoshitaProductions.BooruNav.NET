using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using KinoshitaProductions.BooruNav.Controls;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

// These exercise the control in isolation (not attached to the styled app tree), so the
// App.axaml style that injects Accent3 via DynamicResource does not apply here — the counts
// below reflect only what the control builds from its own properties.
public class GlowBackgroundTests
{
    [AvaloniaFact]
    public void Builds_one_glow_per_accent_by_default()
    {
        var glow = new GlowBackground(); // Accent1 + Accent2, Accent3 unset

        glow.Children.Count.ShouldBe(2);
        glow.Children.ShouldAllBe(c => c is Ellipse);
    }

    [AvaloniaFact]
    public void A_third_accent_adds_a_third_glow()
    {
        var glow = new GlowBackground { Accent3 = Colors.Teal };

        glow.Children.Count.ShouldBe(3);
    }

    [AvaloniaFact]
    public void Disabling_animation_strips_the_glow_classes()
    {
        var glow = new GlowBackground { IsAnimated = false };

        glow.Children.SelectMany(c => c.Classes).ShouldBeEmpty();
    }
}
