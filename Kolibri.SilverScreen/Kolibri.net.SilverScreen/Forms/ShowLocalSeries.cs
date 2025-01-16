using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Images.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.VisualizeOMDbItem
{
    public partial class ShowLocalSeries : Form
    {
        // static HttpClient client = new HttpClient();
        static List<Item> movies = new List<Item>();
        const string API_KEY = "be7b1bec";
        static IDictionary<string, string> TYPE_TRANSLATION = new Dictionary<string, string> { { "movie", "film" }, { "series", "serie" }, { "episode", "episode" }, { "game", "spill" } };
        static IEnumerable<Item> _serieItems;
        private UserSettings _userSettings;
        private ImageCache _imgCache;

        public ShowLocalSeries(IEnumerable<Item> seriesList, string defaultSeach = null)
        {
            InitializeComponent();
            _serieItems = seriesList.OrderByDescending(x => x.ImdbRating).ToList();
            Init(defaultSeach);
        }
        public ShowLocalSeries(UserSettings userSettings)
        {
            InitializeComponent();
            this._userSettings = userSettings;

            using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
            {
                var seriesList = tmp.GetAllItemsByType("Series");

                _serieItems = seriesList.OrderByDescending(x => x.ImdbRating).ToList();
            }

            Init();

        }

        private void Init(string defaultSeach = null)
        {

            pictureBox.Image = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
            searchBtn.BackgroundImage = ImageUtilities.Base64ToImage(ImageUtilities.SearchGlassImage());
            var img = Icons.GetFolderIcon().ToBitmap();
            img.RotateFlip(RotateFlipType.Rotate270FlipX);
            buttonLookUp.BackgroundImage = img;


            if (!string.IsNullOrEmpty(defaultSeach))
            {
                searchTxt.Text = defaultSeach;

            }
            detailsViewBtn_Click(null, null);
            resultsToGet.SelectedIndex = 1;
            //searchBtn_Click(null, null);
            _imgCache = new ImageCache(_userSettings);

            this.Text = $"Series list count: {_serieItems.Count()}";
        }


        public static async Task<ItemSearch> searchMovies(string title, int page)
        {
            ItemSearch movies = null;
            try
            {
                if (string.IsNullOrEmpty(title))
                    movies = new ItemSearch() { Search = _serieItems.ToArray() };
                else
                {
                    var search = _serieItems.Where(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).ToArray();
                    movies = new ItemSearch() { Search = search };
                }
            }
            catch { }
            return movies;
        }

        public static async Task<Item> getMovieDetails(string id)
        {
            Item item = null;
            try
            {
                item = _serieItems.First(x => x.ImdbId == id);
            }
            catch { }
            return item;
        }

        public Image getImage(string url)
        {
            Image img;
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(url);
                MemoryStream ms = new MemoryStream(bytes);
                img = Image.FromStream(ms);
            }
            catch
            {
                img = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
            }
            return img;
        }

        private void disableSearch()
        {
            searchBtn.Enabled = false;
            searchTxt.Enabled = false;
            movieList.Enabled = false;
            resultsToGet.Enabled = false;
        }

        private void enableSearch()
        {
            searchBtn.Enabled = true;
            searchTxt.Enabled = true;
            movieList.Enabled = true;
            resultsToGet.Enabled = true;
        }

        private async void searchBtn_Click(object sender, EventArgs e)
        {

            int totalResults = 10, gotResults = 0, toGet = int.Parse(resultsToGet.SelectedItem.ToString()), page = 1;
            disableSearch();
            movies = new List<Item>();
            while (gotResults < Math.Min(totalResults, toGet))
            {
                ItemSearch result = await searchMovies(searchTxt.Text, page++);

                if (result == null || result.Search == null || result.totalResults < 1)
                {
                    SetNotFoundProperties();

                    enableSearch();
                    return;
                }
                gotResults += result.Search.Count();
                totalResults = result.totalResults;
                movies.AddRange(result.Search);
            }
            enableSearch();
            refreshMovieList();
        }
        private void refreshMovieList()
        {
            try
            {
                movieList.Items.Clear();
                movieImageList.Images.Clear();
                foreach (Item movie in movies)
                {
            var jall =         _imgCache.FindImage(movie.ImdbId);
                    if (jall == null)
                    {
                        var img = getImage(movie.Poster);
                        movieImageList.Images.Add(movie.ImdbId, img);
                        _imgCache.InsertImage(movie.ImdbId, (Bitmap)img);
                    }
                    else { movieImageList.Images.Add(movie.ImdbId,(Image)jall.Image); }
                    movieList.Items.Add(movie.Title, movie.ImdbId);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Year);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(TYPE_TRANSLATION[movie.Type]);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.ImdbRating);

                    //Legg til elementer over, den siste i listen må være poster pga display - movieList_ItemSelectionChanged
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Poster);
                }
                movieList.Focus();
                if (movieList.Items.Count > 0)
                    movieList.Items[0].Selected = true;
            }
            catch (Exception ex) { }

        }

        private void tileViewBtn_Click(object sender, EventArgs e)
        {
            movieList.View = View.LargeIcon;
            tileViewBtn.Enabled = false;
            detailsViewBtn.Enabled = true;
            refreshMovieList();
        }

        private void detailsViewBtn_Click(object sender, EventArgs e)
        {
            try
            {
                movieList.View = View.Details;
                detailsViewBtn.Enabled = false;
                tileViewBtn.Enabled = true;
            }
            catch (Exception ex)
            {
            }

        }
        private void SetNotFoundProperties()
        {
            try
            {
                Item result = new Item() { Title = $"NotFound {searchTxt.Text}" };

                pictureBox.Image = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
                titleContentLabel.Text = result.Title;
                productionContentLabel.Text = result.Production;
                directorContentLabel.Text = result.Director;
                countryContentLabel.Text = result.Country;
                ratingContentLabel.Text = result.ImdbRating;
                yearContentLabel.Text = result.Year;
                totalContentLabel.Text = result.Dvd;
                typeContentLabel.Text = result.Genre;  //result.Type;
                runtimeContentLabel.Text = result.Runtime;

                plotContentLabel.Text = result.Plot;

            }
            catch (Exception)
            { }

        }
        private async void movieList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;
            //pictureBox.Image = getImage(e.Item.SubItems[3].Text);
            pictureBox.Image = getImage(e.Item.SubItems[e.Item.SubItems.Count - 1].Text);
            Item result = await getMovieDetails(e.Item.ImageKey);
            if (result == null)
            {
                SetNotFoundProperties();
                return;
            }
            else
            {
                pictureBox.Tag = result.ImdbId;
            }
            titleContentLabel.Text = result.Title;
            productionContentLabel.Text = result.Production;
            directorContentLabel.Text = result.Director;
            countryContentLabel.Text = result.Country;
            ratingContentLabel.Text = result.ImdbRating;
            yearContentLabel.Text = result.Year;
            totalContentLabel.Text = result.TotalSeasons;
            typeContentLabel.Text = result.Genre;//  result.Type;
            runtimeContentLabel.Text = result.Runtime;
            plotContentLabel.Text = result.Plot;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                var imdbid = ((sender as PictureBox)).Tag.ToString();
                Uri uri = new Uri($"https://www.imdb.com/title/{imdbid}/");
                HTMLUtilities.OpenURLInBrowser(uri);

            }
            catch (Exception)
            {
            }
        }

        private void buttonLookUp_Click(object sender, EventArgs e)
        {
            try
            {
                var folder = FolderUtilities.LetOppMappe(_userSettings.UserFilePaths.SeriesSourcePath, "Let opp mappe med serie(r)");
                if (folder != null && folder.Exists)
                {
                    Form form = new Kolibri.net.SilverScreen.OMDBForms.OMDBSearchForSeries(folder, _userSettings);
                    form.ShowDialog();
                }

                try
                {
                    using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                    {
                        var seriesList = tmp.GetAllItemsByType("Series");

                        _serieItems = seriesList.OrderByDescending(x => x.ImdbRating).ToList();
                    }
                    Init(folder.Name);
                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void ContentLabel_Click(object sender, EventArgs e)
        {

            try
            {
                if (sender.Equals(ratingContentLabel))
                {
                    string val = ratingContentLabel.Text;
                    if (InputDialogs.InputBox("Sett verdi", "Oppdater verdien", ref val) == DialogResult.OK && val.IsNumeric())
                    {
                        Item result = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult();
                        result.ImdbRating = val.Replace(',', '.');
                        using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                        {
                            tmp.Update(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            try
            {
                Item result = null;
                LiteDBController tmp = null;
                try
                {
                    tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false);
                    result = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult();

                            var file = tmp.FindFile(result.ImdbId);
                    if (file != null)
                    {
                        if (file.ItemFileInfo.Exists)
                            FileUtilities.OpenFolderHighlightFile(file.ItemFileInfo);
                        else if (Directory.Exists(file.FullName))
                        {
                            FolderUtilities.OpenFolderInExplorer(file.FullName);
                        }
                        else { throw new FileNotFoundException($"{result.Title} - location not found"); }
                    }
                    else
                    {
                        throw new FileNotFoundException($"{result.Title} - location not found");
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, ex.GetType().Name);

                    var folder = FolderUtilities.LetOppMappe(_userSettings.UserFilePaths.SeriesSourcePath, $"{ex.Message}");
                    if (folder != null && folder.Exists && result != null)
                    {
                        result.TomatoUrl = folder.FullName;
                        tmp.Update(result);
                        tmp.Update(new FileItem(result.ImdbId, folder.FullName));
                    }

                }
                tmp.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}