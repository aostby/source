namespace Kolibri.Kino.Core.Abstractions;

/// <summary>A title on the Plex account's watchlist (in Norwegian Plex: "Huskeliste").</summary>
/// <param name="Title">Plex's title.</param>
/// <param name="Year">The year, when Plex has one.</param>
/// <param name="Type">"movie" or "series".</param>
/// <param name="ImdbId">The IMDb id from Plex's guids (imdb://tt…), or null when Plex has none.</param>
/// <param name="Poster">Plex's poster address (a public image on plex.tv), or null.</param>
public sealed record PlexWatchlistItem(string Title, string? Year, string Type, string? ImdbId, string? Poster);

/// <summary>
/// The Plex account's watchlist ("Huskeliste" in Norwegian Plex; app.plex.tv → Watchlist): titles to remember for
/// later, kept on plex.tv for the account, not on the Plex server. Read with the Plex token in Settings.
/// </summary>
public interface IPlexWatchlist
{
    /// <summary>A Plex token is set (the watchlist belongs to that account).</summary>
    bool IsConfigured { get; }

    /// <summary>The whole watchlist, newest first as Plex gives it.</summary>
    Task<IReadOnlyList<PlexWatchlistItem>> GetWatchlistAsync(CancellationToken ct = default);
}
