using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Shouldly;

namespace KinoshitaProductions.BooruNav.E2ETests;

/// <summary>
/// End-to-end smoke tests that drive the real <see cref="MainWindow"/> — the same window the
/// Desktop head shows — wired up exactly as <c>App.OnFrameworkInitializationCompleted</c> does
/// it. In-process and cross-platform (runs on Linux/CI with no display server).
/// </summary>
public class AppSmokeTests
{
    private static MainWindow ShowMainWindow()
    {
        var window = new MainWindow { DataContext = new MainViewModel() };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    [AvaloniaFact]
    public void Main_window_hosts_the_main_view()
    {
        var window = ShowMainWindow();

        window.GetVisualDescendants().OfType<MainView>().Count().ShouldBe(1);
        window.Title.ShouldBe("KinoshitaProductions.BooruNav");
    }

    [AvaloniaFact]
    public void Greeting_flows_end_to_end_to_the_rendered_text()
    {
        var window = ShowMainWindow();

        var greeting = window.GetVisualDescendants().OfType<TextBlock>().First();
        greeting.Text.ShouldBe("Welcome to Avalonia!");
    }
}
