using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>One service's test.</summary>
/// <param name="Missing">
/// What the service needs that isn't filled in ("API key", "server name and token"), so it wasn't tried; null when
/// nothing is missing. <paramref name="Ok"/> then says whether the service is optional.
/// </param>
public sealed record ConnectionCheck(string Service, bool Ok, string Message, string? Missing = null);

/// <summary>
/// Checks API keys and the Plex token in a settings object before it is saved.
/// </summary>
public interface IConnectionTester
{
    Task<IReadOnlyList<ConnectionCheck>> TestAsync(UserSettings settings, CancellationToken ct = default);
}
