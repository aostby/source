using System.Diagnostics.Metrics;
using Kolibri.Kino.KMDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public class KmdbMonitoringTests
{
    [Theory]
    [InlineData("/ping", "GET", true)]
    [InlineData("/ping", "HEAD", true)]
    [InlineData("/health", "GET", true)]
    [InlineData("/metrics", "GET", true)]
    [InlineData("/pingx", "GET", false)]
    public void Monitoring_needs_no_key(string path, string method, bool open) =>
        Assert.Equal(open, ApiKeyMiddleware.IsOpen(new Microsoft.AspNetCore.Http.PathString(path), method));

    [Theory]
    [InlineData(HealthStatus.Healthy, 1)]
    [InlineData(HealthStatus.Degraded, 0.5)]
    [InlineData(HealthStatus.Unhealthy, 0)]
    public void Health_becomes_a_number_for_Grafana(HealthStatus status, double score) => Assert.Equal(score, KmdbMetrics.Score(status));

    [Fact]
    public void Requests_are_counted_by_kind()
    {
        Assert.Equal("search", OmdbService.Kind(new OmdbRequest { S = "matrix" }));
        Assert.Equal("episode", OmdbService.Kind(new OmdbRequest { I = "tt1", Season = 1, Episode = 2 }));
        Assert.Equal("season", OmdbService.Kind(new OmdbRequest { I = "tt1", Season = 1 }));
        Assert.Equal("id", OmdbService.Kind(new OmdbRequest { I = "tt1" }));
        Assert.Equal("title", OmdbService.Kind(new OmdbRequest { T = "The Matrix" }));
    }

    [Fact]
    public async Task OMDb_lookups_and_health_show_up_as_metrics()
    {
        using var services = new ServiceCollection().AddMetrics().BuildServiceProvider();
        var meters = services.GetRequiredService<IMeterFactory>();
        var state = new HealthState();
        using var metrics = new KmdbMetrics(meters, state);
        using var lookups = new MetricCollector<long>(meters, KmdbMetrics.MeterName, "kmdb.omdb.requests");
        using var health = new MetricCollector<double>(meters, KmdbMetrics.MeterName, "kmdb.health.status");

        var omdb = new OmdbService(new NoTitles(), metrics: metrics);
        await omdb.GetAsync(new OmdbRequest { I = "tt0000001" });
        var lookup = Assert.Single(lookups.GetMeasurementSnapshot());
        Assert.Equal(("id", "not_found"), ((string)lookup.Tags["kind"]!, (string)lookup.Tags["result"]!));

        state.Update(new HealthReport(new Dictionary<string, HealthReportEntry>
        {
            ["imdb"] = new(HealthStatus.Healthy, null, TimeSpan.FromMilliseconds(5), null, null),
            ["silverscreen"] = new(HealthStatus.Degraded, null, TimeSpan.FromMilliseconds(5), null, null),
        }, TimeSpan.FromMilliseconds(10)));
        health.RecordObservableInstruments();
        var values = health.GetMeasurementSnapshot().ToDictionary(m => (string)m.Tags["check"]!, m => m.Value);
        Assert.Equal(1, values["imdb"]);
        Assert.Equal(0.5, values["silverscreen"]);
    }

    private sealed class NoTitles : IImdbRepository
    {
        public Task<TitleRow?> GetTitleAsync(string imdbId, CancellationToken ct = default) => Task.FromResult<TitleRow?>(null);
        public Task<TitleRow?> FindByTitleAsync(string title, IReadOnlyCollection<string> titleTypes, int? year, CancellationToken ct = default) => Task.FromResult<TitleRow?>(null);
        public Task<SearchPage> SearchAsync(string query, IReadOnlyCollection<string> titleTypes, int? year, int page, int pageSize, CancellationToken ct = default) => Task.FromResult(new SearchPage([], 0, false));
        public Task<IReadOnlyDictionary<string, string>> NamesAsync(IReadOnlyCollection<string> nameIds, CancellationToken ct = default) => Task.FromResult<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>());
        public Task<IReadOnlyList<string>> ActorsAsync(string imdbId, int max, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<string>>([]);
        public Task<EpisodeLink?> GetEpisodeLinkAsync(string episodeId, CancellationToken ct = default) => Task.FromResult<EpisodeLink?>(null);
        public Task<int?> TotalSeasonsAsync(string seriesId, CancellationToken ct = default) => Task.FromResult<int?>(null);
        public Task<IReadOnlyList<SeasonEpisodeRow>> SeasonAsync(string seriesId, int season, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<SeasonEpisodeRow>>([]);
        public Task<string?> EpisodeIdAsync(string seriesId, int season, int episode, CancellationToken ct = default) => Task.FromResult<string?>(null);
        public Task<bool> PingAsync(CancellationToken ct = default) => Task.FromResult(true);
    }
}
