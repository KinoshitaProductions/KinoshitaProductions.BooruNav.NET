using System;
using Avalonia.Data;

namespace KinoshitaProductions.BooruNav.Localization;

/// <summary>
/// XAML markup extension: <c>Text="{i18n:Translate Welcome_Title}"</c>. Returns a (reflection)
/// binding to <see cref="LocalizationService.Instance"/>'s indexer, so it works regardless of
/// <c>AvaloniaUseCompiledBindingsByDefault</c> and updates live when the language changes.
/// </summary>
public sealed class TranslateExtension
{
    public TranslateExtension(string key) => Key = key;

    public string Key { get; set; }

    public Binding ProvideValue(IServiceProvider serviceProvider) => new()
    {
        Mode = BindingMode.OneWay,
        Source = LocalizationService.Instance,
        Path = $"[{Key}]",
    };
}
