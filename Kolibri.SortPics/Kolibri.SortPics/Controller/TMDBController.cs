﻿using Kolibri.MovieAPI.Controller;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using SortPics.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using static System.Net.WebRequestMethods;

namespace SortPics.Controller
{
    public class TMDBController
    {
        private static TMDbLib.Client.TMDbClient _client;
        private static LiteDBController _contr;
        string _apiKey = "YOUR_API_KEY";
        //Account https://www.themoviedb.org/faq/account --- https://www.themoviedb.org/u/aostby behind the barricades
        //API -- https://developers.themoviedb.org/3/getting-started/introduction
        //key https://www.themoviedb.org/talk/60a834cb501cf2005930515e?page=1#60a834cb501cf20059305161
       

        public TMDBController(LiteDBController contr,string apikey = null  )
        {
            if (apikey == null)
                this._apiKey = GetTMDBKey();
            else
                this._apiKey = apikey;

            _client = new TMDbLib.Client.TMDbClient(_apiKey);
           _contr = contr;  
        }
        public string GetImageUrl(string path, int size = 500)
        {
            string ret = string.Empty;

            if (string.IsNullOrEmpty(path))
                return path;
            try
            {
                if (!_client.HasConfig)
                {
                    try
                    {
                        var json = AccessConfig.TMCBConfig;
                        if (string.IsNullOrEmpty(json)) throw new Exception();

                        var tmdbconfig = (TMDbConfig)JsonConvert.DeserializeObject(json, typeof(TMDbConfig));
                        _client.SetConfig(tmdbconfig);
                    }
                    catch (Exception)
                    {
                        var conf = _client.GetConfigAsync().Result;
                        AccessConfig.TMCBConfig = JsonConvert.SerializeObject(conf);
                        Settings.Default.Save();
                    }  
                }
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
         
        public Movie GetMovie(int id)
        {
            LiteDBController contr = _contr;
     
            Movie movie = contr.FindMovie(id.ToString());
            if (movie == null)
            {
                //13448 angels and demaons
                var t = Task.Run(() => _client.GetMovieAsync(id));
                movie = t.Result;
                if (movie != null&&!string.IsNullOrEmpty(movie.ImdbId))
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

        internal Item GetMovie(SearchMovie searchMovie)
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


            return item;

        }

        internal static string GetTMDBKey(bool obtain=false)
        {
            string ret = string.Empty;
            if (!obtain)
            {
                ret = AccessConfig.TMDBkey; 
            }
            if (!String.IsNullOrEmpty(ret)) return ret;
            else
            {
                bool ok = Kolibri.FormUtilities.InputDialogs.InputBox("TMDB key required, please submit one, or cancel to obtain it.", "Please submit your tmdb key", ref ret) == System.Windows.Forms.DialogResult.OK;
                if (ok)
                {
                    if (!string.IsNullOrEmpty(ret))
                    {
                        AccessConfig.TMDBkey = ret;
                        return ret;
                    }
                    {
                        GetTMDBKey();
                    }
                }
                else
                {
                    if (MessageBox.Show("No OMDB key found. Do you want to obtain one?\n\rUsually it's free :)", "OMDB key missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    { Process.Start("https://www.themoviedb.org/login"); }
                    else
                    {
                        ret = string.Empty;
                        throw new Exception("Weeeeelll without key, and one wont obtain key even though one is needed....");
                    }
                }
            }
            AccessConfig.TMDBkey = ret;
            return ret;
        }


        //private   async Task Main( )
        //{
        //    // Instantiate a new client, all that's needed is an API key, but it's possible to 
        //    // also specify if SSL should be used, and if another server address should be used.
        //      TMDbClient client = _client;

        //    // We need the config from TMDb in case we want to get stuff like images
        //    // The config needs to be fetched for each new client we create, but we can cache it to a file (as in this example).
        //    await FetchConfig(client);

        //    // Try fetching a movie
        //    await FetchMovieExample(client);

        //    // Once we've got a movie, or person, or so on, we can display images. 
        //    // TMDb follow the pattern shown in the following example
        //    // This example also shows an important feature of most of the Get-methods.
        //    await FetchImagesExample(client);

        //    Console.WriteLine("Done.");
        //    Console.ReadLine();
        //}

        //private static async Task FetchConfig(TMDbClient client)
        //{
        //    FileInfo configJson = new FileInfo("config.json");

        //    Console.WriteLine("Config file: " + configJson.FullName + ", Exists: " + configJson.Exists);

        //    if (configJson.Exists && configJson.LastWriteTimeUtc >= DateTime.UtcNow.AddHours(-1))
        //    {
        //        Console.WriteLine("Using stored config");
        //        string json = File.ReadAllText(configJson.FullName, Encoding.UTF8);

        //        //client.SetConfig(Instance.DeserializeFromString<TMDbConfig>(json));
        //    }
        //    else
        //    {
        //        Console.WriteLine("Getting new config");
        //        TMDbConfig config = await client.GetConfigAsync();

        //        Console.WriteLine("Storing config");
        //        string json = TMDbJsonSerializer.Instance.SerializeToString(config);
        //        File.WriteAllText(configJson.FullName, json, Encoding.UTF8);
        //    }

        //    Spacer();
        //}

        //private static async Task FetchImagesExample(TMDbClient client)
        //{
        //    const int movieId = 76338; // Thor: The Dark World (2013)

        //    // In the call below, we're fetching the wanted movie from TMDb, but we're also doing something else.
        //    // We're requesting additional data, in this case: Images. This means that the Movie property "Images" will be populated (else it will be null).
        //    // We could combine these properties, requesting even more information in one go:
        //    //      client.GetMovieAsync(movieId, MovieMethods.Images);
        //    //      client.GetMovieAsync(movieId, MovieMethods.Images | MovieMethods.Releases);
        //    //      client.GetMovieAsync(movieId, MovieMethods.Images | MovieMethods.Trailers | MovieMethods.Translations);
        //    //
        //    // .. and so on..
        //    // 
        //    // Note: Each method normally corresponds to a property on the resulting object. If you haven't requested the information, the property will most likely be null.

        //    // Also note, that while we could have used 'client.GetMovieImagesAsync()' - it was better to do it like this because we also wanted the Title of the movie.
        //    Movie movie = await client.GetMovieAsync(movieId, MovieMethods.Images);

        //    Console.WriteLine("Fetching images for '" + movie.Title + "'");

        //    // Images come in two forms, each dispayed below
        //    Console.WriteLine("Displaying Backdrops");
        //    await ProcessImages(client, movie.Images.Backdrops.Take(3), client.Config.Images.BackdropSizes);
        //    Console.WriteLine();

        //    Console.WriteLine("Displaying Posters");
        //    await ProcessImages(client, movie.Images.Posters.Take(3), client.Config.Images.PosterSizes);
        //    Console.WriteLine();

        //    Spacer();
        //}

        //private static async Task ProcessImages(TMDbClient client, IEnumerable<ImageData> images, IEnumerable<string> sizes)
        //{
        //    // Displays basic information about each image, as well as all the possible adresses for it.
        //    // All images should be available in all the sizes provided by the configuration.

        //    List<ImageData> imagesLst = images.ToList();
        //    List<string> sizesLst = sizes.ToList();

        //    foreach (ImageData imageData in imagesLst)
        //    {
        //        Console.WriteLine(imageData.FilePath);
        //        Console.WriteLine("\t " + imageData.Width + "x" + imageData.Height);

        //        // Calculate the images path
        //        // There are multiple resizing available for each image, directly from TMDb.
        //        // There's always the "original" size if you're in doubt which to choose.
        //        foreach (string size in sizesLst)
        //        {
        //            Uri imageUri = client.GetImageUrl(size, imageData.FilePath);
        //            Console.WriteLine("\t -> " + imageUri);
        //        }

        //        Console.WriteLine();
        //    }

        //    // Download an image for testing, uses the internal HttpClient in the API.
        //    Console.WriteLine("Downloading image for the first url, as a test");

        //    Uri testUrl = client.GetImageUrl(sizesLst.First(), imagesLst.First().FilePath);
        //    byte[] bts = await client.GetImageBytesAsync(sizesLst.First(), imagesLst.First().FilePath);

        //    Console.WriteLine($"Downloaded {testUrl}: {bts.Length} bytes");
        //}

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
            var ret =  await _client.GetTvSeasonAsync(tvshowId, seasonNumber);
            return ret;
        }
        public async Task<TvEpisode> GetEpisode(int tvshowId, int seasonNumber, int episodeNumber)
        {
            var ret = await _client.GetTvEpisodeAsync(tvshowId, seasonNumber, episodeNumber);
            return ret;   

        } 
        #endregion

    }
}