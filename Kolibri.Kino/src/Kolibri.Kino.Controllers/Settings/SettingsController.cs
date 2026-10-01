using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kolibri.Kino.Controllers.Settings;

/// <summary>
/// Behind the Settings window (port of SilverScreen's "Innstillinger" property grid).
/// </summary>
public sealed class SettingsController(ISettingsStore settings, IConnectionTester tester, ILogger<SettingsController> logger)
{
    public const string DefaultKeysMessage =
        "You're using the shared default OMDb/TMDb keys. They work, but everyone using them shares OMDb's daily limit " +
        "(1,000 requests), so lookups can stop early. Free keys of your own: omdbapi.com/apikey.aspx and themoviedb.org/settings/api.";

    /// <summary>True while the OMDb or TMDb key is one of the shared defaults; the UI then asks for real keys.</summary>
    public bool UsesDefaultKeys => settings.Current.UsesDefaultKeys();

    /// <summary>A copy to edit; nothing changes until <see cref="SaveAsync"/>.</summary>
    public UserSettings GetEditableCopy() => settings.Current.Clone();

    /// <summary>Tries the OMDb key, TMDb key and Plex token in <paramref name="edited"/> without saving them.</summary>
    public Task<IReadOnlyList<ConnectionCheck>> TestAsync(UserSettings edited, CancellationToken ct = default) =>
        tester.TestAsync(edited, ct);

    /// <summary>Saves to the database; OMDb, TMDb and Plex use the new values from the next call on.</summary>
    public async Task SaveAsync(UserSettings edited, CancellationToken ct = default)
    {
        await settings.SaveAsync(edited, ct).ConfigureAwait(false);
        logger.LogInformation("Settings saved for {User}", edited.UserName);
    }
}
