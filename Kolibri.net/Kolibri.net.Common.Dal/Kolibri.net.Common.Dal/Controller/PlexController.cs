using System.Xml.Linq;
namespace Kolibri.net.Common.Dal.Controller
{ 
    public class PlexController
    {
        private readonly string _plexBaseUrl;
        private readonly string _plexToken;
        private readonly HttpClient _http;

        // imdbId -> PlexMovie
        private readonly Dictionary<string, PlexMovie> _cache = new();
        private bool _initialized;

        public PlexController(string plexBaseUrl, string plexToken)
        {
            _plexBaseUrl = plexBaseUrl.TrimEnd('/');
            _plexToken = plexToken;

            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Plex-Token", plexToken);
        }

        public async Task< bool> CheckSettings()
        {
            //http://<PLEX_HOST>:32400/library/sections?X-Plex-Token=YOUR_TOKEN
            try
            {
                var url = $"{_plexBaseUrl}/library/sections/?X-Plex-Token={_plexToken}";

                var xml = await _http.GetStringAsync(url);
                var doc = XDocument.Parse(xml);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        public async Task<PlexMovie?> FindByTitleAsync(string title, int? year = null)
        {
            if (!_initialized)
                await InitializeAsync();

            title = title.Trim();

            var matches = _cache.Values
                .Where(m =>
                    string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase));

            if (year.HasValue)
            {
                matches = matches.Where(m =>
                    int.TryParse(m.Year, out var y) && y == year.Value);
            }

            // If multiple matches, return the first (or refine further)
            return matches.FirstOrDefault();
        }

        /// <summary>
        /// Public API: find a movie in Plex by IMDb ID
        /// </summary>
        public async Task<PlexMovie?> FindByImdbAsync(string imdbId)
        {
            if (!_initialized)
                await InitializeAsync();

            _cache.TryGetValue(imdbId, out var movie);
            return movie;
        }

        /// <summary>
        /// One-time initialization:
        /// 1. Find all movie sections
        /// 2. Scan movies
        /// 3. Build IMDb cache
        /// </summary>
        private async Task InitializeAsync()
        {
            var movieSectionIds = await GetMovieSectionIdsAsync();

            foreach (var sectionId in movieSectionIds)
            {
                await LoadSectionAsync(sectionId);
            }

            _initialized = true;
        }

        /// <summary>
        /// Find all "movie" libraries automatically
        /// </summary>
        private async Task<List<int>> GetMovieSectionIdsAsync()
        {
            var xml = await _http.GetStringAsync($"{_plexBaseUrl}/library/sections");
            var doc = XDocument.Parse(xml);

            return doc.Descendants("Directory")
                .Where(d => d.Attribute("type")?.Value == "movie")
                .Select(d => int.Parse(d.Attribute("key")!.Value))
                .ToList();
        }

        /// <summary>
        /// Load movies from a single Plex movie library
        /// </summary>
        private async Task LoadSectionAsync(int sectionId)
        {
            var url =
                $"{_plexBaseUrl}/library/sections/{sectionId}/all?includeGuids=1";

            var xml = await _http.GetStringAsync(url);
            var doc = XDocument.Parse(xml);

            foreach (var video in doc.Descendants("Video"))
            {
                var imdbGuid = video.Elements("Guid")
                    .Select(g => g.Attribute("id")?.Value)
                    .FirstOrDefault(id => id != null && id.StartsWith("imdb://"));

                if (imdbGuid == null)
                    continue;

                var imdbId = imdbGuid.Replace("imdb://", "");

                _cache[imdbId] = new PlexMovie
                {
                    ImdbId = imdbId,
                    RatingKey = video.Attribute("ratingKey")?.Value,
                    Title = video.Attribute("title")?.Value,
                    Year = video.Attribute("year")?.Value
                };
            }
        }
    }

    public class PlexMovie
    {
        public string ImdbId { get; set; }
        public string RatingKey { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
    }
}