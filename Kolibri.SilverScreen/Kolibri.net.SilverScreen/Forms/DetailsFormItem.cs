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
            else if (rating >= 4 && rating <= 5) { tbRated.BackColor = Color.LightSalmon; }
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
                FileInfo info = new FileInfo(path);
                _itemPath = info;
                if (!info.Exists) { labelFileExists.ForeColor = Color.Salmon; }
                if (info.Directory.Exists) { labelFileExists.ForeColor = Color.Green; }
                try
                {
                    //The size of the current file in bytes
                    var mb = info.Length / 1048576;
                    if (mb <= 700)
                    { labelQuality.BackColor = Color.Red; labelQuality.Text = "LOW Quality"; }
                    if (mb <= 1000)
                    { labelQuality.BackColor = Color.Salmon; labelQuality.Text = "Low Quality"; }
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
                var type = Enum.Parse<Kolibri.net.SilverScreen.Controls.Constants.MultimediaType>(_item.Type, true);
                SearchForItem(_item);
            }
            catch (Exception ex) { }

        }

        private void SearchForItem(Item item)
        {
            List<SearchItem> liste = new List<SearchItem>();
            OMDbApiNet.Model.Item nm = null;

            var title = $"{item.Title}";//dgv.CurrentCell.Value;  
            string year = $"{item.Year}";
            if (InputDialogs.InputBox("Filmsøk", "Søk etter filmtittel", ref title) == DialogResult.OK)
            {
                liste = _OMDB.GetByTitle(title, OMDbApiNet.OmdbType.Movie, 3);

                if (liste != null && liste.Count == 1)
                {
                    nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));
                }
                else if (_TMDB != null && !string.IsNullOrEmpty(_TMDB.ApiKey))
                {
                    var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                    List<SearchMovie> tLibList = t.Result;
                    if (tLibList != null && tLibList.Count == 1)
                    {

                        Movie tmdbMovie = _TMDB.GetMovie(tLibList[0].Id);
                        nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                        if (nm.ImdbRating == "N/A")
                        {
                            nm.ImdbRating = $"{tmdbMovie.VoteAverage}".Replace(",", ".");
                        }
                    }
                }
                else if (nm == null && liste != null)
                {
                    object ttid = string.Empty;
                    DataSet ds = DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));
                    if (InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                    {
                        DataTable table = new DataView(ds.Tables[0].Copy(), $"ImdbId='{(ttid as ListViewItem).SubItems[2].Text}'", "", DataViewRowState.CurrentRows).ToTable();
                        nm = _OMDB.GetMovieByIMDBTitle(table.Rows[0]["Title"].ToString(), Convert.ToInt32(table.Rows[0]["Year"]));
                    }
                }
            }
            if (nm == null&&_TMDB!=null)
            {

                var t = Task.Run(() => _TMDB.FetchMovie(title, Convert.ToInt32(year)));
                List<SearchMovie> tLibList = t.Result;
                if (tLibList != null && tLibList.Count() > 0)
                {
                    try
                    {
                        object tmdbId = string.Empty;
                        DataSet ds = DataSetUtilities.AutoGenererDataSet(new ArrayList(tLibList));
                        if (InputDialogs.ChooseListBox("Choose corect Movie (TMDB)", "Set the correct value",
                            new DataView(ds.Tables[0].Copy(), $"", "", DataViewRowState.CurrentRows).ToTable(true, "Id", "Title", "VoteAverage", "OriginalTitle", "ReleaseDate", "OriginalLanguage", "Overview")
                            , ref tmdbId) == DialogResult.OK)
                        {

                            Movie tmdbMovie = _TMDB.GetMovie(Convert.ToInt32((tmdbId as ListViewItem).SubItems[0].Text));
                            // DataTable table = new DataView(ds.Tables[0].Copy(), $"Id='{(ttid as ListViewItem).SubItems[0].Text}'", "", DataViewRowState.CurrentRows).ToTable(true, "Id"," Title"," VoteAverage","  OriginalTitle"," ReleaseDate"," OriginalLanguange"," Overview");
                            if (tmdbMovie != null && !string.IsNullOrEmpty(tmdbMovie.ImdbId))
                                nm = _OMDB.GetMovieByIMDBid(tmdbMovie.ImdbId);
                            liste = new List<OMDbApiNet.Model.SearchItem>();
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

                if (nm == null)
                {
                    if (InputDialogs.Generic2ValuesDialog("Not found. Put exact year", "", ref title, ref year, "Tittel", "Utgivelsesår") == DialogResult.OK)
                    {
                        liste = _OMDB.GetByTitle(title, Convert.ToInt32(year), OMDbApiNet.OmdbType.Movie, 2);
                        if (liste != null)
                        {
                            object ttid = string.Empty;
                            DataSet ds = DataSetUtilities.AutoGenererTypedDataSet(new ArrayList(liste));


                            if (InputDialogs.ChooseListBox("Choose correct Movie", "Set the correct value", ds.Tables[0], ref ttid) == DialogResult.OK)
                            {
                            }
                        }
                        if (liste == null)
                        {
                            nm = _OMDB.GetMovieByIMDBTitle(title.ToString(), Convert.ToInt32(year));
                        }

                        else
                        {
                            nm = _OMDB.GetMovieByIMDBTitle(liste[0].Title.ToString(), Convert.ToInt32(liste[0].Year));

                        }

                        liste = new List<OMDbApiNet.Model.SearchItem>();
                    }
                    if (liste == null) liste = new List<OMDbApiNet.Model.SearchItem>();
                }
            }
            if (nm == null&&_TMDB!=null)
            {
                var test = _TMDB.FetchMovie(title, Convert.ToInt32(year));

                if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "chrome.exe"; // @"""C:\Program Files (x86)\Google\Chrome\Application\chrome.exe""";
                    startInfo.Arguments = $@"https://www.imdb.com/find?q={title.Replace(" ", "+")}&ref_=nv_sr_sm";
                    Process.Start(startInfo);
                }
            }
            else if (nm != null)
            {

                if (liste == null)
                    liste = new List<OMDbApiNet.Model.SearchItem>();

                {
                    DialogResult res =
                                     MessageBox.Show($"{liste.Count} move(s) were found:\r\n\r\nTitle: {nm.Title}\r\nImdbRating: {nm.ImdbRating}\r\nYear: {nm.Year}\r\nActors: {nm.Actors}\r\nPlot :{nm.Plot}", "Is this movie correct?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        
                        Item newItem = new Item()
                        {
                            Title = nm.Title,
                            Year = (nm.Year),
                            ImdbId = nm.ImdbId,
                            ImdbRating = nm.ImdbRating,
                            Genre = nm.Genre,
                            Plot = nm.Plot,
                            Runtime = nm.Runtime,
                            Rated = nm.Rated,
                            
                            TomatoUrl = _itemPath.FullName
                        };
                        _liteDB.DeleteItem(_item.ImdbId);
                        _liteDB.Upsert(nm);
                        _liteDB.Upsert(newItem);
                         _liteDB.Upsert(new FileItem(nm.ImdbId,_itemPath.FullName));
                        _item = newItem;
                        //Form form = new DetailsFormItem(nm, _liteDB);
                        //form.ShowDialog();
                    }
                    else if (MessageBox.Show("Nothing found. Go to imdb.com to search for the movie online", title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://imdb.com");
                    }
                }
            }
        }
    }
}