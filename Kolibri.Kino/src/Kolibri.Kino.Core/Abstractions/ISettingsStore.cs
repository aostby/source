using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// The user's settings in the database (shared with SilverScreen). Loaded once and cached.
/// </summary>
public interface ISettingsStore
{
    /// <summary>The current settings. Treat as read-only; change a <see cref="UserSettings.Clone"/> and save it.</summary>
    UserSettings Current { get; }

    /// <summary>
    /// Saves <paramref name="settings"/>. Fields Kino doesn't know are kept; empty values are not written,
    /// so SilverScreen keeps its defaults for them.
    /// </summary>
    Task SaveAsync(UserSettings settings, CancellationToken ct = default);

    /// <summary>Raised after <see cref="SaveAsync"/>, e.g. so clients using a changed API key are rebuilt.</summary>
    event EventHandler? Changed;
}
