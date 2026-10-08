namespace Kolibri.Kino.Core.Models;

/// <summary>The online services whose calls Kino counts (see <see cref="Abstractions.IApiUsageTracker"/>).</summary>
public static class ApiServices
{
    public const string Omdb = "OMDb";
    public const string Tmdb = "TMDb";
    public const string SubDl = "SubDL";
    public const string Plex = "Plex";

    public static IReadOnlyList<string> All { get; } = [Omdb, Tmdb, SubDl, Plex];
}

/// <summary>Calls to one service, and how many of them failed (an HTTP error such as "Request limit reached", or no answer).</summary>
public sealed record ApiCallCount(int Calls, int Errors)
{
    public static ApiCallCount Zero { get; } = new(0, 0);

    public static ApiCallCount operator +(ApiCallCount a, ApiCallCount b) => new(a.Calls + b.Calls, a.Errors + b.Errors);
}

/// <summary>One day's calls (local date) per service in <see cref="ApiServices"/>.</summary>
public sealed record ApiUsageDay(DateOnly Date, IReadOnlyDictionary<string, ApiCallCount> Services)
{
    public ApiCallCount For(string service) => Services.TryGetValue(service, out var count) ? count : ApiCallCount.Zero;
}

/// <summary>Calls to add to a day's stored count.</summary>
public sealed record ApiUsageEntry(DateOnly Date, string Service, ApiCallCount Count);
