using OMDbApiNet.Model;

namespace Kolibri.Kino.Core.Abstractions;

/// <summary>
/// The movies a media server (Plex) already knows, with IMDb ids. Free to query, so it's tried before TMDb/OMDb.
/// </summary>
public interface IMediaServerLibrary
{
    /// <summary>False when no server/token is configured; lookups then return nothing.</summary>
    bool IsConfigured { get; }

    /// <summary>Reloads the movie list from the server (done at the start of each scan).</summary>
    Task RefreshAsync(CancellationToken ct = default);

    Task<Item?> FindByImdbIdAsync(string imdbId, CancellationToken ct = default);

    /// <summary>A movie with exactly this title (or original title), released in <paramref name="year"/> ±1 when given.</summary>
    Task<Item?> FindMovieByTitleAsync(string title, int? year, CancellationToken ct = default);

    /// <summary>Movies whose title contains <paramref name="text"/>, for manual lookup.</summary>
    Task<IReadOnlyList<Item>> SearchAsync(string text, CancellationToken ct = default);
}
