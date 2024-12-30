using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using MoviesFromImdb.Controller;
 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Kolibri.net.SilverScreen.IMDBForms
{
    public partial class MovieDetailsForm : Form
    {
        private BindingSource _bsMovies;
        private LiteDBController _liteDB;
        private IMDBDAL _IMDBDAL;
        public MovieDetailsForm(LiteDBController liteDB)
        {
            _liteDB = liteDB;
            InitializeComponent();
            Init();
        }

        public MovieDetailsForm(LiteDBController liteDB, WatchList obj)
        {_liteDB = liteDB;  
            _IMDBDAL = new IMDBDAL(_liteDB);
            InitializeComponent();
            tbTitle.Text = obj.Title;
            tbYear.Text = obj.Year.EndsWith('-') ? obj.Year.TrimEnd('-'): obj.Year;
            tbRated.Text = obj.ImdbRating;
            tbRuntime.Text = obj.Runtime;
            tbGenre.Text = obj.Genre;
            tbActors.Text = obj.Actors;
            tbPlot.Text = obj.Plot;
            tbMetascore.Text = obj.Metascore;
            pbPoster.ImageLocation = obj.Poster;
            linkTrailer.Tag = obj.ImdbId;
            btnLeft.Hide();
            btnRight.Hide();
            
            Init(obj.ImdbId);
        }

        public MovieDetailsForm(BindingSource bsMovies)
        {
            InitializeComponent();
            btnAddToWatchlist.Hide();
            _bsMovies = bsMovies;
            FillUpFields(_bsMovies);
            Init();
       
        }

        private void Init(string imdbId=null) {
            linkLabelOpenFilePath.Visible = false;
            if (!string.IsNullOrWhiteSpace(imdbId))
            { 
               var tmp =  _liteDB.FindFile(imdbId);
                if (tmp != null) { linkLabelOpenFilePath.Tag = tmp.FullName; linkLabelOpenFilePath.Visible = true;

                    this.Text = $"{this.Text} - {Path.GetFileNameWithoutExtension(tmp.FullName)}";
                }
                else {
                   var tmpM=  _liteDB.FindItem(imdbId);
                    if (tmpM == null)
                    {
                        this.BackColor = Color.LightSalmon;
                    }
                    else
                        this.Text = $"{this.Text} - {tmpM.Title} (local path unknown)";
                }

            }
        }

        private void FillUpFields(BindingSource bsMovies)
        {
            DataRowView drv = bsMovies.Current as DataRowView;

            if (drv != null)
            {
                tbTitle.Text = drv["Title"].ToString();
                tbYear.Text = drv["Year"].ToString();
                tbRuntime.Text = drv["Runtime"].ToString();
                tbRated.Text = drv["imdbRating"].ToString();
                tbGenre.Text = drv["Genre"].ToString();
                tbActors.Text = drv["Actors"].ToString();
                tbPlot.Text = drv["Plot"].ToString();
                tbMetascore.Text = drv["Metascore"].ToString();
                pbPoster.ImageLocation = drv["Poster"].ToString();
                linkTrailer.Text = drv["trailer"].ToString();

                btnLeft.Enabled = _bsMovies.IndexOf(drv) != 0;
                btnRight.Enabled = _bsMovies.IndexOf(drv) != _bsMovies.Count - 1;
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            _bsMovies.MovePrevious();
            FillUpFields(_bsMovies);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            _bsMovies.MoveNext();
            FillUpFields(_bsMovies);
        }

        private void MovieDetailsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Right) btnRight.PerformClick();

            if (e.KeyCode == Keys.Left) btnLeft.PerformClick();
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
                Picture = arr
            };

            _IMDBDAL.AddMovie(obj);
        }
        private void linkLabelOpenFilePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(linkTrailer.Text);
                FileUtilities.Start(new FileInfo($"{linkLabelOpenFilePath.Tag}"));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }
        private void linkTrailer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(linkTrailer.Text);
                FileUtilities.Start(new Uri($"https://www.imdb.com/title/{linkTrailer.Tag}"));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
      
        }

     
    }
}
