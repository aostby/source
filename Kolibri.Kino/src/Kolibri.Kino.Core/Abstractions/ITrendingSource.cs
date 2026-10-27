using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>What's trending on TMDb this week: one page of each list (TMDb gives 20 a page).</summary>
public interface ITrendingSource
{
    /// <summary>
    /// The list, in TMDb's order, with the IMDb id of each movie or series (one request each). Ids in
    /// <paramref name="knownImdbIds"/> (TMDb id → IMDb id, from the last fetch) aren't asked for again.
    /// Empty when there's no TMDb key.
    /// </summary>
    Task<IReadOnlyList<TrendingItem>> GetTrendingAsync(
        TrendingKind kind, IReadOnlyDictionary<int, string> knownImdbIds, CancellationToken ct = default);
}

/// <summary>The last trending list of each kind, and when it was fetched, so a visit doesn't ask TMDb every time.</summary>
public interface ITrendingCache
{
    Task<(IReadOnlyList<TrendingItem> Items, DateTime FetchedUtc)?> LoadAsync(TrendingKind kind, CancellationToken ct = default);

    Task SaveAsync(TrendingKind kind, IReadOnlyList<TrendingItem> items, DateTime fetchedUtc, CancellationToken ct = default);
}
