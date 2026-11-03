using System.Globalization;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.Core.Abstractions;

namespace Kolibri.Kino.Controllers.Suggestions;

/// <param name="Movie">The suggested movie (TMDb's title, year and small poster).</param>
/// <param name="ImdbRating">Its IMDb rating: the library's when it's there, else the one kept with the similar movies.</param>
/// <param name="Score">The stars (1–10) of the movies you liked that it's like, added up.</param>
/// <param name="Because">Those movies' titles, best liked first.</param>
/// <param name="InLibrary">In the library, without a file (with one it isn't suggested).</param>
/// <param name="Fresh">Like a movie you rated in the last <see cref="SuggestionsController.FreshFor"/>.</param>
public sealed record Suggestion(TmdbMovieHit Movie, string? ImdbRating, int Score, IReadOnlyList<string> Because, bool InLibrary, bool Fresh = false);

/// <summary>
/// "Suggestions for you" on the Library page: movies like the ones you rated 3½ stars or more, from the similar movies
/// kept when their details were opened (no TMDb calls here). A movie like several of them comes first. Movies you have
/// (with a file) or have rated are left out. Movies like one you rated lately come first in the row, so a new rating shows
/// at once; the rest are picked from the best 40, a new pick each day. "See all" shows every one (<see cref="GetAllAsync"/>).
/// </summary>
public sealed class SuggestionsController(IRatingRepository ratings, ISimilarCache similar, IMovieRepository movies, LibraryFiles libraryFiles)
{
    /// <summary>Your rating (1–10) a movie needs to suggest others: 3½ stars.</summary>
    public const int MinRating = 7;

    /// <summary>Fewer liked movies with kept similar movies than this, and nothing is suggested (the row would be thin and repetitive).</summary>
    public const int MinSources = 3;

    public const int Shown = 20;
    public const int Pool = 40;

    /// <summary>A rating this new makes its similar movies <see cref="Suggestion.Fresh"/>.</summary>
    public static readonly TimeSpan FreshFor = TimeSpan.FromDays(7);

    /// <summary>At most this many fresh suggestions go first in the row, so it isn't all one movie's.</summary>
    public const int FreshShown = 8;

    /// <summary>
    /// The row: up to <see cref="FreshShown"/> fresh suggestions first (best first), then the rest of <see cref="Shown"/>
    /// picked from the best <see cref="Pool"/>, a new pick each day, best first.
    /// </summary>
    public async Task<IReadOnlyList<Suggestion>> GetAsync(DateTime? today = null, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct).ConfigureAwait(false);
        var fresh = all.Where(s => s.Fresh).Take(FreshShown).ToList();
        var best = all.Take(Pool).Except(fresh).ToList();

        // A new pick of the best each day, shown best first: the row changes a little without asking TMDb.
        var day = (today ?? DateTime.Today).Date;
        var random = new Random(int.Parse(day.ToString("yyyyMMdd", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        return fresh.Concat(best.OrderBy(_ => random.Next()).Take(Shown - fresh.Count).OrderBy(best.IndexOf)).ToList();
    }

    /// <summary>Every suggestion, best first (for "See all"); empty until <see cref="MinSources"/> liked movies have similar movies kept.</summary>
    public async Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken ct = default)
    {
        var rated = await ratings.GetAllAsync(ct).ConfigureAwait(false);
        var liked = rated.Where(r => r.Rating >= MinRating).ToDictionary(r => r.ImdbId, StringComparer.OrdinalIgnoreCase);
        var sources = (await similar.GetManyAsync(liked.Keys, ct).ConfigureAwait(false)).Where(s => s.Hits.Count > 0).ToList();
        if (sources.Count < MinSources) return [];

        var freshSince = DateTime.UtcNow - FreshFor;
        var ratedIds = rated.Select(r => r.ImdbId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidates = sources
            .SelectMany(s => s.Hits.Select(h => (Hit: h, Source: liked[s.SourceImdbId])))
            .Where(x => !ratedIds.Contains(x.Hit.Movie.ImdbId))
            .GroupBy(x => x.Hit.Movie.ImdbId, StringComparer.OrdinalIgnoreCase)
            .Select(g => (
                Hit: g.First().Hit,
                Score: g.DistinctBy(x => x.Source.ImdbId).Sum(x => x.Source.Rating),
                Because: g.DistinctBy(x => x.Source.ImdbId).OrderByDescending(x => x.Source.Rating).Select(x => x.Source.Title ?? x.Source.ImdbId).ToList(),
                Fresh: g.Any(x => x.Source.UpdatedUtc.ToUniversalTime() >= freshSince)))
            .ToList();

        // Not the ones you have: in the library with a file.
        var local = await movies.GetByImdbIdsAsync(candidates.Select(c => c.Hit.Movie.ImdbId), ct).ConfigureAwait(false);
        var have = await libraryFiles.WithFileAsync(local.Keys, ct).ConfigureAwait(false);

        return candidates
            .Where(c => !have.Contains(c.Hit.Movie.ImdbId))
            .Select(c => new Suggestion(c.Hit.Movie,
                local.TryGetValue(c.Hit.Movie.ImdbId, out var item) && Clean(item.ImdbRating) is { } own ? own : c.Hit.ImdbRating,
                c.Score, c.Because, local.ContainsKey(c.Hit.Movie.ImdbId), c.Fresh))
            .OrderByDescending(s => s.Score)
            .ThenByDescending(s => WatchlistController.Rating(s.ImdbRating))
            .ThenBy(s => s.Movie.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>"Because you liked Klaus, Slow Horses and The Boys" (up to three).</summary>
    public static string BecauseText(Suggestion s) =>
        "Because you liked " + (s.Because.Count switch
        {
            1 => s.Because[0],
            2 => $"{s.Because[0]} and {s.Because[1]}",
            _ => $"{s.Because[0]}, {s.Because[1]} and {(s.Because.Count == 3 ? s.Because[2] : $"{s.Because.Count - 2} more")}",
        });

    private static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) || text == "N/A" ? null : text.Trim();
}
