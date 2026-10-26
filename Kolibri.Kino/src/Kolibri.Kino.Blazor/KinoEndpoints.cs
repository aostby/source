using System.Text;
using System.Text.RegularExpressions;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Playback;
using Kolibri.Kino.Controllers.Watchlists;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Plain HTTP endpoints next to the pages: poster images (so the browser loads and caches them itself),
/// the video file and its subtitles for the Play page, and the watchlist CSV download.
/// </summary>
internal static partial class KinoEndpoints
{
    public static void MapKinoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/poster/{imdbId}", PosterAsync);
        app.MapGet("/stream/{imdbId}", StreamAsync);
        app.MapGet("/subtitle/{imdbId}", SubtitleAsync);
        app.MapGet("/export/watchlist", ExportWatchlistAsync);
        app.MapGet("/export/wishlist", ExportWishlistAsync);
        app.MapGet("/export/ratings", ExportRatingsAsync);
    }

    /// <summary>
    /// The movie's own file (or with ?ep=, an episode in the series folder), with range requests so the player
    /// can seek. Only the file the library links to the id, or one inside the linked folder.
    /// </summary>
    private static async Task<IResult> StreamAsync(string imdbId, string? ep, PlaybackController playback, CancellationToken ct)
    {
        if (!ImdbId().IsMatch(imdbId)) return Results.NotFound();

        return await playback.GetFileAsync(imdbId, ep, ct) is { } file
            ? Results.File(file.LocalPath, file.ContentType, enableRangeProcessing: true)
            : Results.NotFound();
    }

    /// <summary>A subtitle next to the file /stream gives (?file= its name), as WebVTT for the player's track.</summary>
    private static async Task<IResult> SubtitleAsync(string imdbId, string? ep, string file, PlaybackController playback, CancellationToken ct)
    {
        if (!ImdbId().IsMatch(imdbId)) return Results.NotFound();

        return await playback.GetSubtitleVttAsync(imdbId, ep, file, ct) is { } vtt
            ? Results.Text(vtt, "text/vtt", Encoding.UTF8)
            : Results.NotFound();
    }

    private static async Task<IResult> PosterAsync(string imdbId, MovieController movies, HttpContext http, CancellationToken ct)
    {
        if (!ImdbId().IsMatch(imdbId)) return Results.NotFound();

        var bytes = await movies.GetPosterAsync(imdbId, ct);
        if (bytes is null) return Results.NotFound();

        // Posters don't change; a week in the browser cache saves the NAS the work.
        http.Response.Headers.CacheControl = "public, max-age=604800";
        return Results.File(bytes, ImageType(bytes));
    }

    /// <summary>The wishlist as CSV for Excel, with "In SilverScreen" as true/false.</summary>
    private static async Task<IResult> ExportWishlistAsync(
        Kolibri.Kino.Controllers.Wishlist.WishlistController wishlist, ILoggerFactory logs, CancellationToken ct)
    {
        try
        {
            var rows = await wishlist.GetAsync(ct);
            return CsvFile(Kolibri.Kino.Controllers.Wishlist.WishlistController.ToCsv(rows), $"Wishlist - {DateTime.Now:yyyy-MM-dd}.csv");
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            return ExportFailed(logs, ex, "the wishlist");
        }
    }

    /// <summary>Your ratings as a JSON file, for Settings → Importer vurderinger fra fil in the Windows app.</summary>
    private static async Task<IResult> ExportRatingsAsync(
        Kolibri.Kino.Controllers.Ratings.RatingController ratings, ILoggerFactory logs, CancellationToken ct)
    {
        try
        {
            var json = await ratings.ExportAsync(ct);
            return Results.File(Encoding.UTF8.GetBytes(json), "application/json; charset=utf-8", $"Kino ratings - {DateTime.Now:yyyy-MM-dd}.json");
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            return ExportFailed(logs, ex, "your ratings");
        }
    }

    private static async Task<IResult> ExportWatchlistAsync(string list, WatchlistController watchlists, ILoggerFactory logs, CancellationToken ct)
    {
        try
        {
            var items = await watchlists.GetItemsAsync(list, ct);
            return CsvFile(WatchlistController.ToCsv(items), $"{list}.csv");
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            return ExportFailed(logs, ex, $"the watchlist \"{list}\"");
        }
    }

    /// <summary>CSV as a download, with a BOM so Excel reads it as UTF-8.</summary>
    private static IResult CsvFile(string csv, string fileName) =>
        Results.File(new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray(),
            "text/csv; charset=utf-8", fileName);

    /// <summary>
    /// A failed export (e.g. the database is busy and the wait for it timed out) is logged, and the browser gets a short
    /// plain-text page saying why, instead of a broken download.
    /// </summary>
    private static IResult ExportFailed(ILoggerFactory logs, Exception ex, string what)
    {
        logs.CreateLogger("Kolibri.Kino.Blazor.Export").LogError(ex, "Exporting {What} failed", what);
        return Results.Text($"Couldn't export {what}: {ex.Message}\nGo back and try again in a moment.",
            "text/plain; charset=utf-8", statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    /// <summary>The cache stores the bytes as downloaded: almost always JPEG, sometimes PNG.</summary>
    private static string ImageType(byte[] bytes) =>
        bytes is [0x89, 0x50, 0x4E, 0x47, ..] ? "image/png"
        : bytes is [0x47, 0x49, 0x46, ..] ? "image/gif"
        : bytes is [0x52, 0x49, 0x46, 0x46, _, _, _, _, 0x57, 0x45, 0x42, 0x50, ..] ? "image/webp"
        : "image/jpeg";

    [GeneratedRegex(@"^tt\d{5,10}$")]
    private static partial Regex ImdbId();
}
