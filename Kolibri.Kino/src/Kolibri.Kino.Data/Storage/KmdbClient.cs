using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using LiteDB;
using Microsoft.Extensions.Options;

namespace Kolibri.Kino.Data.Storage;

/// <summary>KMDb answered with an error (not "not found"): the caller falls back to the LiteDB file.</summary>
public sealed class KmdbUnavailableException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// HTTP calls to Kolibri.Kino.KMDb's /silverscreen API: the library's collections as JSON documents, and images.
/// The address is KinoOptions.KmdbUrl (Docker) or the user's "KMDb URL"; the key is the Uranus token.
/// </summary>
public sealed class KmdbClient(ISettingsStore settings, IOptions<KinoOptions> options, HttpMessageHandler? handler = null)
{
    /// <summary>Most documents KMDb returns in one page.</summary>
    public const int PageSize = 500;

    private readonly HttpClient _http = handler is null
        ? new HttpClient { Timeout = TimeSpan.FromSeconds(30) }
        : new HttpClient(handler, disposeHandler: false) { Timeout = TimeSpan.FromSeconds(30) };

    /// <summary>KMDb's base address ending in "/", or null when none is set.</summary>
    public string? BaseUrl
    {
        get
        {
            var configured = options.Value.KmdbUrl;
            if (!string.IsNullOrWhiteSpace(configured))
                return new UserSettings { KmdbUrl = configured }.GetKmdbBaseUrl();
            return settings.Current.GetKmdbBaseUrl();
        }
    }

    public string? ApiKey => settings.Current.UranusApiKey is { Length: > 0 } key ? key.Trim() : null;

    public bool IsConfigured => BaseUrl is not null && ApiKey is not null;

    /// <summary>/ping: KMDb runs (no key needed, no database touched).</summary>
    public async Task<bool> PingAsync(CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(5));
        try
        {
            using var response = await _http.GetAsync(BaseUrl + "ping", cts.Token).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            return false;
        }
    }

    /// <summary>GET /silverscreen with the key: null when the library answers, else why not.</summary>
    public async Task<string?> CheckLibraryAsync(CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(10));
        try
        {
            using var response = await SendAsync(HttpMethod.Get, "silverscreen", null, cts.Token).ConfigureAwait(false);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => null,
                HttpStatusCode.Unauthorized => "KMDb did not accept the key (the Uranus token).",
                _ => $"KMDb's library answered {(int)response.StatusCode}.",
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            return $"KMDb's library did not answer ({ex.Message}).";
        }
    }

    public async Task<BsonDocument?> GetAsync(string collection, BsonValue id, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"silverscreen/{Seg(collection)}/{Seg(Id(id))}", null, ct).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureAsync(response, ct).ConfigureAwait(false);
        using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
        return BsonJson.ToDocument(json.RootElement);
    }

    /// <summary>All documents, or those where a field has a value, page by page. A collection KMDb doesn't have is empty.</summary>
    public async Task<IReadOnlyList<BsonDocument>> ListAsync(string collection, string? field, BsonValue? value, CancellationToken ct)
    {
        var docs = new List<BsonDocument>();
        for (var page = 1; ; page++)
        {
            var query = $"page={page}&pageSize={PageSize}";
            if (field is not null) query += $"&field={Uri.EscapeDataString(field)}" + (value is null || value.IsNull ? "" : $"&value={Uri.EscapeDataString(Id(value))}");
            using var response = await SendAsync(HttpMethod.Get, $"silverscreen/{Seg(collection)}?{query}", null, ct).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound) return docs;
            await EnsureAsync(response, ct).ConfigureAwait(false);
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
            var items = json.RootElement.GetProperty("Items");
            foreach (var item in items.EnumerateArray()) docs.Add(BsonJson.ToDocument(item));
            var total = json.RootElement.GetProperty("Total").GetInt32();
            if (items.GetArrayLength() < PageSize || docs.Count >= total) return docs;
        }
    }

    /// <summary>Changed documents since a time (UTC), for keeping the LiteDB file up to date.</summary>
    public async Task<IReadOnlyList<BsonDocument>> ChangedSinceAsync(string collection, DateTime sinceUtc, CancellationToken ct)
    {
        var docs = new List<BsonDocument>();
        var since = Uri.EscapeDataString(sinceUtc.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture));
        for (var page = 1; ; page++)
        {
            using var response = await SendAsync(HttpMethod.Get, $"silverscreen/{Seg(collection)}?page={page}&pageSize={PageSize}&since={since}", null, ct).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound) return docs;
            await EnsureAsync(response, ct).ConfigureAwait(false);
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);
            var items = json.RootElement.GetProperty("Items");
            foreach (var item in items.EnumerateArray()) docs.Add(BsonJson.ToDocument(item));
            if (items.GetArrayLength() < PageSize) return docs;
        }
    }

    public async Task PutAsync(string collection, BsonDocument doc, CancellationToken ct)
    {
        var body = new StringContent(BsonJson.Serialize(doc), Encoding.UTF8, "application/json");
        using var response = await SendAsync(HttpMethod.Put, $"silverscreen/{Seg(collection)}/{Seg(Id(doc["_id"]))}", body, ct).ConfigureAwait(false);
        await EnsureAsync(response, ct).ConfigureAwait(false);
    }

    /// <summary>Many whole documents in one request (PUT /silverscreen/{collection} with a list).</summary>
    public async Task PutManyAsync(string collection, IReadOnlyCollection<BsonDocument> docs, CancellationToken ct)
    {
        var body = new StringContent(BsonJson.Serialize(new BsonArray(docs)), Encoding.UTF8, "application/json");
        using var response = await SendAsync(HttpMethod.Put, $"silverscreen/{Seg(collection)}", body, ct).ConfigureAwait(false);
        await EnsureAsync(response, ct).ConfigureAwait(false);
    }

    public async Task<bool> DeleteAsync(string collection, BsonValue id, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Delete, $"silverscreen/{Seg(collection)}/{Seg(Id(id))}", null, ct).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        await EnsureAsync(response, ct).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// An image from KMDb's cache. IMDb ids (and tt…_S01E02) go through /poster, which finds the best poster (or the
    /// episode's still) and can fetch one KMDb doesn't have yet; other keys through /image.
    /// </summary>
    public async Task<byte[]?> GetImageAsync(string key, CancellationToken ct)
    {
        var path = key.StartsWith("tt", StringComparison.Ordinal) && !key.Contains('/') ? $"silverscreen/poster/{Seg(key)}" : $"silverscreen/image/{Seg(key)}";
        using var response = await SendAsync(HttpMethod.Get, path, null, ct).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureAsync(response, ct).ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
    }

    /// <summary>The best image of a type for a title (/silverscreen/art/{id}/{type}), or null.</summary>
    public async Task<byte[]?> GetArtAsync(string imdbId, string type, CancellationToken ct)
    {
        using var response = await SendAsync(HttpMethod.Get, $"silverscreen/art/{Seg(imdbId)}/{Seg(type)}", null, ct).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureAsync(response, ct).ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
    }

    public async Task PutImageAsync(string key, byte[] data, CancellationToken ct)
    {
        var body = new ByteArrayContent(data);
        body.Headers.ContentType = new MediaTypeHeaderValue(ImageInfo.ContentType(data) is var type && type.StartsWith("image/", StringComparison.Ordinal) ? type : "application/octet-stream");
        using var response = await SendAsync(HttpMethod.Put, $"silverscreen/image/{Seg(key)}", body, ct).ConfigureAwait(false);
        await EnsureAsync(response, ct).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, HttpContent? content, CancellationToken ct)
    {
        var baseUrl = BaseUrl ?? throw new KmdbUnavailableException("No KMDb URL is set.");
        using var request = new HttpRequestMessage(method, baseUrl + path) { Content = content };
        if (ApiKey is { } key) request.Headers.Add("X-Api-Key", key);
        try
        {
            return await _http.SendAsync(request, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            throw new KmdbUnavailableException($"KMDb did not answer: {ex.Message}", ex);
        }
    }

    private static async Task EnsureAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;
        var text = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        throw new KmdbUnavailableException($"KMDb answered {(int)response.StatusCode}: {text[..Math.Min(300, text.Length)]}");
    }

    /// <summary>An id as text in a URL: strings as they are, ObjectIds as hex, numbers as digits.</summary>
    internal static string Id(BsonValue id) => id.Type switch
    {
        BsonType.String => id.AsString,
        BsonType.ObjectId => id.AsObjectId.ToString(),
        BsonType.Int32 or BsonType.Int64 or BsonType.Double or BsonType.Decimal => Convert.ToString(id.RawValue, System.Globalization.CultureInfo.InvariantCulture)!,
        _ => id.ToString(),
    };

    private static string Seg(string text) => Uri.EscapeDataString(text);
}
