using System.Text.RegularExpressions;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.Extensions.Logging;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Controllers.Lookup;

/// <param name="Details">Full details when the source already has them (library, Plex); null for OMDb search hits.</param>
public sealed record LookupCandidate(string ImdbId, string Title, string Year, string Type, string Source, Item? Details);

/// <param name="Note">Set when a source couldn't be searched (e.g. OMDb limit reached); the other results are still shown.</param>
public sealed record LookupResult(IReadOnlyList<LookupCandidate> Candidates, string? Note);

/// <param name="ConflictPath">The movie is already linked to this other, existing file; nothing was changed.</param>
public sealed record LinkOutcome(bool Linked, string? ConflictPath);

/// <summary>
/// Find the movie for one file by hand, for files the scan couldn't match. Port of the file part of
/// SilverScreen's MovieForm (search by title/year or IMDb id, then "Update" to link the file).
/// </summary>
/// <remarks>
/// SilverScreen asked OMDb for one exact title; this lists candidates from the library, Plex and OMDb search.
/// </remarks>
public sealed partial class ManualLookupController(
    IMovieFileNameParser parser,
    IMovieRepository movies,
    IMediaServerLibrary plex,
    IMovieInfoProvider omdb,
    IFileItemRepository files,
    IMediaFileScanner scanner,
    IPosterProvider posters,
    MovieLinker linker,
    ILogger<ManualLookupController> logger)
{
    private const int MaxCandidates = 50;

    /// <summary>What to search for first: an IMDb id from the name, else the best title and year.</summary>
    public (string Query, int? Year) Suggest(string filePath)
    {
        var parsed = parser.Parse(filePath);
        return parsed.ImdbId is { } id
            ? (id, null)
            : (parsed.Titles.FirstOrDefault() ?? Path.GetFileNameWithoutExtension(filePath), parsed.Year);
    }

    public async Task<LookupResult> SearchAsync(string query, int? year, CancellationToken ct = default)
    {
        var text = query.Trim();
        if (text.Length == 0) return new LookupResult([], null);

        if (ImdbId().IsMatch(text))
        {
            var item = await linker.FindByImdbIdAsync(text.ToLowerInvariant(), usePlex: plex.IsConfigured, ct).ConfigureAwait(false);
            return new LookupResult(item is null ? [] : [FromItem(item, "IMDb id")], null);
        }

        var found = new List<LookupCandidate>();
        string? note = null;

        found.AddRange((await movies.SearchByTitleAsync(text, ct).ConfigureAwait(false))
            .Where(i => InYear(i.Year, year)).Select(i => FromItem(i, "library")));

        if (plex.IsConfigured)
        {
            try
            {
                found.AddRange((await plex.SearchAsync(text, ct).ConfigureAwait(false))
                    .Where(i => InYear(i.Year, year)).Select(i => FromItem(i, "Plex")));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Plex search failed");
                note = $"Plex: {ex.Message}";
            }
        }

        try
        {
            found.AddRange((await omdb.SearchAsync(text, ct: ct).ConfigureAwait(false))
                .Where(s => InYear(s.Year, year))
                .Select(s => new LookupCandidate(s.ImdbId, s.Title, s.Year, s.Type, "OMDb", null)));
        }
        catch (MovieInfoUnavailableException ex)
        {
            note = note is null ? ex.Message : $"{note}; {ex.Message}";
        }

        // Cheapest source wins for each movie: library, then Plex, then OMDb.
        var candidates = found
            .Where(c => !string.IsNullOrEmpty(c.ImdbId))
            .DistinctBy(c => c.ImdbId, StringComparer.OrdinalIgnoreCase)
            .Take(MaxCandidates)
            .ToList();
        return new LookupResult(candidates, note);
    }

    /// <summary>Full details for a candidate (one OMDb call for OMDb search hits).</summary>
    public async Task<Item?> GetDetailsAsync(LookupCandidate candidate, CancellationToken ct = default) =>
        candidate.Details ?? await omdb.GetByImdbIdAsync(candidate.ImdbId, ct).ConfigureAwait(false);

    public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => posters.GetPosterAsync(item, ct);

    /// <summary>
    /// Links <paramref name="filePath"/> to the candidate's movie. If that movie is already linked to another file
    /// that still exists, nothing changes unless <paramref name="replaceExisting"/> is set.
    /// </summary>
    public async Task<LinkOutcome> LinkAsync(string filePath, LookupCandidate candidate, bool replaceExisting, CancellationToken ct = default)
    {
        var item = await GetDetailsAsync(candidate, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"No details found for {candidate.ImdbId}.");

        if (!replaceExisting
            && await files.GetByImdbIdAsync(item.ImdbId, ct).ConfigureAwait(false) is { } existing
            && !string.Equals(existing.FullName, filePath, StringComparison.OrdinalIgnoreCase)
            && (await scanner.FindExistingAsync([existing.FullName], ct).ConfigureAwait(false)).Count > 0)
        {
            return new LinkOutcome(false, existing.FullName);
        }

        await linker.LinkAsync(item, filePath, ct).ConfigureAwait(false);
        logger.LogInformation("Linked {Path} to {ImdbId} ({Title}) by hand", filePath, item.ImdbId, item.Title);
        return new LinkOutcome(true, null);
    }

    private static LookupCandidate FromItem(Item item, string source) =>
        new(item.ImdbId, item.Title, item.Year ?? "", item.Type ?? "", source, item);

    /// <summary>OMDb years look like "2011" or "2005–2010" for series.</summary>
    private static bool InYear(string? itemYear, int? year) =>
        year is null || (itemYear ?? "").StartsWith(year.Value.ToString(), StringComparison.Ordinal);

    [GeneratedRegex(@"^tt\d{7,9}$", RegexOptions.IgnoreCase)]
    private static partial Regex ImdbId();
}
