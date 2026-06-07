using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Localization;
using KinoshitaProductions.BooruNav.Presentation;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Single-page welcome wizard: one view, internal step index driven by Back/Next. Finishing or
/// skipping broadcasts <see cref="WelcomeCompleted"/> for the shell to act on. Also surfaces the
/// language chooser (the wizard's strings are localized in the view via <c>{i18n:Translate}</c>).
/// </summary>
public partial class WelcomeViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;
    private readonly ILocalizationService _localization;

    public int StepCount => 3;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyPropertyChangedFor(nameof(StepLabel))]
    private int _stepIndex;

    public bool IsLastStep => StepIndex >= StepCount - 1;

    // Language-neutral (numerals), so it needs no translation.
    public string StepLabel => $"{StepIndex + 1} / {StepCount}";

    public IReadOnlyList<CultureInfo> Languages => _localization.AvailableLanguages;

    public CultureInfo SelectedLanguage
    {
        get => _localization.CurrentLanguage;
        set
        {
            if (Equals(value, _localization.CurrentLanguage))
                return;
            _localization.SetLanguage(value);
            OnPropertyChanged();
        }
    }

    public WelcomeViewModel(IMessenger messenger, ILocalizationService localization)
    {
        _messenger = messenger;
        _localization = localization;
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Back() => StepIndex--;

    private bool CanGoBack() => StepIndex > 0;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void Next() => StepIndex++;

    private bool CanGoNext() => StepIndex < StepCount - 1;

    [RelayCommand]
    private void Finish() => _messenger.Send(new WelcomeCompleted());

    [RelayCommand]
    private void Skip() => _messenger.Send(new WelcomeCompleted());
}
