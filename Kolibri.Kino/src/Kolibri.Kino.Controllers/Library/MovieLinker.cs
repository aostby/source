using System.Globalization;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Library;

/// <summary>
/// Shared by the scan and manual lookup: find a movie by IMDb id (cheapest source first) and link it to a file.
/// </summary>
public sealed class MovieLinker(
    IFileItemRepository files,
    IMovieRepository movies,
    IMediaServerLibrary plex,
    IMovieInfoProvider omdb,
    ILogger<MovieLinker> logger)
{
    /// <summary>Plex's ratings above this are checked against IMDb's own (through OMDb): Plex rates good movies too high.</summary>
    public const double PlexRatingCheckAbove = 8.5;

    /// <summary>The library, then Plex (free), then OMDb.</summary>
    public async Task<Item?> FindByImdbIdAsync(string imdbId, bool usePlex, CancellationToken ct)
    {
        if (await movies.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } local) return local;
        if (usePlex && plex.IsConfigured && await plex.FindByImdbIdAsync(imdbId, ct).ConfigureAwait(false) is { } fromPlex)
            return await CheckPlexRatingAsync(fromPlex, ct).ConfigureAwait(false);
        return await omdb.GetByImdbIdAsync(imdbId, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// A movie from Plex rated above <see cref="PlexRatingCheckAbove"/> gets IMDb's own rating from OMDb instead (one
    /// OMDb request, so only for these), with OMDb's votes and list of ratings. When OMDb has no rating for it or can't
    /// be asked (daily limit, bad key), Plex's rating stays.
    /// Only for items from Plex: ratings in the library, or set by hand, are never changed here.
    /// </summary>
    public async Task<Item> CheckPlexRatingAsync(Item fromPlex, CancellationToken ct)
    {
        if (fromPlex.Type is { Length: > 0 } type && !type.Equals("movie", StringComparison.OrdinalIgnoreCase)) return fromPlex;
        if (MovieController.ParseImdbRating(fromPlex.ImdbRating) is not { } plexRating || plexRating <= PlexRatingCheckAbove) return fromPlex;

        Item? fromOmdb;
        try
        {
            fromOmdb = await omdb.GetByImdbIdAsync(fromPlex.ImdbId, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning("Kept Plex's rating {Rating} for {ImdbId}; OMDb couldn't be asked: {Reason}", fromPlex.ImdbRating, fromPlex.ImdbId, ex.Message);
            return fromPlex;
        }
        if (MovieController.ParseImdbRating(fromOmdb?.ImdbRating) is not { } imdbRating) return fromPlex;

        var text = imdbRating.ToString("0.0", CultureInfo.InvariantCulture);
        logger.LogInformation("{ImdbId} ({Title}): Plex's rating {PlexRating} replaced by IMDb's {ImdbRating} from OMDb",
            fromPlex.ImdbId, fromPlex.Title, fromPlex.ImdbRating, text);
        fromPlex.ImdbRating = text;
        if (fromOmdb!.ImdbVotes is { Length: > 0 } votes && votes != "N/A") fromPlex.ImdbVotes = votes;
        if (fromOmdb.Ratings is { Count: > 0 } ratings) fromPlex.Ratings = ratings;
        return fromPlex;
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
