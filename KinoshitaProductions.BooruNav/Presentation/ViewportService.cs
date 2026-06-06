using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Default <see cref="IViewportService"/>. Holds the current viewport, marshals every request to
/// the UI thread (so subscribers never have to think about threading), dedupes no-op requests via
/// record-struct equality, and broadcasts changes through <see cref="IMessenger"/>.
/// </summary>
public sealed class ViewportService : IViewportService
{
    private readonly IMessenger _messenger;

    public ViewportService(IMessenger messenger) => _messenger = messenger;

    public Viewport Current { get; private set; } = Viewport.Desktop;

    public void Request(Viewport viewport)
    {
        if (Dispatcher.UIThread.CheckAccess())
            Apply(viewport);
        else
            Dispatcher.UIThread.Post(() => Apply(viewport));
    }

    public void Request(Func<Viewport, Viewport> mutate)
    {
        // Snapshot Current on the UI thread so a background caller can't race the field.
        if (Dispatcher.UIThread.CheckAccess())
            Apply(mutate(Current));
        else
            Dispatcher.UIThread.Post(() => Apply(mutate(Current)));
    }

    private void Apply(Viewport viewport)
    {
        if (viewport == Current)
            return;

        Current = viewport;
        _messenger.Send(new ViewportChanged(viewport));
    }
}
