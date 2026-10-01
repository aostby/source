using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers;

/// <summary>
/// Movie use cases for the UI: browse the local library, search OMDb, and import into the library.
/// Forms talk to this class only; they never touch LiteDB or OMDb directly.
/// </summary>
public sealed class MovieController(
    IMovieRepository repository,
    IMovieInfoProvider infoProvider,
    IFileItemRepository files,
    ITmdbLinks tmdb,
    IPosterProvider posters,
    ILogger<MovieController> logger)
{
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
        if (await GetPosterAsync(new Item { ImdbId = imdbId }, ct).ConfigureAwait(false) is { } cached) return cached;
        try
        {
            return await GetDetailsAsync(imdbId, ct).ConfigureAwait(false) is { } item
                ? await GetPosterAsync(item, ct).ConfigureAwait(false)
                : null;
        }
        catch (MovieInfoUnavailableException ex)
        {
            logger.LogWarning("No poster for {ImdbId}: {Reason}", imdbId, ex.Message);
            return null;
        }
    }

    /// <summary>The poster, or null if there is none or it can't be fetched right now.</summary>
    public async Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            return await posters.GetPosterAsync(item, ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Poster download failed for {ImdbId}", item.ImdbId);
            return null;
        }
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

    public Task<IReadOnlyList<Item>> SearchLocalAsync(string text, CancellationToken ct = default) =>
        string.IsNullOrWhiteSpace(text)
            ? repository.GetAllAsync(ct: ct)
            : repository.SearchByTitleAsync(text.Trim(), ct);

    public Task<IReadOnlyList<SearchItem>> SearchOnlineAsync(string query, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        return infoProvider.SearchAsync(query.Trim(), ct: ct);
    }

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
}
