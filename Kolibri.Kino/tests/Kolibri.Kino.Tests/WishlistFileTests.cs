using System.Net;
using Kolibri.Kino.Controllers.Wishlist;
using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Tests;

public sealed class WishlistFileTests
{
    [Fact]
    public void Merging_keeps_each_title_once_the_first_list_first()
    {
        static WishlistItem W(string title, string year, string id = "") => new() { Title = title, Year = year, ImdbId = id };
        var uranus = new[] { W("Grindhouse", "2007", "tt0462322"), W("Heat", "1995"), W("Bosch S01E02", "2014", "tt3502248_S01E02") };
        var local = new[]
        {
            W("Grindhouse (same id)", "2007", "TT0462322"),
            W("heat", "1995", "tt0113277"),               // Uranus has it without an id
            W("Bosch S01E02", "2014", "tt3502248_S01E02"),
            W("Bosch S01E03", "2014", "tt3502248_S01E03"),
            W("Heat", "1986", "tt0093164"),               // another Heat
            W("Dune: Part Three", "2026", "tt31378509"),
        };

        var (items, added) = WishlistFile.Merge(uranus, local);

        Assert.Equal(3, added);
        Assert.Equal(["tt0462322", "", "tt3502248_S01E02", "tt3502248_S01E03", "tt0093164", "tt31378509"], items.Select(i => i.ImdbId));
    }

    private static IEnumerable<(string Title, string? Year, string ImdbId, string? Type)> Rows(WishlistFileRead read) =>
        read.Items.Select(i => (i.Title, i.Year, i.ImdbId, i.Type));

    [Fact]
    public void Kinos_own_wishlist_CSV_is_read_with_ids_and_types()
    {
        var csv = "﻿Title;Year;Type;IMDb rating;Genre;IMDb id;In SilverScreen;File;Added\r\n" +
                  "Grindhouse;2007;movie;7.5;Action, Horror;tt0462322;false;;2026-11-01\r\n" +
                  "The Bear S03E04: Napkins;2024;episode;;Comedy;tt14452776_S03E04;false;;2026-11-02\r\n" +
                  "No year;;movie;;;tt0000001;false;;2026-11-02\r\n";

        var read = WishlistFile.Parse(csv);

        Assert.Equal("CSV", read.Format);
        Assert.Equal([("Grindhouse", "2007", "tt0462322", "movie"), ("The Bear S03E04: Napkins", "2024", "tt14452776_S03E04", "episode")], Rows(read));
        Assert.Equal(1, read.Skipped);
        Assert.Equal(("Action, Horror", "7.5"), (read.Items[0].Genre, read.Items[0].ImdbRating));
    }

    [Fact]
    public void Other_programs_CSV_with_commas_and_other_names()
    {
        var csv = "Name,Released,IMDb link,Kind\n" +
                  "\"Mission: Impossible, Fallout\",2018-07-27,https://www.imdb.com/title/tt4912910/,Film\n" +
                  "Breaking Bad,2008–2013,,TV Series\n";

        Assert.Equal([("Mission: Impossible, Fallout", "2018", "tt4912910", "movie"), ("Breaking Bad", "2008", "", "series")],
            Rows(WishlistFile.Parse(csv)));
    }

    [Fact]
    public void Norwegian_names_work_too() =>
        Assert.Equal([("Kon-Tiki", "2012", "", null)], Rows(WishlistFile.Parse("Tittel;År\nKon-Tiki;2012\n")));

    [Fact]
    public void JSON_as_a_list_an_object_holding_one_or_Trakts_nested_form()
    {
        Assert.Equal([("Grindhouse", "2007", "tt0462322", null)],
            Rows(WishlistFile.Parse("""[{"title":"Grindhouse","year":2007,"imdb_id":"462322"}]""")));
        Assert.Equal([("Heat", "1995", "tt0113277", "movie")],
            Rows(WishlistFile.Parse("""{"exported":"2026-11-03","items":[{"Title":"Heat","Year":"1995","imdbID":"tt0113277","Type":"movie"}]}""")));
        Assert.Equal([("Arrival", "2016", "tt2543164", "movie")],
            Rows(WishlistFile.Parse("""[{"type":"movie","movie":{"title":"Arrival","year":2016,"ids":{"trakt":1,"imdb":"tt2543164"}}}]""")));
    }

    [Fact]
    public void XML_with_elements_or_attributes()
    {
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <wishlist>
              <movie><title>Grindhouse</title><year>2007</year><imdbId>tt0462322</imdbId></movie>
              <movie title="Heat" year="1995" />
              <movie><title>No year</title></movie>
            </wishlist>
            """;

        var read = WishlistFile.Parse(xml);

        Assert.Equal("XML", read.Format);
        Assert.Equal([("Grindhouse", "2007", "tt0462322", null), ("Heat", "1995", "", null)], Rows(read));
        Assert.Equal(1, read.Skipped);
    }

    [Theory]
    [InlineData("Film;Laget\nHeat;1995\n", "clearly the year")]
    [InlineData("Id;Year\ntt0113277;1995\n", "clearly the title")]
    [InlineData("""[{"what":"Heat","when":1995}]""", "clearly the title")]
    [InlineData("""[{"title":"Heat",""", "isn't valid JSON")]
    [InlineData("<wishlist><movie>", "isn't valid XML")]
    [InlineData("  ", "empty")]
    public void A_file_without_a_clear_title_and_year_is_refused(string text, string expected) =>
        Assert.Contains(expected, Assert.Throws<FormatException>(() => WishlistFile.Parse(text)).Message);

    [Fact]
    public async Task The_wishlist_on_Uranus_is_fetched_with_the_token()
    {
        string? url = null, key = null;
        var handler = new Handler(request =>
        {
            url = request.RequestUri!.AbsoluteUri;
            key = request.Headers.GetValues("X-Api-Key").Single();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Title;Year;IMDb id\nGrindhouse;2007;tt0462322\n") };
        });
        var web = new WebWishlist(new InMemorySettingsStore(new UserSettings { UranusUrl = "https://example.test/KolibriKino/", UranusApiKey = "secret" }), new HttpClient(handler));

        var read = await web.GetAsync();

        Assert.Equal("https://example.test/KolibriKino/get.php?type=Wishlist", url);
        Assert.Equal("secret", key);
        Assert.Equal("tt0462322", read.Items.Single().ImdbId);
    }

    [Fact]
    public async Task No_wishlist_on_Uranus_is_said_in_words()
    {
        var web = new WebWishlist(new InMemorySettingsStore(new UserSettings { UranusUrl = "https://example.test/", UranusApiKey = "secret" }),
            new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.NotFound))));
        Assert.Contains("no wishlist yet", (await Assert.ThrowsAsync<InvalidOperationException>(() => web.GetAsync())).Message);
        Assert.False(new WebWishlist(new InMemorySettingsStore(), new HttpClient()).IsConfigured);
    }

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => Task.FromResult(respond(request));
    }
}
