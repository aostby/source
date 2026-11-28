using System.Diagnostics.Metrics;
using System.Reflection;
using Kolibri.Kino.Core.Abstractions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Monitoring for the web app, like KMDb's: /health (the library and where it is stored, as JSON), and /metrics
/// (OpenTelemetry in Prometheus format, for Grafana). Both answer without a login, as the rest of the app.
/// </summary>
public static class KinoMonitoring
{
    public const string MeterName = "Kolibri.Kino.Blazor";

    public static IServiceCollection AddKinoMonitoring(this IServiceCollection services)
    {
        services.AddSingleton<KinoHealthState>();
        services.AddSingleton<IHealthCheckPublisher, KinoHealthPublisher>();
        services.Configure<HealthCheckPublisherOptions>(o =>
        {
            o.Delay = TimeSpan.FromSeconds(10);
            o.Period = TimeSpan.FromSeconds(30);
        });
        services.AddHealthChecks()
            .AddCheck<LibraryHealthCheck>("library", HealthStatus.Unhealthy, timeout: TimeSpan.FromSeconds(30))
            .AddCheck<StorageHealthCheck>("storage", HealthStatus.Degraded);

        services.AddSingleton<KinoMetrics>();
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("kolibri-kino", serviceVersion: typeof(KinoMonitoring).Assembly.GetName().Version?.ToString()))
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()   // http.server.request.duration per route and status, Blazor circuits
                .AddRuntimeInstrumentation()      // GC, memory, thread pool
                .AddMeter(MeterName)
                .AddPrometheusExporter());
        return services;
    }

    public static WebApplication MapKinoMonitoring(this WebApplication app)
    {
        // Resolved here so the gauges exist from the first scrape.
        app.Services.GetRequiredService<KinoMetrics>();
        app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteHealthAsync });
        app.MapPrometheusScrapingEndpoint("/metrics");
        return app;
    }

    private static Task WriteHealthAsync(HttpContext context, HealthReport report)
    {
        var state = context.RequestServices.GetRequiredService<KinoHealthState>();
        context.Response.ContentType = "application/json; charset=utf-8";
        var body = new
        {
            status = report.Status.ToString(),
            service = "Kolibri.Kino.Blazor",
            version = state.Version,
            uptimeSeconds = (long)(DateTime.UtcNow - state.StartedUtc).TotalSeconds,
            durationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 1),
            checks = report.Entries.ToDictionary(e => e.Key, e => new
            {
                status = e.Value.Status.ToString(),
                durationMs = Math.Round(e.Value.Duration.TotalMilliseconds, 1),
                description = e.Value.Description,
                error = e.Value.Exception?.Message,
                data = e.Value.Data.Count == 0 ? null : e.Value.Data,
            }),
        };
        return context.Response.WriteAsync(JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}

/// <summary>The last health report, for the metrics (Grafana sees what /health says).</summary>
public sealed class KinoHealthState
{
    public DateTime StartedUtc { get; } = DateTime.UtcNow;

    public string Version { get; } =
        typeof(KinoHealthState).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "?";

    public IReadOnlyList<(string Name, HealthStatus Status, TimeSpan Duration)> Checks { get; private set; } = [];

    public void Update(HealthReport report) =>
        Checks = report.Entries.Select(e => (e.Key, e.Value.Status, e.Value.Duration)).ToList();
}

public sealed class KinoHealthPublisher(KinoHealthState state) : IHealthCheckPublisher
{
    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        state.Update(report);
        return Task.CompletedTask;
    }
}

/// <summary>The library answers (a lookup in the LiteDB file or in KMDb, whichever is in use). Without it Kino shows nothing: unhealthy.</summary>
public sealed class LibraryHealthCheck(IMovieRepository movies) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await movies.GetByImdbIdAsync("tt0000000", ct);
            return HealthCheckResult.Healthy("The library answers.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("The library does not answer.", ex);
        }
    }
}

/// <summary>
/// Where the library is: MariaDB through KMDb is healthy; the LiteDB file is healthy when no KMDb is set up, and
/// degraded when one is set up but doesn't answer (Kino then keeps changes for KMDb until it does).
/// </summary>
public sealed class StorageHealthCheck(IStorageStatus? storage = null) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        if (storage is null) return Task.FromResult(HealthCheckResult.Healthy("The LiteDB file."));
        var state = storage.Current;
        var data = new Dictionary<string, object> { ["storage"] = state.Active.ToString(), ["pendingChanges"] = state.PendingChanges };
        if (state.KmdbUrl is { } url) data["kmdbUrl"] = url;
        return Task.FromResult(state.UsesKmdb || state.KmdbUrl is null
            ? HealthCheckResult.Healthy(state.Message, data)
            : HealthCheckResult.Degraded(state.Message, data: data));
    }
}

/// <summary>The web app's own metrics: health per check, storage in use, changes waiting for KMDb, uptime.</summary>
public sealed class KinoMetrics : IDisposable
{
    private readonly Meter _meter;

    public KinoMetrics(IMeterFactory meters, KinoHealthState health, IStorageStatus? storage = null)
    {
        _meter = meters.Create(KinoMonitoring.MeterName);
        _meter.CreateObservableGauge("kino.health.status", () => health.Checks.Select(c =>
                new Measurement<double>(c.Status switch { HealthStatus.Healthy => 1, HealthStatus.Degraded => 0.5, _ => 0 },
                    new KeyValuePair<string, object?>("check", c.Name))),
            description: "Last health check per part: 1 healthy, 0.5 degraded, 0 unhealthy.");
        _meter.CreateObservableGauge("kino.storage.kmdb", () => storage?.Current.UsesKmdb == true ? 1 : 0,
            description: "1 while the library is in MariaDB through KMDb, 0 while it is in the LiteDB file.");
        _meter.CreateObservableGauge("kino.storage.pending", () => storage?.Current.PendingChanges ?? 0,
            unit: "{change}", description: "Changes made in the LiteDB file while KMDb didn't answer, not yet sent there.");
        _meter.CreateObservableGauge("kino.uptime", () => (DateTime.UtcNow - health.StartedUtc).TotalSeconds,
            unit: "s", description: "Time since the web app started.");
    }

    public void Dispose() => _meter.Dispose();
}
