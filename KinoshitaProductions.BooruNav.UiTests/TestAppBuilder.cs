using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(KinoshitaProductions.BooruNav.UiTests.TestAppBuilder))]

namespace KinoshitaProductions.BooruNav.UiTests;

/// <summary>
/// Boots the real <see cref="BooruNav.App"/> on Avalonia's headless platform so [AvaloniaFact]
/// tests run against the same styles, templates and view locator the app uses at runtime.
/// </summary>
public sealed class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<BooruNav.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont();
}
