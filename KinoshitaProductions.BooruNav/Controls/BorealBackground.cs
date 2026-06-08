using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// "Aurora" background: a couple of wide, soft pools of a single <see cref="Accent"/> colour glowing
/// up from the bottom edge and fading out before the top (see <see cref="TopSpacing"/>). Fully
/// transparent over whatever it's placed on, responsive (glows are sized relative to the container),
/// and compositor-friendly (animates only RenderTransform / Opacity — see BorealBackground.axaml).
/// Two deliberately wide, overlapping lights read as one smooth wash and keep overdraw low.
/// </summary>
public class BorealBackground : Panel
{
    public static readonly StyledProperty<Color> AccentProperty =
        AvaloniaProperty.Register<BorealBackground, Color>(nameof(Accent), Color.FromArgb(0xFF, 0x3F, 0xE0, 0xA8));

    /// <summary>Peak opacity (0..1) of the brightest beam's core, multiplied into the accent's alpha.</summary>
    public static readonly StyledProperty<double> IntensityProperty =
        AvaloniaProperty.Register<BorealBackground, double>(nameof(Intensity), 0.35);

    /// <summary>
    /// Fraction of the height (0..1) kept dark at the top: the glow's vertical reach is
    /// <c>1 - TopSpacing</c>, so a larger value pulls the light further down.
    /// </summary>
    public static readonly StyledProperty<double> TopSpacingProperty =
        AvaloniaProperty.Register<BorealBackground, double>(nameof(TopSpacing), 0.25);

    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<BorealBackground, bool>(nameof(IsAnimated), true);

    public Color Accent { get => GetValue(AccentProperty); set => SetValue(AccentProperty, value); }
    public double Intensity { get => GetValue(IntensityProperty); set => SetValue(IntensityProperty, value); }
    public double TopSpacing { get => GetValue(TopSpacingProperty); set => SetValue(TopSpacingProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }

    // Two constant beams rising from the bottom: horizontal centre, horizontal radius (deliberately
    // wide — > 1.0 of the width — so the pools blend and their edges never read as moving shapes),
    // and per-beam intensity. Vertical reach comes from TopSpacing, shared by both.
    private static readonly Beam[] Beams =
    {
        new(CenterX: 0.35, RadiusX: 1.50, Intensity: 1.00),
        new(CenterX: 0.65, RadiusX: 1.60, Intensity: 0.80),
    };

    static BorealBackground()
    {
        var affectsVisual = new AvaloniaProperty[]
        {
            AccentProperty, IntensityProperty, TopSpacingProperty, IsAnimatedProperty,
        };
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
        var accent = Accent;
        var coreAlpha = (byte)(Math.Clamp(Intensity * beam.Intensity, 0, 1) * accent.A);
        var core = Color.FromArgb(coreAlpha, accent.R, accent.G, accent.B);
        var edge = Color.FromArgb(0, accent.R, accent.G, accent.B);

        var reach = Math.Clamp(1.0 - TopSpacing, 0.05, 2.0);
        var origin = new RelativePoint(beam.CenterX, 1.0, RelativeUnit.Relative); // bottom edge
        return new RadialGradientBrush
        {
            Center = origin,
            GradientOrigin = origin,
            RadiusX = new RelativeScalar(beam.RadiusX, RelativeUnit.Relative),
            RadiusY = new RelativeScalar(reach, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(core, 0),
                new GradientStop(edge, 1),
            },
        };
    }

    private readonly record struct Beam(double CenterX, double RadiusX, double Intensity);
}
