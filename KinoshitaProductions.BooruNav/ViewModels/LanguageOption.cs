using System;
using System.Globalization;
using System.Text;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Presentation wrapper for a language: the underlying culture plus the parts the picker shows —
/// a region flag emoji, the uppercased language code, and the native language name. The flag comes
/// from the region subtag (so it needs a *specific* culture like en-US), the code from the language.
/// </summary>
public sealed record LanguageOption(CultureInfo Culture, string Flag, string Code, string Name)
{
    public static LanguageOption From(CultureInfo culture) => new(
        culture,
        FlagOf(culture),
        culture.TwoLetterISOLanguageName.ToUpperInvariant(),
        LanguageName(culture));

    // Native language name without the region (e.g. "English", "español") — the flag carries the region.
    private static string LanguageName(CultureInfo culture) =>
        culture.Parent.Name.Length > 0 ? culture.Parent.NativeName : culture.NativeName;

    private static string FlagOf(CultureInfo culture)
    {
        try
        {
            return RegionalIndicator(new RegionInfo(culture.Name).TwoLetterISORegionName);
        }
        catch (ArgumentException)
        {
            return "🌐"; // neutral culture (no region)
        }
    }

    // A two-letter region code maps to two Regional Indicator Symbols that render as a flag.
    private static string RegionalIndicator(string region)
    {
        if (region.Length != 2)
            return "🌐";

        var builder = new StringBuilder(4);
        foreach (var c in region.ToUpperInvariant())
        {
            if (c is < 'A' or > 'Z')
                return "🌐";
            builder.Append(char.ConvertFromUtf32(0x1F1E6 + (c - 'A')));
        }
        return builder.ToString();
    }
}
