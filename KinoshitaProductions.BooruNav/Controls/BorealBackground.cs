using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// "Aurora" background: four constant shades of a single <see cref="Accent"/> colour glowing up
/// from the bottom edge and fading out toward the top. Fully transparent over whatever it's placed
/// on (no opaque fill), responsive (the glows are sized relative to the container), and
/// compositor-friendly (animates only RenderTransform / Opacity — see BorealBackground.axaml).
/// Sibling to <see cref="GlowBackground"/>; this one is bottom-anchored and single-accent.
/// </summary>
public class BorealBackground : Panel
{
    public static readonly StyledProperty<Color> AccentProperty =
        AvaloniaProperty.Register<BorealBackground, Color>(nameof(Accent), Color.FromArgb(0xFF, 0x3F, 0xE0, 0xA8));

    /// <summary>Peak opacity (0..1) of the brightest beam's core, multiplied into each shade's alpha.</summary>
    public static readonly StyledProperty<double> IntensityProperty =
        AvaloniaProperty.Register<BorealBackground, double>(nameof(Intensity), 0.35);

    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<BorealBackground, bool>(nameof(IsAnimated), true);

    public Color Accent { get => GetValue(AccentProperty); set => SetValue(AccentProperty, value); }
    public double Intensity { get => GetValue(IntensityProperty); set => SetValue(IntensityProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }

    // Four constant beams rising from the bottom: horizontal centre, horizontal radius, vertical
    // reach (>= 1.0 spills past the top), shade (lerp toward white), and per-beam intensity.
    private static readonly Beam[] Beams =
    {
        new(CenterX: 0.18, RadiusX: 0.50, RadiusY: 0.95, ShadeMix: 0.00, Intensity: 1.00),
        new(CenterX: 0.42, RadiusX: 0.60, RadiusY: 1.18, ShadeMix: 0.16, Intensity: 0.85),
        new(CenterX: 0.64, RadiusX: 0.45, RadiusY: 1.00, ShadeMix: 0.32, Intensity: 0.92),
        new(CenterX: 0.85, RadiusX: 0.55, RadiusY: 0.88, ShadeMix: 0.50, Intensity: 0.70),
    };

    static BorealBackground()
    {
        var affectsVisual = new AvaloniaProperty[] { AccentProperty, IntensityProperty, IsAnimatedProperty };
        foreach (var p in affectsVisual)
            p.Changed.AddClassHandler<BorealBackground>((c, _) => c.Rebuild());
    }

    public BorealBackground()
    {
        ClipToBounds = true;
        Rebuild();
    }

    private void Rebuild()
    {
        Children.Clear();

        for (var i = 0; i < Beams.Length; i++)
        {
            var rect = new Rectangle
            {
                Fill = BuildBeamBrush(Beams[i]),
                IsHitTestVisible = false,
            };
            if (IsAnimated)
                rect.Classes.Add($"beam{i}");
            Children.Add(rect);
        }
    }

    private RadialGradientBrush BuildBeamBrush(Beam beam)
    {
        var shade = Lerp(Accent, Colors.White, beam.ShadeMix);
        var coreAlpha = (byte)(Math.Clamp(Intensity * beam.Intensity, 0, 1) * shade.A);
        var core = Color.FromArgb(coreAlpha, shade.R, shade.G, shade.B);
        var edge = Color.FromArgb(0, shade.R, shade.G, shade.B);

        var origin = new RelativePoint(beam.CenterX, 1.0, RelativeUnit.Relative); // bottom edge
        return new RadialGradientBrush
        {
            Center = origin,
            GradientOrigin = origin,
            RadiusX = new RelativeScalar(beam.RadiusX, RelativeUnit.Relative),
            RadiusY = new RelativeScalar(beam.RadiusY, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(core, 0),
                new GradientStop(edge, 1),
            },
        };
    }

    private static Color Lerp(Color from, Color to, double t)
    {
        t = Math.Clamp(t, 0, 1);
        byte Mix(byte a, byte b) => (byte)(a + (b - a) * t);
        return Color.FromArgb(from.A, Mix(from.R, to.R), Mix(from.G, to.G), Mix(from.B, to.B));
    }

    private readonly record struct Beam(double CenterX, double RadiusX, double RadiusY, double ShadeMix, double Intensity);
}
