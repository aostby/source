using Kolibri.Kino.Controllers.Cleanup;
using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.LocalMovies;
using Kolibri.Kino.Controllers.Logging;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Controllers.Playback;
using Kolibri.Kino.Controllers.Scanning;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Controllers.Settings;
using Kolibri.Kino.Controllers.Subtitles;
using Kolibri.Kino.Controllers.Usage;
using Kolibri.Kino.Controllers.Watchlists;
using Kolibri.Kino.Controllers.Wishlist;
using Microsoft.Extensions.DependencyInjection;

namespace Kolibri.Kino.Controllers;

public static class ControllerServiceCollectionExtensions
{
    public static IServiceCollection AddKinoControllers(this IServiceCollection services)
    {
        services.AddTransient<MovieLinker>();
        services.AddTransient<LibraryFiles>();
        services.AddTransient<MovieController>();
        services.AddTransient<RecentlyAddedController>();
        services.AddTransient<PlaybackController>();
        services.AddTransient<LocalMoviesController>();
        services.AddTransient<MovieScanController>();
        services.AddTransient<CleanupController>();
        services.AddTransient<ManualLookupController>();
        services.AddTransient<SettingsController>();
        services.AddTransient<WatchlistController>();
        services.AddTransient<WishlistController>();
        services.AddTransient<Ratings.RatingController>();
        services.AddTransient<Trending.TrendingController>();
        services.AddTransient<LocalSeriesController>();
        services.AddTransient<NewSeriesController>();
        services.AddTransient<RecentSeriesController>();
        services.AddTransient<ApiUsageController>();
        services.AddTransient<SubtitleController>();
        services.AddTransient<LogController>();
        return services;
    }
}
