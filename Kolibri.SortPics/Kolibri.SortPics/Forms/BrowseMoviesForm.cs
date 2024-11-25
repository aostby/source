 
using Kolibri.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using SortPics.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kolibri.Common.MovieAPI.Controller;

namespace SortPics.Forms
{
    public partial class BrowseMoviesForm : Form
    {
        private LiteDBController _LITEDB;
        private ImageCache _imageCache;

        FileInfo _htmlFile = new FileInfo(@"c:\temp\html.html");
        public BrowseMoviesForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {

            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            linkLabelOpenInBrowser.Tag = _htmlFile;
            try
            {
                _imageCache = new ImageCache(true, Application.ProductName);
            }
            catch (Exception)
            {
            }

            try
            { 
            }
            catch (Exception)
            {

            }

            try
            {

                _LITEDB = new   LiteDBController(false, true);

                comboBoxGenre.Items.AddRange(Kolibri.Common.Utilities.MovieUtilites.GenreList.ToArray());

                List<string> year = Kolibri.Common.Utilities.MovieUtilites.YearList.ToList();
                if (!year.Contains(DateTime.Now.Year.ToString()))
                    year.Insert(0, DateTime.Now.Year.ToString());

                comboBoxYear.Items.AddRange(year.ToArray());

                comboBoxGenre.SelectedIndex = 0;
                comboBoxYear.SelectedIndex = 0;

                comboBoxYear.SelectedIndex = comboBoxYear.FindStringExact(DateTime.Now.Year.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
         
            Task.Run(async () => await _imageCache.InitImages(_LITEDB));

            try
            
            {   this.Text = this.Text + $"({_imageCache.NumElements})";
                labelInfo.Text = $"Intializing images. Please search for items in movies and series collection saved in \r\n {_LITEDB.LastBConnectionString}";
            }
            catch (Exception ex)
            {
                labelInfo.Text = $"Intializing images. Please search for items in movies and series collection. ";
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

            try
            { 
                IEnumerable<Item> ret = null;

                string searhText = textBox1.Text;
                string genre = comboBoxGenre.Text;
                string year = comboBoxYear.Text;

                if (string.IsNullOrWhiteSpace(searhText + genre + year))
                {
                    labelInfo.Text = $"{DateTime.Now.ToShortTimeString()} - No items found for this search. Check your parameters";                        
                    return;
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    ret = _LITEDB.FindItemByGenreNew(genre);

                    if (!string.IsNullOrEmpty(searhText) && ret != null && ret.Count() > 0)
                    {
                        ret = ret.Where(a => a.Title.ToUpper().Contains(searhText.ToUpper()));
                    }

                    try
                    {

                        if (!string.IsNullOrEmpty(year) && ret != null && ret.Count() > 0)
                        {
                            int min = year.Split('-').FirstOrDefault().Trim().ToInt().GetValueOrDefault();
                            int max = year.Split('-').LastOrDefault().Trim().ToInt().GetValueOrDefault();

                            ret = ret.Where(d => (int)d.Year.ToInt().GetValueOrDefault() >= min && (int)d.Year.ToInt().GetValueOrDefault() <= max);
                        }
                    }
                    catch (Exception) { }
                }

                if (ret == null && !string.IsNullOrEmpty(year))
                {
                    try
                    {
                        ret = _LITEDB.FindAllItems();
                        if (!string.IsNullOrEmpty(year) && ret != null && ret.Count() > 0)
                        {
                            int min = year.Split('-').FirstOrDefault().Trim().ToInt().GetValueOrDefault();
                            int max = year.Split('-').LastOrDefault().Trim().ToInt().GetValueOrDefault();

                            ret = ret.Where(d => (int)d.Year.ToInt().GetValueOrDefault() >= min && (int)d.Year.ToInt().GetValueOrDefault() <= max);
                        }
                    }
                    catch (Exception) { }


                    if (!string.IsNullOrEmpty(searhText) && ret != null && ret.Count() > 0)
                    {
                        ret = ret.Where(a => a.Title.ToUpper().Contains(searhText.ToUpper()));
                    }


                }

                if (ret == null && !string.IsNullOrEmpty(searhText))
                {
                    ret = _LITEDB.FindItemByTitle(searhText);
                }

                var gen = (ret != null) ? ret.ToList() : null;

                if (ret != null && ret.Count() > 0)
                {
                    if (radioButtonRating.Checked)
                        ret = checkBoxDecending.Checked ? ret.OrderByDescending(s => s.ImdbRating) : ret.OrderBy(s => s.ImdbRating);
                    else if (radioButtonYear.Checked)
                        ret = checkBoxDecending.Checked ? ret.OrderByDescending(s => s.Year) : ret.OrderBy(s => s.Year);
                    labelInfo.Text = $"{DateTime.Now.ToShortTimeString()} - Number of items found for this search: searhText: {searhText} + genre: {genre} + year: {year} = {ret.Count()}";
                    DisplayHtml(ret, $"{searhText } {genre} {year}");
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

                var html = CreateHTML(liste, null, title);
                Kolibri.Common.Utilities.FileUtilities.WriteStringToFile(html, _htmlFile.FullName, Encoding.UTF8);
                splitContainer1.Panel2.Controls.Clear();
                try
                {
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
            _imageCache.InitImages(_LITEDB,  items);

            if (cols == null)
                cols = new List<string>() {  "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot", "ImdbId" };

            StringBuilder html = new StringBuilder();
            try
            {
                ArrayList liste = new ArrayList();
                liste.AddRange(items.ToList());

                var dt = new DataView(Kolibri.Common.Utilities.DataSetUtilities.AutoGenererDataSet(liste).Tables[0], "", "", DataViewRowState.CurrentRows).ToTable(false, cols.ToArray());

                if (dt.Columns["Image"] == null)
                {
                    dt.Columns.Add("Image", typeof(Bitmap));
                    dt.Columns["Image"].SetOrdinal(0);
                }

                var files = items;
                html.Append($@"<html><title>{title}</title><body><h2>{title}</h2>
<style>
  table {{table-layout: fixed; }}
    .Plot {{width: 30%; }}
</style>
<table class='table table-responsive' border='1'>");


                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Item));
                DataTable table = new DataTable();
                html.Append("<tr>");
                html.Append("<th>");
                html.Append("Image");
                html.Append("</th>");

                foreach (string  prop in cols)
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
                    Item movie = _LITEDB.FindItemByTitle($"{row["Title"]}", $"{row["Year"]}".ToInt().GetValueOrDefault());
                    html.Append("<tr>");

                    foreach (DataColumn item in row.Table.Columns)
                    {

                        byte[] arr = null;
                        html.Append("<td>");

                        if (item.ColumnName != "Image")
                        {
                            if (item.ColumnName.Equals("Title"))
                            {
                                FileInfo info = null;
                                string path = string.Empty;

                                //                   movie = _LITEDB.FindItemByTitle($"{row["Title"]}", $"{row["Year"]}".ToInt().GetValueOrDefault());
                                if (movie != null)
                                {
                                    var file = _LITEDB.FindFile(movie.ImdbId);
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
                            else if(item.ColumnName.Equals("ImdbId")) {
                                string titleLink=$@"https://www.imdb.com/title/{row[item.ColumnName]}";
                                string temp = $@"<a href=""{titleLink}"">{row[item.ColumnName]}</a>";
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
                                Bitmap img;
                                string key = row["ImdbId"].ToString();
                                if (_imageCache.CacheImageExists(key) || (movie != null && _imageCache.CacheImageExists(movie.Poster)))
                                {
                                    byte[] bytes = null;
                                    if (_imageCache.CacheImageExists(key))
                                        bytes = _imageCache.GetByteArray(key);
                                    if (bytes == null && (movie != null && _imageCache.CacheImageExists(movie.Poster)))
                                        bytes = _imageCache.GetByteArray(movie.Poster);
                                    if (bytes != null)
                                    {
                                        //          html.Append("<img height='100px' width='75px' src=\'data:image/jpg;base64," + Convert.ToBase64String(bytes) + "' />");
                                        html.Append("<img  alt='Girl in a jacket'   height='100px' width='75px' src=\'data:image/jpg;base64," + Convert.ToBase64String(bytes) + "' />");
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
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
                _imageCache.Save();
            }
            catch (Exception ex)
            {

            }
        }

        private void linkLabelOpenInBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start((sender as LinkLabel).Tag.ToString());

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

            try
            {   // ShowDataBase(form.SourcePath);
                var liste = _LITEDB.FindAllItems("movie").ToList();
                if(radioButtonRating.Checked)                liste = liste.OrderByDescending(o => o.ImdbRating).ToList();
                if(radioButtonYear.Checked) liste = liste.OrderByDescending(o => o.Year).ToList();    
                string searchText = String.IsNullOrWhiteSpace(textBox1.Text) ? "Enter moviename or clear this field" : textBox1.Text;
                SetForm(form: new Kolibri.VisualizeOMDbItem.ShowLocalMovies(searchText, liste));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
           
        }
    }
}