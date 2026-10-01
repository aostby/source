using Kolibri.Kino.Controllers.Library;
using Kolibri.Kino.Controllers.Lookup;
using Kolibri.Kino.Core.Abstractions;
using Kolibri.Kino.Core.Models;
using Kolibri.Kino.Data;
using Microsoft.Extensions.Logging.Abstractions;
using OMDbApiNet.Model;
using static Kolibri.Kino.Tests.LiteDbMovieRepositoryTests;

namespace Kolibri.Kino.Tests;

public sealed class ManualLookupControllerTests : IDisposable
{
    private readonly TempDatabase _temp = new();
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-lookup-").FullName;
    private readonly KinoDatabase _db;
    private readonly LiteDbMovieRepository _movies;
    private readonly LiteDbFileItemRepository _files;
    private readonly FakeOmdb _omdb = new();
    private readonly FakePlex _plex = new();
    private readonly ManualLookupController _lookup;

    public ManualLookupControllerTests()
    {
        _db = new KinoDatabase(_temp.Path);
        _movies = new LiteDbMovieRepository(_db);
        _files = new LiteDbFileItemRepository(_db);
        _lookup = new ManualLookupController(new MovieFileNameParser(), _movies, _plex, new SearchableOmdb(_omdb), _files,
            new FileSystemMediaScanner(), new NoPosters(), new MovieLinker(_files, _movies, _plex, _omdb),
            NullLogger<ManualLookupController>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _temp.Dispose();
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public void Suggests_title_and_year_from_the_file_or_the_imdb_tag()
    {
        Assert.Equal(("Spies In Disguise", 2019), _lookup.Suggest(@"X:\Spies In Disguise (2019) [1080p]\Spies.In.Disguise.2019.1080p.mp4"));
        Assert.Equal(("tt9808494", (int?)null), _lookup.Suggest(@"X:\Baby Einstein {imdb-tt9808494}\06BE_MACDONALD.avi"));
    }

    [Fact]
    public async Task Lists_library_then_Plex_then_OMDb_once_per_movie_in_the_given_year()
    {
        await _movies.UpsertAsync(Movie("tt0000001", "Frost Library", year: "2013"));
        _plex.Add(Movie("tt0000002", "Frost Plex", year: "2013")).Add(Movie("tt0000001", "Frost Library", year: "2013"));
        _omdb.Add(Movie("tt0000003", "Frost Online", year: "2013")).Add(Movie("tt0000004", "Frost Old", year: "1990"));

        var result = await _lookup.SearchAsync("frost", 2013);

        Assert.Equal(["tt0000001 library", "tt0000002 Plex", "tt0000003 OMDb"], result.Candidates.Select(c => $"{c.ImdbId} {c.Source}"));
    }

    [Fact]
    public async Task An_imdb_id_is_looked_up_directly()
    {
        _plex.Add(Movie("tt0133093", "The Matrix"));

        var candidate = Assert.Single((await _lookup.SearchAsync("TT0133093", null)).Candidates);

        Assert.Equal("The Matrix", candidate.Title);
        Assert.Equal(0, _omdb.Calls);
    }

    [Fact]
    public async Task Link_asks_before_taking_over_a_movie_linked_to_another_existing_file()
    {
        var existing = Touch("old.mkv");
        var file = Touch("new.mkv");
        await _files.UpsertAsync(new FileItem { ImdbId = "tt0133093", FullName = existing });
        _omdb.Add(Movie("tt0133093", "The Matrix"));
        var candidate = new LookupCandidate("tt0133093", "The Matrix", "1999", "movie", "OMDb", null);

        var first = await _lookup.LinkAsync(file, candidate, replaceExisting: false);
        Assert.Equal((false, existing), (first.Linked, first.ConflictPath));
        Assert.Equal(existing, (await _files.GetByImdbIdAsync("tt0133093"))?.FullName);

        var second = await _lookup.LinkAsync(file, candidate, replaceExisting: true);
        Assert.True(second.Linked);
        Assert.Equal(file, (await _files.GetByImdbIdAsync("tt0133093"))?.FullName);
        Assert.Equal(file, (await _movies.GetByImdbIdAsync("tt0133093"))?.TomatoUrl);
    }

    private string Touch(string name)
    {
        var path = Path.Combine(_folder, name);
        File.WriteAllText(path, "x");
        return path;
    }

    /// <summary>FakeOmdb plus a title search (OMDb's s= list).</summary>
    private sealed class SearchableOmdb(FakeOmdb inner) : IMovieInfoProvider
    {
        public Task<IReadOnlyList<SearchItem>> SearchAsync(string query, int page = 1, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<SearchItem>>(inner.ById.Values
                .Where(i => i.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(i => new SearchItem { ImdbId = i.ImdbId, Title = i.Title, Year = i.Year, Type = i.Type })
                .ToList());

        public Task<Item?> GetByImdbIdAsync(string imdbId, CancellationToken ct = default) => inner.GetByImdbIdAsync(imdbId, ct);
        public Task<Item?> GetMovieByTitleAsync(string title, int? year, CancellationToken ct = default) => inner.GetMovieByTitleAsync(title, year, ct);
        public Task<Item?> GetSeriesByTitleAsync(string title, int? year, CancellationToken ct = default) => inner.GetSeriesByTitleAsync(title, year, ct);
    }

    private sealed class NoPosters : IPosterProvider
    {
        public Task<byte[]?> GetPosterAsync(Item item, CancellationToken ct = default) => Task.FromResult<byte[]?>(null);
    }
}
