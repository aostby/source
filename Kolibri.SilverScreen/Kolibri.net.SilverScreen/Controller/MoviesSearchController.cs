using javax.swing.text;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Tools;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Data;
using System.Text;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace Kolibri.net.SilverScreen.Controller
{
    public class MoviesSearchController
    {
        public event EventHandler<string> ProgressUpdated;
        public StringBuilder CurrentLog { get; set; } = new StringBuilder();

        private LiteDBController _liteDB;
        private TMDBController _TMDB;
        private OMDBController _OMDB;
        private PlexController _plex; 
     

        private List<DirectoryInfo> _cleanDirsAfterSearch = new List<DirectoryInfo>();


        /// <summary>
        /// Oppdatering: 
        /// null = Ingenting
        /// true =Alt 
        /// false = Kun filinformasjon
        /// </summary>
        private bool? _updateTriState;

        private UserSettings _settings { get; }
     
        IProgress<int> _progress;

        /// <summary>
        /// Oppdaterer utifra oppgitte parameter
        /// </summary>
        /// <param name="userSettings">MÅ være satt</param>
        /// <param name="liteDB">hvis oppgitt, brukes denne, hvis ikke intansieres den på bakgrunn av userSettings, hvis mulig</param>
        /// <param name="tmdb">hvis oppgitt, brukes denne, hvis ikke intansieres den på bakgrunn av userSettings, hvis mulig</param>
        /// <param name="omdb">hvis oppgitt, brukes denne, hvis ikke intansieres den på bakgrunn av userSettings, hvis mulig</param>
        /// <param name="updateTriState">null = Ingenting, true=alt, false= kun filinfromasjon</param>
        public MoviesSearchController(UserSettings userSettings, LiteDBController liteDB = null, TMDBController tmdb = null, OMDBController omdb = null, PlexController plex = null, IProgress<int> progress = null)
        {
            CurrentLog = new StringBuilder();
            _settings = userSettings;
            _updateTriState = null;
            if (progress == null) _progress = new Progress<int>();
            else _progress = progress;

            try
            {
                if (liteDB != null) { _liteDB = liteDB; } else { _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath), false, false); }
            }
            catch (Exception ex) { SetStatusLabelText(ex.Message, ex.GetType().Name); }
            try
            {
                if (tmdb != null) { _TMDB = tmdb; } else { _TMDB = new TMDBController(_liteDB, _settings.TMDBkey); }
                //   _seriesCache = new SeriesCache(new FileInfo(_liteDB.ConnectionString.Filename).Directory);

            }
            catch (Exception ex) { SetStatusLabelText(ex.Message, ex.GetType().Name); }
            try
            {
                if (omdb != null) { _OMDB = omdb; } else { _OMDB = new OMDBController(_settings.OMDBkey, _liteDB); }
            }
            catch (Exception ex) { SetStatusLabelText(ex.Message, ex.GetType().Name); }

            try
            {
                if (plex != null) { _plex = plex; }
                else if (!string.IsNullOrEmpty(_settings.XPlexToken))
                {
                    _plex = new PlexController(_settings);
                }
            }
            catch (Exception ex)
            {
                _plex = null; SetStatusLabelText(ex.Message, ex.GetType().Name);
            }
        }

        private void SetStatusLabelText(string message, string type = "INFO")
        {
            try
            {
                if (CurrentLog == null) CurrentLog = new StringBuilder();
                string text = $"{DateTime.Now.ToShortTimeString()} [{type.ToUpper()}] {message}";

                CurrentLog.AppendLine(text);
                ProgressUpdated?.Invoke(this, text);
            }
            catch (Exception ex) { }
        }

        #region Movie Item
        public async Task<List<Item>> SearchForMovies(DirectoryInfo dir, bool? updateTriState = null)
        {
            int count = 0;
            _updateTriState = updateTriState;
            ClearIfTristateTrue(dir);

            CurrentLog.Clear();
            _progress?.Report(count);

            List<Item> ret = new List<Item>();
            if (!Init(dir, updateTriState)) return ret;


            var numFiles = await MovieUtilites.GetCommonMovieFiles(dir);
            foreach (var filePath in numFiles)
            {
                _progress?.Report(ProgressBarHelper.CalculatePercent(count, numFiles.Count));
                try
                {
                    count = count + 1;

                    int year; string title; string fileTitle;
                    FileInfo file = new FileInfo(filePath); 

                    GetTitleAndYear(file, out year, out title, out fileTitle);
                    var item = await GetItem(file, year, title, fileTitle);
                    if (item != null)
                        ret.Add(item);
                    else
                    {
                        SetStatusLabelText($"{title} ikke funnet! {file.FullName}.", "NOTFOUND");
                    }
                    _progress?.Report(ProgressBarHelper.CalculatePercent(count, numFiles.Count));
                    await AddToUnwantedExtraFiles(file.Directory);

                }
                catch (Exception ex)
                {
                }
            }
            _progress?.Report(100); 
            await RemoveUnwantedExtraFilesExecute();
                SetStatusLabelText($"Søket fullført.", "FINISHED");     
            return ret;
        }

        private static void GetTitleAndYear(FileInfo file,out int year, out string title, out string fileNameAsTitle )
        {
            var movFile = MovieUtilites.DetectMovieFile(file);
            if (movFile != null)
            {
                year = movFile.Year.ToInt().GetValueOrDefault();               
                title = MovieUtilites.GetMovieTitle(file.Name); 
                fileNameAsTitle = MovieUtilites.GetMovieTitleLight(file.Name);
            }
            else
            {
                year = MovieUtilites.GetYear(file.Directory.Name);
                if (year.Equals(0) || year.Equals(1))
                    year = MovieUtilites.GetYear(file.Name);
                title = MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(file.Name));
                fileNameAsTitle = MovieUtilites.GetMovieTitleLight( Path.GetFileNameWithoutExtension(file.Name));  
            }
        }

        private void ClearIfTristateTrue(DirectoryInfo dir)
        {
            if (_updateTriState==true)
            {
                //Slett items
                try
                {
                    foreach (FileItem fi in _liteDB.FindAllFileItems(dir))
                    {
                      
                        if (_updateTriState == true)
                        {
                            if (!fi.ItemFileInfo.Exists)
                            {
                                SetStatusLabelText($"Sletter {fi.FullName} fra databasen.", "DELETE");
                                _liteDB.DeleteItem(fi.ImdbId);
                                _liteDB.Delete(fi);
                            }
                        }
                       
                    }
                }
                catch (Exception) { }
            }
        }

        private async Task<bool> AddToUnwantedExtraFiles(DirectoryInfo dirInfo)
        {
            bool ret = true;
            if (_cleanDirsAfterSearch == null) _cleanDirsAfterSearch = new List<DirectoryInfo>();

            if (dirInfo != null && Directory.Exists(dirInfo.FullName))
            { _cleanDirsAfterSearch.Add(dirInfo); }
            return ret;
        }
        private async Task<bool> RemoveUnwantedExtraFilesExecute()
        {
            bool ret = true;
            foreach (DirectoryInfo dir in _cleanDirsAfterSearch.Distinct())
            {
                try
                {
                    var remove = FileUtilities.GetFileDialogFilter(new List<string>() { "nfo", "txt", "jpg" }.ToArray());

                    var removeFiles = FileUtilities.GetFiles(dir, remove, true);
                    if (removeFiles.Count() >= 1)
                    {
                        foreach (var item in removeFiles)
                        {

                            try
                            {
                                File.SetAttributes(item.FullName, FileAttributes.Normal); //sett attributter i tilfelle de er read only
                                ret = true;
                            }
                            catch (Exception)
                            {
                                SetStatusLabelText($"Filatrtibutter kan ikke endres {item.Name} fra {dir.Name}.", "ATTRIBUTES");
                                ret = false;
                            }
                            item.Delete();
                            SetStatusLabelText($"Slettet {item.Name} fra {dir.Name}.", "DELETE");
                            ret = true;
                        }
                        FileUtilities.DeleteEmptyDirs(dir);
                        SetStatusLabelText($"Søket fullført, siste var {dir.FullName}.", "FINISHED");
                    }
                }
                catch (Exception ex)
                { SetStatusLabelText(ex.Message); ret = false; }

            }
            _cleanDirsAfterSearch.Clear();
            return ret;

        }
         
        private bool Init(DirectoryInfo dir, bool? updateTriState)
        {
            if (_TMDB == null || _OMDB == null)
            {
                SetStatusLabelText("Trenger både _TMDB pg _OMDB for å fortsette, sjekk innstillinger/settings.", "ERROR");
                return false;
            } 
            _updateTriState = updateTriState;
            return true;
        }

        private async Task<Item> GetItem(FileInfo file, string imdbid)
        {
            Item ret = null;
            if (string.IsNullOrEmpty(imdbid)) return ret;
            try
            {
             ret= await  _liteDB.FindItemAsync(imdbid);

                if (_updateTriState != null&&ret!=null&& string.IsNullOrEmpty(ret.TomatoUrl))
                {
                  
                    if (!$"{ret.TomatoUrl}".Equals(file.FullName)&&!file.FullName.Equals(ret.TomatoUrl))
                    {
                        ret.TomatoUrl = file.FullName;
                        await _liteDB.UpdateAsync(ret);
                        await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                        SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_liteDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS"); 
                    }
                  
                  
                }

            }
            catch (Exception ex)
            {
                SetStatusLabelText($"{ex.Message} - {file.FullName}.", "ERROR");
            } 
            return ret;
        
        }


        /// <summary>
        /// Finnes denne filmen i liteDB, oppdaterer vi kun filstien og returnerer hvis ingenting skal endres forøvrig
        /// </summary>
        /// <param name="file"></param>
        /// <param name="year"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<Item> GetItem(FileInfo file, int year, string title ,string alternativeSearchTitle=null)
        {
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            //Item item = await GetItem(file, filePath.ImdbIdFromDirectoryName());
            Item movie =   await GetItem(file, file.Name.ImdbIdFromDirectoryName());
            if (movie != null && !string.IsNullOrEmpty(movie.TomatoUrl) && File.Exists(movie.TomatoUrl))
            {
                return movie;
            }



            //Finnes denne filmen i liteDB, oppdaterer vi kun filstien og returnerer hvis ingenting skal endres forøvrig
            movie = await _liteDB.FindItemByTitle(title, year, alternativeSearchTitle);
            if (_updateTriState == null && movie != null)
            {
                return movie;
            }
            if (movie == null || !file.FullName.Equals(movie.TomatoUrl))
            {     //Finn ved hjelp av LiteDB eller PLEX
                movie = await SearchLiteDB(file, year, title);
            }

            if (movie != null && !string.IsNullOrEmpty(movie.TomatoUrl) && File.Exists(movie.TomatoUrl))
            {
                return movie;
            }


            //Finn ved hjelp av TMDB
            if (movie == null && _TMDB != null)
            {
                movie = await SearchComplexTMDB(file, year, title);

            }
            //Finn ved hjelp av OMDB
            if (movie == null && _OMDB != null)
            {
                movie = await SearchComplexOMDB(file, year, title);
            }

            return movie;
        }
        private async Task<Item> SearchLiteDB(FileInfo file, int year, string title)
        {
            Item ret = null;
            var test =await  _liteDB.FindByFileNameAsync(file);
                 
            try
            {
                if (test != null)
                {
                    ret = await _liteDB.FindItemAsync(test.ImdbId);
                    if (ret != null)
                    {
                        if (_updateTriState != null)
                        {
                            if (!ret.TomatoUrl.GetHashCode().Equals(test.FullName.GetHashCode()))
                            {
                                ret.TomatoUrl = file.FullName;
                                await _liteDB.UpdateAsync(ret);
                                await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                                SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_liteDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS");
                            }
                            
                            return ret;
                        }
                    }
                    else {
                        if (ret != null && _updateTriState==false) return ret;
                    }
                }
                //plex
                if (ret == null && _plex != null)
                {
                    Item? srcPlex = await _plex?.FindByTitleAsync(title, year);

                    if (srcPlex != null)
                    {
                        ret = srcPlex;
                        if (ret != null)
                        {
                            if (test == null || (!$"{ret.TomatoUrl}".Equals(file.FullName)))
                            {
                                ret.TomatoUrl = file.FullName;
                             var ok=   await _liteDB.UpsertAsync(ret);
                             ok=ok&&   await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));

                            }
                            SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_plex)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS");
                            return ret;
                        }
                    }
                }
            }
            catch (Exception ex)
            { SetStatusLabelText($"{file.FullName} - {ex.Message}", "ERROR"); }
        
            return ret;
        }
        private async Task<Item> SearchComplexTMDB(FileInfo file, int year, string title)
        {
            var test = await _liteDB.FindByFileNameAsync(file);
            if (test != null) {
                var item = await _liteDB.FindItemAsync(test.ImdbId);
                if (item != null) return item;
            }


            Item ret = null;
            try
            {
                //   var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                List<SearchMovie> tLibList = await _TMDB.FetchMovie(title, Convert.ToInt32(year));
                if (tLibList != null && tLibList.Count > 1)
                {
                    try
                    {
                        var match = tLibList.Find(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)); //&& Convert.ToDateTime(x.ReleaseDate, CultureInfo.InvariantCulture).Year.Equals(Convert.ToInt32(year)));
                        if (match != null) { tLibList.Clear(); tLibList.Add(match); }
                    }
                    catch (Exception ex) { }
                }

                if (tLibList != null && tLibList.Count == 1)
                {
                    try
                    {
                        Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                        if (!string.IsNullOrEmpty(tmdbMovie.ImdbId))
                        {
                            ret = await _liteDB.FindItemAsync(tmdbMovie.ImdbId);
                            if (ret == null)
                                ret = await _plex?.FindByImdbAsync(tmdbMovie.ImdbId);
                            if (ret == null)
                                ret = await _OMDB.GetMovieByIMDBidAsync(tmdbMovie.ImdbId);


                            if (ret != null)
                            {
                                if ($"{ret.TomatoUrl}".Equals(file.FullName))
                                {
                                    SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_TMDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS");
                                    return ret;
                                }
                                if (test == null || (!$"{ret.TomatoUrl}".Equals(test.FullName)))
                                {
                                    ret.TomatoUrl = file.FullName;
                                     await _liteDB.UpsertAsync(ret);
                            var testes=        await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                                  
                                }
                                SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_TMDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "NEW");
                                return ret;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = await _TMDB.GetMovieAsync(tLibList[0]);
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
                                        ret = await _liteDB.FindItemAsync(tmdbMovie.ImdbId);
                                    }
                                if (ret == null)
                                {
                                    ret = await  _OMDB.GetMovieByIMDBidAsync(tmdbMovie.ImdbId);
                                }
                                if (ret != null)
                                {
                                    if (!$"{ret.TomatoUrl}".Equals(file.FullName))
                                    {

                                        ret.TomatoUrl = file.FullName;
                                        await _liteDB.UpsertAsync(ret);
                                        _ = await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                                    }
                                    SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_TMDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS");
                                    return ret;
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
                ret = null;
            }

            return ret;
        }
        private async Task<Item> SearchComplexOMDB(FileInfo file, int year, string title)
        {

            Item ret = null;

            //Sjekk om tittelen finnes i LiteDB som tittel/år
            if (ret == null && _updateTriState == false)
            {
                ret = await _liteDB.FindItemByTitle(title, year);
                if (ret != null)
                {
                    if (ret.TomatoUrl.Equals(file.FullName))
                    {
                        ret.TomatoUrl = file.FullName;
                        _liteDB.UpdateAsync(ret);
                        await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                    }
                    SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_liteDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "EXISTS");

                    return ret;
                }
            }

            //Hvis vi ikke har funnet filmen nå, leter vi vha TMDB søk
            if (ret == null)
            {

                //Finn filmen vha omdb tittel og år
                if (ret == null)
                    ret = _OMDB.GetMovieByIMDBTitle(title, year);
                if (ret != null)
                {
                    ret.TomatoUrl = file.FullName;
                    await _liteDB.UpsertAsync(ret);
                    await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                    SetStatusLabelText($"{ret.ImdbId} Fant via [{nameof(_OMDB)}] {ret.Title} - oppdaterer filsti til {file.FullName}.", "NEW");
                    return ret;
                }
            }
            //er dette nødvendig nå?
            if (ret != null && File.Exists(ret.TomatoUrl))
            {
                try
                {
                    if (!ret.Title.Contains("sample", StringComparison.OrdinalIgnoreCase))
                    {
                        var test = await _liteDB.FindByFileNameAsync(file);
                        {
                            if (test != null && !$"{ret.TomatoUrl}".Equals(test.FullName))
                            {
                                ret.TomatoUrl = file.FullName;
                                await _liteDB.UpdateAsync(ret);
                                await _liteDB.UpsertAsync(new FileItem(ret.ImdbId, file.FullName));
                            }
                        }
                    }
                }
                catch (Exception)
                { }
            }

            return ret;
        }




        #endregion

        #region Series (Obsolete - moved)
        //public async Task<List<KolibriTVShow>> SearchForSeriesAsync(DirectoryInfo source)
        //{
        //    CurrentLog = new StringBuilder();
        //    List<KolibriTVShow> list = new List<KolibriTVShow>();    
        //    foreach (DirectoryInfo dir in source.GetDirectories("*.*", SearchOption.AllDirectories))
        //    {
        //        var fList = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(dir, MovieUtilites.MoviesCommonFileExt(true), SearchOption.TopDirectoryOnly);
        //        if (fList.Count() < 1) { continue; } //SetStatusLabelText($"Fant ingen filer i {dir.FullName}.", "NOTFOUND"); return new List<KolibriTVShow>(); }
        //        var dt = SeriesUtilities.SeriesEpisode(fList);
        //        List<Episode> epList = DataSetUtilities.ConvertToList<Episode>(dt);
        //        var tmp =   GetKolibriTVShows(epList, MovieUtilites.GetMovieTitle(dir.FullName));
        //        if (tmp.FirstOrDefault() != null) { list.Add(tmp.FirstOrDefault()); }
        //    }
        //    return list;
        //}

        /*public KolibriTVShow GetShowById(string imdbid)
        {
            if (imdbid.IsNumeric()) throw new Exception("Probably not an ImdbId, they start with tt: " + imdbid);
            Item item = _liteDB.FindItem(imdbid);
            if (item == null) throw new NotFoundException(new TMDbStatusMessage() { StatusCode = 401, StatusMessage = $"Item {imdbid} not found in local database" });

            KolibriTVShow tv = new KolibriTVShow();
            tv.SeasonList = new List<KolibriSeason>(); 
            tv.Item = item;

            for (int i = 0; i < item.TotalSeasons.ToInt32(); i++)
            {
                try
                {
                    var kses = _liteDB.FindSeason(item.ImdbId, i + 1);
                    if (kses == null)
                    {
                        try
                        {
                            Season ses = _OMDB.SeasonByImdbId(item.ImdbId, (i + 1).ToString());
                            kses = JsonConvert.DeserializeObject<KolibriSeason>(ses.JsonSerializeObject());
                            kses.Title = item.Title;
                            kses.SeriesId = item.ImdbId;
                            _liteDB.Upsert(kses);
                        }
                        catch (Exception)
                        { }

                    }
                    if (kses != null)
                    {
                        tv.SeasonList.Add(kses);

                    }

                }
                catch (Exception ex)
                {
                    var jaa = i;
                }
            }
            //List < DataTable > datatableList = new List<DataTable>();
            //foreach (var season in tv.SeasonList)
            //{
            //    datatableList.Add( DataGrivViewControls.EpisodeToDataTable(season));
            //} 
            //var eplist = DataSetUtilities.ConvertToList<Episode>(DataSetUtilities.MergeListOfSimilarTables(datatableList));
            //var ret = GetKolibriTVShows(eplist, item?.Title).FirstOrDefault();
           
            
            return tv;
        }*/
        /*
        public KolibriTVShow GetShowById_old(string imdbid)
        {
            if (imdbid.IsNumeric()) throw new Exception("Probably not an ImdbId, they start with tt: " + imdbid);
            Item item = _liteDB.FindItem(imdbid);
            //TvShow tv = Task.Run(() => _liteDB.FindTvShowByTitle(item.Title.FirstToUpper())).GetAwaiter().GetResult();

            List<DataTable> datatableList = new List<DataTable>();
            List<KolibriSeason> seasonList = new List<KolibriSeason>();

            for (int i = 0; i < item.TotalSeasons.ToInt32(); i++)
            {
                try
                { 
                    var kses = _liteDB.FindSeason(item.ImdbId, i+1);
                    if (kses ==null){
                        try
                        {
                            Season ses = _OMDB.SeasonByImdbId(item.ImdbId, (i + 1).ToString());
                            kses = JsonConvert.DeserializeObject<KolibriSeason>(ses.JsonSerializeObject());
                             kses.Title = item.Title;
                            kses.SeriesId = item.ImdbId;
                            _liteDB.Upsert(kses);
                        }
                        catch (Exception)
                        { }
                        
                    }
                    if (kses != null)
                    {
                        seasonList.Add(kses);

                        var table = DataSetUtilities.AutoGenererDataSet<SeasonEpisode>(kses.Episodes.ToList()).Tables[0];
                        System.Data.DataColumn newColumn = new System.Data.DataColumn("Season", typeof(System.String));
                        newColumn.DefaultValue = i.ToString();
                        table.Columns.Add(newColumn);
                        newColumn = new System.Data.DataColumn("SeriesId", typeof(System.String));
                        newColumn.DefaultValue = kses.SeriesId;
                        table.Columns.Add(newColumn);

                        var epTable = SeriesUtilities.SortAndFormatSeriesTable(table);
                        datatableList.Add(epTable);
                    }

                }
                catch (Exception ex)
                {
                    var jaa = i;
                }
            }
            var eplist = DataSetUtilities.ConvertToList<Episode>(DataSetUtilities.MergeListOfSimilarTables(datatableList));
            var ret= GetKolibriTVShows(eplist, item?.Title).FirstOrDefault();
            ret.SeasonList = seasonList;
            ret.Item = item;
            return ret;
        }
        */
        /* private List<KolibriTVShow> GetKolibriTVShows(List<Episode> epList, string showName=null)
         {
             Dictionary<string, KolibriTVShow> showDic = new Dictionary<string, KolibriTVShow>();
             foreach (var omdbEpisode in epList)
             {
                 SetStatusLabelText($"Behandler {omdbEpisode.Title} - {omdbEpisode.SeasonNumber} - {omdbEpisode.EpisodeNumber}... Vennligst vent.", "INFO");
                 KolibriTVShow show = new KolibriTVShow() { Title = omdbEpisode.Title };
                 if (showDic.Keys.Contains(omdbEpisode.Title))
                 {
                     show = showDic[omdbEpisode.Title]; 
                 }
                 try
                 {
                     bool upsert = false;
                     if (show.Item == null) show.Item = _liteDB.FindItemByTitle(showName??omdbEpisode.Title.FirstToUpper()).FirstOrDefault() ?? GetItem(show.Title).Result;
                     if (show.Item == null) {
                         show.Item = _liteDB.FindItem(omdbEpisode.ImdbId);
                         if (show.Item == null && !string.IsNullOrEmpty(omdbEpisode.ImdbId)) {
                             show.Item = _OMDB.GetItemByImdbId(omdbEpisode.ImdbId);
                             if (show.Item != null) _liteDB.Upsert(show.Item);
                         }
                     }
                     if (show.TvShow == null) show.TvShow = _liteDB.FindTvShowByTitle(showName??omdbEpisode.Title.FirstToUpper());
                     if (show.TvShow == null) show.TvShow = _liteDB.FindTvShowByTitle(showName.FirstToUpper());
                     Episode ep = omdbEpisode;
                     SeasonEpisode sep = _liteDB.FindSeasonEpisode(showName, omdbEpisode.SeasonNumber.ToInt32().ToString(), omdbEpisode.EpisodeNumber.ToInt32().ToString());
                     if (sep == null && show.TvShow != null)
                     {
                         //var tempEp = show.EpisodeList.FirstOrDefault(x => x.SeasonNumber.Equals(omdbEpisode.SeasonNumber) && x.EpisodeNumber.Equals(omdbEpisode.EpisodeNumber));
                         var tempEp = show.EpisodeList.FirstOrDefault(x => x.Season.Equals(omdbEpisode.SeasonNumber) && x.Episode.Equals(omdbEpisode.EpisodeNumber));
                         if (tempEp!=null)
                         { ep = tempEp.DeepCopy<Episode>();
                             ep.SeriesId = show.Item.ImdbId;
                         }
                     }
                     if (sep != null)
                     {
                         var jall = _liteDB.GetEpisode(sep.ImdbId);
                         if (ep == null) { ep = omdbEpisode; upsert = true; };
                         if (string.IsNullOrWhiteSpace(ep.ImdbId)) ep.ImdbId = sep.ImdbId;
                         if (string.IsNullOrWhiteSpace(ep.SeriesId)) ep.SeriesId = omdbEpisode.SeriesId;
                         if (string.IsNullOrWhiteSpace(ep.Released)) ep.Released = sep.Released;
                         if (!string.IsNullOrWhiteSpace(sep.Title)) ep.Title = sep.Title;
                     }

                     if (!string.IsNullOrEmpty(ep.ImdbId)) { if (upsert) _liteDB.Upsert(ep); }
                     {
                         var tmp = ep.DeepCopy<KolibriSeasonEpisode>();
                         tmp.SeriesId = show.Item.ImdbId;
                         if(_liteDB.Upsert(ep))
                             show.EpisodeList.Add(tmp); 

                     }
                     showDic[show.Title] = show;
                 }
                 catch (AggregateException aex)
                 {
                     SetStatusLabelText($"{aex.Message}.", aex.GetType().Name);
                 }
                 catch (Exception ex)
                 {
                     omdbEpisode.Error = ex.Message;
                     SetStatusLabelText($"{omdbEpisode.Title} - {omdbEpisode.SeasonNumber} - {omdbEpisode.EpisodeNumber}: {ex.Message}.", "ERROR");
                 }
             }

             return showDic.Values.ToList();
         }
         */
        /* private async Task<DataTable> SearchForSeriesEpisodes(List<string> titleList)
         {
             string currentItem = "Uten navn";
             var totEpisodes = new List<Episode>();
             foreach (var title in titleList)
             {
                 currentItem = title;
                 SetStatusLabelText($"{currentItem}.", "INFO");
                 try
                 {

                     var item = await  GetItem(currentItem) ;
                     if (item != null)
                     {
                         currentItem = item.Title;
                         var imdbId = item.ImdbId;

                         SetStatusLabelText($"Item: {item.Title} - {item.ImdbId}.", "INFO");

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
                                 KolibriSeason kSeason = season.DeepCopy<KolibriSeason>();
                                 kSeason.SeriesId = imdbId;
                                 _liteDB.Upsert(kSeason);
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
                                             SetStatusLabelText($"Item: {item.Title} - {item.ImdbId} ({episode.SeasonNumber} - {episode.EpisodeNumber}).", "EP_OMDB");
                                         }
                                     }
                                     if (episode != null)
                                     {
                                         if (!totEpisodes.ToList().Any(s => (s.SeriesId == episode.SeriesId && s.Title == episode.Title)))
                                         {
                                             totEpisodes.Add(episode);
                                             SetStatusLabelText($"Item: {item.Title} - {item.ImdbId} ({episode.SeasonNumber} - {episode.EpisodeNumber}).", "EP_ADDList");
                                         }
                                     }
                                 }
                             }
                         }
                     }

                 }
                 catch (AggregateException aex) { SetStatusLabelText($"{currentItem} {aex.Message} - {aex.InnerException}", aex.GetType().Name); }
                 catch (Exception ex) { SetStatusLabelText($"{currentItem} {ex.Message} - {ex.InnerException}", ex.GetType().Name); }//throw new Exception($"Error: {ex} - {currentItem}"); }
             }
             var ds = DataSetUtilities.AutoGenererTypedDataSet(totEpisodes.ToList());
             var ret = SeriesUtilities.SortAndFormatSeriesTable(ds.Tables[0]);
             ret.TableName = FileUtilities.SafeFileName(currentItem);
             return ret;
         }
         */
        /*   public async Task<DataTable> SearchForSeriesEpisodes(DirectoryInfo dir)
           {
               SetStatusLabelText($"Init {dir.FullName}.", "INFO");
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
               return await SearchForSeriesEpisodes(titleList);
           }
      */
        /* private async Task<Item> GetItem(string seriesName)
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
                             item = _OMDB.GetMovieByIMDBid(seriesSearchItem.ImdbId, insert:true);
                             if (item != null) { _liteDB.Upsert(item); }
                             else { SetStatusLabelText($"No Item found for {seriesName} - {seriesSearchItem}"); }
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
                                 if (tvRes != null) {
                                     _liteDB.Upsert(tvRes);
                                 }

                             }
                             else { SetStatusLabelText($"{seriesName} - no item found ({System.Reflection.MethodBase.GetCurrentMethod().Name})", "NOTFOUND"); }
                         }
                         catch (Exception ex)
                         {
                             SetStatusLabelText($"{seriesName} - {ex}", ex.GetType().Name);
                         }
                     }
                 }
             }
             catch (AggregateException aex)
             {
                 SetStatusLabelText($"{seriesName} - {aex} {aex.InnerException}", aex.GetType().Name);
             }
             return item;
         }
         */

        //private Season GetSeason(Item item, string seriesName, string seasonnumber)
        //{
        //    Season season = null;
        //    if (season == null && item != null)
        //    {
        //        season = _liteDB.FindSeason(item.Title, seasonnumber.ToString());
        //    }
        //    else if (item == null)
        //    {
        //        season = _liteDB.FindSeason(seriesName, seasonnumber);
        //    }
        //    if (season == null)
        //    {
        //        if (item != null)
        //        {
        //            season = _OMDB.SeasonByImdbId(item.ImdbId, seasonnumber);
        //            if (season == null)
        //                season = _OMDB.SeriesBySeason(seriesName, seasonnumber);
        //            if (season != null)
        //                _liteDB.Upsert(season);
        //        }
        //        else
        //        {
        //            season = _OMDB.SeriesBySeason(seriesName, seasonnumber.ToString());
        //        }
        //    }

        //    return season;
        //}
        //private List<Season> GetSeasonList(string seriesname, string season)
        //{
        //    return _seriesCache.Get(seriesname, season.ToInt32());
        //}
        /*private bool CompareNumberOfSeasonEpisodes(string seriesTitle, Season season, List<Season> listOfFileSeasons)
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
        }*/
        /*private DataTable SearchForSeriesEpisodesList(List<string> titleList)
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
            var ret = SeriesUtilities.SortAndFormatSeriesTable(ds.Tables[0]);
            ret.TableName = FileUtilities.SafeFileName(currentItem);
            return ret;
        }*/
        /* private List<Season> CreateSeasonListTotal(DataTable dataTable)
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
                         SetStatusLabelText($"File - found {s.Title}", "FOUND");
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
                                 SetStatusLabelText($"File - episode found {ep.JsonSerializeObject()}", "INFO");
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
                                     SetStatusLabelText($"File - episode added to cache {ep.JsonSerializeObject()}", "CACHEADD");
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
         */
        #endregion
    }
}