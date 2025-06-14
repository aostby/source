﻿using com.sun.org.apache.bcel.@internal.generic;
using javax.print.attribute.standard;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controller;
using OMDbApiNet.Model;
using System.Data;
using System.Data.SqlClient;
using System.Text;
 

namespace Kolibri.net.SilverScreen.OMDBForms
{
    public partial class OMDBSearchForSeriesForm : Form
    {
        public DirectoryInfo Source { get; private set; }
        private UserSettings _settings;
        private LiteDBController _liteDB;
        private KolibriTVShowSearchController _contr;
        private List<string> _sourceSeriesFiles = new List<string>();
        private List<string> _sourceSeriesFolders = new List<string>();

        private Item _currentItem = null;

        //private DirectoryInfo[] _directoryInfos;

        public OMDBSearchForSeriesForm(DirectoryInfo source, UserSettings settings, LiteDBController liteDB)
        {
            InitializeComponent();
            Application.DoEvents();

            _settings = settings;
            _liteDB = liteDB;
            if (_liteDB == null)
            {
                _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath), false, false);
            }
            if (_contr == null) _contr = new KolibriTVShowSearchController(_settings, _liteDB);

            Source = source;
            Init();
        }



        private void Init()
        {

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            labelFilsti.Text = Source.FullName;
            SetStatusLabelText(Source.FullName);


            try
            {

                _sourceSeriesFiles = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(Source, MovieUtilites.MoviesCommonFileExt(true), SearchOption.AllDirectories);
                this.Text = $"Search for Series - {Source.FullName} - [{_sourceSeriesFiles.Count} files in path]";
                Application.DoEvents();
                var path = _sourceSeriesFiles.FirstOrDefault();
                string title = $"{SeriesUtilities.GetSeriesTitle(path)}".FirstToUpper();
                string year = $"{MovieUtilites.GetYear(Source.Name)}".Trim();
                textBoxSearchValue.Text = $"{title}".Trim();


                var queryGroupByExt = from file in _sourceSeriesFiles
                                      group file by Path.GetDirectoryName(file) into folderGroup
                                      orderby folderGroup.Count(), folderGroup.Key
                                      select folderGroup;

                _sourceSeriesFolders = queryGroupByExt.Select(x => x.Key).ToList();

                var queryGroupByName = from dir
                                       in _sourceSeriesFolders
                                       group Name by Path.GetDirectoryName(dir) into folderGroup
                                       orderby folderGroup.Count(), folderGroup.Key
                                       select folderGroup;

                var firstLevel = queryGroupByName.Select(x => x.Key).ToList();

                DataTable resultTable = DataSetUtilities.ColumnNamesToDataTable("Folders", "Title", "ImdbId").Tables[0];
                resultTable.TableName = "Init";
                foreach (var item in firstLevel)
                {
                    string tit = SeriesUtilities.GetSeriesTitle(item);
                    Item omdb = null;
                    if (item.Contains("{"))
                        omdb = _liteDB.FindItem(item.ImdbIdFromDirectoryName());
                    if (omdb == null) { omdb = _liteDB.FindItemByTitle(tit).FirstOrDefault(); }



                    resultTable.Rows.Add(
                              $"{item}",
                              $"{(omdb == null ? tit : omdb.Title)}",
                              $"{(omdb == null ? "" : omdb.ImdbId)}");
                }
                dataGridView1.DataSource = resultTable;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                SetStatusLabelText($"{Source.Name} - {queryGroupByExt.Count()} subfolders found.");

            }

            catch (Exception ex)
            {
                textBoxSearchValue.Text = "Tulsa King";
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            this.Cursor = Cursors.Default;
        }

        private void SetStatusLabelText(string message)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetStatusLabelText(message); }
                    ));
                else
                {
                    toolStripStatusLabelStatus.Text = message;
                    Thread.Sleep(1);
                }
                try
                {
                    Application.DoEvents();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnProgressUpdated(object sender, string progress)
        {
            try
            {
                SetStatusLabelText(progress);
            }
            catch (Exception)
            { }
        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ProcessThisFolder(Source);
            this.Cursor = Cursors.Default;
        }

        private async void ProcessThisFolder(DirectoryInfo source)
        {
            if (_contr == null) _contr = new KolibriTVShowSearchController(_settings);
            _contr.ProgressUpdated += OnProgressUpdated;

            StringBuilder Log = new StringBuilder();
            try
            {
                List<DataTable> list = new List<DataTable>();

                SetStatusLabelText($"{source.Name} - Searching for details (Start: {DateTime.Now.ToShortTimeString()})");

                try
                {
                    //List<Episode> epList = new List<Episode>();
                    List<Episode> epList = new List<Episode>();
                    DataTable table = null;

                    if (radioButtonLocal.Checked)
                    {
                        var test = await _contr.SearchForSeriesAsync(source);
                        epList = test.SelectMany(x => x.EpisodeList).OrderBy(x => x.Title).ToList();
                        table = DataSetUtilities.AutoGenererDataSet<Episode>(epList).Tables[0];
                        table = SeriesUtilities.SortAndFormatSeriesTable(table);
                    }
                    else
                    {
                        table = await _contr.SearchForSeriesEpisodes(source);
                    }
                    table.TableName = DataSetUtilities.LegalSheetName(source.Name);
                    list.Add(table);
                }
                catch (Exception ex)
                {
                    string error = $"{source.Name} - {ex.Message}";
                    Log.AppendLine($"{error}{Environment.NewLine}{"*".PadRight(72, '*')}");
                    SetStatusLabelText(error);

                    Application.DoEvents();
                    Thread.Sleep(20);
                }

                DataTable resultTable = DataSetUtilities.MergeListOfSimilarTables(list);
                //ta bort duplikate rader
                var tmp = new DataView(resultTable, null, $"{resultTable.Columns[0].ColumnName} ASC, SeasonNumber ASC, EpisodeNumber ASC", DataViewRowState.CurrentRows).ToTable(true, DataSetUtilities.ColumnNames(resultTable));

                resultTable = new DataView(tmp).ToTable(true, DataSetUtilities.ColumnNames(resultTable));
                dataGridView1.DataSource = resultTable;

                //TODO - finn feilen med dette
                //Kolibri.net.SilverScreen.Controls.DataGrivViewControls dgwController = new Controls.DataGrivViewControls(MultimediaType.Series, _liteDB);
                //dataGridView1 = new DataGridView();
                //dataGridView1 = dgwController.GetSeasonEpisodeDataGridView(resultTable);
                Log.AppendLine(_contr.CurrentLog.ToString());
                if (Log.Length >= 72)
                {
                    OutputDialogs.ShowRichTextBoxDialog($"{nameof(Log)} - {DateTime.Now.ToShortTimeString()}", Log.ToString(), this.Size);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonManual_Click(object sender, EventArgs e)
        {

            try
            {
                var yearItem = _liteDB.FindItem(textBoxSearchValue.Text);
                string year = "";
                if (!string.IsNullOrWhiteSpace(textBoxYearValue.Text)) { year = textBoxYearValue.Text; }
                IMDBForms.MovieForm form = new IMDBForms.MovieForm(_settings, new FileInfo(textBoxSearchValue.Text), year);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            try
            {
                var str = _contr.CurrentLog.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    OutputDialogs.ShowRichTextBox($"{this.Text} - {DateTime.Now.ToShortTimeString()}", str.ToString(), this.Size);
                }
            }
            catch (Exception ex)
            {
                SetStatusLabelText($"Log not found ({this.Text}) - {ex.Message}");
            }
        }

        private void textBoxManual_TextChanged(object sender, EventArgs e)
        {
            buttonImdbIdSearch.Enabled = textBoxSearchValue.Text.StartsWith("tt", StringComparison.InvariantCultureIgnoreCase);
        }

        private void buttonImdbIdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Item item = null;
                using (OMDBController omdbC = new OMDBController(_settings.OMDBkey, rottenTomatoRatings: true))
                {
                    // item = _liteDB.FindItem(textBoxManual.Text); //Ikke hent lokal kopi, poenget med manuelt søk er å hente ny versjon fra OMDB
                    if (item == null) item = omdbC.GetItemByImdbId(textBoxSearchValue.Text);
                    if (item == null)
                    {
                        using (TMDBController contr = new TMDBController(_liteDB, _settings.TMDBkey))
                        {
                            var t = Task.Run(() => contr.FindById(textBoxSearchValue.Text)).GetAwaiter().GetResult();
                            if (t != null)
                            {

                                Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog($"{t.GetType().Name} - {textBoxSearchValue.Text}", t.JsonSerializeObject(), this.Size);
                            }
                        }
                    }
                    else
                    {
                        if (_liteDB.Upsert(item))
                        {
                            string text = $"Oppdaterte lokal database med {item.Title} - {item.ImdbId} ({item.Year})";
                            SetStatusLabelText(text);
                            MessageBox.Show(text, "Lokal database oppdatert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string text = $"Feil ved oppdatering av lokal database. Sjekk innstillinger. {item.Title} - {item.ImdbId} ({item.Year})";
                            SetStatusLabelText(text);
                            MessageBox.Show(text, "Lokal database oppdatert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonTmdbSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (TMDBController contr = new TMDBController(_liteDB, _settings.TMDBkey))
                {
                    var t = Task.Run(() => contr.FetchSerie(textBoxSearchValue.Text)).GetAwaiter().GetResult();
                    if (t != null)
                    {
                        if (t.Count == 1)
                        {

                            var year = t.FirstOrDefault().FirstAirDate.Value.ToString("yyyy");
                            IMDBForms.MovieForm form = new IMDBForms.MovieForm(_settings, new FileInfo(t.FirstOrDefault().Name), year);
                            form.ShowDialog();
                        }
                        else
                        {
                            Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog($"{t.GetType().Name} - {textBoxSearchValue.Text}", t.JsonSerializeObject(), this.Size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var folder = FolderUtilities.LetOppMappe(Source.FullName, "Let opp mappe med serie(r)");
                if (folder != null && folder.Exists)
                {
                    Source = folder;
                    dataGridView1.DataSource = null;
                    Application.DoEvents();
                    Init();
                }
            }
            catch (Exception ex)
            {
                SetStatusLabelText(ex.Message);
            }

        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var folder = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                FolderUtilities.OpenFolderInExplorer(folder);
            }
            catch (Exception ex)
            {
                SetStatusLabelText(ex.Message);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            labelPath.Text = "Path: ";
            try
            {
                if (dataGridView1.Focused)
                {
                    var item = _liteDB.FindItem(dataGridView1.SelectedRows[0].Cells["ImdbId"].Value.ToString());
                    pictureBoxCurrent.Image = (Bitmap)ImageUtilities.GetImageFromUrl(item.Poster);
                    labelPath.Text = item.TomatoUrl.ToString();
                }
                else
                {
                    pictureBoxCurrent.Image = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
                }
            }
            catch (Exception ex)
            {
                pictureBoxCurrent.Image = (Bitmap)ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
            }
        }

        private async void ContextMenuEvent_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxYearValue.Text = string.Empty;
                if (sender.Equals(toolStripMenuItemNavn))
                {
                    var item = _liteDB.FindItem(dataGridView1.SelectedRows[0].Cells["ImdbId"].Value.ToString());
                    if (item != null)
                    {
                        textBoxSearchValue.Text = item.Title;
                        textBoxYearValue.Text = item.Year;
                        if (item.Year.Length > 4)
                            textBoxYearValue.Text = item.Year.Substring(0, 4);
                    }
                    else
                    {
                        textBoxSearchValue.Text = SeriesUtilities.GetSeriesTitle(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                    }
                }
                if (sender.Equals(toolStripMenuItemImdbId))
                {
                    string ret = string.Empty;
                    bool ok = InputDialogs.InputBox("Sett en IMDB id for denne mappen.", "Please submit folder IMDBId", ref ret) == DialogResult.OK;
                    if (ok)
                    {
                        if (!string.IsNullOrWhiteSpace(ret))
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(dataGridView1.SelectedRows[0].Cells["Folders"].Value.ToString());
                            if (dirInfo.Exists)
                            {
                                if (!dirInfo.FullName.Contains(ret))
                                {


                                    var destination = dirInfo.FullName + $" {{imdb-{ret}}}";
                                    Directory.Move(dirInfo.FullName, destination);
                                    string tit = dataGridView1.SelectedRows[0].Cells["Title"].Value.ToString();
                                    var item = _liteDB.FindItem(tit);
                                    if (item != null)
                                    {
                                        item.TomatoUrl = destination;
                                        _liteDB.Upsert(item);
                                        _liteDB.Upsert(new FileItem(item.ImdbId, item.TomatoUrl));
                                    }
                                }

                            }
                        }
                        Init();
                    }
                }
                if (sender.Equals(toolStripMenuItemSearchThisFolder))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dataGridView1.SelectedRows[0].Cells["Folders"].Value.ToString());

                    var test = await _contr.SearchForSeriesAsync(dirInfo);
                    if (test.Count() == 1)
                    {
                        var ktv = test[0];
                        ktv.Item.TomatoUrl = dirInfo.FullName;
                        _liteDB.Upsert(ktv.Item);
                        _liteDB.Upsert(new FileItem(ktv.Item.ImdbId, ktv.Item.TomatoUrl));

                    }
                    else
                    {
                        radioButtonCombo.Checked = string.IsNullOrEmpty(dataGridView1.SelectedRows[0].Cells["ImdbId"].Value.ToString());
                        ProcessThisFolder(dirInfo);
                    }
                    await Task.Delay(7000);
                    Init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void buttonExecuteChange_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text)) throw new Exception("Cannot change from nothing");


                var list = _liteDB.FindAllFileItems();
                foreach (var fItem in list)
                {
                    if (fItem.FullName.Contains(textBox1.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        fItem.FullName = fItem.FullName.Replace(textBox1.Text, textBox2.Text);
                        fItem.ItemFileInfo.Refresh();
                        _liteDB.Upsert(fItem);
                        if (Directory.Exists(fItem.FullName))
                        {
                            Item item = _liteDB.FindItem(fItem.ImdbId);
                            if (item != null && item.Type == "series")
                            {
                                SetStatusLabelText($"Updating path for {fItem.FullName}");

                                item.TomatoUrl = fItem.ItemFileInfo.FullName;
                                _liteDB.Upsert(item);
                            }
                        }

                    }
                    var txt = $"{fItem.ItemFileInfo.Exists} - {fItem.FullName}";
                    builder.AppendLine(txt);
                }

                Kolibri.net.Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog("Report filepaths", builder.ToString(), this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void labelPath_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(labelPath.Text);
                SetStatusLabelText($"{labelPath.Text} - copied to clipboard");
            }
            catch (Exception ex)
            {
                SetStatusLabelText($"{ex.Message} - {ex.GetType().Name}");
            }
        }

        private void buttonFindAndUpdateByIMDBID_Click(object sender, EventArgs e)
        {
            string whatsup = string.Empty;
            StringBuilder builder = new StringBuilder();
            try
            {
                if (_sourceSeriesFolders.Count==0) throw new Exception("Cannot change - no folders found!!!"); 
                
                foreach (var folder in _sourceSeriesFolders)
                {
                    whatsup = string.Empty;
                    if (folder.Contains($"-tt")&&folder.Contains("{"))
                    {
                        whatsup = folder;

                        if (folder.Contains("Game")) {
                            string watch = "out";
                        }

                        string imdbid = folder.Substring(0,folder.IndexOf("}")).Split("-").LastOrDefault();
                        
                       FileItem fItem = new FileItem(imdbid,  folder.Substring(0, folder.LastIndexOf("}")+1));   
                        
                       
                        if (Directory.Exists(fItem.FullName))
                        { _liteDB.Upsert(fItem);
                            Item item = _liteDB.FindItem(imdbid);
                            if (item != null && item.Type == "series")
                            {
                                SetStatusLabelText($"Updating path for {fItem.FullName}");

                                item.TomatoUrl =fItem.FullName;
                                _liteDB.Upsert(item);
                            }
                        }
                        var txt = $"{Directory.Exists( folder)} - {fItem.FullName}";
                        builder.AppendLine(txt);
                    }
                  
                }

                Kolibri.net.Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog("Report filepaths", builder.ToString(), this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(whatsup +" :"+ ex.Message, ex.GetType().Name);
            }
        }
    }
}