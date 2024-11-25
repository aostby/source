using System.Configuration;
 
using Kolibri.Common.Utilities;
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Threading;
 
using Kolibri.Common.MovieAPI.Controller;

namespace SortPics.Forms
{
    public partial class LiteDBMovieForm : Form
    {
    

        private OMDBController _OMDB;
        private LiteDBController _LITEDB;
        private TMDBController _TMDB;
     
      //private Dictionary<string, Bitmap> _imageCache = new Dictionary<string, Bitmap>();


        private List<string> _genres = new List<string>();

        private ImageCache _imageCache;


        private DataTable _resultTable;
        private FileInfo _htmlFile;
        private FileInfo _pdfFile = GetPDFFile();
  //      private string _imagespath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\images.xml";

        private static FileInfo GetPDFFile()
        {
            var pdfFile = new FileInfo(@"c:\temp\html.pdf");
            try { if (pdfFile.Exists) pdfFile.Delete(); } catch (Exception) { }
            try { if (!pdfFile.Directory.Exists) pdfFile.Directory.Create(); } catch (Exception) { }
            pdfFile.Refresh();
            return pdfFile;
        }

  
        public LiteDBMovieForm()
        {
            InitializeComponent();
            Init();
        }

        private void SetLabelText(string text)
        {

            toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft;
            toolStripStatusLabel1.Text = text;
            try
            {
                toolStripStatusLabel1.GetCurrentParent().Update();
            }
            catch (Exception)
            { }
        }

        private void Init()
        {
            buttonDisplayCurrentHTML.Enabled = false;
            buttonPrint.Enabled = buttonDisplayCurrentHTML.Enabled;
            SetLabelText($"Init... klikk {buttonVis.Text}");

            this.Text = $"Search for movies via local db and OMDB ({Assembly.GetExecutingAssembly().GetName().Version.ToString()})";

            _OMDB = new OMDBController(OMDBController.GetOmdbKey());
            _LITEDB = new LiteDBController(false, true);
            _TMDB = new TMDBController(_LITEDB);

            _imageCache = new ImageCache(_LITEDB, true, "SortPics"); 

            _htmlFile = new FileInfo(@"c:\temp\html.html");
            try { if (_htmlFile.Exists) _htmlFile.Delete(); } catch (Exception) { }
            #region imageCache local
            //try
            //{
            //    if (File.Exists(_imagespath))
            //    {
            //        DataSet ds = new DataSet();
            //        ds.ReadXml(_imagespath);
            //        foreach (DataRow row in ds.Tables[0].Rows)
            //        {
            //            try
            //            {
            //                var jall = Kolibri.Utilities.ImageUtilities.Base64ToImage(row[1].ToString());
            //                var test = (Bitmap)jall;
            //                _imageCache[row[0].ToString()] = test;
            //            }
            //            catch (Exception ex)
            //            {
            //                SetLabelText(ex.Message);
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex) { SetLabelText($"Could not read {_imagespath}: {ex.Message}"); }
            #endregion

            var task = Task.Run(async () => await InitGenres());
            if (!task.Result)
                task = Task.Run(async () => await InitGenresDB());

            if (comboBoxGenre.Items.Count <= 0)
                comboBoxGenre.Items.AddRange(_genres.ToArray());

    //        Task.Run(async () => await InitImages());
        }
        public async Task<bool> InitGenres()
        {
            bool ret = true;
            try
            {
                List<string> liste = _TMDB.GetGenreList().Select(x => x.Name).ToList();

                liste.Insert(0, "--ALL--");
                liste = liste.Distinct().Select(s => s.Trim()).ToList();
                try
                {
                    var newList = liste.Select(x => x.Replace("Science Fiction", "Sci-Fi")).ToList();
                    liste = newList;
                }
                catch (Exception)
                { }

                _genres = liste;
            }
            catch (Exception ex)
            { ret = false; }
            return ret;
        }

        public async Task<bool> InitGenresDB()
        {
            bool ret = true;
            try
            {
                var items = _LITEDB.FindAllItems();
                //           var urls = items.Select(x => x.Poster).ToList();
                var genre = items.Select(x => x.Genre).ToList();
                string thesht = string.Empty; try
                {

                    for (int i = 0; i < genre.Count; i++)
                    {
                        string text = genre[i].ToString() + ",";
                        thesht += text;
                    }
                }
                catch (Exception) { }

                List<string> liste = new List<string>();
                liste.AddRange(thesht.Split(',').ToArray());
                liste.Insert(0, "--ALL--");
                liste = liste.Distinct().Select(s => s.Trim()).ToList();
                _genres = liste;

            }
            catch (Exception ex)
            { }
            return ret;
        }

     

        private void ShowDataBase(DirectoryInfo directoryInfo = null)
        {
            try
            {

                IEnumerable<LiteDBController.FileItem> list;

                if (_resultTable == null)
                {
                    list = _LITEDB.FindAllFileItems();
                    if (list.Count() == 0)
                    {
                        MessageBox.Show("No moives were found!", "Database seems empty");
                        return;
                    }
                    DataTable resultTable = null;

                    foreach (var fileItem in list)
                    {
                        OMDbApiNet.Model.Item movie = _LITEDB.FindItem(fileItem.ImdbId);
                        if (movie != null)
                        {
                            var tempTable =  DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { movie });
                            if (resultTable == null) { resultTable = tempTable.Tables[0]; }
                            else { resultTable.Merge(tempTable.Tables[0]); }

                            //var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                            //resultTable = temp;
                            //resultTable.TableName = "KolibriMovies"; //Kolibri.Utilities.ExcelUtilities.LegalSheetName((form.SourcePath.Name));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));

                        }
                    }
                    if (resultTable.DataSet == null)
                    {
                        DataSet ds = new DataSet();
                        ds.Tables.Add(resultTable);
                    }
                    try
                    {
                        var temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable();
                        resultTable = temp;
                        resultTable.TableName = "KolibriMovies"; //Kolibri.Utilities.ExcelUtilities.LegalSheetName((form.SourcePath.Name));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));


                    }
                    catch (Exception)
                    { }


                    _resultTable = resultTable;
                    SetLabelText($"{list.Count()} filmer funnet. Lesing fra db fullført");
                }
                SetLabelText("Starter visning av data");
                ShowGridView(_resultTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void DataSetToExcel(DataSet ds)
        {
            List<string> columns = new List<string>() { "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };
            DataSetToExcel(ds, columns);
        }


        private void DataSetToExcel(DataSet ds, List<string> columns)
        {
            DirectoryInfo DestinationPath = Kolibri.Common.Utilities.FileUtilities.LetOppMappe();
            if (DestinationPath == null) return;

            DataTable resultTable = ds.Tables[0];

            DataTable temp = new DataView(resultTable, "", "ImdbRating desc, Title ASC", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
            if (temp.DataSet == null)
            {
                ds = new DataSet();
                ds.Tables.Add(temp);
            }
            string filePath = Path.Combine(DestinationPath.FullName, resultTable.TableName + ".xlsx");
            try { if (File.Exists(filePath)) File.Delete(filePath); } catch (Exception) { }

            Kolibri.Common.Utilities.ExcelUtilities.GenerateExcel2007(new FileInfo(filePath), temp.DataSet);
            Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(filePath));
        }


        private void ShowGridView(DataTable table)
        {
            List<string> visibleColumns = new List<string>(){ "Title"
            ,"ImdbRating"
            ,"Year"
            ,"Rated"
            ,"Runtime"
            ,"Genre"
            ,"Plot"
            //,"Image"
            };

            try
            {
                splitContainer2.Panel2.Controls.Clear();
                if (visibleColumns.Contains("Image"))
                {
                    if (!table.Columns.Contains("Image"))
                    {
                        table.Columns.Add("Image", typeof(Image)); }
                }

                    DataGridView dgv = new DataGridView();           
                dgv.DataSource = null;
                dgv.DataSource = table;
                dgv.ClearSelection();
                dgv.Refresh();
                
                //dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);

                //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllHeaders;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgv.AllowUserToOrderColumns = true;
                dgv.AllowUserToResizeColumns = true;

                dgv.Dock = DockStyle.Fill;
                splitContainer2.Panel2.Controls.Add(dgv);

                dgv.Visible = false;

                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => col.Visible = false);
                // dataGridView1.Rows.OfType<DataGridViewRow>().ToList().ForEach(row => { if (!row.IsNewRow) row.Visible = false; });
                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => { if (visibleColumns.Contains(col.Name)) col.Visible = true; });
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                try
                {
                    if (dgv.Columns.Contains("Image"))
                        dgv.Columns["Image"].DisplayIndex = 0; // or 1, 2, 3 etc 
                    dgv.Columns["Title"].DisplayIndex = 1; // or 1, 2, 3 etc
                    dgv.Columns["Title"].Width = 150;
                    dgv.Columns["ImdbRating"].DisplayIndex = 2; // or 1, 2, 3 etc

                    dgv.Sort(dgv.Columns["ImdbRating"], ListSortDirection.Descending);
                    DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                    lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (dgv.Columns.Contains("Plot"))
                    {
                        dgv.Columns["Plot"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dgv.Columns["Plot"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
               

                }
                catch (Exception ex) { }
              


                /********************************/
                if (dgv.Columns.Contains("Image"))
                    ((DataGridViewImageColumn)dgv.Columns["Image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                panelDetails.Controls.Clear();
                foreach (DataGridViewRow item in dgv.Rows)
                {
                    try
                    {  
                        {
                            if ((item.DataBoundItem as System.Data.DataRowView) == null) continue;

                            {
                                item.Visible = true;
                                DataRow row = ((System.Data.DataRowView)item.DataBoundItem).Row;
                                try
                                {
                                    if (dgv.Columns.Contains("Image"))
                                    {
                                        if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(row["ImdbId"].ToString()))
                                        {
                                            var jall = (Bitmap)_imageCache.RetrieveImage(row["ImdbId"].ToString());

                                            row["Image"] = jall;
                                        }

                                        if (row["Image"] as Bitmap != null)
                                        {
                                            //try
                                            //{
                                            //    item.Height = (int)((Bitmap)_imageCache.RetrieveImage(row["ImdbId"].ToString())).Height / 3;
                                            //}
                                            //catch (Exception)
                                            //{ }
                                        }
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                /******************************/
                
               
                try
                {
                    if (comboBoxGenre.Items.Count <= 0)
                    {
                        string thesht = string.Empty;
                        if (dgv.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgv.Rows.Count - 1; i++)
                            {
                                string text = dgv.Rows[i].Cells["Genre"].Value.ToString() + ",";
                                thesht += text;
                            }
                        }

                        List<string> liste = new List<string>();
                        liste.AddRange(thesht.Split(',').ToArray());
                        liste.Insert(0, "--ALL--");
                        liste = liste.Distinct().Select(s => s.Trim()).ToList();
                        comboBoxGenre.Items.Clear();
                        comboBoxGenre.Items.AddRange(liste.ToArray());
                    }
                }
                catch (Exception)
                { }
                dgv.SelectionChanged += new EventHandler(DataGridView1_SelectionChanged);
                dgv.Visible = true;
            }
            catch (Exception ex)
            { }


        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
           
            try
            { panelDetails.Visible = false;
            string title = null; int year = 0; int index = 0;
                DataGridView DataGridView1 = sender as DataGridView;

                if (DataGridView1.SelectedRows.Count > 0)
                {
                    index = DataGridView1.SelectedRows[0].Index;
                }
                else
                    try
                    {
                        if (DataGridView1.SelectedRows.Count >= 1)
                            index = DataGridView1.CurrentRow.Index;
                    }
                    catch (Exception) { }

                if (DataGridView1.Rows[index].Cells["Title"].Value != null)
                {
                    if (DataGridView1.Rows[index].Cells["Title"].Value.ToString().Length != 0) 
                    {
                        title = DataGridView1.Rows[index].Cells["Title"].Value.ToString();

                        if (!int.TryParse(DataGridView1.Rows[index].Cells["Year"].Value.ToString(), out year)) 
                        {
                            try
                            {//Bare ta de 4 første, hvis ikke får året bare bli null
                                int.TryParse(DataGridView1.Rows[index].Cells["Year"].Value.ToString().Substring(0, 4), out year);
                            }
                            catch (Exception)
                            { }
                        }

                        try
                        {
                            if (DataGridView1.Columns.Contains("Image"))
                            {
                                var jall = _imageCache.FindImage(DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString());
                                if (jall != null && jall.Image != null)
                                {
                                    DataGridView1.Rows[index].Cells["Image"].Value = jall.Image;
                                }
                            }

                        }
                        catch (Exception)
                        { }

                    }
                }

                SetLabelText($"{title} - {year}");


                // Execute some stored procedure for row updates here  
                var movie = _LITEDB.FindItemByTitle(title, year);
                if (movie == null)
                {
                    movie = _OMDB.GetMovieByIMDBTitle(title, year);
                    if (movie == null)
                    {
                        movie = new OMDbApiNet.Model.Item() { Title = title, Year = $"{year}" };
                    }
                }

                if (movie != null)
                {
                    Form form = new DetailsFormItem(movie, _LITEDB);
                    SetForm(form);

                    try
                    {
                        if (!_imageCache.CacheImageExists(movie.ImdbId))
                        {
                            var dic = new Dictionary<string, string>() { { movie.ImdbId, movie.Poster } };
                            var task = Task.Run(async () => await (_imageCache.GetImagesAsync(dic)));
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            }

            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            } 
            try
            {
                panelDetails.Visible = true;

            }
            catch (Exception ex)
            {


            }

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
                //nullstill allerede hentede data
                _resultTable = null;

                SetLabelText($"Starter lesing fra db ({_LITEDB.ConnectionString.Filename})");

                if (comboBoxGenre.Items.Count <= 0)
                    comboBoxGenre.Items.AddRange(_genres.ToArray());
                buttonVis.Enabled = false;

                ShowDataBase();

                buttonVis.Enabled = true;
                if (_resultTable != null)
                {
                    SetLabelText($"Fullført - lesning fra db, initialiserer bilder... {_resultTable.Rows.Count}");
                    buttonVis.Text = $"Last filmer fra lokal database ({_resultTable.Rows.Count})";
                }
                buttonDisplayCurrentHTML.Enabled = true;
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }
        private void LiteDBMovieForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try {
                _imageCache.Save();
                _imageCache.Dispose();
            }

            catch (Exception ex)
            {  
            } 
            _LITEDB.Dispose();
            Thread.Sleep(5);

        }

        private void LiteDBMovieForm_FormClosing_old(object sender, FormClosingEventArgs e)
        {
            //string errors = string.Empty;
            //string SigBase64 = null;
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //try
            //{
            //    SigBase64 = null;
            //    var list = _imageCache.Keys.ToList();
            //    foreach (var item in list)
            //    {
            //        try
            //        {
            //            byte[] byteImage = null;
            //            try
            //            {
            //                var TEST = _imageCache[item];
            //                byteImage = ImageUtilities.BitmapToBytes((Bitmap)TEST.Clone());
            //            }
            //            catch (Exception ex)
            //            {
            //            }

            //            if (byteImage != null && byteImage.Length > 0)
            //            {
            //                try
            //                {

            //                    SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
            //                    dic[item] = SigBase64;

            //                }
            //                catch (Exception ex)
            //                {
            //                }

            //            }
            //            else
            //            {
            //                var NULL = ("AKSLFAØSFJ");
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            errors += ex.Message;
            //        }
            //    }

            //    if (dic.Count() > 0)
            //    {
            //        DataSet ds = new DataSet("imageCache");
            //        var datatable = dic.ToDataTable();
            //        if (datatable.DataSet == null)
            //        {
            //            datatable.TableName = "Images";

            //            ds.Tables.Add(datatable);
            //        }
            //        ds.WriteXml(_imagespath);
            //    }

            //}

            //catch (Exception ex)
            //{
            //    errors += ex.Message;
            //}
            //_LITEDB.Dispose();
            //Thread.Sleep(5);

        }

        private void comboBoxGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBoxGenre.Visible = false;
                if (_resultTable == null)
                    buttonVis.PerformClick();
                string searchvalue = comboBoxGenre.Text;
                SearchBy("Genre", searchvalue); comboBoxGenre.Visible = true;
            }
            catch (Exception ex)
            {  MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void SearchBy(string typeSearch, string searchValue)
        {

            try
            {
                if (!this.Visible) return;
                if (string.IsNullOrEmpty(searchValue))
                {
                    SetLabelText($"{typeSearch} Cannot search for nothing - {searchValue}"); return;
                }

                var jall = splitContainer2.Panel2.Controls[0] as DataGridView;
                if (jall == null)
                {
                    ShowGridView(_resultTable);
                    jall = splitContainer2.Panel2.Controls[0] as DataGridView;
                }

                if (jall.Columns.Contains("Image"))
                    ((DataGridViewImageColumn)jall.Columns["Image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                panelDetails.Controls.Clear();
                foreach (DataGridViewRow item in jall.Rows)
                {
                    try
                    {
                        if (searchValue.StartsWith("--"))
                            item.Visible = true;
                        else
                        {
                            if ((item.DataBoundItem as System.Data.DataRowView) == null) continue;

                            string jupp = $"{((item.DataBoundItem as System.Data.DataRowView)).Row[typeSearch]}";
                            string genre = $"{((item.DataBoundItem as System.Data.DataRowView)).Row["Genre"]}";

                            if (typeSearch.Equals("ImdbRating"))
                            {
                                try
                                {
                                    if (comboBoxGenre.Text.StartsWith("--"))
                                    {
                                        if (jupp.StartsWith("N/A")) jupp = "10.0";
                                        item.Visible = Convert.ToDecimal(jupp.Replace(".", ",")) > Convert.ToDecimal(searchValue.Replace(".", ","));
                                    }
                                    else
                                    {
                                        if (jupp.StartsWith("N/A")) jupp = "10.0";
                                        item.Visible = Convert.ToDecimal(jupp.Replace(".", ",")) > Convert.ToDecimal(searchValue.Replace(".", ",")) &&
                                           (comboBoxGenre.Text.IndexOf(genre, 0, StringComparison.CurrentCultureIgnoreCase) != -1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        item.Visible = Convert.ToDecimal(jupp) > Convert.ToDecimal(searchValue);
                                    }
                                    catch (Exception comma)
                                    {
                                        item.Visible = false;
                                    }
                                }

                            }
                            else if (jupp.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) < 0)
                                item.Visible = false;

                            else
                            {
                                item.Visible = true;
                                DataRow row = ((System.Data.DataRowView)item.DataBoundItem).Row;
                                try
                                {
                                    if (row.Table.Columns.Contains("Image"))
                                    {
                                        if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(row["Poster"].ToString()))
                                        {
                                            row["Image"] = (Bitmap)_imageCache.RetrieveImage(row["Poster"].ToString());
                                        }
                                        if (row["Image"] as Bitmap != null)
                                        {
                                            try
                                            {
                                                item.Height = (int)((Bitmap)_imageCache.RetrieveImage(row["Poster"].ToString())).Height / 3;

                                            }
                                            catch (Exception)
                                            { }
                                        }
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SetLabelText($"DB with fitler {typeSearch} by {searchValue} error {ex.Message}");
                    }
                }
                SetLabelText($"DB with fitler {typeSearch} by {searchValue} contains {jall.Rows.GetRowCount(DataGridViewElementStates.Visible)} out of {jall.Rows.Count} rows");
            }
            catch (Exception ex)
            { }
        }


        private void buttonTitle_Click(object sender, EventArgs e)
        {
            try
            {

 
            if (_resultTable == null)
                buttonVis.PerformClick();
            string searchvalue = textBoxTitle.Text;

            SearchBy("Title", searchvalue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.KeyCode == Keys.Enter)
                {

                    if (sender.Equals(textBoxTitle)) buttonTitle.PerformClick();
                    if (sender.Equals(textBoxYear)) buttonYear.PerformClick();
                    if (sender.Equals(textBoxRating)) buttonRating.PerformClick();
                }
            }
            catch (Exception ex)
            { }
        }

        private void buttonDisplayCurrent_Click(object sender, EventArgs e)
        {   try
            {
                var dgv = (splitContainer2.Panel2.Controls[0] as DataGridView);
                List<DataRow> rows = new List<DataRow>();
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    if (dgvr != null && dgvr.Index < (dgv.DataSource as DataTable).Rows.Count)
                    {
                        if (dgvr.Visible)
                        {
                            DataRow row = ((DataRowView)dgvr.DataBoundItem).Row;

                            rows.Add(row);
                        }
                    }
                }


                DataTable resultTable = rows.CopyToDataTable<DataRow>();

                if (!resultTable.Columns.Contains("Image"))
                {
                    resultTable.Columns.Add("Image", typeof(Image));
                    resultTable.Columns["Image"].SetOrdinal(0);
                }

                resultTable.TableName = Kolibri.Common.Utilities.ExcelUtilities.LegalSheetName((resultTable.TableName));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));
                if (resultTable.DataSet == null)
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(resultTable);
                }

                DirectoryInfo DestinationPath = new DirectoryInfo(Path.GetTempPath());
                string html = DataSetToHTML(resultTable,
                new List<string>() { "Image", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" , "ImdbId"});
                //ShowHTML(html);


                if (sender.Equals(buttonDisplayCurrentHTML))
                {
                    ShowHTML(html);
                }
                else
                {
                    var tempPath = _htmlFile; // Kolibri.Utilities.FileUtilities.GetTempFile("html");
                    Kolibri.Common.Utilities.FileUtilities.WriteStringToFile(html, tempPath.FullName, Encoding.UTF8);
                    SetLabelText($"{resultTable.Rows.Count} filmer funnet.");
                    Kolibri.Common.Utilities.FileUtilities.Start(tempPath.FullName);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            } 
            buttonPrint.Enabled = buttonDisplayCurrentHTML.Enabled; 
        }

        private string DataSetToHTML(DataTable dtable, List<string> cols)
        {
            StringBuilder html = new StringBuilder();
            try
            {
                var files = _LITEDB.FindAllFileItems();

                //Populating a DataTable from database.
                DataTable dt = new DataView(dtable, "", "", DataViewRowState.CurrentRows).ToTable(false, cols.ToArray());
                //Building an HTML string.
                html.Append("<table>");
                //Table start.

                //Building the Header row.
                html.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    html.Append("<th>");
                    html.Append(column.ColumnName);
                    html.Append("</th>");
                }
                html.Append("</tr>");
                //Building the Data rows.
                foreach (DataRow row in dt.Rows)
                {
                    html.Append("<tr>");

                    foreach (DataColumn item in row.Table.Columns)
                    {
                        byte[] bytes = null;
                        html.Append("<td>");

                        if (item.ColumnName != "Image")
                        {
                            if (item.ColumnName.Equals("Title"))
                            {
                                string path = string.Empty;
                                var movie = _LITEDB.FindItemByTitle($"{row["Title"]}", $"{row["Year"]}".ToInt32());
                                if (movie != null)
                                {
                                    var file = _LITEDB.FindFile(movie.ImdbId);
                                    if (file != null)
                                    {
                                        FileInfo info = new FileInfo(file.FullName);
                                        path = info.Directory.FullName;
                                    }

                                    if (!String.IsNullOrEmpty(path))
                                    {
                                        string temp = $@"<a href=""{path}"">{row[item.ColumnName]}</a>";
                                        html.Append(temp);
                                    }

                                    else
                                    {
                                        html.Append(row[item.ColumnName]);
                                    }
                                }

                            }
                            else
                            {
                                html.Append(row[item.ColumnName]);
                            }
                        }
                        else
                        {
                            if (item.Table.Columns.Contains("Image"))
                            {
                                if (row["Image"] != DBNull.Value)
                                {

                                    bytes = Kolibri.Common.Utilities.ImageUtilities.BitmapToBytes(row["Image"] as Bitmap);

                                }
                                else if (row["ImdbId"] != DBNull.Value)
                                {

                                    bytes = _imageCache.GetItemsFromCache($"{row["ImdbId"]}");

                                }
                                else {
                                    var title = _LITEDB.FindItemByTitle($"{ row["Title"]}", $"{ row["Year"]}".ToInt32());
                                    bytes = _imageCache.GetItemsFromCache(title.ImdbId);
                                }
                                if (bytes != null)
                                    html.Append("<img height='100px' width='75px' src=\'data:image/jpg;base64," + Convert.ToBase64String(bytes) + "' />");
                            }
                        }
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }

                //Table end.
                html.Append("</table>");
                return html.ToString();
            }
            catch (Exception)
            { html.Append("<table class='table table-responsive' border='1'/>"); } 
            return html.ToString();
        }

        private void ShowHTML(string html)
        {
            splitContainer2.Panel2.Controls.Clear();
            try
            {

                if (!_htmlFile.Directory.Exists) _htmlFile.Directory.Create();
                Kolibri.Common.Utilities.FileUtilities.WriteStringToFile(html, _htmlFile.FullName, Encoding.UTF8);
                _htmlFile.Refresh();
                WebBrowser browser = new WebBrowser();
                browser.Navigate(_htmlFile.FullName);
                browser.Dock = DockStyle.Fill;
                splitContainer2.Panel2.Controls.Add(browser);
                //System.Diagnostics.Process.Start(info.FullName);
            }
            catch (Exception ex)
            { }
        }

        private void buttonRating_Click(object sender, EventArgs e)
        {
            try
            {


                if (_resultTable == null)
                    buttonVis.PerformClick();
                string searchValue = textBoxRating.Text;
                decimal num;
                if (!Decimal.TryParse(searchValue.Replace(".", ","), out num))
                {
                    SetLabelText($"{buttonRating.Text} Cannot search for ratings larger than  - {searchValue}    ({num})"); return;
                }
                SearchBy("ImdbRating", searchValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBoxYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                    (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonYear_Click(object sender, EventArgs e)
        {
            if (_resultTable == null)
                buttonVis.PerformClick();
            string searchValue = textBoxYear.Text;
            try
            {
                decimal num = searchValue.ToInt32();
                if (!Decimal.TryParse(searchValue.Replace(".", ","), out num))
                {
                    SetLabelText($"{buttonRating.Text} Cannot search for years  - {searchValue}    ({num})"); return;
                }
            }
            catch (Exception)
            { }
            SearchBy("Year", searchValue);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            try
            {
                WebBrowser webBrowser1 = splitContainer2.Panel2.Controls[0] as WebBrowser;
                webBrowser1.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            buttonPrint.Enabled = false;
            string genre = comboBoxGenre.Text;
            string title = textBoxTitle.Text;
            string year = textBoxYear.Text;
            string rating = textBoxRating.Text;

            string sql = $"{genre}{title}{year}{rating}";
            List<string> query = new List<string>();

            try
            {
                if (_resultTable == null)
                    buttonVis.PerformClick();

                if (string.IsNullOrEmpty(sql)) { throw new MissingPrimaryKeyException("No filter set!!!"); }
                sql = string.Empty;
                if (!genre.Equals("--ALL--")) { query.Add($"Genre LIKE '%{genre}%' "); }
                if (!string.IsNullOrEmpty(title)) { query.Add($"Title LIKE '%{title}%' "); }
                if (!string.IsNullOrEmpty(year)) { query.Add($"Year >= '{year}' "); }
                if (!string.IsNullOrEmpty(rating)) { query.Add($"ImdbRating >= '{rating}' "); }

                if (query.Count == 0) { comboBoxGenre_SelectedIndexChanged(null, null); return; }


                sql = string.Join(" AND ", query);

                DataTable table = new DataView(_resultTable, sql, "ImdbRating DESC", DataViewRowState.CurrentRows).ToTable();
                if (table.Rows.Count <= 0) throw new DataException("No data found matching the searchstring \r\n" + sql.Replace("%", string.Empty));

                foreach (DataRow row in table.Rows)
                {

                    try
                    {
                        if (row.Table.Columns.Contains("Image"))
                        {
                            if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(row["Poster"].ToString()))
                            {
                                row["Image"] = (Bitmap)_imageCache.RetrieveImage(row["Poster"].ToString());
                            }
                        }

                    }
                    catch (Exception)
                    { }
                }


                ShowGridView(table);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

            try
            {
                _LITEDB.Dispose();
            }
            catch (Exception ex)
            { }
        }

   
    }
}