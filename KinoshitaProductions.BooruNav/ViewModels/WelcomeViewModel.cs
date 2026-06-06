using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;

namespace KinoshitaProductions.BooruNav.ViewModels;

/// <summary>
/// Single-page welcome wizard: one view, internal step index driven by Back/Next. Finishing or
/// skipping broadcasts <see cref="WelcomeCompleted"/> for the shell to act on.
/// </summary>
public partial class WelcomeViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;

    public int StepCount => 3;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyPropertyChangedFor(nameof(StepLabel))]
    private int _stepIndex;

    public bool IsLastStep => StepIndex >= StepCount - 1;

    public string StepLabel => $"Step {StepIndex + 1} of {StepCount}";

    public WelcomeViewModel(IMessenger messenger) => _messenger = messenger;

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
