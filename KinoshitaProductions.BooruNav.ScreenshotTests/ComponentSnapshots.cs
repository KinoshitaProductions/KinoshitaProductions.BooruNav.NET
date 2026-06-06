using Avalonia;
using Avalonia.Headless.XUnit;
using KinoshitaProductions.BooruNav.Controls;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.ScreenshotTests;

/// <summary>
/// Per-component review snapshots in light and dark. Each run regenerates the PNGs so a human can
/// eyeball them; the assertions only guard that something actually rendered.
/// </summary>
public class ComponentSnapshots(ITestOutputHelper output)
{
    [AvaloniaFact]
    public void GlowBackground_in_both_themes()
    {
        foreach (var (label, theme) in Screenshot.Themes)
        {
            var shot = Screenshot.OfControl(new GlowBackground(), new Size(480, 320), $"components/GlowBackground.{label}", theme);

            shot.Frame.PixelSize.Width.ShouldBeGreaterThan(0);
            output.WriteLine($"wrote {shot.Path} ({shot.Frame.PixelSize.Width}x{shot.Frame.PixelSize.Height})");
        }
    }

    [AvaloniaFact]
    public void MainView_in_both_themes()
    {
        foreach (var (label, theme) in Screenshot.Themes)
        {
            var view = new MainView { DataContext = new MainViewModel() };
            var shot = Screenshot.OfControl(view, new Size(800, 450), $"components/MainView.{label}", theme);

            shot.Frame.PixelSize.Width.ShouldBeGreaterThan(0);
            output.WriteLine($"wrote {shot.Path} ({shot.Frame.PixelSize.Width}x{shot.Frame.PixelSize.Height})");
        }
    }
}
