using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.SilverScreen.Controls;
using Kolibri.net.SilverScreen.IMDBForms;
using OMDbApiNet.Model;
using System.Data;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class ShowLocalMoviesForm : Form
    {
        private List<string> _initializedMissing = new List<string>();
        LiteDBController _liteDB;
        TMDBController _TMDB;
        OMDBController _OMDB;
        ImageCacheDB _imageCache;

        MultimediaType _type;
        private readonly UserSettings _userSettings;
        private IEnumerable<FileItem> _fileItems;
        private List<string> _searchFiles;
        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;

        public ShowLocalMoviesForm(MultimediaType type, UserSettings settings)
        {
            InitializeComponent();
            _type = type;
            this._userSettings = settings;
            this.Text = $"{_type.ToString()}";

            StartUp();
            Init();

        }

        private async void StartUp()
        {
            _searchFiles = new List<string>();
            this.Text = $"{_type.ToString()} - {_userSettings.LiteDBFilePath}";
            GetPathSearchFiles();
            _liteDB = new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false);

            _imageCache = new ImageCacheDB(_userSettings);
            buttonOpenFolder.Image = Icons.GetFolderIcon().ToBitmap();
            try
            {
                if (_type.Equals(MultimediaType.movie) || _type.Equals(MultimediaType.Movies))
                {
                    if (_TMDB == null && !string.IsNullOrEmpty(_userSettings.OMDBkey))
                        _TMDB = new TMDBController(_liteDB, _userSettings.OMDBkey);
                }
            }
            catch (Exception)
            {
                _TMDB = null;
            }
        }

        private async void Init()
        {
            _fileItems = new List<FileItem>();

            SetForm(new Form());
            int diff = 0;
            //List<string> common = MovieUtilites.MoviesCommonFileExt(true);
            //var masks = common.Select(r => string.Concat('*', r)).ToArray();
            //var searchStr = "*" + string.Join("|*", common);

            if (!checkBoxSimple.Checked && _userSettings != null)
            {
                if (string.IsNullOrEmpty(_userSettings.TMDBkey))
                    checkBoxSimple.Checked = true;
            }
            if (_type.Equals(MultimediaType.Series))
            {
                checkBoxSimple.Checked = true;
            }

            var path = GetCurentPath();
            textBoxSource.Text = path;
            SetLabelText($"Current filepaht: {path} - Searching for {_type}");
            var sublist = new List<string>();
            _fileItems = _liteDB.FindAllFileItems(new DirectoryInfo(path)).ToList();
            //filter
            if (!radioButtonFilterAlle.Checked)
            {

                if (radioButtonFilterNoneExistant.Checked)
                {
                    diff = _fileItems.Count();
                    var filtered = _fileItems.Where(x => !x.ItemFileInfo.Exists);
                    if (filtered != null)
                    {
                        diff = filtered.Count();
                        _fileItems = filtered;
                    }
                }
                else if (radioButtonFilterNotMatched.Checked)
                {
                    diff = _searchFiles.Count();

                    if (_fileItems.Count() != diff)
                    {
                        diff = _searchFiles.Count - _fileItems.Count();


                        int teller = 0;
                        foreach (var srch in _searchFiles)
                        {
                            teller++;
                            if (sublist.Contains(srch)) continue;

                            var has = _fileItems.ToList().Find(cus => cus.FullName.Equals(srch));
                            if (has == null)
                            {

                                var filter = Kolibri.net.Common.Utilities.MovieUtilites.MulitpartFilter();
                                var matches = filter.Where(x => srch.ToLower().Contains(x.ToLower()));
                                if (matches != null && matches.Count() > 0)
                                {
                                    var masks = MovieUtilites.MoviesCommonFileExt(true).Select(r => string.Concat('*', r)).ToArray();
                                    if (!masks.Contains("@__thumb"))
                                    {
                                        sublist.Add($"[MULTIPART] {Path.GetFileName(srch)}: {srch}");
                                    }
                                }
                                else
                                {
                                    //sublist.Add(srch);
                                    var test = _liteDB.FindByFileName(new FileInfo(srch));
                                    if (test == null)
                                    {
                                        sublist.Add($"[NULL] {MovieUtilites.GetMovieTitle(srch)} -  The file is not found by LiteDB search for FilteItems using: {srch}");
                                        if (!_initializedMissing.Contains(srch) && diff < 25)
                                        {
                                            try
                                            {
                                                _initializedMissing.Add(srch);
                                            }
                                            catch (Exception ex)
                                            { }
                                        }
                                        else
                                        {
                                            SetLabelText($"Too many files {teller}/{_searchFiles.Count()} - select a smaller collection. Or wait...");
                                        }
                                    }
                                }
                            }
                        }
                        SetLabelText($"Num files in dir: {_searchFiles.Count} - ({_fileItems.Count()} - {teller})");

                    }
                }
                else
                {
                    _fileItems = new List<FileItem>();
                }
            }
            else { diff = _searchFiles.Count - _fileItems.Count(); }


            if (_dgvController == null) _dgvController = new DataGrivViewControls(_type, _liteDB);
            int count = 0;

            switch (_type)
            {
                case MultimediaType.movie:
                case MultimediaType.Movies:

                    var list = new List<Item>();
                    list = GetItems(_fileItems);
                    count = list.Count;
                    ShowGridForDBItems(list);



                    break;
                case MultimediaType.Series:

                    var episodes = new List<SeasonEpisode>();
                    episodes = GetEpisodes(_fileItems);
                    count = episodes.Count;
                    ShowGridForDBEpisodes(episodes);
                    break;
                case MultimediaType.Audio:
                    break;
                case MultimediaType.Pictures:
                    break;
                default:
                    break;
            }
            string text = $"{count} found in LiteDB (diff: {diff} - folder: {_searchFiles.Count})";
            if (sublist.Count > 1)
            {
                var form = Kolibri.net.Common.FormUtilities.Controller.OutputFormController.RichTextBoxForm(
                    $"{(diff - _fileItems.Count())} - {radioButtonFilterNotMatched.Text}",
                    string.Join(Environment.NewLine, sublist.ToArray()), this.Size);
                SetForm(form, splitContainer2.Panel2);

            }
            buttonMissing.Enabled = sublist.Count() >= 1;
            //if (!radioButtonFilterAlle.Checked)  text = $"{count} / diff: {diff} files";
            labelNumItemsDB.Text = text;
            SetLabelText(labelNumItemsDB.Text);
        }


        /// <summary>
        /// henter filer fra inneværende sti
        /// </summary>
        /// <returns></returns>
        private async void GetPathSearchFiles(string dInfo = null)
        {
            List<string> ret = new List<string>();
            if (dInfo == null)
                dInfo = GetCurentPath();
            try
            {
                ret = Directory.EnumerateFiles(dInfo, "*.*", SearchOption.AllDirectories)
                                  .Where(file => MovieUtilites.MoviesCommonFileExt(true).ToArray()
                                   .Contains(Path.GetExtension(file))).ToList();
                //Filter
                ret = ret.Where(cdr => !cdr?.Contains("@__thumb") == true).ToList();
            }
            catch (Exception)
            {
                ret = new List<string>();

            }

            _searchFiles = ret;
        }


        private List<Item> GetItems(IEnumerable<FileItem> searchFiles = null)
        {
            var task = Task.Run<Task<IEnumerable<Item>>>(async () => await _liteDB.FindAllItems(_type.ToString()));
            var ret = task.Result.Result.ToList();
            if (searchFiles != null)
            {
                var sublist = new List<Item>();

                foreach (var srch in searchFiles)
                {
                    var has = ret.Find(cus => cus.ImdbId.Equals(srch.ImdbId));
                    if (has != null) sublist.Add(has);
                }
                ret = sublist;
            }
            return ret;
        }
        private List<SeasonEpisode> GetEpisodes(IEnumerable<FileItem> files = null)
        {
            var ret = _liteDB.FindAllSeasonEpisodes();
            if (files != null && files.Count() >= 1)
            {
                var sublist = new List<SeasonEpisode>();

                foreach (var item in files)
                {
                    var has = ret.Find(cus => cus.ImdbId.Equals(item.ImdbId));
                    if (has != null) sublist.Add(has);
                }
                ret = sublist;
                //ret = ret.Where(x=> x.ImdbId.Equals( files.ToList().Find(y=>y.ImdbId.Equals(x.ImdbId)
                //                 || x.ImdbId.Equals(files.ToList().Find(t=>t.ImdbId.Equals(x.Title, StringComparison.OrdinalIgnoreCase)))
            }

            return ret;
        }


        private void ShowGridForDBEpisodes(List<SeasonEpisode> list = null)
        {
            string path = GetCurentPath();
            DataTable resultTable = null;
            try
            {
                if (_fileItems != null && _fileItems.Count() > 0)
                {
                    SetLabelText($"Searching for {_type}.....");
                    var lookup = _fileItems.Distinct().ToDictionary(x => x.ImdbId);

                    if (list != null && list.Count > 0)
                    {
                        var searchList = list;
                        resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];
                    }
                    else
                    {
                        var searchList = lookup.Where(t => ((t.Key != null)) && lookup.ContainsKey(t.Key));
                        resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];
                    }
                    var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                    resultTable = temp;
                    resultTable.TableName = DataSetUtilities.LegalTableName(System.IO.Path.GetFileNameWithoutExtension(path));
                    if (resultTable.DataSet == null)
                    {
                        DataSet ds = new DataSet();
                        ds.Tables.Add(resultTable);
                    }

                    ShowGridView(resultTable);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void ShowGridForDBItems(List<Item> list = null)
        {
            string path = GetCurentPath();
            DataTable resultTable = new DataTable();
            try
            {
                if (list != null && list.Count > 0)
                {
                    SetLabelText($"Searching for {_type}.....");
                    var lookup = _fileItems.Distinct().ToDictionary(x => x.ImdbId);

                    if (list != null)
                    {
                        var searchList = list;
                        resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];
                        var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                        resultTable = temp;
                        resultTable.TableName = DataSetUtilities.LegalTableName(System.IO.Path.GetFileNameWithoutExtension(path));
                        if (_fileItems.Count() > list.Count()) SetLabelText($"NB - det er bare funnet {list.Count()} av {_fileItems.Count()}. ");
                    }
                    else
                    {
                        var searchList = lookup.Where(t => ((t.Key != null)) && lookup.ContainsKey(t.Key));
                        resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];
                    }

                    if (resultTable.DataSet == null)
                    {
                        DataSet ds = new DataSet();
                        ds.Tables.Add(resultTable);
                    }

                    ShowGridView(resultTable);
                }
                else
                {
                    ShowGridView(resultTable);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private string GetCurentPath()
        {
            string ret = string.Empty;
            switch (_type)
            {
                case MultimediaType.Movies: ret = _userSettings.UserFilePaths.MoviesSourcePath; break;
                case MultimediaType.Series: ret = _userSettings.UserFilePaths.SeriesSourcePath; break;
                case MultimediaType.Audio:
                    break;
                case MultimediaType.Pictures:
                    break;
                default:
                    throw new NotImplementedException(_type.ToString());

                    break;
            }
            if (!Directory.Exists(ret))
            {
                ret = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            return ret.Trim();
        }
        private async void SetCurrentPath(DirectoryInfo dInfo, bool init = true)
        {
            if ((dInfo.FullName != _userSettings.UserFilePaths.MoviesSourcePath) || _searchFiles.Count <= 0) { GetPathSearchFiles(dInfo.FullName); }
            switch (_type)
            {
                case MultimediaType.Movies:
                    _userSettings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
                    break;
                case MultimediaType.Series:
                    _userSettings.UserFilePaths.SeriesSourcePath = dInfo.FullName;

                    break;
                case MultimediaType.Audio:
                    throw new NotImplementedException(_type.ToString());
                    break;
                case MultimediaType.Pictures:
                    throw new NotImplementedException(_type.ToString());
                    break;
                default:
                    throw new NotImplementedException(_type.ToString());
                    return;
                    break;
            }

            if (init)
            {
                _liteDB.Upsert(_userSettings);
                textBoxSource.Text = dInfo.FullName;
                SetLabelText($"{_type} - set to {dInfo.FullName}");

                bool? tristate = null;
                if (radioButtonUpdateNew.Checked)
                    tristate = false;
                if (radioButtonUpdateAll.Checked)
                    tristate = true;

               
                if (_type.Equals(Constants.MultimediaType.movie) || _type.Equals(Constants.MultimediaType.Movies))
                { MoviesSearchController searchController = new MoviesSearchController(_userSettings, updateTriState: tristate);
                    Task.Run(async () => searchController.SearchForMovies(dInfo));
                }
                else if (_type.Equals(Constants.MultimediaType.Series))
                {
                    KolibriTVShowSearchController kTVsearch = new KolibriTVShowSearchController(_userSettings, _liteDB);
                    Task.Run(async () => kTVsearch.SearchForSeriesEpisodes(dInfo));
                }
                Init();
            }
        }
        private void ShowGridView(DataTable tableItem)
        {
            try
            {
                Form view = _dgvController.GetMulitMediaDBDataGridViewAsForm(tableItem);
                (view.Controls[0] as DataGridView).SelectionChanged += DataGridView_LocalSelectionChanged;
                SetForm(view, splitContainer2.Panel1);

                SetLabelText($"{tableItem.Rows.Count} rader.");

                switch (_type)
                {
                    case MultimediaType.movie:
                    case MultimediaType.Movies:

                        var movie = _liteDB.FindItem(tableItem.Rows[0]["ImdbId"].ToString());
                        SetForm(movie, splitContainer2.Panel2);
                        break;
                    case MultimediaType.Series:

                        var series = _liteDB.FindSeasonEpisode(tableItem.Rows[0]["ImdbId"].ToString());
                        //     SetForm(new DetailsFormSeries(series, _liteDB, _OMDB, _TMDB, null, _imageCache), splitContainer2.Panel2);
                        throw new NotImplementedException();
                        break;
                    case MultimediaType.Audio:
                        break;
                    case MultimediaType.Pictures:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void Nullstill()
        {
            try
            {
                Form form = new Form();
                SetForm(form, splitContainer2.Panel1);
                SetForm(form, splitContainer2.Panel1);

            }
            catch (Exception)
            {
            }

        }

        private void DataGridView_LocalSelectionChanged(object sender, EventArgs e)
        {
            try
            { 
                SetForm(_dgvController.CurrentItem, splitContainer2.Panel2);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void buttonOpenFolder_Click(object sender, EventArgs e)
        {
            _searchFiles = new List<string>();
            var dInfo = FileUtilities.LetOppMappe(GetCurentPath(), $"Let opp mappe ({_type})");
            if (dInfo != null && dInfo.Exists)
            {
                SetCurrentPath(dInfo);
            }
        }
        private void SetForm(Object mm, SplitterPanel setPanel)
        {
            Form form;
            SplitterPanel panel = setPanel;

            if (_type.Equals(MultimediaType.Series))
            {
                // form = new DetailsFormSeries(mm as SeasonEpisode, _liteDB, _OMDB, _TMDB, null, _imageCache);
                throw new NotImplementedException();
            }
            else
            {
                if (checkBoxSimple.Checked) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(mm as Item, _liteDB, tmdb: _TMDB, imagecache: _imageCache); }
                else { form = new MovieForm(_userSettings, mm as Item); }
            }
            SetForm(form, panel);
        }

        private void SetForm(Form form, SplitterPanel setPanel = null)
        {
            try
            {
                SplitterPanel panel = setPanel;
                if (panel == null) panel = splitContainer2.Panel2;
                if (panel == splitContainer2.Panel1)
                {
                    foreach (Control item in panel.Controls)
                    {
                        if (!item.Equals(statusStrip1))
                        {
                            panel.Controls.Remove(item);
                        }
                    }

                }
                else
                {
                    panel.Controls.Clear();
                }

                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;

                panel.Controls.Add(form);
                form.Show();
                SetLabelText(form.Text);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }
        private void SetLabelText(string text)
        {
            try
            {
                toolStripStatusLabelStatus.GetCurrentParent().BringToFront();
                toolStripStatusLabelStatus.Text = text;
            }
            catch (Exception ex) { }
        }

        private void radioButtonFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if ((sender as RadioButton).Checked)
                {
                    SetLabelText($"{(sender as RadioButton).Text} (initializing: {_initializedMissing.Count()})");
                    Init();
                }
            }
        }

        private void buttonMissing_Click(object sender, EventArgs e)
        {
            try
            {
                if (_initializedMissing == null || _initializedMissing.Count < 1 || _initializedMissing.Count > 30)
                {
                    SetLabelText($"Kan ikke utføre søk med {_initializedMissing.Count} elementer");
                }
                else
                {
                    var filter = Kolibri.net.Common.Utilities.MovieUtilites.MulitpartFilter();
                    foreach (string srch in _initializedMissing)
                    {
                        var matches = filter.Where(x => srch.ToLower().Contains(x.ToLower()));
                        var ant = matches.Count();
                        if (matches != null && matches.Count() < 1)
                        {
                            IMDBForms.MovieForm form = new MovieForm(_userSettings, new FileInfo(srch));
                            form.MdiParent = this.MdiParent;
                            form.Show();
                        }
                        else
                        {
                            SetLabelText($"[MULTIPART] skipped: {Path.GetFileName(srch)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, ex.GetType().Name); }
        }
    }
}