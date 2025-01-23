using com.sun.tools.@internal.ws.processor.model;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Dal.Interfaces;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;

using OMDbApiNet.Model;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
//using Microsoft.Office.Interop.Excel;

namespace Kolibri.net.SilverScreen.Controller
{
    public class MultiMediaSearchController
    {

        public event EventHandler<string> ProgressUpdated; 

        public StringBuilder CurrentLog { get; set; }
        private LiteDBController _liteDB;
        private TMDBController _TMDB;
        private OMDBController _OMDB;
        private SeriesCache _seriesCache;


        /// <summary>
        /// Oppdatering: 
        /// null = Ingenting
        /// true =Alt 
        /// false = Kun filinformasjon
        /// </summary>
        private readonly bool? _updateTriState;

        private UserSettings _settings { get; }
        public MultiMediaSearchController(UserSettings userSettings, LiteDBController liteDB = null, TMDBController tmdb = null, OMDBController omdb = null, bool? updateTriState = null)
        {
            CurrentLog = new StringBuilder();
            _settings = userSettings;
            _updateTriState = updateTriState;
            try
            {
                if (liteDB != null) { _liteDB = liteDB; } else { _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath), false, false); }
            }
            catch (Exception ex) { SetStatusLabelText(ex.Message); }
            try
            {
                if (tmdb != null) { _TMDB = tmdb; } else { _TMDB = new TMDBController(_liteDB, _settings.TMDBkey); }
                _seriesCache = new SeriesCache(new FileInfo(_liteDB.ConnectionString.Filename).Directory);

            }
            catch (Exception ex) { SetStatusLabelText(ex.Message); }
            try
            {
                if (omdb != null) { _OMDB = omdb; } else { _OMDB = new OMDBController(_settings.OMDBkey, liteDB); }
            }
            catch (Exception ex) { SetStatusLabelText(ex.Message); }
        }

        private void SetStatusLabelText(string message)
        {
            try
            {
                if (CurrentLog == null) CurrentLog = new StringBuilder();
                string text = $"{DateTime.Now.ToShortTimeString()} {message}";

                CurrentLog.AppendLine(text);
                ProgressUpdated?.Invoke(this, text);
            }
            catch (Exception ex)
            {

            }
        }

        #region Movie Item
        public async void SearchForMovies(DirectoryInfo dir)
        {
            if (_TMDB == null || _OMDB == null) {SetStatusLabelText("Trenger både _TMDB pg _OMDB for å fortsette, sjekk innstillinger/settings.");  return; }

            List<Item> _currentList = new List<Item>();
            DataTable resultTable = null;

            List<string> common = MovieUtilites.MoviesCommonFileExt(true);
            var masks = common.Select(r => string.Concat('*', r)).ToArray();
            var searchStr = "*." + string.Join("|*.", common);
            if (_updateTriState == true)
            {
                //Slett items
                try
                {
                    foreach (FileItem fi in _liteDB.FindAllFileItems(dir))
                    {
                        SetStatusLabelText($"Sletter {fi.FullName} fra databasen.");
                        //_liteDB.DeleteItem(item.ImdbId);
                        _liteDB.Delete(fi); 
                    }
                }
                catch (Exception) { }
            }
            foreach (var filter in masks)
            {
                try
                {

                    using (var e = await Task.Run(() => Directory.EnumerateFiles(dir.FullName, filter, new EnumerationOptions() { RecurseSubdirectories = true }).GetEnumerator()))
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
                                SetStatusLabelText($"Legger til {movie.Title} i listen.");
                            }
                            else
                            {

                            }

                            try
                            {
                                var remove = FileUtilities.GetFileDialogFilter(new List<string>() { "nfo", "txt", "jpg" }.ToArray());

                                var removeFiles = FileUtilities.GetFiles(file.Directory, remove, true);
                                if (removeFiles.Count() >= 1)
                                {
                                    foreach (var item in removeFiles)
                                    {
                                        item.Delete();
                                        SetStatusLabelText($"Slettet {item.Name} fra {file.Directory.Name}.");

                                    }
                                    FileUtilities.DeleteEmptyDirs(file.Directory);
                                }
                            }
                            catch (Exception ex)
                            { }

                        };
                    }
                }

                catch (Exception ex)
                {
                }

            }

            foreach (var item in _currentList)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item.ImdbId) && File.Exists(item.TomatoUrl))
                        _liteDB.Upsert(new FileItem(item.ImdbId, item.TomatoUrl));
                }
                catch (Exception ex)
                {
                }
            }
        }
        private async Task<Item> GetItem(FileInfo file, int year, string title)
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
            else
            {
                //Finnes denne filmen i liteDB, oppdaterer vi kun filstien og returnerer hvis ingenting skal endres forøvrig
                var test = _liteDB.FindByFileName(file);
                if (test != null && _updateTriState == null)
                {
                    movie = _liteDB.FindItem(test.ImdbId);
                    if (movie != null)
                    {
                        movie.TomatoUrl = file.FullName;
                        _liteDB.Update(movie);
                        _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                        return movie;
                    }
                }
                //Finn ved hjelp av TMDB
                if (movie == null)
                {
                    try
                    {
                        //   var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                        List<SearchMovie> tLibList = await _TMDB.FetchMovie(title, Convert.ToInt32(year));

                        if (tLibList != null && tLibList.Count == 1)
                        {
                            try
                            {
                                Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                                if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                {
                                    movie = _liteDB.FindItem(tmdbMovie.ImdbId);
                                    if (movie == null)
                                        movie = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);

                                    if (movie != null)
                                    {
                                        movie.TomatoUrl = file.FullName;
                                        _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                                        _liteDB.Upsert(movie);
                                        return movie;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                movie = _TMDB.GetMovie(tLibList[0]);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (year > 1)
                                {
                                    SearchMovie result;
                                    result = tLibList.Find(y => (y.Title.Equals(title) && y.ReleaseDate.Value.Year.Equals(year))); // && s.Title.StartsWith(title));
                                    if (result == null) result = tLibList.Find(y => (y.Title.Equals(title) && y.ReleaseDate.Value.Year.Equals(year - 1)));
                                    if (result == null) result = tLibList.OrderByDescending(x => x.Popularity).FirstOrDefault(s => s.ReleaseDate.Value.Year.Equals(year));// && s.Title.StartsWith(title));
                                    if (result != null)
                                    {
                                        var tmdbMovie = _TMDB.GetMovie(result.Id);
                                        if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                            if (_updateTriState == null)
                                            {
                                                movie = _liteDB.FindItem(tmdbMovie.ImdbId);
                                            }
                                        if (movie == null)
                                        {
                                            movie = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                        }
                                        if (movie != null)
                                        {

                                            movie.TomatoUrl = file.FullName;
                                            _liteDB.Upsert(movie);
                                            _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                                            return movie;
                                        }
                                        else { }
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                    catch (Exception ex)
                    {
                        movie = null;
                    }
                }

                //Sjekk om tittelen finnes i LiteDB som tittel/år
                if (movie == null && _updateTriState == false)
                {
                    movie = _liteDB.FindItemByTitle(title, year);
                    if (movie != null)
                    {
                        movie.TomatoUrl = file.FullName;
                        _liteDB.Update(movie);
                        _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                        return movie;
                    }
                }

                //Hvis vi ikke har funnet filmen nå, leter vi vha TMDB søk
                if (movie == null)
                {

                    //Finn filmen vha omdb tittel og år
                    if (movie == null)
                        movie = _OMDB.GetMovieByIMDBTitle(title, year);
                    if (movie != null)
                    {
                        movie.TomatoUrl = file.FullName;
                        _liteDB.Upsert(movie);
                        _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                        return movie;
                    }
                    else
                    {
                        //Mangler tmdbID
                        var temp = new OMDbApiNet.Model.Item() { Title = title, Year = year.ToString(), ImdbRating = "Unknown", Response = "false", TomatoUrl = file.FullName };
                    }
                }

                if (movie != null && File.Exists(movie.TomatoUrl))
                {
                    try
                    {
                        if (!movie.Title.Contains("sample", StringComparison.OrdinalIgnoreCase))
                        {

                            movie.TomatoUrl = file.FullName;
                            _liteDB.Update(movie);
                            _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
                        }
                    }
                    catch (Exception)
                    { }
                }

            }
            return movie;
        }
        #endregion
        public async Task<List<KolibriTVShow>> SearchForSeriesAsync(DirectoryInfo dir)
        {
            CurrentLog = new StringBuilder();

            var fList = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(dir, MovieUtilites.MoviesCommonFileExt(true), SearchOption.AllDirectories);
            if (fList.Count() < 1) { SetStatusLabelText($"Fant ingen filer i {dir.FullName}."); return new List<KolibriTVShow>(); }
            var dt = SeriesUtilities.SeriesEpisode(fList);
            var list = DataSetUtilities.ColumnNames(dt);
            List<Episode> epList = DataSetUtilities.ConvertToList<Episode>(dt);
            Dictionary<string, KolibriTVShow> showDic = new Dictionary<string, KolibriTVShow>();
            foreach (var omdbEpisode in epList)
            {
                SetStatusLabelText($"Behandler {omdbEpisode.Title}... Vennligst vent.");
                KolibriTVShow show = new KolibriTVShow() { Title = omdbEpisode.Title };
                    if (showDic.Keys.Contains(omdbEpisode.Title))
                    {
                        show = showDic[omdbEpisode.Title];
                    }
                try
                {
                    bool upsert = false; 
                    if (show.Item == null) show.Item = _liteDB.FindItemByTitle(omdbEpisode.Title).FirstOrDefault() ?? GetItem(show.Title).Result;
                    if (show.TvShow == null) show.TvShow = _liteDB.FindTvShowByTitle(omdbEpisode.Title);
                    Episode ep = omdbEpisode;
                    SeasonEpisode sep = _liteDB.FindSeasonEpisode(show.Title, omdbEpisode.SeasonNumber.ToInt32().ToString(), omdbEpisode.EpisodeNumber.ToInt32().ToString());
                    if (sep == null && show.TvShow != null)
                    {
                        ep = show.EpisodeList.FirstOrDefault(x => x.SeasonNumber.Equals(omdbEpisode.SeasonNumber) && x.EpisodeNumber.Equals(omdbEpisode.EpisodeNumber));
                    }
                    if (sep != null)
                    {
                        var jall=  _liteDB.GetEpisode(sep.ImdbId);
                        if (ep == null) { ep = omdbEpisode; upsert = true; };
                        if (string.IsNullOrWhiteSpace(ep.ImdbId)) ep.ImdbId = sep.ImdbId;
                        if (string.IsNullOrWhiteSpace(ep.SeriesId)) ep.SeriesId = omdbEpisode.SeriesId;
                        if (string.IsNullOrWhiteSpace(ep.Released)) ep.Released = sep.Released;
                        if (!string.IsNullOrWhiteSpace(sep.Title)) ep.Title = sep.Title;
                    }

                    if (!string.IsNullOrEmpty(ep.ImdbId)) { if (upsert) _liteDB.Upsert(ep); }
                    show.EpisodeList.Add(ep);
                    showDic[show.Title] = show;
                }
                catch (AggregateException aex)
                {
                    SetStatusLabelText($"{aex.Message}.");
                }
                catch (Exception ex)
                {
                    SetStatusLabelText($"{ex.Message}.");
                }
            }

            return showDic.Values.ToList();
        }

        private DataTable SearchForSeriesEpisodes(List<string> titleList)
        {
            string currentItem = "Uten navn";
            var totEpisodes = new List<Episode>();
            foreach (var title in titleList)
            {
                currentItem = title;
                SetStatusLabelText($"{currentItem}.");
                try
                {

                    var item = GetItem(currentItem).GetAwaiter().GetResult();
                    if (item != null)
                    {
                        currentItem = item.Title;
                        var imdbId = item.ImdbId;

                        SetStatusLabelText($"Item: {item.Title} - {item.ImdbId}.");

                        TvShow tvShow = _liteDB.FindTvShow(imdbId);
                        if (tvShow == null)
                        {
                            var sTv = Task.Run(() => _TMDB.FetchSerie(currentItem.ToString())).Result.Find(x => x.Name.Equals(item.Title, StringComparison.OrdinalIgnoreCase));
                            tvShow = Task.Run(() => _TMDB.GetTVShow(sTv.Id)).GetAwaiter().GetResult();
                            if (tvShow != null && tvShow.ExternalIds == null)
                                tvShow.ExternalIds = new TMDbLib.Objects.General.ExternalIdsTvShow() { ImdbId = item.ImdbId };
                            _liteDB.Upsert(tvShow);
                        }

                        if (tvShow != null)
                        {
                            foreach (var s in tvShow.Seasons)
                            {
                                if (s.SeasonNumber == 0) s.SeasonNumber = 1;
                                
                                var season = _liteDB.FindSeason(item.Title, s.SeasonNumber.ToString()) ?? _OMDB.SeriesBySeason(item.Title, s.SeasonNumber.ToString());
                                _liteDB.Upsert(season);
                                foreach (var se in season.Episodes)
                                {
                                    var episode = _liteDB.GetEpisode(se.ImdbId);
                                    if (episode == null || string.IsNullOrEmpty(episode.SeriesId))
                                    {
                                        episode = _OMDB.SeriesEpisode(item.Title, s.SeasonNumber.ToString(), se.Episode);
                                        if (episode != null)
                                        {
                                            episode.Response = FileUtilities.SafeFileName(item.Title);
                                            _liteDB.Upsert(episode);
                                        }
                                    }
                                    if (episode != null)
                                    {
                                        if (!totEpisodes.ToList().Any(s => (s.SeriesId == episode.SeriesId && s.Title == episode.Title)))
                                        {
                                            totEpisodes.Add(episode);
                                            var b = _liteDB.Upsert(episode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
                catch (AggregateException aex) { SetStatusLabelText($"{currentItem} {aex.Message} - {aex.InnerException}"); }
                catch (Exception ex) { SetStatusLabelText($"{currentItem} {ex.Message} - {ex.InnerException}"); }//throw new Exception($"Error: {ex} - {currentItem}"); }
            }
            var ds = DataSetUtilities.AutoGenererTypedDataSet(totEpisodes.ToList());
            var ret = FormatReturnTable(ds.Tables[0]);
            ret.TableName = FileUtilities.SafeFileName(currentItem);
            return ret;
        }

        public DataTable SearchForSeriesEpisodes(DirectoryInfo dir)
        {
            SetStatusLabelText($"Init {dir.FullName}.");
            var showList = new List<TvShow>();

            var fList = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(dir, MovieUtilites.MoviesCommonFileExt(true), SearchOption.AllDirectories);
            if (fList.Count() < 1) return null;
            var table = SeriesUtilities.SeriesEpisode(fList);
            string serCol = table.Columns.Contains("Name") ? "Name" : "Title";
            List<DataTable> result = table.AsEnumerable()
            .GroupBy(row => row.Field<string>(serCol))
            .Select(g => g.CopyToDataTable())
            .ToList();

            var titleList = table.AsEnumerable().Select(dr => dr.Field<string>(serCol)).Distinct().ToList().OrderBy(n => n).ToList();
            return SearchForSeriesEpisodes(titleList);
        }
        private DataTable SearchForSeriesEpisodesList(List<string> titleList)
        {
            string currentItem = "Uten navn";
            var totEpisodes = new List<Episode>();
            foreach (var title in titleList)
            {
                currentItem = title;
                try
                {
                    var item = _liteDB.FindItemsByType(OMDbApiNet.OmdbType.Series).Where(x => x.Title.Equals(title)).First();

                    if (item == null)
                    {
                        item = _OMDB.GetSeriesByTitle(title, null);
                        if (item != null)
                        {
                            _liteDB.Upsert(item);
                        }
                    }
                    if (item != null)
                    {
                        currentItem = item.Title;
                        var imdbId = item.ImdbId;

                        TvShow tvShow = _liteDB.FindTvShow(imdbId);
                        if (tvShow == null)
                        {
                            var sTv = Task.Run(() => _TMDB.FetchSerie(currentItem.ToString())).Result.Find(x => x.Name.Equals(item.Title, StringComparison.OrdinalIgnoreCase));
                            tvShow = Task.Run(() => _TMDB.GetTVShow(sTv.Id)).GetAwaiter().GetResult();
                            if (tvShow != null && tvShow.ExternalIds == null)
                                tvShow.ExternalIds = new TMDbLib.Objects.General.ExternalIdsTvShow() { ImdbId = item.ImdbId };
                            _liteDB.Upsert(tvShow);
                        }

                        if (tvShow != null)
                        {
                            foreach (var s in tvShow.Seasons)
                            {
                                if (s.SeasonNumber == 0) s.SeasonNumber = 1;
                                var season = _liteDB.FindSeason(item.Title, s.SeasonNumber.ToString()) ?? _OMDB.SeriesBySeason(item.Title, s.SeasonNumber.ToString());
                                _liteDB.Upsert(season);
                                foreach (var se in season.Episodes)
                                {
                                    var episode = _liteDB.GetEpisode(se.ImdbId);
                                    if (episode == null)
                                    {
                                        episode = _OMDB.SeriesEpisode(item.Title, s.SeasonNumber.ToString(), se.Episode);
                                        episode.Response = FileUtilities.SafeFileName(item.Title);
                                        _liteDB.Upsert(episode);
                                    }
                                    if (episode != null)
                                    {
                                        if (!totEpisodes.ToList().Any(s => (s.SeriesId == episode.SeriesId && s.Title == episode.Title)))
                                        {
                                            totEpisodes.Add(episode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else { }

                }
                catch (Exception ex) { }//throw new Exception($"Error: {ex} - {currentItem}"); }
            }
            var ds = DataSetUtilities.AutoGenererTypedDataSet(totEpisodes.ToList());
            var ret = FormatReturnTable(ds.Tables[0]);
            ret.TableName = FileUtilities.SafeFileName(currentItem);
            return ret;
        }


        private DataTable FormatReturnTable(DataTable? formatTable)
        {
            string name = formatTable.Columns.Contains("Name") ? "name" : "Title";
            string season = formatTable.Columns.Contains("Season") ? "Season" : "SeasonNumber";
            string episode = formatTable.Columns.Contains("Episode") ? "Episode" : "EpisodeNumber";
            DataTable resultTable = new DataView(formatTable, "", $"{name} ASC, {season} ASC, {episode} ASC", DataViewRowState.CurrentRows).ToTable();

            List<string> columns = new List<string>() { "Name", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };
            if ((!resultTable.Columns.Contains("Year") || !resultTable.Columns.Contains("Name")))
                columns = DataSetUtilities.ColumnNames(resultTable).ToList();//series

            resultTable = new DataView(resultTable, "", $"{name} asc, ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());

            if (resultTable.Columns.Contains("Response"))
            {
                resultTable.Columns["Response"].SetOrdinal(0);
            }
            if (resultTable.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(resultTable);
            }

            return resultTable;
        }

        private List<Season> CreateSeasonListTotal(DataTable dataTable)
        {

            List<Season> ret = new List<Season>();

            List<DataTable> result = dataTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("Name"))
                .Select(g => g.CopyToDataTable())
                .ToList();
            foreach (var table in result)
            {
                try
                {
                    DataTable dt = null;

                    List<string> seasons = table.AsEnumerable().Select(x => x["Season"].ToString()).Distinct().ToList();

                    foreach (string season in seasons)
                    {
                        string showTitle = string.Empty; string epTitle = string.Empty; string episode = string.Empty;
                        DataView dv = new DataView(table, $" Season = '{season}'", "Episode", DataViewRowState.CurrentRows);
                        dt = dv.ToTable();
                        var s = (from row in table.AsEnumerable()
                                 select new Season()
                                 {
                                     SeasonNumber = season.ToInt32().ToString(), // $"{row["Season"]}".ToInt32().ToString(),
                                     Title = Kolibri.net.Common.Utilities.MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(row["FullName"].ToString())),
                                     TotalSeasons = seasons.Count.ToString()
                                 }).FirstOrDefault();
                        s.Episodes = new List<SeasonEpisode>();
                           SetStatusLabelText($"File - found {s.Title}");
                        foreach (DataRow row in dt.Rows)
                        {
                            showTitle = row["Name"].ToString();
                            episode = row["Episode"].ToString();
                            try
                            {
                                string filename = Path.GetFileNameWithoutExtension(row["FullName"].ToString());
                                try
                                {
                                    epTitle = filename.Substring(filename.IndexOf(episode));
                                }
                                catch (Exception ex)
                                { }


                                SeasonEpisode ep = new SeasonEpisode()
                                {
                                    Episode = row["Episode"].ToString().ToInt32().ToString(),
                                    Title = epTitle,
                                    ImdbRating = "N/A"
                                };
                                SetStatusLabelText($"File - episode found {ep.JsonSerializeObject()}");
                                SeasonEpisode epCached = _seriesCache.Get(showTitle, s.SeasonNumber.ToInt32(), episode.ToInt32());
                                if (epCached == null)
                                {
                                    try
                                    {
                                        SearchTv sTv = _seriesCache.SearchTvCacheGet(showTitle);
                                        if (sTv == null)
                                        {
                                            sTv = Task.Run(() => _TMDB.FetchSerie(table.Rows[0]["Name"].ToString())).Result.FirstOrDefault();
                                            _seriesCache.TVSeasonCacheSet(sTv);
                                        }
                                        if (sTv != null)
                                        {
                                            TvSeason tvSes = _seriesCache.TVSeasonCacheGet(showTitle, s.SeasonNumber.ToInt32());
                                            if (tvSes == null)
                                            {
                                                tvSes = Task.Run(() => _TMDB.GetSeason(sTv.Id, s.SeasonNumber.ToInt32())).Result;
                                                _seriesCache.TVSeasonCacheSet(showTitle, tvSes);
                                            }
                                            if (tvSes != null)
                                            {
                                                var tvEp = tvSes.Episodes.Find(e => e.EpisodeNumber.Equals(episode.ToInt32()));
                                                ep.Title = tvEp.Name;
                                                ep.Released = tvEp.AirDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                                            }
                                            else
                                            {
                                                var eps = Task.Run(() => _TMDB.GetEpisode(sTv.Id, s.SeasonNumber.ToInt32(), episode.ToInt32())).Result;
                                                ep.Released = eps.AirDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                                                ep.Title = eps.Name;
                                            }
                                        }

                                    }
                                    catch (Exception)
                                    {
                                    }
                                      SetStatusLabelText($"File - episode added to cache {ep.JsonSerializeObject()}");
                                    _seriesCache.Add(showTitle, season.ToInt32(), ep);
                                }
                                else
                                {
                                    ep = epCached;
                                }
                                s.Episodes.Add(ep);
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (s.TotalSeasons.ToInt32() < s.SeasonNumber.ToInt32())
                            s.TotalSeasons = s.SeasonNumber.ToInt32().ToString();
                        ret.Add(s);
                        _seriesCache.Add(showTitle, s);

                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }

            }
            return ret;
        }
        private async Task<Item> GetItem(string seriesName)
        {
            Item item = null;
            try
            { 
            try { item = _liteDB.FindItemByTitle(seriesName).FirstOrDefault(); } catch (Exception ex) { }
            if (item == null)
            {
                var itemList = _liteDB.FindItemsByType(OMDbApiNet.OmdbType.Series);
                item = itemList.Where(x => x.Title.Equals(seriesName)).FirstOrDefault();
                if (item == null)
                {
                    var il = _liteDB.FindAllFileItems().ToList().FindAll(x => Path.GetFileNameWithoutExtension(x.FullName).ToArray().First() == seriesName.ToArray().First());
                    foreach (var fi in il)
                    {
                        var fiName = MovieUtilites.GetMovieTitle(fi.FullName);
                        if (fiName.Equals(seriesName))
                        { item = _liteDB.FindItem(fi.ImdbId); break; }
                    }
                }
            }

            if (item == null)
            {
                List<SearchItem> seriesSearchItemList = _OMDB.GetByTitle(seriesName, OMDbApiNet.OmdbType.Series).ToList();
                if (seriesSearchItemList.Count >= 1)
                {
                    var seriesSearchItem = seriesSearchItemList.Find(x => x.Title.Equals(seriesName));
                    if (seriesSearchItem == null)
                        seriesSearchItem = seriesSearchItemList.FirstOrDefault();

                    try { item = _liteDB.FindItem(seriesSearchItem.ImdbId); } catch (Exception ex) { }

                    if (item == null && seriesSearchItem != null)
                    {
                        item = _OMDB.GetMovieByIMDBid(seriesSearchItem.ImdbId);
                        if (item != null) { _liteDB.Upsert(item); }
                        else { /*SetLabelText($"No Item found for {title}");*/ }
                    }
                }
                if (item == null)
                {
                        try
                        {
                            var t = Task.Run(() => _TMDB.FetchSerie(seriesName));
                            var res = t.Result;
                            if (res.Count > 0)
                            {
                                var tv = Task.Run(() => _TMDB.GetTVShow(res.First().Id));
                                var tvRes = tv.Result;
                                if (tvRes != null && tvRes.ExternalIds != null)
                                {
                                    var imdbId = tvRes.ExternalIds.ImdbId;
                                    item = _OMDB.GetItemByImdbId(imdbId);
                                }
                            }
                            else { SetStatusLabelText($"{seriesName} - no item found ({System.Reflection.MethodBase.GetCurrentMethod().Name})"); }
                        }
                        catch (Exception ex)
                        {
                            SetStatusLabelText($"{seriesName} - {ex}");
                        }
                }
            }
            }
            catch (AggregateException aex)
            {
                SetStatusLabelText($"{seriesName} - {aex} {aex.InnerException}");
            }
            return item;
        }

        private Season GetSeason(Item item, string seriesName, string seasonnumber)
        {
            Season season = null;
            if (season == null && item != null)
            {
                season = _liteDB.FindSeason(item.Title, seasonnumber.ToString());
            }
            else if (item == null)
            {
                season = _liteDB.FindSeason(seriesName, seasonnumber);
            }
            if (season == null)
            {
                if (item != null)
                {
                    season = _OMDB.SeriesByImdbId(item.ImdbId, seasonnumber);
                    if (season == null)
                        season = _OMDB.SeriesBySeason(seriesName, seasonnumber);
                    if (season != null)
                        _liteDB.Upsert(season);
                }
                else
                {
                    season = _OMDB.SeriesBySeason(seriesName, seasonnumber.ToString());
                }
            }

            return season;
        }
        private List<Season> GetSeasonList(string seriesname, string season)
        {
            return _seriesCache.Get(seriesname, season.ToInt32());
        }
        private bool CompareNumberOfSeasonEpisodes(string seriesTitle, Season season, List<Season> listOfFileSeasons)
        {
            bool ret = false;
            try
            {
                Season tempSeason = null;
                List<Season> tempList = listOfFileSeasons;
                if (tempList == null)

                    _seriesCache.Get(seriesTitle, season.SeasonNumber.ToInt32()).ToList();
                tempSeason = tempList.Find(e => e != null && e.SeasonNumber.ToInt32().ToString() == season.SeasonNumber.ToString());

                if (season != null && season.Episodes != null && season.TotalSeasons != null)
                {
                    if (tempSeason.Episodes.Count != season.Episodes.Count())
                    {
                        //          SetLabelText($"{seriesTitle} ({season.SeasonNumber}) - Forskjell mellom angitt OMDB/TMDB antall episoder totalt {season.Episodes.Count}  for sesongen og antall episoder i fillisten {tempSeason.Episodes.Count}");
                    }
                    else
                    {
                        //        SetLabelText($"{seriesTitle} ({season.SeasonNumber}) - OK - angitt OMDB/TMDB antall episoder totalt {season.Episodes.Count}  for sesongen og antall episoder i fillisten {tempSeason.Episodes.Count} stemmer overens.");
                        ret = true;
                    }
                }
            }
            catch (Exception ex) { };
            return ret;
        }
    }
}
