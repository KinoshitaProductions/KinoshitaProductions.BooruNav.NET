using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;

namespace KinoshitaProductions.BooruNav.Controls;

/// <summary>
/// Wraps a single child and, when the active <see cref="Viewport"/> is <see cref="FormFactor.Mobile"/>
/// on a desktop window (or in the previewer), letterboxes that child into an aspect-correct,
/// phone-sized box — scaling it down to fit when the window is smaller. On a real phone it is an
/// inert pass-through, so the same tree runs everywhere.
///
/// The XAML attributes (<see cref="Mode"/>, <see cref="Aspect"/>, <see cref="DeviceSize"/>,
/// <see cref="Orientation"/>) are *seeds*: on attach, and whenever they change, they are pushed to
/// the DI-registered <see cref="IViewportService"/>, which is the single source of truth. The host
/// then re-lays-out from that service (and from anyone else who calls <c>Request(...)</c>).
/// </summary>
public sealed class ViewportHost : Decorator
{
    public static readonly StyledProperty<FormFactor> ModeProperty =
        AvaloniaProperty.Register<ViewportHost, FormFactor>(nameof(Mode), FormFactor.Desktop);

    public static readonly StyledProperty<AspectRatio> AspectProperty =
        AvaloniaProperty.Register<ViewportHost, AspectRatio>(nameof(Aspect), AspectRatio.Ratio16__9);

    /// <summary>Explicit device size (DIPs). When null, size is derived from <see cref="Aspect"/>.</summary>
    public static readonly StyledProperty<Size?> DeviceSizeProperty =
        AvaloniaProperty.Register<ViewportHost, Size?>(nameof(DeviceSize));

    public static readonly StyledProperty<ScreenOrientation> OrientationProperty =
        AvaloniaProperty.Register<ViewportHost, ScreenOrientation>(nameof(Orientation), ScreenOrientation.Portrait);

    public FormFactor Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public AspectRatio Aspect { get => GetValue(AspectProperty); set => SetValue(AspectProperty, value); }
    public Size? DeviceSize { get => GetValue(DeviceSizeProperty); set => SetValue(DeviceSizeProperty, value); }
    public ScreenOrientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

    /// <summary>Long-edge length (DIPs) used when deriving a size from an aspect ratio alone.</summary>
    private const double DefaultLongEdge = 900d;

    private IViewportService? _service;
    private IMessenger? _messenger;

    static ViewportHost()
    {
        AffectsMeasure<ViewportHost>(ModeProperty, AspectProperty, DeviceSizeProperty, OrientationProperty);

        foreach (var p in new AvaloniaProperty[] { ModeProperty, AspectProperty, DeviceSizeProperty, OrientationProperty })
            p.Changed.AddClassHandler<ViewportHost>((host, _) => host.PushSeedToService());
    }

    public ViewportHost()
    {
        ClipToBounds = true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Controls can't get constructor injection (XAML news them up), so pull from the root.
        _service = App.Services?.GetService(typeof(IViewportService)) as IViewportService;
        _messenger = App.Services?.GetService(typeof(IMessenger)) as IMessenger;

        // Weak registration: the static handler captures nothing, so a forgotten unsubscribe won't leak.
        _messenger?.Register<ViewportHost, ViewportChanged>(this, static (host, msg) => host.OnViewportChanged(msg.Viewport));

        PushSeedToService();   // seed the service from the XAML-declared attributes
        InvalidateMeasure();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _messenger?.UnregisterAll(this);
        _messenger = null;
        _service = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void PushSeedToService() => _service?.Request(BuildSeedViewport());

    private Viewport BuildSeedViewport() => new(Mode, Aspect, DeviceSize, Orientation);

    // Service is authoritative; on change we only need to re-run layout.
    private void OnViewportChanged(Viewport viewport)
    {
        InvalidateMeasure();
        InvalidateArrange();
    }

    /// <summary>The viewport to render. Falls back to the local seed when there is no service (previewer).</summary>
    private Viewport Current => _service?.Current ?? BuildSeedViewport();

    // The only place a phone viewport should NOT be letterboxed is an actual phone, where the
    // single-view shell already fills the screen. Everywhere else (desktop, browser, previewer,
    // headless tests) emulation is the whole point.
    private static bool IsRealMobileDevice => OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    private bool Emulating => Current.Mode == FormFactor.Mobile && !IsRealMobileDevice;

    private Size ResolveDeviceSize()
    {
        var vp = Current;

        Size size;
        if (vp.Size is { } explicitSize)
            size = explicitSize;
        else
            size = new Size(DefaultLongEdge / vp.Aspect.Ratio, DefaultLongEdge); // portrait-natural

        // Sizes are authored portrait (w <= h); flip to honour the requested orientation.
        var wantsPortrait = vp.Orientation == ScreenOrientation.Portrait;
        var isPortrait = size.Width <= size.Height;
        if (wantsPortrait != isPortrait)
            size = new Size(size.Height, size.Width);

        return size * vp.Scaling;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var child = Child;
        if (child is null)
            return default;

        if (!Emulating)
        {
            child.Measure(availableSize);
            return child.DesiredSize;
        }

        var device = ResolveDeviceSize();
        child.Measure(device);

        // Fill the available space so there is room to letterbox; fall back to the device size if unbounded.
        var width = double.IsInfinity(availableSize.Width) ? device.Width : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? device.Height : availableSize.Height;
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var child = Child;
        if (child is null)
            return finalSize;

        if (!Emulating)
        {
            child.RenderTransform = null;
            child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        var device = ResolveDeviceSize();

        // Uniform scale to fit; never enlarge past 1:1.
        var scale = Math.Min(1d, Math.Min(finalSize.Width / device.Width, finalSize.Height / device.Height));
        if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
            scale = 1d;

        var offsetX = Math.Max(0d, (finalSize.Width - device.Width * scale) / 2d);
        var offsetY = Math.Max(0d, (finalSize.Height - device.Height * scale) / 2d);

        // Arrange the child at full device size, then scale+center it via a render transform
        // (render transforms participate in hit-testing, so input still maps correctly).
        child.Arrange(new Rect(0, 0, device.Width, device.Height));
        child.RenderTransformOrigin = RelativePoint.TopLeft;
        child.RenderTransform = new TransformGroup
        {
            Children =
            {
                new ScaleTransform(scale, scale),
                new TranslateTransform(offsetX, offsetY),
            },
        };

        return finalSize;
    }
}
