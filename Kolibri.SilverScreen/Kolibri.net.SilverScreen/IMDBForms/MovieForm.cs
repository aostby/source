using com.sun.java.swing.plaf.motif.resources;
using javax.xml.crypto;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using MoviesFromImdb.Controller;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.IMDBForms
{
    public partial class MovieForm : Form
    {
        FileInfo _info = null;
        LiteDBController _liteDB;
        OMDBController _omdbController;
        //string _apiKey = "e17f08db";
        UserSettings _userSettings;
        IMDBDAL _IMDBDAL;
        public MovieForm(UserSettings userSettings)
        {
            _userSettings = userSettings;
            InitializeComponent();
            Init();
        }
        public MovieForm(UserSettings userSettings, FileInfo info, string year="")
        {
            _info = info;
            _userSettings = userSettings;
            InitializeComponent();
            this.Text = $"File: {info.Name}";
            
            Init();
            tbSearch.Text = MovieUtilites.GetMovieTitle(info.FullName);
            tbYearParameter.Text = year;
            if(string.IsNullOrWhiteSpace(year)) tbYearParameter.Text = MovieUtilites.GetYear(info.Directory.FullName).ToString();
            buttonUpdate.Visible = true;

        }
        public MovieForm(UserSettings userSettings, Item item)
        {
            _userSettings = userSettings;
            InitializeComponent();
            Init();
            tbSearch.Text = item.Title;
            tbYearParameter.Text = item.Year.EndsWith('–') ? item.Year.TrimEnd('–') : item.Year;
            if (item.Type.Equals("series") & item.Year.Contains('–'))
            {
                tbYearParameter.Text = item.Year.Substring(0, item.Year.IndexOf('–'));
            }
            try
            {
                //          _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
                this.Text = $"{Assembly.GetExecutingAssembly().GetName().Name} - {_userSettings.FavoriteWatchList}";
                var list = _liteDB.WishListFindAll().ToList();
                var names = list.Select(c => c.WatchListName.ToString()).Distinct();

                comboBox1.DataSource = names.ToList();
                if (names.Contains(_userSettings.FavoriteWatchList))
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(_userSettings.FavoriteWatchList);
            }
            catch (Exception)
            { }


            btnSearch_Click(btnSearch, null);

        }

        private void Init()
        {
            if (_liteDB == null)
            {
                _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
            }
            //https://www.c-sharpcorner.com/article/autocomplete-textbox-in-C-Sharp/
            _IMDBDAL = new IMDBDAL(_liteDB);

            tbSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            try
            {
                tbSearch.AutoCompleteCustomSource = ToAutoCompleteStringCollection
                            (_liteDB.FindAllItems().GetAwaiter().GetResult().Select(s => s.Title).ToList());
            }
            catch (Exception ex)
            { }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int YearParameterNumber;
            int.TryParse(tbYearParameter.Text.Trim().TrimEnd('-'), out YearParameterNumber);

            #region tomt søk - retur med beskjed
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                Random rand = new Random();
                var tmp = _liteDB.FindAllItems().GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(tbYearParameter.Text))
                {
                    tmp = tmp.Where(m => m.Year == tbYearParameter.Text);
                }

                var movi = tmp.Skip(rand.Next(tmp.Count())).FirstOrDefault();
                tbSearch.Text = movi.Title;
                tbYearParameter.Text = movi.Year;
                MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); return;
            }
            #endregion

            var parameter = "t"; //title
            if (tbSearch.Text.StartsWith("tt", StringComparison.OrdinalIgnoreCase)) { parameter = "i"; } //id //https://www.omdbapi.com/ - options
            string url = $@"http://www.omdbapi.com/?{parameter}={tbSearch.Text.Trim()}";
            if (parameter.Equals("t") && YearParameterNumber>1800) { url += "&y=" + tbYearParameter.Text.Trim(); }
            url += $"&apikey={_userSettings.OMDBkey}";

            string json = null;

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                try
                {
                    string year = tbYearParameter.Text.Trim().TrimEnd('-').ToInt32().ToString();
                    Item sItem = parameter == "t" ? _liteDB.FindItemByTitle(tbSearch.Text.Trim().FirstToUpper(),YearParameterNumber) : _liteDB.FindItem(tbSearch.Text.Trim().FirstToUpper());
                    if (sItem != null && sItem.Year.TrimEnd('–').ToInt32() != YearParameterNumber) { sItem = null; }

                    if (sItem == null)
                    {
                        json = wc.DownloadString(url);
                        //  var result = JsonConvert.DeserializeObject<WatchList>(json);
                        sItem = JsonConvert.DeserializeObject<Item>(json);
                    }
                    if (sItem!=null)
                    {
                        tbTitle.Text = sItem.Title;
                        tbYear.Text = sItem.Year;
                        tbRated.Text = sItem.ImdbRating;
                        tbRuntime.Text = sItem.Runtime;
                        tbGenre.Text = sItem.Genre;
                        tbActors.Text = sItem.Actors;
                        tbPlot.Text = sItem.Plot;
                        tbMetascore.Text = sItem.Metascore;
                        pbPoster.ImageLocation = sItem.Poster;
                        labelImdbId.Text = sItem.ImdbId;
                        labelImdbRating.Text = sItem.ImdbRating;
                        var f= _liteDB.FindFile(labelImdbId.Text);
                        FileItem file = f.Result;

                        linkLabelOpenFilePath.BackColor = file==null ?Color.Yellow:( (file!=null&&file.ItemFileInfo.Exists)? Control.DefaultBackColor:Color.LightSalmon);
                    
                        try
                        {
                            string alternative = string.Empty;
                            using (TMDBController tmdbC = new TMDBController(_liteDB, _userSettings.TMDBkey))
                            {
                               var t= Task.Run(()=>tmdbC.FindById(labelImdbId.Text));
                             var res = t.Result;
                                if (sItem.Type.StartsWith("serie", StringComparison.OrdinalIgnoreCase))
                                {
                                    var tmpTV = tmdbC.GetTVShow(res.TvResults.FirstOrDefault().Id);
                                }
                                else
                                {

                                    var tmpMov = tmdbC.GetMovie(res.MovieResults.FirstOrDefault().Id);
                                    
                                    labelOmdbId.Text = tmpMov.Id.ToString();
                                      alternative =    tmpMov.AlternativeTitles.JsonSerializeObject().TrimEnd("null".ToCharArray());
                                    toolTip1.SetToolTip(tbTitle, alternative);

                                }
                            }
                            toolTip1.SetToolTip(tbTitle, alternative);
                        }
                        catch (Exception)
                        {   toolTip1.SetToolTip(tbTitle, string.Empty);
                        }
                     


                    }
                    else { MessageBox.Show("Movie not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Movie not found! {ex.Message}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnAddToWatchlist_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbTitle.Text) && string.IsNullOrEmpty(tbYear.Text) && string.IsNullOrEmpty(tbRated.Text) && string.IsNullOrEmpty(tbRuntime.Text) && string.IsNullOrEmpty(tbGenre.Text) && string.IsNullOrEmpty(tbActors.Text) && string.IsNullOrEmpty(tbPlot.Text) && string.IsNullOrEmpty(tbMetascore.Text) && string.IsNullOrEmpty(pbPoster.ImageLocation))
            {
                MessageBox.Show("Fields cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Image img = pbPoster.Image;
            byte[] arr;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

            WatchList obj = new WatchList()
            {
                Title = tbTitle.Text,
                Year = tbYear.Text,
                Rated = tbRated.Text,
                Runtime = tbRuntime.Text,
                Genre = tbGenre.Text,
                Actors = tbActors.Text,
                Plot = tbPlot.Text,
                Metascore = tbMetascore.Text,
                Poster = pbPoster.ImageLocation,
                Picture = arr,
                Trailer = TrailerUrl(labelImdbId.Text),
                ImdbId = labelImdbId.Text,
                ImdbRating = labelImdbRating.Text,
                Watched = "N"

            };
            obj.WatchListName = comboBox1.SelectedValue.ToString();

            _IMDBDAL.AddMovie(obj);

        }

        private void btnWatchList_Click(object sender, EventArgs e)
        {
            WatchlistForm frm = new WatchlistForm(_liteDB, comboBox1.SelectedValue.ToString());
            frm.ShowDialog();
        }

        private void brnRefresh_Click(object sender, EventArgs e)
        {
            tbActors.Text = tbGenre.Text = tbMetascore.Text = tbPlot.Text = tbRated.Text = tbRuntime.Text = tbSearch.Text = tbTitle.Text = tbYear.Text = tbYearParameter.Text = pbPoster.ImageLocation = string.Empty;
        }

        private void MovieForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) btnSearch.PerformClick();
        }

        private void btnTop100_Click(object sender, EventArgs e)
        {
            Top100IMDbForm frm = new Top100IMDbForm(_liteDB);
            frm.ShowDialog();
        }
        private void btnRecommend_Click(object sender, EventArgs e)
        {
            try
            {
                RecomendMovie(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void RecomendMovie(object sender, EventArgs e)
        {
            string type = "movie"; int number = 1000; string genre = tbGenre.Text;
            string[] words = genre.Split(',');
            if (_liteDB == null)
            {
                LiteDBController _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
            }
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                var movies = _liteDB.FindAllItems().GetAwaiter().GetResult().ToList()
                    .Where(m => m.ImdbRating != "N/A" && m.Type == type)
                    .OrderByDescending(o => o.ImdbRating).Take(number).ToList();

                Random rnd = new Random();
                int r = rnd.Next(movies.Count);

                string url = "http://www.omdbapi.com/?i=" + movies[r].ImdbId.Trim() + $"&apikey={_userSettings.OMDBkey}";

                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var json = wc.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WatchList>(json);

                    if (result.Response == "True")
                    {
                        MovieDetailsForm frm1 = new MovieDetailsForm(_liteDB, result);
                        frm1.Show();

                    }
                    else
                    {
                        MessageBox.Show("Movie not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                return;
            }
            else
            {
                Top100IMDbForm frm = new Top100IMDbForm(_liteDB, tbTitle.Text, tbYear.Text.Substring(0, 4).ToInt32());
                frm.ShowDialog();
            }

        }
        private string TrailerUrl(string imdbId = null)
        {
            if (!string.IsNullOrEmpty(imdbId))
            {
                return $"https://www.imdb.com/title/{imdbId}";
            }

            return @"https://www.imdb.com/";

        }
        private void linkLabelOpenFilePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            (sender as LinkLabel).BackColor = Control.DefaultBackColor;
            if (string.IsNullOrEmpty(tbSearch.Text) || string.IsNullOrEmpty(tbYear.Text))
            {
                MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var url = _liteDB.FindFile(labelImdbId.Text);
                FileUtilities.OpenFolderHighlightFile(url.GetAwaiter().GetResult().ItemFileInfo);
            }
            catch (Exception ex)
            {
                (sender as LinkLabel).BackColor = Color.Red;
                if (checkBoxLookUp.Checked)
                {
                    {
                        FileInfo info = FileUtilities.LetOppFil(new DirectoryInfo(_userSettings.UserFilePaths.MoviesDestinationPath), $"Finn fil som matcher {labelImdbId.Text}");
                        if (info.Exists)
                        {
                            FileItem item = new FileItem(labelImdbId.Text, info.FullName);
                            _liteDB.Upsert(item);
                        }
                    }
                }
            }
        }

        private void linkTrailer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {


            try
            {
                if (string.IsNullOrEmpty(tbSearch.Text) || string.IsNullOrEmpty(tbYear.Text))
                {
                    MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var url = TrailerUrl(labelImdbId.Text);

                //  System.Diagnostics.Process.Start(url);
                FileUtilities.Start(new Uri(url));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private AutoCompleteStringCollection ToAutoCompleteStringCollection(IEnumerable<string> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");
            var autoComplete = new AutoCompleteStringCollection();
            foreach (var item in enumerable) autoComplete.Add(item);
            return autoComplete;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show($"{_liteDB.ConnectionString.Filename}", "Data is stored at");
            }
            catch (Exception ex)
            { }
        }

        private void buttonNewList_Click(object sender, EventArgs e)
        {
            try
            {
                string newName = $"{DateTime.Now.ToShortDateString()}_list";

                var res = Kolibri.net.Common.FormUtilities.Forms.InputDialogs.InputBox("Set name for new list", "Please set a name for new item", ref newName);
                if (res == DialogResult.OK)
                {
                    var list = comboBox1.Items.Cast<string>().ToList();
                    list.Insert(0, newName);
                    comboBox1.DataSource = null;
                    comboBox1.DataSource = list;

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Visible) return;
            try
            {
                _userSettings.FavoriteWatchList = (sender as ComboBox).SelectedValue.ToString();
                _userSettings.Save();
            }
            catch (Exception ex)
            {
            }

        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(labelImdbId.Text))
                {
                    string imdbId = labelImdbId.Text;
                    Item mov = _liteDB.FindItem(imdbId);
                    if (mov != null)
                    {
                        if (_info != null)
                        {
                            mov.TomatoUrl = _info.FullName;
                            _liteDB.Upsert(new FileItem(imdbId, _info.FullName));
                        }
                        _liteDB.Update(mov);
                    }
                    else if (mov == null)
                    {
                        OMDBController oMDB = new OMDBController(_userSettings.OMDBkey, _liteDB);
                        mov = oMDB.GetItemByImdbId(imdbId);
                        if (mov != null)
                        {
                            if (_info != null)
                            {
                                mov.TomatoUrl = _info.FullName;
                                var f = _liteDB.Upsert(new FileItem(imdbId, _info.FullName));
                            }
                            var i = _liteDB.Upsert(mov);
                        }
                        else
                        {
                            throw new Exception($"no movie found: {tbSearch.Text}");
                        }

                    }
                    if (mov != null)
                    {
                        string movfile = _info == null ? string.Empty : _info.Name;
                        MessageBox.Show($"{movfile} has been updated.", $"({mov.Type}): {imdbId} - {mov.Title}");
                        //buttonUpdate.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void tbYearParameter_TextChanged(object sender, EventArgs e)
        {
            if (tbYearParameter.Text.Length == 0) { tbYearParameter.BackColor = Color.White; }
            else { tbYearParameter.BackColor = tbYearParameter.Text.Length == 4 ? Color.White : Color.LightSalmon; }
        }
    }
}

