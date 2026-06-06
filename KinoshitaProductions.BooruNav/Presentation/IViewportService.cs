using System;

namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Single source of truth for the requested UI presentation. Resolve from DI and call
/// <see cref="Request(Viewport)"/> from anywhere (a debug menu, a screenshot harness, …);
/// changes are applied on the UI thread and broadcast as <see cref="ViewportChanged"/>.
/// </summary>
public interface IViewportService
{
    /// <summary>The currently applied viewport. Read on the UI thread.</summary>
    Viewport Current { get; }

    /// <summary>Request a new viewport. Thread-safe; the change lands on the UI thread.</summary>
    void Request(Viewport viewport);

    /// <summary>
    /// Request a viewport derived from the current one, e.g. <c>v =&gt; v with { Mode = FormFactor.Mobile }</c>.
    /// The mutator runs on the UI thread against a consistent snapshot of <see cref="Current"/>.
    /// </summary>
    void Request(Func<Viewport, Viewport> mutate);
}
