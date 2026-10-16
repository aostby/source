using Kolibri.Kino.Controllers;
using Kolibri.Kino.Controllers.Watchlists;

namespace Kolibri.Kino.Blazor;

/// <summary>
/// Loads Plex's movie list once the app has started, so the Watched filter (Not watched / Watched) answers at once the
/// first time it is chosen instead of waiting for Plex (seconds for a big library). Read-only: nothing is written.
/// When Plex is busy (it answers 500 or cut-off XML during its maintenance), it tries again a few times.
/// </summary>
public sealed class WatchedWarmUp(IServiceProvider services, ILogger<WatchedWarmUp> logger) : BackgroundService
{
    private const int Attempts = 4;
    private static readonly TimeSpan RetryAfter = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!services.GetRequiredService<WatchlistController>().PlexConfigured) return;
            var movies = services.GetRequiredService<MovieController>();
            for (var attempt = 1; attempt <= Attempts; attempt++)
            {
                var (watched, note) = await movies.GetWatchedAsync(refreshPlex: attempt > 1, stoppingToken);
                if (note is null)
                {
                    logger.LogInformation("Watched ready: {Count} title(s) watched on Plex or a watchlist", watched.Count);
                    return;
                }
                logger.LogWarning("Plex's movie list not loaded at startup (attempt {Attempt} of {Attempts}): {Note}", attempt, Attempts, note);
                if (attempt < Attempts) await Task.Delay(RetryAfter, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Loading what's watched at startup failed; the Watched filter will ask when chosen");
        }
    }
}
