using System.Globalization;
using KinoshitaProductions.BooruNav.Presentation;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

public class AspectRatioTests
{
    [Theory]
    [InlineData("20:9", 20, 9)]
    [InlineData("19.5:9", 19.5, 9)]
    [InlineData("16x9", 16, 9)]
    [InlineData("16X9", 16, 9)]
    [InlineData("16/9", 16, 9)]
    [InlineData(" 4 : 3 ", 4, 3)]
    public void Parses_major_minor_forms(string text, double major, double minor)
    {
        var ar = AspectRatio.Parse(text, CultureInfo.InvariantCulture);

        ar.Major.ShouldBe(major);
        ar.Minor.ShouldBe(minor);
    }

    [Fact]
    public void A_single_number_is_treated_as_ratio_to_one()
    {
        var ar = AspectRatio.Parse("1.78", CultureInfo.InvariantCulture);

        ar.Minor.ShouldBe(1);
        ar.Ratio.ShouldBe(1.78, 0.0001);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("16:0")]   // zero minor
    [InlineData("0:9")]    // zero major
    [InlineData("16:9:4")] // too many parts
    public void Invalid_input_does_not_parse(string text)
    {
        AspectRatio.TryParse(text, CultureInfo.InvariantCulture, out _).ShouldBeFalse();
    }

    [Fact]
    public void Ratio_is_long_edge_over_short_edge()
    {
        AspectRatio.Ratio20__9.Ratio.ShouldBe(20d / 9d, 0.0001);
        AspectRatio.Ratio16__9.ToString().ShouldBe("16:9");
    }
}
