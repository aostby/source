using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// KMDb's own metrics (OpenTelemetry meter "Kolibri.Kino.KMDb"), exported at /metrics for Prometheus/Grafana and over
/// OTLP when OTEL_EXPORTER_OTLP_ENDPOINT is set. In Prometheus the names get underscores and _total on counters:
/// kmdb_omdb_requests_total, kmdb_health_status, kmdb_library_rows …
/// </summary>
public sealed class KmdbMetrics : IDisposable
{
    public const string MeterName = "Kolibri.Kino.KMDb";

    private readonly Meter _meter;
    private readonly Counter<long> _omdbRequests;
    private readonly Counter<long> _enriched;
    private readonly Counter<long> _syncDocuments;
    private readonly Counter<long> _writes;
    private readonly Counter<long> _images;
    private readonly Counter<long> _posterFetches;
    private readonly HealthState _health;

    public KmdbMetrics(IMeterFactory meters, HealthState health)
    {
        _health = health;
        _meter = meters.Create(MeterName);
        _omdbRequests = _meter.CreateCounter<long>("kmdb.omdb.requests", "{request}",
            "OMDb-style lookups, by kind (id, title, search, season, episode) and result (found, not_found, error).");
        _enriched = _meter.CreateCounter<long>("kmdb.library.enriched", "{answer}",
            "OMDb answers that got values from the SilverScreen library, by kind (item, episode, season, search, library_only).");
        _syncDocuments = _meter.CreateCounter<long>("kmdb.sync.documents", "{document}",
            "Documents merged by POST /silverscreen/{collection}/sync, by collection and whether they changed.");
        _writes = _meter.CreateCounter<long>("kmdb.library.writes", "{write}",
            "Writes to the library, by collection and operation (put, delete, binary).");
        _images = _meter.CreateCounter<long>("kmdb.images.served", "{image}", "Image requests, by whether the image was found.");
        _posterFetches = _meter.CreateCounter<long>("kmdb.posters.fetched", "{poster}",
            "Posters not in the cache, fetched from TMDb or another public address (found) or not to be had (not found).");

        _meter.CreateObservableGauge("kmdb.health.status", () => _health.Checks.Select(c =>
                new Measurement<double>(Score(c.Status), new KeyValuePair<string, object?>("check", c.Name))),
            description: "Last health check per dependency: 1 healthy, 0.5 degraded, 0 unhealthy.");
        _meter.CreateObservableGauge("kmdb.health.check.duration", () => _health.Checks.Select(c =>
                new Measurement<double>(c.Duration.TotalSeconds, new KeyValuePair<string, object?>("check", c.Name))),
            unit: "s", description: "How long the last health check per dependency took.");
        _meter.CreateObservableGauge("kmdb.library.rows", () => _health.Collections.Select(c =>
                new Measurement<long>(c.Rows, new KeyValuePair<string, object?>("collection", c.Name))),
            unit: "{row}", description: "Rows per SilverScreen collection when it was last copied.");
        _meter.CreateObservableGauge("kmdb.library.copy.age", () => _health.Collections.Select(c =>
                new Measurement<double>((DateTime.UtcNow - c.CopiedUtc).TotalSeconds, new KeyValuePair<string, object?>("collection", c.Name))),
            unit: "s", description: "Time since each SilverScreen collection was copied from LiteDB.");
        _meter.CreateObservableGauge("kmdb.uptime", () => (DateTime.UtcNow - _health.StartedUtc).TotalSeconds,
            unit: "s", description: "Time since the service started.");
    }

    public void OmdbRequest(string kind, string result) =>
        _omdbRequests.Add(1, new KeyValuePair<string, object?>("kind", kind), new KeyValuePair<string, object?>("result", result));

    public void Enriched(string kind) => _enriched.Add(1, new KeyValuePair<string, object?>("kind", kind));

    public void SyncDocument(string collection, bool changed) =>
        _syncDocuments.Add(1, new KeyValuePair<string, object?>("collection", collection), new KeyValuePair<string, object?>("changed", changed));

    public void Write(string collection, string operation) =>
        _writes.Add(1, new KeyValuePair<string, object?>("collection", collection), new KeyValuePair<string, object?>("operation", operation));

    public void Image(bool found) => _images.Add(1, new KeyValuePair<string, object?>("found", found));

    public void PosterFetched(bool found) => _posterFetches.Add(1, new KeyValuePair<string, object?>("found", found));

    internal static double Score(HealthStatus status) => status switch
    {
        HealthStatus.Healthy => 1,
        HealthStatus.Degraded => 0.5,
        _ => 0,
    };

    public void Dispose() => _meter.Dispose();
}
