using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using YoutubeSearch;

namespace MoviesFromImdb
{
    public partial class MovieForm : Form
    {
        public MovieForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string url;
            if (string.IsNullOrEmpty(tbYearParameter.Text))
            {
                url = "http://www.omdbapi.com/?t=" + tbSearch.Text.Trim() + "&apikey=e17f08db";
            }
            else
            {

                int YearParameterNumber;
                if (!int.TryParse(tbYearParameter.Text, out YearParameterNumber))
                {
                    MessageBox.Show("The Year format is bad;Enter only year!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                url = "http://www.omdbapi.com/?t=" + tbSearch.Text.Trim() + "&y=" + tbYearParameter.Text.Trim() + "&apikey=e17f08db";
            }

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var json = wc.DownloadString(url);
                var result = JsonConvert.DeserializeObject<ImdbEntity>(json);

                if (result.Response == "True")
                {
                    tbTitle.Text = result.Title;
                    tbYear.Text = result.Year;
                    tbRated.Text = result.imdbRating;
                    tbRuntime.Text = result.Runtime;
                    tbGenre.Text = result.Genre;
                    tbActors.Text = result.Actors;
                    tbPlot.Text = result.Plot;
                    tbMetascore.Text = result.Metascore;
                    pbPoster.ImageLocation = result.Poster;
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

            ImdbEntity obj = new ImdbEntity()
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
                Trailer = TrailerUrl()
            };

            DAL.AddMovie(obj);

        }

        private void btnWatchList_Click(object sender, EventArgs e)
        {
            WatchlistForm frm = new WatchlistForm();
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
            Top100IMDbForm frm = new Top100IMDbForm();
            frm.ShowDialog();
        }

        private void btnRecommend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                MessageBox.Show("Please enter movie name and click search before we recommend you a movie!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string genre = tbGenre.Text;
            string[] words = genre.Split(',');

            String name = "movies";
            String constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            @"C:\Users\luka.jovic\Downloads\movies.xls" +
                            ";Extended Properties='Excel 12.0;HDR=YES;';";

            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable data = new DataTable();
            sda.Fill(data);

            List<string> movies = new List<string>();

            #region withouth random iterator
            //foreach (DataRow row in data.Rows)
            //{
            //    if (!string.IsNullOrEmpty(row[8].ToString()))
            //    {
            //        if (row[8].ToString().Contains(words[0]))
            //        {
            //            movies.Add(row[0].ToString());
            //            if (movies.Count == 50)
            //            {
            //                break;
            //            }

            //        }

            //    }
            //}
            #endregion

            Random rndm = new Random();
            int ra = rndm.Next(1, 1000);

            for (int i = ra; i < data.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(data.Rows[i][8].ToString()))
                {
                    if (data.Rows[i][8].ToString().Contains(words[0]))
                    {
                        movies.Add(data.Rows[i][0].ToString());
                        if (movies.Count == 50)
                        {
                            break;
                        }

                    }
                }
            }

            Random rnd = new Random();
            int r = rnd.Next(movies.Count);

            string url = "http://www.omdbapi.com/?i=" + (string)movies[r].Trim() + "&apikey=e17f08db";

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var json = wc.DownloadString(url);
                var result = JsonConvert.DeserializeObject<ImdbEntity>(json);

                if (result.Response == "True")
                {
                    MovieDetailsForm frm1 = new MovieDetailsForm(result);
                    frm1.Show();

                }
                else
                {
                    MessageBox.Show("Movie not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }  
        }

        private string TrailerUrl()
        {
            string querystring = tbSearch.Text + "movie trailer" + tbYear.Text;

            int querypages = 1;

            List<string> trailer = new List<string>();

            var items = new VideoSearch();

            foreach (var item in items.SearchQuery(querystring, querypages))
            {
                trailer.Add(item.Url);
                if (trailer.Count == 1)
                {
                    break;
                }

            }

            string url = trailer[0].ToString();
            return url;

        }

        private void linkTrailer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text) || string.IsNullOrEmpty(tbYear.Text))
            {
                MessageBox.Show("Please enter movie name!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var url = TrailerUrl();

            System.Diagnostics.Process.Start(url);
        }
    }
}

