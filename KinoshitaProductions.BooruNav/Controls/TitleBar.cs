using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// A simple custom window title bar: a draggable strip with a title and minimize / maximize-restore
/// / close buttons. It drives the hosting <see cref="Window"/> (found via the visual root), so it
/// gracefully no-ops on single-view platforms (no window). Pair it with a window that sets
/// <c>ExtendClientAreaToDecorationsHint="True"</c> for real custom chrome; it also works standalone
/// for prototyping. Extend the behaviour in this class and the look in TitleBar.axaml.
/// </summary>
public class TitleBar : TemplatedControl
{
    private const string PartDragArea = "PART_DragArea";
    private const string PartMinimizeButton = "PART_MinimizeButton";
    private const string PartMaximizeButton = "PART_MaximizeButton";
    private const string PartCloseButton = "PART_CloseButton";

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<TitleBar, string?>(nameof(Title));

    public static readonly StyledProperty<bool> ShowWindowControlsProperty =
        AvaloniaProperty.Register<TitleBar, bool>(nameof(ShowWindowControls), defaultValue: true);

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowWindowControls
    {
        get => GetValue(ShowWindowControlsProperty);
        set => SetValue(ShowWindowControlsProperty, value);
    }

    /// <summary>The window this title bar belongs to, or null on single-view platforms.</summary>
    private Window? HostWindow => TopLevel.GetTopLevel(this) as Window;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find<Control>(PartDragArea) is { } dragArea)
            dragArea.PointerPressed += OnDragAreaPressed;

        Wire(e, PartMinimizeButton, OnMinimize);
        Wire(e, PartMaximizeButton, OnMaximizeRestore);
        Wire(e, PartCloseButton, OnClose);
    }

    private static void Wire(TemplateAppliedEventArgs e, string name, EventHandler<RoutedEventArgs> handler)
    {
        if (e.NameScope.Find<Button>(name) is { } button)
            button.Click += handler;
    }

    private void OnDragAreaPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.ClickCount == 2)
            ToggleMaximize();
        else
            HostWindow?.BeginMoveDrag(e);
    }

    private void OnMinimize(object? sender, RoutedEventArgs e)
    {
        if (HostWindow is { } window)
            window.WindowState = WindowState.Minimized;
    }

    private void OnMaximizeRestore(object? sender, RoutedEventArgs e) => ToggleMaximize();

    private void OnClose(object? sender, RoutedEventArgs e) => HostWindow?.Close();

    private void ToggleMaximize()
    {
        if (HostWindow is not { } window)
            return;

        window.WindowState = window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}
