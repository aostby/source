using System.Xml.Linq;
using Kolibri.Kino.Controllers.Series;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public sealed class RecentSeriesTests
{
    private static long Unix(DateTime local) => new DateTimeOffset(local).ToUnixTimeSeconds();

    [Fact]
    public void Plex_episodes_are_grouped_by_show_with_its_imdb_id_newest_first()
    {
        var now = DateTime.Now;
        // Shaped like HTPC's answers on 2026-10-09 (/all?type=2&includeGuids=1 and /all?type=4&sort=addedAt:desc).
        var shows = XDocument.Parse("""
            <MediaContainer>
              <Directory ratingKey="161449" title="MobLand"><Guid id="imdb://tt31510819"/><Guid id="tmdb://247718"/></Directory>
              <Directory ratingKey="160292" title="Slow Horses"><Guid id="imdb://tt5875444"/></Directory>
              <Directory ratingKey="1" title="No Guid Show"/>
            </MediaContainer>
            """);
        var episodes = XDocument.Parse($"""
            <MediaContainer>
              <Video grandparentRatingKey="160292" grandparentTitle="Slow Horses" parentIndex="6" index="3" title="Resurrection" addedAt="{Unix(now.AddHours(-1))}"/>
              <Video grandparentRatingKey="161449" grandparentTitle="MobLand" parentIndex="2" index="3" title="Bonzo Goes to Bitburg" addedAt="{Unix(now.AddDays(-2))}"/>
              <Video grandparentRatingKey="161449" grandparentTitle="MobLand" parentIndex="2" index="1" title="I Wanna Be Your Dog" addedAt="{Unix(now.AddDays(-2))}"/>
              <Video grandparentRatingKey="1" grandparentTitle="No Guid Show" parentIndex="1" index="1" title="Pilot" addedAt="{Unix(now.AddDays(-3))}"/>
              <Video grandparentRatingKey="160292" grandparentTitle="Slow Horses" parentIndex="6" index="1" title="Old" addedAt="{Unix(now.AddDays(-40))}"/>
            </MediaContainer>
            """);

        var updates = PlexLibrary.RecentFrom(shows, [episodes], now.AddDays(-30));

        Assert.Equal(["Slow Horses", "MobLand", "No Guid Show"], updates.Select(u => u.Title));
        Assert.Equal(("tt5875444", "S06E03"), (updates[0].ImdbId, updates[0].Describe())); // the 40-day-old one is left out
        Assert.Equal(("tt31510819", "S02E01, S02E03"), (updates[1].ImdbId, updates[1].Describe()));
        Assert.Null(updates[2].ImdbId);
    }

    [Fact]
    public async Task Updates_are_matched_to_the_library_by_imdb_id_then_title()
    {
        var episode = new RecentEpisode(2, 1, "x", DateTime.Now);
        var server = new FakeServer(
            new RecentSeriesUpdate("tt31510819", "MobLand", [episode]),
            new RecentSeriesUpdate(null, "Slow Horses", [episode]),
            new RecentSeriesUpdate("tt0000001", "Not Here", [episode]));
        LocalSeries Series(string id, string title) => new(new Item { ImdbId = id, Title = title }, null, false);
        var library = new[] { Series("tt31510819", "MobLand"), Series("tt5875444", "Slow Horses"), Series("tt9", "Other") };

        var report = await new RecentSeriesController(server).GetAsync(library, days: 14);

        Assert.Equal(["tt31510819", "tt5875444"], report.InLibrary.Keys.Order());
        Assert.Equal("Not Here", Assert.Single(report.NotInLibrary).Title);
        Assert.Equal(DateTime.Now.AddDays(-14), server.Since, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Many_new_episodes_are_described_briefly() =>
        Assert.Equal("S01E01–S01E06 (6)",
            new RecentSeriesUpdate(null, "x", Enumerable.Range(1, 6).Select(i => new RecentEpisode(1, i, "", DateTime.Now)).ToList()).Describe());

    private sealed class FakeServer(params RecentSeriesUpdate[] updates) : IMediaServerSeries
    {
        public DateTime Since { get; private set; }
        public bool IsConfigured => true;

        public Task<IReadOnlyList<RecentSeriesUpdate>> GetRecentlyUpdatedSeriesAsync(DateTime since, CancellationToken ct = default)
        {
            Since = since;
            return Task.FromResult<IReadOnlyList<RecentSeriesUpdate>>(updates);
        }
    }
}
