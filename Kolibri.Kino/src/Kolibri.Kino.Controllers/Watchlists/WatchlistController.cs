using System.Globalization;
using System.Reflection;
using System.Text;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Watchlists;

/// <param name="MovedFrom">The list the movie was on before, if another (a movie is on one list at a time).</param>
public sealed record AddToWatchlistResult(string Title, string? MovedFrom);

/// <summary>
/// Watchlists (port of SilverScreen's WatchListController and WatchlistForm): movies to watch, per named list,
/// optionally copied to a Plex playlist of the same name.
/// </summary>
public sealed class WatchlistController(
    IWatchListRepository watchlists,
    IFileItemRepository files,
    ISettingsStore settings,
    IMediaServerPlaylists plex,
    IPosterProvider posters,
    MovieLinker linker,
    ILogger<WatchlistController> logger)
{
    public const string DefaultList = "MyMovies";

    /// <summary>The list shown first (UserSettings.FavoriteWatchList).</summary>
    public string FavoriteList => string.IsNullOrWhiteSpace(settings.Current.FavoriteWatchList) ? DefaultList : settings.Current.FavoriteWatchList;

    public bool PlexConfigured => plex.IsConfigured;

    /// <summary>All list names; the favorite is always included.</summary>
    public async Task<IReadOnlyList<string>> GetListNamesAsync(CancellationToken ct = default)
    {
        var names = (await watchlists.GetListNamesAsync(ct).ConfigureAwait(false)).ToList();
        if (!names.Contains(FavoriteList, StringComparer.OrdinalIgnoreCase)) names.Insert(0, FavoriteList);
        return names;
    }

    public Task<IReadOnlyList<WatchListItem>> GetItemsAsync(string listName, CancellationToken ct = default) =>
        watchlists.GetItemsAsync(listName, ct);

    public async Task SetFavoriteAsync(string listName, CancellationToken ct = default)
    {
        if (string.Equals(listName, settings.Current.FavoriteWatchList, StringComparison.Ordinal)) return;
        var changed = settings.Current.Clone();
        changed.FavoriteWatchList = listName;
        await settings.SaveAsync(changed, ct).ConfigureAwait(false);
    }

    public Task CreateListAsync(string listName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(listName);
        return watchlists.CreateListAsync(listName.Trim(), ct);
    }

    /// <summary>
    /// Puts the movie on <paramref name="listName"/> (details from the library, Plex or OMDb), unwatched.
    /// A movie already on another list is moved, as in SilverScreen.
    /// </summary>
    public async Task<AddToWatchlistResult> AddAsync(string imdbId, string listName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        ArgumentException.ThrowIfNullOrWhiteSpace(listName);

        var item = await linker.FindByImdbIdAsync(imdbId, usePlex: true, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"No details found for {imdbId}.");
        var previous = await watchlists.GetAsync(imdbId, ct).ConfigureAwait(false);
        var file = await files.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);

        var entry = CopyOf(item);
        entry.WatchListName = listName.Trim();
        entry.Watched = previous is not null && string.Equals(previous.WatchListName, entry.WatchListName, StringComparison.OrdinalIgnoreCase)
            ? previous.Watched
            : "N";
        entry.Trailer = $"https://www.imdb.com/title/{item.ImdbId}";
        entry.FilePath = file is null ? null : Path.GetDirectoryName(file.FullName);

        await watchlists.UpsertAsync(entry, ct).ConfigureAwait(false);
        var movedFrom = previous is not null && !string.Equals(previous.WatchListName, entry.WatchListName, StringComparison.OrdinalIgnoreCase)
            ? previous.WatchListName
            : null;
        logger.LogInformation("Added {ImdbId} to watchlist {List}{Moved}", imdbId, entry.WatchListName, movedFrom is null ? "" : $" (moved from {movedFrom})");
        return new AddToWatchlistResult(item.Title, movedFrom);
    }

    public Task<bool> SetWatchedAsync(string imdbId, bool watched, CancellationToken ct = default) =>
        watchlists.SetWatchedAsync(imdbId, watched, ct);

    /// <summary>Removes the movie from its list and, if asked, from the Plex playlist of that name. Returns whether Plex had it.</summary>
    public async Task<bool> RemoveAsync(string imdbId, string listName, bool alsoFromPlex, CancellationToken ct = default)
    {
        await watchlists.RemoveAsync(imdbId, ct).ConfigureAwait(false);
        return alsoFromPlex && plex.IsConfigured && await plex.RemoveFromPlaylistAsync(listName, imdbId, ct).ConfigureAwait(false);
    }

    /// <summary>Copies every movie on the list to the Plex playlist of the same name (created if missing).</summary>
    public async Task<PlaylistSyncResult> CopyToPlexAsync(string listName, CancellationToken ct = default)
    {
        var items = await watchlists.GetItemsAsync(listName, ct).ConfigureAwait(false);
        return await plex.AddToPlaylistAsync(listName, items.Select(i => i.ImdbId).ToList(), ct).ConfigureAwait(false);
    }

    public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => posters.GetPosterAsync(item, ct);

    /// <summary>
    /// The list as CSV (semicolon-separated, UTF-8 with BOM, so Excel opens it directly). Replaces SilverScreen's
    /// Excel-interop export, which needed Office installed.
    /// </summary>
    public static string ToCsv(IEnumerable<WatchListItem> items)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Title;Year;Genre;IMDb rating;Runtime;Watched;IMDb id;Actors;Plot");
        foreach (var i in items)
            csv.AppendLine(string.Join(';', new[] { i.Title, i.Year, i.Genre, i.ImdbRating, i.Runtime, i.Watched == "Y" ? "Yes" : "No", i.ImdbId, i.Actors, i.Plot }.Select(Field)));
        return csv.ToString();

        static string Field(string? value)
        {
            var text = value is null or "N/A" ? "" : value;
            return text.IndexOfAny([';', '"', '\n', '\r']) >= 0 ? $"\"{text.Replace("\"", "\"\"")}\"" : text;
        }
    }

    /// <summary>Every Item field copied onto a new watchlist entry (SilverScreen copied a subset).</summary>
    private static WatchListItem CopyOf(Item item)
    {
        var entry = new WatchListItem();
        foreach (var p in typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite))
            p.SetValue(entry, p.GetValue(item));
        return entry;
    }

    /// <summary>Numeric IMDb rating for sorting; unrated sorts last.</summary>
    public static double Rating(string? imdbRating) =>
        double.TryParse(imdbRating, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : -1;
}
