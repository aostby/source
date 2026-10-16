using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers;

/// <summary>
/// Movie use cases for the UI: browse the local library, search OMDb, and import into the library.
/// Forms talk to this class only; they never touch LiteDB or OMDb directly.
/// </summary>
public sealed partial class MovieController(
    IMovieRepository repository,
    IMovieInfoProvider infoProvider,
    IFileItemRepository files,
    ITmdbLinks tmdb,
    IPosterProvider posters,
    IMediaServerLibrary plex,
    ILogger<MovieController> logger,
    IWatchListRepository? watchlists = null)
{
    /// <summary>
    /// The titles you have watched: played to the end on Plex (for the token's user; reloaded from Plex first when
    /// <paramref name="refreshPlex"/>), and marked watched on a Kino watchlist. <c>Note</c> says when Plex couldn't
    /// be asked (not set up, or offline); the watchlists are used anyway.
    /// </summary>
    public async Task<(IReadOnlySet<string> ImdbIds, string? Note)> GetWatchedAsync(bool refreshPlex = false, CancellationToken ct = default)
    {
        var watched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string? note = null;
        if (!plex.IsConfigured)
        {
            note = "Plex isn't set up in Settings, so only watchlists say what's watched";
        }
        else
        {
            try
            {
                if (refreshPlex) await plex.RefreshAsync(ct).ConfigureAwait(false);
                watched.UnionWith(await plex.GetWatchedImdbIdsAsync(ct).ConfigureAwait(false));
            }
            // Any Plex trouble (offline, a 500, a cut-off XML answer) leaves the watchlists to say what's watched.
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Watched movies could not be read from Plex");
                note = $"Plex didn't answer ({ex.Message}), so only watchlists say what's watched";
            }
        }

        if (watchlists is not null)
            foreach (var list in await watchlists.GetListNamesAsync(ct).ConfigureAwait(false))
                watched.UnionWith((await watchlists.GetItemsAsync(list, ct).ConfigureAwait(false)).Where(i => i.Watched == "Y").Select(i => i.ImdbId));

        return (watched, note);
    }

    /// <summary>IMDb ids neither Plex nor TMDb had a poster for, so a list of them doesn't ask again this session.</summary>
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _noPosterElsewhere = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Everything known about the title, for the details window: the library entry, or else OMDb's full record
    /// (not stored). Null when neither has it.
    /// </summary>
    public async Task<Item?> GetDetailsAsync(string imdbId, CancellationToken ct = default) =>
        await repository.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false)
        ?? await infoProvider.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);

    /// <summary>The library entry, or null if the title isn't in the library. Never asks OMDb.</summary>
    public Task<Item?> GetFromLibraryAsync(string imdbId, CancellationToken ct = default) =>
        repository.GetByImdbIdAsync(imdbId, ct);

    /// <summary>
    /// The poster for an IMDb id, for pages that only have the id. The image cache is tried first, so a poster seen
    /// before costs no lookup; otherwise the poster address comes from the library entry or OMDb.
    /// Null if there is none or it can't be fetched right now.
    /// </summary>
    public async Task<byte[]?> GetPosterAsync(string imdbId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        // Without a poster address the provider only looks in the cache.
        if (await posters.GetPosterAsync(new Item { ImdbId = imdbId }, ct).ConfigureAwait(false) is { } cached) return cached;
        try
        {
            return await GetPosterAsync(await GetDetailsAsync(imdbId, ct).ConfigureAwait(false) ?? new Item { ImdbId = imdbId }, ct).ConfigureAwait(false);
        }
        catch (MovieInfoUnavailableException ex)
        {
            logger.LogWarning("No poster for {ImdbId}: {Reason}", imdbId, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// The poster, or null if there is none or it can't be fetched right now. When the item's own poster is missing
    /// or gone (OMDb's links to Amazon's image server expire), Plex's poster is used, else TMDb's, as SilverScreen's
    /// poster fix did with Plex. The one found is kept in the image cache, so it is looked up only once.
    /// </summary>
    public async Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            if (await posters.GetPosterAsync(item, ct).ConfigureAwait(false) is { } poster) return poster;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Poster download failed for {ImdbId}", item.ImdbId);
        }
        return await GetPosterElsewhereAsync(item.ImdbId, ct).ConfigureAwait(false);
    }

    private async Task<byte[]?> GetPosterElsewhereAsync(string? imdbId, CancellationToken ct)
    {
        // Only real ids: series episodes use made-up ones (tt…_S01E02) for their stills.
        if (ImdbIdIn(imdbId) is not { } id || id != imdbId?.Trim().ToLowerInvariant() || _noPosterElsewhere.ContainsKey(id)) return null;

        foreach (var (source, find) in new (string, Func<Task<string?>>)[]
                 {
                     ("Plex", async () => plex.IsConfigured ? await plex.FindPosterAsync(id, ct).ConfigureAwait(false) : null),
                     ("TMDb", () => tmdb.FindPosterAsync(id, ct)),
                 })
        {
            try
            {
                if (await find().ConfigureAwait(false) is not { Length: > 0 } url || url == "N/A") continue;
                // Downloads it and keeps it in the image cache under the IMDb id.
                if (await posters.GetPosterAsync(new Item { ImdbId = imdbId!, Poster = url }, ct).ConfigureAwait(false) is { } found)
                {
                    logger.LogInformation("Poster for {ImdbId} from {Source}", imdbId, source);
                    return found;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException { CancellationToken.IsCancellationRequested: false } or InvalidOperationException)
            {
                logger.LogWarning("No poster for {ImdbId} from {Source}: {Reason}", imdbId, source, ex.Message);
            }
        }
        _noPosterElsewhere[id] = true;
        return null;
    }

    /// <summary>
    /// The TMDb page for the IMDb id; if TMDb doesn't know it (or can't be asked), a TMDb search for the title.
    /// </summary>
    public async Task<string> GetTmdbPageAsync(string imdbId, string? title, CancellationToken ct = default)
    {
        try
        {
            if (await tmdb.FindPageAsync(imdbId, ct).ConfigureAwait(false) is { } page) return page;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "TMDb lookup for {ImdbId} failed; opening a search instead", imdbId);
        }
        return $"https://www.themoviedb.org/search?query={Uri.EscapeDataString(title ?? imdbId)}";
    }

    /// <summary>
    /// The file (or, for a series, the folder) the entry is linked to, or null. SilverScreen also kept the path in
    /// TomatoUrl, which is used when there is no link.
    /// </summary>
    public async Task<string?> GetLinkedPathAsync(string imdbId, CancellationToken ct = default)
    {
        if (await files.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } link) return link.FullName;
        var item = await repository.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);
        return item?.TomatoUrl is { Length: > 0 } path && path != "N/A" && Path.IsPathRooted(path) ? path : null;
    }

    public Task<IReadOnlyList<Item>> GetLocalAsync(string? type = null, CancellationToken ct = default) =>
        repository.GetAllAsync(type, ct);

    /// <summary>
    /// Titles containing the text, or the whole library for none. An IMDb id (tt33510831, or an IMDb title link)
    /// finds that entry instead, or nothing when it isn't in the library.
    /// </summary>
    public async Task<IReadOnlyList<Item>> SearchLocalAsync(string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return await repository.GetAllAsync(ct: ct).ConfigureAwait(false);
        if (ImdbIdIn(text) is { } imdbId)
            return await repository.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } item ? [item] : [];
        return await repository.SearchByTitleAsync(text.Trim(), ct).ConfigureAwait(false);
    }

    /// <summary>OMDb's search for the text; an IMDb id (or IMDb title link) looks up that one title.</summary>
    public async Task<IReadOnlyList<SearchItem>> SearchOnlineAsync(string query, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        if (ImdbIdIn(query) is not { } imdbId) return await infoProvider.SearchAsync(query.Trim(), ct: ct).ConfigureAwait(false);

        return await infoProvider.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } item
            ? [new SearchItem { ImdbId = item.ImdbId, Title = item.Title, Year = item.Year, Type = item.Type, Poster = item.Poster }]
            : [];
    }

    /// <summary>
    /// The IMDb id when the text is one ("tt33510831", any case) or an IMDb title link
    /// (https://www.imdb.com/title/tt33510831/…); otherwise null, so it's searched as a title.
    /// </summary>
    public static string? ImdbIdIn(string? text)
    {
        var match = ImdbIdPattern().Match(text?.Trim() ?? string.Empty);
        return match.Success ? match.Groups[1].Value.ToLowerInvariant() : null;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^(?:\S*imdb\.com/(?:[a-z-]+/)?title/)?(tt\d{7,10})(?:[/?#]\S*)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)]
    private static partial System.Text.RegularExpressions.Regex ImdbIdPattern();

    /// <summary>
    /// Fetches full details from OMDb and stores them locally.
    /// Returns the stored item, or null if OMDb did not recognise the id.
    /// </summary>
    public async Task<Item?> ImportAsync(string imdbId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);

        var item = await infoProvider.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);
        if (item is null)
        {
            logger.LogWarning("OMDb returned no item for {ImdbId}", imdbId);
            return null;
        }

        var inserted = await repository.UpsertAsync(item, ct).ConfigureAwait(false);
        logger.LogInformation("{Action} {ImdbId} ({Title})", inserted ? "Imported" : "Updated", item.ImdbId, item.Title);
        return item;
    }

    /// <summary>
    /// Removes the entry from the library, and its file/folder link so nothing points to a missing entry.
    /// Files on disk are not touched. Returns false if the entry wasn't there.
    /// </summary>
    public async Task<bool> RemoveFromLibraryAsync(string imdbId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        var removed = await repository.DeleteAsync(imdbId, ct).ConfigureAwait(false);
        var unlinked = await files.DeleteAsync(imdbId, ct).ConfigureAwait(false);
        logger.LogInformation("Removed {ImdbId} from the library{Link}", imdbId, unlinked ? " with its file link" : "");
        return removed;
    }

    /// <summary>
    /// Sets the library entry's IMDb rating by hand (when OMDb/TMDb had none), also in its list of ratings, which
    /// the details show. Null clears it ("N/A"). Returns the updated entry, or null if it isn't in the library.
    /// </summary>
    public async Task<Item?> SetImdbRatingAsync(string imdbId, double? rating, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imdbId);
        if (rating is < 0 or > 10) throw new ArgumentOutOfRangeException(nameof(rating), rating, "An IMDb rating is from 0 to 10.");
        if (await repository.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is not { } item) return null;

        var text = rating is { } r ? Math.Round(r, 1, MidpointRounding.AwayFromZero).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) : "N/A";
        item.ImdbRating = text;
        const string source = "Internet Movie Database";
        var ratings = (item.Ratings ?? []).Where(x => x.Source != source).ToList();
        if (rating is not null) ratings.Insert(0, new Rating { Source = source, Value = $"{text}/10" });
        item.Ratings = ratings;

        await repository.UpsertAsync(item, ct).ConfigureAwait(false);
        logger.LogInformation("Set the IMDb rating of {ImdbId} ({Title}) to {Rating} by hand", imdbId, item.Title, text);
        return item;
    }

    /// <summary>
    /// TMDb's score for the title (its users' average, 0–10, rounded to one decimal) and the number of votes, to fill in
    /// when IMDb's own rating is missing. Null when TMDb has none or can't be asked (no key, offline).
    /// </summary>
    public async Task<(double Rating, int Votes)?> GetTmdbRatingAsync(string imdbId, CancellationToken ct = default)
    {
        try
        {
            return await tmdb.FindRatingAsync(imdbId, ct).ConfigureAwait(false) is { } score
                ? (Math.Round(score.Rating, 1, MidpointRounding.AwayFromZero), score.Votes)
                : null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning("No TMDb rating for {ImdbId}: {Reason}", imdbId, ex.Message);
            return null;
        }
    }

    /// <summary>"7.3", "7,3" or "7.3/10" as 7.3; null for anything else, or outside 0–10.</summary>
    public static double? ParseImdbRating(string? text)
    {
        var value = (text ?? string.Empty).Trim();
        if (value.EndsWith("/10", StringComparison.Ordinal)) value = value[..^3].Trim();
        return double.TryParse(value.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var r)
               && r is >= 0 and <= 10 ? Math.Round(r, 1, MidpointRounding.AwayFromZero) : null;
    }
}
