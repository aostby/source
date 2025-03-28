using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Images.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Data;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class DetailsFormItem : Form
    {
        private BindingSource _bsMovies;
        internal Item _item;
        internal FileItem _itemPath;
        internal LiteDBController _liteDB;
        internal OMDBController _OMDB;
        internal TMDBController _TMDB;
        internal SubDLSubtitleController _subDL;
        internal ImageCacheDB _imageCache;

        [Obsolete("Designer only", true)] public DetailsFormItem() { InitializeComponent(); }
        //public DetailsFormItem(OMDbApiNet.Model.Item item, LiteDBController contr)
        //{ 
        //    InitializeComponent();
        //    _liteDB = contr;
        //    _item = item;
        //    this.FormBorderStyle = FormBorderStyle.None;
        //    Init(_item);
        //}
        public DetailsFormItem(OMDbApiNet.Model.Item item, LiteDBController contr
            , OMDBController omdb=null
            , TMDBController tmdb = null
            , SubDLSubtitleController subDL=null  
            , ImageCacheDB imagecache=null
            )
        {
            InitializeComponent();
            _liteDB = contr;
            _item = item;
            this.FormBorderStyle = FormBorderStyle.None;
            _OMDB = omdb;
            _TMDB=tmdb;
            _subDL = subDL; 
            _imageCache = imagecache;  
            Init(_item);
        }

        private void Init(Item item)
        {
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
  
            pbPoster.ImageLocation = item.Poster;
            try
            {
                UserSettings settings = _liteDB.GetUserSettings();

                if (_OMDB == null) { try { _OMDB = new OMDBController(settings.OMDBkey, _liteDB); } catch (Exception ex) { throw new Exception("OMDB cannot be null. make sure you have the correct API key", ex); } }
                if (_TMDB == null) { try { _TMDB = new TMDBController(_liteDB, $"{settings.TMDBkey}"); } catch (Exception ex) { } }
                if (_subDL == null) { try { _subDL = new SubDLSubtitleController(settings); } catch (Exception) { } }
                if (_imageCache == null) { try { _imageCache = new ImageCacheDB(_liteDB); } catch (Exception ex) { } };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

            //Dersom alt er initialisert, sett farger
            InitButtons();
        }

        private async void InitButtons(bool verbose = false)
        {
            try
            {
                string path;
                var t = await _liteDB.FindFile(_item.ImdbId);
                path = t.FullName;

                toolTipDetail.SetToolTip(linkLabelOpenFilepath, path);
                FileInfo info = new FileInfo(path);
                _itemPath = new FileItem(_item.ImdbId, info.FullName);
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
            buttonSearch.Image = Icons.GetFolderIcon().ToBitmap();
            buttonSubtitleSearch.Image = Icons.GetFolderIcon().ToBitmap();
            if (_imageCache != null)
            {
                try
                {
                    if (_imageCache.FindImageAsync(nameof(buttonSearch)) == null) {await _imageCache.InsertImageAsync(nameof(buttonSearch), (Bitmap)ImageUtilities.GetImageFromUrl("https://github.com/JuzerShakir/Investigate_TMDb_Movies/raw/master/logo.jpg")); }
                    if (_imageCache.FindImageAsync(nameof(buttonSubtitleSearch)) == null) { await _imageCache.InsertImageAsync(nameof(buttonSubtitleSearch), (Bitmap)ImageUtilities.GetImageFromUrl("https://subdl.com/logo/fav.png")); }

                    ImagePoster image = await _imageCache.FindImageAsync(nameof(buttonSearch));
                    buttonSearch.Image = ImageUtilities.FixedSize(image.Image, 16, 16);
                    image = await _imageCache.FindImageAsync(nameof(buttonSubtitleSearch));
                    buttonSubtitleSearch.Image = ImageUtilities.FixedSize(image.Image, 16, 16);
                }
                catch (Exception) { }
            }
            try
            {
                if (string.IsNullOrEmpty(_subDL.apikey))
                {
                    if (verbose)
                    {
                        throw new NoNullAllowedException($"SubDL krever en API Key, og din er tom. Vennligst legg inn en nøkkel for å kunne søke etter undertekster");
                    }
                }

                FileInfo info = _itemPath.ItemFileInfo;

                FileInfo srtInfo = new FileInfo(Path.ChangeExtension(_itemPath.FullName, ".srt"));
                if (!srtInfo.Exists)
                {
                    bool dirExists = Directory.Exists(Path.Combine(info.Directory.FullName, "Subs"));
                    if (!dirExists)
                    {
                        if (info.Extension.Equals(".mkv", StringComparison.OrdinalIgnoreCase))
                        {
                            buttonSubtitleSearch.BackColor = Color.PapayaWhip;
                        }
                        else { buttonSubtitleSearch.BackColor = Color.LightSalmon; }
                        toolTipDetail.SetToolTip(buttonSubtitleSearch, $" Filtype: {info.Extension}. Filmen mangler srt fil eller en mappe med undertekster.");

                    }
                    else buttonSubtitleSearch.BackColor = Control.DefaultBackColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", ex.GetType().Name);
            }
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

        private async void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
                    var t = await _liteDB.FindFile(_item.ImdbId);
                    var path = t.FullName;
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

                List<Item> imdbItems = new List<Item>();
                foreach (var item in liste)
                {
                    try
                    {
                        var movie = _TMDB.GetMovie(item.Id);
                        var local = _liteDB.FindItem(movie.ImdbId);
                        if (local != null)
                        { imdbItems.Insert(0, local); }
                        else
                        { }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var ds = DataSetUtilities.AutoGenererDataSet(imdbItems);
                if (ds.Tables.Count <= 0)
                {
                    ds = DataSetUtilities.AutoGenererDataSet(liste);
                }
                var cols = Kolibri.net.SilverScreen.Controls.Constants.VisibleTMDBColumns.ToList();


                if (ds.Tables[0].Columns.Contains("ReleaseDate"))
                {
                    cols = new List<string> { "OriginalTitle", "ReleaseDate", "Title", "OriginalLanguage", "Overview", "VoteAverage", "VoteCount", "Id", "MediaType", "Popularity" };


                }

                DataTable dt = new DataView(ds.Tables[0]).ToTable(false, cols.ToArray());
                dt.TableName = FileUtilities.SafeFileName(_item.Title);
                Kolibri.net.Common.FormUtilities.Visualizers.VisualizeDataSet($"{_item.Title} - {_item.Year} - Søk etter lignende filmer - fant {dt.Rows.Count} stk, {imdbItems.Count} lokalt.", dt, this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }


        private async void tbActors_Clicked(object sender, EventArgs e)
        {
            try
            {
                var t = await _TMDB.GetMovieCredits(_item.Title, _item.Year.ToInt32());
                var theList = t.Cast.ToList();

                var ds = DataSetUtilities.AutoGenererDataSet(theList.ToList<Cast>());

                DataTable dt = new DataView(ds.Tables[0], null, "Order ASC", DataViewRowState.CurrentRows).ToTable(false);
                dt.TableName = DataSetUtilities.LegalTableName("Actors");

                Kolibri.net.Common.FormUtilities.Visualizers.VisualizeDataSet(_item.Title, dt.DataSet, this.Size);


            }
            catch (Exception ex)
            {
            }

        }

        private void buttonRediger_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            form.Size = new Size(500, 500);


            Button button = new Button();
            button.DialogResult = DialogResult.OK;
            button.Text = "Lagre";
            button.Click += Button_Click;
            button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button.Dock = DockStyle.Bottom;
            button.BringToFront();
            form.Controls.Add(button);


            PropertyGrid propertyGrid1 = new PropertyGrid();
            propertyGrid1.CommandsVisibleIfAvailable = true;
            //propertyGrid1.Location = new Point(10, 20);
            propertyGrid1.Size = new System.Drawing.Size(400, 300);
            propertyGrid1.TabIndex = 1;
            propertyGrid1.Text = "Innstillinger";
            propertyGrid1.SelectedObject = _item;
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            propertyGrid1.Size = new Size(495, 480);
            // propertyGrid1.Dock = DockStyle.Top;
            form.Controls.Add(propertyGrid1);

            var res = form.ShowDialog();


            if (res == DialogResult.OK)
            {
                _liteDB.Update(_item);
                _liteDB.Update(_itemPath);
                Init(_item);
            }
        }
        private void Button_Click(object? sender, EventArgs e)
        {
            (sender as Button).DialogResult = System.Windows.Forms.DialogResult.OK;
            ((sender as Button).Parent as Form).DialogResult = DialogResult.OK;
            ((sender as Button).Parent as Form).Close();
        }

        private void buttonSubtitleSearch_Click(object sender, EventArgs e)
        { 

            try
            {
                if (string.IsNullOrEmpty(_subDL.apikey))
                    throw new NoNullAllowedException($"SubDL krever en API Key, og din er tom. Vennligst legg inn en nøkkel for å kunne søke etter undertekster");


                FileInfo info = _itemPath.ItemFileInfo;


                FileInfo srtInfo = new FileInfo(Path.ChangeExtension(_itemPath.FullName, ".srt"));
                bool dirExists = Directory.Exists(Path.Combine(info.Directory.FullName, "Subs"));
                var mmi = _OMDB.GetItemByImdbId(_item.ImdbId);
                dirExists = dirExists && mmi.Type == "movie";
                if (info.Exists && !dirExists)
                {
                    var jall = _subDL.SearchByIMDBid(_item.ImdbId);
                    if (jall.status == true && jall.subtitles != null && jall.subtitles.Count >= 1)
                    {
                        foreach (var sub in jall.subtitles)
                        {
                            try
                            {
                                string url = $"https://dl.subdl.com{sub.url}";

                                FileInfo subInfo = new FileInfo(Path.Combine(info.Directory.FullName, "Subs", FileUtilities.SafeFileName($"{sub.language}_{sub.release_name}.zip")));

                                var exist = Kolibri.net.Common.Utilities.FileUtilities.DownloadFile(url, subInfo.FullName);

                                if (!exist) throw new FileNotFoundException(subInfo.FullName);

                            }
                            catch (Exception ex)
                            {
                                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"List contained no elements for this path.{Environment.NewLine}{ex.Message}. Try searching for elements and try again", _itemPath.FullName);
            }
        }
    }
}