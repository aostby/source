using Kolibri.Kino.Controllers.Cleanup;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.LocalMovies;
using Kolibri.Kino.Controllers.Logging;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Controllers.Scanning;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Controllers.Settings;
using Kolibri.Kino.Controllers.Subtitles;
using Kolibri.Kino.Controllers.Usage;
using Kolibri.Kino.Controllers.Watchlists;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.Controllers;

public static class ControllerServiceCollectionExtensions
{
    public static IServiceCollection AddKinoControllers(this IServiceCollection services)
    {
        services.AddTransient<MovieLinker>();
        services.AddTransient<MovieController>();
        services.AddTransient<LocalMoviesController>();
        services.AddTransient<MovieScanController>();
        services.AddTransient<CleanupController>();
        services.AddTransient<ManualLookupController>();
        services.AddTransient<SettingsController>();
        services.AddTransient<WatchlistController>();
        services.AddTransient<LocalSeriesController>();
        services.AddTransient<NewSeriesController>();
        services.AddTransient<ApiUsageController>();
        services.AddTransient<SubtitleController>();
        services.AddTransient<LogController>();
        return services;
    }
}
