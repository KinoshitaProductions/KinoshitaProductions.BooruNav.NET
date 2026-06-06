using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using KinoshitaProductions.BooruNav.ViewModels;
using KinoshitaProductions.BooruNav.Views;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

public class MainViewTests
{
    [AvaloniaFact]
    public void Greeting_binding_renders_the_view_model_text()
    {
        var view = new MainView { DataContext = new MainViewModel() };
        var window = new Window { Content = view };

        window.Show();
        Dispatcher.UIThread.RunJobs(); // flush layout + compiled bindings

        var textBlock = view.GetVisualDescendants().OfType<TextBlock>().Single();
        textBlock.Text.ShouldBe("Welcome to Avalonia!");
    }
}
