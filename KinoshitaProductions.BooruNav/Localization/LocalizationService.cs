using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace KinoshitaProductions.BooruNav.Localization;

/// <summary>
/// Default <see cref="ILocalizationService"/>, backed by the Strings.resx family via
/// <see cref="ResourceManager"/> (English is the neutral fallback; es comes from the satellite
/// assembly). Switching language sets the thread UI culture and raises the indexer change so all
/// <c>{i18n:Translate}</c> bindings re-read.
/// </summary>
public sealed class LocalizationService : ObservableObject, ILocalizationService
{
    /// <summary>Shared instance used by the markup extension and registered in DI.</summary>
    public static LocalizationService Instance { get; } = new();

    private static readonly ResourceManager Strings =
        new("KinoshitaProductions.BooruNav.Resources.Strings", typeof(LocalizationService).Assembly);

    public IReadOnlyList<CultureInfo> AvailableLanguages { get; } = new[]
    {
        CultureInfo.GetCultureInfo("en-US"),
        CultureInfo.GetCultureInfo("es-MX"),
    };

    public CultureInfo CurrentLanguage { get; private set; }

    public LocalizationService()
    {
        // Start in the system language if it's supported, else English.
        var system = CultureInfo.CurrentUICulture;
        CurrentLanguage = AvailableLanguages.FirstOrDefault(
            c => c.TwoLetterISOLanguageName == system.TwoLetterISOLanguageName) ?? AvailableLanguages[0];
        ApplyCulture(CurrentLanguage);
    }

    public string this[string key] => Strings.GetString(key, CurrentLanguage) ?? $"[{key}]";

    public string Get(string key) => this[key];

    public void SetLanguage(CultureInfo culture)
    {
        if (Dispatcher.UIThread.CheckAccess())
            Apply(culture);
        else
            Dispatcher.UIThread.Post(() => Apply(culture));
    }

    private void Apply(CultureInfo culture)
    {
        if (Equals(culture, CurrentLanguage))
            return;

        CurrentLanguage = culture;
        ApplyCulture(culture);

        OnPropertyChanged("Item[]");                 // refresh every {i18n:Translate} binding
        OnPropertyChanged(nameof(CurrentLanguage));
    }

    private static void ApplyCulture(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
