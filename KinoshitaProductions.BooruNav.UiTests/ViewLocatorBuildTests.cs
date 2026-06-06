using Avalonia.Headless.XUnit;
using KinoshitaProductions.BooruNav;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

// Build() instantiates real controls via reflection, so it needs a running Avalonia app.
public class ViewLocatorBuildTests
{
    private readonly ViewLocator _locator = new();

    [AvaloniaFact]
    public void Build_maps_a_view_model_to_its_view()
    {
        var control = _locator.Build(new MainViewModel());

        control.ShouldBeOfType<MainView>();
    }

    [AvaloniaFact]
    public void Build_returns_null_for_null_input()
    {
        _locator.Build(null).ShouldBeNull();
    }
}
