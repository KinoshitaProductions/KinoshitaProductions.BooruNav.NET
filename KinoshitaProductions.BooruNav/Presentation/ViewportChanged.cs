namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Broadcast (via <see cref="CommunityToolkit.Mvvm.Messaging.IMessenger"/>) whenever the active
/// <see cref="Viewport"/> changes. Always raised on the UI thread by <see cref="ViewportService"/>.
/// </summary>
public sealed record ViewportChanged(Viewport Viewport);
