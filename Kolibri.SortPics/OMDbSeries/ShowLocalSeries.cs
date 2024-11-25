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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.VisualizeOMDbItem
{
    public partial class ShowLocalSeries : Form
    {
       // static HttpClient client = new HttpClient();
        static List<Item> movies = new List<Item>();
        const string API_KEY = "be7b1bec";
        static IDictionary<string, string> TYPE_TRANSLATION = new Dictionary<string, string> { { "movie", "film" }, { "series", "serie" }, { "episode", "episode" }, { "game", "spill" } };
     static   IEnumerable<Item> _serieItems;

        public ShowLocalSeries(string defaultSeach=null, IEnumerable<Item> seriesList = null)
        {
            InitializeComponent();
            //client.BaseAddress = new Uri("http://omdbapi.com/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            resultsToGet.SelectedIndex = 1;
            pictureBox.Image = Properties.Resources.no_image;
             _serieItems = seriesList.ToList();   
            if(!string.IsNullOrEmpty( defaultSeach))
            {
             searchTxt.Text = defaultSeach;
               
            }
            detailsViewBtn_Click(null, null);
            searchBtn_Click(null, null);
         
        }
        public static async Task<ItemSearch> searchMovies(string title, int page)
        {
            ItemSearch movies = null;
            try
            {
                if (string.IsNullOrEmpty(title))
                    movies = new ItemSearch() { Search = _serieItems.ToArray() };
                else {
                    var search = _serieItems.Where(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).ToArray();
                    movies = new ItemSearch() { Search = search }; 
                } 
            }
            catch {}
            return movies;
        }

        public static async Task<Item> getMovieDetails(string id)
        {
             Item item = null;
            try
            {
                item = _serieItems.First(x => x.ImdbId== id);
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
          
            int totalResults = 10, gotResults = 0, toGet = int.Parse(resultsToGet.SelectedItem.ToString()), page = 1;
            disableSearch();
            movies = new List<Item>();
            while (gotResults<Math.Min(totalResults, toGet))
            {
                ItemSearch result = await searchMovies(searchTxt.Text, page++);
           
                  if (result==null||result.Search == null|| result.totalResults<1)
                {
                    SetNotFoundProperties();
                    
                    enableSearch();
                    return;
                }
                gotResults += result.Search.Count();
                totalResults =  result.totalResults;
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
                    movieImageList.Images.Add(movie.ImdbId, getImage(movie.Poster));
                    movieList.Items.Add(movie.Title, movie.ImdbId);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Year);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(TYPE_TRANSLATION[movie.Type]);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Poster);
                }
                movieList.Focus();
                if(movieList.Items.Count > 0)
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
            {}
           
        }
        private async void movieList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;
            pictureBox.Image = getImage(e.Item.SubItems[3].Text);
            Item result = await getMovieDetails(e.Item.ImageKey);
            if (result==null)
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
    }
}
