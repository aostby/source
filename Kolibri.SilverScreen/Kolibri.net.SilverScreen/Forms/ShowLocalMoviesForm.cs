using com.sun.jndi.toolkit.dir;
using com.sun.org.apache.bcel.@internal.generic;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.SilverScreen.Controls;
using Kolibri.net.SilverScreen.IMDBForms;
using OMDbApiNet.Model;
using System.Data;
using System.Linq;
using TMDbLib.Objects.Search;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class ShowLocalMoviesForm : Form
    {
        private List<string> _initializedMissing = new List<string>();
        LiteDBController _liteDB;
        TMDBController _TMDB;
        OMDBController _OMDB;
        ImageCache _imageCache;

        MultimediaType _type;
        private readonly UserSettings _settings;
        private IEnumerable<FileItem> _fileItems;
        private List<string> _searchFiles;
        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;

        public ShowLocalMoviesForm(MultimediaType type, UserSettings settings)
        {
            InitializeComponent();
            _type = type;
            this._settings = settings;
            this.Text = $"{_type.ToString()}";
            _liteDB = new LiteDBController(new FileInfo(settings.LiteDBFilePath), false, false);
            _searchFiles = new List<string>();
            Init();

        }

        private async void Init()
        {
            _fileItems = new List<FileItem>();
            labelNumItemsFILE.Text = $"Num files in dir: {_searchFiles.Count}";

            int diff = 0;
            List<string> common = MovieUtilites.MoviesCommonFileExt(true);
            var masks = common.Select(r => string.Concat('*', r)).ToArray();
            var searchStr = "*" + string.Join("|*", common);

            if (!checkBoxSimple.Checked && _settings != null)
            {
                if (string.IsNullOrEmpty(_settings.TMDBkey))
                    checkBoxSimple.Checked = true;
            }
            if (_type.Equals(MultimediaType.Series))
            {
                checkBoxSimple.Checked = true;
            }

            _imageCache = new ImageCache(_settings);
            try
            {
                if (_type.Equals(MultimediaType.movie))
                {
                    if (!string.IsNullOrEmpty(_settings.OMDBkey))
                        _TMDB = new TMDBController(_liteDB, _settings.OMDBkey);
                }
            }
            catch (Exception)
            {
                _TMDB = null;
            }
            buttonOpenFolder.Image = Icons.GetFolderIcon().ToBitmap();


            this.Text = $"{_type.ToString()} - {_settings.LiteDBFilePath}";
            var path = GetCurentPath();
            textBoxSource.Text = path;
            SetLabelText($"Current filepaht: {path} - Searching for {_type}");

            _fileItems = _liteDB.FindAllFileItems(new DirectoryInfo(path)).ToList();
            //filter
            if (!radioButtonFilterAlle.Checked)
            {
                SetForm(new Form());
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
                        var sublist = new List<string>();
                        //         var searchFiles = FileUtilities.GetFiles(new DirectoryInfo(GetCurentPath()), searchStr, SearchOption.AllDirectories);

                        labelNumItemsFILE.Text = $"Num files in dir: {_searchFiles.Count}";

                        //var searchFiles = Directory.EnumerateFiles(GetCurentPath(), "*.*", SearchOption.AllDirectories)
                        //        .Where(file => MovieUtilites.MoviesCommonFileExt(true).ToArray()
                        //         .Contains(Path.GetExtension(file))).ToList();

                        int teller = 0;
                        foreach (var srch in _searchFiles)
                        {
                            teller++;
                            if (sublist.Contains(srch)) continue;

                            var has = _fileItems.ToList().Find(cus => cus.FullName.Equals(srch));
                            if (has == null)
                            {
                                sublist.Add(srch);
                                var filter = Kolibri.net.Common.Utilities.MovieUtilites.MulitpartFilter();
                                var matches = filter.Where(x => srch.ToLower().Contains(x.ToLower()));
                                if (matches != null && matches.Count() > 0)
                                {
                                    if (!masks.Contains("@__thumb"))
                                    {
                                        sublist.Add($"[MULTIPART] The file is a mulitpart file: {srch}");
                                    }
                                }
                                else
                                {
                                    var test = _liteDB.FindByFileName(new FileInfo(srch));
                                    if (test == null)
                                    {
                                        sublist.Add($"[NULL] The file is not found by LiteDB search for FilteItems using: {srch}");
                                        if (!_initializedMissing.Contains(srch) && _searchFiles.Count() < 85)
                                        {
                                            try
                                            {
                                                IMDBForms.MovieForm form = new MovieForm(_settings, new FileInfo(srch));
                                                form.Show();
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
                        if (sublist.Count > 0)
                        {
                            Kolibri.net.Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBox(
                                $"{(diff - _fileItems.Count())} - {radioButtonFilterNotMatched.Text}",
                                string.Join(Environment.NewLine, sublist.ToArray()), FastColoredTextBoxNS.Language.Custom, this.Size);
                        }
                    } }
                else
                {
                    _fileItems = new List<FileItem>();
                }
            }


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
            string text = $"{count} found in LiteDB ({diff})";
            if (!radioButtonFilterAlle.Checked)
                text = $"{count} / diff: {diff} files";
            labelNumItemsDB.Text = text;
            SetLabelText(labelNumItemsDB.Text);

        }
    

        /// <summary>
        /// henter filer fra inneværende sti
        /// </summary>
        /// <returns></returns>
        private async void GetPathSearchFiles(string dInfo = null)
        {
            if (dInfo == null)
                dInfo = GetCurentPath();

            var searchFiles = Directory.EnumerateFiles(dInfo, "*.*", SearchOption.AllDirectories)
                      .Where(file => MovieUtilites.MoviesCommonFileExt(true).ToArray()
                       .Contains(Path.GetExtension(file))).ToList();

            _searchFiles= searchFiles;
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
                case MultimediaType.Movies: ret = _settings.UserFilePaths.MoviesSourcePath; break;
                case MultimediaType.Series: ret = _settings.UserFilePaths.SeriesSourcePath; break;
                case MultimediaType.Audio:
                    break;
                case MultimediaType.Pictures:
                    break;
                default:
                    throw new NotImplementedException(_type.ToString());

                    break;
            }
            return ret.Trim();
        }
        private async void SetCurrentPath(DirectoryInfo dInfo, bool init = true)
        {
            if (_searchFiles == null || _searchFiles.Count <= 0) {   GetPathSearchFiles(dInfo.FullName); }
            switch (_type)
            {
                case MultimediaType.Movies:
                    _settings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
                    break;
                case MultimediaType.Series:
                    _settings.UserFilePaths.SeriesSourcePath = dInfo.FullName;

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
                _liteDB.Upsert(_settings);
                textBoxSource.Text = dInfo.FullName;
                SetLabelText($"{_type} - set to {dInfo.FullName}");

                bool? tristate = null;
                if (radioButtonUpdateNew.Checked)
                    tristate = false;
                if (radioButtonUpdateAll.Checked)
                    tristate = true;

                MultiMediaSearchController searchController = new MultiMediaSearchController(_settings, updateTriState: tristate);
                if (_type.Equals(Constants.MultimediaType.movie) || _type.Equals(Constants.MultimediaType.Movies))
                {
                    Task.Run(async () => searchController.SearchForMovies(dInfo));
                }
                else if (_type.Equals(Constants.MultimediaType.Series))
                {

                    Task.Run(async () => searchController.SearchForSeriesEpisodes(dInfo));
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


                SetForm(_dgvController.Current, splitContainer2.Panel2);
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
                if (checkBoxSimple.Checked) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(mm as Item, _liteDB, tmdb: _TMDB); }
                else { form = new MovieForm(_settings, mm as Item); }
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
            catch (Exception ex)            {            }
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
    }
}