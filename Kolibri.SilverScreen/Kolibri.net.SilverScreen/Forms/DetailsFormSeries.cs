using com.sun.org.apache.xpath.@internal.functions;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Images.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controls;
using OMDbApiNet;
using OMDbApiNet.Model;
using System.Data;
using System.Windows.Forms.VisualStyles;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;
using static com.sun.tools.javac.util.Name;
using Cast = TMDbLib.Objects.TvShows.Cast;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class DetailsFormSeries : Form
    {
        private BindingSource _bsMovies;
        internal SeasonEpisode _seasonEpisode;
        internal FileItem _itemPath;
        internal LiteDBController _liteDB;        
        internal SubDLSubtitleController _subDL;
        internal ImageCache _imageCache;

        private UserSettings _userSettings; 


        public KolibriTVShow _ktv { get; }

        [Obsolete("Designer only", true)] public DetailsFormSeries() { InitializeComponent(); }

        public DetailsFormSeries(KolibriTVShow kTV, UserSettings settings)
        {
            
            InitializeComponent();
            _liteDB = null;
            _ktv = kTV;
            var item = _ktv.Item;
            _userSettings = settings;
            _imageCache = new ImageCache(_userSettings);
            if (_ktv != null && _ktv.Item != null)
            {
                this.Text += $" - {kTV.Item.Title}";
            }
            Init();
            Init(item);
        }

        private void Init()
        {
            tabControlSeasons.TabPages.Clear();
            foreach (var s in _ktv.SeasonList)
            {
                TabPage tabPage = new TabPage(s.SeasonNumber.PadLeft(2, '0'));
                //SeriesUtilities.SortAndFormatSeriesTable()

                DataGrivViewControls dgvtrls = new DataGrivViewControls(Constants.MultimediaType.Series, new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false));
                var table = DataSetUtilities.AutoGenererDataSet(s.Episodes.ToList()).Tables[0];
                System.Data.DataColumn newColumn = new System.Data.DataColumn("Season", typeof(System.String));
                newColumn.DefaultValue = s.SeasonNumber;
                table.Columns.Add(newColumn);

                var contr = dgvtrls.GetMulitMediaDBDataGridViewAsForm(SeriesUtilities.SortAndFormatSeriesTable(table));
                tabPage.Controls.Add(contr.Controls[0]);
                tabControlSeasons.TabPages.Add(tabPage);
            }
        }
        private void Init(Item item)
        {if (item != null)
            {

                tbTitle.Text = item.Title;
                tbYear.Text = item.Year.Substring(0, 4);
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

                tbActors.Text = item.Actors;
                tbRated.Text = item.ImdbRating;
                tbRated.BackColor = Color.Red;

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
                    pbPoster.Image = _imageCache.FindImage(item.Poster).Image;
                }
                catch (Exception)
                {
                    pbPoster.ImageLocation = item.Poster;
                }

                if (item == _ktv.Item)
                {
                    labelTotalSeasons.Text = "Total Seasons";
                    textBoxTotalSeasons.Text = _ktv.Item.TotalSeasons.ToInt32().ToString();
                }



                //Dersom alt er initialisert, sett farger
                InitButtons();

            }

        }
 
        private void InitButtons(bool verbose = false)
        {
            //throw new NotImplementedException();
       /*     try
            {
                string path = _liteDB.FindFile(_seasonEpisode.ImdbId).FullName;
                toolTipDetail.SetToolTip(linkLabelOpenFilepath, path);
                FileInfo info = new FileInfo(path);
                _itemPath = new FileItem(_seasonEpisode.ImdbId, info.FullName);
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
                    if (_imageCache.FindImage(nameof(buttonSearch)) == null) { _imageCache.InsertImage(nameof(buttonSearch), (Bitmap)ImageUtilities.GetImageFromUrl("https://github.com/JuzerShakir/Investigate_TMDb_Movies/raw/master/logo.jpg")); }
                    if (_imageCache.FindImage(nameof(buttonSubtitleSearch)) == null) { _imageCache.InsertImage(nameof(buttonSubtitleSearch), (Bitmap)ImageUtilities.GetImageFromUrl("https://subdl.com/logo/fav.png")); }

                    ImagePoster image = _imageCache.FindImage(nameof(buttonSearch));
                    buttonSearch.Image = ImageUtilities.FixedSize(image.Image, 16, 16);
                    image = _imageCache.FindImage(nameof(buttonSubtitleSearch));
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
       */
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
                { Uri link = null;
                    if (_seasonEpisode != null) {
                    link= new Uri($"https://www.imdb.com/title/{_seasonEpisode.ImdbId}/");
                    }
                    else {
                        link = new Uri($"https://www.imdb.com/title/{_ktv.Item.ImdbId}/");
                    }
                    // System.Diagnostics.Process.Start(link);
                    FileUtilities.Start(link);
                }
                else if (sender.Equals(linkLabelOpenFilepath))
                {
                    string path = string.Empty;

                    if (_seasonEpisode != null) { path = _liteDB.FindFile(_seasonEpisode.ImdbId).FullName; }
                    else { path = _userSettings.UserFilePaths.SeriesSourcePath; }

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
            var deletd = _liteDB.DeleteItem(_seasonEpisode.ImdbId);
            if (deletd != 1)
                MessageBox.Show($"Deletion of item {_seasonEpisode.ImdbId}  failed");
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
                throw new NotImplementedException();

                //var t = Task.Run(() => _TMDB.GetMovieSimilar(_seasonEpisode.Title, _seasonEpisode.Released.Substring(0,4).ToInt32()));
                //var liste = t.Result.ToList();

                //List<Item> imdbItems = new List<Item>();
                //foreach (var item in liste)
                //{
                //    try
                //    {
                //        var movie = _TMDB.GetMovie(item.Id);
                //        var local = _liteDB.FindItem(movie.ImdbId);
                //        if (local != null)
                //        { imdbItems.Insert(0, local); }
                //        else
                //        { }
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //}
                //var ds = DataSetUtilities.AutoGenererDataSet(imdbItems);
                //if (ds.Tables.Count <= 0)
                //{
                //    ds = DataSetUtilities.AutoGenererDataSet(liste);
                //}
                //var cols = Kolibri.net.SilverScreen.Controls.Constants.VisibleTMDBColumns.ToList();


                //if (ds.Tables[0].Columns.Contains("ReleaseDate"))
                //{
                //    cols = new List<string> { "OriginalTitle", "ReleaseDate", "Title", "OriginalLanguage", "Overview", "VoteAverage", "VoteCount", "Id", "MediaType", "Popularity" };


                //}

                //DataTable dt = new DataView(ds.Tables[0]).ToTable(false, cols.ToArray());
                //dt.TableName = FileUtilities.SafeFileName(_seasonEpisode.Title);
                //Kolibri.net.Common.FormUtilities.Visualizers.VisualizeDataSet($"{_seasonEpisode.Title} - {_seasonEpisode.Released} - Søk etter lignende filmer - fant {dt.Rows.Count} stk, {imdbItems.Count} lokalt.", dt, this.Size);
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
                //var t = await _TMDB.GetMovieCredits(_seasonEpisode.Title, _seasonEpisode.Released.Substring(0,4).ToInt32());
                //var theList = t.Cast.ToList();

                //var ds = DataSetUtilities.AutoGenererDataSet(theList.ToList<Cast>());

                //DataTable dt = new DataView(ds.Tables[0], null, "Order ASC", DataViewRowState.CurrentRows).ToTable(false);
                //dt.TableName = DataSetUtilities.LegalTableName("Actors");

                //Kolibri.net.Common.FormUtilities.Visualizers.VisualizeDataSet(_seasonEpisode.Title, dt.DataSet, this.Size);


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
            propertyGrid1.SelectedObject = _seasonEpisode;
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            propertyGrid1.Size = new Size(495, 480);
            // propertyGrid1.Dock = DockStyle.Top;
            form.Controls.Add(propertyGrid1);

            var res = form.ShowDialog();


            if (res == DialogResult.OK)
            {
                _liteDB.Update(_seasonEpisode);
                _liteDB.Update(_itemPath);
                //Init(_seasonEpisode);
                throw new NotImplementedException();
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

                throw new NotImplementedException();

                FileInfo info = _itemPath.ItemFileInfo;


                //FileInfo srtInfo = new FileInfo(Path.ChangeExtension(_itemPath.FullName, ".srt"));
                //bool dirExists = Directory.Exists(Path.Combine(info.Directory.FullName, "Subs"));
                //var mmi = _OMDB.GetItemByImdbId(_seasonEpisode.ImdbId);
                //dirExists = dirExists && mmi.Type == "movie";
                //if (info.Exists && !dirExists)
                //{
                //    var jall = _subDL.SearchByIMDBid(_seasonEpisode.ImdbId);
                //    if (jall.status == true && jall.subtitles != null && jall.subtitles.Count >= 1)
                //    {
                //        foreach (var sub in jall.subtitles)
                //        {
                //            try
                //            {
                //                string url = $"https://dl.subdl.com{sub.url}";

                //                FileInfo subInfo = new FileInfo(Path.Combine(info.Directory.FullName, "Subs", FileUtilities.SafeFileName($"{sub.language}_{sub.release_name}.zip")));

                //                var exist = Kolibri.net.Common.Utilities.FileUtilities.DownloadFile(url, subInfo.FullName);

                //                if (!exist) throw new FileNotFoundException(subInfo.FullName);

                //            }
                //            catch (Exception ex)
                //            {
                //                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"List contained no elements for this path.{Environment.NewLine}{ex.Message}. Try searching for elements and try again", _itemPath.FullName);
            }
        }
    }
}