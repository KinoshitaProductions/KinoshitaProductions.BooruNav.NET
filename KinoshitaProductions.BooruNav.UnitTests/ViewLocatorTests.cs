using KinoshitaProductions.BooruNav;
using KinoshitaProductions.BooruNav.ViewModels;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

// ViewLocator.Match is pure routing logic (no Avalonia runtime needed), so it lives in the
// unit suite. The reflection-based Build() path, which instantiates real controls, is covered
// in the UI test project under [AvaloniaFact].
public class ViewLocatorTests
{
    private readonly ViewLocator _locator = new();

    [Fact]
    public void Match_is_true_for_view_models() =>
        _locator.Match(new MainViewModel()).ShouldBeTrue();

    [Fact]
    public void Match_is_false_for_non_view_models() =>
        _locator.Match(new object()).ShouldBeFalse();

    [Fact]
    public void Match_is_false_for_null() =>
        _locator.Match(null).ShouldBeFalse();
}
