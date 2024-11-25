using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using Sayaka.Common; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using Kolibri.Common.MovieAPI.Controller;
using Kolibri.Common.MovieAPI.Entities;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using static Kolibri.Common.MovieAPI.Controller.LiteDBController;

namespace SortPics.Forms
{
    public partial class SeriesForm : Form
    {
        private MALFileForm form;
        private OMDBController _OMDB;
        private  LiteDBController _LITEDBSeries;
        private TMDBController _TMDB;
        private SeriesCache _seriesCache;

        private string report = string.Empty;

        private Dictionary<string, Season> _seasonDic = null;

        public SeriesForm()
        {
            InitializeComponent();
            Init();
        }

        private void SetLabelText(string text)
        {
            toolStripStatusLabelFilnavn.TextAlign = ContentAlignment.MiddleLeft;
            toolStripStatusLabelFilnavn.Text = text;
            report += $"{Environment.NewLine}{DateTime.Now.ToString("s")} {text}";
            try
            {
                toolStripStatusLabelFilnavn.Parent.Update();
            }
            catch (Exception)
            { }
        }

        private void Init()
        {
            string omdbkey = OMDBController.GetOmdbKey();
            this.Text = $"Search for movies via OMDB ({Assembly.GetExecutingAssembly().GetName().Version.ToString()})";
            form = new MALFileForm("TV");
            form.DestinationPath = form.SourcePath;
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.Visible = true;
            splitContainer1.Panel1.Controls.Add(form);
            try
            {
                _LITEDBSeries = new  LiteDBController(true, false, true);
                linkLabelLiteDB.Text = _LITEDBSeries.ConnectionString.Filename;
                linkLabelLiteDB.Tag = linkLabelLiteDB.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Continuing with default settings");
                _LITEDBSeries = new LiteDBController(false, false);
                linkLabelLiteDB.Text = _LITEDBSeries.ConnectionString.Filename;
                linkLabelLiteDB.Tag = linkLabelLiteDB.Text;
            }

            try
            {
                FlowLayoutPanel linkPanel = new FlowLayoutPanel();
                linkPanel.HorizontalScroll.Enabled = false;
                linkPanel.FlowDirection = FlowDirection.TopDown;
                linkPanel.Height = groupBoxMovieLinks.Height;
                linkPanel.Width = groupBoxMovieLinks.Width;
                foreach (var key in  MovieUtilites.MovieLinksDic.Keys)
                {
                    LinkLabel link = new LinkLabel();
                    link.Width = 5000;
                    link.Text = key;
                    link.Tag =  MovieUtilites.MovieLinksDic[key];
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                    linkPanel.Controls.Add(link);
                }
                groupBoxMovieLinks.Controls.Add(linkPanel);
            }
            catch (Exception)
            { }
            _OMDB = new  OMDBController(omdbkey, _LITEDBSeries);
            _TMDB = new TMDBController(_LITEDBSeries);
            _seriesCache = new SeriesCache(new FileInfo(_LITEDBSeries.ConnectionString.Filename).Directory);
            textBoxSeriesSearch.Text = Properties.Settings.Default.SeriesForm_Search;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SetLabelText($"starting search (DB): {checkBoxDB.Checked}");
                Application.DoEvents();
                if (checkBoxDB.Checked)
                {
                    SearchForSeries();
                    try
                    {
                        panelDetails.Controls.Clear();
                    }
                    catch (Exception)
                    { }
                }
                else
                    ShowMoviesFromDB(form.SourcePath);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void ShowMoviesFromDB(DirectoryInfo directoryInfo = null)
        {
            DataSet ds;
            if (directoryInfo != null)
            {
                var list = _LITEDBSeries.FindAllFileItems(directoryInfo);
                if (list != null && list.Count() > 0)
                {
                    List<OMDbApiNet.Model.Item> omdbItems = new List<OMDbApiNet.Model.Item>();
                    foreach (var item in list)
                    {
                        try
                        {
                            omdbItems.Add(_LITEDBSeries.FindItem(item.ImdbId));
                        }
                        catch (Exception) { }
                    }
                    ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(omdbItems.ToArray()));
                    ShowGridView(ds.Tables[0]);
                }
            }
        }

        private void SearchForSeries()
        {
            report = string.Empty;
            List<Season> listOfFileSeasons = null;
            List<Season> listOfSeasons = null;
          
            #region method

            DataTable resultTable = null;

            List<string> common = Kolibri.Common.Utilities.FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = Kolibri.Common.Utilities.FileUtilities.GetFiles(form.SourcePath, searchStr, true);
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
                    string title = Kolibri.Common.Utilities.MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(file.Name));

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

                    SetLabelText($"{title} {seasonnumber} {epNr} - {seriesName}");

                    item = GetItem(title, seriesName);
                    season = GetSeason(item, seriesName, seasonnumber);

                  

                    if (season == null)
                    {
                        if (listOfSeasons == null)
                            listOfSeasons = _seriesCache.Get(seriesName, seasonnumber.ToInt32()).ToList();
                        season = listOfSeasons.Find(e => e != null && e.SeasonNumber.ToInt32().ToString() == seasonnumber.ToString());

                        if (season != null)
                        {
                            _LITEDBSeries.Upsert(season);
                        }
                    }

                    if (season == null)
                        continue;


                 var episodesExists=   CompareNumberOfSeasonEpisodes(title, season, totalSeasonList);


                    SeasonEpisode seasonepisode = null;
                    seasonepisode = season.Episodes.Find(e => e.Episode.Replace("E", string.Empty).Replace("e", string.Empty).ToInt32().ToString() == epNr.ToString());

                    if (seasonepisode == null)
                    {
                        seasonepisode = _LITEDBSeries.FindSeasonEpisode(seriesName, seasonnumber, epNr.ToString());
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
                            _LITEDBSeries.Upsert(season);

                        }
                        catch (Exception ex)
                        {
                            SetLabelText(ex.Message);
                            try
                            {//???????
                                var episode = _OMDB.SeriesEpisode(seriesName, $"{seasonnumber}", $"{epNr}");
                                using (var ms = new MemoryStream())
                                {
                                    IFormatter formatter = new BinaryFormatter();
                                    formatter.Serialize(ms, episode);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    seasonepisode = (SeasonEpisode)formatter.Deserialize(ms);
                                }
                            }
                            catch (Exception) { }
                        }
                    }

                    SetLabelText($"Season found: {season.Title} {season.SeasonNumber}/{season.TotalSeasons} episodes: {season.Episodes.Count}. Totalt i folder(e): {count}/{list.Count()}");
                    count++;
                    int year = DateTime.Now.Year;
                    try
                    {
                        year = seasonepisode.Released.ToDateTime().Year;
                    }
                    catch (Exception ex) { }

                    SetLabelText($" ({count}/{list.Count()}) {title} - {year}");


                    if (seasonepisode != null)
                    {
                        _LITEDBSeries.Upsert(seasonepisode);
                        if (checkBoxFileName.Checked)
                        {
                            _LITEDBSeries.Upsert(item);
                            _LITEDBSeries.Upsert(new LiteDBController.FileItem(item.ImdbId, file.Directory.FullName));
                            if (seasonepisode != null && seasonepisode.ImdbId != null)
                            {
                                _LITEDBSeries.Upsert(seasonepisode);
                                _LITEDBSeries.Upsert(new LiteDBController.FileItem(seasonepisode.ImdbId, file.FullName));
                            }
                        }


                        var tempTable =  DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { seasonepisode });
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
                    SetLabelText($"{message} {ex.Message}");
                    Application.DoEvents();
                    Thread.Sleep(2000);
                }
            }

            var temp = new DataView(resultTable, "", "name ASC, Season ASC, Episode ASC", DataViewRowState.CurrentRows).ToTable();
            resultTable = temp;
            resultTable.TableName = Kolibri.Common.Utilities.ExcelUtilities.LegalSheetName((form.SourcePath.Name));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));
            if (resultTable.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(resultTable);
            }

            ShowGridView(resultTable);

            if (checkBoxExcel.Checked)
            {
                DataSet ds;
                List<string> columns = new List<string>() { "Name", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };


                temp = new DataView(resultTable, "", "name asc, ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
                if (temp.DataSet == null)
                {
                    ds = new DataSet();
                    ds.Tables.Add(temp);
                }
                string filePath = Path.Combine(form.DestinationPath.FullName, resultTable.TableName + ".xlsx");
                try { if (File.Exists(filePath)) File.Delete(filePath); } catch (Exception) { }

                Kolibri.Common.Utilities.ExcelUtilities.GenerateExcel2007(new FileInfo(filePath), temp.DataSet);
                Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(filePath));
            }

            #endregion
            ShowReport();
        }

        private void ShowReport()
        {
            try
            {
                if (
                        report.Contains("Forskjell mellom angitt OMDB/TMDB antall episoder totalt") 
                    ||  report.Contains("No elements found for"))
                {
                    Kolibri.Common.FormUtilities.OutputDialogs.ShowRichTextBoxDialog("Report", report, this.Size);
                }

            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Log if the numer of episodes on TMDB/IMDB matches the number of episodes found in filepath
        /// </summary>
        /// <param name="seriesTitle"></param>
        /// <param name="season"></param>
        /// <param name="listOfFileSeasons"></param>
        /// <returns></returns>
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
                        SetLabelText($"{seriesTitle} ({season.SeasonNumber}) - Forskjell mellom angitt OMDB/TMDB antall episoder totalt {season.Episodes.Count}  for sesongen og antall episoder i fillisten {tempSeason.Episodes.Count}");
                    }
                    else
                    {
                        SetLabelText($"{seriesTitle} ({season.SeasonNumber}) - OK - angitt OMDB/TMDB antall episoder totalt {season.Episodes.Count}  for sesongen og antall episoder i fillisten {tempSeason.Episodes.Count} stemmer overens.");
                        ret = true;
                    }
                }
            }
            catch (Exception ex) { };
            return ret;
        }

        private Season GetSeason(Item item, string seriesName, string seasonnumber)
        {
            Season season = null;
            if (season == null && item != null)
            {
                season = _LITEDBSeries.FindSeason(item.Title, seasonnumber.ToString());
            }
            else if (item == null)
            {
                season = _LITEDBSeries.FindSeason(seriesName, seasonnumber);
            }
            if (season == null)
            {
                if (item != null)
                {
                    season = _OMDB.SeriesByImdbId(item.ImdbId, seasonnumber);
                    if (season == null)
                        season = _OMDB.SeriesBySeason(seriesName, seasonnumber);
                    if (season != null)
                        _LITEDBSeries.Upsert(season);
                }
                else
                {
                    season = _OMDB.SeriesBySeason(seriesName, seasonnumber.ToString());
                }
            }

            return season;
        }

        private Item GetItem(string title, string seriesName)
        {
            Item item = null;
            try { item = _LITEDBSeries.FindItemByTitle(seriesName).FirstOrDefault(); } catch (Exception ex) { } 
            if (item == null)
            {
                List<SearchItem> seriesSearchItemList = _OMDB.GetByTitle(seriesName, OMDbApiNet.OmdbType.Series).ToList();



                var seriesSearchItem = seriesSearchItemList.Find(x=>x.Title.Equals(seriesName));
                if (seriesSearchItem == null)   
                    seriesSearchItem     = seriesSearchItemList.FirstOrDefault();   
                if (seriesSearchItemList.Count > 1)
                    SetLabelText($"Search resultet in {seriesSearchItemList.Count()} items. Choosing {seriesSearchItem}");

                try { item = _LITEDBSeries.FindItem(seriesSearchItem.ImdbId); } catch (Exception ex) { }
                if (item == null)
                {
                    item = _OMDB.GetMovieByIMDBid(seriesSearchItem.ImdbId);
                    if (item != null) { _LITEDBSeries.Upsert(item); }
                    else { SetLabelText($"No Item found for {title}"); }
                }
            }
            return item;
        }

        private List<Season> GetSeasonList(string seriesname, string season) {
            return _seriesCache.Get(seriesname, season.ToInt32());
        }

        /// <summary>
        /// Creates a list of the total amount of seasons available for the given series, inkluding the ones in the datatable (does not look the ones from file up)
        /// </summary>
        /// <param name="dataTable">DataTable on the form of the SeriesEpisode datatable</param>
        /// <returns></returns>
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
                                     Title = Kolibri.Common.Utilities.MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(row["FullName"].ToString())),
                                     TotalSeasons = seasons.Count.ToString()
                                 }).FirstOrDefault();
                        s.Episodes = new List<SeasonEpisode>();
                        SetLabelText($"File - found {s.Title}");
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
                                SetLabelText($"File - episode found {ep.JsonSerializeObject()}");
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
                                    SetLabelText($"File - episode added to cache {ep.JsonSerializeObject()}");
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

        public Item GetItem(FileInfo file, int year, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            Item movie = null;
            var test = _LITEDBSeries.FindByFileName(file);
            if (test != null)
            {
                movie = _LITEDBSeries.FindItem(test.ImdbId);
            }
            //Finn ved hjelp av TMDB
            if (movie == null)
            {
                try
                {
                    var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                    List<SearchMovie> tLibList = t.Result;
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
                                    if (!_LITEDBSeries.Insert(movie) && checkBoxFileName.Checked)
                                    {
                                        try
                                        {
                                            if (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl))
                                            {
                                                movie.TomatoUrl = file.FullName;
                                                _LITEDBSeries.Update(movie);
                                                _LITEDBSeries.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
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
                                movie = _TMDB.GetMovie(tLibList[0] );
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
                movie = _LITEDBSeries.FindItemByTitle(title, year);
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
                                        _LITEDBSeries.Insert(movie);
                                        _LITEDBSeries.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
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
                    if (!_LITEDBSeries.Insert(movie) && checkBoxFileName.Checked)
                    {
                        _LITEDBSeries.Update(movie);
                    };
                    _LITEDBSeries.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
                    //_seriesCache.Add(movie);
                    return movie;
                }
                else
                {
                    movie = new OMDbApiNet.Model.Item() { Title = title, Year = year.ToString(), ImdbRating = "Unknown", Response = "false", TomatoUrl = file.FullName };

                }
            }
            if (checkBoxFileName.Checked)
            {
                if (checkBoxFileName.Checked && movie != null && (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl)))
                {
                    try
                    {
                        movie.TomatoUrl = file.FullName;
                        _LITEDBSeries.Update(movie);
                        _LITEDBSeries.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
                    }
                    catch (Exception)
                    { }
                }
                //else if(checkBoxFileName.Checked && file.Exists)
                //{
                //    var dbFile = _LITEDB.FindByFileName(file);
                //    if (dbFile == null)
                //    {
                //        LiteDBController.FileItem item = new LiteDBController.FileItem(movie.ImdbId, file.FullName);
                //        _LITEDB.Upsert(item);
                //    }
                //}
            }
            return movie;
        }

        private void ShowGridView(DataTable table)
        {
            try
            {
                //    List<string> visibleColumns = new List<string>(){"Name", "Title","Episode"  ,"ImdbRating","Released", "ImdbId"    ,"Season"          
                //,"Rated" ,"Genre" ,"Year"};
                //    for (int i = 0; i < visibleColumns.Count; i++)
                //    {
                //        if (table.Columns.Contains(visibleColumns[i]))
                //        {
                //            table.Columns[visibleColumns[i]].SetOrdinal(table.Columns.Count-1);
                //        }

                //    }


                splitContainer2.Panel2.Controls.Clear();
                try
                {

                    DataGridView dgv = new DataGridView();
                    dgv.DataSource = table;
                    dgv.Dock = DockStyle.Fill;
                    splitContainer2.Panel2.Controls.Add(dgv);

                    //          dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => col.Visible = false);
                    // dataGridView1.Rows.OfType<DataGridViewRow>().ToList().ForEach(row => { if (!row.IsNewRow) row.Visible = false; });
                    //              dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => { if (visibleColumns.Contains(col.Name)) col.Visible = true; });
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgv.AllowUserToResizeColumns = true;
                    dgv.SelectionChanged += new EventHandler(DataGridView_SelectionChanged);
                    dgv.CellContentDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellContentDoubleClick);
                    dgv.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(DataGridView_RowPrePaint);
                    dgv.KeyDown += Dgv_KeyDown;
                    dgv.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dgv_CellMouseDown);

                    try
                    {
                        //       dgv.Columns["Title"].DisplayIndex = 0; // or 1, 2, 3 etc dersom dgv ikke er added to panel 2 funker ikke dette 
                        dgv.Columns["Title"].Width = 150;
                        //     dgv.Columns["Name"].DisplayIndex = 0;
                        //   dgv.Columns["ImdbRating"].DisplayIndex = 1; // or 1, 2, 3 etc

                        //dgv.Sort(dgv.Columns["ImdbRating"], ListSortDirection.Descending);
                        DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                        lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    catch (Exception) { }
                }
                catch (Exception)
                { }
            }
            catch (Exception ex)
            { }
        }

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {

                if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
                {

                    DataGridViewRow row = (sender as DataGridView).Rows[e.RowIndex];
                    DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                    string objValue = row.Cells["ImdbId"].Value.ToString();
                    if (c.OwningColumn.Name.Equals("ImdbId", StringComparison.OrdinalIgnoreCase))
                    {
                        string se = $"(S{(row.Cells["Season"].Value).ToString().PadLeft(2, '0')}E{row.Cells["Episode"].Value.ToString().PadLeft(2, '0')}) {row.Cells["Name"].Value}";
                        if (Kolibri.Common.FormUtilities.InputDialogs.InputBox("Sett inn ImdbId " + se, "Legg til ImdbId", ref objValue) == DialogResult.OK)
                        {
                            row.Cells["ImdbId"].Value = objValue;
                            UpdateSeason(
                                row.Cells["Name"].Value,
                                       row.Cells["Season"].Value,
                                              row.Cells["Episode"].Value,
                                                     row.Cells["ImdbId"].Value

                                );

                            return;
                        }
                    }

                    if (c.OwningColumn.Name.Equals("ImdbRating", StringComparison.OrdinalIgnoreCase)) {
                          objValue = row.Cells["ImdbRating"].Value.ToString().Replace("N/A", string.Empty); ;
                        string se = $"{row.Cells["Name"].Value} (S{(row.Cells["Season"].Value).ToString().PadLeft(2, '0')}E{row.Cells["Episode"].Value.ToString().PadLeft(2, '0')})";
                        if (Kolibri.Common.FormUtilities.InputDialogs.InputBox($"Sett inn Imdb rating {se}", "Legg til ImdbRating", ref objValue) == DialogResult.OK)
                        {
                            row.Cells["ImdbRating"].Value = objValue;
                            UpdateSeason(
                                row.Cells["Name"].Value,
                                       row.Cells["Season"].Value,
                                              row.Cells["Episode"].Value,
                                                     row.Cells["ImdbId"].Value,
                                                      row.Cells["ImdbRating"].Value

                                );

                            return;
                        }
                    }

                        OMDbApiNet.Model.Item nm = null;
                    if (c.Selected)
                    {
                        var title = $"{(row.DataBoundItem as DataRowView)["Name"]}";//dgv.CurrentCell.Value;  

                        if (Kolibri.Common.FormUtilities.InputDialogs.InputBox("Seriesøk", "Søk etter serie", ref title) == DialogResult.OK)
                        {
                            var liste = _OMDB.GetByTitle(title, OMDbApiNet.OmdbType.Series, 2);

                            if (liste != null && liste.Count > 1)
                            { }


                            if (liste != null && liste.Count == 1)
                            {
                                nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));
                            }
                            else
                            {
                                var t = Task.Run(() => _TMDB.FetchSerie(title));//, Convert.ToInt32(year)));
                                List<SearchTv> tLibList = t.Result;

                                if (tLibList != null && tLibList.Count > 1)
                                {
                                    var single = tLibList.Where(x => x.Name.Equals(title)).FirstOrDefault();
                                    if (single != null)
                                    {
                                        tLibList.Clear();
                                        tLibList.Add(single);
                                    }
                                }
                                if (tLibList != null && tLibList.Count == 1)
                                {
                                    try
                                    {
                                        var task = Task.Run(() => _TMDB.GetSeason(tLibList[0].Id, $"{(row.DataBoundItem as DataRowView)["Season"]}".ToInt32()));
                                        var season = task.Result;

                                        if (season != null && season.Episodes.Count() > 0)
                                        {
                                            try
                                            {
                                                object tmdbId = string.Empty;
                                                DataSet ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererDataSet(new ArrayList(season.Episodes));
                                                if (Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Choose corect episode (TMDB)", $"Set the correct value for {title}",
                                                    new DataView(ds.Tables[0].Copy(), $"", "", DataViewRowState.CurrentRows).ToTable(true, "Name", "SeasonNumber", "EpisodeNumber", "AirDate", "Overview")
                                                    , ref tmdbId) == DialogResult.OK)

                                                {
                                                    string serTit = (tmdbId as ListViewItem).SubItems[0].Text;
                                                    int sNr = (tmdbId as ListViewItem).SubItems[1].Text.ToInt32();
                                                    int epNr = (tmdbId as ListViewItem).SubItems[2].Text.ToInt32();
                                                    SeasonEpisode ep = _seriesCache.Get(Name, sNr, epNr);
                                                    if (ep == null)
                                                    {
                                                        ep = new SeasonEpisode()
                                                        {
                                                            Episode = epNr.ToString(),
                                                            Title = serTit,
                                                            Released = (tmdbId as ListViewItem).SubItems[3].Text.ToDateTime().ToString("yyyy-MM-dd")
                                                        };

                                                        Season insertSeason = _LITEDBSeries.FindSeason(title, sNr.ToString());
                                                        if (insertSeason == null)
                                                        {
                                                            insertSeason = new Season() { Title = title };
                                                            insertSeason.SeasonNumber = sNr.ToString();
                                                            insertSeason.Episodes = new List<SeasonEpisode> { ep };
                                                            _LITEDBSeries.Insert(insertSeason);
                                                        }
                                                        else {
                                                            var  te = insertSeason.Episodes.Find(ie => ie.Episode.Equals(epNr));
                                                            if (te == null)
                                                            {
                                                                te = ep;
                                                                insertSeason.Episodes.Add(te);                                                            }

                                                            _LITEDBSeries.Update(insertSeason);
                                                        }
                                                        


                                                    }
                                                    else
                                                        objValue = ep.ImdbId;

                                                    row.Cells["Title"].Value = ep.Title;
                                                    row.Cells["Released"].Value = ep.Released;
                                                    if (string.IsNullOrWhiteSpace(objValue))
                                                    {
                                                        if (Kolibri.Common.FormUtilities.InputDialogs.InputBox("Mangler  inn ImdbId", "Legg til ImdbId", ref objValue) == DialogResult.OK)
                                                        {
                                                            ep.ImdbId = objValue;
                                                        }
                                                        else
                                                        {
                                                            var searchURL = Kolibri.Common.Utilities.HTMLUtilities.GoogleSearchString($"IMDB {title} {ep.Title}");
                                                            //var resHTTP = Kolibri.Utilities.HTMLUtilities.GetHTML(searchURL);
                                                            //var httpTable = DataSetUtilities.HTMLTablesToDataSet(resHTTP);
                                                            Process.Start(searchURL.AbsoluteUri);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ep.ImdbId = objValue;
                                                    }

                                                    if (!string.IsNullOrEmpty(ep.ImdbId))
                                                    {
                                                        row.Cells["ImdbId"].Value = ep.ImdbId;
                                                        if (_LITEDBSeries.Upsert(ep))
                                                        {

                                                            Season s = _LITEDBSeries.FindSeason(title, sNr.ToString());
                                                            if (s != null)
                                                            {
                                                                SeasonEpisode se = s.Episodes.Find(ex => ex.Episode.Equals(epNr));
                                                                if (se == null)
                                                                {
                                                                    se = ep;
                                                                    s.Episodes.Add(se);
                                                                }
                                                               
                                                                _LITEDBSeries.Update(s);
                                                            }
                                                        }
                                                        _seriesCache.Add(title, sNr, ep);
                                                    }



                                                    //          var serEp = Task.Run(() => _TMDB.GetEpisode(season.Id.GetValueOrDefault(), season.SeasonNumber, epNr)); ;
                                                    //var serEptask = serEp.Result;
                                                    //// DataTable table = new DataView(ds.Tables[0].Copy(), $"Id='{(ttid as ListViewItem).SubItems[0].Text}'", "", DataViewRowState.CurrentRows).ToTable(true, "Id"," Title"," VoteAverage","  OriginalTitle"," ReleaseDate"," OriginalLanguange"," Overview");
                                                    ////if (serEp != null && !string.IsNullOrEmpty(serEp.ImdbId))
                                                    ////    nm = _OMDB.GetMovieByIMDBid(serEp.ImdbId);
                                                    ////liste = new List<OMDbApiNet.Model.SearchItem>();   
                                                    ///
                                                    /// 
                                                    /// _LITEDBSeries.Upsert(ep);
                                                }
                                            }
                                            catch (Exception ex)
                                            { }

                                        }


                                    }
                                    catch (Exception ex)
                                    { }



                                    //        Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                                    //        nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                    //    }
                                    //    else if (nm == null && liste != null)
                                    //    {
                                    //        object ttid = string.Empty;
                                    //        DataSet ds = Kolibri.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));
                                    //        if (Kolibri.FormUtilities.InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                                    //        {
                                    //            DataTable table = new DataView(ds.Tables[0].Copy(), $"ImdbId='{(ttid as ListViewItem).SubItems[2].Text}'", "", DataViewRowState.CurrentRows).ToTable();
                                    //            nm = _OMDB.GetMovieByIMDBTitle(table.Rows[0]["Title"].ToString(), Convert.ToInt32(table.Rows[0]["Year"]));
                                    //        }
                                    //    }
                                    //}
                                    //if (nm == null)
                                    //{

                                    //    var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                                    //    List<SearchMovie> tLibList = t.Result;
                                    //    if (tLibList != null && tLibList.Count() > 0)
                                    //    {
                                    //        try
                                    //        {
                                    //            object tmdbId = string.Empty;
                                    //            DataSet ds = Kolibri.Utilities.DataSetUtilities.AutoGenererDataSet(new ArrayList(tLibList));
                                    //            if (Kolibri.FormUtilities.InputDialogs.ChooseListBox("Choose corect Movie (TMDB)", "Set the correct value",
                                    //                new DataView(ds.Tables[0].Copy(), $"", "", DataViewRowState.CurrentRows).ToTable(true, "Id", "Title", "VoteAverage", "OriginalTitle", "ReleaseDate", "OriginalLanguage", "Overview")
                                    //                , ref tmdbId) == DialogResult.OK)
                                    //            {

                                    //                Movie tmdbMovie = _TMDB.GetMovie(Convert.ToInt32((tmdbId as ListViewItem).SubItems[0].Text));
                                    //                // DataTable table = new DataView(ds.Tables[0].Copy(), $"Id='{(ttid as ListViewItem).SubItems[0].Text}'", "", DataViewRowState.CurrentRows).ToTable(true, "Id"," Title"," VoteAverage","  OriginalTitle"," ReleaseDate"," OriginalLanguange"," Overview");
                                    //                if (tmdbMovie != null && !string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                    //                    nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                    //                liste = new List<OMDbApiNet.Model.SearchItem>();
                                    //            }
                                    //        }
                                    //        catch (Exception ex)
                                    //        {

                                    //        }

                                    //    }

                                    //    if (nm == null)
                                    //    {
                                    //        if (Kolibri.FormUtilities.InputDialogs.Generic2ValuesDialog("Not found. Put exact year", "", ref title, ref year, "Tittel", "Utgivelsesår") == DialogResult.OK)
                                    //        {
                                    //            liste = _OMDB.GetByTitle(title, Convert.ToInt32(year), OMDbApiNet.OmdbType.Movie, 2);
                                    //            if (liste != null)
                                    //            {
                                    //                object ttid = string.Empty;
                                    //                DataSet ds = Kolibri.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));


                                    //                if (Kolibri.FormUtilities.InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                                    //                {
                                    //                }
                                    //            }
                                    //            if (liste == null)
                                    //            {
                                    //                nm = _OMDB.GetMovieByIMDBTitle(title.ToString(), Convert.ToInt32(year));
                                    //            }

                                    //            else
                                    //            {
                                    //                nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));

                                    //            }

                                    //            liste = new List<OMDbApiNet.Model.SearchItem>();
                                    //        }
                                    //        if (liste == null) liste = new List<OMDbApiNet.Model.SearchItem>();
                                    //    }
                                    //}
                                    //if (nm == null)
                                    //{
                                    //    var test = _TMDB.FetchMovie(title, Convert.ToInt32(year));

                                    //    if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    //    {




                                    //        ProcessStartInfo startInfo = new ProcessStartInfo();
                                    //        startInfo.FileName = "chrome.exe"; // @"""C:\Program Files (x86)\Google\Chrome\Application\chrome.exe""";
                                    //        startInfo.Arguments = $@"https://www.imdb.com/find?q={title.Replace(" ", "+")}&ref_=nv_sr_sm";
                                    //        Process.Start(startInfo);


                                    //    }
                                    //}
                                    //else if (nm != null)
                                    //{

                                    //    if (liste == null)
                                    //        liste = new List<OMDbApiNet.Model.SearchItem>();

                                    //    {
                                    //        DialogResult res =
                                    //                         MessageBox.Show($"{liste.Count} move(s) were found:\r\n\r\nTitle: {nm.Title}\r\nImdbRating: {nm.ImdbRating}\r\nYear: {nm.Year}\r\nActors: {nm.Actors}\r\nPlot :{nm.Plot}", "Is this movie correct?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    //        if (res == DialogResult.Yes)
                                    //        {
                                    //            (row.DataBoundItem as DataRowView)["Title"] = nm.Title;
                                    //            (row.DataBoundItem as DataRowView)["Year"] = Convert.ToInt32(nm.Year);
                                    //            (row.DataBoundItem as DataRowView)["ImdbId"] = nm.ImdbId;
                                    //            (row.DataBoundItem as DataRowView).Row["ImdbId"] = nm.ImdbId;

                                    //            (row.DataBoundItem as DataRowView).Row["ImdbRating"] = nm.ImdbRating;
                                    //            (row.DataBoundItem as DataRowView).Row["Genre"] = nm.Genre;
                                    //            (row.DataBoundItem as DataRowView).Row["Plot"] = nm.Plot;
                                    //            (row.DataBoundItem as DataRowView).Row["Runtime"] = nm.Runtime;
                                    //            (row.DataBoundItem as DataRowView).Row["Rated"] = nm.Rated;

                                    //            row.DataGridView.EndEdit();
                                    //            row.DataGridView.Refresh();

                                    //            _LITEDBSeries.Insert(nm);
                                    //            _LITEDBSeries.Upsert(new LiteDBController.FileItem(nm.ImdbId, $"{ (row.DataBoundItem as DataRowView)["TomatoUrl"]}"));
                                    //            Form form = new DetailsFormMovie(nm, _LITEDBSeries);
                                    //            form.ShowDialog();
                                    //        }
                                    //        else if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                    //        {
                                    //            System.Diagnostics.Process.Start("http://imdb.com");
                                }
                            }
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void UpdateSeason(object seriesName, object seasonNumber, object episodeNumber, object imdbId , object imdbRating=null)
        {
            try
            {
                var season = _LITEDBSeries.FindSeason(seriesName.ToString(), seasonNumber.ToString());
                var ep = season.Episodes.Find(x => x.Episode == episodeNumber.ToString());
                if (ep == null && imdbId != null)
                {
                    var temp = _seriesCache.Get(seriesName.ToString(), seasonNumber.ToString().ToInt32()).FirstOrDefault();
                    var tempEp = temp.Episodes.Find(x => x.Episode == episodeNumber.ToString());
                    if (tempEp != null)
                    {
                        ep = tempEp;
                        season.Episodes.Add(ep);
                        ep = season.Episodes.Find(x => x.Episode == episodeNumber.ToString());
                    }

                }
           
                ep.ImdbId = imdbId.ToString();
                if (imdbRating != null)
                    ep.ImdbRating = imdbRating.ToString().Replace(",", ".");
 
                _LITEDBSeries.Upsert(season);
                _LITEDBSeries.Upsert(ep);
            }
            catch (Exception ex)
            { 
            }
        }

        private void Dgv_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                try
                {
                    DataGridView dgv = sender as DataGridView;
                    var test = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView)[0];//dgv.CurrentCell.Value;  


                    if (MessageBox.Show($"Delete move\r\r {test}, \r\n which will include parent folder and all its content?", "Delete from both file and database?",
                                     System.Windows.Forms.MessageBoxButtons.YesNoCancel,
                                     System.Windows.Forms.MessageBoxIcon.Question,
                                     System.Windows.Forms.MessageBoxDefaultButton.Button3,
                                    MessageBoxOptions.DefaultDesktopOnly) == DialogResult.Yes)
                    {
                        DataRow row = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView).Row;
                        string id = $"{row["ImdbId"]}";
                        if (!string.IsNullOrEmpty(id))
                        {
                            var fileitem = _LITEDBSeries.FindFile(id);
                            if (fileitem != null)
                            {
                                _LITEDBSeries.DeleteItem(id);

                                FileInfo info = new FileInfo(fileitem.FullName);
                                if (info.Exists)
                                {
                                    try
                                    {
                                        Kolibri.Common.Utilities.FolderUtilities.DeleteDirectory(info.FullName);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, ex.GetType().Name);
                                    }
                                    info.Refresh();
                                    if (info.Exists)
                                    {
                                        info.Delete();
                                    }
                                }
                            }
                        }
                        else
                        {
                            SetLabelText($"Move {test} was not deleted. Please do a manual deletion from system.");
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }

                }
                catch (Exception ex)
                {
                    e.Handled = true;
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }

            }
        }

        private void DataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals("del")) { }// == Keys.Delete) {

            //Then find which item/ row to delete by getting the SelectedRows property if your
            //    DataGridView is on FullRowSelect or RowHeaderSelect mode, else you can determine the row with something like this:

            //i = SelectedCells[0].RowIndex

        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {

            string title = null; int year = 0; int index = 0;
            string titleColumn = "Title";

            try
            {
                DataGridView DataGridView1 = sender as DataGridView;

                if (DataGridView1.SelectedRows.Count > 0)
                    index = DataGridView1.SelectedRows[0].Index;
                else
                    try
                    { index = DataGridView1.CurrentRow.Index; }
                    catch (Exception) { }

                if (!DataGridView1.Columns.Contains(titleColumn))
                    titleColumn = "Name";

                if (DataGridView1.Rows[index].Cells[titleColumn].Value
                          != null)
                {
                    if (DataGridView1.Rows[index].
                        Cells[titleColumn].Value.ToString().Length != 0)
                    {
                        title = DataGridView1.Rows[index].Cells[titleColumn].Value.ToString();
                        if (DataGridView1.Columns.Contains("Year"))
                            year = int.Parse(DataGridView1.Rows[index].Cells["Year"].Value.ToString());
                        else if (DataGridView1.Columns.Contains("Released"))
                        {
                            DateTime dt = new DateTime();
                            if (DateTime.TryParse(DataGridView1.Rows[index].Cells["Released"].Value.ToString(), out dt))
                                year = dt.Year;
                            else
                            {
                                if (DataGridView1.Columns.Contains("ImdbId"))
                                {
                                    if (string.IsNullOrEmpty(DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString()))
                                    {
                                    
                                    }
                                }
                            }
                        }
                        else if (DataGridView1.Columns.Contains("AirDate"))
                            year = DateTime.Parse(DataGridView1.Rows[index].Cells["AirDate"].Value.ToString()).Year;
                    }
                }

                try
                {
                    string seasonColumn = "Season";
                    if (!DataGridView1.Columns.Contains(seasonColumn))
                        seasonColumn = "SeasonNumber";

                    string episodeColumn = "Episode";
                    if (!DataGridView1.Columns.Contains(episodeColumn))
                        episodeColumn = "EpisodeNumber";

                    SetLabelText($"{title} - {year} (S{(string.Format("{0:D2}", DataGridView1.Rows[index].Cells[seasonColumn].Value.ToString().ToInt32()))}E{ (string.Format("{0:D2}", DataGridView1.Rows[index].Cells[episodeColumn].Value.ToString().ToInt32()))})");

                }
                catch (Exception ex)
                {

                    SetLabelText(ex.Message);
                }

                if (!DataGridView1.Columns.Contains("ImdbId")) {

                    SetLabelText($"Imdbid finnes ikke for {title}. Prøv igjen og oppgi imdbid.");
                    return;
                }

                var imdbId = DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString();

                var name = DataGridView1.Rows[index].Cells["Name"].Value.ToString();

                // Execute some stored procedure for row updates here   
                var item = _LITEDBSeries.FindItem(imdbId);  //_LITEDBSeries.FindItemByTitle(title, year);
                if (item == null)
                {
                    item = _OMDB.GetItemByImdbId(imdbId);
                }

                if (item == null)
                {
                    item = new OMDbApiNet.Model.Item() { Title = title, Year = $"{year}" };
                }

                if (item != null)
                {
                    //var episode = _OMDB.SeriesEpisode(item.ImdbId,
                    //    int.Parse(DataGridView1.Rows[index].Cells["Season"].Value.ToString()),
                    //    int.Parse(DataGridView1.Rows[index].Cells["Episode"].Value.ToString()));
                    //if (episode != null)
                    //    _LITEDBSeries.Upsert(episode);

                    Form form = new DetailsFormSerie(item, _LITEDBSeries, imdbId);
                    SetForm(form);
                }
            }

            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void DataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                string imdbid = $"{ dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";

                if (string.IsNullOrEmpty(imdbid))
                {
                    if (dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Beige)
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
                    }
                }
                else
                {
                    LiteDBController.FileItem info = _LITEDBSeries.FindFile(imdbid);
                    if (info != null)
                    {
                        string ext = Path.GetExtension(info.FullName);
                        FileInfo srtFile = new FileInfo(info.FullName.Replace(ext, ".srt"));
                        if (!srtFile.Exists)
                        {
                            var searchStr = "*.srt|*.sub";
                            var test = Kolibri.Common.Utilities.FileUtilities.GetFiles(srtFile.Directory, searchStr, true);
                            if (test.Length >= 0)
                            {
                                foreach (var item in test)
                                {
                                    var jall = test.Where(f => Path.GetFileNameWithoutExtension(item.FullName).Equals(Path.GetFileNameWithoutExtension(info.FullName)));
                                    if (jall != null && jall.Count() >= 0 && jall.FirstOrDefault() != null)
                                    {
                                        srtFile = jall.FirstOrDefault();
                                        break;
                                    }
                                }
                            }
                        }
                        if (!srtFile.Exists)
                            dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                        else
                            if (srtFile.Length <= 1) { dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red; }
                        else
                        {
                            if (dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.White)
                            {
                                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                            }
                        }

                    }
                }
            }
            catch (Exception)
            { }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;

                string imdbid = $"{ dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";


                if (!string.IsNullOrEmpty(imdbid))
                {
                    LiteDBController.FileItem info = _LITEDBSeries.FindFile(imdbid);
                    if (info != null)
                    {
                        FileInfo file = new FileInfo(info.FullName);
                        Kolibri.Common.Utilities.FileUtilities.OpenFolderMarkFile(file);
                    }
                    else
                    {
                        string url = $"https://www.imdb.com/title/{imdbid}";
                        Process.Start(url);
                    }
                }
                else
                {
                    try
                    {
                        string path = Path.GetDirectoryName(dgv.Rows[e.RowIndex].Cells["TomatoUrl"].Value.ToString());
                        System.Diagnostics.Process.Start(path);
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Process.Start(form.SourcePath.FullName); ;
                    }
                }
            }
            catch (Exception) { }
        }
        private void SetForm(Form form, Panel panel = null)
        {
            Panel pan = panelDetails;
            if (panel != null)
                pan = panel;

            pan.Controls.Clear();
            try
            {
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;

                if (panel != null)
                    form.Dock = DockStyle.Fill;

                pan.Controls.Add(form);
                form.Show();
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);

            }
        }

        private void buttonVis_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkBoxVisualize.Checked)
                    ShowDataBase();
                else
                {
                    // ShowDataBase(form.SourcePath);
                    var liste = _LITEDBSeries.FindAllItems("series").ToList();
                    
                    SetForm(new Kolibri.Common.VisualizeOMDbItem.ShowLocalSeries(textBoxSeriesSearch.Text,liste ), splitContainer2.Panel2);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void ShowDataBase(DirectoryInfo directoryInfo = null)
        {
            IEnumerable<LiteDBController.FileItem> list;

            if (directoryInfo != null)
                list = _LITEDBSeries.FindAllFileItems(directoryInfo);
            else list = _LITEDBSeries.FindAllFileItems();
            if (list.Count() > 0)
            {
                try
                {

                    List<LiteDBController.FileItem> SortedList = list.OrderByDescending(o => o.FullName).ToList();
                    list = SortedList;
                }
                catch (Exception sortEx) { }
            }

            if (true) { 
                DataTable resultTable = null;

                foreach (var fileItem in list)
                {
                    OMDbApiNet.Model.Item movie = _LITEDBSeries.FindItem(fileItem.ImdbId);
                    if (movie != null && movie.Type.Equals("series"))
                    {
                        var tempTable = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { movie });
                        if (resultTable == null) { resultTable = tempTable.Tables[0]; }
                        else { resultTable.Merge(tempTable.Tables[0]); }


                        var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                        resultTable = temp;
                        resultTable.TableName = Kolibri.Common.Utilities.ExcelUtilities.LegalSheetName((form.SourcePath.Name));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));
                        if (resultTable.DataSet == null)
                        {
                            DataSet ds = new DataSet();
                            ds.Tables.Add(resultTable);
                        }
                    }
                }
                ShowGridView(resultTable);
            }
        }

        private void ShowHTML(string html)
        {
            splitContainer2.Panel2.Controls.Clear();
            try
            {
                FileInfo info = new FileInfo(@"c:\temp\html.html");
                if (!info.Directory.Exists) info.Directory.Create();
                Kolibri.Common.Utilities.FileUtilities.WriteStringToFile(html, info.FullName, Encoding.UTF8);

                //CefSharp.WinForms.ChromiumWebBrowser browser = new CefSharp.WinForms.ChromiumWebBrowser(info.FullName)
                //{
                //    Dock = DockStyle.Fill,
                //};
                //splitContainer2.Panel2.Controls.Add(browser);

                System.Diagnostics.Process.Start(info.FullName);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// http://bl.ocks.org/CrandellWS/2e7d918cbae163ca9c1b
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string HTMLTemplate(IEnumerable<LiteDBController.FileItem> items)
        {
            List<Item> SortedList = new List<Item>();


            foreach (var item in items)
            {
                OMDbApiNet.Model.Item movie = _LITEDBSeries.FindItem(item.ImdbId);
                if (movie == null) continue;
                SortedList.Add(movie);
            }

            SortedList = SortedList.OrderByDescending(o => o.ImdbRating).ToList();

            //   string HTML = File.ReadAllText("TMP_OMDBTemplateFile.html");
            string HTML = File.ReadAllText("TMP_OMDBTableFile.html");

            if (SortedList != null && SortedList.Count() > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in SortedList)
                {
                    //string link = GetSimpleItem(item);
                    string link = GetTableData(item);
                    builder.Append(link);
                }

                int index = HTML.LastIndexOf(@"<div class=""container"">") + @"<div class=""container"">".Length + 1;

                HTML = HTML.Insert(index, builder.ToString());
            }
            return HTML;
        }
        private string GetSimpleItem(LiteDBController.FileItem item)
        {
            string link = string.Empty;
            OMDbApiNet.Model.Item movie = _LITEDBSeries.FindItem(item.ImdbId);
            if (movie == null) return string.Empty;

            link = $@"<a href=""FILE://{Path.GetDirectoryName(item.FullName)}"">
         <img alt=""{movie.Title}"" title=""{movie.Title} ({movie.Year})"" Qries=""{movie.Director}"" src=""{movie.Poster}""
         width=100"" height=""30"">";
            return link;
        }
        private string GetTableData(LiteDBController.FileItem item)
        {//  <!-- <td>{link}</td> -->

            OMDbApiNet.Model.Item movie = _LITEDBSeries.FindItem(item.ImdbId);
            if (movie == null) return string.Empty;
            return GetTableData(movie);
        }

        private string GetTableData(Item movie)
        {
            if (movie == null) return string.Empty;
            string link = $@"<a href=""FILE://{Path.GetDirectoryName(movie.TomatoUrl)}"">link</a>";
            string image = $@"<img alt=""{movie.Title}"" title=""{movie.Title} ({movie.Year})"" Qries=""{movie.TomatoUrl}"" src=""{movie.Poster}"" width=""70"" height=""100""> ";
            string data = $@" <tr>
                <td align=""left"" valign=""top"" width=""15%"" >{image}</td>              
                <td>{movie.Title}</td>
                <td align=""left""  width=""3%"">{movie.ImdbRating}</td>
                <td>{movie.Year}</td>
                <td>{movie.Plot}</td>
            <tr>";
            return data;
        }

        private void OMDBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _seriesCache.Save();
            _LITEDBSeries.Dispose();
        }
        private void buttonSubtitleSearch_Click(object sender, EventArgs e)
        {List   <string> urllist= new List<string>();   
            bool okGotIt = false;
            List<string> common = Kolibri.Common.Utilities.FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = Kolibri.Common.Utilities.FileUtilities.GetFiles(form.SourcePath, searchStr, true);

            SetLabelText($"{list.Count()} element found at {form.SourcePath} ");
            report = string.Empty;
            foreach (var item in list)
            {

                string ep = SeriesUtilities.GetEpisodeFromFilename(item.FullName);
                searchStr = $"*{ep}*.srt";
                var srtlist = Kolibri.Common.Utilities.FileUtilities.GetFiles(item.Directory, searchStr, true);
                if (srtlist.Length == 0)
                {
                    SetLabelText($"No elements found for {item.Name} -  {searchStr}"); 
                     
                     
                    FileInfo info = new FileInfo(item.FullName);
                    SetLabelText($"Searching for {info.Name}");

                    FileInfo srtInfo = new FileInfo(Path.ChangeExtension(item.FullName, ".srt"));
                    bool dirExists = Directory.Exists(Path.Combine(info.Directory.FullName, "Subs"));
                   FileItem sItem = _LITEDBSeries.FindAllFileItems(form.SourcePath).FirstOrDefault();

                    var mmi = _OMDB.GetItemByImdbId(sItem.ImdbId);

                    dirExists = dirExists && mmi.Type == "movie";
                    try
                    {
                        if (info.Exists && !dirExists)
                        {
                            if (srtInfo.Exists) { continue; }

                            var jall = Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByIMDBid(sItem.ImdbId);
                            if (jall.status == false) {

                                jall = Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByNameAndType(MovieUtilites.GetMovieTitle(item.Name), mmi.Year);
                            }


                            if (jall.status == false)
                            {
                                var strurl = $"https://subdl.com/search/{MovieUtilites.GetMovieTitle(item.Name)}";
                                if (!urllist.Contains(strurl))
                                {
                                    HTMLUtilities.OpenURLInBrowser(new Uri(strurl));
                                    urllist.Add(strurl);
                                }
                            }



                            if (jall.status == true && jall.subtitles != null && jall.subtitles.Count >= 1)
                            {
                                foreach (var sub in jall.subtitles)
                                {
                                    try
                                    {
                                        string url = $"https://dl.subdl.com{sub.url}";

                                        FileInfo subInfo = new FileInfo(Path.Combine(info.Directory.FullName, "Subs", FileUtilities.SafeFileName($"{sub.language}_{sub.release_name}.zip")));

                                        var exist = Kolibri.Common.Utilities.FileUtilities.DownloadFile(url, subInfo.FullName);

                                        if (!exist) throw new FileNotFoundException(subInfo.FullName);

                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Logg(Logger.LoggType.Feil, ex.Message);
                                    }
                                } 
                            }
                        } 
                    }
                    catch (Exception ex) { 
                        SetLabelText($"No elements found for {item.Name} -  {searchStr}");
                    } 
                }
                else if (srtlist.Length == 1)
                {
                    FileInfo srt = srtlist.FirstOrDefault();
                    String episodeName = Path.GetFileNameWithoutExtension(item.Name);
                    string srtName = Path.GetFileNameWithoutExtension(srt.Name);
                    if (srtName.Equals(episodeName)) { continue; }
                    else
                    {

                        try
                        {
                            FileInfo newNameFullPath = new FileInfo(Path.Combine(item.Directory.FullName, Path.GetFileNameWithoutExtension(item.Name) + srt.Extension));
                            System.IO.File.Move(srt.FullName, newNameFullPath.FullName);
                        }
                        catch (Exception ex)
                        {
                            SetLabelText(ex.Message);
                        }
                    }
                }
                else
                {
                    if (!okGotIt)
                    {
                        var res = MessageBox.Show($"More than one file found for {item.Name}. To stop this message, click 'Yes', to continue nagging about this click 'No', and to cancel the operation click Cancel", $"Found {srtlist.Count()} subtitle items", MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Yes) { okGotIt = true; }
                        else if (res == DialogResult.Cancel)
                        {
                            MessageBox.Show("Exiting the file renaming process."); return;
                        }
                    }

                }

            }
            if (!string.IsNullOrEmpty(report)) { ShowReport(); }
        }
        private void buttonSubtitleSearch_Click_old(object sender, EventArgs e)
        {
            string sd_id = string.Empty;
            var list = _LITEDBSeries.FindAllFileItems(form.SourcePath);
            SetLabelText($"{list.Count()} element found at {form.SourcePath} ");
            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var test = SubDLSubtitleController.SearchByIMDBid("tt7949218");
                    
                    
                    var isFolder = item.FullName.IsFileOrFolder();
                    //   https://subdl.com/subtitle/sd1301754/goliath
                    //"Kan virke som det funker best på mapper og ikke episoder", "Fortsette?", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.Cancel)
                    if ( isFolder.isFolder) { continue; }
                    var ep =  _OMDB.GetItemByImdbId(item.ImdbId) ;
                    var name = MovieUtilites.GetMovieTitle(item.FullName); 
                    var year = MovieUtilites.GetYear(ep.Year);

                    var t = Task.Run(() => _TMDB.FetchSerie(name));//, Convert.ToInt32(year)));
                    List<SearchTv> tLibList = t.Result;
                    var searchTv = tLibList.FirstOrDefault(x => x.Name.Equals(name));
                    var tmdbId = searchTv.Id;

                    var tvt = Task.Run(() =>  _TMDB.GetTVShow(tmdbId));
                    var res = tvt.Result;
                    SetLabelText($"Searching for {ep}");
                    try
                    {
                        SubDL subdlcompare = null;
                        SubDL subdl = null;
                          subdl =      SubDLSubtitleController.SearchByNameAndType(name, year.ToString(), "tv");
                        if (subdl.status == true && subdl.subtitles?.Count <= 0)
                        {
                            subdlcompare =    Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByFileName(Path.GetFileNameWithoutExtension(item.FullName));
                            if (subdlcompare.status == true && subdl.subtitles?.Count <= 0) { subdl = subdlcompare; }
                        }

                        if (subdl != null && subdl.status != false)
                        {
                            subdlcompare = SubDLSubtitleController.SearchBySD_ID(subdl.results.FirstOrDefault().sd_id.ToString());
                            if (subdlcompare.status == true && subdlcompare.subtitles?.Count > subdl.subtitles?.Count) { subdl = subdlcompare; }

                        }
                        if (subdl.status == false)
                        {
                            subdl = Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByFileName(Path.GetFileNameWithoutExtension(item.FullName));
                            if (subdl.status == false)
                            {
                                OMDbApiNet.Model.Item mmi = _LITEDBSeries.FindItem(item.ImdbId);
                                subdl = Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByItemValues(mmi);

                            }

                        }
                        if (subdl.status == true && subdl.subtitles != null && subdl.subtitles.Count >= 1)
                        {
                            foreach (var sub in subdl.subtitles)
                            {
                                try
                                {
                                    string url = $"https://dl.subdl.com{sub.url}";

                                    FileInfo subInfo = new FileInfo(Path.Combine(item.FullName, "Subs", FileUtilities.SafeFileName($"{sub.language}_{sub.release_name}.zip")));

                                    var exist = Kolibri.Common.Utilities.FileUtilities.DownloadFile(url, subInfo.FullName);

                                    if (!exist) throw new FileNotFoundException(subInfo.FullName);

                                }
                                catch (Exception ex)
                                {
                                    Logger.Logg(Logger.LoggType.Feil, ex.Message);
                                    SetLabelText(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            Uri url = new Uri($@"https://subdl.com/subtitle/dd{subdl.results.FirstOrDefault().sd_id}/{subdl.results.FirstOrDefault().name.Replace(" ", "-").ToLower() }");
                            HTMLUtilities.OpenURLInBrowser(url);
                        } 

                    }
                    catch (Exception ex) { SetLabelText(ex.Message); }

                }

            }
            else
            {
                MessageBox.Show("List contained no elements for this path. Try searching for elements and try again", form.SourcePath.FullName);
            }
            #region old search
            //try
            //{
            //  //  string path = @"\\lissinshare\Public\APPS\UTILS\SubSync\SubSync.exe";
            //  string   path = "E:\\RELEASE\\SubSync\\SubSync.exe";

            //    var process = new Process
            //    {
            //        StartInfo =
            //  {
            //      FileName = path,
            //      Arguments = $@"""{form.DestinationPath}"""
            //  }
            //    };
            //    process.Start();


            //}
            //catch (Exception ex)
            //{
            //    SetLabelText(ex.Message);
            //}
            #endregion
        }


        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Remove movieitem with nonexistant filepath?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                foreach (var movie in _LITEDBSeries.FindAllItems())
                {

                    if (!string.IsNullOrEmpty(movie.TomatoUrl))
                    {
                        try
                        {
                            FileInfo info = new FileInfo(movie.TomatoUrl);
                            if (!info.Exists)
                            {
                                _LITEDBSeries.DeleteItem(movie.ImdbId);
                            }
                        }
                        catch (Exception ex)
                        {
                            SetLabelText(ex.Message);
                        }
                    }
                }
            }

        }

        private void checkBoxDB_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as CheckBox).Checked)
                checkBoxExcel.Checked = true;

        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender.Equals(linkLabelLiteDB))
            {
                try
                {
                    FileInfo info = new FileInfo(linkLabelLiteDB.Tag.ToString());

                    Process.Start(info.Directory.FullName);
                }
                catch (Exception) { }
            }
            else
            {
                try
                {
                    if (sender as LinkLabel != null)
                    {
                        var s = sender as LinkLabel;

                        Process.Start($"{ s.Tag}");
                    }
                }
                catch (Exception) { }
            }
        }

        private void buttonSeriesDB_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Let opp LastSeriesDBConnectionString";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(ofd.FileName))
                        {
                            AccessConfig.LastOMDBConnectionString = ofd.FileName; 
                            Init();
                        }
                    }
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void buttonExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (splitContainer2.Panel2.Controls.Count < 1) throw new InvalidDataException("Finner ingen rader med filminformasjon. Let opp data!");

                DataGridView view = splitContainer2.Panel2.Controls[0] as DataGridView;
                if (view == null) throw new InvalidDataException("Finner ingen rader med filminformasjon. Let opp data!");
                DataTable table = view.DataSource as DataTable;
                var cols = Kolibri.Common.Utilities.DataSetUtilities.ColumnNames(table);
                DataSet ds;
                List<string> columns = new List<string>() { "Title", "ImdbRating", "Year", "Country", "Runtime", "Genre", "Plot", "ImdbId" };

                object refereres = null;
                var res = Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Velg kolonner, avbryt  eller velg bare 1 for standard utvalg", "Velg kolonner som excelarket skal bestå av", cols.ToList(), ref refereres, true);
                if (res == DialogResult.OK)
                {
                    ListView.SelectedListViewItemCollection collection = refereres as System.Windows.Forms.ListView.SelectedListViewItemCollection;
                    if (collection != null && collection.Count > 1)
                    {
                        List<string> list = collection.Cast<ListViewItem>()
                                          .Select(item => item.Text)
                                          .ToList();
                        columns = list;
                    }
                }

                DataTable temp = new DataView(table.Copy(), "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
                if (temp.DataSet == null) { ds = new DataSet(); ds.Tables.Add(temp); }

                if (temp.Columns.Contains("ImdbId"))
                {
                    foreach (DataRow row in temp.Rows)
                    {
                        try
                        {
                            row["ImdbId"] = $"https://www.imdb.com/title/{row["ImdbId"]}/";
                        }
                        catch (Exception)
                        { }
                    }
                }

                #region image not working
                if (temp.Columns.Contains("Poster"))
                {
                    temp.Columns.Add("Image", typeof(Bitmap));
                    temp.Columns["Image"].SetOrdinal(0);

                    // ((DataGridViewImageColumn)temp.Columns["Image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

                    foreach (DataRow row in temp.Rows)
                    {
                        try
                        {
                            var url = $"{ row["Poster"]}";
                            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            System.Net.WebResponse webResponse = webRequest.GetResponse();
                            System.IO.Stream stream = webResponse.GetResponseStream();
                            var image = (Bitmap)System.Drawing.Image.FromStream(stream);
                            webResponse.Close();

                            try
                            {

                                if (image as Bitmap != null)
                                {
                                    row["Image"] = image;
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                string filePath = Path.Combine(form.DestinationPath.FullName, table.TableName + ".xlsx");
                try { if (File.Exists(filePath)) File.Delete(filePath); } catch (Exception) { }

                Kolibri.Common.Utilities.ExcelUtilities.GenerateExcel2007(new FileInfo(filePath), temp.DataSet);
                Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(filePath));

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private async void buttonSeriesSearch_Click(object sender, EventArgs e)
        {
            TvSeason season = null;
            DataTable ret = null;
            Properties.Settings.Default.SeriesForm_Search = textBoxSeriesSearch.Text;
            this.Text = $"searching for {Properties.Settings.Default.SeriesForm_Search}";
            List <SearchTv> tvliste = Task.Run(() => _TMDB.FetchSerie(Properties.Settings.Default.SeriesForm_Search)).Result;
            if (tvliste == null || tvliste.Count < 1) {
                SetLabelText($"Fant ingen resultat for {textBoxSeriesSearch.Text}");
                return;
            } 

             object tmdbId = tvliste;
                       
            DataSet ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererDataSet(new ArrayList(tvliste.ToList()));
            ds.Tables[0].TableName = "SeriesSearch";
            foreach ( DataRow row in ds.Tables[0].Rows)
            {
                row["PosterPath"] = _TMDB.GetImageUrl($"{row["PosterPath"]}");
            }
            var colums = DataSetUtilities.ColumnNames(ds.Tables[0]);
            string[] seriesColumns  = new List<string>() { "Name", "FirstAirDate", "Overview", "OriginalName", "Id", "VoteAverage" , "PosterPath"}.ToArray();

            if (Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Choose correct episode (TMDB)", $"Set the correct value for search value{Properties.Settings.Default.SeriesForm_Search} ",
                new DataView(
                    ds.Tables[0].Copy(), $"", "", DataViewRowState.CurrentRows).ToTable(true, seriesColumns)
                , ref tmdbId, false, "PosterPath") == DialogResult.OK)
            {
                int sNr = (tmdbId as ListViewItem).SubItems[4].Text.ToInt32();
                var searchTV = tvliste.Find(x => x.Id == sNr);
                var test = Task.Run(() => _TMDB.GetTVShow(sNr));
                for (int i = 0; i < test.Result.Seasons.Count; i++)
                {
                    try
                    {
                        season = Task.Run(() => _TMDB.GetSeason(searchTV.Id, i + 1)).Result;

                        if (season != null)
                        {
                            var j = season.Episodes;

                            DataTable epTable = new DataView(DataSetUtilities.AutoGenererDataSet(season.Episodes.ToList()).Tables[0]).ToTable(false, "Name", "Id", "AirDate", "SeasonNumber", "EpisodeNumber", "VoteAverage", "Overview");
                            if (ret == null)
                                ret = epTable;
                            else
                                ret.Merge(epTable);
                        }
                    }
                    catch (Exception ex)
                    { SetLabelText($"Feil oppstod ({searchTV.Name}): {ex.Message}"); }
                }
                //lagre søket til neste gang.
                Properties.Settings.Default.Save();

                Form form = Kolibri.Common.FormUtilities.Controller.OutputFormController.DataTableForm($"{Properties.Settings.Default.SeriesForm_Search}", ret, ret.Columns["OverView"], this.Size);
                SetForm(form, splitContainer2.Panel2);
            }
            else
            {
                SetLabelText($"Fant ingen resultat for {textBoxSeriesSearch.Text}");
            }
        }

        private void textBoxSeriesSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSeriesSearch_Click(this, new EventArgs());
            }
        }
    }
}