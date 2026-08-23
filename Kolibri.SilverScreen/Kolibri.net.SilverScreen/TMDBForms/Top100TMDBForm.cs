
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Windows.Forms;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;


namespace Kolibri.net.SilverScreen.IMDBForms
{
    public partial class Top100TMDbForm : Form
    {
        private LiteDBController _liteDB;
        private UserSettings _userSettings;
        private SearchContainer<SearchMovie>? _templist;
        private TMDBController _tmdb;

        public Top100TMDbForm(LiteDBController liteDB, SearchContainer<SearchMovie>? templist, UserSettings userSettings = null)
        {
            InitializeComponent();
            this._templist = templist;
            _liteDB = liteDB;
            _userSettings = userSettings;
            if (_userSettings == null)
                _userSettings = _liteDB.GetUserSettings();

            if (_tmdb == null) { _tmdb = new TMDBController(_liteDB, _userSettings.TMDBkey); }


            top100Movies();
        }

        private void RecomendMovie(string title, int year)
        {
            try
            {

                Kolibri.net.Common.Dal.Controller.TMDBController contr = new TMDBController(_liteDB);
                var sim = Task.Run(() => contr.GetMovieSimilar(title, year)).Result;//vi mangler ImdbId når vi gjør dette, vi må hente en liste av Movies
                var list = Task.Run(() => contr.GetMovies(sim)).Result;
                //var serializer = new JavaScriptSerializer();
                //var movies = serializer.Serialize(list.ToList().OrderByDescending(o => o.ImdbRating).ToList());

                var movies = JsonConvert.SerializeObject(list.ToList().OrderByDescending(o => o.ImdbRating).ToList(), Formatting.Indented);

                movies = movies.Replace("ImdbRating", "Rank");
                movies = movies.Replace("ReleaseDate", "Year");
                movies = movies.Replace("Released", "Year");

                movies = movies.Replace("imdbRating", "Rank");
                //       var result = JsonConvert.DeserializeObject<List<Top100TMDb>>(movies);

                gridTop100.AutoGenerateColumns = true;
                //       gridTop100.DataSource = result;

                this.gridTop100.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;//Title
                this.gridTop100.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;//Rank
                this.gridTop100.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;//ImdbID
                gridTop100.Parent.Text = $"{System.Reflection.MethodBase.GetCurrentMethod().Name} {gridTop100.Rows.Count} based on {title} ({year})";

            }
            catch (Exception ex)
            {
            }
        }
        private void top100Movies()
        {
            int number = 100;

            var movies =
                JsonConvert.SerializeObject(_templist, Formatting.Indented);
            movies = movies.Replace("ImdbRating", "Rank");

            var list = _templist.Results.ToList();
            DataSet res = DataSetUtilities.AutoGenererTypedDataSet<SearchMovie>(list);
            var table = res.Tables[0];
            table.Columns.Add("ImdbId");
            foreach (DataRow row in table.Rows)
            {

                var id = $"{row["id"]}";

                var movie = _tmdb.GetMovie(id, false);
                row["ImdbId"] = movie?.ImdbId;
            }
            gridTop100.AutoGenerateColumns = true;

            gridTop100.DataSource = table;

            this.gridTop100.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridTop100.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridTop100.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridTop100.Parent.Text = $"Top {gridTop100.Rows.Count} movies and series";

        }


        private void miMovieDetails_Click(object sender, EventArgs e)
        {
            string tt = string.Empty;
            try
            {
                tt = gridTop100["ImdbId", gridTop100.CurrentCell.RowIndex].Value.ToString().Trim();
                //string tt = gridTop100.SelectedRows[0].Cells["ImdbId"].Value.ToString();
                
                string url = "http://www.omdbapi.com/?i=" + tt + $"&apikey={_userSettings.OMDBkey}";

                using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    var json = wc.DownloadString(url);
                    var result = JsonConvert.DeserializeObject<WatchListItem>(json);

                    if (result.Response == "True")
                    {
                        MovieDetailsForm frm = new MovieDetailsForm(_liteDB, result);
                        frm.MdiParent = this.MdiParent;

                        frm.Show();

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Movie (tt = {tt}) not found!", "Information - " + System.Reflection.MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    

        private void gridTop100_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }
            }
            else {
                try
                {
                    if (e.RowIndex != -1) {

                        var row = (((sender as DataGridView).DataSource) as DataTable).Rows[e.RowIndex];
                       var imdbid= row["ImdbId"].ToString();
                        var rrl = $"https://www.imdb.com/title/{imdbid}";
                        HTMLUtilities.OpenURLInBrowser(new Uri(rrl));

                    }
                }
                catch (Exception) { }
            
            }


        }

        private void Top100IMDbForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void gridTop100_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                string tmp = $"{gridTop100.Rows[e.RowIndex].Cells["TomatoUrl"].Value}";

                if (string.IsNullOrEmpty(tmp))
                {
                    gridTop100.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PeachPuff;
                }
            }
            catch (Exception) { }
        
        }
    }
}