namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Cross-cutting app state used to drive startup routing. For now just first-run detection;
/// extend as needed (last-opened, auth, …).
/// </summary>
public interface IAppState
{
    /// <summary>True until the user has completed (or skipped) the welcome wizard.</summary>
    bool IsFirstRun { get; set; }
}
