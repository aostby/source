using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using MoviesFromImdb.Controller;
using Newtonsoft.Json;
using OMDbApiNet.Model;
using System.Data;
using System.Net;
using System.Text; 

namespace Kolibri.net.SilverScreen.IMDBForms
{
    public partial class MovieForm : Form
    {
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
        public MovieForm(UserSettings userSettings, Item item)
        {
            _userSettings = userSettings;
            InitializeComponent();
              Init(); 
            tbSearch.Text = item.Title;
            tbYearParameter.Text = item.Year.EndsWith('–') ? item.Year.TrimEnd('–') : item.Year;
            if (item.Type.Equals("series") & item.Year.Contains('–')) {
                tbYearParameter.Text = item.Year.Substring(0,item.Year.IndexOf('–'));
            }
            btnSearch_Click(btnSearch, null);
      
        }

        private void Init()
        {
            _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
            //https://www.c-sharpcorner.com/article/autocomplete-textbox-in-C-Sharp/
            _IMDBDAL = new IMDBDAL(_liteDB); 

            tbSearch.AutoCompleteMode= AutoCompleteMode.SuggestAppend;
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

                // MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); return;
            }



            string url;
            if (string.IsNullOrEmpty(tbYearParameter.Text))
            {
                url = "http://www.omdbapi.com/?t=" + tbSearch.Text.Trim() + $"&apikey={_userSettings.OMDBkey}";
            }
            else
            {

                int YearParameterNumber;
                if (!int.TryParse(tbYearParameter.Text, out YearParameterNumber))
                {
                    MessageBox.Show("The Year format is bad;Enter only year!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                url = "http://www.omdbapi.com/?t=" + tbSearch.Text.Trim() + "&y=" + tbYearParameter.Text.Trim() + $"&apikey={_userSettings.OMDBkey}";
            }

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var json = wc.DownloadString(url);
                        var result = JsonConvert.DeserializeObject<WatchList>(json);

                if (result.Response == "True")
                {
                    tbTitle.Text = result.Title;
                    tbYear.Text = result.Year;
                    tbRated.Text = result.ImdbRating;
                    tbRuntime.Text = result.Runtime;
                    tbGenre.Text = result.Genre;
                    tbActors.Text = result.Actors;
                    tbPlot.Text = result.Plot;
                    tbMetascore.Text = result.Metascore;
                    pbPoster.ImageLocation = result.Poster;
                    labelImdbId.Text = result.ImdbId;
                    labelImdbRating.Text = result.ImdbRating;

                  linkLabelOpenFilePath.BackColor = Control.DefaultBackColor;
                }
                else
                {
                    MessageBox.Show("Movie not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                ImdbRating = labelImdbRating.Text ,
            Watched = "N"
                
            };

            _IMDBDAL.AddMovie(obj);

        }

        private void btnWatchList_Click(object sender, EventArgs e)
        {
            WatchlistForm frm = new WatchlistForm(_liteDB);
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
        {     string type = "movie"; int number = 1000;    string genre = tbGenre.Text;
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
                Top100IMDbForm frm = new Top100IMDbForm(_liteDB, tbTitle.Text,tbYear.Text.Substring(0,4).ToInt32());
                frm.ShowDialog();
            } 

        }

        private string TrailerUrl(string imdbId=null)
        {
            if (!string.IsNullOrEmpty(imdbId)) {
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
                FileUtilities.OpenFolderHighlightFile(url.ItemFileInfo);
            }
            catch (Exception ex)
            {
                (sender as LinkLabel).BackColor= Color.Red;
                 
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
        private AutoCompleteStringCollection ToAutoCompleteStringCollection(
           IEnumerable<string> enumerable)
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
    }
}

