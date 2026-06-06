namespace KinoshitaProductions.BooruNav.Presentation;

/// <summary>
/// How the UI should present itself. On a desktop window <see cref="Mobile"/> letterboxes the
/// content into a phone-shaped viewport (device emulation); on a real phone it is a no-op.
/// </summary>
public enum FormFactor
{
    /// <summary>Fill the available space (normal desktop/window behaviour).</summary>
    Desktop,

    /// <summary>Constrain content to a phone-sized, aspect-correct viewport.</summary>
    Mobile,
}
