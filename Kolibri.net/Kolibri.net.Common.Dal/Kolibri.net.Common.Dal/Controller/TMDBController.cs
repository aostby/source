using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System.Diagnostics;
using TMDbLib.Objects.Credit;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace Kolibri.net.Common.Dal.Controller
{
    public class TMDBController
    {
        private   TMDbLib.Client.TMDbClient _client;
        private   LiteDBController _contr;
        private string _apiKey = string.Empty;// "YOUR_API_KEY";
        public string ApiKey { get { return _apiKey; } }
        //Account https://www.themoviedb.org/faq/account --- https://www.themoviedb.org/u/aostby behind the barricades
        //API -- https://developers.themoviedb.org/3/getting-started/introduction
        //key https://www.themoviedb.org/talk/60a834cb501cf2005930515e?page=1#60a834cb501cf20059305161

        //rottentomatoes-csharp på github

        public TMDBController(LiteDBController contr, string apikey = null)
        {
            if (contr != null)
                _apiKey = contr.GetUserSettings().TMDBkey;


            else if (string.IsNullOrWhiteSpace(apikey))
            { this._apiKey = GetTMDBKey(); }
            else
            { this._apiKey = apikey; }

            _client = new TMDbLib.Client.TMDbClient(_apiKey);
            _contr = contr;
        }

        private TMDbConfig GetDefaultSettings() {
            string json = @"{""images"":{""base_url"":""http://image.tmdb.org/t/p/"",""secure_base_url"":""https://image.tmdb.org/t/p/"",""backdrop_sizes"":[""w300"",""w780"",""w1280"",""original""],""logo_sizes"":[""w45"",""w92"",""w154"",""w185"",""w300"",""w500"",""original""],""poster_sizes"":[""w92"",""w154"",""w185"",""w342"",""w500"",""w780"",""original""],""profile_sizes"":[""w45"",""w185"",""h632"",""original""],""still_sizes"":[""w92"",""w185"",""w300"",""original""]},""change_keys"":[""adult"",""air_date"",""also_known_as"",""alternative_titles"",""biography"",""birthday"",""budget"",""cast"",""certifications"",""character_names"",""created_by"",""crew"",""deathday"",""episode"",""episode_number"",""episode_run_time"",""freebase_id"",""freebase_mid"",""general"",""genres"",""guest_stars"",""homepage"",""images"",""imdb_id"",""languages"",""name"",""network"",""origin_country"",""original_name"",""original_title"",""overview"",""parts"",""place_of_birth"",""plot_keywords"",""production_code"",""production_companies"",""production_countries"",""releases"",""revenue"",""runtime"",""season"",""season_number"",""season_regular"",""spoken_languages"",""status"",""tagline"",""title"",""translations"",""tvdb_id"",""tvrage_id"",""type"",""video"",""videos""]}";

            
            if (string.IsNullOrEmpty(json)) throw new Exception();

            var tmdbconfig = (TMDbConfig)JsonConvert.DeserializeObject(json, typeof(TMDbConfig));
            return tmdbconfig;
        }
        public string GetImageUrl(string path, int size = 500)
        {
            string ret = string.Empty;
            try { 
            if (string.IsNullOrEmpty(path))
                return path; 
                var tmdbconfig =GetDefaultSettings();
                _client.SetConfig(tmdbconfig);


            ret = _client.GetImageUrl($"w{size}", path).AbsoluteUri;
                // https://developers.themoviedb.org/3/configuration/get-api-configuration
                //return _client.GetImageUrl("40", path).AbsolutePath;

            }
            catch (Exception ex)
            {
                ret = $"https://image.tmdb.org/t/p/w500/{path}";
            }
            return ret;
        }
        public Movie GetMovie(string id, bool isImdbId)
        {
            LiteDBController contr = _contr;

            Movie movie = contr.FindMovie(id.ToString(), isImdbId);
            if (movie == null)
            {
                //13448 angels and demaons
                var t = Task.Run(() => _client.GetMovieAsync(id));
                movie = t.Result;
                if (movie != null && !string.IsNullOrEmpty(movie.ImdbId))
                    contr.Upsert(movie);
            }

            if (movie == null)
                return null;
            var ret = movie;
            return ret;
        }
        public Movie GetMovie(int id)
        {
            LiteDBController contr = _contr;

            Movie movie = contr.FindMovie(id.ToString());
            if (movie == null)
            {
                //13448 angels and demaons
                var t = Task.Run(() => _client.GetMovieAsync(id));
                movie = t.Result;
                if (movie != null && !string.IsNullOrEmpty(movie.ImdbId))
                    contr.Upsert(movie);
            }

            if (movie == null)
                return null;
            var ret = movie;
            return ret;
        }

        public async Task FetchMovie(string moviename)
        {
            string query = moviename;

            // This example shows the fetching of a movie.
            // Say the user searches for "Thor" in order to find "Thor: The Dark World" or "Thor"
            SearchContainer<SearchMovie> results = await _client.SearchMovieAsync(query);

            // The results is a list, currently on page 1 because we didn't specify any page.
            Console.WriteLine("Searched for movies: '" + query + "', found " + results.TotalResults + " results in " +
                              results.TotalPages + " pages");

            // Let's iterate the first few hits
            foreach (SearchMovie result in results.Results.Take(3))
            {
                // Print out each hit
                Console.WriteLine(result.Id + ": " + result.Title);
                Console.WriteLine("\t Original Title: " + result.OriginalTitle);
                Console.WriteLine("\t Release date  : " + result.ReleaseDate);
                Console.WriteLine("\t Popularity    : " + result.Popularity);
                Console.WriteLine("\t Vote Average  : " + result.VoteAverage);
                Console.WriteLine("\t Vote Count    : " + result.VoteCount);
                Console.WriteLine();
                Console.WriteLine("\t Backdrop Path : " + result.BackdropPath);
                Console.WriteLine("\t Poster Path   : " + result.PosterPath);

                Console.WriteLine();
            }

            Spacer();
        }

        public async Task<List<SearchMovie>> FetchMovie(string moviename, int year)
        {
            string query = moviename;

            // This example shows the fetching of a movie.
            // Say the user searches for "Thor" in order to find "Thor: The Dark World" or "Thor"
            SearchContainer<SearchMovie> results = await _client.SearchMovieAsync(query, 0, false, year);
            if (results == null) return null;
            return results.Results;
        }

        public Item GetMovie(SearchMovie searchMovie)
        {
            Movie movie = GetMovie(searchMovie.Id);
            if (movie == null) return null;
            Item item = new Item();


            item.Title = movie.Title;
            item.ImdbId = movie.ImdbId;
            item.Country = movie.OriginalLanguage;
            item.Language = movie.SpokenLanguages[0].Name;
            item.Year = (movie.ReleaseDate == null ? DateTime.Now.Year : movie.ReleaseDate.GetValueOrDefault().Year).ToString();
            item.Type = "Movie";
            item.Poster = item.Poster;
            item.ImdbRating = $"{movie.VoteAverage}";
            item.Genre = string.Join(", ", movie.Genres.Select(s=>s.Name).ToArray());

            return item;

        }

        public  string GetTMDBKey(bool obtain = false, bool replace = false)
        {
            UserSettings settings = _contr.GetUserSettings();
            string ret = string.Empty;
            if (!obtain)
            { 
                ret = settings.TMDBkey;

                if (!string.IsNullOrEmpty(ret) && replace)
                {
                    if (MessageBox.Show($"{ret} - value found. Do you wish to type in a different one?)", System.Reflection.MethodBase.GetCurrentMethod().Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                       settings.TMDBkey = string.Empty;
                        ret = string.Empty;
                        GetTMDBKey();
                    }
                }

            }
            if (!String.IsNullOrEmpty(ret)) return ret;
            else
            {
                bool ok =  InputDialogs.InputBox("TMDB key required, please submit one, or cancel to obtain it.", "Please submit your tmdb key", ref ret) == System.Windows.Forms.DialogResult.OK;
                if (ok)
                {
                    if (!string.IsNullOrEmpty(ret))
                    {
                       settings.TMDBkey = ret;
                        return ret;
                    }
                    {
                        GetTMDBKey();
                    }
                }
                else
                {
                    if (MessageBox.Show("No TMDB key found. Do you want to obtain one?\n\rUsually it's free :)", "OMDB key missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    { Process.Start("https://www.themoviedb.org/login"); }
                    else
                    {
                        ret = string.Empty;
                        throw new Exception("Weeeeelll without key, and one wont obtain key even though one is needed....");
                    }
                }
            }
            settings.TMDBkey = ret;
            _contr.Upsert(settings);
            return ret;
        } 
     
        private static void Spacer()
        {
            Console.WriteLine();
            Console.WriteLine(" ----- ");
            Console.WriteLine();
        }


        #region nye metoder for å utvide reportoaret
        public List<Genre> GetGenreList()
        {
            List<Genre> ret = Task.Run(() => _client.GetMovieGenresAsync()).Result;
            return ret;
        }

        public async Task<List<SearchTv>> FetchSerie(string seriename)
        {
            string query = seriename;

            // This example shows the fetching of a movie.
            // Say the user searches for "Thor" in order to find "Thor: The Dark World" or "Thor"
            SearchContainer<SearchTv> results = await _client.SearchTvShowAsync(query);
            if (results == null) return null;
            return results.Results;
        }
        public async Task<TvShow> GetTVShow(int tvShowId)
        {
            var results = await _client.GetTvShowAsync(tvShowId);
            if (results == null) return null;
            return results;
        }
        public async Task<TvSeason> GetSeason(int tvshowId, int seasonNumber)
        {
            var ret = await _client.GetTvSeasonAsync(tvshowId, seasonNumber);
            return ret;
        }
        public async Task<TvEpisode> GetEpisode(int tvshowId, int seasonNumber, int episodeNumber)
        {
            var ret = await _client.GetTvEpisodeAsync(tvshowId, seasonNumber, episodeNumber);
            return ret;

        }
        public async Task<FindContainer> FindById(string imdbId)
        {
            var ret = await _client.FindAsync(TMDbLib.Objects.Find.FindExternalSource.Imdb, imdbId);
            return ret;

        }
        public async Task< TvEpisode >FindByImdbId(string imdbId, int episodenr)
        {
            var tmp = await _client.FindAsync(TMDbLib.Objects.Find.FindExternalSource.Imdb, imdbId);
            var show = tmp.TvEpisode[0].ShowId;
            var season = tmp.TvEpisode[0].SeasonNumber;

            var next = await _client.GetTvEpisodeAsync(show, season , episodenr);
            return next;

        }

        public async Task<List<SearchMovie>> GetMovieSimilar(string title, int year)
        {
            List<SearchMovie> ret = new List<SearchMovie>();
            var t =  Task.Run(() =>   FetchMovie(title, year) );
            List<SearchMovie> tLibList = t.Result;
            if (tLibList != null && tLibList.Count >= 1)
            {
                SearchContainer<SearchMovie> res = await _client.GetMovieSimilarAsync(tLibList[0].Id);
                ret = res.Results.ToList();
            }
            
            return ret;
        }


        public async Task<TMDbLib.Objects.Movies.Credits> GetMovieCredits(string title, int year)
        {
            
            var t = Task.Run(() => FetchMovie(title, year));
            List<SearchMovie> tLibList = t.Result;
            if (tLibList != null && tLibList.Count >= 1)
            {
                var mof = _client.GetMovieAsync(tLibList.FirstOrDefault().Id);
                var ja = mof.Result;

                var res = await _client.GetAggregateCredits(ja.Id);
              var tull = await  _client.GetCreditsAsync(ja.Id.ToString());

         var        ret = await _client.GetMovieCreditsAsync(ja.Id);
                return ret;

 
                
            }

            return null;
        }

        public async Task<List<Item>> GetMovies(List<SearchMovie> titles)
        {
            List<Item> ret = new List<Item>();
            foreach (SearchMovie item in titles)
            {
                try
                {
                var temp = GetMovie(item);
                    if (temp != null)
                    ret.Add(temp);

                }
                catch (Exception ex)
                {
                }
            }  
            return ret;
        }


        #endregion
    }
}