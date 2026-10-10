using Kolibri.Kino.Controllers.Library;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Reads the library's file dates once the app has started, so Recently added and the Library shelf are ready
/// when someone opens them. The first run after a deploy walks the disks (minutes on a NAS with cold disks);
/// later starts only read the stored dates.
/// </summary>
public sealed class RecentlyAddedWarmUp(IServiceProvider services, ILogger<RecentlyAddedWarmUp> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var recent = services.GetRequiredService<RecentlyAddedController>();
            var report = await recent.GetAsync(stoppingToken);
            logger.LogInformation("Recently added ready: {Count} item(s), {Missing} without a reachable file", report.Items.Count, report.WithoutDate);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Reading file dates at startup failed; Recently added will read them when opened");
        }
    }
}
