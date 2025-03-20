using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;


namespace Kolibri.net.SilverScreen.Forms
{
    public partial class IMDbDataFilesForm : Form
    {
        private UserSettings _userSettings;
        private ToolTip _toolTip;
        private DirectoryInfo _destinationDir;
        private ImageCacheDB _imgCache;
        private LiteDBController contr;
        private List<string> localImdbIds;
        private List<string> seriesList;

        private Dictionary<string, List<string>> _lineDic = new Dictionary<string, List<string>>();
     

        Regex regexName = new Regex("\"[^\"]+\""),
          regexYear = new Regex("\\d+$"),
          regexEpisodeName = new Regex("{[^(]+"),
          regexSeason = new Regex("\\(#[^\\.]+"),
          regexEpisode = new Regex("\\.\\d+\\)}"),
          regexMovieName = new Regex("^[^(]+");


        private List<string> _gzFiles = new List<string>() {  "title.basics.tsv.gz",  "title.episode.tsv.gz", "title.crew.tsv.gz", "title.ratings.tsv.gz", "name.basics.tsv.gz", "title.akas.tsv.gz", "title.principals.tsv.gz" };

        private List<string> _files;

        public IMDbDataFilesForm(UserSettings settings)
        {
            InitializeComponent();
            _userSettings = settings; 
            contr = new LiteDBController(new FileInfo(_userSettings.LiteDBFilePath), false, false);
           
            _files = _gzFiles.Select(s => s.Replace(".gz", string.Empty)).ToList();
            Init();
        }


        private void SetStatusLabelText(string message)
        {
            try
            {
                Task.Delay(1).GetAwaiter().GetResult();
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetStatusLabelText(message); }
                    ));
                else
                {

                    toolStripLabel1.Text = message;
                }
            }
            catch (Exception ex)
            { }
        }
        private void Init()
        {
            localImdbIds = contr.FindAllFileItems().Select(t => t.ImdbId).ToList();
            seriesList   = contr.GetAllItemsByType("Series").Select(t => t.ImdbId).ToList();
            toolStripProgressBar1.Visible = false;
         
            SetStatusLabelText($"Initializing buttons {DateTime.Now.ToShortTimeString()}.");
            var list = this.Controls.OfType<FlowLayoutPanel>().ToList();
            foreach (var child in list)
            {
                child.Controls.Clear();
                child.Enabled = true;
            }

            _destinationDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(_userSettings.IMDbDataFiles)));
            if ((!_destinationDir.Exists)) _destinationDir.Create();

            linkLabelOnline.Text = _userSettings.IMDbDataFiles;
            linkLabelOnline.Tag = _userSettings.IMDbDataFiles;
            linkLabelLocal.Text = _destinationDir.FullName;
            linkLabelLocal.Tag = _destinationDir;

            _toolTip = new ToolTip();
            _imgCache = new Kolibri.net.Common.Dal.Controller.ImageCacheDB(_userSettings);


            try
            {
                foreach (string fileName in _gzFiles)
                {
                    Button button = new Button();
                    button.Text = fileName;
                    button.Name = $"button{button.Text}";
                    button.Click += buttonfile_Click;
                    SetToolTipForButton(button);
                    button.Width = flowLayoutPanelGZ.Width - 10;
                    flowLayoutPanelGZ.Controls.Add(button);

                    Button filebutton = new Button();
                    filebutton.Text = fileName.Replace(".gz", string.Empty);
                    filebutton.Name = $"button{filebutton.Text}";
                    filebutton.Click += buttondatafile_Click;
                    SetToolTipForButton(filebutton);
                    filebutton.Enabled = GetDestination(filebutton).Exists;
                    filebutton.Width = flowLayoutPanelDataFiles.Width - 10;
                    flowLayoutPanelDataFiles.Controls.Add(filebutton);

                    Button updateData = new Button();
                    updateData.Text = fileName.Replace(".tsv.gz", string.Empty);
                    updateData.Name = $"button{updateData.Text}";
                    updateData.Click += findOutOfIt;
                    SetToolTipForButton(updateData, true);
                    updateData.Enabled = _lineDic.Keys.Contains(filebutton.Text);
                    updateData.Width = flowLayoutPanelUpdate.Width - 10;
                    flowLayoutPanelUpdate.Controls.Add(updateData);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private FileInfo GetDestination(Button button) {
        var file= new FileInfo(Path.Combine(_destinationDir.FullName, button.Text));
            
            return file;
        }

        public async void findOutOfIt(object? sender, EventArgs e)
        {
            List<Item> res = new List<Item>();

            int counter = 0;
            bool ret = false;
            Button button = sender as Button;

            try
            {
                var key = _lineDic.Keys.FirstOrDefault(x => x.StartsWith(button.Text));
                var lines = _lineDic[key];

                FileInfo info = GetDestination(button);
                long length = 0;
                if (info.Exists)
                {
                    length = info.Length;
                }
                SetStatusLabelText($"{lines.Count()} files found. Enabeling buttons [{DateTime.Now.ToShortTimeString()}] {Path.GetFileNameWithoutExtension(info.Name)}: {info.Name} ({FileUtilities.GetFilesize(length)})");

                var progress = InitProgressBar();

                switch (key)
                {
                    case "title.episode.tsv":
                        if (!_lineDic.ContainsKey("title.basics.tsv"))
                        {
                            try
                            {
                                buttondatafile_Click(new Button() { Text = key, Name = key }, e);

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        var filteredBiglist = await GetEpisodes(info.Name, lines, localImdbIds, progress);
                        var episodeList = filteredBiglist.Where(a => seriesList.Where(b => b == a.SeriesId).Any());
                        foreach (var item in episodeList)
                        {

                            var localEp = contr.GetEpisode(item.ImdbId);
                            if (localEp == null) //, kan være tusen stykker totalt
                            {

                                SetStatusLabelText($"Updating Series episodes: {item.Title}  {counter} / {lines.Count()} ");
                                //     contr.Upsert(item);  TODO - hent fra TMDB

                            }
                        }
                        break;
                    case "name.basics.tsv":

                        break;
                    case "title.basics.tsv":
                        res = await GetItemsFrom(key, lines, progress);
                        break;

                    default:
                        res = await GetItemsFrom(key, lines, progress);
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            }
            
      

        private async void buttondatafile_Click(object? sender, EventArgs e)
        {
            try
            {
                Progress<int> progress = InitProgressBar();

                Button button = (sender as Button);
                button.Enabled = false;
                FileInfo info =GetDestination(button);
                toolStripProgressBar1.Visible = true;
                var result = await ReadIMDBData(  button, progress);                
                toolStripProgressBar1.Visible = false;
                button.Enabled = true;
                if (!result) { throw new Exception($"Failed to read {info.Name}"); }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            Init();
        }

 

        /// <summary>
        /// Leser data fra en stor fil, dersom den er diger, filtreres den opp imot imdbId som ligger i DB fra før.
        /// </summary>
        /// <param name="button">Knappen som har sendt kommandoen</param>
        /// <param name="progress">Fremgangsindikator</param>
        /// <returns></returns>
        private async Task<bool> ReadIMDBData(Button button, IProgress<int> progress)
        {

            bool ret = false;
            FileInfo info = GetDestination(button);

            SetStatusLabelText($"Initializing [{DateTime.Now.ToShortTimeString()}] {Path.GetFileNameWithoutExtension(info.Name)} ({FileUtilities.GetFilesize(info.Length)}): {info.Name}");
            var huge = FileUtilities.GetFilesize(info.Length).Contains("gib", StringComparison.OrdinalIgnoreCase);
            
            List<string> lines = huge? await info.ReadAllLinesAsync(localImdbIds, progress):await info.ReadAllLinesAsync(progress:progress);
            _lineDic[info.Name] = lines;
            //button.Tag = ret;
            ret = lines.Count() > 0;
            return ret;
        }
        private async Task<List<Episode>> GetEpisodes(string name, IEnumerable<string> lines, List<string> localImdbIds, IProgress<int> progress)
        {
            List<Episode> items = new List<Episode>();

            int counter = 0;
            int totalItems = lines.Count();
            int lastPercentage = 0;
            foreach (string line in lines)
            {
                counter++;
                int progressPercentage = (int)((double)counter / totalItems * 100);
                if (progressPercentage.Equals(0)) progressPercentage = 1;
                if (progressPercentage > 100) progressPercentage = 100;
         
              
                if (line.Contains("-------------") || line.Contains("tconst")||string.IsNullOrWhiteSpace(line)) { continue; }
                var arr = line.Split("\t");                
                if (localImdbIds.Find(x => x.Equals(arr[1])) == null)
                {
                    if (progressPercentage > lastPercentage)
                    {lastPercentage = progressPercentage;   
                        SetStatusLabelText($"(status: {counter}/{lines.Count()} - {progressPercentage}%)"); await Task.Delay(4);
                        progress.Report(progressPercentage);
                        await Task.Delay(10);
                    }   
                    continue;
                }
                //SetStatusLabelText($" {line}");
                OMDbApiNet.Model.Episode item = new Episode();
                item.ImdbId = arr[0];
                item.SeriesId = arr[1];
                item.SeasonNumber = arr[2];
                item.EpisodeNumber = arr[3];

                SetStatusLabelText($"Found episode for SeriesID: {item.SeriesId} (status: {counter}/{lines.Count()} - {progressPercentage}%)");
                item.Title = $"Season {item.SeasonNumber} Episode {item.EpisodeNumber}";
                items.Add(item);
            }
            return items;
        }

        private async Task<List<Item>> GetItemsFrom(string name, IEnumerable<string> lines, IProgress<int> progress)
        {
          
            int counter = 0;
            long currentLength = 0;
            long totalLength = lines.Count();
            int lastPercentage = 0;
            List<Item> items = new List<Item>();
            
            Item item = new Item();
       

            foreach (string line in lines)
            {
                counter++; currentLength += line.Length;
                if (progress != null)
                {
                    int percentage = (int)((currentLength / (double)totalLength) * 100.0);
                    if (percentage > lastPercentage)
                    {
                        lastPercentage = percentage;
                        progress.Report(lastPercentage);
                    }
                }

                if (line.Contains("-------------")||line.Contains("nconst")|| line.Contains("tconst")||string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                SetStatusLabelText($" {line}");

                var arr = line.Split("\t");
                if (localImdbIds.Find(x => x.Equals(arr.FirstOrDefault())) == null)
                {
                    item.ImdbId = arr.FirstOrDefault();
                }
                if (line.StartsWith('"'.ToString()) && regexSeason.Match(line).Value != string.Empty)
                {
                    string year = regexYear.Match(line).Value;

                    item.Title = regexName.Match(line).Value.Replace("\"", "").Replace("'", "´");
                    item.Year = (year == string.Empty ? "'" : year + ", '");
                    item.Title = regexEpisodeName.Match(line).Value.Replace("{", "").Trim().Replace("'", "´");
                    // item.Season= regexSeason.Match(line).Value.Replace("(#", "") ;
                    // item.Episode = regexEpisode.Match(line).Value.Replace(")}", "").Replace(".", "") + ", 1);");
                }
                else
                {
                    if (counter < 15) continue;
                    string year = regexYear.Match(line).Value;
                    item.Title = regexMovieName.Match(line).Value.Trim().Replace("'", "´");
                    item.Year = (year == string.Empty ? "'" : "', " + year);
                }
                items.Add(item);
            }
            return items;
        }

        private void SetToolTipForButton(Button button, bool checkLines=false)
        {
            Dictionary<string, string> tDic = new Dictionary<string, string>() {
            { "name.basics", $"{Environment.NewLine}Names of actors and other prominent persons{Environment.NewLine} nconst\tprimaryName\tbirthYear\tdeathYear\tprimaryProfession\tknownForTitles" },
{"title.basics", $"{Environment.NewLine}Comprimented information about an imdb{Environment.NewLine} tconst\ttitleType\tprimaryTitle\toriginalTitle\tisAdult\tstartYear\tendYear\truntimeMinutes\tgenres" },
{"title.akas", $"{Environment.NewLine}Mostly useless large file with AKA info{Environment.NewLine}titleId\tordering\ttitle\tregion\tlanguage\ttypes\tattributes\tisOriginalTitle"},
{"title.crew", $"{Environment.NewLine}Credits writers, and points to the name file{Environment.NewLine}tconst	directors	writers"},
{"title.episode", $"{Environment.NewLine}ShowEpisodes, points to title.basic or title.principals (all sorts of catagories){Environment.NewLine}tconst\tparentTconst\tseasonNumber\tepisodeNumber"},
{"title.principals", $"{Environment.NewLine}All sorts of pointers, to directors, producers, writers etc. LARGE file, avoid if not important{Environment.NewLine}tconst\tordering\tnconst\tcategory\tjob\tcharacters"},
{"title.ratings", $"{Environment.NewLine}IMDB scores for titles, and episodes.{Environment.NewLine}tconst\taverageRating\tnumVotes"}
        };
            try
            {
                if (!checkLines)
                {
                    FileInfo info = new FileInfo(Path.Combine(_destinationDir.FullName, button.Text));
                    var key = string.Join(".",  info.Name.Split(".").Take(2));
                    if (info.Exists)
                    {
                        button.BackColor = Color.LimeGreen;
                        _toolTip.SetToolTip(button, $"{button.Text} exist ({FileUtilities.GetFilesize(info.Length)}). Please hit button to download fresh version {tDic[key]}");
                        if (info.LastWriteTime < DateTime.Now.AddDays(-15))
                        {
                            button.BackColor = Color.LightYellow;
                        }
                        if (!info.Extension.Equals(".gz", StringComparison.OrdinalIgnoreCase) && info.Exists)
                        {
                            button.BackColor = Color.LimeGreen;
                            FileInfo dataFile = button.Tag as FileInfo;
                            _toolTip.SetToolTip(button, $"{button.Text} is a text file ({FileUtilities.GetFilesize(info.Length)}). Please hit button to update {_userSettings.LiteDBFilePath} {tDic[key]}");
                        }
                    }
                    else
                    {
                        button.BackColor = Color.LightSalmon;
                        _toolTip.SetToolTip(button, $"{button.Text} does not exist. Please hit button to download");
                    }
                   
                }
                else {
                    if (_lineDic.Keys.Count>0&&_lineDic.Keys.First(x => x.Contains(button.Text)) != null)
                    { button.BackColor = Color.LimeGreen; }
                    else { button.BackColor = _lineDic.Keys.Count >= 0 ? Color.LightSalmon:   Color.LightYellow; }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void buttonfile_Click(object sender, EventArgs e)
        {
            flowLayoutPanelGZ.Enabled = false;
            try
            {
                string filename = GetDestination(sender as Button).Name;
                FileInfo destination = new FileInfo(Path.Combine(_destinationDir.FullName, filename));
                Uri baseUri = new Uri(linkLabelOnline.Tag.ToString());
                Uri url = new Uri(baseUri, filename);
                if (Kolibri.net.Common.Utilities.FileUtilities.DownloadFile(url, destination))
                {
                    FileUtilities.Decompress(destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            Init();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = (sender as LinkLabel).Text;
                FileUtilities.Start(new Uri(url));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private Progress<int> InitProgressBar()
        {
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = 100;
            Progress<int> progress = new Progress<int>(value => { toolStripProgressBar1.Value = value; });
            return progress;
        }
    }
}