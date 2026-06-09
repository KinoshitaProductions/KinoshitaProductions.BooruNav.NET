using Avalonia.Controls;
using KinoshitaProductions.BooruNav.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// Self-contained language picker backed by the app-global ILocalizationService — it resolves its
/// own view model from the composition root (like DeviceSwitcher), so it has no dependency on the
/// host's DataContext. Drop it anywhere, e.g. a TitleBar's RightContent slot.
/// </summary>
public partial class LanguageSelector : UserControl
{
    public LanguageSelector()
    {
        InitializeComponent();

        // Null in the previewer (no composition root); the ComboBox just stays empty there.
        if (App.Services is { } services)
            DataContext = services.GetRequiredService<LanguageSelectorViewModel>();
    }
}
