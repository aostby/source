using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>TMDb's Discover: titles by keyword and filters, a page (20) at a time, with their IMDb ids.</summary>
public interface IDiscoverSource
{
    /// <summary>The page; empty when there's no TMDb key.</summary>
    Task<DiscoverPage> DiscoverAsync(DiscoverRequest request, CancellationToken ct = default);

    /// <summary>TMDb's genres for movies, or for TV shows.</summary>
    Task<IReadOnlyList<DiscoverGenre>> GetGenresAsync(bool series, CancellationToken ct = default);
}

/// <summary>Discover pages kept for a while, by request, so going back and forth doesn't ask TMDb every time.</summary>
public interface IDiscoverCache
{
    Task<(DiscoverPage Page, DateTime FetchedUtc)?> LoadAsync(string key, CancellationToken ct = default);

    Task SaveAsync(string key, DiscoverPage page, DateTime fetchedUtc, CancellationToken ct = default);
}
