using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Kolibri.Kino.KMDb;

/// <summary>Settings for the service (section "Kmdb").</summary>
public sealed class KmdbOptions
{
    /// <summary>
    /// The key clients send, as OMDb's apikey= or the X-Api-Key header. The same key as the Uranus web service
    /// (kino_config.php, UranusApiKey in Kino's settings).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The Plex token (XPlexToken in Kino's settings), for posters that only Plex has. It stays in the service: it is
    /// sent to Plex in a header and never appears in an answer or a log. Empty: Plex posters are not fetched.
    /// </summary>
    public string? PlexToken { get; set; }

    /// <summary>The Plex server (default http://HTPC:32400). In Docker, compose maps the name HTPC to its address.</summary>
    public string? PlexUrl { get; set; }
}

/// <summary>
/// Lets a request through only with the right key, from apikey= (as OMDb) or the X-Api-Key header. The Swagger page,
/// /health and the library's images (/silverscreen/poster, /silverscreen/image: used in &lt;img src&gt;, where a key
/// would end up in web pages) need no key. Answers like OMDb: 401 with {"Response":"False","Error":"…"}.
/// </summary>
public sealed class ApiKeyMiddleware(RequestDelegate next, Microsoft.Extensions.Options.IOptions<KmdbOptions> options)
{
    public const string HeaderName = "X-Api-Key";
    public const string QueryName = "apikey";

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsOpen(context.Request.Path, context.Request.Method))
        {
            await next(context);
            return;
        }

        var sent = context.Request.Query[QueryName].FirstOrDefault() ?? context.Request.Headers[HeaderName].FirstOrDefault();
        var error = Check(sent, options.Value.ApiKey);
        if (error is null)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(OmdbFormat.Error(error)));
    }

    /// <summary>Null when <paramref name="sent"/> is the key; otherwise OMDb's message.</summary>
    internal static string? Check(string? sent, string? expected)
    {
        if (string.IsNullOrEmpty(sent)) return "No API key provided.";
        if (string.IsNullOrEmpty(expected)) return "Invalid API key!";
        var a = SHA256.HashData(Encoding.UTF8.GetBytes(sent));
        var b = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return CryptographicOperations.FixedTimeEquals(a, b) ? null : "Invalid API key!";
    }

    /// <summary>Open without a key: Swagger, monitoring (/ping, /health, /metrics), and reading the library's images.</summary>
    internal static bool IsOpen(PathString path, string method = "GET") =>
        path.StartsWithSegments("/swagger") || IsMonitoring(path) || path.Equals("/favicon.ico")
        || ((HttpMethods.IsGet(method) || HttpMethods.IsHead(method))
            && (path.StartsWithSegments("/silverscreen/poster", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/silverscreen/image", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/silverscreen/art", StringComparison.OrdinalIgnoreCase)));

    /// <summary>The monitoring endpoints: polled often, open without a key, and not traced.</summary>
    internal static bool IsMonitoring(PathString path) =>
        path.StartsWithSegments("/ping") || path.StartsWithSegments("/health") || path.StartsWithSegments("/metrics");
}
