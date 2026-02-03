using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Tools;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.SilverScreen.IMDBForms;
using OMDbApiNet.Model;
using sun.net.www.content.audio;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class MyMoviesForm : Form
    {
        private List<string> _initializedMissing = new List<string>();
        LiteDBController _liteDB;
        TMDBController _TMDB;
        OMDBController _OMDB;
        ImageCacheDB _imageCache;

        private readonly UserSettings _userSettings;
        private IEnumerable<FileItem> _fileItems;
        private List<string> _searchFiles;
        private List<string> _currentSearch = new List<string>();

        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;


        public MyMoviesForm()
        {
            InitializeComponent();
        }

        public MyMoviesForm(UserSettings userSettings)
        {
            _userSettings = userSettings;
            InitializeComponent();
            StartUp();
        }

        private async void StartUp()
        {
            textBoxSource.Text = GetCurentPath();
            _searchFiles = new List<string>();
            this.Text = $" - {_userSettings.LiteDBFilePath}";
            GetPathSearchFiles();
            _liteDB = new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false);

            _imageCache = new ImageCacheDB(_userSettings);
            buttonOpenFolder.Image = Icons.GetFolderIcon().ToBitmap();
            // buttonManual.Image = Icons.IconFromExtensionShell("avi", Icons.SystemIconSize.Small).ToBitmap();
            try
            {
                if (_TMDB == null && !string.IsNullOrWhiteSpace(_userSettings.OMDBkey))
                    _TMDB = new TMDBController(_liteDB, _userSettings.OMDBkey);

            }
            catch (Exception)
            {
                _TMDB = null;
            }
        }

        private void SetLabelText(string message)
        {
            try
            {
                Task.Delay(1).GetAwaiter().GetResult();
                if (InvokeRequired)
                    Invoke(new System.Windows.Forms.MethodInvoker(
                        delegate { SetLabelText(message); }
                    ));
                else
                {

                    toolStripStatusLabelStatus.Text = message;
                    Thread.Sleep(3);

                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// henter filer fra inneværende sti
        /// </summary>
        /// <returns></returns>
        private async void GetPathSearchFiles(string dInfo = null)
        {

            if (dInfo == null)
                dInfo = GetCurentPath();
            try
            {
                if (_currentSearch == null || _currentSearch.Count < 1 || !_currentSearch.FirstOrDefault().Contains(dInfo))
                {
                    _currentSearch = Directory.EnumerateFiles(dInfo, "*.*", SearchOption.AllDirectories)
                                  .Where(file => MovieUtilites.MoviesCommonFileExt(true).ToArray()
                                   .Contains(Path.GetExtension(file))).ToList();

                    //Filter
                    _currentSearch = _currentSearch.Where(cdr => !cdr?.Contains("@__thumb") == true).ToList();
                }
            }
            catch (Exception)
            {
                _currentSearch = new List<string>();
            }
            _searchFiles = _currentSearch;
        }

        private string GetCurentPath()
        {
            string ret = string.Empty;

            ret = _userSettings.UserFilePaths.MoviesSourcePath;

            if (!Directory.Exists(ret))
            {
                ret = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            return ret.Trim();
        }

        private void buttonOpenFolder_Click(object sender, EventArgs e)
        {
            _searchFiles = new List<string>();
            DirectoryInfo dInfo = null;
            try
            {
                dInfo = new DirectoryInfo(textBoxSource.Text);
            }
            catch (Exception)
            {
                dInfo = new DirectoryInfo(GetCurentPath());
            }

            dInfo = FileUtilities.LetOppMappe(dInfo.FullName, $"Let opp mappe ({Assembly.GetEntryAssembly().GetName().Name})");
            if (dInfo != null && dInfo.Exists)
            {
                SetCurrentPath(dInfo);
            }
        }

        private async void SetCurrentPath(DirectoryInfo dInfo)
        {
            if ((dInfo.FullName != _userSettings.UserFilePaths.MoviesSourcePath) || _searchFiles.Count <= 0) { GetPathSearchFiles(dInfo.FullName); }

            _userSettings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
            _liteDB.Update(_userSettings);

            _liteDB.Upsert(_userSettings);
            textBoxSource.Text = dInfo.FullName;
            SetLabelText($"Path - set to {dInfo.FullName}");

            var progress = ProgressBarHelper.InitProgressBar(toolStripProgressBar1);
            MoviesSearchController searchController = new MoviesSearchController(_userSettings, progress: progress);
            await searchController.SearchForMovies(dInfo);
            if (!string.IsNullOrWhiteSpace(searchController.CurrentLog.ToString()))
            {
                SetLabelText($"Log contains {searchController.CurrentLog.ToString().Split(Environment.NewLine).Length} lines");
                // OutputDialogs.ShowRichTextBox($"CurrentLog", searchController.CurrentLog.ToString(), this.Size);
                progress = ProgressBarHelper.InitProgressBar(toolStripProgressBar1);
            }
            var count = _liteDB.FindAllFileItems(dInfo). Count();
            var diff = _searchFiles.Count - count;
            labelNumItemsDB.Text = $"{count} found in LiteDB (diff: {diff} - folder: {_searchFiles.Count})";
            labelNumItemsDB.Tag = dInfo;
        }

        private void labelNumItemsDB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (sender.Equals(labelNumItemsDB))
                {
                    DirectoryInfo dirInfo = labelNumItemsDB.Tag as DirectoryInfo;
                    if (dirInfo == null)
                    {
                        dirInfo = new DirectoryInfo(textBoxSource.Text);
                        SetLabelText($"No folder really used {buttonOpenFolder.Text}. Using the displayed text as source.");
                    }

                    FolderUtilities.OpenFolderInExplorer(dirInfo.FullName);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void SetForm(Form form, Panel setPanel = null)
        {
            try
            {
                Panel panel = setPanel;
                if (panel == null) panel = panel1;
                if (panel == panel)
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
        private void SetForm(Object mm, Panel setPanel)
        {
            Form form;
            Panel panel = setPanel;

         
                if (true) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(mm as Item, _liteDB, tmdb: _TMDB, imagecache: _imageCache); }
                else { form = new MovieForm(_userSettings, mm as Item); }
            
            SetForm(form, panel);
        }
        private void ShowGridForDBItems(List<Item> list = null)
        {
            string path = GetCurentPath();
            DataTable resultTable = new DataTable();
            try
            {
                if (list != null && list.Count > 0)
                {
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

        private void ShowGridView(DataTable tableItem)
        {
            try
            {
                Form view = _dgvController.GetMulitMediaDBDataGridViewAsForm(tableItem);
                (view.Controls[0] as DataGridView).SelectionChanged += DataGridView_LocalSelectionChanged;
                SetForm(view, panel1);
                
                SetLabelText($"{tableItem.Rows.Count} rader.");

                var movie = _liteDB.FindItem(tableItem.Rows[0]["ImdbId"].ToString());
                SetForm(movie, panel1);
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
                SetForm(_dgvController.CurrentItem, panel1);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var list = _liteDB.FindItems(_fileItems);
            ShowGridForDBItems(list);
        }
    }
}