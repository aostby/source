
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controller;
using OMDbApiNet.Model;
using sun.java2d.pipe;
using System.Collections;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace Kolibri.Common.VisualizeOMDbItem
{
    public partial class ShowLocalSeriesForm : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        // static HttpClient client = new HttpClient();
        static List<Item> movies = new List<Item>();

        static IDictionary<string, string> TYPE_TRANSLATION = new Dictionary<string, string> {
            { "movie", "film" }, { "series", "serie" }, { "episode", "episode" }, { "game", "spill" } };
        //static IEnumerable<Item> _serieItems;
        private static List<Item> _serieItems;
        private UserSettings _userSettings;
        private ImageCacheDB _imgCache;

        private LiteDBController _liteDB; 

        public ShowLocalSeriesForm(UserSettings userSettings)
        {
            InitializeComponent();
            this._userSettings = userSettings;

            _liteDB = new(new FileInfo(_userSettings.LiteDBFilePath), false, false);

            var seriesList = _liteDB.GetAllItemsByType("Series");

            _serieItems = seriesList.OrderByDescending(x => x.ImdbRating).ToList();

            lvwColumnSorter = new ListViewColumnSorter();
            this.movieList.ListViewItemSorter = lvwColumnSorter;
            Init();

        }

        private async Task Init(string defaultSeach = null)
        {

            pictureBox.Image = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
            searchBtn.BackgroundImage = ImageUtilities.Base64ToImage(ImageUtilities.SearchGlassImage());
            var img = Icons.GetFolderIcon().ToBitmap();
            img.RotateFlip(RotateFlipType.Rotate270FlipX);
            buttonLookUp.BackgroundImage = img;


            if (!string.IsNullOrEmpty(defaultSeach))
            {
                searchTxt.Text = defaultSeach;

            }
            detailsViewBtn_Click(null, null);
            //resultsToGet.SelectedIndex = 1;
            //searchBtn_Click(null, null);
            _imgCache = new ImageCacheDB(_userSettings);

            this.Text = $"Series list count: {_serieItems.Count()}";


            foreach (var item in _serieItems)
            {
                await _imgCache.FindImageAsync(item.ImdbId);
            }
        }


        public static async Task<ItemSearch> searchMovies(string title, int page)
        {
            ItemSearch movies = null;
            try
            {
                if (string.IsNullOrEmpty(title))
                    movies = new ItemSearch() { Search = _serieItems.ToArray() };
                else
                {
                    var search = _serieItems.Where(x => x.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (search == null || search.Length < 1)
                        search = _serieItems.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToArray();
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

        public async Task<Image> getImage(string url)
        {
            Image img = null;
            try
            {
                var test = await _imgCache.FindImageAsync(url);
                if (test != null)
                {
                    img = test.Image;
                    SetStatusLabelText($"Using cached image for {new Uri(url).Segments.LastOrDefault().ToString()}");
                }
                if (test == null)
                {
                    WebClient wc = new WebClient();
                    byte[] bytes = wc.DownloadData(url);
                    MemoryStream ms = new MemoryStream(bytes);
                    img = Image.FromStream(ms);
                    await _imgCache.InsertImageAsync(url, (Bitmap)img);
                    SetStatusLabelText($"Image read from {url}");
                }
            }
            catch
            {
                img = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
            }
            return await Task.FromResult<Image>(img);
        }

        private void disableSearch()
        {
            searchBtn.Enabled = false;
            searchTxt.Enabled = false;
            movieList.Enabled = false;
            //resultsToGet.Enabled = false;
        }

        private void enableSearch()
        {
            searchBtn.Enabled = true;
            searchTxt.Enabled = true;
            movieList.Enabled = true;
            // resultsToGet.Enabled = true;
        }

        private async void searchBtn_Click(object sender, EventArgs e)
        {
            //int.Parse(resultsToGet.SelectedItem.ToString())
            int totalResults = 10, gotResults = 0, toGet = 500, page = 1;
            disableSearch();
            movies = new List<Item>();
            while (gotResults < Math.Min(totalResults, toGet))
            {
                ItemSearch result = await searchMovies(searchTxt.Text, page++);

                if (result == null || result.Search == null || result.totalResults < 1)
                {
                    SetNotFoundProperties();

                    enableSearch();
                    return;
                }
                gotResults += result.Search.Count();
                totalResults = result.totalResults;
                movies.AddRange(result.Search);
                //_imgCache.Save();
                SetStatusLabelText($"Image Cache saved to {_imgCache.DefaultDBPath()}");
            }
            enableSearch();
      await      refreshMovieList();
        }
        private async Task<bool> refreshMovieList()
        {
            bool ret = true;
            try
            {
                movieList.Items.Clear();
                movieImageList.Images.Clear();
                foreach (Item movie in movies)
                {
                    var jall = await _imgCache.FindImageAsync(movie.ImdbId);
                    if (jall == null)
                    {
                        var img = await getImage(movie.Poster);
                        movieImageList.Images.Add(movie.ImdbId, img);  
                    }
                    else { movieImageList.Images.Add(movie.ImdbId, (Image)jall.Image); }
                    movieList.Items.Add(movie.Title, movie.ImdbId);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Year);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(TYPE_TRANSLATION[movie.Type]);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.ImdbRating);
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.ImdbId);

                    //Legg til elementer over, den siste i listen må være poster pga display - movieList_ItemSelectionChanged
                    movieList.Items[movieList.Items.Count - 1].SubItems.Add(movie.Poster);
                }
                movieList.Focus();
                if (movieList.Items.Count > 0)
                {
                    movieList.Items[0].Selected = true;
                }
                label1.Text = "Results found: " + movieList.Items.Count;
                SetStatusLabelText(label1.Text);
            }
            catch (Exception ex)
            {
                ret = false;
                SetStatusLabelText($"Error occured: {ex.Message}");
            }
            return ret;
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
                SetStatusLabelText($"Error occured: {ex.Message}");
            }

        }

        private void SetStatusLabelText(string message)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetStatusLabelText(message); }
                    ));
                else
                {
                    statusLabel.Text = message;
                }
            }
            catch (Exception ex)
            { }
        }
        private void SetNotFoundProperties()
        {
            try
            {
                Item result = new Item() { Title = $"NotFound {searchTxt.Text}" };

                pictureBox.Image = ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());
                titleContentLabel.Text = result.Title;
                imdbContentLabel.Text = result.ImdbId;
                directorContentLabel.Text = result.Director;
                countryContentLabel.Text = result.Country;
                ratingContentLabel.Text = result.ImdbRating;
                yearContentLabel.Text = result.Year;
                totalContentLabel.Text = result.Dvd;
                typeContentLabel.Text = result.Genre;  //result.Type;
                runtimeContentLabel.Text = result.Runtime;

                plotContentLabel.Text = result.Plot;
                SetStatusLabelText($"Item not found - using defaults");
            }
            catch (Exception ex)
            { SetStatusLabelText($"Error occured: {ex.Message}"); }

        }
        private async void movieList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;
            //pictureBox.Image = getImage(e.Item.SubItems[3].Text);
            pictureBox.Image = await getImage(e.Item.SubItems[e.Item.SubItems.Count - 1].Text);
            Item result = await getMovieDetails(e.Item.ImageKey);
            if (result == null)
            {
                SetNotFoundProperties();
                return;
            }
            else
            {
                pictureBox.Tag = result.ImdbId;
            }
            titleContentLabel.Text = result.Title;
            imdbContentLabel.Text = result.ImdbId;
            directorContentLabel.Text = result.Director;
            countryContentLabel.Text = result.Country;
            ratingContentLabel.Text = result.ImdbRating;
            yearContentLabel.Text = result.Year;
            totalContentLabel.Text = result.TotalSeasons;
            typeContentLabel.Text = result.Genre;//  result.Type;
            runtimeContentLabel.Text = result.Runtime;
            plotContentLabel.Text = result.Plot;
            try
            {
                var path = await _liteDB.FindFile(result.ImdbId);

                if (path == null)
                {
                    movieList.Items[e.ItemIndex].BackColor = Color.LightSalmon;
                }
                else
                {
                    if (Path.Exists(path.FullName))
                        movieList.Items[e.ItemIndex].BackColor = Color.LimeGreen;
                    else
                        movieList.Items[e.ItemIndex].BackColor = Color.Wheat;
                }
            }
            catch (Exception ex) { }

        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                var imdbid = pictureBox.Tag.ToString();
                Uri uri = new Uri($"https://www.imdb.com/title/{imdbid}/");
                HTMLUtilities.OpenURLInBrowser(uri);

            }
            catch (Exception ex)
            {
                SetStatusLabelText($"Error occured: {ex.Message}");
            }
        }

        private void buttonLookUp_Click(object sender, EventArgs e)
        {
            try
            {
                var folder = FolderUtilities.LetOppMappe(_userSettings.UserFilePaths.SeriesSourcePath, "Let opp mappe med serie(r)");
                if (folder != null && folder.Exists)
                {
                    _userSettings.UserFilePaths.SeriesSourcePath = folder.FullName;
                    _userSettings.Save();
                    Form form = new Kolibri.net.SilverScreen.OMDBForms.OMDBSearchForSeriesForm(folder, _userSettings);
                    form.ShowDialog();

                }

                try
                {
                    using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                    {
                        var seriesList = tmp.GetAllItemsByType("Series");

                        _serieItems = seriesList.OrderByDescending(x => x.ImdbRating).ToList();
                    }
                    Init(folder.Name);
                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void ContentLabel_Click(object sender, EventArgs e)
        {

            try
            {
                if (sender.Equals(ratingContentLabel))
                {
                    string val = ratingContentLabel.Text;
                    if (InputDialogs.InputBox("Sett verdi", "Oppdater verdien", ref val) == DialogResult.OK && val.IsNumeric())
                    {
                        Item result = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult();
                        result.ImdbRating = val.Replace(',', '.');
                        using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                        {
                            tmp.Update(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Item item = null;
                using (LiteDBController tmp = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                {
                    try
                    {

                        item = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult();
                        var file = await  tmp.FindFile(item.ImdbId);
                        if (file != null)
                        {
                            if (file.ItemFileInfo.Exists)
                                FileUtilities.OpenFolderHighlightFile(file.ItemFileInfo);
                            else if (Directory.Exists(file.FullName))
                            {
                                var path = new DirectoryInfo(file.FullName).GetDirectories(item.Title, SearchOption.AllDirectories).FirstOrDefault();
                                if (path == null)
                                {
                                    FolderUtilities.OpenFolderInExplorer(file.FullName);
                                }
                                else
                                {
                                    FolderUtilities.OpenFolderInExplorer(path.Exists ? path.FullName : file.FullName);
                                }
                            }
                            else { throw new FileNotFoundException($"{item.Title} - location not found"); }
                        }
                        else
                        {
                            var files = new DirectoryInfo(_userSettings.UserFilePaths.SeriesSourcePath).GetFiles(item.Title.Replace(" ", "*"));
                            var dir = files.FirstOrDefault().DirectoryName;


                            if (dir != null) { var folder = FolderUtilities.LetOppMappe(dir, "Searching for missing folder"); }
                            else
                            {
                                throw new FileNotFoundException($"{item.Title}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, ex.GetType().Name);

                        var folder = FolderUtilities.LetOppMappe(_userSettings.UserFilePaths.SeriesSourcePath, $"{ex.Message}");
                        if (folder != null && folder.Exists && item != null)
                        {
                            item.TomatoUrl = folder.FullName;
                            tmp.Update(item);
                            tmp.Update(new FileItem(item.ImdbId, folder.FullName));
                        }

                    }
                    tmp.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
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
            this.movieList.Sort();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                using (LiteDBController _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false))
                {
                    Item item = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult(); ;

                    Form form = new Form();
                    form.Size = new Size(500, 500);
                    form.Text = $"{item.Title} ({item.Year})";

                    Button button = new Button();
                    button.DialogResult = DialogResult.OK;
                    button.Text = "Lagre";
                    button.Name = button.Text;
                    button.Click += Button_Click;
                    button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                    button.Dock = DockStyle.Bottom;
                    button.BringToFront();
                    form.Controls.Add(button);

                    button = new Button();
                    button.DialogResult = DialogResult.OK;
                    button.Text = "Slette";
                    button.Name = button.Text;
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
                    propertyGrid1.Text = $"Item ({item.Title})";
                    propertyGrid1.SelectedObject = item;
                    propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
                    propertyGrid1.Size = new Size(495, 480);
                    // propertyGrid1.Dock = DockStyle.Top;
                    form.Controls.Add(propertyGrid1);

                    var res = form.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        SetStatusLabelText($"Oppdaterer {item.Title}");
                        if (_liteDB.Upsert(item))
                        {
                            _serieItems.RemoveAll(x => x.ImdbId == item.ImdbId);
                            movies.RemoveAll(x => x.ImdbId == item.ImdbId);
                            movies.Add(item);
                            _serieItems.Add(item);
                        }
                    }
                    if (res == DialogResult.Yes)
                    {
                        SetStatusLabelText($"Sletter lokal metadata om {item.Title}. Endringer blir synlige når vinduet åpnes på nytt");
                        _liteDB.Delete(item);
                        try
                        {
                            _serieItems.RemoveAll(x => x.ImdbId == item.ImdbId);
                            movies.RemoveAll(x => x.ImdbId == item.ImdbId);
                            searchTxt.Text = string.Empty;
                            SetNotFoundProperties();
                            refreshMovieList();
                        }
                        catch (Exception)
                        { }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void Button_Click(object? sender, EventArgs e)
        {
            if ((sender as Button).Name.Equals("Lagre"))
            {
                (sender as Button).DialogResult = System.Windows.Forms.DialogResult.OK;
                ((sender as Button).Parent as Form).DialogResult = DialogResult.OK;
                ((sender as Button).Parent as Form).Close();
            }
            else if ((sender as Button).Name.Equals("Slette"))
            {
                (sender as Button).DialogResult = System.Windows.Forms.DialogResult.Yes;
                ((sender as Button).Parent as Form).DialogResult = DialogResult.Yes;
                ((sender as Button).Parent as Form).Close();
            }
        }

        private async  void movieList_DoubleClick(object sender, EventArgs e)
        {
            await Task.Delay(4);
            Item item = null;
            try
            {
                item = await getMovieDetails(pictureBox.Tag.ToString());
                SetStatusLabelText($"{item.Title} - searching for {item.TotalSeasons} seasons.");
                MultiMediaSearchController mmc = new MultiMediaSearchController(_userSettings, _liteDB, updateTriState: false);
                var show = mmc.GetShowById(item.ImdbId);
                Form form = new Kolibri.net.SilverScreen.Forms.DetailsFormSeries(show, _liteDB, _imgCache);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                SetStatusLabelText(ex.Message);
                if (item != null)
                {
                    SetStatusLabelText($"{item.Title} - {ex.Message}");
                    item.Error = ex.Message;
                    OutputDialogs.ShowRichTextBoxDialog($"{item.GetType().Name} - Total seasons set to: {item.TotalSeasons} - {ex.Message} - {item.Title}", item.JsonSerializeObject(), this.Size);

                    var link = new Uri($"https://www.imdb.com/title/{item.ImdbId}/");
                    FileUtilities.Start(link);
                }
            }
        }

        private void contextMenu_Click(object sender, EventArgs e)
        {
            try
            {
                Item item = getMovieDetails(pictureBox.Tag.ToString()).GetAwaiter().GetResult();
                if (item == null) { throw new Exception("Fant ikkje na der serien"); }

                if (sender.Equals(toolStripMenuItemDelete))
                {
                    if (_liteDB.Delete(item))
                    {
                        SetStatusLabelText($"{item.Title} deleted. Please refresh");
                        _serieItems.Remove(item);
                    }
                }
                else if (sender.Equals(toolStripMenuItemSetIMDBTagOnFolder))
                {
                    if (Path.Exists(item.TomatoUrl) && !item.TomatoUrl.Contains(item.ImdbId))
                    {
                        var res = MessageBox.Show($"{item.Title}", "Endre navnet på filsti?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.Yes)
                        {
                            try
                            {
                                var destination = item.TomatoUrl + $" {{tmdb-{item.ImdbId}}}";
                                Directory.Move(item.TomatoUrl, destination);
                                item.TomatoUrl = destination;
                                _liteDB.Upsert(item);
                                _liteDB.Upsert(new FileItem(item.ImdbId, item.TomatoUrl));
                            }
                            catch (Exception ex)
                            {

                                SetStatusLabelText($"Error: {ex.Message}");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!Path.Exists(item.TomatoUrl))
                        {
                            MessageBox.Show($"{item.TomatoUrl} ({item.Title})", "Sorry, the filepath does not exist");
                            var res = MessageBox.Show($"Wanna look up this folder?", "Wanna fix it?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (res == DialogResult.Yes)
                            {
                                var resultSearch = FolderUtilities.LetOppMappe(item.TomatoUrl, $"Finn hovedmappe for {item.Title}");
                                if (resultSearch != null)
                                {
                                    item.TomatoUrl = resultSearch.FullName;
                                    contextMenu_Click(toolStripMenuItemSetIMDBTagOnFolder, null);

                                }
                            }


                        }
                        else
                        {

                            SetStatusLabelText($"{item.TomatoUrl} is allready set to a tt id.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatusLabelText($"Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface. Used to sort columns by row value
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor. Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {

            try
            {


                int compareResult;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

                // Calculate correct return value based on object comparison
                if (OrderOfSort == SortOrder.Ascending)
                {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                }
                else
                {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}