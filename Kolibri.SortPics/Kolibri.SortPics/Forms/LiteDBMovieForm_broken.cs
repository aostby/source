using System.Configuration;
using CefSharp;
using Kolibri.Utilities;
using SortPics.Controller;
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

namespace SortPics.Forms
{
    public partial class LiteDBMovieForm
        : Form
    {
        public string omdbkey = "d0971580";

        private OMDBController _OMDB;
        private LiteDBController _LITEDB;
        private TMDBController _TMDB;
        private ImageCache _imageCache; 

        private DataTable _resultTable;
        private FileInfo _htmlFile;
        private FileInfo _pdfFile = GetPDFFile();

        private static FileInfo GetPDFFile()
        {
            var pdfFile = new FileInfo(@"c:\temp\html.pdf");
            try { if (pdfFile.Exists) pdfFile.Delete(); } catch (Exception) { }
            try { if (!pdfFile.Directory.Exists) pdfFile.Directory.Create(); } catch (Exception) { }
            pdfFile.Refresh();
            return pdfFile;
        }

        private List<string> _genres = Kolibri.Utilities.MovieUtilites.GenreList.ToList();

        public LiteDBMovieForm()
        {
            InitializeComponent();
            Init();
        }

        private void SetLabelText(string text)
        {
            try
            {
                toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft;
                toolStripStatusLabel1.Text = text;
                toolStripStatusLabel1.GetCurrentParent().Update();
            }
            catch (Exception)
            { }
        }

        private void Init()
        {
            _imageCache = new ImageCache(true, Application.ProductName);
            buttonDisplayCurrentHTML.Enabled = false;
            buttonPrint.Enabled = buttonDisplayCurrentHTML.Enabled;
            SetLabelText($"Init... klikk {buttonVis.Text}");

            this.Text = $"Search for movies via local db and OMDB ({Assembly.GetExecutingAssembly().GetName().Version.ToString()})";

            _OMDB = new Controller.OMDBController(omdbkey);
            _LITEDB = new Controller.LiteDBController(false, true);
            _TMDB = new Controller.TMDBController(_LITEDB);

            _htmlFile = new FileInfo(@"c:\temp\html.html");
            try { if (_htmlFile.Exists) _htmlFile.Delete(); } catch (Exception) { }


            if (!File.Exists(_imageCache.CachePath))
            { SetLabelText($"Could not read {_imageCache.CachePath}: file does not exist"); } 
            if (comboBoxGenre.Items.Count <= 0)
                comboBoxGenre.Items.AddRange(_genres.ToArray());
            SetLabelText($"ImageCache has {_imageCache.NumElements} elements");

        } 

        private void ShowDataBase(DirectoryInfo directoryInfo = null)
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
                        var tempTable = Kolibri.Utilities.DataSetUtilities.AutoGenererTypedDataSet(new System.Collections.ArrayList() { movie });
                        if (resultTable == null) { resultTable = tempTable.Tables[0].Copy(); }
                        else { resultTable.Merge(tempTable.Tables[0].Copy()); }
                        tempTable.Dispose(); 
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

        private void DataSetToExcel(DataSet ds)
        {
            List<string> columns = new List<string>() { "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };
            DataSetToExcel(ds, columns);
        } 
        private void DataSetToExcel(DataSet ds, List<string> columns)
        {
            DirectoryInfo DestinationPath = Kolibri.Utilities.FileUtilities.LetOppMappe();
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

            Kolibri.Utilities.ExcelUtilities.GenerateExcel2007(new FileInfo(filePath), temp.DataSet);
            Kolibri.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(filePath));
        } 
      
        private void ShowGridView(DataTable table)
        {
            try
            {
                var dic = table.AsEnumerable()
         .ToDictionary(row => row.Field<string>("ImdbId"),
                                   row => row.Field<string>("Poster"));
                Task.Run(() => _imageCache.GetImagesAsync(_LITEDB, dic));
            }
            catch (Exception)
            { } 

            List<string> visibleColumns = new List<string>(){ "Title"
            ,"ImdbRating"
            ,"Year"
            ,"Rated"
            ,"Runtime"
            ,"Genre"
            ,"Plot"
            ,"Image"};

            try
            {
                while (splitContainer2.Panel2.Controls.Count > 0)
                {
                    var control = splitContainer2.Panel2.Controls[0];
                    splitContainer2.Panel2.Controls.RemoveAt(0);
                    control.Dispose();
                } 

                splitContainer2.Panel2.Controls.Clear();

                if (!table.Columns.Contains("Image"))
                { table.Columns.Add("Image", typeof(Image)); }

                DataGridView dgv = new DataGridView();
                dgv.DataSource = null;
                dgv.DataSource = table;
                dgv.ClearSelection();
                dgv.Refresh();

           
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllHeaders;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgv.AllowUserToOrderColumns = true;
                dgv.AllowUserToResizeColumns = true;

                dgv.Dock = DockStyle.Fill;
                splitContainer2.Panel2.Controls.Add(dgv);
                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => col.Visible = false);
                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => { if (visibleColumns.Contains(col.Name)) col.Visible = true; });
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                try
                {
                    dgv.Columns["Image"].DisplayIndex = 0; // or 1, 2, 3 etc 
                    dgv.Columns["Title"].DisplayIndex = 1; // or 1, 2, 3 etc
                    dgv.Columns["Title"].Width = 150;
                    dgv.Columns["ImdbRating"].DisplayIndex = 2; // or 1, 2, 3 etc

                    dgv.Sort(dgv.Columns["ImdbRating"], ListSortDirection.Descending);
                    DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                    lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (dgv.Columns.Contains("Plot"))
                    {
                        dgv.Columns["Plot"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["Plot"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    }

                }
                catch (Exception) { } 

                /********************************/
                if (dgv.Columns.Contains("Image"))
                    ((DataGridViewImageColumn)dgv.Columns["Image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                panelDetails.Controls.Clear();
                foreach (DataGridViewRow item in dgv.Rows)
                {
                    try
                    {  
                        {  if ((item.DataBoundItem as System.Data.DataRowView) == null) continue;

                            {
                                item.Visible = true;
                                DataRow row = ((System.Data.DataRowView)item.DataBoundItem).Row;
                                try
                                {
                                    string poster = row["Poster"].ToString();
                                    string imdbid = row["ImdbId"].ToString();
                                    if (_imageCache.CacheImageExists(poster))
                                        row["Image"] = _imageCache.RetrieveImage(poster);
                                    else if (_imageCache.CacheImageExists(imdbid))
                                        row["Image"] = _imageCache.RetrieveImage(imdbid);
                                }
                                catch (Exception ex)
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
            string title = null; int year = 0; int index = 0;
            try
            {  panelDetails.Visible = false;
                DataGridView DataGridView1 = sender as DataGridView; 

                if (DataGridView1.SelectedRows.Count > 0)
                    index = DataGridView1.SelectedRows[0].Index;
                else
                    try
                    {
                        if (DataGridView1.SelectedRows.Count >= 1)
                            index = DataGridView1.CurrentRow.Index;
                    }
                    catch (Exception) { }

                if (DataGridView1.Rows[index].Cells["Title"].Value
                          != null)
                {
                    if (DataGridView1.Rows[index].
                        Cells["Title"].Value.ToString().Length != 0)
                    {
                        title = DataGridView1.Rows[index].Cells["Title"].Value.ToString();

                        if (!int.TryParse(DataGridView1.Rows[index].Cells["Year"].Value.ToString(), out year)) ;
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
                                //LiteDBController.ImagePoster poster = null;
                                //string imdbid = DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString();
                                //if (_imageCache.CacheImageExists(imdbid))
                                //{
                                //    DataGridView1.Rows[index].Cells["Image"].Value = _imageCache.RetrieveImage(imdbid);
                                //}
                            
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
                }

                if (movie == null)
                {
                    movie = new OMDbApiNet.Model.Item() { Title = title, Year = $"{year}" };
                }

                if (movie != null)
                {
                    Form form = new MovieDetailsForm(movie, _LITEDB);
                    SetForm(form);
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
            { }
        }

        private void SetForm(Form form)
        {
            try
            {
                while (panelDetails.Controls.Count > 0)
                {
                    var control = this.panelDetails.Controls[0];
                    this.panelDetails.Controls.RemoveAt(0);
                    control.Dispose();
                }

                panelDetails.Controls.Clear();

            }
            catch (Exception)
            { }

          
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
            try
            {
                _imageCache.Save();
                _LITEDB.Dispose();
            }
            catch (Exception)
            { }

        }

        private void comboBoxGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxGenre.Visible = false;
            if (_resultTable == null)
                buttonVis.PerformClick();
            string searchvalue = comboBoxGenre.Text;
            SearchBy("Genre", searchvalue); comboBoxGenre.Visible = true;
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

                var dgv = splitContainer2.Panel2.Controls[0] as DataGridView;
                if (dgv == null)
                {
                    ShowGridView(_resultTable);
                    dgv = splitContainer2.Panel2.Controls[0] as DataGridView;
                }

                if (dgv.Columns.Contains("Image"))
                    ((DataGridViewImageColumn)dgv.Columns["Image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;

                panelDetails.Controls.Clear();

                foreach (DataGridViewRow item in dgv.Rows)
                { string imdbid = ((item.DataBoundItem as System.Data.DataRowView)).Row["ImdbId"].ToString();
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
                                //DataRow row = ((System.Data.DataRowView)item.DataBoundItem).Row;
                                //try
                                //{
                                //    if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(row["Poster"].ToString()))
                                //    {
                                //        row["Image"] = (Bitmap)_imageCache.RetrieveImage(row["Poster"].ToString());
                                //    }
                                //    else if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(imdbid))
                                //    {
                                //        row["Image"] = (Bitmap)_imageCache.RetrieveImage(imdbid);
                                //    }

                                //    //if (row["Image"] as Bitmap != null)
                                //    //{
                                //    //    try
                                //    //    {
                                //    //        item.Height = (int)((Bitmap)_imageCache.RetrieveImage(row["Poster"].ToString())).Height / 3;

                                //        //    }
                                //        //    catch (Exception)
                                //        //    { }
                                //        //}
                                //}
                                //catch (Exception)
                                //{ }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SetLabelText($"DB with fitler {typeSearch} by {searchValue} error {ex.Message}");
                    }
                }
                SetLabelText($"DB with fitler {typeSearch} by {searchValue} contains {dgv.Rows.GetRowCount(DataGridViewElementStates.Visible)} out of {dgv.Rows.Count} rows");
            }
            catch (Exception ex)
            { }
            
            //Lagre unna evt nye bilder
       //     _imageCache.Save();
        
        }


        private void buttonTitle_Click(object sender, EventArgs e)
        {
            if (_resultTable == null)
                buttonVis.PerformClick();
            string searchvalue = textBoxTitle.Text;

            SearchBy("Title", searchvalue);

        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {

                if (sender.Equals(textBoxTitle)) buttonTitle.PerformClick();
                if (sender.Equals(textBoxYear)) buttonYear.PerformClick();
                if (sender.Equals(textBoxRating)) buttonRating.PerformClick();
            }

        }

        private void buttonDisplayCurrent_Click(object sender, EventArgs e)
        {

            try
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

                resultTable.TableName = Kolibri.Utilities.ExcelUtilities.LegalSheetName((resultTable.TableName));//Kolibri.Utilities.MovieUtilites.GetMovieTitle(form.SourcePath.Name));
                if (resultTable.DataSet == null)
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(resultTable);
                }

                DirectoryInfo DestinationPath = new DirectoryInfo(Path.GetTempPath());
                string html = DataSetToHTML(resultTable,
                new List<string>() { "Image", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" });
                //ShowHTML(html);


                if (sender.Equals(buttonDisplayCurrentHTML))
                {
                    ShowHTML(html);
                }
                else
                {
                    var tempPath = _htmlFile; // Kolibri.Utilities.FileUtilities.GetTempFile("html");
                    Kolibri.Utilities.FileUtilities.WriteStringToFile(html, tempPath.FullName, Encoding.UTF8);
                    SetLabelText($"{resultTable.Rows.Count} filmer funnet.");
                    Kolibri.Utilities.FileUtilities.Start(tempPath.FullName);
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
                        byte[] arr = null;
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
                            if (row["Image"] != DBNull.Value)
                            {
                                byte[] bytes = null;
                                bytes = Kolibri.Utilities.ImageUtilities.BitmapToBytes(row["Image"] as Bitmap);
                                html.Append("<img height='100px' width='75px' src=\'data:image/jpg;base64," + Convert.ToBase64String(bytes) + "' />");

                            }
                        }
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }

                //Table end.
                html.Append("</table>"); return html.ToString();
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
                Kolibri.Utilities.FileUtilities.WriteStringToFile(html, _htmlFile.FullName, Encoding.UTF8);
                _htmlFile.Refresh();
                CefSharp.WinForms.ChromiumWebBrowser browser = new CefSharp.WinForms.ChromiumWebBrowser(_htmlFile.FullName)
                {
                    Dock = DockStyle.Fill,
                };
                splitContainer2.Panel2.Controls.Add(browser);
                //System.Diagnostics.Process.Start(info.FullName);
            }
            catch (Exception ex)
            { }
        }

        private void buttonRating_Click(object sender, EventArgs e)
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

        private void textBoxYear_KeyPress(object sender, KeyPressEventArgs e)
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
                CefSharp.WinForms.ChromiumWebBrowser webBrowser1 = splitContainer2.Panel2.Controls[0] as CefSharp.WinForms.ChromiumWebBrowser;
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
                if (!string.IsNullOrEmpty(year)) { query.Add($"Year >= '%{year}%' "); }
                if (!string.IsNullOrEmpty(rating)) { query.Add($"ImdbRating >= '%{rating}%' "); }

                if (query.Count == 0) { comboBoxGenre_SelectedIndexChanged(null, null); return; }


                sql = string.Join(" AND ", query);

                DataTable table = new DataView(_resultTable, sql, "ImdbRating DESC", DataViewRowState.CurrentRows).ToTable();
                if (table.Rows.Count <= 0) throw new DataException("No data found matching the searchstring \r\n" + sql.Replace("%", string.Empty));

           

                foreach (DataRow row in table.Rows)
                {
                    string imdbid = $"{row["ImdbId"]}";
                    string poster = $"{row["Poster"]}";
                    try
                    {
                        if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(poster))
                        {
                            row["Image"] = (Bitmap)_imageCache.RetrieveImage(poster);
                        }
                        else if (row["Image"] as Bitmap == null && _imageCache.CacheImageExists(imdbid)) {
                            row["Image"] = (Bitmap)_imageCache.RetrieveImage(imdbid);
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