using Microsoft.AspNetCore.Mvc;
using OMDbApiNet.Model;

namespace Kolibri.Kino.KMDb;

/// <summary>
/// OMDb's API on the IMDb database. GET / takes the same parameters as http://www.omdbapi.com/ and answers in the same
/// JSON, so an OMDb client only needs the address changed. The other routes are the same lookups with the values in
/// the path, handy on the Swagger page.
/// </summary>
[ApiController]
[Tags("OMDb")]
[Produces("application/json")]
public sealed class OmdbController(OmdbService omdb) : ControllerBase
{
    /// <summary>This service's address as the client called it, for links to posters.</summary>
    private string BaseUrl => $"{Request.Scheme}://{Request.Host}";

    /// <summary>OMDb-compatible lookup: by id (i), by title (t), search (s), season or episode.</summary>
    /// <remarks>
    /// Examples: <c>?i=tt0133093</c> · <c>?t=The Matrix&amp;y=1999</c> · <c>?s=matrix&amp;type=movie&amp;page=2</c> ·
    /// <c>?i=tt0903747&amp;Season=1</c> · <c>?i=tt0903747&amp;Season=1&amp;Episode=1</c>.
    /// The key goes in <c>apikey</c> (as at OMDb) or the X-Api-Key header. Search matches the start of a title, also after
    /// The/A/An, and alternative titles; without type= it leaves out single episodes.
    /// Not in IMDb's data sets, so always "N/A": Plot, Poster, Rated, Released, Language, Country, Awards, Metascore.
    /// </remarks>
    /// <param name="request">OMDb's parameters.</param>
    /// <param name="apikey">The API key (or send the X-Api-Key header).</param>
    /// <param name="plot">Accepted for OMDb clients; there is no plot in IMDb's data sets.</param>
    /// <param name="r">Accepted for OMDb clients; the answer is always JSON.</param>
    /// <param name="ct"></param>
    [HttpGet("/")]
    [ProducesResponseType<Item>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] OmdbRequest request, [FromQuery] string? apikey = null,
        [FromQuery] string? plot = null, [FromQuery] string? r = null, CancellationToken ct = default) =>
        Ok(await omdb.GetAsync(request, BaseUrl, ct));

    /// <summary>A title by IMDb id: a movie or series (Item) or an episode (Episode).</summary>
    [HttpGet("/title/{imdbId}")]
    [ProducesResponseType<Item>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Title(string imdbId, CancellationToken ct) =>
        Ok(await omdb.GetAsync(new OmdbRequest { I = imdbId }, BaseUrl, ct));

    /// <summary>The best match for a title (most votes), like OMDb's t=.</summary>
    /// <param name="t">The title, e.g. The Matrix (the Norwegian or another alternative title works too).</param>
    /// <param name="type">movie, series, episode or game.</param>
    /// <param name="y">Year (the first year for a series).</param>
    /// <param name="ct"></param>
    [HttpGet("/find")]
    [ProducesResponseType<Item>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Find([FromQuery] string t, [FromQuery] string? type = null, [FromQuery] int? y = null, CancellationToken ct = default) =>
        Ok(await omdb.GetAsync(new OmdbRequest { T = t, Type = type, Y = y }, BaseUrl, ct));

    /// <summary>Search titles, 10 per page, most votes first, like OMDb's s=.</summary>
    /// <param name="s">The start of the title (also after The/A/An), at least two characters.</param>
    /// <param name="type">movie, series, episode or game.</param>
    /// <param name="y">Year (the first year for a series).</param>
    /// <param name="page">1–100.</param>
    /// <param name="ct"></param>
    [HttpGet("/search")]
    [ProducesResponseType<SearchList>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string s, [FromQuery] string? type = null, [FromQuery] int? y = null,
        [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await omdb.GetAsync(new OmdbRequest { S = s, Type = type, Y = y, Page = page }, BaseUrl, ct));

    /// <summary>The episodes of one season of a series, like OMDb's i=…&amp;Season=….</summary>
    [HttpGet("/series/{imdbId}/season/{season:int}")]
    [ProducesResponseType<Season>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Season(string imdbId, int season, CancellationToken ct) =>
        Ok(await omdb.GetAsync(new OmdbRequest { I = imdbId, Season = season }, BaseUrl, ct));

    /// <summary>One episode of a series, like OMDb's i=…&amp;Season=…&amp;Episode=….</summary>
    [HttpGet("/series/{imdbId}/season/{season:int}/episode/{episode:int}")]
    [ProducesResponseType<Episode>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Episode(string imdbId, int season, int episode, CancellationToken ct) =>
        Ok(await omdb.GetAsync(new OmdbRequest { I = imdbId, Season = season, Episode = episode }, BaseUrl, ct));
}
