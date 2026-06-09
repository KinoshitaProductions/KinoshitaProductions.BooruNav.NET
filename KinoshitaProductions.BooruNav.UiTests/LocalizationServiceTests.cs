using System.Globalization;
using Avalonia.Headless.XUnit;
using KinoshitaProductions.BooruNav.Localization;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

public class LocalizationServiceTests
{
    [AvaloniaFact]
    public void Strings_switch_when_the_language_changes()
    {
        var originalUi = CultureInfo.CurrentUICulture;
        var originalDefault = CultureInfo.DefaultThreadCurrentUICulture;
        try
        {
            var loc = new LocalizationService();

            loc.SetLanguage(CultureInfo.GetCultureInfo("en-US"));
            loc["Welcome_Finish"].ShouldBe("Get started");

            loc.SetLanguage(CultureInfo.GetCultureInfo("es-MX"));
            loc["Welcome_Finish"].ShouldBe("Comenzar");          // es-MX → es satellite via fallback
            loc.CurrentLanguage.Name.ShouldBe("es-MX");
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalUi;
            CultureInfo.DefaultThreadCurrentUICulture = originalDefault;
        }
    }

    [AvaloniaFact]
    public void Missing_key_is_surfaced_not_swallowed()
    {
        var loc = new LocalizationService();

        loc["NoSuchKey"].ShouldBe("[NoSuchKey]");
    }

    [AvaloniaFact]
    public void Advertises_both_seeded_languages()
    {
        var loc = new LocalizationService();

        loc.AvailableLanguages.Select(c => c.Name)
            .ShouldBe(new[] { "en-US", "es-MX" });
    }
}
