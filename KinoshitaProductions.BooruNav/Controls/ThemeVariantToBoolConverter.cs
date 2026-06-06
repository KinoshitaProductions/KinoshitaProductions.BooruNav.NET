using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// Binds a single-select value (e.g. the current <c>ThemeVariant</c>) to a group of RadioButtons:
/// <c>Convert</c> returns true for the matching option; <c>ConvertBack</c> writes the option back
/// only when its button becomes checked (the unchecked siblings yield <see cref="BindingOperations.DoNothing"/>
/// so they don't clobber the value).
/// </summary>
public sealed class ThemeVariantToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Equals(value, parameter);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? parameter : BindingOperations.DoNothing;
}
