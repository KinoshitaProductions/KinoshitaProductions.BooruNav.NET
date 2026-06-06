using System;
using System.ComponentModel;
using System.Globalization;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// A screen aspect ratio expressed as <c>major:minor</c> (long edge : short edge, so
/// <see cref="Ratio"/> is always &gt;= 1). Parseable from XAML as "20:9" / "19.5:9" /
/// "16x9" thanks to <see cref="AspectRatioConverter"/>.
/// </summary>
[TypeConverter(typeof(AspectRatioConverter))]
public readonly record struct AspectRatio(double Major, double Minor)
{
    /// <summary>Long edge divided by short edge (e.g. 20:9 → ~2.22).</summary>
    public double Ratio => Major / Minor;

    public static readonly AspectRatio Ratio3__2 = new(3, 2);          // older tablets
    public static readonly AspectRatio Ratio16__9 = new(16, 9);     // classic 16:9
    public static readonly AspectRatio Ratio19_5__9 = new(19.5, 9); // iPhone X..15
    public static readonly AspectRatio Ratio20__9 = new(20, 9);       // tall Android

    public static AspectRatio Parse(string text, IFormatProvider? provider = null)
    {
        if (TryParse(text, provider, out var result))
            return result;
        throw new FormatException($"Invalid aspect ratio '{text}'. Use 'W:H' such as '20:9'.");
    }

    public static bool TryParse(string? text, IFormatProvider? provider, out AspectRatio result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var culture = provider ?? CultureInfo.InvariantCulture;
        var parts = text.Split(
            new[] { ':', 'x', 'X', '/', '×' },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 2
            && double.TryParse(parts[0], NumberStyles.Float, culture, out var major)
            && double.TryParse(parts[1], NumberStyles.Float, culture, out var minor)
            && major > 0 && minor > 0)
        {
            result = new AspectRatio(major, minor);
            return true;
        }

        // Single number is treated as ratio:1 (e.g. "1.78").
        if (parts.Length == 1
            && double.TryParse(parts[0], NumberStyles.Float, culture, out var ratio)
            && ratio > 0)
        {
            result = new AspectRatio(ratio, 1);
            return true;
        }

        return false;
    }

    public override string ToString() => $"{Major}:{Minor}";
}

/// <summary>Lets XAML set <see cref="AspectRatio"/> properties from strings like "20:9".</summary>
public sealed class AspectRatioConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string s ? AspectRatio.Parse(s, culture) : base.ConvertFrom(context, culture, value);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        => destinationType == typeof(string) && value is AspectRatio a ? a.ToString() : base.ConvertTo(context, culture, value, destinationType);
}
