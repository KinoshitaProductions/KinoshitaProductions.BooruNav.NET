namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// In-memory <see cref="IAppState"/> stub: every launch is a "first run" until a persisted
/// settings store backs <see cref="IsFirstRun"/>. Registered as a DI singleton.
/// </summary>
public sealed class AppState : IAppState
{
    public bool IsFirstRun { get; set; } = true;
}
