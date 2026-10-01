using System.Globalization;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.LocalMovies;

/// <summary>
/// Use cases behind the local movies window (port of SilverScreen's ShowLocalMoviesForm):
/// pick a movie folder, list what the library knows about it, and find gaps in both directions.
/// </summary>
public sealed class LocalMoviesController(
    IFileItemRepository files,
    IMovieRepository movies,
    IMediaFileScanner scanner,
    ISettingsStore settings,
    IPosterProvider posters,
    ILogger<LocalMoviesController> logger)
{
    /// <summary>UserFilePaths.MoviesSourcePath from the user settings (shared with SilverScreen).</summary>
    public Task<string?> GetMoviesFolderAsync(CancellationToken ct = default) =>
        Task.FromResult(settings.Current.UserFilePaths.MoviesSourcePath);

    public Task SetMoviesFolderAsync(string folder, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        var changed = settings.Current.Clone();
        changed.UserFilePaths.MoviesSourcePath = folder;
        return settings.SaveAsync(changed, ct);
    }

    public async Task<LocalMoviesResult> LoadAsync(string folder, LocalMoviesFilter filter, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        var libraryFiles = await files.GetInFolderAsync(folder, ct).ConfigureAwait(false);

        if (filter == LocalMoviesFilter.NotInLibrary)
        {
            var known = libraryFiles.Select(f => f.FullName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var onDisk = await scanner.FindVideoFilesAsync(folder, ct).ConfigureAwait(false);
            var unmatched = onDisk
                .Where(path => !known.Contains(path))
                .Order(StringComparer.OrdinalIgnoreCase)
                .Select(path => new UnmatchedFile(path, MovieFileRules.IsMultipart(path)))
                .ToList();
            return new LocalMoviesResult(folder, filter, [], unmatched, libraryFiles.Count);
        }

        var existing = await scanner.FindExistingAsync(libraryFiles.Select(f => f.FullName), ct).ConfigureAwait(false);
        var candidates = filter == LocalMoviesFilter.MissingFiles
            ? libraryFiles.Where(f => !existing.Contains(f.FullName)).ToList()
            : libraryFiles;

        var items = await movies.GetByImdbIdsAsync(candidates.Select(f => f.ImdbId), ct).ConfigureAwait(false);

        // Same order as SilverScreen: best rated first, then title.
        var result = candidates
            .Select(f => new LocalMovie(f.ImdbId, f.FullName, existing.Contains(f.FullName), items.GetValueOrDefault(f.ImdbId)))
            .OrderByDescending(m => Rating(m.Item))
            .ThenBy(m => m.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new LocalMoviesResult(folder, filter, result, [], libraryFiles.Count);
    }

    /// <summary>The poster image, or null if there is none or it can't be fetched right now.</summary>
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

    private static double Rating(Item? item) =>
        double.TryParse(item?.ImdbRating, NumberStyles.Float, CultureInfo.InvariantCulture, out var rating) ? rating : -1;
}
