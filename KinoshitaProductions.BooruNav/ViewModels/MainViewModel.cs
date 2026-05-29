using CommunityToolkit.Mvvm.ComponentModel;

namespace KinoshitaProductions.BooruNav.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
