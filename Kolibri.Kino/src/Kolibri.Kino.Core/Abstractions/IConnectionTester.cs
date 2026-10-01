using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

public sealed record ConnectionCheck(string Service, bool Ok, string Message);

/// <summary>
/// Checks API keys and the Plex token in a settings object before it is saved.
/// </summary>
public interface IConnectionTester
{
    Task<IReadOnlyList<ConnectionCheck>> TestAsync(UserSettings settings, CancellationToken ct = default);
}
