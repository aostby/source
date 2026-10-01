using Kolibri.Kino.Core;
using Kolibri.Kino.Core.Abstractions;
using OMDbApiNet;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Data;

/// <summary>
/// <see cref="IMovieInfoProvider"/> backed by OMDbApiNet's async client, using the OMDb key from the user settings
/// (a new key takes effect on the next call).
/// </summary>
/// <remarks>
/// OMDbApiNet reports every OMDb error ("Movie not found!", "Request limit reached!", "Invalid API key!")
/// as an HttpRequestException carrying OMDb's message. Not-found becomes null; the rest become
/// <see cref="MovieInfoUnavailableException"/> so a long scan stops instead of failing every file.
/// </remarks>
public sealed class OmdbMovieInfoProvider(ISettingsStore settings) : IMovieInfoProvider
{
    private (string Key, IAsyncOmdbClient Client)? _client;

    public async Task<IReadOnlyList<SearchItem>> SearchAsync(string query, int page = 1, CancellationToken ct = default)
    {
        var result = await Call(c => c.GetSearchListAsync(query, page), ct).ConfigureAwait(false);
        return result?.SearchResults ?? [];
    }

    public async Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default)
    {
        var item = await Call(c => c.GetItemByIdAsync(imdbId, fullPlot: true), ct).ConfigureAwait(false);
        return item?.Response == "True" ? item : null;
    }

    public async Task<Item?> GetMovieByTitleAsync(string title, int? year, CancellationToken ct = default)
    {
        var item = await Call(c => c.GetItemByTitleAsync(title, OmdbType.Movie, year, fullPlot: true), ct).ConfigureAwait(false);
        return item?.Response == "True" ? item : null;
    }

    public async Task<Item?> GetSeriesByTitleAsync(string title, int? year, CancellationToken ct = default)
    {
        var item = await Call(c => c.GetItemByTitleAsync(title, OmdbType.Series, year, fullPlot: true), ct).ConfigureAwait(false);
        return item?.Response == "True" ? item : null;
    }

    private IAsyncOmdbClient Client()
    {
        var key = settings.Current.OMDBkey;
        if (string.IsNullOrWhiteSpace(key))
            throw new MovieInfoUnavailableException("OMDb: no API key. Set the OMDb key in Settings.");

        var cached = _client;
        if (cached is { } c && c.Key == key) return c.Client;

        var client = new AsyncOmdbClient(key, false);
        _client = (key, client);
        return client;
    }

    private async Task<T?> Call<T>(Func<IAsyncOmdbClient, Task<T>> request, CancellationToken ct) where T : class
    {
        try
        {
            // OMDbApiNet has no CancellationToken support, so we stop waiting instead.
            return await request(Client()).WaitAsync(ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (IsOmdbMessage(ex))
        {
            if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("Incorrect IMDb ID", StringComparison.OrdinalIgnoreCase))
                return null;

            throw new MovieInfoUnavailableException($"OMDb: {ex.Message}", ex);
        }
        catch (NullReferenceException ex) when (OmdbErrors.IsLibraryCrash(ex))
        {
            // OMDbApiNet crashes on HTTP 401 (daily limit, bad key); ask OMDb for the real message.
            var message = await OmdbErrors.ExplainAsync(settings.Current.OMDBkey ?? "", ct).ConfigureAwait(false);
            throw new MovieInfoUnavailableException($"OMDb: {message}", ex);
        }
    }

    /// <summary>An error OMDb answered with, rather than a network failure.</summary>
    private static bool IsOmdbMessage(HttpRequestException ex) => ex.StatusCode is null && ex.InnerException is null;
}
