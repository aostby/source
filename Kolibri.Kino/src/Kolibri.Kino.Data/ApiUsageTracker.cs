using System.Diagnostics.Metrics;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Data;

/// <summary>
/// Counts every HTTP request this process makes to OMDb, TMDb, SubDL or Plex, including those made inside
/// OMDbApiNet and TMDbLib, by listening to .NET's own "http.client.request.duration" measurement
/// (one per finished request, with the server's address and the status code). Counts are kept in memory and
/// saved every half minute, before a report, and when the program closes.
/// </summary>
public sealed class ApiUsageTracker : IApiUsageTracker, IDisposable
{
    private static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(30);

    private readonly IApiUsageRepository _repository;
    private readonly ISettingsStore _settings;
    private readonly MeterListener? _listener;
    private readonly Timer _timer;
    private readonly SemaphoreSlim _saving = new(1, 1);
    private readonly Lock _lock = new();
    private Dictionary<(DateOnly Date, string Service), ApiCallCount> _pending = [];

    public ApiUsageTracker(IApiUsageRepository repository, ISettingsStore settings)
        : this(repository, settings, listen: true)
    {
    }

    /// <summary><paramref name="listen"/> false: only <see cref="Record"/> counts (for tests).</summary>
    internal ApiUsageTracker(IApiUsageRepository repository, ISettingsStore settings, bool listen)
    {
        _repository = repository;
        _settings = settings;
        if (listen)
        {
            _listener = new MeterListener
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    if (instrument is { Meter.Name: "System.Net.Http", Name: "http.client.request.duration" })
                        listener.EnableMeasurementEvents(instrument);
                },
            };
            _listener.SetMeasurementEventCallback<double>(OnRequestFinished);
            _listener.Start();
        }
        _timer = new Timer(_ => _ = SaveQuietlyAsync(), null, SaveInterval, SaveInterval);
    }

    public void Record(string service, bool failed, DateOnly? date = null)
    {
        var key = (date ?? DateOnly.FromDateTime(DateTime.Now), service);
        lock (_lock)
            _pending[key] = (_pending.TryGetValue(key, out var count) ? count : ApiCallCount.Zero) + new ApiCallCount(1, failed ? 1 : 0);
    }

    public async Task FlushAsync(CancellationToken ct = default)
    {
        await _saving.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            Dictionary<(DateOnly Date, string Service), ApiCallCount> taken;
            lock (_lock)
            {
                if (_pending.Count == 0) return;
                taken = _pending;
                _pending = [];
            }

            try
            {
                await _repository.AddAsync(taken.Select(p => new ApiUsageEntry(p.Key.Date, p.Key.Service, p.Value)).ToList(), ct)
                    .ConfigureAwait(false);
            }
            catch
            {
                // Keep the counts for the next save.
                lock (_lock)
                    foreach (var (key, count) in taken)
                        _pending[key] = (_pending.TryGetValue(key, out var newer) ? newer : ApiCallCount.Zero) + count;
                throw;
            }
        }
        finally
        {
            _saving.Release();
        }
    }

    private void OnRequestFinished(Instrument instrument, double duration, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        string? host = null;
        int? port = null, status = null;
        var error = false;
        foreach (var tag in tags)
        {
            switch (tag.Key)
            {
                case "server.address": host = tag.Value as string; break;
                case "server.port": port = tag.Value as int?; break;
                case "http.response.status_code": status = tag.Value as int?; break;
                case "error.type": error = true; break;
            }
        }

        if (ApiServiceHosts.Classify(host, port, PlexServer()) is { } service)
            Record(service, error || status >= 400);
    }

    private string? PlexServer()
    {
        try
        {
            return _settings.Current.XPlexServerName;
        }
        catch (Exception)
        {
            // Never let counting break a request (e.g. the database is closing).
            return null;
        }
    }

    private async Task SaveQuietlyAsync()
    {
        try
        {
            await FlushAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Tried again at the next save.
            System.Diagnostics.Debug.WriteLine($"API usage not saved: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _listener?.Dispose();
        _timer.Dispose();
        SaveQuietlyAsync().GetAwaiter().GetResult();
        _saving.Dispose();
    }
}

/// <summary>Which service a request went to, from the server's address.</summary>
internal static class ApiServiceHosts
{
    public const int PlexPort = 32400;

    /// <summary>
    /// omdbapi.com (also img.omdbapi.com), api.themoviedb.org (not its image server), subdl.com, and Plex:
    /// port 32400, the Plex server name from Settings, plex.tv or *.plex.direct. Null for anything else (e.g. posters).
    /// </summary>
    public static string? Classify(string? host, int? port, string? plexServer)
    {
        if (string.IsNullOrWhiteSpace(host)) return null;
        if (Is(host, "omdbapi.com")) return ApiServices.Omdb;
        if (host.Equals("api.themoviedb.org", StringComparison.OrdinalIgnoreCase)) return ApiServices.Tmdb;
        if (Is(host, "subdl.com")) return ApiServices.SubDl;
        if (port == PlexPort || Is(host, "plex.tv") || Is(host, "plex.direct")
            || (!string.IsNullOrWhiteSpace(plexServer) && host.Equals(plexServer.Trim(), StringComparison.OrdinalIgnoreCase)))
            return ApiServices.Plex;
        return null;

        static bool Is(string host, string domain) =>
            host.Equals(domain, StringComparison.OrdinalIgnoreCase) || host.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase);
    }
}
