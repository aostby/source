using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Formutilities.Controller;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controls;
//using Microsoft.Office.Interop.Excel;
using OMDbApiNet.Model;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Kolibri.net.Common.MovieAPI.Forms
{
    public partial class BrowseMoviesForm : Form
    {
        private LiteDBController _LITEDB;
        //private ImageCache _imageCache;
        private UserSettings _userSettings;

        FileInfo _htmlFile = new FileInfo(@"c:\temp\preview\html.html");
        public BrowseMoviesForm(UserSettings userSettings)
        {
            InitializeComponent();
            _userSettings = userSettings;
            Init();
        }

        private void Init()
        {
            buttonVisualize.Enabled = false;

            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            linkLabelOpenInBrowser.Tag = _htmlFile;
            //try { _imageCache = new ImageCache(_userSettings); } catch (Exception) { }

            try
            {
                _LITEDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);

                comboBoxGenre.Items.AddRange(MovieUtilites.GenreList.ToArray());

                List<string> year = MovieUtilites.YearList.ToList();
                if (!year.Contains(DateTime.Now.Year.ToString()))
                    year.Insert(0, DateTime.Now.Year.ToString());
                year.Insert(0, "All");

                comboBoxYear.Items.AddRange(year.ToArray());
                comboBoxGenre.SelectedIndex = 0;
                comboBoxYear.SelectedIndex = 0;
                comboBoxYear.SelectedIndex = comboBoxYear.FindStringExact(DateTime.Now.Year.ToString());


                tbSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
                try
                { 
                    tbSearch.AutoCompleteCustomSource = AutoCompleteController.ToAutoCompleteStringCollection
                                (_LITEDB.FindAllItems().GetAwaiter().GetResult().Select(s => s.Title).ToList());
                }
                catch (Exception ex)
                { }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

            //Task.Run(async () => await _imageCache.InitImages(_LITEDB));

            try

            {
              //  this.Text = this.Text + $"({_imageCache.NumElements})";
                labelInfo.Text = $"Intializing images. Please search for items in movies and series collection saved in \r\n {_LITEDB.ConnectionString}";
            }
            catch (Exception ex)
            {
                labelInfo.Text = $"Intializing images. Please search for items in movies and series collection. ";
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            buttonVisualize.Enabled = false;
            try
            {
                List<Item> ret = null;

                string searhText = tbSearch.Text;
                string genre = comboBoxGenre.Text;
                string year = comboBoxYear.Text;

                if (string.IsNullOrWhiteSpace(searhText + genre + year))
                {
                    labelInfo.Text = $"{DateTime.Now.ToShortTimeString()} - No items found for this search. Check your parameters";
                    return;
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    ret = _LITEDB.FindItemByGenreNew(genre).ToList();

                    if (!string.IsNullOrEmpty(searhText) && ret != null && ret.Count() > 0)
                    {
                        ret = ret.Where(a => a.Title.ToUpper().Contains(searhText.ToUpper())).ToList();
                    }

                    try
                    {

                        if (!string.IsNullOrEmpty(year) && ret != null && ret.Count() > 0&&year!="All")
                        {
                            int min = year.Split('-').FirstOrDefault().Trim().ToInt().GetValueOrDefault();
                            int max = year.Split('-').LastOrDefault().Trim().ToInt().GetValueOrDefault();

                            ret = ret.Where(d => (int)d.Year.ToInt().GetValueOrDefault() >= min && (int)d.Year.ToInt().GetValueOrDefault() <= max).ToList();
                        }
                    }
                    catch (Exception) { }
                }

                if (ret == null && !string.IsNullOrEmpty(year))
                {
                    try
                    {
                        ret = _LITEDB.FindAllItems().Result.ToList();
                        if (!string.IsNullOrEmpty(year) && ret != null && ret.Count() > 0)
                        {
                            int min = year.Split('-').FirstOrDefault().Trim().ToInt().GetValueOrDefault();
                            int max = year.Split('-').LastOrDefault().Trim().ToInt().GetValueOrDefault();

                            ret = ret.Where(d => (int)d.Year.ToInt().GetValueOrDefault() >= min && (int)d.Year.ToInt().GetValueOrDefault() <= max).ToList();
                            if (checkBoxDecending.Checked)
                            {
                                ret = ret.OrderByDescending(x => x.Year).ThenByDescending(y => y.ImdbRating).ToList();
                            }
                            else
                            {
                                ret = ret.OrderBy(x => x.Year).ThenBy(y => y.ImdbRating).ToList();
                            }
                        }
                    }
                    catch (Exception) { }


                    if (!string.IsNullOrEmpty(searhText) && ret != null && ret.Count() > 0)
                    {
                        ret = ret.Where(a => a.Title.ToUpper().Contains(searhText.ToUpper())).ToList();
                    }


                }

                if (ret == null && !string.IsNullOrEmpty(searhText))
                {
                    ret = _LITEDB.FindItemByTitle(searhText).ToList();
                }

                var gen = (ret != null) ? ret.ToList() : null;

                if (ret != null && ret.Count() > 0)
                {
                    if (radioButtonRating.Checked)
                        ret = checkBoxDecending.Checked ? ret.OrderByDescending(s => s.ImdbRating).ToList() : ret.OrderBy(s => s.ImdbRating).ToList();
                    else if (radioButtonYear.Checked)
                        ret = checkBoxDecending.Checked ? ret.OrderByDescending(s => s.Year).ToList() : ret.OrderBy(s => s.Year).ToList();
                    labelInfo.Text = $"{DateTime.Now.ToShortTimeString()} - Number of items found for this search: searhText: {searhText} + genre: {genre} + year: {year} = {ret.Count()}";
                    DisplayHtml(ret, $"{searhText} {genre} {year}");
                    buttonVisualize.Tag = ret;
                    buttonVisualize.Enabled = buttonVisualize.Tag != null && ret.Count() > 0;
                }

                else
                {
                    MessageBox.Show("No movies found!", "{searhText} + {genre} + {year}");
                    labelInfo.Text = $"{DateTime.Now.ToShortTimeString()} - No items found for this search. Check your parameters searhText: {searhText} + genre: {genre} + year: {year}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void DisplayHtml(IEnumerable<Item> liste, string title = null)
        {
            try
            {
                try { if (_htmlFile.Exists) _htmlFile.Delete(); } catch (Exception) { }

                if (!_htmlFile.Directory.Exists) _htmlFile.Directory.Create();

                try
                {   var html = CreateHTML(liste, null, title);
                FileUtilities.WriteStringToFile(html, _htmlFile.FullName, Encoding.UTF8);
                splitContainer1.Panel2.Controls.Clear();

                     WebBrowser browser = new WebBrowser();  

                    browser.Navigate(_htmlFile.FullName);

                    browser.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Add(browser);
                    browser.Show();
                }
                catch (Exception ex)
                { }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private string CreateHTML(IEnumerable<Item> items, List<string> cols = null, string title = null)
        {
            //_imageCache.InitImages(_LITEDB);

            if (cols == null)
                cols = new List<string>() { "Title", "ImdbRating", "Year", "Type", "Runtime", "Genre", "Plot", "ImdbId", "Rated" };

            StringBuilder html = new StringBuilder();
            try
            {
                ArrayList liste = new ArrayList();
                liste.AddRange(items.ToList());

                var dt = new DataView(DataSetUtilities.AutoGenererDataSet(liste).Tables[0], "", "", DataViewRowState.CurrentRows).ToTable(false, cols.ToArray());

                if (dt.Columns["Image"] == null)
                {
                    dt.Columns.Add("Image", typeof(Bitmap));
                    dt.Columns["Image"].SetOrdinal(0);
                }
                string htmltitle = $"{title} ({liste.Count} items)";
                var files = items;
                html.Append($@"<html>
<head>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <title>{htmltitle}</title>
<script>
    function toggle_visibility(id) 
    {{ 
        var e = document.getElementById(id);
        e.style.display = ((e.style.display!='none') ? 'none' : 'block');
    }} 
  </script>

</head>
<body><h2>{htmltitle}</h2>
  </p>  
    Vis eller skjul filer:
    <a href=""#"" onclick=""toggle_visibility('movie')"">movie</a>
     <a href=""#"" onclick=""toggle_visibility('episode')"">episode</a>
     <a href=""#"" onclick=""toggle_visibility('series')"">series</a>
  </p>
<style>
  table {{table-layout: fixed; }}
    .Plot {{width: 30%; }}
 
img{{width:75px;
  height: 100px;
  transition: .5s ease-in-out;
}}
img:hover{{transform: scale(1.5)}}

</style>
<table class='table table-responsive' border='1'>");


                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Item));
                DataTable table = new DataTable();
                html.Append("<tr>");
                html.Append("<th>");
                html.Append("Image");
                html.Append("</th>");

                foreach (string prop in cols)
                {
                    if (dt.Columns.Contains(prop))
                    {
                        if (prop.Equals("Plot"))
                            html.Append($@"<th class=""{prop}"">");
                        else
                            html.Append("<th>");
                        html.Append(prop);
                        html.Append("</th>");
                    }
                }
                html.Append("</tr>");
                //Building the Data rows.
                foreach (DataRow row in dt.Rows)
                {
                    html.Append($@"<div id=""{row["Type"]}"" style=""display:block"">");
                    Item movie = _LITEDB.FindItemByTitle($"{row["Title"]}", $"{row["Year"]}".ToInt().GetValueOrDefault());
                  
                    html.Append($"<tr>");
                    foreach (DataColumn item in row.Table.Columns)
                    {

                        byte[] arr = null;


                     
                        html.Append($"<td>");

                        if (item.ColumnName != "Image")
                        {
                            if (item.ColumnName.Equals("Title"))
                            {
                                FileInfo info = null;
                                string path = string.Empty;

                                if (movie != null)
                                {
                                    var file = _LITEDB.FindFile(movie.ImdbId).GetAwaiter().GetResult();
                                    if (file != null)
                                    {
                                        info = new FileInfo(file.FullName);
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
                            else if (item.ColumnName.Equals("ImdbId"))
                            {
                                string titleLink = $@"https://www.imdb.com/title/{row[item.ColumnName]}";
                                string temp = $@"<a href=""{titleLink}"" target=""_blank"">{row[item.ColumnName]}</a>";
                                html.Append(temp);
                            }
                            else
                            {
                                html.Append(row[item.ColumnName]);
                            }
                        }
                        else
                        {
                            try
                            {
                                string key = row["ImdbId"].ToString();
                                html.Append($@"<img alt=""{movie.Title}"" src=""{movie.Poster}"" />");
                            }
                            catch (Exception ex)
                            { }
                        }
                        html.Append("</td>");
                       
                    }
                    html.Append(" </tr>");

                    html.Append("</div>");
                    movie = null;
                }

                //Table end.
                html.Append("</table></body></html>");

                return html.ToString();
            }
            catch (Exception ex)
            { html.Append($"<table class='table table-responsive' border='1'><tr><td>{ex.Message}</td></tr></table>"); }

            return html.ToString();
        }

        private void BrowseMoviesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //_imageCache.Save();
            }
            catch (Exception ex) { }
        }

        private void linkLabelOpenInBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FileUtilities.Start((sender as LinkLabel).Tag as FileInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void SetForm(Form form, Panel panel = null)
        {
            Panel pan = splitContainer1.Panel2;
            if (panel != null)
                pan = panel;

            pan.Controls.Clear();
            try
            {
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;

                if (pan != null)
                    form.Dock = DockStyle.Fill;

                pan.Controls.Add(form);
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonVisualize_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            try
            {
                List<Item> items = buttonVisualize.Tag as List<Item>;
                DataTable resultTable = DataSetUtilities.ConvertToDataTable(items);
                DataGrivViewControls dgvtrls = new DataGrivViewControls(Constants.MultimediaType.Series, new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false));
                Form form = dgvtrls.GetMulitMediaDBDataGridViewAsForm(resultTable);
                splitContainer1.Panel2.Controls.Add(form);

                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}