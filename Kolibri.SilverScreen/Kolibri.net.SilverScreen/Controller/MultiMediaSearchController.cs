using com.sun.org.apache.bcel.@internal.generic;
using Kolibri.Common.MovieAPI.Controller;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Controller
{
    public class MultiMediaSearchController
    {
        LiteDBController _liteDB;
        TMDBController _TMDB;
        OMDBController _OMDB;   
        MultimediaType _type;

        private UserSettings _settings { get; }
        public MultiMediaSearchController(UserSettings userSettings, LiteDBController liteDB = null, TMDBController tmdb = null, OMDBController omdb = null)
        {
            _settings = userSettings;
            try
            {
                if (liteDB != null) { _liteDB = liteDB; } else { _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath), false, false); }
            }
            catch (Exception) { }
            try
            {
                if (tmdb != null) { _TMDB = tmdb; } else { _TMDB = new TMDBController(_liteDB, _settings.TMDBkey); }
            }
            catch (Exception) { }
            try
            {
                if (omdb != null) { _OMDB = omdb; } else { _OMDB = new OMDBController(_settings.OMDBkey); }
            }
            catch (Exception) { }
        }

        public async void SearchForMovies(DirectoryInfo dir)
        {
            if(_TMDB==null||_OMDB==null) return;    

            List<Item> _currentList = new List<Item>();
            DataTable resultTable = null;

            List<string> common = FileUtilities.MoviesCommonFileExt(true);
            
            var masks = common.Select(r => string.Concat('*', r)).ToArray();
        


            var searchStr = "*." + string.Join("|*.", common);
            foreach (var filter in masks) {
                using (var e = await Task.Run(() => Directory.EnumerateFiles(dir.FullName,filter, new EnumerationOptions() { RecurseSubdirectories = true }).GetEnumerator()))
                {
                    while (await Task.Run(() => e.MoveNext()))
                    {
                        FileInfo file = new FileInfo(e.Current);

                        int year = MovieUtilites.GetYear(file.Directory.Name);
                        if (year.Equals(0) || year.Equals(1))
                            year = MovieUtilites.GetYear(file.Name);

                        string title = MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(file.Name));

                        Item movie = await GetItem(file, year, title);
                        if (movie != null)
                        {
                            _currentList.Add(movie);
                        }
                    };
                } }


        }
        private async Task< Item >GetItem(FileInfo file, int year, string title)
        {
            Item movie = null;
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            if (_TMDB == null)
            {
                try
                {
                    _TMDB = new TMDBController(_liteDB, _settings.TMDBkey);
                }
                catch (Exception) { return movie; }
            }
            else { 


                var test = _liteDB.FindByFileName(file);
                if (test != null)
                {
                    movie = _liteDB.FindItem(test.ImdbId);
                    if (movie != null) return movie;
                }
                //Finn ved hjelp av TMDB
                if (movie == null)
                {
                    try
                    {
                     //   var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                  List<SearchMovie> tLibList=await _TMDB.FetchMovie(title, Convert.ToInt32(year));
                     
                        if (tLibList != null && tLibList.Count == 1)
                        {
                            Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                            if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                            {
                                try
                                {
                                    movie = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                    if (movie != null)
                                    {
                                        if (!_liteDB.Insert(movie) )
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl))
                                                {
                                                    movie.TomatoUrl = file.FullName;
                                                    _liteDB.Update(movie);
                                                    _liteDB.Upsert(new  FileItem(movie.ImdbId, file.FullName));
                                                }
                                            }
                                            catch (Exception)
                                            { }
                                        }
                                        return movie;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    movie = _TMDB.GetMovie(tLibList[0]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        movie = null;
                    }
                }

                //Sjekk om tittelen finnes i LiteDB som tittel/år
                if (movie == null)
                {
                    movie = _liteDB.FindItemByTitle(title, year);

                }

                //Hvis vi ikke har funnet filmen nå, leter vi vha TMDB søk
                if (movie == null)
                {
                    try
                    {
                        var t = Task.Run(() => _TMDB.FetchMovie(title, year));

                        if (t.Exception == null)
                        {
                            if (t.Result != null && t.Result.Count >= 1)
                            {
                                List<SearchMovie> tLibList = t.Result;

                                if (year > 1)
                                {
                                    var result = tLibList.FirstOrDefault(s => s.ReleaseDate.Value.Year.Equals(year) && s.Title.StartsWith(title));
                                    if (result != null)
                                    {
                                        var tmdbMovie = _TMDB.GetMovie(result.Id);
                                        if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                            movie = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                        if (movie != null)
                                        {
                                            if (string.IsNullOrEmpty(movie.TomatoUrl))
                                                movie.TomatoUrl = file.FullName;
                                            _liteDB.Insert(movie);
                                            _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                                            return movie;
                                        }
                                    }
                                }
                            }
                        }
                        else { }
                    }
                    catch (Exception ex)
                    { }
                    //Finn filmen vha omdb tittel og år
                    if (movie == null )
                        movie = _OMDB.GetMovieByIMDBTitle(title, year);
                    if (movie != null)
                    {
                        if (string.IsNullOrEmpty(movie.TomatoUrl))
                            movie.TomatoUrl = file.FullName;
                        if (string.IsNullOrEmpty(movie.TomatoUrl))
                            movie.TomatoUrl = file.FullName;
                        if (!_liteDB.Insert(movie))
                        {
                            _liteDB.Update(movie);
                        };
                        _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                        return movie;
                    }
                    else
                    {
                        movie = new OMDbApiNet.Model.Item() { Title = title, Year = year.ToString(), ImdbRating = "Unknown", Response = "false", TomatoUrl = file.FullName };
                    }
                }
               
                    if (movie != null && (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl)))
                    {
                        try
                        {
                            movie.TomatoUrl = file.FullName;
                            _liteDB.Update(movie);
                            _liteDB.Upsert(new  FileItem(movie.ImdbId, file.FullName));
                        }
                        catch (Exception)
                        { }
                    }

                }
                return movie;
            }

      
        
    }
}
