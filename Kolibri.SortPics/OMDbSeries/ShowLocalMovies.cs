using OMDbApiNet.Model;
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
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kolibri.VisualizeOMDbItem
{
    public partial class ShowLocalMovies : Form
    {
        // static HttpClient client = new HttpClient();
        private static List<Item> movies = new List<Item>();
        private static IDictionary<string, string> TYPE_TRANSLATION = new Dictionary<string, string> { { "movie", "film" }, { "series", "serie" }, { "episode", "episode" }, { "game", "spill" }, { "rating", "rating" } };
        private static IEnumerable<Item> _serieItems;

        private ListViewColumnSorter lvwColumnSorter;

        public ShowLocalMovies(string defaultSeach = null, IEnumerable<Item> seriesList = null)
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            movieList.ListViewItemSorter = lvwColumnSorter;


            //client.BaseAddress = new Uri("http://omdbapi.com/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            resultsToGet.SelectedIndex = 1;
            pictureBox.Image = Properties.Resources.no_image;
            _serieItems = seriesList.ToList();
            if (!string.IsNullOrEmpty(defaultSeach))
            {
                searchTxt.Text = defaultSeach;

            }
            resultsToGet.SelectedIndex = resultsToGet.FindStringExact("500");
            detailsViewBtn_Click(null, null);
            searchBtn_Click(null, null);

        }
        public static async Task<ItemSearch> searchMovies(string title, int page)
        {
            ItemSearch movies = null;
            try
            {
                if (string.IsNullOrEmpty(title))
                    movies = new ItemSearch() { Search = _serieItems.Take(page).ToArray() };
                else
                {
                    var search = _serieItems.Where(x => x.Title.ToLower().Contains(title.ToLower())).Take(page).ToArray();
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

        public Image getImage(string url, int? toGet = 0)
        {
            Image img;
            try
            {
                if (toGet > 200)
                    img = Properties.Resources.no_image;
                else
                {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(url);
                    MemoryStream ms = new MemoryStream(bytes);
                    img = Image.FromStream(ms);
                }
            }
            catch
            {
                img = Properties.Resources.no_image;
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
            try
            {



                int totalResults = 10, gotResults = 0, toGet = int.Parse(resultsToGet.SelectedItem.ToString()), page = 1;
                disableSearch();
                movies = new List<Item>();
                while (gotResults < Math.Min(totalResults, toGet))
                {
                    ItemSearch result = await searchMovies(searchTxt.Text, toGet);

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
            catch (Exception ex)
            {
            }
        }

        private void refreshMovieList()
        {
            try
            {
                movieList.Items.Clear();
                movieImageList.Images.Clear();
                movieImageList = new ImageList();
                foreach (Item movie in movies)
                {
                    movieImageList.Images.Add(movie.ImdbId, getImage(movie.Poster, int.Parse(resultsToGet.SelectedItem.ToString())));
                    ListViewItem item = new ListViewItem(movie.Title, movie.ImdbId);
                    item.Tag = movie;   
                    movieList.Items.Add(item);
                    item.SubItems.Add(movie.Year);
                    item.SubItems.Add(movie.ImdbRating);
                    item.SubItems.Add(movie.Genre);
                    item.SubItems.Add(TYPE_TRANSLATION[movie.Type]);
                    item.SubItems.Add(movie.Poster);
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
            catch (Exception)
            { }

        }
        private void SetNotFoundProperties()
        {
            try
            {
                Item result = new Item() { Title = $"NotFound {searchTxt.Text}" };

                pictureBox.Image = Properties.Resources.no_image;
                titleContentLabel.Text = result.Title;
                productionContentLabel.Text = result.Production;
                directorContentLabel.Text = result.Director;
                countryContentLabel.Text = result.Country;
                yearContentLabel.Text = result.Year;
                totalContentLabel.Text = result.Dvd;
                typeContentLabel.Text = result.Type;
                runtimeContentLabel.Text = result.Runtime;
                plotContentLabel.Text = result.Plot;

            }
            catch (Exception)
            {


            }

        }
        private async void movieList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                if (!e.IsSelected)
                    return;
                if (e.Item != null && e.Item.SubItems != null && e.Item.SubItems.Count >= 4)
                    pictureBox.Image = getImage(e.Item.SubItems[e.Item.SubItems.Count-1] .Text);
                else
                    pictureBox.Image = Properties.Resources.no_image;
                Item result = await getMovieDetails(e.Item.ImageKey);
                if (result == null)
                {
                    SetNotFoundProperties();
                    return;
                }
                titleContentLabel.Text = result.Title;
                productionContentLabel.Text = result.Production;
                directorContentLabel.Text = result.Director;
                countryContentLabel.Text = result.Country;
                yearContentLabel.Text = result.Year;
                totalContentLabel.Text = result.TotalSeasons;
                typeContentLabel.Text = result.Type;
                runtimeContentLabel.Text = result.Runtime;
                plotContentLabel.Text = result.Plot;
            }
            catch (Exception ex) { }

        }

        private void movieList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            movieList.Sort();
        }

        private void movieList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
               
                    ListViewHitTestInfo info = movieList.HitTest(e.X, e.Y);
                    ListViewItem item = info.Item;

                    if (item != null)
                    {
                    Item selItem =(Item)item.Tag ;
                    Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(selItem.TomatoUrl));

                      
                    }
                    else
                    {
                        this.movieList.SelectedItems.Clear();
                        //MessageBox.Show("No Item is selected");
                    }
                
            }
            catch (Exception)
            {

             
            }
        }
    }
}