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
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class MultiMediaForm : Form
    {
        LiteDBController _liteDB;
        TMDBController _TMDB;
        MultimediaType _type;
        private readonly UserSettings _settings;
        private IEnumerable<FileItem> _files;
        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;
        private List<Item> _items;

        public MultiMediaForm(MultimediaType type, UserSettings settings)
        {
            InitializeComponent();
            _type = type;
            this._settings = settings;
            this.Text = $"{_type.ToString()}";
            _liteDB = new LiteDBController(new FileInfo(settings.LiteDBFilePath), false, false);
            checkBoxSimple.Checked = _settings != null && string.IsNullOrEmpty( _settings.TMDBkey);
            Init();

        }

        private void Init()
        {
            buttonOpenFolder.Image = Icons.GetFolderIcon().ToBitmap();

            _files = new List<FileItem>();
            this.Text = $"{_type.ToString()} - {_settings.LiteDBFilePath}";
            var path = GetCurentPath();
            textBoxSource.Text = path;
            SetLabelText($"Current filepaht: {path} - Searching for {_type}");
            _files = _liteDB.FindAllFileItems(new DirectoryInfo(path)).ToList();
            labelNumItemsDB.Text = $"{_files.Count()} files found";
            SetLabelText(labelNumItemsDB.Text);

            if (_dgvController == null) _dgvController = new DataGrivViewControls(_liteDB);
            _items = GetItems();

        


            ShowGridForDBItems();
        }

        private List<Item> GetItems()
        {
            var task = Task.Run<Task<IEnumerable<Item>>>(async () => await _liteDB.FindAllItems(_type.ToString()));
            return task.Result.Result.ToList();
        }

        private void ShowGridForDBItems()
        {
            string path = GetCurentPath();

            try
            {
                var movie = _liteDB.FindItem(_files.FirstOrDefault().ImdbId);

                if (_files != null && _files.Count() > 0)
                {
                    SetLabelText($"Searching for {_type}.....");
                    var lookup = _files.Distinct().ToDictionary(x => x.ImdbId);
                    var searchList = _items.Where(t => lookup.ContainsKey(t.ImdbId));

                    var resultTable = DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList(searchList.ToArray())).Tables[0];

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

                    Task.Run(async () =>
                        searchController.SearchForMovies(dInfo)
                    );

                }
                Init();
            }
        }
        private void ShowGridView(DataTable tableItem)
        {
            try
            {
                SetLabelText($"{tableItem.Rows.Count} rader.");

                var movie = _liteDB.FindItem(tableItem.Rows[0]["ImdbId"].ToString());
                SetForm(movie);
                Form view = _dgvController.GetMovieItemDataGridViewAsForm(tableItem);
                (view.Controls[0] as DataGridView).SelectionChanged += DataGridView_LocalSelectionChanged;
                SetForm(view, splitContainer2.Panel1);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }
        private void DataGridView_LocalSelectionChanged(object sender, EventArgs e)
        {
            try
            {
                SetForm(_dgvController.CurrentItem);
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
        private void SetForm(Item item)
        {
            Form form;
            SplitterPanel panel = splitContainer2.Panel2;
            if (checkBoxSimple.Checked) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(item, _liteDB); }
            else            {                form = new MovieForm(_settings, item);            }
            SetForm(form, panel);
        }

        private void SetForm(Form form, SplitterPanel setPanel = null)
        {
            try
            {
                SplitterPanel panel = splitContainer2.Panel1;
                if (setPanel != null) panel = setPanel;
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