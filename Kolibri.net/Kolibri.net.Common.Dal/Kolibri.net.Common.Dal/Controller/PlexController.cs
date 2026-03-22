using com.sun.tools.@internal.ws.processor.model;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet;
using OMDbApiNet.Model;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMDbLib.Objects.TvShows;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace Kolibri.net.Common.Dal.Controller
{
    public class PlexController
    {
        private string _plexBaseUrl;
        private string _plexToken;
        private XDocument _playlists;

        private readonly HttpClient _http;

        // imdbId -> PlexMovie
        private readonly Dictionary<string, OMDbApiNet.Model.Item> _cache = new();

        private bool _initialized;
        private UserSettings _settings;
        private ImageCacheDB _icdb;

        /// <summary>
        /// not to be used
        /// </summary>
        public PlexController() { }

        [Obsolete("Use PlexController(UserSettings) instead")]
        public PlexController(string plexBaseUrl, string plexToken)
        {
            _plexBaseUrl = plexBaseUrl.TrimEnd('/');
            _plexToken = plexToken;

            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Plex-Token", plexToken);
        }

        public PlexController(UserSettings settings)
        {
            this._settings = settings;

            _plexBaseUrl = $"http://{_settings.XPlexServerName}:32400";
            _plexBaseUrl = _plexBaseUrl.TrimEnd('/');
            _plexToken = _settings.XPlexToken;
            _http = new HttpClient();
            try
            {
                _http.DefaultRequestHeaders.Add("X-Plex-Token", _plexToken);
                _icdb = new ImageCacheDB(_settings);
            }
            catch (Exception ex) { _icdb = null; }
        }

        public async Task<bool> CheckSettings()
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


        public async Task<OMDbApiNet.Model.Item?> FindByTitleAsync(string title, int? year = null)
        {
            OMDbApiNet.Model.Item ret = null;

            if (!_initialized)
                await InitializeAsync();

            title = title.Trim();

            var matches = _cache.Values
                .Where(m =>
                    string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase));
            if (matches.Count() == 1)
            {
                ret = matches.FirstOrDefault();
                if (ret != null && ret.Year != null && ret.Year.Equals(year?.ToString()))
                {
                    return ret;
                }

            }

            if (matches.Count() < 0)
            {
                matches = _cache.Values
                    .Where(m =>
                        string.Equals(m.Title.Split(" ").FirstOrDefault(), title.Split(" ").FirstOrDefault(), StringComparison.OrdinalIgnoreCase));

            }
            if (matches.Count() == 0)
            {
                matches = _cache.Values
                    .Where(m =>
                        string.Equals(m.Title.StartsWith(title.Split(" ").FirstOrDefault()), StringComparison.OrdinalIgnoreCase));

            }

            if (year.HasValue)
            {
                matches = matches.Where(m =>
                    int.TryParse(m.Year, out var y) && y == year.Value);
            }
            if (matches.Count() == 1)
            {
                // If multiple matches, return the first (or refine further)
                ret = matches.First();
                if (ret != null && _icdb != null)
                    ret.Poster = await _icdb.GetPosterUrlAsync(ret.ImdbId);
            }
            return ret;
        }

        /// <summary>
        /// Public API: find a movie in Plex by IMDb ID
        /// </summary>
        public async Task<OMDbApiNet.Model.Item?> FindByImdbAsync(string imdbId)
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
        public async Task InitializeAsync()
        {
            var movieSectionIds = await GetMovieSectionIdsAsync();

            foreach (var sectionId in movieSectionIds)
            {
                await LoadSectionAsync(sectionId);
            }

            _initialized = true;
        }


        /// <summary>
        ///  /// <summary>
        /// Find all "movie" libraries automatically
        /// playlist type can be audio or video
        /// </summary>
        /// </summary>
        /// <param name="playlistType">audio or video</param>
        /// <returns></returns>
        public async Task<List<string>> GetPlaylistsAsync(string playlistType = "video")
        {
            if (_playlists == null)
            {
                var xml = await _http.GetStringAsync($"{_plexBaseUrl}/playlists");
                _playlists = XDocument.Parse(xml);
            }
            return _playlists.Descendants("Playlist")
                .Where( a=> a.Attribute("playlistType").Value.Equals(playlistType))
                .Select(d =>  d.Attribute("title").Value)
                .ToList();
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
            try
            {
                var url = $"{_plexBaseUrl}/library/sections/{sectionId}/all?includeGuids=1";

                var xml = await _http.GetStringAsync(url);
                if (xml == null) return;
                var doc = XDocument.Parse(xml);

                foreach (var video in doc.Descendants("Video"))
                {
                    var imdbGuid = video.Elements("Guid")
                        .Select(g => g.Attribute("id")?.Value)
                        .FirstOrDefault(id => id != null && id.StartsWith("imdb://"));

                    if (imdbGuid == null)
                        continue;

                    var imdbId = imdbGuid.Replace("imdb://", "");
                    var thumb = video.Attribute("thumb")?.Value;
                    if (thumb != null) { thumb = $"{_plexBaseUrl}{thumb}?X-Plex-Token={_plexToken}"; }

                    try
                    {
                        _cache[imdbId] = new OMDbApiNet.Model.Item
                        {
                            ImdbId = imdbId,
                            Metascore = video.Attribute("ratingKey")?.Value,
                            ImdbRating = video.Attribute("audienceRating")?.Value,
                            Title = video.Attribute("title")?.Value,
                            Year = video.Attribute("year")?.Value,
                            Released = video.Attribute("originallyAvailableAt")?.Value,
                            Plot = video.Attribute("summary")?.Value,
                            Rated = video.Attribute("contentRating")?.Value,
                            Country = $"{video.Elements("Country").Select(g => g.Attribute("tag")?.Value).FirstOrDefault()}",
                            Director = $"{video.Elements("Director").Select(g => g.Attribute("tag")?.Value).FirstOrDefault()}",
                            //  Writer = $"{video.Elements("Country").Select(g => g.Attribute("tag")?.Value).FirstOrDefault(tag => tag != null)}",
                            Writer = string.Join(",", video.Elements("Writer").Select(g => g.Attribute("tag")?.Value).ToArray()),
                            //  Actors = $"{video.Elements("Role").Select(g => g.Attribute("tag")?.Value).FirstOrDefault(tag => tag != null)}",
                            Actors = string.Join(",", video.Elements("Role").Select(g => g.Attribute("tag")?.Value).ToArray()),
                            Type = video.Attribute("type")?.Value,
                            TomatoUrl = video.Attribute("file")?.Value,
                            Runtime = StringUtilities.FormatMinutesAsHoursAndMinutes(Convert.ToInt32(TimeSpan.FromMilliseconds($"{(video.Attribute("duration")?.Value)}".ToLong(0)).TotalMinutes)),
                            Genre = string.Join(",", video.Elements("Genre").Select(g => g.Attribute("tag")?.Value).ToArray()),
                            Website = $"https://www.imdb.com/title/{imdbId}",
                            Poster = (thumb == null) ? "N/A" : thumb,
                            TomatoRating = video.Attribute("ratingKey")?.Value,

                        };
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                var ret = ex.Message;
            }
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            if (!_initialized) await InitializeAsync();
            return _cache.Values.ToList();
        }

        public async Task<bool> AddElementToPlaylist(string playlistName, string imdbId)
        {
            try
            {
                var machineIdentityXml = XDocument.Parse(await _http.GetStringAsync($"{_plexBaseUrl}/identity"));
                var machineIdentity = machineIdentityXml.Root.Attribute("machineIdentifier")?.Value;


                // 1. Finn Playlist ID basert på navn

                var xml = _playlists;

                var playlist = xml.Descendants("Playlist")
                    .FirstOrDefault(p => p.Attribute("title")?.Value == playlistName);


                string playlistKey = playlist.Attribute("ratingKey").Value;

                var item = await FindByImdbAsync(imdbId);
                 

                string itemUri = Uri.EscapeDataString($"server://{machineIdentity}/com.plexapp.plugins.library/library/metadata/{item.TomatoRating}");

                string url = $"{_plexBaseUrl}/playlists/{playlistKey}/items?uri={itemUri}&X-Plex-Token={_settings.XPlexToken}";

                var resp = await _http.PutAsync(url, null);

                if (resp.IsSuccessStatusCode)
                {
                    Console.WriteLine("Element lagt til i spillelisten!");
                    return true;
                }
                else { return false; }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
