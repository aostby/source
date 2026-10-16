using System.Net;
using System.Text;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

/// <summary>TMDb answers faked; checks how few requests a series costs.</summary>
public sealed class SeriesInfoProviderTests
{
    private readonly FakeTmdb _tmdb = new();
    private readonly SeriesInfoProvider _provider;

    public SeriesInfoProviderTests() =>
        _provider = new SeriesInfoProvider(new InMemorySettingsStore(new UserSettings { TMDBkey = "key" }), new HttpClient(_tmdb));

    [Fact]
    public async Task Finds_the_show_then_gets_it_with_all_seasons_in_one_request()
    {
        var result = await _provider.GetSeasonsAsync("tt0118276", totalSeasonsHint: 2);

        Assert.Equal(1399, result.TmdbId);
        Assert.Equal([1, 2, 0], result.Seasons.Select(s => s.SeasonNumber));
        Assert.Equal(["Pilot", "Two"], result.Seasons[0].Episodes.Select(e => e.Title));
        Assert.Equal(new DateTime(2011, 4, 17), result.Seasons[0].Episodes[0].AirDate);
        Assert.Equal("https://image.tmdb.org/t/p/w300/still.jpg", result.Seasons[0].Episodes[0].StillUrl);
        Assert.Equal(2, _tmdb.Requests.Count);
        Assert.StartsWith("/3/find/tt0118276", _tmdb.Requests[0]);
        Assert.Contains("append_to_response=season/0,season/1,season/2", _tmdb.Requests[1]);
    }

    [Fact]
    public async Task A_known_tmdb_id_skips_the_lookup_and_missing_seasons_come_in_one_more_request()
    {
        var result = await _provider.GetSeasonsAsync("tt0118276", totalSeasonsHint: 1, tmdbId: 1399);

        Assert.Equal([1, 2, 0], result.Seasons.Select(s => s.SeasonNumber));
        Assert.Equal(2, _tmdb.Requests.Count);
        Assert.Contains("append_to_response=season/0,season/1&", _tmdb.Requests[0]);
        Assert.Contains("append_to_response=season/2&", _tmdb.Requests[1]);
    }

    [Fact]
    public async Task From_a_season_asks_only_for_that_one_and_later()
    {
        var result = await _provider.GetSeasonsAsync("tt0118276", totalSeasonsHint: 2, tmdbId: 1399, fromSeason: 2);

        Assert.Equal([2], result.Seasons.Select(s => s.SeasonNumber));
        Assert.Single(_tmdb.Requests);
        Assert.Contains("append_to_response=season/2&", _tmdb.Requests[0]);
    }

    /// <summary>A show (TMDb 1399) with specials and two seasons; "season/N" parts only when asked for, as TMDb does.</summary>
    private sealed class FakeTmdb : HttpMessageHandler
    {
        public List<string> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var path = request.RequestUri!.PathAndQuery;
            Requests.Add(path);
            string json;
            if (path.StartsWith("/3/find/"))
            {
                json = """{"tv_results":[{"id":1399}]}""";
            }
            else
            {
                var asked = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query)["append_to_response"]?.Split(',') ?? [];
                var parts = new List<string>
                {
                    "\"seasons\":[{\"season_number\":0,\"episode_count\":1},{\"season_number\":1,\"episode_count\":2},{\"season_number\":2,\"episode_count\":1}]",
                };
                foreach (var part in asked)
                {
                    var n = int.Parse(part["season/".Length..]);
                    var episodes = n switch
                    {
                        1 => """[{"episode_number":1,"name":"Pilot","air_date":"2011-04-17","still_path":"/still.jpg","vote_average":8.1},{"episode_number":2,"name":"Two","air_date":"2011-04-24"}]""",
                        _ => """[{"episode_number":1,"name":"Only","air_date":""}]""",
                    };
                    parts.Add($"\"season/{n}\":{{\"name\":\"Season {n}\",\"episodes\":{episodes}}}");
                }
                json = "{" + string.Join(",", parts) + "}";
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
        }
    }
}
