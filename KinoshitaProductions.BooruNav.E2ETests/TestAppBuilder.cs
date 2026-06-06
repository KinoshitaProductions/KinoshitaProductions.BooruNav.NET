using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(KinoshitaProductions.BooruNav.E2ETests.TestAppBuilder))]

namespace KinoshitaProductions.BooruNav.E2ETests;

/// <summary>
/// Boots the real <see cref="BooruNav.App"/> headlessly so end-to-end tests can drive the
/// actual <see cref="BooruNav.Views.MainWindow"/> the Desktop head shows. See README.md for
/// the headless-vs-Appium rationale.
/// </summary>
public sealed class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<BooruNav.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont();
}
