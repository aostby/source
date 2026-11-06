using Kolibri.Kino.KMDb;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMDbApiNet.Model;

namespace Kolibri.Kino.Tests;

public class KmdbFormatTests
{
    [Theory]
    [InlineData("movie", 1999, null, "1999")]
    [InlineData("tvSeries", 2008, 2013, "2008–2013")]
    [InlineData("tvMiniSeries", 2019, null, "2019–")]
    [InlineData("tvSeries", 2020, 2020, "2020")]
    [InlineData("tvEpisode", 2013, null, "2013")]
    [InlineData("movie", null, null, "N/A")]
    public void Years_are_written_as_OMDb_writes_them(string type, int? start, int? end, string expected) =>
        Assert.Equal(expected, OmdbFormat.Year(type, start, end));

    [Theory]
    [InlineData("movie", "movie")]
    [InlineData("tvMovie", "movie")]
    [InlineData("short", "movie")]
    [InlineData("tvSeries", "series")]
    [InlineData("tvMiniSeries", "series")]
    [InlineData("tvEpisode", "episode")]
    [InlineData("videoGame", "game")]
    public void IMDb_title_types_become_OMDb_types(string titleType, string expected) =>
        Assert.Equal(expected, OmdbFormat.OmdbType(titleType));

    [Fact]
    public void Numbers_and_lists_use_OMDb_formats()
    {
        Assert.Equal("2,291,970", OmdbFormat.Votes(2291970));
        Assert.Equal("8.7", OmdbFormat.Rating(8.7m));
        Assert.Equal("7.0", OmdbFormat.Rating(7m));
        Assert.Equal("136 min", OmdbFormat.Runtime(136));
        Assert.Equal("N/A", OmdbFormat.Runtime(null));
        Assert.Equal("Action, Sci-Fi", OmdbFormat.Genre("Action,Sci-Fi"));
        Assert.Equal("N/A", OmdbFormat.Genre(null));
        var rating = Assert.Single(OmdbFormat.Ratings(8.7m));
        Assert.Equal(("Internet Movie Database", "8.7/10"), (rating.Source, rating.Value));
        Assert.Empty(OmdbFormat.Ratings(null));
    }

    [Fact]
    public void Names_keep_the_crew_order_and_skip_unknown_ids()
    {
        var names = new Dictionary<string, string> { ["nm2"] = "Lilly", ["nm1"] = "Lana" };
        Assert.Equal("Lana, Lilly", OmdbFormat.Names(OmdbFormat.Split("nm1,nm9,nm2,nm1"), names, 5));
        Assert.Equal("Lana", OmdbFormat.Names(OmdbFormat.Split("nm1,nm2"), names, 1));
        Assert.Equal("N/A", OmdbFormat.Names(OmdbFormat.Split(null), names, 5));
    }

    [Fact]
    public void Search_patterns_also_match_after_an_article_and_escape_wildcards()
    {
        Assert.Equal(["matrix%", "The matrix%", "A matrix%", "An matrix%"], MariaDbImdbRepository.SearchPatterns("matrix"));
        Assert.Equal(["The Matrix%"], MariaDbImdbRepository.SearchPatterns("The Matrix"));
        Assert.Equal(@"100\% \_x\\", MariaDbImdbRepository.EscapeLike(@"100% _x\"));
    }

    [Theory]
    [InlineData("tt0133093", true)]
    [InlineData("tt10838180", true)]
    [InlineData("tt123", false)]
    [InlineData("nm0000206", false)]
    [InlineData("tt0133093'; DROP", false)]
    public void Only_IMDb_title_ids_are_looked_up(string id, bool valid) => Assert.Equal(valid, OmdbService.IsImdbId(id));
}

public class KmdbApiKeyTests
{
    [Fact]
    public void The_key_is_checked_with_OMDb_messages()
    {
        Assert.Null(ApiKeyMiddleware.Check("secret", "secret"));
        Assert.Equal("Invalid API key!", ApiKeyMiddleware.Check("wrong", "secret"));
        Assert.Equal("No API key provided.", ApiKeyMiddleware.Check(null, "secret"));
        Assert.Equal("No API key provided.", ApiKeyMiddleware.Check("", "secret"));
        // No key configured: nothing gets in.
        Assert.Equal("Invalid API key!", ApiKeyMiddleware.Check("anything", null));
    }

    [Theory]
    [InlineData("/swagger/index.html", true)]
    [InlineData("/swagger/v1/swagger.json", true)]
    [InlineData("/health", true)]
    [InlineData("/", false)]
    [InlineData("/title/tt0133093", false)]
    [InlineData("/swaggerx", false)]
    public void Only_Swagger_and_health_are_open(string path, bool open) =>
        Assert.Equal(open, ApiKeyMiddleware.IsOpen(new Microsoft.AspNetCore.Http.PathString(path)));
}

public class KmdbOmdbServiceTests
{
    private static readonly TitleRow Matrix = new("tt0133093", "movie", "The Matrix", "The Matrix", false, 1999, null, 136,
        "Action,Sci-Fi", 8.7m, 2291970, "nm0905154,nm0905152", "nm0905152,nm0905154");

    private static readonly TitleRow BreakingBad = new("tt0903747", "tvSeries", "Breaking Bad", null, false, 2008, 2013, 48,
        "Crime,Drama,Thriller", 9.5m, 2701405, "nm0533713", "nm0319213");

    private static readonly TitleRow Ozymandias = new("tt2301451", "tvEpisode", "Ozymandias", null, false, 2013, null, 47,
        "Crime,Drama,Thriller", 9.5m, 510909, "nm0426059", "nm0319213");

    private readonly FakeImdb _db = new();
    private OmdbService Service => new(_db);

    public KmdbOmdbServiceTests()
    {
        _db.Titles.AddRange([Matrix, BreakingBad, Ozymandias]);
        _db.Names["nm0905154"] = "Lana Wachowski";
        _db.Names["nm0905152"] = "Lilly Wachowski";
        _db.Names["nm0319213"] = "Vince Gilligan";
        _db.Names["nm0426059"] = "Rian Johnson";
        _db.Actors["tt0133093"] = ["Keanu Reeves", "Laurence Fishburne", "Carrie-Anne Moss"];
        _db.Episodes.Add((Ozymandias.Id, new EpisodeLink(BreakingBad.Id, 5, 14)));
    }

    [Fact]
    public async Task A_movie_by_id_is_an_OMDb_item()
    {
        var item = Assert.IsType<Item>(await Service.GetAsync(new OmdbRequest { I = "tt0133093" }));
        Assert.Equal("The Matrix", item.Title);
        Assert.Equal("1999", item.Year);
        Assert.Equal("Lana Wachowski, Lilly Wachowski", item.Director);
        Assert.Equal("Lilly Wachowski, Lana Wachowski", item.Writer);
        Assert.Equal("Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss", item.Actors);
        Assert.Equal("8.7", item.ImdbRating);
        Assert.Equal("2,291,970", item.ImdbVotes);
        Assert.Equal("movie", item.Type);
        Assert.Equal("N/A", item.Plot);
        Assert.Null(item.TotalSeasons);
        Assert.Equal("True", item.Response);
    }

    [Fact]
    public async Task A_series_has_total_seasons_and_no_director()
    {
        _db.Seasons[BreakingBad.Id] = 5;
        var item = Assert.IsType<Item>(await Service.GetAsync(new OmdbRequest { I = "tt0903747" }));
        Assert.Equal("series", item.Type);
        Assert.Equal("2008–2013", item.Year);
        Assert.Equal("5", item.TotalSeasons);
        Assert.Equal("N/A", item.Director);
        Assert.Equal("Vince Gilligan", item.Writer);
        Assert.Equal("N/A", item.Actors);
    }

    [Fact]
    public async Task An_episode_id_gives_an_episode_with_its_series()
    {
        var episode = Assert.IsType<Episode>(await Service.GetAsync(new OmdbRequest { I = "tt2301451" }));
        Assert.Equal(("Ozymandias", "5", "14", "tt0903747", "episode"),
            (episode.Title, episode.SeasonNumber, episode.EpisodeNumber, episode.SeriesId, episode.Type));
        Assert.Equal("Rian Johnson", episode.Director);
    }

    [Fact]
    public async Task Season_and_episode_of_a_series()
    {
        _db.Seasons[BreakingBad.Id] = 5;
        _db.SeasonRows[(BreakingBad.Id, 5)] = [new SeasonEpisodeRow(Ozymandias.Id, 14, "Ozymandias", 2013, 9.5m)];

        var season = Assert.IsType<Season>(await Service.GetAsync(new OmdbRequest { I = "tt0903747", Season = 5 }));
        Assert.Equal(("Breaking Bad", "5", "5"), (season.Title, season.SeasonNumber, season.TotalSeasons));
        var row = Assert.Single(season.Episodes);
        Assert.Equal(("Ozymandias", "14", "9.5", "tt2301451"), (row.Title, row.Episode, row.ImdbRating, row.ImdbId));

        var episode = Assert.IsType<Episode>(await Service.GetAsync(new OmdbRequest { T = "Breaking Bad", Season = 5, Episode = 14 }));
        Assert.Equal("tt2301451", episode.ImdbId);
        Assert.Equal(("5", "14"), (episode.SeasonNumber, episode.EpisodeNumber));

        Assert.Equal("Series or season not found!", Error(await Service.GetAsync(new OmdbRequest { I = "tt0903747", Season = 9 })));
        Assert.Equal("Series or episode not found!", Error(await Service.GetAsync(new OmdbRequest { I = "tt0903747", Season = 5, Episode = 99 })));
        Assert.Equal("Series or season not found!", Error(await Service.GetAsync(new OmdbRequest { I = "tt0133093", Season = 1 })));
    }

    [Fact]
    public async Task Search_gives_an_OMDb_search_list()
    {
        _db.Search = (q, types, year, page, size) => new SearchPage([Matrix], 21, false);
        var list = Assert.IsType<SearchList>(await Service.GetAsync(new OmdbRequest { S = "matrix", Page = 3 }));
        var hit = Assert.Single(list.SearchResults);
        Assert.Equal(("The Matrix", "1999", "tt0133093", "movie", "N/A"), (hit.Title, hit.Year, hit.ImdbId, hit.Type, hit.Poster));
        Assert.Equal("21", list.TotalResults);
        Assert.Equal("True", list.Response);
        Assert.Equal(("matrix", 3, OmdbService.PageSize), (_db.LastSearch!.Value.Query, _db.LastSearch.Value.Page, _db.LastSearch.Value.Size));
        Assert.DoesNotContain("tvEpisode", _db.LastSearch.Value.Types);

        await Service.GetAsync(new OmdbRequest { S = "matrix", Type = "series" });
        Assert.Equal(["tvSeries", "tvMiniSeries"], _db.LastSearch.Value.Types);
    }

    [Fact]
    public async Task Errors_are_OMDb_errors()
    {
        Assert.Equal("Incorrect IMDb ID.", Error(await Service.GetAsync(new OmdbRequest { I = "nope" })));
        Assert.Equal("Error getting data.", Error(await Service.GetAsync(new OmdbRequest { I = "tt7777777" })));
        Assert.Equal("Movie not found!", Error(await Service.GetAsync(new OmdbRequest { T = "No such film" })));
        Assert.Equal("Incorrect IMDb ID.", Error(await Service.GetAsync(new OmdbRequest())));
        Assert.Equal("Too many results.", Error(await Service.GetAsync(new OmdbRequest { S = "x" })));
        Assert.StartsWith("Invalid type!", Error(await Service.GetAsync(new OmdbRequest { S = "matrix", Type = "film" })));
        _db.Search = (_, _, _, _, _) => new SearchPage([], 0, TooMany: true);
        Assert.Equal("Too many results.", Error(await Service.GetAsync(new OmdbRequest { S = "the" })));
        _db.Search = (_, _, _, _, _) => new SearchPage([], 0, TooMany: false);
        Assert.Equal("Movie not found!", Error(await Service.GetAsync(new OmdbRequest { S = "zzzz" })));
    }

    [Fact]
    public async Task The_JSON_uses_OMDb_names_so_OMDb_clients_read_it()
    {
        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        var json = JObject.Parse(JsonConvert.SerializeObject(await Service.GetAsync(new OmdbRequest { I = "tt0133093" }), settings));
        Assert.Equal("tt0133093", (string?)json["imdbID"]);
        Assert.Equal("8.7", (string?)json["imdbRating"]);
        Assert.Equal("True", (string?)json["Response"]);
        Assert.Null(json["totalSeasons"]);
        Assert.Null(json["tomatoMeter"]);
        Assert.Equal("The Matrix", JsonConvert.DeserializeObject<Item>(json.ToString())!.Title);

        var error = JObject.Parse(JsonConvert.SerializeObject(OmdbFormat.Error("Movie not found!")));
        Assert.Equal("""{"Response":"False","Error":"Movie not found!"}""", error.ToString(Formatting.None));
    }

    private static string Error(object response) => Assert.IsType<ErrorResponse>(response).Error;

    private sealed class FakeImdb : IImdbRepository
    {
        public List<TitleRow> Titles { get; } = [];
        public Dictionary<string, string> Names { get; } = [];
        public Dictionary<string, IReadOnlyList<string>> Actors { get; } = [];
        public List<(string Id, EpisodeLink Link)> Episodes { get; } = [];
        public Dictionary<string, int> Seasons { get; } = [];
        public Dictionary<(string, int), IReadOnlyList<SeasonEpisodeRow>> SeasonRows { get; } = [];
        public Func<string, IReadOnlyCollection<string>, int?, int, int, SearchPage> Search { get; set; } = (_, _, _, _, _) => new SearchPage([], 0, false);
        public (string Query, IReadOnlyCollection<string> Types, int Page, int Size)? LastSearch { get; private set; }

        public Task<TitleRow?> GetTitleAsync(string imdbId, CancellationToken ct = default) =>
            Task.FromResult(Titles.FirstOrDefault(t => t.Id == imdbId));

        public Task<TitleRow?> FindByTitleAsync(string title, IReadOnlyCollection<string> titleTypes, int? year, CancellationToken ct = default) =>
            Task.FromResult(Titles.FirstOrDefault(t => t.PrimaryTitle == title && titleTypes.Contains(t.TitleType) && (year is null || t.StartYear == year)));

        public Task<SearchPage> SearchAsync(string query, IReadOnlyCollection<string> titleTypes, int? year, int page, int pageSize, CancellationToken ct = default)
        {
            LastSearch = (query, titleTypes, page, pageSize);
            return Task.FromResult(Search(query, titleTypes, year, page, pageSize));
        }

        public Task<IReadOnlyDictionary<string, string>> NamesAsync(IReadOnlyCollection<string> nameIds, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyDictionary<string, string>>(Names.Where(n => nameIds.Contains(n.Key)).ToDictionary());

        public Task<IReadOnlyList<string>> ActorsAsync(string imdbId, int max, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<string>>(Actors.GetValueOrDefault(imdbId, []).Take(max).ToList());

        public Task<EpisodeLink?> GetEpisodeLinkAsync(string episodeId, CancellationToken ct = default) =>
            Task.FromResult(Episodes.Where(e => e.Id == episodeId).Select(e => e.Link).FirstOrDefault());

        public Task<int?> TotalSeasonsAsync(string seriesId, CancellationToken ct = default) =>
            Task.FromResult(Seasons.TryGetValue(seriesId, out var n) ? n : (int?)null);

        public Task<IReadOnlyList<SeasonEpisodeRow>> SeasonAsync(string seriesId, int season, CancellationToken ct = default) =>
            Task.FromResult(SeasonRows.GetValueOrDefault((seriesId, season), []));

        public Task<string?> EpisodeIdAsync(string seriesId, int season, int episode, CancellationToken ct = default) =>
            Task.FromResult(Episodes.Where(e => e.Link.SeriesId == seriesId && e.Link.Season == season && e.Link.Episode == episode)
                .Select(e => e.Id).FirstOrDefault());

        public Task<bool> PingAsync(CancellationToken ct = default) => Task.FromResult(true);
    }
}
