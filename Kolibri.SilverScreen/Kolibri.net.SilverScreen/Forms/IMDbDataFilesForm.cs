using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using TMDbLib.Objects.General;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class IMDbDataFilesForm : Form
    {
        bool _cancel=false;

        private UserSettings _userSettings;
        private ToolTip _toolTip;
        private DirectoryInfo _destinationDir;
        private LiteDBController _contr;

        private Dictionary<string, List<string>> _lineDic = new Dictionary<string, List<string>>();
        // private Dictionary<string, object> _lineDic = new Dictionary<string, object>();


        Regex regexName = new Regex("\"[^\"]+\""),
          regexYear = new Regex("\\d+$"),
          regexEpisodeName = new Regex("{[^(]+"),
          regexSeason = new Regex("\\(#[^\\.]+"),
          regexEpisode = new Regex("\\.\\d+\\)}"),
          regexMovieName = new Regex("^[^(]+");


        private List<string> _gzFiles = new List<string>() { "title.basics.tsv.gz", "title.episode.tsv.gz", "title.crew.tsv.gz", "title.ratings.tsv.gz", "name.basics.tsv.gz", "title.akas.tsv.gz", "title.principals.tsv.gz" };

        private List<string> _files;

        public IMDbDataFilesForm(UserSettings settings)
        {
            InitializeComponent();
            _userSettings = settings;
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

            toolStripProgressBar1.Visible = false;

            SetStatusLabelText($"[INITIALIZING] All buttons {DateTime.Now.ToShortTimeString()}.");
            var list = this.Controls.OfType<GroupBox>().ToList();
            foreach (var item in list)
            {
                foreach (var child in item.Controls.OfType<FlowLayoutPanel>().ToList())
                {
                    child.Controls.Clear();
                    child.Enabled = true;
                }
            }
     

            _destinationDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(_userSettings.IMDbDataFiles)));
            if ((!_destinationDir.Exists)) _destinationDir.Create();
            _contr = new LiteDBController(new FileInfo(Path.Combine(_destinationDir.FullName, "ImdbDataFiles.db")), false, false);

            linkLabelOnline.Text = _userSettings.IMDbDataFiles;
            linkLabelOnline.Tag = _userSettings.IMDbDataFiles;
            linkLabelLocal.Text = _destinationDir.FullName;
            linkLabelLocal.Tag = _destinationDir;
            linkLabelIMDBdb.Text = _contr.ConnectionString.Filename;
            linkLabelIMDBdb.Tag = _contr.ConnectionString.Filename;

            _toolTip = new ToolTip();



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
        private FileInfo GetDestination(Button button)
        {
            var file = new FileInfo(Path.Combine(_destinationDir.FullName, button.Text));

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

                var progress = InitProgressBar(toolStripProgressBar1);
                res = await GetItemsFrom(key, lines, _contr, progress);

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
                Progress<int> progress = InitProgressBar(toolStripProgressBar1);

                Button button = (sender as Button);
                button.Enabled = false;
                FileInfo info = GetDestination(button);
                toolStripProgressBar1.Visible = true;
                var result = await ReadIMDBData(button, progress);
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
            var localImdbIds = _contr.FindAllItems().GetAwaiter().GetResult().Select(s => s.ImdbId).ToList();
            List<string> lines = huge ? await info.ReadAllLinesAsync(localImdbIds, progress) : await info.ReadAllLinesAsync(null, progress: progress);
            _lineDic[info.Name] = lines;
            //button.Tag = ret;
            ret = lines.Count() > 0;
            return ret;
        }
       /* private async Task<List<Episode>> GetEpisodes(string name, IEnumerable<string> lines, IProgress<int> progress)
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
                if (progressPercentage > lastPercentage)
                {
                    lastPercentage = progressPercentage;
                    SetStatusLabelText($"(status: {counter}/{lines.Count()} - {progressPercentage}%)"); await Task.Delay(4);
                    progress.Report(progressPercentage);
                    await Task.Delay(10);
                }

                if (line.Contains("-------------") || line.Contains("tconst") || string.IsNullOrWhiteSpace(line)) { continue; }
                var arr = line.Split("\t");

            
            }
            return items;
        }
        */
        private async Task<List<Item>> GetItemsFrom(string name, List<string> orglines, LiteDBController contr,   IProgress<int> progress)
        {  
            List<string> lines = orglines.ToList();
            int counter = 0;
            long totalLength = lines.Count();
            int lastPercentage = 0;
            List<Item> items = new List<Item>(); 
          
            List<string> localImdbIds = new List<string>();

            SetStatusLabelText($"[INITIALIZING] {name} {DateTime.Now.ToShortTimeString()} - Starting with {localImdbIds.Count()} items in {_contr.ConnectionString.Filename}");
            await Task.Delay(1);


            if (name.Contains("title.basics"))
            {
                  localImdbIds = _contr.FindAllItems().GetAwaiter().GetResult().Select(s => s.ImdbId).ToList();
                if (localImdbIds.Count() > 0)
                {
                    SetStatusLabelText($"[INITIALIZING] {name} {DateTime.Now.ToShortTimeString()} - Taking the first {localImdbIds.Count()} away");
                    await Task.Delay(1);
                    //lines = orglines. GetRange(localImdbIds.Count(), orglines.Count() - localImdbIds.Count());
                    lines = orglines.Take<string>(new Range(localImdbIds.Count(), orglines.Count() - localImdbIds.Count())).ToList();

                    totalLength = lines.Count();
                    counter = localImdbIds.Count(); 
                }
            } 

            foreach (string line in lines)
            {   
                counter++;
                if (line.Contains("-------------") || line.Contains("nconst") || line.Contains("tconst") || string.IsNullOrWhiteSpace(line))
                { continue; }

             
                await Task.Delay(1);

                if (progress != null)
                {
                    int percentage = (int)((counter / (double)totalLength) * 100.0);
                    if (percentage == 0) percentage = 1;
                    if (percentage <= 99 && percentage > lastPercentage)
                    {
                        lastPercentage = percentage;
                        progress.Report(lastPercentage);
                    }
                }

                if (_cancel) {
                    SetStatusLabelText($"[CANCEL] Cancel button pressed at ({counter}/{totalLength}) ");
                   _cancel = false; 
                    break;
                }

                var arr = line.Split("\t");

                if (name.Contains("name.basics")) {

                    // "nconst	primaryName	birthYear	deathYear	primaryProfession	knownForTitles"
                    throw new NotImplementedException(name);

                }
                if (name.Contains("title.ratings"))
                {
                    Item item = _contr.FindItem(arr[0]);
                    if (item != null)
                    {
                        item.Rated = arr[1];
                        item.ImdbVotes = arr[2];
                        contr.Update(item);
                        SetStatusLabelText($"[UPDATE] ({counter}/{totalLength} - {lastPercentage}%) {item.ImdbId} - {item.Title}  ");
                        await Task.Delay(2);
                    }
                }


                else if (name.Contains("title.crew"))
                {
                    Item item = _contr.FindItem(arr[0]);
                    if (item != null)
                    {
                        //item.Writer = 
                    }
                }

                else if (name.Contains("episode"))
                {
                    //SetStatusLabelText($" {line}");
                    OMDbApiNet.Model.Episode ep = new Episode();
                    ep.ImdbId = arr[0];
                    ep.SeriesId = arr[1];
                    ep.SeasonNumber = arr[2];
                    ep.EpisodeNumber = arr[3];
                    ep.Title = $"Season {ep.SeasonNumber} Episode {ep.EpisodeNumber}";



                    var tt = _contr.FindItem(ep.SeriesId);
                    if (tt != null)
                    {
                        ep.Genre ??= tt.Genre;
                    }

                    tt = _contr.FindItem(ep.ImdbId);
                    if (tt != null)
                    {
                        ep.Title = tt.Title == null ? ep.Title = $"Season {ep.SeasonNumber} Episode {ep.EpisodeNumber}" : tt.Title;
                    }

                    if (_contr.Insert(ep))
                    {
                        SetStatusLabelText($"[INSERT] ({counter}/{totalLength} - {lastPercentage}%) Found episode for SeriesID: {ep.SeriesId} - {ep.Title} (status: {counter}/{lines.Count()} - {lastPercentage}%)");


                    }

                }

                else if (arr.Length.Equals(9) && (arr[1].Equals("tvEpisode") || arr[1].Equals("tvSeries") || arr[1].Equals("movie")) && arr[5].CompareTo("1920") >= 0)
                {

                    if (!localImdbIds.Contains(arr[0]))
                    {
                        Item item = new Item();
                        item.ImdbId = arr.FirstOrDefault();
                        bool isAdult = arr[4].Equals("1");
                        item.Type = arr[1];
                        item.Title = arr[2];
                        item.Year = arr[5];
                        item.Runtime = $"{arr[7].ToInt32()}";
                        item.Genre = arr[8];
                        if (_contr.Insert(item))
                        {
                            SetStatusLabelText($"[INSERT] ({counter}/{totalLength} - {lastPercentage}%) {item.ImdbId} - {item.Title}  ");
                            await Task.Delay(2);
                        }
                    }
                }
          

            }
            progress.Report(100);
            return items;
        }

        private void SetToolTipForButton(Button button, bool checkLines = false)
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
                    var key = string.Join(".", info.Name.Split(".").Take(2));
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
                else
                {
                    if (_lineDic.Keys.Count > 0 && _lineDic.Keys.First(x => x.Contains(button.Text)) != null)
                    { button.BackColor = Color.LimeGreen; }
                    else { button.BackColor = _lineDic.Keys.Count >= 0 ? Color.LightSalmon : Color.LightYellow; }
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
        private Progress<int> InitProgressBar(ToolStripProgressBar pb)
        {
            pb.Visible = true;
            pb.Minimum = 0;
            pb.Maximum = 100;
            pb.Value = 0;
            Progress<int> progress = new Progress<int>(value =>
            {
                if (value <= 100)
                    pb.Value = value;
                else pb.Value = 100;
                try
                {
                    Thread.Sleep(2);

                    using (Graphics gr = pb.ProgressBar.CreateGraphics())
                    {
                        //Switch to Antialiased drawing for better (smoother) graphic results
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.DrawString(value.ToString() + "%",
                            SystemFonts.DefaultFont,
                            Brushes.Black,
                            new PointF(pb.Width / 2 - (gr.MeasureString(value.ToString() + "%",
                                SystemFonts.DefaultFont).Width / 2.0F),
                            pb.Height / 2 - (gr.MeasureString(value.ToString() + "%",
                                SystemFonts.DefaultFont).Height / 2.0F)));
                    }
                }
                catch (Exception ex)
                { }
            });
            return progress;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _cancel = true;
        }
    }
}