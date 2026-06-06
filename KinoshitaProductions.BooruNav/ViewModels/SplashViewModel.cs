using CommunityToolkit.Mvvm.ComponentModel;

namespace KinoshitaProductions.BooruNav.ViewModels;

public partial class SplashViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _message = "Loading…";
}
