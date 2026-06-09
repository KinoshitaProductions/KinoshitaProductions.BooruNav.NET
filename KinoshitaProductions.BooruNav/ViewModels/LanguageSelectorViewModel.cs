using System.Collections.Generic;
using System.Linq;
using KinoshitaProductions.BooruNav.Localization;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Backs the standalone <c>LanguageSelector</c> widget. Projects the app-global
/// <see cref="ILocalizationService"/>'s cultures into <see cref="LanguageOption"/>s (flag + code +
/// name) and exposes the *settable* <see cref="SelectedLanguage"/> a ComboBox needs.
/// </summary>
public sealed class LanguageSelectorViewModel : ViewModelBase
{
    private readonly ILocalizationService _localization;

    public LanguageSelectorViewModel(ILocalizationService localization)
    {
        _localization = localization;
        Languages = localization.AvailableLanguages.Select(LanguageOption.From).ToList();
    }

    public IReadOnlyList<LanguageOption> Languages { get; }

    public LanguageOption? SelectedLanguage
    {
        get => Languages.FirstOrDefault(o => o.Culture.Equals(_localization.CurrentLanguage));
        set
        {
            if (value is null || value.Culture.Equals(_localization.CurrentLanguage))
                return;

            _localization.SetLanguage(value.Culture);
            OnPropertyChanged();
        }
    }
}
