using Kolibri.Common.MovieAPI.Controller;
using Kolibri.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortPics.Forms
{
    public partial class BrowseMoviesFormOld : Form
    {
        private LiteDBController _LITEDB;
        private ImageCache _imageCache;
        public BrowseMoviesFormOld()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            try
            {
                _imageCache = new ImageCache(true, Application.ProductName);
            }
            catch (Exception) { }

            try
            {

                _LITEDB = new LiteDBController(false, true);

                List<string> genreList = new List<string>() { "", "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Documentary", "Drama", "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western", };
                comboBoxGenre.Items.AddRange(genreList.ToArray());
                var MyList = Enumerable.Range(DateTime.Now.Year, DateTime.Now.Year - DateTime.Now.AddYears(-10).Year + 1).ToList();
                List<string> yearList = new List<string>() { "", "2022", "2021", "2020", "2019", "2015-2018", "2010-2014", "2000-2009", "1990-1999", "1980-1989", "1970-1979", "1900-1969", };
                comboBoxYear.Items.AddRange(yearList.ToArray());

                comboBoxGenre.SelectedIndex = 0;
                comboBoxYear.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

            Task.Run(async () => await _imageCache.InitImages(_LITEDB));
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            IEnumerable<Item> ret = null;

            string searhText = textBox1.Text;
            string genre = comboBoxGenre.Text;
            string year = comboBoxYear.Text;

            if (string.IsNullOrWhiteSpace(searhText + genre + year))
                return;

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

                DisplayHtml(ret);

            }
            else
            {
                MessageBox.Show("No movies found!", "searhText + genre + year");

            }
        }

        private void DisplayHtml(IEnumerable<Item> liste)
        {
            try
            {
                FileInfo _htmlFile = new FileInfo(@"c:\temp\html.html");
                try { if (_htmlFile.Exists) _htmlFile.Delete(); } catch (Exception) { }

                if (!_htmlFile.Directory.Exists) _htmlFile.Directory.Create();

                var html = CreateHTML(liste);

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

        private string CreateHTML(IEnumerable<Item> items, List<string> cols = null)
        {
            _imageCache.InitImages(_LITEDB, items);
            if (cols == null)
                cols = new List<string>() { "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot", "ImdbId" };

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
                html.Append(" <html><body><table>");


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
                                        html.Append("<img height='100px' width='75px' src=\'data:image/jpg;base64," + Convert.ToBase64String(bytes) + "' />");
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
            { html.Append("<table class='table table-responsive' border='1'/>"); }

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
    }
}