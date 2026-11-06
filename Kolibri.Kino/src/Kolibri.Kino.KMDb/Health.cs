using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Newtonsoft.Json;

namespace Kolibri.Kino.KMDb;

/// <summary>The last health report, kept for the metrics (Grafana sees the same status as /health).</summary>
public sealed class HealthState
{
    public sealed record Check(string Name, HealthStatus Status, TimeSpan Duration);

    public DateTime StartedUtc { get; } = DateTime.UtcNow;

    public string Version { get; } =
        typeof(HealthState).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "?";

    public IReadOnlyList<Check> Checks { get; private set; } = [];

    public IReadOnlyList<SilverScreenCollection> Collections { get; set; } = [];

    public void Update(HealthReport report) =>
        Checks = report.Entries.Select(e => new Check(e.Key, e.Value.Status, e.Value.Duration)).ToList();
}

/// <summary>Runs the health checks every 30 seconds in the background and keeps the result for the metrics.</summary>
public sealed class HealthStatePublisher(HealthState state) : IHealthCheckPublisher
{
    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        state.Update(report);
        return Task.CompletedTask;
    }
}

/// <summary>The IMDb database answers and has its titles. Without it the OMDb API can't answer: unhealthy.</summary>
public sealed class ImdbHealthCheck(IImdbRepository imdb) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            return await imdb.PingAsync(ct)
                ? HealthCheckResult.Healthy("The IMDb database answers.")
                : HealthCheckResult.Unhealthy("The IMDb database has no titles.");
        }
        catch (Exception ex) when (ex is MySqlException or TimeoutException or InvalidOperationException)
        {
            return HealthCheckResult.Unhealthy("The IMDb database does not answer.", ex);
        }
    }
}

/// <summary>
/// The SilverScreen library in MariaDB answers. Without it OMDb answers lack the library's values and /silverscreen
/// fails, but the OMDb API still works: degraded.
/// </summary>
public sealed class SilverScreenHealthCheck(ISilverScreenRepository library, HealthState state) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var collections = await library.CollectionsAsync(ct);
            state.Collections = collections;
            if (collections.Count == 0) return HealthCheckResult.Degraded("The library has not been copied to MariaDB.");
            var data = new Dictionary<string, object>
            {
                ["collections"] = collections.Count,
                ["rows"] = collections.Sum(c => c.Rows),
                ["lastCopiedUtc"] = collections.Max(c => c.CopiedUtc),
            };
            return HealthCheckResult.Healthy("The SilverScreen library answers.", data);
        }
        catch (Exception ex) when (ex is MySqlException or TimeoutException or InvalidOperationException)
        {
            return HealthCheckResult.Degraded("The SilverScreen library does not answer.", ex);
        }
    }
}

/// <summary>
/// Plex's poster list (only with the Plex token). Without it posters only Plex has are not served, nothing else:
/// degraded. Off (no token) counts as healthy.
/// </summary>
public sealed class PlexHealthCheck(PlexPosterIndex plex) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        if (!plex.Enabled) return Task.FromResult(HealthCheckResult.Healthy("Plex posters are off (no Plex token)."));
        var (titles, loaded) = plex.Status;
        var data = new Dictionary<string, object> { ["titles"] = titles, ["loadedUtc"] = loaded };
        return Task.FromResult(titles > 0
            ? HealthCheckResult.Healthy("Plex's poster list is loaded.", data)
            : HealthCheckResult.Degraded("Plex's poster list is not loaded (Plex did not answer yet).", data: data));
    }
}

/// <summary>Writes /health as JSON: overall status, then each check with its status, time and details.</summary>
public static class HealthResponse
{
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var state = context.RequestServices.GetRequiredService<HealthState>();
        context.Response.ContentType = "application/json; charset=utf-8";
        var body = new
        {
            status = report.Status.ToString(),
            service = "Kolibri.Kino.KMDb",
            version = state.Version,
            uptimeSeconds = (long)(DateTime.UtcNow - state.StartedUtc).TotalSeconds,
            durationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 1),
            checks = report.Entries.ToDictionary(e => e.Key, e => new
            {
                status = e.Value.Status.ToString(),
                durationMs = Math.Round(e.Value.Duration.TotalMilliseconds, 1),
                description = e.Value.Description,
                // The message only (no stack trace): what failed, e.g. "Unable to connect to any of the specified MySQL hosts."
                error = e.Value.Exception?.Message,
                data = e.Value.Data.Count == 0 ? null : e.Value.Data,
            }),
        };
        return context.Response.WriteAsync(JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}
