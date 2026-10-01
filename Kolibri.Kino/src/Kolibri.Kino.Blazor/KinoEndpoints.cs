using System.Text;
using System.Text.RegularExpressions;
using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Watchlists;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Plain HTTP endpoints next to the pages: poster images (so the browser loads and caches them itself)
/// and the watchlist CSV download.
/// </summary>
internal static partial class KinoEndpoints
{
    public static void MapKinoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/poster/{imdbId}", PosterAsync);
        app.MapGet("/export/watchlist", ExportWatchlistAsync);
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

    private static async Task<IResult> ExportWatchlistAsync(string list, WatchlistController watchlists, CancellationToken ct)
    {
        var items = await watchlists.GetItemsAsync(list, ct);
        var csv = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(WatchlistController.ToCsv(items)))
            .ToArray();
        return Results.File(csv, "text/csv; charset=utf-8", $"{list}.csv");
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
