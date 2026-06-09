using System.Globalization;
using KinoshitaProductions.BooruNav.ViewModels;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

public class LanguageOptionTests
{
    [Theory]
    [InlineData("en-US", "EN", "🇺🇸")]
    [InlineData("es-MX", "ES", "🇲🇽")]
    public void Derives_code_and_flag_from_a_specific_culture(string cultureName, string code, string flag)
    {
        var option = LanguageOption.From(CultureInfo.GetCultureInfo(cultureName));

        option.Code.ShouldBe(code);
        option.Flag.ShouldBe(flag);
        option.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void A_neutral_culture_has_no_region_so_the_flag_falls_back_to_a_globe()
    {
        var option = LanguageOption.From(CultureInfo.GetCultureInfo("en"));

        option.Flag.ShouldBe("🌐");
    }
}
