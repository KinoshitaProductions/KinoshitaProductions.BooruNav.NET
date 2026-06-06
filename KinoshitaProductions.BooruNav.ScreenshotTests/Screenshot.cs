using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;

namespace KinoshitaProductions.BooruNav.ScreenshotTests;

/// <summary>A captured frame and the PNG path it was written to.</summary>
internal sealed record Shot(string Path, WriteableBitmap Frame);

/// <summary>
/// Renders Avalonia controls/windows through the headless Skia backend and writes PNGs under the
/// artifacts directory. Used both for human review and to produce publishing assets. Diffing
/// against committed baselines can be layered on later — see README.
/// </summary>
internal static class Screenshot
{
    /// <summary>The theme variants every visual is captured in.</summary>
    public static readonly (string Label, ThemeVariant Theme)[] Themes =
    {
        ("Light", ThemeVariant.Light),
        ("Dark", ThemeVariant.Dark),
    };

    private static readonly string OutputDir = ResolveOutputDir();

    /// <summary>Capture a single control, hosted in a borderless window of the given size.</summary>
    public static Shot OfControl(Control content, Size size, string name, ThemeVariant? theme = null, double scale = 1.0)
    {
        // Headless windows draw no OS chrome, so the client area fills the requested size.
        var window = new Window
        {
            Content = content,
            Width = size.Width,
            Height = size.Height,
        };
        return Capture(window, name, theme, scale);
    }

    /// <summary>Capture a full window (e.g. the real MainWindow) exactly as the app shows it.</summary>
    public static Shot OfWindow(Window window, string name, ThemeVariant? theme = null, double scale = 1.0)
        => Capture(window, name, theme, scale);

    private static Shot Capture(Window window, string name, ThemeVariant? theme, double scale)
    {
        if (theme is not null && Application.Current is { } app)
            app.RequestedThemeVariant = theme;

        window.Show();
        if (scale != 1.0)
            window.SetRenderScaling(scale);
        Dispatcher.UIThread.RunJobs(); // flush layout, bindings and a render pass

        try
        {
            var frame = window.CaptureRenderedFrame()
                ?? throw new InvalidOperationException(
                    "No frame was rendered. Ensure TestAppBuilder uses .UseSkia() with " +
                    "AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false }.");

            var path = Path.GetFullPath(Path.Combine(OutputDir, name + ".png"));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            frame.Save(path);
            return new Shot(path, frame);
        }
        finally
        {
            window.Close();
        }
    }

    private static string ResolveOutputDir()
    {
        var env = Environment.GetEnvironmentVariable("SCREENSHOT_OUTPUT_DIR");
        if (!string.IsNullOrWhiteSpace(env))
            return Path.GetFullPath(env);

        var fromBuild = typeof(Screenshot).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "ScreenshotOutputDir")?.Value;

        return Path.GetFullPath(fromBuild ?? Path.Combine(AppContext.BaseDirectory, "screenshots"));
    }
}
