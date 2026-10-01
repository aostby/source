using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>
/// Shared by the scan and manual lookup: find a movie by IMDb id (cheapest source first) and link it to a file.
/// </summary>
public sealed class MovieLinker(IFileItemRepository files, IMovieRepository movies, IMediaServerLibrary plex, IMovieInfoProvider omdb)
{
    /// <summary>The library, then Plex (free), then OMDb.</summary>
    public async Task<Item?> FindByImdbIdAsync(string imdbId, bool usePlex, CancellationToken ct)
    {
        if (await movies.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } local) return local;
        if (usePlex && plex.IsConfigured && await plex.FindByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } fromPlex) return fromPlex;
        return await omdb.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Links <paramref name="path"/> to the movie (replacing any earlier link for it) and stores the movie.
    /// SilverScreen keeps the file path in Item.TomatoUrl and relies on it, so that is kept in sync too.
    /// </summary>
    public async Task<FileItem> LinkAsync(Item item, string path, CancellationToken ct)
    {
        var link = new FileItem { ImdbId = item.ImdbId, FullName = path };
        await files.UpsertAsync(link, ct).ConfigureAwait(false);
        item.TomatoUrl = path;
        await movies.UpsertAsync(item, ct).ConfigureAwait(false);
        return link;
    }
}
