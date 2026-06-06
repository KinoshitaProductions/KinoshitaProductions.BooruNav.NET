using System.Collections.Generic;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>A named <see cref="Viewport"/> for the device-switcher UI (and tests).</summary>
public sealed record DevicePreset(string Name, Viewport Viewport)
{
    public override string ToString() => Name;

    /// <summary>The presets offered by the device switcher.</summary>
    public static IReadOnlyList<DevicePreset> All { get; } = new[]
    {
        new DevicePreset("Desktop (fill)", Viewport.Desktop),
        new DevicePreset("Pixel 8", Viewport.Pixel8),
        new DevicePreset("iPhone 15", Viewport.IPhone15),
        new DevicePreset("Phone 20:9", new Viewport(FormFactor.Mobile, AspectRatio.Ratio20__9, null)),
        new DevicePreset("Phone 16:9", new Viewport(FormFactor.Mobile, AspectRatio.Ratio16__9, null)),
    };
}
