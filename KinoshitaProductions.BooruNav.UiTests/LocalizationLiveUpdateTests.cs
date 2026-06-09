using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using KinoshitaProductions.BooruNav.Localization;
using Shouldly;

namespace KinoshitaProductions.BooruNav.UiTests;

// Reproduces the "texts don't update on language change" report: a OneWay binding to the localizer's
// string indexer (what {i18n:Translate} produces) should re-read when the language switches.
public class LocalizationLiveUpdateTests
{
    [AvaloniaFact]
    public void Indexer_binding_refreshes_when_language_changes()
    {
        var originalUi = CultureInfo.CurrentUICulture;
        var originalDefault = CultureInfo.DefaultThreadCurrentUICulture;
        try
        {
            var loc = new LocalizationService();
            loc.SetLanguage(CultureInfo.GetCultureInfo("en-US"));

            var textBlock = new TextBlock();
            textBlock.Bind(TextBlock.TextProperty,
                new Binding("[Welcome_Finish]") { Source = loc, Mode = BindingMode.OneWay });

            var window = new Window { Content = textBlock };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            textBlock.Text.ShouldBe("Get started");

            loc.SetLanguage(CultureInfo.GetCultureInfo("es-MX"));
            Dispatcher.UIThread.RunJobs();
            textBlock.Text.ShouldBe("Comenzar");
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalUi;
            CultureInfo.DefaultThreadCurrentUICulture = originalDefault;
        }
    }
}
