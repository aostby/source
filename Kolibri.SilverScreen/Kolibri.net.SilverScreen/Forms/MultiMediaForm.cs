using com.sun.istack.@internal.localization;
using javax.swing.plaf.multi;
using Kolibri.net.Common.Dal.Controller;
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
using System.Windows.Forms;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class MultiMediaForm : Form
    {
        LiteDBController _liteDB;
        
        TMDBController _TMDB;
        OMDBController _OMDB;
        ImageCache _imageCache;

        MultimediaType _type;
        private readonly UserSettings _settings;
        private IEnumerable<FileItem> _files;
        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;
    //    private List<Item> _items;

        public MultiMediaForm(MultimediaType type, UserSettings settings)
        {
            InitializeComponent();
            _type = type;
            this._settings = settings;
            this.Text = $"{_type.ToString()}";
            _liteDB = new LiteDBController(new FileInfo(settings.LiteDBFilePath), false, false);
      
            Init();

        }

        private void Init()
        {
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

            _files = new List<FileItem>();
            this.Text = $"{_type.ToString()} - {_settings.LiteDBFilePath}";
            var path = GetCurentPath();
            textBoxSource.Text = path;
            SetLabelText($"Current filepaht: {path} - Searching for {_type}");

            _files = _liteDB.FindAllFileItems(new DirectoryInfo(path)).ToList();
            //filter
            if (radioButtonNoneExistant.Checked)
            {
                var filtered = _files.Where(x => !x.ItemFileInfo.Exists);
                if (filtered != null && filtered.Count() >= 1) _files = filtered;
            }

            if (_dgvController == null) _dgvController = new DataGrivViewControls(_type, _liteDB);
            int count = 0;
         
            switch (_type)
            {
                case MultimediaType.movie:
                case MultimediaType.Movies:
                    
                    var list = new List<Item>();
                    list = GetItems(_files);
                    count = list.Count;
                    ShowGridForDBItems(list);
                    break;
                case MultimediaType.Series:
                  
                    var episodes = new List<SeasonEpisode>();
                    episodes = GetEpisodes(_files);
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
            labelNumItemsDB.Text = $"{count} files found";
            SetLabelText(labelNumItemsDB.Text);
        }

        private List<Item> GetItems(IEnumerable<FileItem> files=null)
        {
            var task = Task.Run<Task<IEnumerable<Item>>>(async () => await _liteDB.FindAllItems(_type.ToString()));
            var ret= task.Result.Result.ToList();
            if (files != null && files.Count() >= 1)
            {
                var sublist = new List<Item>();

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
                if (_files != null && _files.Count() > 0)
                {
                    SetLabelText($"Searching for {_type}.....");
                    var lookup = _files.Distinct().ToDictionary(x => x.ImdbId);

                    if (list != null&&list.Count>0)
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

        private void ShowGridForDBItems(List<Item> list=null)
        {   string path = GetCurentPath();
            DataTable resultTable = null;
            try
            {
                if (_files != null && _files.Count() > 0)
                {
                    SetLabelText($"Searching for {_type}.....");
                    var lookup = _files.Distinct().ToDictionary(x => x.ImdbId);

                    if (list != null)
                    {
                        var searchList = list;
                        resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];
                        var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                        resultTable = temp;
                        resultTable.TableName = DataSetUtilities.LegalTableName(System.IO.Path.GetFileNameWithoutExtension(path));
                        if (_files.Count() > list.Count()) SetLabelText($"NB - det er bare funnet {list.Count()} av {_files.Count()}. ");
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
        private void SetCurrentPath(DirectoryInfo dInfo, bool init = true)
        {
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
                if (!radioButtonNone.Checked)
                {
                    MultiMediaSearchController searchController = new MultiMediaSearchController(_settings, updateNewOnly: radioButtonNew.Checked);
                    if (_type.Equals(Constants.MultimediaType.movie) || _type.Equals(Constants.MultimediaType.Movies)) Task.Run(async () => searchController.SearchForMovies(dInfo));
                    else if (_type.Equals(Constants.MultimediaType.Series) || _type.Equals(Constants.MultimediaType.Series)) Task.Run(async () => searchController.SearchForSeries(dInfo));

                }
                else if (_type.Equals(Constants.MultimediaType.Series))
                {
                    MultiMediaSearchController searchController = new MultiMediaSearchController(_settings, updateNewOnly: radioButtonNew.Checked);
                    Task.Run(async () => searchController.SearchForSeries(dInfo));
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
                SetForm(view,splitContainer2.Panel1);

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
                        SetForm(new DetailsFormSeries(series, _liteDB, _OMDB, _TMDB, null, _imageCache), splitContainer2.Panel2);
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
            { Form form = new Form();
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
                form = new DetailsFormSeries(mm as SeasonEpisode, _liteDB, _OMDB, _TMDB, null, _imageCache);
            }
            else
            {
                if (checkBoxSimple.Checked) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(mm as Item, _liteDB, tmdb: _TMDB); }
                else { form = new MovieForm(_settings, mm as Item); }
            }
            SetForm(form, panel);
        }

        private void SetForm(Form form, SplitterPanel setPanel=null)
        {
            try
            {
                SplitterPanel panel = setPanel;
                if (panel == null) panel = splitContainer2.Panel2;
                panel.Controls.Clear();

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
            toolStripStatusLabelStatus.Text = text;
        }
    }
}