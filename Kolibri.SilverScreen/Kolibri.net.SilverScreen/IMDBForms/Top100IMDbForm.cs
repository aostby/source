
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using System.Windows.Forms;


namespace Kolibri.net.SilverScreen.IMDBForms
{
    public partial class Top100IMDbForm : Form
    {
        LiteDBController _liteDB;
        public Top100IMDbForm(LiteDBController liteDB, string title = "", int year = 0)
        {
            _liteDB = liteDB;
            InitializeComponent();
            if (string.IsNullOrEmpty(title))
            {
                top100Movies();
            }
            else RecomendMovie(title, year);
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
                movies = movies.Replace("imdbRating", "Rank");
                var result = JsonConvert.DeserializeObject<List<Top100IMDb>>(movies);

                gridTop100.AutoGenerateColumns = true;
                gridTop100.DataSource = result;

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
            //if (!File.Exists(@"C:\Users\your\Documents\TestApp\MoviesFromImdb\top100.json"))
            //{
            //    MessageBox.Show("File top100.json not exist!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //string json = File.ReadAllText(@"C:\Users\your\Documents\TestApp\MoviesFromImdb\top100.json");
            //LiteDBController _liteDB = new LiteDBController( false, false, false);
            //var serializer = new JavaScriptSerializer();
            //var movies = serializer.Serialize(_liteDB.FindAllItems().ToList()
            var movies =
                JsonConvert.SerializeObject(
                    _liteDB.FindAllItems().GetAwaiter().GetResult()
                .Where(m => m.ImdbRating != "N/A")
                .OrderByDescending(o => o.ImdbRating)
                .Take(number).ToList(), Formatting.Indented);
            movies = movies.Replace("ImdbRating", "Rank");
            var result = JsonConvert.DeserializeObject<List<Top100IMDb>>(movies);


            gridTop100.AutoGenerateColumns = true;
            gridTop100.DataSource = result;

            this.gridTop100.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridTop100.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridTop100.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridTop100.Parent.Text = $"Top {gridTop100.Rows.Count} movies and series";

        }


        private void miMovieDetails_Click(object sender, EventArgs e)
        {
            string tt = gridTop100[gridTop100.ColumnCount - 2, gridTop100.CurrentCell.RowIndex].Value.ToString().Trim();
            //string tt = gridTop100.SelectedRows[0].Cells["ImdbId"].Value.ToString();

            string url = "http://www.omdbapi.com/?i=" + tt + "&apikey=e17f08db";

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var json = wc.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WatchList>(json);

                if (result.Response == "True")
                {
                    MovieDetailsForm frm = new MovieDetailsForm(_liteDB, result);
                    frm.Show();

                }
                else
                {
                    MessageBox.Show($"Movie (tt = {tt}) not found!", "Information - " + System.Reflection.MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

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