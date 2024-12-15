using Kolibri.Common.MovieAPI.Controller;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Collections;
using System.Data;
using System.Diagnostics;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class DetailsFormItem : Form
    {
        private BindingSource _bsMovies;
        internal Item _item;
        internal FileInfo _itemPath;
        internal LiteDBController _liteDB;
        internal OMDBController _OMDB;
        internal TMDBController _TMDB;

        [Obsolete("Designer only", true)] public DetailsFormItem() { InitializeComponent(); }
        public DetailsFormItem(OMDbApiNet.Model.Item item, LiteDBController contr)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            _liteDB = contr;
            _item = item;
            InitializeComponent();
            tbTitle.Text = item.Title;
            tbYear.Text = item.Year;
            tbRated.Text = item.ImdbRating;
            tbRated.BackColor = Color.Red;
            int rating = 0;
            if (item.ImdbRating.IsNumeric() && item.ImdbRating.Substring(0, 1).ToInt32() > 0)
                rating = item.ImdbRating.Substring(0, 1).ToInt32();
            if (rating >= 3 && rating <= 4) { tbRated.BackColor = Color.Red; }
            if (rating >= 4 && rating <= 5) { tbRated.BackColor = Color.LightSalmon; }
            else if (rating >= 5 && rating <= 6) { tbRated.BackColor = Color.LightGreen; }
            else if (rating >= 7 && rating <= 8) { tbRated.BackColor = Color.LimeGreen; }
            else if (rating >= 9) { tbRated.BackColor = Color.Green; }

            tbRuntime.Text = item.Runtime;
            tbGenre.Text = item.Genre;
            tbActors.Text = item.Actors;
            tbPlot.Text = item.Plot;
            tbMetascore.Text = item.Metascore;

            try
            {
                string path = _liteDB.FindFile(_item.ImdbId).FullName;
                toolTipDetail.SetToolTip(linkLabelOpenFilepath, path);
                FileInfo info = new FileInfo(path);
                _itemPath = info;
                if (!info.Exists) { labelFileExists.ForeColor = Color.Salmon; toolTipDetail.SetToolTip(labelFileExists, info.Exists.ToString()); }
                if (info.Directory.Exists) { labelFileExists.ForeColor = Color.Green; }
                try
                {
                    //The size of the current file in bytes
                    var mb = info.Length / 1048576;
                    if (mb <= 710)
                    { labelQuality.BackColor = Color.Red; labelQuality.Text = "LOW Quality"; }
                    if (mb <= 1000)
                    { labelQuality.BackColor = Color.Salmon; labelQuality.Text = "Low Quality"; }
                    if (mb >= 1800)
                    { labelQuality.BackColor = Color.LightCyan; labelQuality.Text = "HIGH Quality"; }

                    toolTipDetail.SetToolTip(labelQuality, $"{labelQuality.Text} - {ByteUtilities.GetByteSize(info.Length)}");

                }
                catch (Exception)
                {
                }

            }
            catch (Exception ex) { labelFileExists.ForeColor = Color.Salmon; }
            pbPoster.ImageLocation = item.Poster;

            try
            {
                _OMDB = new OMDBController(_liteDB.GetUserSettings().OMDBkey, _liteDB);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            try
            {
                UserSettings settings = _liteDB.GetUserSettings();
                _TMDB = new TMDBController(_liteDB, $"{settings.TMDBkey}");
            }
            catch (Exception ex)
            { }
        }


        private void MovieDetailsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
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

            Item obj = new Item()
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
                // Picture = arr
            };

            //_LITEDB.AddToWishList(obj);
        }

        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (sender.Equals(linkTrailer))
                {
                    Uri link = new Uri($"https://www.imdb.com/title/{_item.ImdbId}/");
                    // System.Diagnostics.Process.Start(link);
                    FileUtilities.Start(link);
                }
                else if (sender.Equals(linkLabelOpenFilepath))
                {
                    string path = _liteDB.FindFile(_item.ImdbId).FullName;

                    if (File.Exists(path))
                    {
                        FileUtilities.OpenFolderHighlightFile(new FileInfo(path));
                    }
                    else if (Directory.Exists(path))
                    {// System.Diagnostics.Process.Start(path);
                        FileUtilities.Start(new DirectoryInfo(path));
                    }

                }
            }
            catch (Exception ex)
            { }
        }
        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            var deletd = _liteDB.DeleteItem(_item.ImdbId);
            if (deletd != 1)
                MessageBox.Show($"Deletion of item {_item.ImdbId}  failed");
        }

        private void pbPoster_MouseHover(object sender, EventArgs e)
        {
            try
            {
                ToolTip tt = new ToolTip();
                tt.SetToolTip(this.pbPoster, tbPlot.Text);
            }
            catch (Exception ex)
            { }
        }

        private void pbPoster_Click(object sender, EventArgs e)
        {
            try
            {
                Image img = pbPoster.Image;

                PictureBox box = new PictureBox() { Image = img };
                Form form = new Form();
                form.Size = new Size(this.Width, this.Height);
                box.Dock = DockStyle.Fill;
                box.SizeMode = PictureBoxSizeMode.Zoom;
                form.Controls.Add(box);
                form.BringToFront();
                // Screen screen = Screen.FromControl(this);   
                //Rectangle bounds = screen.Bounds;
                //form.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);                                            

                form.Show();

            }
            catch (Exception)
            {

            }

        }

        private void tbPlot_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Form form = new Form();
                RichTextBox plot = new RichTextBox();
                plot.Text = tbPlot.Text;

                plot.Font = new Font("Microsoft San Serif", 16);
                plot.Dock = DockStyle.Fill;
                form.Text = tbTitle.Text.Replace(".", "." + Environment.NewLine);
                form.Controls.Add(plot);
                form.Size = this.Size;
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var t = Task.Run(() => _TMDB.GetMovieSimilar(_item.Title, _item.Year.ToInt32()));
                var liste = t.Result.ToList();
            }
            catch (Exception ex) { }

        }

      
        private void tbActors_TextChanged(object sender, EventArgs e)
        {
        }
    }
}