using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(KinoshitaProductions.BooruNav.ScreenshotTests.TestAppBuilder))]

namespace KinoshitaProductions.BooruNav.ScreenshotTests;

/// <summary>
/// Unlike the UI/E2E suites, this app builder enables the real <b>Skia</b> renderer
/// (<c>UseHeadlessDrawing = false</c>) so <c>CaptureRenderedFrame()</c> produces actual pixels
/// to save as PNGs. That makes it the slower of the headless setups — hence its own project.
/// </summary>
public sealed class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<BooruNav.App>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .WithInterFont();
}
