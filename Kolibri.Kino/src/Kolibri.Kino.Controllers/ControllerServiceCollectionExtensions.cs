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
        services.AddTransient<WebWishlist>();
        // Prowlarr's answers for a few minutes, in "Cache" next to the .exe (not in the database).
        services.AddSingleton(sp => new Prowlarr.ProwlarrCache(
            Path.Combine(AppContext.BaseDirectory, "Cache"),
            TimeSpan.FromMinutes(Math.Max(0, sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Kolibri.Kino.Core.KinoOptions>>().Value.Prowlarr.CacheMinutes)),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Prowlarr.ProwlarrCache>>()));
        services.AddTransient<Prowlarr.ProwlarrController>();
        services.AddTransient<Ratings.RatingController>();
        services.AddTransient<Ratings.WebRatings>();
        services.AddTransient<Trending.TrendingController>();
        services.AddTransient<Discover.DiscoverController>();
        services.AddTransient<Suggestions.SuggestionsController>();
        services.AddTransient<LocalSeriesController>();
        services.AddTransient<NewSeriesController>();
        services.AddTransient<RecentSeriesController>();
        services.AddTransient<ApiUsageController>();
        services.AddTransient<SubtitleController>();
        services.AddTransient<LogController>();
        return services;
    }
}
