using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// Soft "flashlight" glow background: a few translucent radial-gradient pools that
/// drift slowly to emulate an animated gradient hue. Compositor-friendly (animates
/// only RenderTransform). Reuse anywhere; tune via the Accent / Intensity properties.
/// </summary>
public class GlowBackground : Panel
{
    public static readonly StyledProperty<Color> Accent1Property =
        AvaloniaProperty.Register<GlowBackground, Color>(nameof(Accent1), Color.FromArgb(0xFF, 0x4F, 0xB0, 0xFF));

    public static readonly StyledProperty<Color> Accent2Property =
        AvaloniaProperty.Register<GlowBackground, Color>(nameof(Accent2), Color.FromArgb(0xFF, 0xB3, 0x6B, 0xFF));

    public static readonly StyledProperty<Color?> Accent3Property =
        AvaloniaProperty.Register<GlowBackground, Color?>(nameof(Accent3));

    /// <summary>Peak opacity of a glow's core (0..1), multiplied into each accent's alpha.</summary>
    public static readonly StyledProperty<double> IntensityProperty =
        AvaloniaProperty.Register<GlowBackground, double>(nameof(Intensity), 0.22);

    /// <summary>Diameter (px) of the base glow; individual glows scale around this.</summary>
    public static readonly StyledProperty<double> GlowSizeProperty =
        AvaloniaProperty.Register<GlowBackground, double>(nameof(GlowSize), 640d);

    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<GlowBackground, bool>(nameof(IsAnimated), true);

    public Color Accent1 { get => GetValue(Accent1Property); set => SetValue(Accent1Property, value); }
    public Color Accent2 { get => GetValue(Accent2Property); set => SetValue(Accent2Property, value); }
    public Color? Accent3 { get => GetValue(Accent3Property); set => SetValue(Accent3Property, value); }
    public double Intensity { get => GetValue(IntensityProperty); set => SetValue(IntensityProperty, value); }
    public double GlowSize { get => GetValue(GlowSizeProperty); set => SetValue(GlowSizeProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }

    private static readonly double[] SizeFactors = { 1.0, 0.85, 1.15 };

    static GlowBackground()
    {
        var affectsVisual = new AvaloniaProperty[]
        {
            Accent1Property, Accent2Property, Accent3Property,
            IntensityProperty, GlowSizeProperty, IsAnimatedProperty,
        };
        foreach (var p in affectsVisual)
            p.Changed.AddClassHandler<GlowBackground>((c, _) => c.Rebuild());
    }

    public GlowBackground()
    {
        ClipToBounds = true;
        Rebuild();
    }

    private void Rebuild()
    {
        Children.Clear();

        var accents = new List<Color> { Accent1, Accent2 };
        if (Accent3 is { } third)
            accents.Add(third);

        for (var i = 0; i < accents.Count; i++)
        {
            var size = GlowSize * SizeFactors[i % SizeFactors.Length];
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Fill = BuildGlowBrush(accents[i]),
                IsHitTestVisible = false,
            };
            if (IsAnimated)
                ellipse.Classes.Add($"glow{i}");
            Children.Add(ellipse);
        }
    }

    private RadialGradientBrush BuildGlowBrush(Color color)
    {
        var coreAlpha = (byte)(Math.Clamp(Intensity, 0, 1) * color.A);
        var core = Color.FromArgb(coreAlpha, color.R, color.G, color.B);
        var edge = Color.FromArgb(0, color.R, color.G, color.B);

        var brush = new RadialGradientBrush();
        brush.GradientStops.Add(new GradientStop(core, 0));
        brush.GradientStops.Add(new GradientStop(edge, 1));
        return brush;
    }
}
