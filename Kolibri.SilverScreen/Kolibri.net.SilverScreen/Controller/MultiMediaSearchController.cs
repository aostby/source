using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Utilities;
using OMDbApiNet.Model;
using System.Data;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using static Kolibri.net.SilverScreen.Controls.Constants;
using Kolibri.net.Common.Utilities.Extensions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TMDbLib.Objects.TvShows;

namespace Kolibri.net.SilverScreen.Controller
{
    public class MultiMediaSearchController
    {
        LiteDBController _liteDB;
        TMDBController _TMDB;
        OMDBController _OMDB;
        SeriesCache _seriesCache;
        MultimediaType _type;
        private readonly bool _updateNewOnly;

        private UserSettings _settings { get; }
        public MultiMediaSearchController(UserSettings userSettings, LiteDBController liteDB = null, TMDBController tmdb = null, OMDBController omdb = null, bool updateNewOnly = true)
        {
            _settings = userSettings;
            _updateNewOnly = updateNewOnly;
            try
            {
                if (liteDB != null) { _liteDB = liteDB; } else { _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath), false, false); }
            }
            catch (Exception) { }
            try
            {
                if (tmdb != null) { _TMDB = tmdb; } else { _TMDB = new TMDBController(_liteDB, _settings.TMDBkey); }
                _seriesCache = new SeriesCache(new FileInfo(_liteDB.ConnectionString.Filename).Directory);

            }
            catch (Exception) { }
            try
            {
                if (omdb != null) { _OMDB = omdb; } else { _OMDB = new OMDBController(_settings.OMDBkey); }
            }
            catch (Exception) { }
        }
        #region Movie Item
        public async void SearchForMovies(DirectoryInfo dir)
        {
            if (_TMDB == null || _OMDB == null) return;

            List<Item> _currentList = new List<Item>();
            DataTable resultTable = null;

            List<string> common = FileUtilities.MoviesCommonFileExt(true);
            var masks = common.Select(r => string.Concat('*', r)).ToArray();
            var searchStr = "*." + string.Join("|*.", common);

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


                var test = _liteDB.FindByFileName(file);
                if (test != null)
                {
                    if (_updateNewOnly)
                    {
                        movie = _liteDB.FindItem(test.ImdbId);
                        if (movie != null)
                        {
                            movie.TomatoUrl = file.FullName;
                            _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));

                            return movie;
                        }
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
                            Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                            if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                            {
                                try
                                {
                                    movie = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                    if (movie != null)
                                    {

                                        if (!_liteDB.Insert(movie))
                                        {
                                            try
                                            {
                                                if (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl))
                                                {
                                                    movie.TomatoUrl = file.FullName;
                                                    _liteDB.Update(movie);
                                                    _liteDB.Upsert(new FileItem(movie.ImdbId, file.FullName));
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
                if (movie == null && _updateNewOnly)
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
                                    var result = tLibList.FirstOrDefault(s => s.ReleaseDate.Value.Year.Equals(year));// && s.Title.StartsWith(title));
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
                    if (movie == null)
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
                        //Mangler tmdbID
                        var temp  = new OMDbApiNet.Model.Item() { Title = title, Year = year.ToString(), ImdbRating = "Unknown", Response = "false", TomatoUrl = file.FullName};
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

        public async void SearchForSeries(DirectoryInfo dir)
        {  Dictionary<string, Season> _seasonDic = null;
        List<Season> listOfFileSeasons = null;
            List<Season> listOfSeasons = null;

            #region method

            DataTable resultTable = null;

            List<string> common = Kolibri.net.Common.Utilities.FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(dir, searchStr, true);
            if (list.Count() < 1) return;

            var table = SeriesUtilities.SeriesEpisode(list);
            var totalSeasonList = CreateSeasonListTotal(table);

            var count = 0;
            foreach (DataRow row in table.Rows)
            {
                string seasonnumber = "0";

                try
                {
                    #region row

                    Item item = null;
                    Season season = null;
                    FileInfo file = new FileInfo(row["FullName"].ToString());
                    string title =  MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(file.Name));

                    string seriesName = row["Name"].ToString();
                    if (_seasonDic != null)
                    {
                        var well = _seasonDic.Keys;
                        if (well.FirstOrDefault() == (seriesName))
                        {
                            //   seasonepisodeList = _seasonDic[seriesName].ToList();
                        }
                    }
                    seasonnumber = row["Season"].ToString().ToInt32().ToString(); //$"{row["Season"]}".Replace("S", string.Empty).Replace("s", string.Empty).ToInt32().ToString();
                    int epNr = $"{row["Episode"]}".ToInt32();

                //    SetLabelText($"{title} {seasonnumber} {epNr} - {seriesName}");

                    item = GetItem(title, seriesName);
                    season = GetSeason(item, seriesName, seasonnumber);



                    if (season == null)
                    {
                        if (listOfSeasons == null)
                            listOfSeasons = _seriesCache.Get(seriesName, seasonnumber.ToInt32()).ToList();
                        season = listOfSeasons.Find(e => e != null && e.SeasonNumber.ToInt32().ToString() == seasonnumber.ToString());

                        if (season != null)
                        {
                            _liteDB.Upsert(season);
                        }
                    }

                    if (season == null)
                        continue;


                    var episodesExists = CompareNumberOfSeasonEpisodes(title, season, totalSeasonList);


                    SeasonEpisode seasonepisode = null;
                    seasonepisode = season.Episodes.Find(e => e.Episode.Replace("E", string.Empty).Replace("e", string.Empty).ToInt32().ToString() == epNr.ToString());

                    if (seasonepisode == null)
                    {
                        seasonepisode = _liteDB.FindSeasonEpisode(seriesName, seasonnumber, epNr.ToString());
                    }
                    if (seasonepisode == null)
                    {
                        seasonepisode = _seriesCache.Get(seriesName, seasonnumber.ToInt32(), epNr);
                    }

                    if (seasonepisode == null)
                    {
                        if (listOfFileSeasons == null)
                        {
                            listOfFileSeasons = CreateSeasonListTotal(table);

                        }
                        listOfSeasons = listOfFileSeasons.FindAll(lfs => lfs.Title.Equals(seriesName, StringComparison.OrdinalIgnoreCase));

                        try
                        {
                            if (listOfSeasons == null)
                            {

                                try
                                {

                                    var tempSeason = GetSeasonList(seriesName, seasonnumber);

                                }
                                catch (Exception ex)
                                {

                                }

                                //      tempseason = seasonepisodeList.Find(e => e.SeasonNumber.ToInt().GetValueOrDefault().ToString() == seasonnumber.ToString());

                                //    if (!_seasonDic.ContainsKey(seriesName))
                                //        _seasonDic.Add()
                                //                                   seasonepisodeList =
                            }
                            {
                                //var temlist = seasonepisodeList(e==> e)
                            }


                            var tempseason = listOfSeasons.Find(e => e.SeasonNumber.ToInt32().ToString() == seasonnumber.ToString());

                            seasonepisode = tempseason.Episodes.Find(e => e.Episode.Replace("E", string.Empty).Replace("e", string.Empty).ToInt32().ToString() == epNr.ToString());
                            seasonepisode.Episode = seasonepisode.Episode.Replace("E", string.Empty).Replace("e", string.Empty).ToInt32().ToString();
                            var tmdbShow = Task.Run(() => _TMDB.FetchSerie(seriesName)).Result.FirstOrDefault();
                            var tmdbEp = Task.Run(() => _TMDB.GetEpisode(tmdbShow.Id, seasonnumber.Replace("S", string.Empty).ToInt32(), epNr)).Result;
                            seasonepisode.Title = tmdbEp.Name;
                            seasonepisode.Released = tmdbEp.AirDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                            if (tmdbEp.ExternalIds != null)
                            {
                                string jalla = "imdb_id";
                            }

                            season.Episodes.Add(seasonepisode);
                            _liteDB.Upsert(season);

                        }
                        catch (Exception ex)
                        {
                         //   SetLabelText(ex.Message);
                            try
                            {//???????
                                var episode = _OMDB.SeriesEpisode(seriesName, $"{seasonnumber}", $"{epNr}");
                                using (var ms = new MemoryStream())
                                {
#pragma warning disable SYSLIB0011
                                    IFormatter formatter = new BinaryFormatter();
                                    formatter.Serialize(ms, episode);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    seasonepisode = (SeasonEpisode)formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
                                }
                            }
                            catch (Exception) { }
                        }
                    }

                  //  SetLabelText($"Season found: {season.Title} {season.SeasonNumber}/{season.TotalSeasons} episodes: {season.Episodes.Count}. Totalt i folder(e): {count}/{list.Count()}");
                    count++;
                    int year = DateTime.Now.Year;
                    try
                    {
                        year = seasonepisode.Released.ToDateTime().Year;
                    }
                    catch (Exception ex) { }

//                    SetLabelText($" ({count}/{list.Count()}) {title} - {year}");


                    if (seasonepisode != null)
                    {
                        _liteDB.Upsert(seasonepisode);
                        if (true)
                        {
                            _liteDB.Upsert(item);
                            _liteDB.Upsert(new  FileItem(item.ImdbId, file.Directory.FullName));
                            if (seasonepisode != null && seasonepisode.ImdbId != null)
                            {
                                _liteDB.Upsert(seasonepisode);
                                _liteDB.Upsert(new  FileItem(seasonepisode.ImdbId, file.FullName));
                            }
                        }


                        var tempTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { seasonepisode });
                        tempTable.Tables[0].Columns.Add("Name");
                        tempTable.Tables[0].Rows[0]["Name"] = row["Name"];
                        tempTable.Tables[0].Columns["Name"].SetOrdinal(0);
                        if (!tempTable.Tables[0].Columns.Contains("Season"))
                        {
                            tempTable.Tables[0].Columns.Add("Season");
                            tempTable.Tables[0].Rows[0]["Season"] = seasonnumber;
                        }

                        if (resultTable == null)
                        {
                            resultTable = tempTable.Tables[0];
                        }
                        else
                        {
                            resultTable.Merge(tempTable.Tables[0]);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    string message = $"{row["Name"]} {row["Season"]} ep:{row["Episode"]}";
                //    SetLabelText($"{message} {ex.Message}");
                    Application.DoEvents();
                    Thread.Sleep(2000);
                }
            }

            var temp = new DataView(resultTable, "", "name ASC, Season ASC, Episode ASC", DataViewRowState.CurrentRows).ToTable();
            resultTable = temp;
          //  resultTable.TableName = Kolibri.net.Common.Utilities.ExcelUtilities.LegalSheetName((form.SourcePath.Name));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));
            if (resultTable.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(resultTable);
            }

         //   ShowGridView(resultTable);

            if (true)
            {
                DataSet ds;
                List<string> columns = new List<string>() { "Name", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };
                if (!resultTable.Columns.Contains("Year"))
                    columns = DataSetUtilities.ColumnNames(resultTable ).ToList();//series

                temp = new DataView(resultTable, "", "name asc, ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
                if (temp.DataSet == null)
                {
                    ds = new DataSet();
                    ds.Tables.Add(temp);
                }
          //      string filePath = Path.Combine(dir.FullName, resultTable.TableName + ".xlsx");
          //      try { if (File.Exists(filePath)) File.Delete(filePath); } catch (Exception) { }

           //     Kolibri.net.Common.Utilities.ExcelUtilities.GenerateExcel2007(new FileInfo(filePath), temp.DataSet);
            //    Kolibri.net.Common.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(filePath));
            }

            #endregion
           
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
                   //     SetLabelText($"File - found {s.Title}");
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
                                //SetLabelText($"File - episode found {ep.JsonSerializeObject()}");
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
                                  //  SetLabelText($"File - episode added to cache {ep.JsonSerializeObject()}");
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
        private Item GetItem(string title, string seriesName)
        {
            Item item = null;
            try { item = _liteDB.FindItemByTitle(seriesName).FirstOrDefault(); } catch (Exception ex) { }
            if (item == null)
            {
                List<SearchItem> seriesSearchItemList = _OMDB.GetByTitle(seriesName, OMDbApiNet.OmdbType.Series).ToList();



                var seriesSearchItem = seriesSearchItemList.Find(x => x.Title.Equals(seriesName));
                if (seriesSearchItem == null)
                    seriesSearchItem = seriesSearchItemList.FirstOrDefault();
                if (seriesSearchItemList.Count > 1)
                   

                try { item = _liteDB.FindItem(seriesSearchItem.ImdbId); } catch (Exception ex) { }
                if (item == null)
                {
                    item = _OMDB.GetMovieByIMDBid(seriesSearchItem.ImdbId);
                    if (item != null) { _liteDB.Upsert(item); }
                    else { /*SetLabelText($"No Item found for {title}");*/ }
                }
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
