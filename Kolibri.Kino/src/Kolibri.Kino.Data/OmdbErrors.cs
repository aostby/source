using System.Text.Json;

namespace Kolibri.Kino.Data;

/// <summary>
/// OMDb's real error message when OMDbApiNet can't tell.
/// </summary>
/// <remarks>
/// When OMDb answers HTTP 401 (e.g. {"Response":"False","Error":"Request limit reached!"} or "Invalid API key!"),
/// OMDbApiNet 1.3.0 throws a NullReferenceException instead of reporting the error. This asks OMDb once more,
/// directly, and returns its "Error" text.
/// </remarks>
internal static class OmdbErrors
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    /// <summary>True for the exception OMDbApiNet throws on an HTTP 401 answer.</summary>
    public static bool IsLibraryCrash(Exception ex) =>
        ex is NullReferenceException && ex.StackTrace?.Contains("OMDbApiNet", StringComparison.Ordinal) == true;

    /// <summary>
    /// "OMDb: <paramref name="message"/>", plus what to do when it's about the key or the daily limit: with the shared
    /// test key, get a key of your own; else check the key in Settings.
    /// </summary>
    public static string WithKeyHint(string message, bool usesTestKey)
    {
        var aboutKey = message.Contains("key", StringComparison.OrdinalIgnoreCase)
                       || message.Contains("limit", StringComparison.OrdinalIgnoreCase);
        if (!aboutKey) return $"OMDb: {message}";
        return usesTestKey
            ? $"OMDb: {message} You're using the shared test key, whose daily limit everyone shares. " +
              "Get a free key of your own (Help, Prerequisites) and enter it in Settings."
            : $"OMDb: {message} Check the OMDb key in Settings.";
    }

    /// <summary>OMDb's error for this key, e.g. "Request limit reached!", or a generic text if it can't be read.</summary>
    public static async Task<string> ExplainAsync(string apiKey, CancellationToken ct)
    {
        try
        {
            using var response = await Http.GetAsync($"https://www.omdbapi.com/?i=tt0133093&apikey={Uri.EscapeDataString(apiKey)}", ct)
                .ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("Error", out var error) && error.GetString() is { Length: > 0 } message)
                return message;
            return response.IsSuccessStatusCode ? "OMDb answered, but the request failed." : $"OMDb answered HTTP {(int)response.StatusCode}.";
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            return $"OMDb refused the request ({ex.Message}).";
        }
    }
}
