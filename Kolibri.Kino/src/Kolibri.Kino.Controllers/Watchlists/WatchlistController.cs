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

/// <summary>A list name, and where a list of that name is: a Kino watchlist, a Plex playlist, or both.</summary>
public sealed record WatchlistChoice(string Name, bool InKino, bool OnPlex)
{
    public override string ToString() => Name;
}

/// <param name="Choices">Kino's watchlists and Plex's video playlists, by name.</param>
/// <param name="PlexError">Why Plex's playlists couldn't be read, if they couldn't (then only Kino's are listed).</param>
public sealed record WatchlistChoices(IReadOnlyList<WatchlistChoice> Choices, string? PlexError);

/// <summary>What "Sync from Plex playlist" did to a Kino watchlist.</summary>
/// <param name="PlaylistFound">False when Plex had no playlist of that name: then it was made (<paramref name="CreatedOnPlex"/>).</param>
/// <param name="Added">Titles from the playlist that weren't on the watchlist and now are.</param>
/// <param name="Moved">Of those, titles that were on another Kino watchlist ("Title (from List)"); a title is on one list at a time.</param>
/// <param name="MarkedWatched">Titles marked watched because they're watched on Plex.</param>
/// <param name="Skipped">Playlist titles without an IMDb id (e.g. episodes), which a watchlist can't hold.</param>
/// <param name="Failed">Titles that couldn't be added, with why.</param>
public sealed record SyncFromPlexResult(
    bool PlaylistFound, IReadOnlyList<string> Added, IReadOnlyList<string> Moved, int MarkedWatched,
    IReadOnlyList<string> Skipped, IReadOnlyList<string> Failed, PlaylistSyncResult? CreatedOnPlex = null);

/// <summary>One Plex playlist brought into Kino at startup.</summary>
/// <param name="Added">Movies put on the new watchlist of the playlist's name.</param>
/// <param name="ToWatched">Movies watched on Plex, put on Watched (marked watched) instead.</param>
/// <param name="Skipped">Titles left out: already on another Kino watchlist, no IMDb id (episodes), or no details.</param>
public sealed record ImportedPlaylist(string Name, int Added, int ToWatched, IReadOnlyList<string> Skipped);

/// <summary>What bringing Plex's playlists into Kino did; lists that would be empty aren't made.</summary>
public sealed record PlaylistImportResult(IReadOnlyList<ImportedPlaylist> Lists, string? PlexError)
{
    public int Added => Lists.Sum(l => l.Added);
    public int ToWatched => Lists.Sum(l => l.ToWatched);
    public int Skipped => Lists.Sum(l => l.Skipped.Count);
}

/// <summary>What "move watched movies to one list" did.</summary>
/// <param name="Moved">"Title (from List)" for each watched movie moved to the list.</param>
/// <param name="AlreadyThere">Watched movies that were on the list already.</param>
/// <param name="AddedFromPlex">Movies watched on Plex that weren't on any watchlist, now on the list.</param>
/// <param name="MarkedFromPlex">Movies on a watchlist marked watched here because they're watched on Plex.</param>
/// <param name="Failed">Movies watched on Plex that couldn't be added, with why.</param>
/// <param name="PlexMarked">Movies marked watched on Plex now.</param>
/// <param name="PlexAlreadyWatched">Movies Plex had as watched already.</param>
/// <param name="NotOnPlex">Movies Plex doesn't have (or Plex isn't set up: then all of them).</param>
/// <param name="PlexError">Why Plex stopped, if it did (the Kino part is done).</param>
public sealed record MoveWatchedResult(
    string ListName, IReadOnlyList<string> Moved, int AlreadyThere, IReadOnlyList<string> AddedFromPlex, int MarkedFromPlex,
    IReadOnlyList<string> Failed, int PlexMarked, int PlexAlreadyWatched, IReadOnlyList<string> NotOnPlex, string? PlexError);

/// <param name="Kino">What adding to the Kino watchlist did, if asked.</param>
/// <param name="Plex">What adding to the Plex playlist did, if asked.</param>
/// <param name="PlexError">Why adding to Plex failed, if it did (the Kino part is kept).</param>
public sealed record AddToListsResult(string Title, AddToWatchlistResult? Kino, PlaylistSyncResult? Plex, string? PlexError);

/// <summary>
/// Watchlists (port of SilverScreen's WatchListController and WatchlistForm): movies to watch, per named list,
/// optionally copied to a Plex playlist of the same name.
/// </summary>
public sealed class WatchlistController(
    IWatchListRepository watchlists,
    IFileItemRepository files,
    ISettingsStore settings,
    IMediaServerPlaylists plex,
    IMediaServerLibrary plexMovies,
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

    /// <summary>
    /// Every list name, Kino's and Plex's, saying where each is, so the Add to watchlist dialog can tell; SilverScreen
    /// joined lists of the same name without saying. Plex is skipped (with <see cref="WatchlistChoices.PlexError"/>)
    /// when it isn't set up or can't be reached.
    /// </summary>
    public async Task<WatchlistChoices> GetListChoicesAsync(CancellationToken ct = default)
    {
        var kino = await GetListNamesAsync(ct).ConfigureAwait(false);
        IReadOnlyList<string> onPlex = [];
        string? plexError = null;
        if (plex.IsConfigured)
        {
            try
            {
                onPlex = await plex.GetPlaylistNamesAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException)
            {
                plexError = ex.Message;
                logger.LogWarning(ex, "Plex playlists could not be read");
            }
        }

        var choices = kino.Concat(onPlex)
            .GroupBy(n => n.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new WatchlistChoice(g.First().Trim(),
                kino.Contains(g.Key, StringComparer.OrdinalIgnoreCase),
                onPlex.Contains(g.Key, StringComparer.OrdinalIgnoreCase)))
            .OrderBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        return new WatchlistChoices(choices, plexError);
    }

    /// <summary>
    /// Puts the movie on the Kino watchlist and/or the Plex playlist <paramref name="listName"/>, whichever is asked;
    /// each is made if missing. A Plex failure doesn't undo the Kino part.
    /// </summary>
    public async Task<AddToListsResult> AddToListsAsync(string imdbId, string listName, bool toKino, bool toPlex, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(listName);
        if (!toKino && !toPlex) throw new ArgumentException("Choose the Kino watchlist, the Plex playlist, or both.");

        var kino = toKino ? await AddAsync(imdbId, listName, ct).ConfigureAwait(false) : null;
        PlaylistSyncResult? onPlex = null;
        string? plexError = null;
        if (toPlex)
        {
            try
            {
                onPlex = await plex.AddToPlaylistAsync(listName.Trim(), [imdbId], ct).ConfigureAwait(false);
                logger.LogInformation("Added {ImdbId} to Plex playlist {List}: {Result}", imdbId, listName, onPlex);
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException)
            {
                plexError = ex.Message;
                logger.LogWarning(ex, "Adding {ImdbId} to Plex playlist {List} failed", imdbId, listName);
            }
        }

        var title = kino?.Title ?? (await linker.FindByImdbIdAsync(imdbId, usePlex: true, ct).ConfigureAwait(false))?.Title ?? imdbId;
        return new AddToListsResult(title, kino, onPlex, plexError);
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

    /// <summary>The file the title is linked to in the library, or null.</summary>
    public async Task<string?> GetFilePathAsync(string imdbId, CancellationToken ct = default) =>
        (await files.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false))?.FullName;

    public Task<bool> SetWatchedAsync(string imdbId, bool watched, CancellationToken ct = default) =>
        watchlists.SetWatchedAsync(imdbId, watched, ct);

    /// <summary>
    /// Mark watched: the movie is marked watched and moved to <see cref="WatchedList"/> (made if missing), keeping its
    /// other fields, where watched movies are kept. Returns the list it came from, or null if it was on Watched already
    /// (or isn't on a watchlist).
    /// </summary>
    public async Task<string?> MarkWatchedAndMoveAsync(string imdbId, CancellationToken ct = default)
    {
        if (await watchlists.GetAsync(imdbId, ct).ConfigureAwait(false) is not { } item) return null;
        await watchlists.SetWatchedAsync(imdbId, true, ct).ConfigureAwait(false);
        if (string.Equals(item.WatchListName, WatchedList, StringComparison.OrdinalIgnoreCase)) return null;

        var from = item.WatchListName;
        item.Watched = "Y";
        item.WatchListName = WatchedList;
        await watchlists.UpsertAsync(item, ct).ConfigureAwait(false);
        logger.LogInformation("Marked {ImdbId} watched and moved it from {From} to {List}", imdbId, from, WatchedList);
        return from;
    }

    public const string WatchedList = "Watched";

    /// <summary>
    /// At startup (Plex set up): Plex playlists with no Kino watchlist of that name become watchlists, as SilverScreen kept
    /// them in step. Lists already in Kino are left alone, and so is Plex. A title already on another Kino watchlist is
    /// left out (a title is on one Kino list at a time), as are titles without an IMDb id. A title watched on Plex goes on
    /// <see cref="WatchedList"/>, marked watched. A list that would be empty isn't made.
    /// </summary>
    public async Task<PlaylistImportResult> ImportPlexPlaylistsAsync(CancellationToken ct = default)
    {
        if (!plex.IsConfigured) return new PlaylistImportResult([], null);
        IReadOnlyList<string> onPlex;
        try
        {
            onPlex = await plex.GetPlaylistNamesAsync(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException
                                       or TaskCanceledException && !ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Plex playlists could not be read");
            return new PlaylistImportResult([], ex.Message);
        }

        var inKino = (await watchlists.GetListNamesAsync(ct).ConfigureAwait(false)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newLists = onPlex.Select(n => n.Trim()).Where(n => n.Length > 0 && !inKino.Contains(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (newLists.Count == 0) return new PlaylistImportResult([], null);

        // Which Kino list each title is on now (a title is on one at a time).
        var onAList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in inKino)
            foreach (var item in await watchlists.GetItemsAsync(name, ct).ConfigureAwait(false))
                onAList.TryAdd(item.ImdbId, name);

        var imported = new List<ImportedPlaylist>();
        string? plexError = null;
        foreach (var listName in newLists)
        {
            IReadOnlyList<PlaylistEntry>? entries;
            try
            {
                entries = await plex.GetPlaylistEntriesAsync(listName, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or System.Xml.XmlException
                                           or TaskCanceledException && !ct.IsCancellationRequested)
            {
                plexError = ex.Message;
                logger.LogWarning(ex, "Plex playlist {List} could not be read", listName);
                continue;
            }

            int added = 0, toWatched = 0;
            var skipped = new List<string>();
            foreach (var entry in entries ?? [])
            {
                ct.ThrowIfCancellationRequested();
                if (entry.ImdbId is not { } imdbId)
                {
                    skipped.Add($"{entry.Title} (no IMDb id)");
                    continue;
                }
                if (onAList.TryGetValue(imdbId, out var already))
                {
                    skipped.Add($"{entry.Title} (on {already})");
                    continue;
                }

                var target = entry.Watched ? WatchedList : listName;
                try
                {
                    await AddAsync(imdbId, target, ct).ConfigureAwait(false);
                }
                catch (InvalidOperationException ex)
                {
                    skipped.Add($"{entry.Title} ({ex.Message})");
                    continue;
                }
                onAList[imdbId] = target;
                if (entry.Watched)
                {
                    await watchlists.SetWatchedAsync(imdbId, true, ct).ConfigureAwait(false);
                    toWatched++;
                }
                else added++;
            }

            if (added + toWatched == 0 && skipped.Count == 0) continue; // an empty playlist: nothing to report
            imported.Add(new ImportedPlaylist(listName, added, toWatched, skipped));
            logger.LogInformation("Plex playlist {List} brought into Kino: {Added} added, {ToWatched} to {Watched}, {Skipped} left out ({Why})",
                listName, added, toWatched, WatchedList, skipped.Count, string.Join("; ", skipped));
        }
        return new PlaylistImportResult(imported, plexError);
    }

    /// <summary>
    /// Every movie watched anywhere ends up on <paramref name="listName"/> (made if missing), marked watched in Kino and
    /// on Plex: movies marked watched on a Kino watchlist, and movies watched on Plex (marked here; added to the list if
    /// on no watchlist yet). A movie on a watchlist is moved, keeping all its fields, since a movie is on one Kino list
    /// at a time. Plex playlists are left as they are (they can hold a movie that's on others), so only the watched mark
    /// is set there, where Plex has the movie and hasn't got it as watched.
    /// </summary>
    public async Task<MoveWatchedResult> MoveWatchedToListAsync(string listName = WatchedList, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(listName);
        listName = listName.Trim();
        await watchlists.CreateListAsync(listName, ct).ConfigureAwait(false);

        var all = new List<WatchListItem>();
        foreach (var name in await watchlists.GetListNamesAsync(ct).ConfigureAwait(false))
            all.AddRange(await watchlists.GetItemsAsync(name, ct).ConfigureAwait(false));
        var onPlexWatched = plexMovies.IsConfigured
            ? new HashSet<string>(await plexMovies.GetWatchedImdbIdsAsync(ct).ConfigureAwait(false), StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Watched on Plex, on a watchlist, not marked here yet: mark it.
        var markedFromPlex = 0;
        foreach (var item in all.Where(i => i.Watched != "Y" && onPlexWatched.Contains(i.ImdbId)))
        {
            await watchlists.SetWatchedAsync(item.ImdbId, true, ct).ConfigureAwait(false);
            item.Watched = "Y";
            markedFromPlex++;
        }
        var watched = all.Where(i => i.Watched == "Y").ToList();

        var moved = new List<string>();
        var alreadyThere = 0;
        foreach (var item in watched)
        {
            ct.ThrowIfCancellationRequested();
            if (string.Equals(item.WatchListName, listName, StringComparison.OrdinalIgnoreCase))
            {
                alreadyThere++;
                continue;
            }
            moved.Add($"{item.Title} (from {item.WatchListName})");
            item.WatchListName = listName;
            await watchlists.UpsertAsync(item, ct).ConfigureAwait(false); // the stored document keeps every other field
        }
        logger.LogInformation("Moved {Count} watched movie(s) to watchlist {List}: {Movies}", moved.Count, listName, string.Join("; ", moved));

        // Watched on Plex, on no watchlist: added to the list (details from the library or Plex), watched.
        var onAList = all.Select(i => i.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var addedFromPlex = new List<string>();
        var failed = new List<string>();
        foreach (var imdbId in onPlexWatched.Where(id => !onAList.Contains(id)).Order(StringComparer.Ordinal))
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var added = await AddAsync(imdbId, listName, ct).ConfigureAwait(false);
                await watchlists.SetWatchedAsync(imdbId, true, ct).ConfigureAwait(false);
                addedFromPlex.Add(added.Title);
            }
            catch (InvalidOperationException ex)
            {
                failed.Add($"{imdbId}: {ex.Message}");
            }
        }
        logger.LogInformation("Added {Count} movie(s) watched on Plex to watchlist {List}; {Marked} marked watched from Plex, {Failed} failed",
            addedFromPlex.Count, listName, markedFromPlex, failed.Count);

        int marked = 0, alreadyWatched = 0;
        var notOnPlex = new List<string>();
        string? plexError = null;
        if (!plexMovies.IsConfigured) notOnPlex.AddRange(watched.Select(i => i.Title));
        else
        {
            foreach (var item in watched)
            {
                try
                {
                    switch (await plexMovies.MarkWatchedAsync(item.ImdbId, ct).ConfigureAwait(false))
                    {
                        case MarkWatchedOutcome.Marked: marked++; break;
                        case MarkWatchedOutcome.AlreadyWatched: alreadyWatched++; break;
                        default: notOnPlex.Add(item.Title); break;
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or TaskCanceledException && !ct.IsCancellationRequested)
                {
                    plexError = ex.Message;
                    logger.LogWarning(ex, "Marking watched movies on Plex stopped at {ImdbId}", item.ImdbId);
                    break;
                }
            }
            logger.LogInformation("Plex: {Marked} marked watched, {Already} already watched, {Missing} not on Plex", marked, alreadyWatched, notOnPlex.Count);
        }
        return new MoveWatchedResult(listName, moved, alreadyThere, addedFromPlex, markedFromPlex, failed, marked, alreadyWatched, notOnPlex, plexError);
    }

    /// <summary>
    /// After "Mark watched": marks the movie watched on Plex too. Null when Plex isn't set up; otherwise a sentence
    /// for the status bar. Never throws: the watchlist is already saved.
    /// </summary>
    public async Task<string?> MarkWatchedOnPlexAsync(string imdbId, string title, CancellationToken ct = default)
    {
        if (!plexMovies.IsConfigured) return null;
        try
        {
            switch (await plexMovies.MarkWatchedAsync(imdbId, ct).ConfigureAwait(false))
            {
                case MarkWatchedOutcome.NotOnServer: return $"{title} isn't on Plex, so it's watched in Kino only.";
                case MarkWatchedOutcome.AlreadyWatched: return "Plex had it as watched already.";
                default:
                    logger.LogInformation("Marked {ImdbId} watched on Plex", imdbId);
                    return "Also marked watched on Plex.";
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Marking {ImdbId} watched on Plex failed", imdbId);
            return $"Not marked on Plex: {ex.Message}";
        }
    }

    /// <summary>Removes the movie from its list and, if asked, from the Plex playlist of that name. Returns whether Plex had it.</summary>
    public async Task<bool> RemoveAsync(string imdbId, string listName, bool alsoFromPlex, CancellationToken ct = default)
    {
        await watchlists.RemoveAsync(imdbId, ct).ConfigureAwait(false);
        return alsoFromPlex && plex.IsConfigured && await plex.RemoveFromPlaylistAsync(listName, imdbId, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// The other way from <see cref="CopyToPlexAsync"/>: titles in the Plex playlist of the same name that aren't on the
    /// watchlist are added, and titles watched on Plex are marked watched here. A title watched here stays watched,
    /// and titles only on the watchlist are left alone. Without such a playlist on Plex, it's made from this watchlist
    /// (the movies Plex has), as Copy to Plex playlist does.
    /// </summary>
    public async Task<SyncFromPlexResult> SyncFromPlexAsync(string listName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(listName);
        var entries = await plex.GetPlaylistEntriesAsync(listName, ct).ConfigureAwait(false);
        if (entries is null)
        {
            var created = await CopyToPlexAsync(listName, ct).ConfigureAwait(false);
            logger.LogInformation("Plex had no playlist {List}; made it with {Added} movie(s), {Missing} not on Plex", listName, created.Added, created.NotOnServer.Count);
            return new SyncFromPlexResult(false, [], [], 0, [], [], created);
        }

        var onList = (await watchlists.GetItemsAsync(listName, ct).ConfigureAwait(false))
            .Where(i => !string.IsNullOrEmpty(i.ImdbId))
            .ToDictionary(i => i.ImdbId, StringComparer.OrdinalIgnoreCase);
        List<string> added = [], moved = [], skipped = [], failed = [];
        var markedWatched = 0;

        foreach (var entry in entries)
        {
            ct.ThrowIfCancellationRequested();
            if (entry.ImdbId is not { } imdbId)
            {
                skipped.Add(entry.Title);
                continue;
            }

            var wasWatched = false;
            if (onList.TryGetValue(imdbId, out var item))
            {
                wasWatched = item.Watched == "Y";
            }
            else
            {
                try
                {
                    var result = await AddAsync(imdbId, listName, ct).ConfigureAwait(false);
                    added.Add(result.Title);
                    if (result.MovedFrom is not null) moved.Add($"{result.Title} (from {result.MovedFrom})");
                }
                catch (InvalidOperationException ex)
                {
                    failed.Add($"{entry.Title}: {ex.Message}");
                    continue;
                }
            }

            if (!entry.Watched || wasWatched) continue;
            await watchlists.SetWatchedAsync(imdbId, true, ct).ConfigureAwait(false);
            markedWatched++;
        }

        logger.LogInformation("Synced watchlist {List} from Plex: {Added} added, {Watched} marked watched, {Skipped} skipped, {Failed} failed",
            listName, added.Count, markedWatched, skipped.Count, failed.Count);
        return new SyncFromPlexResult(true, added, moved, markedWatched, skipped, failed);
    }

    /// <summary>The Plex playlist of this name in Plex Web, or null when Plex has none (or isn't set up).</summary>
    public async Task<string?> GetPlexPlaylistPageAsync(string listName, CancellationToken ct = default) =>
        plex.IsConfigured ? await plex.GetPlaylistWebPageAsync(listName, ct).ConfigureAwait(false) : null;

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
