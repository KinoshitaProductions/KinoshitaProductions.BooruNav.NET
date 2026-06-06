using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.ScreenshotTests;

/// <summary>
/// Publishing assets: the real <see cref="MainWindow"/> at store-friendly sizes, in light and dark,
/// plus a hi-DPI (@2x) export. These are generated artifacts, not committed — grab them from the
/// artifacts folder (or SCREENSHOT_OUTPUT_DIR in CI) and upload to the store / README.
/// </summary>
public class PublishingScreenshots(ITestOutputHelper output)
{
    private static MainWindow NewMainWindow() =>
        new() { DataContext = new MainViewModel(), Width = 1280, Height = 800 };

    [AvaloniaFact]
    public void Main_window_desktop_light_and_dark()
    {
        foreach (var (label, theme) in Screenshot.Themes)
        {
            var shot = Screenshot.OfWindow(NewMainWindow(), $"publish/MainWindow.Desktop.{label}", theme);

            shot.Frame.PixelSize.Width.ShouldBeGreaterThan(0);
            output.WriteLine($"wrote {shot.Path} ({shot.Frame.PixelSize.Width}x{shot.Frame.PixelSize.Height})");
        }
    }

    [AvaloniaFact]
    public void Main_window_hi_dpi_2x()
    {
        var shot = Screenshot.OfWindow(NewMainWindow(), "publish/MainWindow.Desktop.Light@2x", ThemeVariant.Light, scale: 2.0);

        // @2x should roughly double the pixel dimensions of the 1x capture.
        shot.Frame.PixelSize.Width.ShouldBeGreaterThan(1280);
        output.WriteLine($"wrote {shot.Path} ({shot.Frame.PixelSize.Width}x{shot.Frame.PixelSize.Height})");
    }
}
