using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace KinoshitaProductions.BooruNav.Localization;

/// <summary>
/// Reactive string lookup for the UI. The indexer feeds the <c>{i18n:Translate}</c> markup
/// extension; changing the language raises <see cref="INotifyPropertyChanged"/> for the indexer so
/// every bound string refreshes live. Registered as a DI singleton (the same shared instance the
/// markup extension uses).
/// </summary>
public interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>Localized string for <paramref name="key"/> in the current language.</summary>
    string this[string key] { get; }

    /// <summary>Same as the indexer, for callers that prefer a method.</summary>
    string Get(string key);

    IReadOnlyList<CultureInfo> AvailableLanguages { get; }

    CultureInfo CurrentLanguage { get; }

    /// <summary>Switch language. Thread-safe; the change is applied on the UI thread.</summary>
    void SetLanguage(CultureInfo culture);
}
