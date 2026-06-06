namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// Sent (via <see cref="CommunityToolkit.Mvvm.Messaging.IMessenger"/>) when the user finishes or
/// skips the welcome wizard. The shell listens and navigates to the main page.
/// </summary>
public sealed record WelcomeCompleted;
