using KinoshitaProductions.BooruNav.ViewModels;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

public class MainViewModelTests
{
    [Fact]
    public void Greeting_has_expected_default()
    {
        var vm = new MainViewModel();

        vm.Greeting.ShouldBe("Welcome to Avalonia!");
    }

    [Fact]
    public void Setting_Greeting_raises_PropertyChanged()
    {
        var vm = new MainViewModel();
        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Greeting = "Hello";

        vm.Greeting.ShouldBe("Hello");
        changed.ShouldContain(nameof(MainViewModel.Greeting));
    }

    [Fact]
    public void Setting_Greeting_to_the_same_value_does_not_raise()
    {
        var vm = new MainViewModel();
        var raised = 0;
        vm.PropertyChanged += (_, _) => raised++;

        vm.Greeting = vm.Greeting; // SetProperty short-circuits on equal values

        raised.ShouldBe(0);
    }
}
