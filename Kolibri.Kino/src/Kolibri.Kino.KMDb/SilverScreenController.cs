using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// The SilverScreen library (LiteDB) as copied to MariaDB: one table per collection, with the same names (Item,
/// Episode, FileItem, WatchListItem, KinoRating, Image …). Documents come back as LiteDB held them; nested data
/// (Episodes, Cast, Genres) as JSON, dates in UTC. UserSettings is never served (keys and passwords), and Plex tokens
/// are removed from addresses.
/// </summary>
/// <remarks>
/// Writing, for WinForms and the web app: PUT replaces a document (a change the user made), POST …/sync merges a
/// batch both ways (what one side lacks is filled from the other; with Updated/Fetched/Checked/Added the newer wins)
/// and returns the merged documents for the client to keep, GET …?since= lists what changed, DELETE removes.
/// </remarks>
[ApiController]
[Tags("SilverScreen")]
[Route("silverscreen")]
[Produces("application/json")]
public sealed class SilverScreenController(ISilverScreenRepository library, ISilverScreenWriter writer, PosterService posters, KmdbMetrics? metrics = null) : ControllerBase
{
    private const int MaxPageSize = 500;
    private const int MaxSyncBatch = 1000;
    private const int MaxBinaryBytes = 50 * 1024 * 1024;

    /// <summary>The collections, with row counts and when each was last copied from LiteDB.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SilverScreenCollection>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Collections(CancellationToken ct) => Ok(await library.CollectionsAsync(ct));

    /// <summary>
    /// The poster for a title or episode. No key needed, so it works in &lt;img src&gt;. From the image cache; when it isn't
    /// there, from the library's TMDb poster (Movie/TvShow) or another public poster address it has (never Plex), and
    /// then kept in the cache.
    /// </summary>
    /// <param name="imdbId">tt0133093, or an episode id.</param>
    /// <param name="ct"></param>
    [HttpGet("poster/{imdbId}")]
    public async Task<IActionResult> Poster(string imdbId, CancellationToken ct) =>
        ImageResult(imdbId, await posters.GetAsync(imdbId, ct));

    /// <summary>All images of a title: type (poster, backdrop, still, landscape, thumb, clearlogo …), source, size and address. No key needed.</summary>
    /// <param name="imdbId">tt0133093 (a series lists its episodes' stills too).</param>
    /// <param name="ct"></param>
    [HttpGet("art/{imdbId}")]
    [ProducesResponseType<IReadOnlyList<ArtInfo>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Art(string imdbId, CancellationToken ct) => Ok(await library.ArtListAsync(imdbId, ct));

    /// <summary>The best image of a type for a title, e.g. backdrop, landscape, clearlogo; still with season and episode. No key needed.</summary>
    /// <param name="imdbId">tt0133093</param>
    /// <param name="type">poster, backdrop, still, landscape, thumb, clearlogo, clearart, banner, discart, keyart, characterart.</param>
    /// <param name="season">For stills (and season posters).</param>
    /// <param name="episode">For stills.</param>
    /// <param name="ct"></param>
    [HttpGet("art/{imdbId}/{type}")]
    public async Task<IActionResult> ArtImage(string imdbId, string type, [FromQuery] int? season = null, [FromQuery] int? episode = null, CancellationToken ct = default) =>
        type.Equals("poster", StringComparison.OrdinalIgnoreCase) && season is null
            ? ImageResult(imdbId, await posters.GetAsync(imdbId, ct))
            : ImageResult($"{imdbId} {type}", await library.ArtAsync(imdbId, type.ToLowerInvariant(), season, episode, ct));

    /// <summary>An image from the cache by its key: an IMDb id, tt…_S01E02, tmdb-person-123, or the address it was fetched from. No key needed.</summary>
    /// <param name="id">The image's key (URL-encoded; slashes in an address are fine).</param>
    /// <param name="ct"></param>
    [HttpGet("image/{**id}")]
    public Task<IActionResult> Image(string id, CancellationToken ct) => ImageAsync(Uri.UnescapeDataString(id), ct);

    /// <summary>A page of a collection's documents, ordered by _id; optionally only those where a field has a value.</summary>
    /// <param name="collection">Item, Episode, FileItem, WatchListItem, KinoRating, KinoSeason … (see GET /silverscreen).</param>
    /// <param name="page">From 1.</param>
    /// <param name="pageSize">1–500.</param>
    /// <param name="field">A top-level field to filter on, e.g. WatchListName or SeriesId.</param>
    /// <param name="value">The value the field must have (exact).</param>
    /// <param name="since">Only documents changed at or after this time (UTC, e.g. 2026-11-06T00:00:00Z): what to sync.</param>
    /// <param name="ct"></param>
    [HttpGet("{collection}")]
    [ProducesResponseType<SilverScreenPage>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(string collection, [FromQuery] int page = 1, [FromQuery] int pageSize = 50,
        [FromQuery] string? field = null, [FromQuery] string? value = null, [FromQuery] DateTime? since = null, CancellationToken ct = default)
    {
        if (page < 1 || pageSize is < 1 or > MaxPageSize)
            return BadRequest(OmdbFormat.Error($"page from 1, pageSize 1–{MaxPageSize}."));
        try
        {
            var result = await library.ListAsync(collection, page, pageSize, field, value, since, ct);
            return result is null ? NotFound(OmdbFormat.Error($"No collection '{collection}'.")) : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(OmdbFormat.Error(ex.Message));
        }
    }

    /// <summary>One document by its _id (for Item, Episode, FileItem and most others: the IMDb id).</summary>
    /// <param name="collection">The collection, e.g. Item.</param>
    /// <param name="id">The document's _id, e.g. tt0133093 (KinoSeason/KolibriSeason: tt0903747_1).</param>
    /// <param name="ct"></param>
    [HttpGet("{collection}/{id}")]
    public async Task<IActionResult> Get(string collection, string id, CancellationToken ct)
    {
        var doc = await library.GetAsync(collection, id, ct);
        return doc is null ? NotFound(OmdbFormat.Error($"No '{id}' in {collection}.")) : Content(doc.ToString(), "application/json");
    }

    /// <summary>A binary field of a document, e.g. WatchListItem's Picture.</summary>
    [HttpGet("{collection}/{id}/{field}")]
    public async Task<IActionResult> Binary(string collection, string id, string field, CancellationToken ct)
    {
        var data = await library.BinaryAsync(collection, id, field, ct);
        return data is null ? NotFound(OmdbFormat.Error($"No binary field '{field}' for '{id}' in {collection}.")) : File(data, ContentType(data));
    }

    /// <summary>Stores a document as a whole: a change made in WinForms or the web app. Fields it leaves out become empty.</summary>
    /// <param name="collection">The collection, e.g. KinoRating or WatchListItem.</param>
    /// <param name="id">The document's _id; the body's _id, if given, must match.</param>
    /// <param name="doc">The document, as GET returns it. New fields become new columns.</param>
    /// <param name="ct"></param>
    [HttpPut("{collection}/{id}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Put(string collection, string id, [FromBody] JObject doc, CancellationToken ct)
    {
        if (doc["_id"] is { } bodyId && bodyId.Type != JTokenType.Null && bodyId.ToString() != id)
            return BadRequest(OmdbFormat.Error($"The body's _id ({bodyId}) is not '{id}'."));
        try
        {
            var stored = await writer.PutAsync(collection, id, doc, ct);
            if (stored is not null) metrics?.Write(collection, "put");
            return stored is null ? NotFound(OmdbFormat.Error($"No collection '{collection}'.")) : Content(stored.ToString(), "application/json");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(OmdbFormat.Error(ex.Message));
        }
    }

    /// <summary>
    /// Syncs documents both ways: each is merged with the stored one (what either lacks is filled from the other;
    /// when both have Updated, Fetched, Checked or Added, the newer one's values win). Returns the merged documents,
    /// for the client to store in its own LiteDB.
    /// </summary>
    /// <param name="collection">The collection, e.g. Item.</param>
    /// <param name="docs">Up to 1,000 documents, each with its _id.</param>
    /// <param name="ct"></param>
    [HttpPost("{collection}/sync")]
    [Consumes("application/json")]
    [ProducesResponseType<IReadOnlyList<SyncResult>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Sync(string collection, [FromBody] JArray docs, CancellationToken ct)
    {
        if (docs.Count > MaxSyncBatch) return BadRequest(OmdbFormat.Error($"At most {MaxSyncBatch} documents at a time."));
        if (docs.Any(d => d is not JObject)) return BadRequest(OmdbFormat.Error("The body must be a list of documents."));
        try
        {
            var results = await writer.SyncAsync(collection, docs.Cast<JObject>().ToList(), ct);
            foreach (var r in results ?? []) metrics?.SyncDocument(collection, r.Changed);
            return results is null ? NotFound(OmdbFormat.Error($"No collection '{collection}'.")) : Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(OmdbFormat.Error(ex.Message));
        }
    }

    /// <summary>Removes a document.</summary>
    [HttpDelete("{collection}/{id}")]
    public async Task<IActionResult> Delete(string collection, string id, CancellationToken ct)
    {
        var deleted = await writer.DeleteAsync(collection, id, ct);
        if (deleted == true) metrics?.Write(collection, "delete");
        return deleted switch
        {
            null => NotFound(OmdbFormat.Error($"No collection '{collection}'.")),
            false => NotFound(OmdbFormat.Error($"No '{id}' in {collection}.")),
            true => NoContent(),
        };
    }

    /// <summary>Stores an image in the cache (the request body is the image file; needs the key).</summary>
    /// <param name="id">The image's key, e.g. tt0133093.</param>
    /// <param name="ct"></param>
    [HttpPut("image/{**id}")]
    [Consumes("application/octet-stream", "image/jpeg", "image/png", "image/webp", "image/gif")]
    public async Task<IActionResult> PutImage(string id, CancellationToken ct)
    {
        var key = Uri.UnescapeDataString(id);
        // Kino's own keys say what the image is (tt… poster, tt…_S01E02 still, tmdb-person-… profile): stored with it,
        // so it is found by title and type. Other keys (addresses) are stored as they are.
        if (PosterService.KinoKey(key) is not { } meta) return await PutBinaryAsync("Image", key, "Data", ct);
        var data = await ReadBodyAsync(ct);
        if (data is null) return BadRequest(OmdbFormat.Error($"The body is empty or larger than {MaxBinaryBytes / 1024 / 1024} MB."));
        await writer.StoreImageAsync(key, data, meta.ImdbId, meta.Type, "kino", null, meta.Season, meta.Episode, meta.TmdbId, ct);
        metrics?.Write("Image", "binary");
        return NoContent();
    }

    /// <summary>Stores many whole documents in one go (for a client sending a batch, e.g. file dates). Each needs its _id.</summary>
    /// <param name="collection">The collection; a new one is made when KMDb doesn't have it yet.</param>
    /// <param name="docs">Up to 1,000 documents.</param>
    /// <param name="ct"></param>
    [HttpPut("{collection}")]
    [Consumes("application/json")]
    public async Task<IActionResult> PutMany(string collection, [FromBody] JArray docs, CancellationToken ct)
    {
        if (docs.Count > MaxSyncBatch) return BadRequest(OmdbFormat.Error($"At most {MaxSyncBatch} documents at a time."));
        if (docs.Any(d => d is not JObject o || o["_id"] is null)) return BadRequest(OmdbFormat.Error("The body must be a list of documents, each with its _id."));
        try
        {
            if (!await writer.PutManyAsync(collection, docs.Cast<JObject>().ToList(), ct))
                return NotFound(OmdbFormat.Error($"No collection '{collection}'."));
            metrics?.Write(collection, "put");
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(OmdbFormat.Error(ex.Message));
        }
    }

    /// <summary>Stores a binary field, e.g. WatchListItem's Picture (the request body is the data).</summary>
    [HttpPut("{collection}/{id}/{field}")]
    [Consumes("application/octet-stream", "image/jpeg", "image/png", "image/webp", "image/gif")]
    public Task<IActionResult> PutBinary(string collection, string id, string field, CancellationToken ct) =>
        PutBinaryAsync(collection, id, field, ct);

    private async Task<byte[]?> ReadBodyAsync(CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, ct);
        return buffer.Length is 0 or > MaxBinaryBytes ? null : buffer.ToArray();
    }

    private async Task<IActionResult> PutBinaryAsync(string collection, string id, string field, CancellationToken ct)
    {
        var data = await ReadBodyAsync(ct);
        if (data is null) return BadRequest(OmdbFormat.Error($"The body is empty or larger than {MaxBinaryBytes / 1024 / 1024} MB."));
        if (!await writer.PutBinaryAsync(collection, id, field, data, ct))
            return NotFound(OmdbFormat.Error($"No binary field '{field}' in {collection}."));
        metrics?.Write(collection, "binary");
        return NoContent();
    }

    private async Task<IActionResult> ImageAsync(string id, CancellationToken ct) => ImageResult(id, await library.ImageAsync(id, ct));

    private IActionResult ImageResult(string id, SilverScreenImage? image)
    {
        metrics?.Image(image is not null);
        if (image is null) return NotFound(OmdbFormat.Error($"No image '{id}'."));
        Response.Headers[HeaderNames.CacheControl] = "public, max-age=86400";
        return File(image.Data, ContentType(image.Data), image.Added is { } added ? new DateTimeOffset(added) : null, entityTag: null!);
    }

    /// <summary>The image type from the first bytes (the cache doesn't store it).</summary>
    internal static string ContentType(byte[] data) => data switch
    {
        [0xFF, 0xD8, ..] => "image/jpeg",
        [0x89, 0x50, 0x4E, 0x47, ..] => "image/png",
        [0x47, 0x49, 0x46, ..] => "image/gif",
        [0x52, 0x49, 0x46, 0x46, _, _, _, _, 0x57, 0x45, 0x42, 0x50, ..] => "image/webp",
        [0x42, 0x4D, ..] => "image/bmp",
        _ => "application/octet-stream",
    };
}
