using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Tools;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.SilverScreen.Controls;
using Kolibri.net.SilverScreen.IMDBForms;

using OMDbApiNet.Model;
using System.Data;
using System.Reflection;
using static Kolibri.net.SilverScreen.Controls.Constants;
using static System.Net.Mime.MediaTypeNames;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class MyMoviesForm : Form
    {
        private List<string> _initializedMissing = new List<string>();
        private LiteDBController _liteDB;
        private TMDBController _TMDB;
        private OMDBController _OMDB;
        private ImageCacheDB _imageCache;
        private PlexController _plex;

        private readonly UserSettings _userSettings;
        /// <summary>
        /// Metadata imdbid for en filsti
        /// </summary>
        private IEnumerable<FileItem> _fileItems;
        /// <summary>
        /// fysiske filer som finnes
        /// </summary>
        private List<Item> _searchFiles;
        private List<string> _currentSearch = new List<string>();
        private MoviesSearchController _searchController;
        private Kolibri.net.SilverScreen.Controls.DataGrivViewControls _dgvController;
        private bool isProcessing;

        [Obsolete("Just for initializing, do not use, use with usersettings instead")]
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
        private async Task<bool> InitControllers() {
            bool ret = true;

             _plex = new PlexController(_userSettings); 
           
            _liteDB = new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false);
            _dgvController = new DataGrivViewControls(MultimediaType.Movies, _liteDB);
            _imageCache = new ImageCacheDB(_userSettings);
            try
            {
                if (_TMDB == null && !string.IsNullOrWhiteSpace(_userSettings.TMDBkey))
                    _TMDB = new TMDBController(_liteDB, _userSettings.TMDBkey);
            }
            catch (Exception)
            {
                ret = false;
                _TMDB = null;
            }
            return ret;
        }

        private async void StartUp()
        {
            var progress = ProgressBarHelper.InitProgressBar(toolStripProgressBar1);
            _searchController = new MoviesSearchController(_userSettings, plex: _plex, progress: progress);
            _searchController.ProgressUpdated += OnProgressUpdated;

            _ = Task.Run(async () => await InitControllers()).GetAwaiter().GetResult();
            _searchFiles = new List<Item>();
            textBoxSource.Text = GetCurentPath();
            this.Text = $" - {textBoxSource.Text}";
            SetLabelText(this.Text);
              
            buttonOpenFolder.Image = Icons.GetFolderIcon().ToBitmap();
            radioButtonShowGrid.Checked = true;

            _fileItems= _liteDB.FindAllFileItems(new DirectoryInfo(textBoxSource.Text));

            //var list = await _liteDB.FindAllItems();
            //_fileItems = list.ToList().Where(x => !string.IsNullOrEmpty(x.TomatoUrl) && x.TomatoUrl.StartsWith(_userSettings.UserFilePaths.MoviesSourcePath)).Select(data => new FileItem(data.ImdbId, data.TomatoUrl)).ToList();

            buttonVelg_Click(null, null);
        }


        private async void SetLabelText(string message)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new System.Windows.Forms.MethodInvoker(
                        delegate { SetLabelText(message); }
                    ));
                else
                {
                    toolStripStatusLabelStatus.Text = message;
                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// henter filer fra inneværende sti
        /// </summary>
        /// <returns></returns>
        private async Task<DirectoryInfo> SetPathSearchFiles(DirectoryInfo dInfo = null)
        {
            if (dInfo == null) dInfo = new DirectoryInfo(GetCurentPath());
            try
            {
                _currentSearch = await MovieUtilites.GetCommonMovieFiles(dInfo);

                //Filter
                _currentSearch = _currentSearch.Where(cdr => !cdr?.Contains("@__thumb") == true).ToList();
            }
            catch (Exception)
            {
                _currentSearch = new List<string>();
            }

            return dInfo;
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

        private async void buttonOpenFolder_Click(object sender, EventArgs e)
        {
            SetForm(new Form());
            SetForm(new Form(), splitContainer1.Panel1);
            Thread.SpinWait(5);
            buttonOpenFolder.Enabled = false;
            radioButtonShowGrid.Checked = true;
            groupBoxValg.Enabled = false;

            _searchFiles = new List<Item>();
            DirectoryInfo dInfo = null;
            try
            {
                dInfo = new DirectoryInfo(textBoxSource.Text);
            }
            catch (Exception)
            {
                dInfo = new DirectoryInfo(GetCurentPath());
            }
            string title = $"Let opp mappe ({Assembly.GetEntryAssembly().GetName().Name})";
            labelNumItemsDB.Text = title;

            dInfo = FileUtilities.LetOppMappe(dInfo.FullName,title);
            if (dInfo != null && dInfo.Exists)
            {
                SetLabelText($@"Searching for files in {dInfo.Name} ({dInfo.FullName})");

                if (checkBoxTristate.CheckState == CheckState.Checked)
                {
                    if (
                         MessageBox.Show("Alle metadata (imdbid) knyttet til kildemappe, blir slettet ved valg av kildemappe", "Fortsette?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                }

                _ = await SetCurrentPath(dInfo, checkBoxTristate.CheckState);
                groupBoxValg.Enabled = !groupBoxValg.Enabled;
                buttonVelg_Click(null, null);
            }
            groupBoxValg.Enabled = true;
            buttonOpenFolder.Enabled = true;
        }

        private async Task<DirectoryInfo> SetCurrentPath(DirectoryInfo dInfo, CheckState tristate = CheckState.Indeterminate)
        {
            await SetPathSearchFiles(dInfo);

            _userSettings.UserFilePaths.MoviesSourcePath = dInfo.FullName;
            _liteDB.Update(_userSettings);

            textBoxSource.Text = dInfo.FullName;
            SetLabelText($"Path - set to {dInfo.FullName}");

            _searchFiles = await _searchController.SearchForMovies(dInfo, tristate);
            if (!string.IsNullOrWhiteSpace(_searchController.CurrentLog.ToString()))
            {
                SetLabelText($"Log contains {_searchController.CurrentLog.ToString().Split(Environment.NewLine).Length} lines");
                // OutputDialogs.ShowRichTextBox($"CurrentLog", searchController.CurrentLog.ToString(), this.Size);
                //progress = ProgressBarHelper.InitProgressBar(toolStripProgressBar1);
            }
            _fileItems = _liteDB.FindAllFileItems(dInfo);

            var count = _fileItems.Count();
            var diff = count - _searchFiles.Count;
            labelNumItemsDB.Text = $"{count} found in LiteDB (diff: {diff} - folder: {_searchFiles.Count})  [{dInfo.Name}]";
            labelNumItemsDB.Tag = dInfo;

            return dInfo;
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

        private void SetForm(Form form, SplitterPanel setPanel = null)
        {
            try
            {
                SplitterPanel panel = setPanel;
                if (panel == null) panel = splitContainer1.Panel2;
                if (panel == splitContainer1.Panel1)
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
                form.Dock = DockStyle.Fill;
                panel.Controls.Add(form);
                form.Show();
                SetLabelText($"{form.Text}");
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }
        private async void SetForm(Item mm, SplitterPanel setPanel)
        {
            Form form;
            SplitterPanel panel = setPanel;
            FileInfo fi = null;
            if (checkBoxDetailType.Checked) { form = new Kolibri.net.SilverScreen.Forms.DetailsFormItem(mm as Item, _liteDB, tmdb: _TMDB, imagecache: _imageCache, plex: _plex); }
            else
            {
                Item? item = (Item)(mm as Item);
                try
                {
                    fi = (item != null) ? new FileInfo(mm.TomatoUrl) : null;
                    if (fi == null)
                    {

                        var test = await _liteDB.FindByFileNameAsync(fi);
                        item?.TomatoUrl = test?.ItemFileInfo.FullName;
                    }
                }
                catch (Exception) { }

                form = new MovieForm(_userSettings, mm as Item, fi , _plex);
            }
            form.Text += $" {mm.Title}";
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

        private async void ShowGridView(DataTable tableItem)
        {
            try
            {
                Form view = await _dgvController.GetMulitMediaDBDataGridViewAsForm(tableItem);
                var contr = (view.Controls[0] as DataGridView);
                contr.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //contr.SelectionChanged += DataGridView_LocalSelectionChanged;
                contr.SelectionChanged += DataGridView_LocalSelectionChanged;
                contr.RowPostPaint += new DataGridViewRowPostPaintEventHandler(this.DataGridView1_RowPostPaint);


                SetForm(view, splitContainer1.Panel1);

                SetLabelText($"{tableItem.Rows.Count} rader.");

                var movie = await _liteDB.GetItemAsync(tableItem.Rows[0]["ImdbId"].ToString());
                SetForm(movie, splitContainer1.Panel2);
            }
            catch (Exception ex)
            {
                SetLabelText($"Is the controller set? error: {ex.Message}");
            }
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // This event handles drawing the row numbers in the row header
            var grid = sender as DataGridView;
            if (grid != null)
            {
                string rowIdx = (e.RowIndex + 1).ToString();
                var centerFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Draw the number
                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }
        }

        private void DataGridView_LocalSelectionChanged(object sender, EventArgs e)
        {
            if (isProcessing) return;
            isProcessing = true;
            try
            {
                var dgv = (sender as DataGridView);
                if (dgv.Visible && dgv.SelectedRows.Count == 1)
                {
                    SetForm(_dgvController.CurrentItem, splitContainer1.Panel2);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
            finally
            {
                isProcessing = false;
            }
        }

        private async void buttonVelg_Click(object sender, EventArgs e)
        {
            if (radioButtonShowGrid.Checked)
            {
                var list = await _liteDB.FindItemsAsync(_fileItems);
                if (list != null && list.Count > 0)
                {
                    if (checkBox1.Checked)
                    {
                        TreeViewItemsForm form = new TreeViewItemsForm(list);
                        form.CurrentItemChanged += OnCurrentItemChanged;
                        SetForm(form, splitContainer1.Panel1);
                    }
                    else { ShowGridForDBItems(list); }
                }
                else
                {
                    SetLabelText($"No FileItems found. Is the controller set?");
                }
            }
            else if (radioButtonShowLog.Checked)
            {
                var logg = _searchController.CurrentLog.ToString();
                var err = logg.Split(Environment.NewLine).Where(line => line.Contains("[ERROR]")).ToList();
                err.Add($"/***************************************     ERRORS: {err.Count}     ******************************************************************/\r\n");

                logg = logg.Insert(0, string.Join(Environment.NewLine, err));

                Form form = Common.FormUtilities.Controller.OutputFormController.RichTexBoxForm("Current log",
                    logg,
                    FastColoredTextBoxNS.Language.XML,
                    new Size(50, 50));
                SetForm(form);

            }
            else if (radioButtonDuplicates.Checked)
            {
                SetLabelText("Searching for dupes.... please wait");
                SameFileController contr = new SameFileController(new DirectoryInfo(textBoxSource.Text));
                var list = contr.GetDupes();
                if (list != null && list.Count > 0)
                {
                    var ds = DataSetUtilities.AutoGenererDataSet(list);

                    Form form = Common.FormUtilities.Controller.OutputFormController.DataTableForm($"Dupes {ds.Tables[0].Rows.Count}", ds.Tables[0], ds.Tables[0].Columns[0], new Size(50, 50));
                    SetForm(form);
                }
                else
                {
                    SetForm(Common.FormUtilities.Controller.OutputFormController.RichTextBoxForm("Dupes list", "No dupes found.", new Size(50, 50)));
                }

                return;
            }
            else if (radioButtonShowDiff.Checked || radioButtonEditDiff.Checked)
            {
                try
                {
                    List<string> filepaths = _fileItems.Select(f => f.ItemFileInfo.FullName).ToList();
                    List<string> difflist = _currentSearch.Except(_searchFiles.Select(x => x.TomatoUrl).ToList()).ToList();
                    //    var liste = difflist.FindAll(x => x.Contains("CD", StringComparison.OrdinalIgnoreCase));
                    //    difflist = difflist.Except(liste).ToList();

                    if (difflist != null && difflist.Count == 0)
                    {
                        difflist = _fileItems.Where(x => !(File.Exists( x.ItemFileInfo.FullName))).Select(f => f.ItemFileInfo.FullName).ToList();
                    }
                    else if (difflist.Count >= 10)
                    {

                    }

                    if (radioButtonShowDiff.Checked)
                    {
                        System.Data.DataTable datatable = new System.Data.DataTable();

                        datatable.Columns.Add("File", typeof(String)); datatable.Columns.Add("FullName", typeof(String));
                        for (int i = 0; i < difflist.Count(); i++) { datatable.Rows.Add(Path.GetFileName(difflist[i]), difflist[i]); }
                        Form form = new Form();
                        if (datatable.Rows.Count > 0)
                        {
                            form = Common.FormUtilities.Controller.OutputFormController.DataTableForm("Diff list", datatable, datatable.Columns[1], new Size(50, 50));
                        }
                        else
                        {
                            form = Common.FormUtilities.Controller.OutputFormController.RichTextBoxForm("Diff list", "No diff found", new Size(50, 50));
                        }
                        SetForm(form);
                    }
                    else
                    {
                        if (difflist != null && difflist.Count == 0)
                        {


                            SetForm(Common.FormUtilities.Controller.OutputFormController.RichTextBoxForm("Diff list", "No diff found to edit", new Size(50, 50)));

                        }
                        else
                        {

                            Random random = new Random();
                            foreach (string productName in difflist.OrderBy(item => random.Next()).Take(10))
                            {
                                var info = new FileInfo(productName);
                                if (info.Exists)
                                {
                                    Form form = new MovieForm(_userSettings, info, $"{MovieUtilites.GetYear(info.Directory.FullName)}");
                                    form.MdiParent = this.ParentForm;
                                    form.Show();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                { SetLabelText($"Error occured. {ex.GetType()} - {ex.Message}"); }
            }
        }

        private async void OnCurrentItemChanged(object? sender, EventArgs e)
        {

            if (isProcessing) return;
            isProcessing = true;
            try
            {
                var tvi = (sender as TreeViewItemsForm);
                if (tvi.Visible && tvi.CurrentItem != null)
                {
                    var checkBox = (tvi.Controls.Find("radioButtonActor", true).FirstOrDefault() as RadioButton);
                    if (checkBox != null && checkBox.Checked) {

                        var credits = await _liteDB.GetCredits(tvi.CurrentItem.ImdbId);
                        if (credits == null)
                        {
                            credits = await _TMDB.GetMovieCredits(tvi.CurrentItem.Title, tvi.CurrentItem.Year.ToInt32());
                            if (credits != null && credits.Cast.Count >= 1)
                            {
                                _ = await _liteDB.Upsert(tvi.CurrentItem.ImdbId, credits);
                            }
                        }


                        var form = await CreatateFormController.GenerateFormFromActors(credits, tvi.CurrentItem, _imageCache);
                        SetForm(form, splitContainer1.Panel2);
                    }
                    else
                    { 
                        SetForm(tvi.CurrentItem, splitContainer1.Panel2);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
            finally
            {
                isProcessing = false;
            }
        }

        private void OnProgressUpdated(object sender, string progress)
        {
            try
            {
                SetLabelText(progress);
            }
            catch (Exception ex)
            { SetLabelText(ex.Message); }
        }

        private void checkBoxTristate_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                var state = (sender as CheckBox).CheckState ;
                string text = $"{state} er valgt. ";

                switch (state)  
                {
                    case CheckState.Unchecked:
                        text += "Ingen endringer ved valg av kildemappe";
                        break;
                    case CheckState.Checked:
                        text += "Alle innslag (imdbid) knyttet til kildemappe, blir slettet ved valg av kildemappe";
                        break;
                    case CheckState.Indeterminate:
                        text += "Alle nye filer blir lagt til eller oppdatert ved valg av kildemappe";
                        break;
                    default:
                        break;
                }
                SetLabelText(text);
            }
            catch (Exception ex)
            {
            }
        }
    }
}