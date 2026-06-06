using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using KinoshitaProductions.BooruNav.Presentation;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

public class ThemeServiceTests
{
    [AvaloniaFact]
    public void Request_applies_the_variant_to_the_application()
    {
        var app = Application.Current!;
        var original = app.RequestedThemeVariant;
        try
        {
            var theme = new ThemeService();

            theme.Request(ThemeVariant.Dark);

            theme.Current.ShouldBe(ThemeVariant.Dark);
            app.RequestedThemeVariant.ShouldBe(ThemeVariant.Dark);

            theme.Request(ThemeVariant.Light);

            app.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
        }
        finally
        {
            app.RequestedThemeVariant = original; // keep the shared headless app pristine for other tests
        }
    }
}
