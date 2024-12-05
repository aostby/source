using Kolibri.Common.MovieAPI.Controller;
using Kolibri.Common.Utilities;
using LiteDB;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace SortPics.Forms
{
    public partial class MoviesForm : Form
    {
        private MALFileForm form;
        private OMDBController _OMDB;
        private LiteDBController _LITEDB;
        private TMDBController _TMDB;

        private List<Item> _currentList;

        private Dictionary<string, string> _unsearchableDic = new Dictionary<string, string>();

        public MoviesForm()
        {
            InitializeComponent();
            Init();
        }

        private void SetLabelText(string text)
        {
            toolStripStatusLabelFilnavn.TextAlign = ContentAlignment.MiddleLeft;
            toolStripStatusLabelFilnavn.Text = text;
            try
            {
                toolStripStatusLabelFilnavn.Parent.Update();
            }
            catch (Exception)
            {
            }
        }

        private void Init()
        {

            this.Text = $"Search for movies via OMDB ({Assembly.GetExecutingAssembly().GetName().Version.ToString()})";
            form = new MALFileForm();
            form.DestinationPath = form.SourcePath;
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.Visible = true;

            splitContainer1.Panel1.Controls.Add(form);
            try
            {
                _LITEDB = new LiteDBController(true, false);
                linkLabelLiteDB.Text = _LITEDB.ConnectionString.Filename;
                linkLabelLiteDB.Tag = linkLabelLiteDB.Text;
            }
            catch (Exception)
            {
                _LITEDB = new LiteDBController(false, false);
            }

            try
            {
                FlowLayoutPanel linkPanel = new FlowLayoutPanel();
                linkPanel.HorizontalScroll.Enabled = false;
                linkPanel.FlowDirection = FlowDirection.TopDown;
                linkPanel.Height = groupBoxMovieLinks.Height;
                linkPanel.Width = groupBoxMovieLinks.Width;
                foreach (var key in Kolibri.Common.Utilities.MovieUtilites.MovieLinksDic.Keys)
                {
                    LinkLabel link = new LinkLabel();
                    link.Width = 5000;
                    link.Text = key;
                    link.Tag = Kolibri.Common.Utilities.MovieUtilites.MovieLinksDic[key];
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                    linkPanel.Controls.Add(link);
                }
                groupBoxMovieLinks.Controls.Add(linkPanel);
            }
            catch (Exception)
            {

            }


            var omdbkey = OMDBController.GetOmdbKey(false);
            _OMDB = new OMDBController(omdbkey, _LITEDB);
            _TMDB = new TMDBController(_LITEDB);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SetLabelText($"starting search (DB): {checkBoxDB.Checked}");
                Application.DoEvents();
                if (checkBoxDB.Checked)
                {
                    SearchForMovies();
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
                var list = _LITEDB.FindAllFileItems(directoryInfo);
                if (list != null && list.Count() > 0)
                {
                    List<OMDbApiNet.Model.Item> omdbItems = new List<OMDbApiNet.Model.Item>();
                    foreach (var item in list)
                    {
                        try
                        {
                            omdbItems.Add(_LITEDB.FindItem(item.ImdbId));
                        }
                        catch (Exception) { }

                    }


                    ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(omdbItems.ToArray()));
                    ShowGridView(ds.Tables[0]);
                }
            }
        }

        private void SearchForMovies()
        {
            _currentList = new List<Item>();
            DataTable resultTable = null;

            List<string> common = Kolibri.Common.Utilities.FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = Kolibri.Common.Utilities.FileUtilities.GetFiles(form.SourcePath, searchStr, true);

            SetLabelText($"titles found: {list.Count()}");

            if (list.Count() < 1) return;
            var count = 0;
            foreach (var file in list)
            {
                if (file.Directory.Name.Contains(".@__thumb")) continue;
                count++;
                int year = Kolibri.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                if (year.Equals(0) || year.Equals(1))
                    year = Kolibri.Common.Utilities.MovieUtilites.GetYear(file.Name);

                string title = Kolibri.Common.Utilities.MovieUtilites.GetMovieTitle(Path.GetFileNameWithoutExtension(file.Name));

                SetLabelText($" ({count}/{list.Count()}) {title} - {year}");

                Item movie = GetItem(file, year, title);
                if (movie != null)
                { 

                    _currentList.Add(movie);
                    var tempTable = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { movie });

                    if (resultTable == null)
                    {
                        resultTable = tempTable.Tables[0];
                    }
                    else
                    {
                        resultTable.Merge(tempTable.Tables[0]);
                    }
                }
            }

            var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
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
                List<string> columns = new List<string>() { "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };


                temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
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
        }

        public Item GetItem(FileInfo file, int year, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            Item movie = null;
            var test = _LITEDB.FindByFileName(file);
            if (test != null)
            {
                movie = _LITEDB.FindItem(test.ImdbId);
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
                                    if (!_LITEDB.Insert(movie) && checkBoxFileName.Checked)
                                    {
                                        try
                                        {
                                            if (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl))
                                            {
                                                movie.TomatoUrl = file.FullName;
                                                _LITEDB.Update(movie);
                                                _LITEDB.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
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
            if (movie == null)
            {
                movie = _LITEDB.FindItemByTitle(title, year);

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
                                        _LITEDB.Insert(movie);
                                        _LITEDB.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
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
                if (movie == null && !_unsearchableDic.Keys.Contains(title))
                    movie = _OMDB.GetMovieByIMDBTitle(title, year);


                if (movie != null)
                {
                    if (string.IsNullOrEmpty(movie.TomatoUrl))
                        movie.TomatoUrl = file.FullName;
                    if (string.IsNullOrEmpty(movie.TomatoUrl))
                        movie.TomatoUrl = file.FullName;
                    if (!_LITEDB.Insert(movie) && checkBoxFileName.Checked)
                    {
                        _LITEDB.Update(movie);
                    };
                    _LITEDB.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
                    return movie;
                }
                else
                {
                    movie = new OMDbApiNet.Model.Item() { Title = title, Year = year.ToString(), ImdbRating = "Unknown", Response = "false", TomatoUrl = file.FullName };
                    if (!_unsearchableDic.Keys.Contains(title))
                        _unsearchableDic.Add(title, file.FullName);
                }
            }
            if (checkBoxFileName.Checked)
            {
                if (checkBoxFileName.Checked && movie != null && (string.IsNullOrEmpty(movie.TomatoUrl) || !File.Exists(movie.TomatoUrl)))
                {
                    try
                    {
                        movie.TomatoUrl = file.FullName;
                        _LITEDB.Update(movie);
                        _LITEDB.Upsert(new LiteDBController.FileItem(movie.ImdbId, file.FullName));
                    }
                    catch (Exception)
                    { }
                }
             
            }
            return movie;
        }

        private void ShowGridView(DataTable table)
        {
            try
            {
                List<string> visibleColumns = new List<string>(){ "Title"
            ,"ImdbRating"
            ,"Year"
            ,"Rated"
            ,"Runtime"
            ,"Genre"
            ,"Plot" };

                splitContainer2.Panel2.Controls.Clear();
                try
                {
                    DataGridView dgv = new DataGridView();
                    dgv.DataSource = table;
                    dgv.Dock = DockStyle.Fill;
                    splitContainer2.Panel2.Controls.Add(dgv);

                    dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => col.Visible = false);
                    // dataGridView1.Rows.OfType<DataGridViewRow>().ToList().ForEach(row => { if (!row.IsNewRow) row.Visible = false; });
                    dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => { if (visibleColumns.Contains(col.Name)) col.Visible = true; });
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgv.AllowUserToResizeColumns = true;
                    dgv.SelectionChanged += new EventHandler(DataGridView_SelectionChanged);
                    dgv.CellContentDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellContentDoubleClick);
                    dgv.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(DataGridView_RowPrePaint);
                    dgv.KeyDown += Dgv_KeyDown;
                    dgv.CellValueChanged += Dgv_CellValueChanged;
                    dgv.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(dgv_CellMouseDown);

                    try
                    {
                        dgv.Columns["Title"].DisplayIndex = 0; // or 1, 2, 3 etc dersom dgv ikke er added to panel 2 funker ikke dette 
                        dgv.Columns["Title"].Width = 150;
                        dgv.Columns["ImdbRating"].DisplayIndex = 1; // or 1, 2, 3 etc

                        dgv.Sort(dgv.Columns["ImdbRating"], ListSortDirection.Descending);
                        DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                        lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        SetLabelText($"{dgv.Rows.Count} rader.");
                    }
                    catch (Exception) { }
                }
                catch (Exception)
                { }
            }
            catch (Exception)
            { }
        }

        private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                var columnName = dgv.Columns[e.ColumnIndex].Name;
                if (columnName == "ImdbRating")
                {
                    var newValue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    var old = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView)[e.ColumnIndex];//dgv.CurrentCell.Value;  
                    if (newValue != old)
                    {

                        var id = dgv.Rows[e.RowIndex].Cells["ImdbId"].Value;
                        var movie = _LITEDB.FindItem(id.ToString());
                        movie.ImdbRating = newValue.ToString().Replace(",", ".");
                        _LITEDB.Upsert(movie);

                    }
                }
            }
            catch (Exception) { }
        }

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewRow row = (sender as DataGridView).Rows[e.RowIndex];
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];

                OMDbApiNet.Model.Item nm = null;
                if (c.Selected)
                {
                    var title = $"{(row.DataBoundItem as DataRowView)["Title"]}";//dgv.CurrentCell.Value;  
                    string year = $"{(row.DataBoundItem as DataRowView)["Year"]}";
                    if (Kolibri.Common.FormUtilities.InputDialogs.InputBox("Filmsøk", "Søk etter film", ref title) == DialogResult.OK)
                    {
                        var liste = _OMDB.GetByTitle(title, OMDbApiNet.OmdbType.Movie, 2);

                        if (liste != null && liste.Count == 1)
                        {
                            nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));
                        }
                        else
                        {
                            var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                            List<SearchMovie> tLibList = t.Result;
                            if (tLibList != null && tLibList.Count == 1)
                            {

                                Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                                nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                if (nm.ImdbRating == "N/A")
                                {
                                    nm.ImdbRating = $"{tmdbMovie.VoteAverage}".Replace(",", ".");
                                }
                            }
                            else if (nm == null && liste != null)
                            {
                                object ttid = string.Empty;
                                DataSet ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));
                                if (Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                                {
                                    DataTable table = new DataView(ds.Tables[0].Copy(), $"ImdbId='{(ttid as ListViewItem).SubItems[2].Text}'", "", DataViewRowState.CurrentRows).ToTable();
                                    nm = _OMDB.GetMovieByIMDBTitle(table.Rows[0]["Title"].ToString(), Convert.ToInt32(table.Rows[0]["Year"]));
                                }
                            }
                        }
                        if (nm == null)
                        {

                            var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                            List<SearchMovie> tLibList = t.Result;
                            if (tLibList != null && tLibList.Count() > 0)
                            {
                                try
                                {
                                    object tmdbId = string.Empty;
                                    DataSet ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererDataSet(new ArrayList(tLibList));
                                    if (Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Choose corect Movie (TMDB)", "Set the correct value",
                                        new DataView(ds.Tables[0].Copy(), $"", "", DataViewRowState.CurrentRows).ToTable(true, "Id", "Title", "VoteAverage", "OriginalTitle", "ReleaseDate", "OriginalLanguage", "Overview")
                                        , ref tmdbId) == DialogResult.OK)
                                    {

                                        Movie tmdbMovie = _TMDB.GetMovie(Convert.ToInt32((tmdbId as ListViewItem).SubItems[0].Text));
                                        // DataTable table = new DataView(ds.Tables[0].Copy(), $"Id='{(ttid as ListViewItem).SubItems[0].Text}'", "", DataViewRowState.CurrentRows).ToTable(true, "Id"," Title"," VoteAverage","  OriginalTitle"," ReleaseDate"," OriginalLanguange"," Overview");
                                        if (tmdbMovie != null && !string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                            nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                                        liste = new List<OMDbApiNet.Model.SearchItem>();
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                            }

                            if (nm == null)
                            {
                                if (Kolibri.Common.FormUtilities.InputDialogs.Generic2ValuesDialog("Not found. Put exact year", "", ref title, ref year, "Tittel", "Utgivelsesår") == DialogResult.OK)
                                {
                                    liste = _OMDB.GetByTitle(title, Convert.ToInt32(year), OMDbApiNet.OmdbType.Movie, 2);
                                    if (liste != null)
                                    {
                                        object ttid = string.Empty;
                                        DataSet ds = Kolibri.Common.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));


                                        if (Kolibri.Common.FormUtilities.InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                                        {
                                        }
                                    }
                                    if (liste == null)
                                    {
                                        nm = _OMDB.GetMovieByIMDBTitle(title.ToString(), Convert.ToInt32(year));
                                    }

                                    else
                                    {
                                        nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));

                                    }

                                    liste = new List<OMDbApiNet.Model.SearchItem>();
                                }
                                if (liste == null) liste = new List<OMDbApiNet.Model.SearchItem>();
                            }
                        }
                        if (nm == null)
                        {
                            var test = _TMDB.FetchMovie(title, Convert.ToInt32(year));

                            if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                            {
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = "chrome.exe"; // @"""C:\Program Files (x86)\Google\Chrome\Application\chrome.exe""";
                                startInfo.Arguments = $@"https://www.imdb.com/find?q={title.Replace(" ", "+")}&ref_=nv_sr_sm";
                                Process.Start(startInfo);
                            }
                        }
                        else if (nm != null)
                        {

                            if (liste == null)
                                liste = new List<OMDbApiNet.Model.SearchItem>();

                            {
                                DialogResult res =
                                                 MessageBox.Show($"{liste.Count} move(s) were found:\r\n\r\nTitle: {nm.Title}\r\nImdbRating: {nm.ImdbRating}\r\nYear: {nm.Year}\r\nActors: {nm.Actors}\r\nPlot :{nm.Plot}", "Is this movie correct?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    (row.DataBoundItem as DataRowView)["Title"] = nm.Title;
                                    (row.DataBoundItem as DataRowView)["Year"] = Convert.ToInt32(nm.Year);
                                    (row.DataBoundItem as DataRowView)["ImdbId"] = nm.ImdbId;
                                    (row.DataBoundItem as DataRowView).Row["ImdbId"] = nm.ImdbId;

                                    (row.DataBoundItem as DataRowView).Row["ImdbRating"] = nm.ImdbRating;
                                    (row.DataBoundItem as DataRowView).Row["Genre"] = nm.Genre;
                                    (row.DataBoundItem as DataRowView).Row["Plot"] = nm.Plot;
                                    (row.DataBoundItem as DataRowView).Row["Runtime"] = nm.Runtime;
                                    (row.DataBoundItem as DataRowView).Row["Rated"] = nm.Rated;

                                    row.DataGridView.EndEdit();
                                    row.DataGridView.Refresh();

                                    _LITEDB.Insert(nm);
                                    _LITEDB.Upsert(new LiteDBController.FileItem(nm.ImdbId, $"{(row.DataBoundItem as DataRowView)["TomatoUrl"]}"));
                                    Form form = new DetailsFormItem(nm, _LITEDB);
                                    form.ShowDialog();
                                }
                                else if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start("http://imdb.com");
                                }
                            }
                        }
                    }
                }
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
                            var fileitem = _LITEDB.FindFile(id);
                            if (fileitem != null)
                            {
                                _LITEDB.DeleteItem(id);

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
            try
            {
                DataGridView DataGridView1 = sender as DataGridView;

                if (DataGridView1.SelectedRows.Count > 0)
                    index = DataGridView1.SelectedRows[0].Index;
                else
                    try
                    { index = DataGridView1.CurrentRow.Index; }
                    catch (Exception) { }

                if (DataGridView1.Rows[index].Cells["Title"].Value
                          != null)
                {
                    if (DataGridView1.Rows[index].
                        Cells["Title"].Value.ToString().Length != 0)
                    {
                        title = DataGridView1.Rows[index].Cells["Title"].Value.ToString();

                        year = int.Parse(DataGridView1.Rows[index].Cells["Year"].Value.ToString());
                    }
                }

                SetLabelText($"{title} - {year}");


                // Execute some stored procedure for row updates here  
                var movie = _LITEDB.FindItemByTitle(title, year);
                if (movie == null)
                {
                    movie = _OMDB.GetMovieByIMDBTitle(title, year);
                }

                if (movie == null)
                {
                    movie = new OMDbApiNet.Model.Item() { Title = title, Year = $"{year}", ImdbRating = "unknown" };
                }

                if (movie != null)
                {
                    Form form = new DetailsFormItem(movie, _LITEDB);
                    SetForm(form);
                }
            }

            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }

        private void DataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {if (!this.Visible) return;
            if (e.State != DataGridViewElementStates.Visible) return;
            try
            {
                DataGridView dgv = sender as DataGridView;
                
                string imdbid = $"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";

                if (string.IsNullOrEmpty(imdbid))
                {
                    if (dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Beige)
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
                    }
                }               
                else
                {
                    LiteDBController.FileItem info = _LITEDB.FindFile(imdbid);
                    if (info != null)
                    {
                        string ext = Path.GetExtension(info.FullName).ToLower();
                        FileInfo srtFile = new FileInfo(info.FullName.Replace(ext, ".srt"));
                        DirectoryInfo dInfo = new DirectoryInfo(Path.Combine(srtFile.Directory.FullName, "Subs"));
                        if (!srtFile.Exists && !dInfo.Exists)
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
                        if (!srtFile.Exists && !dInfo.Exists)
                        {
                            dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;

                        }
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
                try
                {
                    if ($"{dgv.Rows[e.RowIndex - 1].Cells["ImdbId"].Value}".Equals($"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}"))
                    { 
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Salmon;
                    }
                }
                catch (Exception ex)
                {
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

                string imdbid = $"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";


                if (!string.IsNullOrEmpty(imdbid))
                {
                    LiteDBController.FileItem info = _LITEDB.FindFile(imdbid);
                    if (info != null)
                    {
                        FileInfo file = new FileInfo(info.FullName);
                        Kolibri.Common.Utilities.FileUtilities.OpenFolderMarkFile(file);
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

        private void SetForm(Form form)
        {
            panelDetails.Controls.Clear();
            try
            {
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;

                panelDetails.Controls.Add(form);
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
                if (_currentList != null&& _currentList.Count >=1) { ShowFolderPathFromDataBase(form.SourcePath, _currentList); }
        
                else { ShowFolderPathFromDataBase(form.SourcePath); }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);

            }

        }
        private void ShowFolderPathFromDataBase(DirectoryInfo directoryInfo, List<Item> itemList = null)
        {
            IEnumerable<LiteDBController.FileItem> list;
            if (itemList == null) { list = _LITEDB.FindAllFileItems(directoryInfo); }
            else {
                var fileList = new List<LiteDBController.FileItem>();   
                foreach (var item in itemList)
                {
                    var test = _LITEDB.FindFile(item.ImdbId);  
                    if(test != null)
                        fileList.Add(test);
                }
                list = fileList.ToArray();
            }

            if (list.Count() > 0)
            {
                try
                {
                    List<LiteDBController.FileItem> SortedList = list.OrderByDescending(o => o.FullName).ToList();
                    list = SortedList;
                    if (checkBoxHTML.Checked)
                    {
                        string html = HTMLTemplate(list, directoryInfo.Name);
                        HTMLUtilities.ShowHTML(html, directoryInfo);
                    }
                    if (checkBoxDynamicHTML.Checked)
                    {
                        var htmlFile = DynamicHTML(list);
                        HTMLUtilities.OpenURLInBrowser(new Uri(htmlFile.FullName));
                      

                    }
                }
                catch (Exception sortEx) { }
            }
        }


        private void DisplayDatabase()
        {
            IEnumerable<LiteDBController.FileItem> list;

            list = _LITEDB.FindAllFileItems();
            if (list.Count() > 0)
            {
                try
                {

                    List<LiteDBController.FileItem> SortedList = list.OrderByDescending(o => o.FullName).ToList();
                    list = SortedList;
                }
                catch (Exception sortEx) { }
            }



            DataTable resultTable = null;

            foreach (var fileItem in list)
            {
                OMDbApiNet.Model.Item movie = _LITEDB.FindItem(fileItem.ImdbId);
                if (movie != null)
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


        private FileInfo DynamicHTML(IEnumerable<LiteDBController.FileItem> items)
        { 
            List<Movie> Movies = new List<Movie>();

            foreach (var item in items)
            {
                Movie movie = _TMDB.GetMovie(item.ImdbId.ToString(), true);
                if (movie == null) continue;
                Movies.Add(movie);
            }
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(Kolibri.Common.Utilities.FolderUtilities.GetCurrentDirectory().FullName, "HTML"));

            var movieArray = JsonConvert.SerializeObject(Movies);
            string json = $"{{\"results\":{movieArray}}}";
            if (json.Contains(@"\""")) { json = json.Replace(@"\""", "'"); }

            var info = new FileInfo(Path.Combine(dest.FullName, "movies.json"));
            File.WriteAllText(info.FullName,json);

            string insert = $@"let text = `{json}`;";
            info = new FileInfo(Path.Combine(dest.FullName, "script_template.js"));
            var script = FileUtilities.ReadTextFile(info.FullName).Insert(0, insert);
            info = new FileInfo(Path.Combine(dest.FullName, "script.js"));
            File.WriteAllText(info.FullName, script);

            var ret = new FileInfo(   Path.Combine(Kolibri.Common.Utilities.FolderUtilities.GetCurrentDirectory().FullName, "HTML", "index.html"));

            return ret;
        }

        /// <summary>
        /// http://bl.ocks.org/CrandellWS/2e7d918cbae163ca9c1b
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string HTMLTemplate(IEnumerable<LiteDBController.FileItem> items, string title = "Movie Website")
        {
            List<Item> SortedList = new List<Item>();

            foreach (var item in items)
            {
                OMDbApiNet.Model.Item movie = _LITEDB.FindItem(item.ImdbId);
                if (movie == null) continue;
                SortedList.Add(movie);
            }
   

            SortedList = SortedList.OrderByDescending(o => o.ImdbRating).ToList(); 

            //   string HTML = File.ReadAllText("TMP_OMDBTemplateFile.html");
            string HTML = File.ReadAllText("TMP_OMDBTableFile.html");
            HTML = HTML.Replace("<title>Movie Website</title>", $"<title>{title}</title>");


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
            OMDbApiNet.Model.Item movie = _LITEDB.FindItem(item.ImdbId);
            if (movie == null) return string.Empty;

            link = $@"<a href=""FILE://{Path.GetDirectoryName(item.FullName)}"">
         <img alt=""{movie.Title}"" title=""{movie.Title} ({movie.Year})"" Qries=""{movie.Director}"" src=""{movie.Poster}""
         width=100"" height=""30"">";
            return link;
        }
        private string GetTableData(LiteDBController.FileItem item)
        {//  <!-- <td>{link}</td> -->

            OMDbApiNet.Model.Item movie = _LITEDB.FindItem(item.ImdbId);
            if (movie == null) return string.Empty;
            return GetTableData(movie);
        }


        private string GetTableData(Item movie)
        {
            if (movie == null) return string.Empty;
            string link = $@"<a href=""FILE://{Path.GetDirectoryName(movie.TomatoUrl)}"">link</a>";
            string image = $@"<img alt=""{movie.Title}"" title=""{movie.Title} ({movie.Year})"" Qries=""{movie.TomatoUrl}"" src=""{movie.Poster}"" width=""70"" height=""100""> ";
            string data = $@" <tr>
                <td align=""left"" valign=""top"" width=""3%"" >{image}</td>              
                <td align=""left""  width=""3%"">{movie.Title}</td>
                <td align=""left""  width=""3%"">{movie.ImdbRating}</td>
                <td align=""left""  width=""3%"">{movie.Year}</td>
                <td align=""left""  width=""15%"">{movie.Plot}</td>
            <tr>";
            return data;
        }

        private void OMDBForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _LITEDB.Dispose();
        }

        private void buttonSubtitleSearch_Click(object sender, EventArgs e)
        {
            var list = _LITEDB.FindAllFileItems(form.SourcePath);
            SetLabelText($"{list.Count()} element found at {form.SourcePath} ");
            if (list != null && list.Count() > 0)
            { 
                foreach (var item in list)
                {

                    FileInfo info = new FileInfo(item.FullName);
                    SetLabelText($"Searching for {info.Name}");

                    FileInfo srtInfo = new FileInfo(Path.ChangeExtension(item.FullName, ".srt"));
                    bool dirExists = Directory.Exists(Path.Combine(info.Directory.FullName, "Subs"));
                    var mmi = _OMDB.GetItemByImdbId(item.ImdbId);
                    dirExists = dirExists && mmi.Type == "movie";
                    try
                    {
                        if (info.Exists && !dirExists)
                        {
                            if (srtInfo.Exists) { continue; }

                            var jall = Kolibri.Common.MovieAPI.Controller.SubDLSubtitleController.SearchByIMDBid(item.ImdbId);
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
                    catch (Exception ex) { var jall = item.ToString(); }

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

                foreach (var movie in _LITEDB.FindAllItems())
                {

                    if (!string.IsNullOrEmpty(movie.TomatoUrl))
                    {
                        try
                        {
                            FileInfo info = new FileInfo(movie.TomatoUrl);
                            if (!info.Exists)
                            {
                                _LITEDB.DeleteItem(movie.ImdbId);
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

                        Process.Start($"{s.Tag}");
                    }
                }
                catch (Exception) { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
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
                    System.Windows.Forms.ListView.SelectedListViewItemCollection collection = refereres as System.Windows.Forms.ListView.SelectedListViewItemCollection;
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
                            var url = $"{row["Poster"]}";
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
    }
}