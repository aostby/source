using System.Text;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Series;

/// <summary>A folder in the series folder that nothing in the library is linked to.</summary>
public sealed record NewSeriesFolder(string Path, SeriesFolderGuess Guess)
{
    public string Name => System.IO.Path.GetFileName(Path.TrimEnd(System.IO.Path.DirectorySeparatorChar));
}

/// <param name="IgnoredCount">Folders skipped because they were ignored earlier (see <see cref="NewSeriesController.IgnoreAsync"/>).</param>
public sealed record NewFolderScan(string Root, IReadOnlyList<NewSeriesFolder> Folders, int IgnoredCount);

public enum NewSeriesStatus
{
    /// <summary>Found and linked to the folder.</summary>
    Linked,

    /// <summary>Found, but the series is linked to another folder that still exists; nothing was changed.</summary>
    LinkedElsewhere,

    /// <summary>Not found with confidence; ask the user (title and year, or the IMDb id).</summary>
    NeedsInput,
}

/// <param name="Item">The series found (Linked, LinkedElsewhere), or the IMDb tag's title when it isn't a series.</param>
/// <param name="Note">Why the user must help (e.g. "tt… is a movie", "OMDb: Request limit reached!").</param>
/// <param name="OtherFolder">For LinkedElsewhere: the folder the series is linked to.</param>
public sealed record NewSeriesResult(NewSeriesFolder Folder, NewSeriesStatus Status, Item? Item, string? Note, string? OtherFolder = null);

/// <param name="ConflictFolder">The series is already linked to this other, existing folder; nothing was changed.</param>
public sealed record SeriesLinkOutcome(bool Linked, string? ConflictFolder);

/// <summary>
/// "Find new series": folders in the series folder (UserFilePaths.SeriesSourcePath) that aren't linked yet are
/// matched to a series and linked, like movies are by the scan.
/// </summary>
/// <remarks>
/// A folder is matched automatically only when that is safe: by an IMDb tag in its name ("{imdb-tt1985443}"),
/// by a series in the library with the same title (and year), or by title and year through TMDb, then OMDb.
/// The rest need the user; see <see cref="SearchAsync"/> and <see cref="LinkAsync"/>. Only series are linked here:
/// a movie in the series folder is reported, never linked. Nothing on disk is changed, except by
/// <see cref="RemoveImdbTagAsync"/> when the user asks.
/// </remarks>
public sealed class NewSeriesController(
    IMovieRepository movies,
    IFileItemRepository files,
    IMediaFileScanner scanner,
    IFolderRenamer renamer,
    ISeriesRepository seriesStore,
    IImdbIdResolver tmdb,
    IMovieInfoProvider omdb,
    ISettingsStore settings,
    MovieLinker linker,
    ManualLookupController lookup,
    ILogger<NewSeriesController> logger)
{
    /// <summary>Folders in the series folder that nothing is linked to, and that aren't ignored unless <paramref name="includeIgnored"/>.</summary>
    /// <exception cref="InvalidOperationException">No series folder is set, or it can't be reached.</exception>
    public async Task<NewFolderScan> FindNewFoldersAsync(bool includeIgnored, CancellationToken ct = default)
    {
        var root = settings.Current.UserFilePaths.SeriesSourcePath;
        if (string.IsNullOrWhiteSpace(root))
            throw new InvalidOperationException("No series folder is set. Set it in Settings (Series source path).");
        if ((await scanner.FindExistingFoldersAsync([root], ct).ConfigureAwait(false)).Count == 0)
            throw new InvalidOperationException($"The series folder {root} can't be reached.");

        // Linked: a series' folder, or any file or folder inside it (e.g. a movie that is stored among the series).
        var linked = (await files.GetAllAsync(ct).ConfigureAwait(false)).Select(f => f.FullName)
            .Concat((await movies.GetAllAsync("series", ct).ConfigureAwait(false)).Select(i => i.TomatoUrl))
            .Where(p => !string.IsNullOrWhiteSpace(p) && p != "N/A")
            .Select(p => Normalize(p!))
            .ToList();
        var ignored = await seriesStore.GetIgnoredFoldersAsync(ct).ConfigureAwait(false);

        var found = new List<NewSeriesFolder>();
        var ignoredCount = 0;
        foreach (var folder in (await scanner.GetSubfoldersAsync(root, ct).ConfigureAwait(false)).Order(StringComparer.CurrentCultureIgnoreCase))
        {
            var name = Path.GetFileName(folder);
            if (name.Length == 0 || name[0] is '#' or '@' or '.' or '$') continue; // #recycle, @eaDir …

            var mine = Normalize(folder);
            if (linked.Any(p => string.Equals(p, mine, StringComparison.OrdinalIgnoreCase)
                                || p.StartsWith(mine + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
                continue;

            if (ignored.Contains(folder) || ignored.Contains(mine))
            {
                ignoredCount++;
                if (!includeIgnored) continue;
            }
            found.Add(new NewSeriesFolder(folder, SeriesFolderName.Parse(name)));
        }
        return new NewFolderScan(root, found, ignoredCount);
    }

    /// <summary>
    /// Matches and links each folder that can be matched safely; the rest come back as <see cref="NewSeriesStatus.NeedsInput"/>.
    /// </summary>
    public async Task<IReadOnlyList<NewSeriesResult>> AddAutomaticallyAsync(
        IReadOnlyList<NewSeriesFolder> folders, IProgress<string>? progress = null, CancellationToken ct = default)
    {
        var library = await movies.GetAllAsync("series", ct).ConfigureAwait(false);
        var results = new List<NewSeriesResult>();
        string? omdbDown = null; // after "Request limit reached!" the rest aren't asked

        foreach (var folder in folders)
        {
            ct.ThrowIfCancellationRequested();
            progress?.Report($"{results.Count + 1}/{folders.Count}: {folder.Name}");
            try
            {
                var result = await AddOneAsync(folder, library, omdbDown is null, ct).ConfigureAwait(false);
                results.Add(result);
            }
            catch (MovieInfoUnavailableException ex)
            {
                omdbDown = ex.Message;
                results.Add(new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null, ex.Message));
            }
        }
        return results;
    }

    private async Task<NewSeriesResult> AddOneAsync(NewSeriesFolder folder, IReadOnlyList<Item> library, bool useOmdb, CancellationToken ct)
    {
        var guess = folder.Guess;

        if (guess.ImdbId is { } id)
        {
            if (!useOmdb && library.All(i => !string.Equals(i.ImdbId, id, StringComparison.OrdinalIgnoreCase)))
                return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null, "OMDb can't be used right now");
            var tagged = await linker.FindByImdbIdAsync(id, usePlex: true, ct).ConfigureAwait(false);
            if (tagged is null)
                return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null, $"Nothing found for the folder's IMDb tag {id}");
            if (!IsSeries(tagged))
                return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, tagged, NotASeries(tagged, "The folder's IMDb tag"));
            return await LinkFoundAsync(folder, tagged, "IMDb tag", ct).ConfigureAwait(false);
        }

        // A series already in the library (e.g. its folder was renamed or moved): same title, and year if known.
        var same = library.Where(i => SameTitle(i.Title, guess.Title) && (guess.Year is null || StartYear(i.Year) == guess.Year)).ToList();
        if (same.Count == 1)
            return await LinkFoundAsync(folder, same[0], "library", ct).ConfigureAwait(false);
        if (same.Count > 1)
            return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null,
                $"The library has {same.Count} series called \"{guess.Title}\": " + string.Join(", ", same.Select(i => $"{i.Year} {i.ImdbId}")));

        // Online only with a year: a title alone ("Daredevil") is too often more than one show.
        if (guess.Year is not { } year)
            return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null, null);

        try
        {
            if (await tmdb.FindSeriesImdbIdAsync(guess.Title, year, ct).ConfigureAwait(false) is { } tmdbId
                && useOmdb
                && await linker.FindByImdbIdAsync(tmdbId, usePlex: true, ct).ConfigureAwait(false) is { } fromTmdb
                && IsSeries(fromTmdb))
                return await LinkFoundAsync(folder, fromTmdb, "TMDb", ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not (OperationCanceledException or MovieInfoUnavailableException))
        {
            logger.LogWarning(ex, "TMDb series search failed for {Title} ({Year})", guess.Title, year);
        }

        if (useOmdb
            && await omdb.GetSeriesByTitleAsync(guess.Title, year, ct).ConfigureAwait(false) is { } fromOmdb
            && IsSeries(fromOmdb) && SameTitle(fromOmdb.Title, guess.Title) && StartYear(fromOmdb.Year) == year)
            return await LinkFoundAsync(folder, fromOmdb, "OMDb", ct).ConfigureAwait(false);

        return new NewSeriesResult(folder, NewSeriesStatus.NeedsInput, null, null);
    }

    private async Task<NewSeriesResult> LinkFoundAsync(NewSeriesFolder folder, Item item, string how, CancellationToken ct)
    {
        var outcome = await LinkAsync(folder, item, replaceExisting: false, ct).ConfigureAwait(false);
        if (!outcome.Linked)
            return new NewSeriesResult(folder, NewSeriesStatus.LinkedElsewhere, item, null, outcome.ConflictFolder);
        return new NewSeriesResult(folder, NewSeriesStatus.Linked, item, $"by {how}");
    }

    /// <summary>
    /// Series for a title (library, OMDb search, and TMDb's match for title + year). An IMDb id shows what it is,
    /// even a movie, so the user sees why it can't be linked.
    /// </summary>
    public async Task<LookupResult> SearchAsync(string query, int? year, CancellationToken ct = default)
    {
        var found = await lookup.SearchAsync(query, year, ct).ConfigureAwait(false);
        var candidates = found.Candidates.Any(c => c.Source == "IMDb id")
            ? [.. found.Candidates]
            : found.Candidates.Where(c => IsSeries(c.Type)).ToList();

        var note = found.Note;
        if (year is { } y && !candidates.Any(c => IsSeries(c.Type)))
        {
            try
            {
                if (await tmdb.FindSeriesImdbIdAsync(query.Trim(), y, ct).ConfigureAwait(false) is { } id
                    && await linker.FindByImdbIdAsync(id, usePlex: true, ct).ConfigureAwait(false) is { } item)
                    candidates.Add(new LookupCandidate(item.ImdbId, item.Title, item.Year ?? "", item.Type ?? "", "TMDb", item));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                note = note is null ? ex.Message : $"{note}; {ex.Message}";
            }
        }
        return new LookupResult(candidates, note);
    }

    /// <summary>Full details for a candidate (one OMDb call for OMDb search hits).</summary>
    public Task<Item?> GetDetailsAsync(LookupCandidate candidate, CancellationToken ct = default) => lookup.GetDetailsAsync(candidate, ct);

    public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => lookup.GetPosterAsync(item, ct);

    /// <summary>
    /// Links the folder to the series. If the series is already linked to another folder that still exists,
    /// nothing changes unless <paramref name="replaceExisting"/> is set.
    /// </summary>
    /// <exception cref="InvalidOperationException"><paramref name="item"/> isn't a series (movies stay out of the series folder).</exception>
    public async Task<SeriesLinkOutcome> LinkAsync(NewSeriesFolder folder, Item item, bool replaceExisting, CancellationToken ct = default)
    {
        if (!IsSeries(item)) throw new InvalidOperationException(NotASeries(item, item.ImdbId) + ".");

        // A link to the folder itself, to something inside it, or to a folder around it (e.g. \\server\Series, as
        // SilverScreen sometimes stored; too broad to be right) is replaced.
        if (!replaceExisting
            && await files.GetByImdbIdAsync(item.ImdbId, ct).ConfigureAwait(false) is { } existing
            && !IsSameOrParent(existing.FullName, folder.Path)
            && !IsSameOrParent(folder.Path, existing.FullName)
            && (await scanner.FindExistingFoldersAsync([existing.FullName], ct).ConfigureAwait(false)).Count > 0)
            return new SeriesLinkOutcome(false, existing.FullName);

        await linker.LinkAsync(item, folder.Path, ct).ConfigureAwait(false);
        logger.LogInformation("Series folder {Folder} linked to {ImdbId} ({Title})", folder.Path, item.ImdbId, item.Title);
        return new SeriesLinkOutcome(true, null);
    }

    /// <summary>"The folder's IMDb tag tt… is "Horizon …" (2024), a movie, not a series; …".</summary>
    private static string NotASeries(Item item, string what) =>
        $"{what} {(what == item.ImdbId ? "" : item.ImdbId + " ")}is \"{item.Title}\" ({item.Year}), a {item.Type ?? "title"}, not a series. " +
        "Only series are linked in the series folder; move a movie to the movies folder, or choose Don't ask again";

    /// <summary>
    /// Takes the {imdb-tt…} tag out of the folder's name on disk (when the tag is wrong), and returns the folder
    /// under its new name, read again without the tag.
    /// </summary>
    /// <exception cref="InvalidOperationException">The name has no tag.</exception>
    public async Task<NewSeriesFolder> RemoveImdbTagAsync(NewSeriesFolder folder, CancellationToken ct = default)
    {
        var newName = SeriesFolderName.WithoutImdbTag(folder.Name);
        if (newName == folder.Name) throw new InvalidOperationException($"\"{folder.Name}\" has no {{imdb-tt…}} tag.");

        var path = await renamer.RenameAsync(folder.Path, newName, ct).ConfigureAwait(false);
        logger.LogInformation("Renamed {Old} to {New} (removed a wrong IMDb tag)", folder.Path, path);
        return new NewSeriesFolder(path, SeriesFolderName.Parse(newName));
    }

    /// <summary>"Find new series" won't ask about this folder again (unless asked to include ignored folders).</summary>
    public Task IgnoreAsync(NewSeriesFolder folder, CancellationToken ct = default) => seriesStore.IgnoreFolderAsync(folder.Path, ct);

    private static bool IsSeries(Item item) => IsSeries(item.Type);

    private static bool IsSeries(string? type) => string.Equals(type, "series", StringComparison.OrdinalIgnoreCase);

    /// <summary>Titles compared on letters and digits only: "Tom Clancys Jack Ryan" is "Tom Clancy's Jack Ryan".</summary>
    internal static bool SameTitle(string? a, string? b) =>
        a is not null && b is not null && Key(a).Length > 0 && string.Equals(Key(a), Key(b), StringComparison.OrdinalIgnoreCase);

    private static string Key(string title)
    {
        var text = title.Replace("&", "and", StringComparison.Ordinal).Normalize(NormalizationForm.FormD);
        return new string(text.Where(char.IsLetterOrDigit).ToArray());
    }

    /// <summary>OMDb years look like "2012" or "2008–2013".</summary>
    private static int? StartYear(string? year) =>
        year is { Length: >= 4 } && int.TryParse(year.AsSpan(0, 4), out var y) ? y : null;

    private static bool IsSameOrParent(string candidate, string folder)
    {
        var (c, f) = (Normalize(candidate), Normalize(folder));
        return string.Equals(c, f, StringComparison.OrdinalIgnoreCase)
               || f.StartsWith(c + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string path) => path.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}
