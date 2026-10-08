using System.Net;
using Kolibri.Kino.Controllers.Usage;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

public sealed class ApiUsageTests : IDisposable
{
    private static readonly DateOnly Today = new(2026, 10, 9);
    private readonly TempDatabase _temp = new();
    private readonly KinoDatabase _db;
    private readonly LiteDbApiUsageRepository _repository;

    public ApiUsageTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _repository = new LiteDbApiUsageRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
    }

    [Theory]
    [InlineData("www.omdbapi.com", 443, ApiServices.Omdb)]
    [InlineData("img.omdbapi.com", 443, ApiServices.Omdb)]
    [InlineData("api.themoviedb.org", 443, ApiServices.Tmdb)]
    [InlineData("api.subdl.com", 443, ApiServices.SubDl)]
    [InlineData("HTPC", 32400, ApiServices.Plex)]
    [InlineData("htpc", 80, ApiServices.Plex)]
    [InlineData("192-168-1-5.abc.plex.direct", 32400, ApiServices.Plex)]
    [InlineData("image.tmdb.org", 443, null)]
    [InlineData("m.media-amazon.com", 443, null)]
    [InlineData("notomdbapi.com", 443, null)]
    public void Requests_are_counted_for_the_service_of_their_host(string host, int port, string? expected) =>
        Assert.Equal(expected, ApiServiceHosts.Classify(host, port, plexServer: "HTPC"));

    [Fact]
    public async Task Adding_sums_into_the_stored_day()
    {
        await _repository.AddAsync([new(Today, ApiServices.Omdb, new(3, 1)), new(Today, ApiServices.Plex, new(2, 0))]);
        await _repository.AddAsync([new(Today, ApiServices.Omdb, new(4, 0)), new(Today.AddDays(-1), ApiServices.Tmdb, new(5, 0))]);

        var day = await _repository.GetDayAsync(Today);
        Assert.Equal((new ApiCallCount(7, 1), new ApiCallCount(2, 0), ApiCallCount.Zero),
            (day!.For(ApiServices.Omdb), day.For(ApiServices.Plex), day.For(ApiServices.Tmdb)));
        Assert.Equal([Today.AddDays(-1), Today], (await _repository.GetDaysAsync(null, null)).Select(d => d.Date));
        Assert.Equal([Today], (await _repository.GetDaysAsync(Today, Today)).Select(d => d.Date));
    }

    [Fact]
    public async Task The_tracker_keeps_counts_until_flushed()
    {
        using var tracker = new ApiUsageTracker(_repository, new InMemorySettingsStore(), listen: false);
        tracker.Record(ApiServices.Omdb, failed: false, Today);
        tracker.Record(ApiServices.Omdb, failed: true, Today);
        Assert.Null(await _repository.GetDayAsync(Today));

        await tracker.FlushAsync();
        await tracker.FlushAsync(); // nothing new: no double counting

        Assert.Equal(new ApiCallCount(2, 1), (await _repository.GetDayAsync(Today))!.For(ApiServices.Omdb));
    }

    [Fact]
    public async Task Real_requests_to_the_Plex_server_are_counted()
    {
        using var server = new HttpListener();
        var port = Random.Shared.Next(49152, 65000);
        server.Prefixes.Add($"http://localhost:{port}/");
        server.Start();
        var answer = Task.Run(async () =>
        {
            var context = await server.GetContextAsync();
            context.Response.StatusCode = 401;
            context.Response.Close();
        });
        var settings = new InMemorySettingsStore(new UserSettings { XPlexServerName = "localhost" });
        var today = DateOnly.FromDateTime(DateTime.Now);
        using var tracker = new ApiUsageTracker(_repository, settings);

        using (var http = new HttpClient())
            using (await http.GetAsync($"http://localhost:{port}/identity")) { }
        await answer;
        await tracker.FlushAsync();

        Assert.Equal(new ApiCallCount(1, 1), (await _repository.GetDayAsync(today))!.For(ApiServices.Plex));
    }

    [Fact]
    public async Task The_30_day_report_has_every_day_newest_first_with_totals_and_today()
    {
        await _repository.AddAsync([
            new(Today, ApiServices.Omdb, new(10, 2)),
            new(Today.AddDays(-3), ApiServices.Tmdb, new(7, 0)),
            new(Today.AddDays(-40), ApiServices.Omdb, new(99, 0)), // outside
        ]);
        var controller = new ApiUsageController(_repository, new NoTracker());

        var report = await controller.GetReportAsync(UsagePeriod.Last30Days, Today, default);

        Assert.Equal(30, report.Rows.Count);
        Assert.Equal("2026-10-09 Fri", report.Rows[0].Period);
        Assert.Equal(7, report.Rows[3].TMDb);
        Assert.Equal((10, 7, 17, 2), (report.Total.OMDb, report.Total.TMDb, report.Total.Total, report.Total.Errors));
        Assert.Equal(10, report.Today.OMDb);
        Assert.Equal([2026], report.Years);
    }

    [Fact]
    public async Task The_year_report_has_one_row_per_month_up_to_now()
    {
        await _repository.AddAsync([
            new(new DateOnly(2026, 2, 1), ApiServices.Plex, new(3, 0)),
            new(new DateOnly(2026, 2, 28), ApiServices.Plex, new(4, 0)),
            new(new DateOnly(2025, 12, 31), ApiServices.Omdb, new(5, 0)),
        ]);
        var controller = new ApiUsageController(_repository, new NoTracker());

        var report = await controller.GetReportAsync(UsagePeriod.ForYear(2026), Today, default);

        Assert.Equal(["2026-10", "2026-09", "2026-08", "2026-07", "2026-06", "2026-05", "2026-04", "2026-03", "2026-02", "2026-01"],
            report.Rows.Select(r => r.Period));
        Assert.Equal(7, report.Rows.Single(r => r.Period == "2026-02").Plex);
        Assert.Equal([2026, 2025], report.Years);
    }

    [Fact]
    public async Task The_month_report_has_its_days_and_all_time_counts_every_month()
    {
        await _repository.AddAsync([new(new DateOnly(2026, 8, 15), ApiServices.SubDl, new(2, 0))]);
        var controller = new ApiUsageController(_repository, new NoTracker());

        var august = await controller.GetReportAsync(UsagePeriod.ForMonth(2026, 8), Today, default);
        var all = await controller.GetReportAsync(UsagePeriod.All, Today, default);

        Assert.Equal(31, august.Rows.Count);
        Assert.Equal(2, august.Total.SubDL);
        Assert.Equal(["2026-10", "2026-09", "2026-08"], all.Rows.Select(r => r.Period));
    }

    private sealed class NoTracker : Core.Abstractions.IApiUsageTracker
    {
        public void Record(string service, bool failed, DateOnly? date = null) { }
        public Task FlushAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
