﻿using DapperGenericRepository.Controller;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.DapperGenericRepository.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.DapperImdbData.Service;
using MySql.Data.MySqlClient;
using OMDbApiNet.Model;
using Sylvan.Data.Csv;
using System.Data;
using System.Diagnostics;
using percentCalc = Kolibri.net.Common.FormUtilities.Tools.ProgressBarHelper;


namespace Kolibri.net.SilverScreen.Forms
{
    public partial class IMDbDataFilesForm : Form
    {
        bool _cancel = false;

        private UserSettings _userSettings;
        private ToolTip _toolTip;
        private DirectoryInfo _destinationDir;
        private LiteDBController _contr;
        private LiteDBController _liteDB;

        private Dictionary<string, List<string>> _lineDic = new Dictionary<string, List<string>>();
        // private Dictionary<string, object> _lineDic = new Dictionary<string, object>();
        private List<string> _localImdbIds;

        private List<string> _gzFiles = new List<string>() { "title.basics.tsv.gz", "title.episode.tsv.gz", "title.crew.tsv.gz", "title.ratings.tsv.gz", "name.basics.tsv.gz", "title.akas.tsv.gz", "title.principals.tsv.gz" };

        private List<string> _files;

        public IMDbDataFilesForm(UserSettings settings)
        {
            InitializeComponent();
            _userSettings = settings;
            _files = _gzFiles.Select(s => s.Replace(".gz", string.Empty)).ToList();
            Init();
        }


        private void Init()
        {
            _toolTip = new ToolTip();
            if (_liteDB == null) _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);

            toolStripProgressBar1.Visible = false;
            _localImdbIds = _liteDB.FindAllItems().GetAwaiter().GetResult().Select(s => s.ImdbId).ToList();
            SetStatusLabelText($"[INIT] All buttons {DateTime.Now.ToShortTimeString()}. Total of {_localImdbIds.Count} items found in local repository.");
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

            _toolTip.SetToolTip(
            buttonTestConnection, _userSettings.DefaultConnection);

            try
            {
                foreach (string fileName in _gzFiles)
                {
                    Button button = new Button();
                    button.Text = fileName;
                    button.Name = $"button{button.Text}";
                    button.Click += buttonDownloadZippedFile_Click;
                    SetToolTipForButton(button);
                    button.Width = flowLayoutPanelGZ.Width - 10;
                    flowLayoutPanelGZ.Controls.Add(button);

                    Button filebutton = new Button();
                    filebutton.Text = fileName.Replace(".gz", string.Empty);
                    filebutton.Name = $"button{filebutton.Text}";
                    filebutton.Click += button_BigUnpackedDataFile_Click;
                    SetToolTipForButton(filebutton);
                    filebutton.Enabled = GetDestination(filebutton).Exists;
                    filebutton.Width = flowLayoutPanelDataFiles.Width - 10;
                    flowLayoutPanelDataFiles.Controls.Add(filebutton);

                    Button updateData = new Button();
                    updateData.Text = fileName.Replace(".tsv.gz", string.Empty);
                    updateData.Name = $"button{updateData.Text}";
                    updateData.Click += button_UpdateLinesOfData_Click;
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
            TestConnection(true);
        }

        #region local helper methods
        private FileInfo GetDestination(Button button)
        {
            var file = new FileInfo(Path.Combine(_destinationDir.FullName, button.Text));

            return file;
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
                    richTextBox1.AppendText($"{Environment.NewLine}[{DateTime.Now.ToString()}] {message}");
                }
            }
            catch (Exception ex)
            { }
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
        #endregion

        public async void button_UpdateLinesOfData_Click(object? sender, EventArgs e)
        {
            List<Item> res = new List<Item>();

            int counter = 0;
            bool ret = false;
            Button button = sender as Button;
            button.Enabled = false;
            groupBoxDataFiles.Enabled = button.Enabled;

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
                var progress = percentCalc.InitProgressBar(toolStripProgressBar1);
                if (checkBoxSilent.Checked) { progress = null; }
                if (lines.Count() >= 1)
                {
                    res = await GetItemsFrom(key, lines, _contr, _localImdbIds, progress);
                }
                else { throw new NotImplementedException(key); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            button.Enabled = true;
            groupBoxDataFiles.Enabled = button.Enabled;
        }
        private async void button_BigUnpackedDataFile_Click(object? sender, EventArgs e)
        {
            groupBoxUpdate.Enabled = false;
            try
            {
                Progress<int> progress = percentCalc.InitProgressBar(toolStripProgressBar1);

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
            groupBoxUpdate.Enabled = true;
        }
        private async void buttonDownloadZippedFile_Click(object sender, EventArgs e)
        {
            flowLayoutPanelGZ.Enabled = false;
            try
            {
                string filename = GetDestination(sender as Button).Name;
                FileInfo destination = new FileInfo(Path.Combine(_destinationDir.FullName, filename));
                Uri baseUri = new Uri(linkLabelOnline.Tag.ToString());
                Uri url = new Uri(baseUri, filename);
                SetStatusLabelText($"[DOWNLOAD] Attempting to download {url} to {destination.FullName}");

                if (Kolibri.net.Common.Utilities.FileUtilities.DownloadFile(url, destination))
                {
                    SetStatusLabelText($"[DECOMPRESSING] {destination.FullName} ( {FileUtilities.GetByteSize(destination)})");
                    FileUtilities.Decompress(destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            Init();
        }

        #region processing routines

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

            SetStatusLabelText($"[INITIALIZING] [{DateTime.Now.ToShortTimeString()}] {button.Parent.Text} {Path.GetFileNameWithoutExtension(info.Name)} ({FileUtilities.GetFilesize(info.Length)}): {info.Name}");
            var huge = FileUtilities.GetFilesize(info.Length).Contains("gib", StringComparison.OrdinalIgnoreCase);

            var sb = await info.ReadAllLinesAsyncStringBuilder(progress);
            if (!huge)
            {
                _lineDic[info.Name] = new List<string>();
                await GetItemsFrom(Path.GetFileNameWithoutExtension(info.FullName), sb.ToString().Split(Environment.NewLine).ToList(), _contr, _localImdbIds, progress);
            }
            else
            {
                await GetItemsFrom(Path.GetFileNameWithoutExtension(info.FullName), sb.ToString().Split(Environment.NewLine).ToList(), _contr, _localImdbIds, progress);
            }
            //button.Tag = ret;
            ret = sb.Length > 10;
            return ret;
        }

        private async Task<List<Item>> GetItemsFrom(string name, List<string> orglines, LiteDBController contr, List<string> localImdbIds = null, IProgress<int> progress = null)
        {
            List<Item> ret = new List<Item>();
            int lastPercentage = 0;
            progress.Report(lastPercentage);
            _cancel = false;

            int counter = 0;
            List<string> lines = orglines;
            long totalLength = lines.Count();

            if (!TestConnection())
            {
                SetStatusLabelText("[DBFAULT] Ingen fungerende databasekobling funnet");
                return ret;
            }
            //Sett opp db servicer
            //DapperBulkInsertController episodeController = new DapperBulkInsertController(_userSettings.DefaultConnection);
            //episodeController.omdbEntityList.Add(ep);
            EpisodeService episodeservice = new EpisodeService(_userSettings.DefaultConnection);
            ItemService itemservice = new ItemService(_userSettings.DefaultConnection);
            SeasonService seasonservice = new SeasonService(_userSettings.DefaultConnection);

            //Siden ImdbId er en string, må en ha en _Id (auto_increment) kollonne for å klare å inserte. Derfor må connectionstring utvides:
            SeasonEpisodeService seasonepisodeservice = new SeasonEpisodeService(_userSettings.DefaultConnection);

            //List<Episode> episodes = new List<Episode>();
            SetStatusLabelText($"[INITIALIZING] {name} {DateTime.Now.ToShortTimeString()} - Starting with {name}  - {totalLength} items ");

            foreach (string line in lines)
            {
                await Task.Delay(2);
                counter++;
                if (counter == 1 || string.IsNullOrWhiteSpace(line))
                { continue; }
                if (_cancel)
                {
                    SetStatusLabelText($"[CANCEL] Cancel button pressed at ({counter}/{totalLength}) for {name}  ");
                    _cancel = false;
                    break;
                }

                var arr = line.Split("\t");

                if (arr[2].IsNumeric() && name.Contains("title.episode"))
                {
                    //SetStatusLabelText($" {line}");
                    OMDbApiNet.Model.Episode ep = new Episode();
                    ep.ImdbId = arr[0];
                    ep.SeriesId = arr[1];
                    ep.SeasonNumber = arr[2];
                    ep.EpisodeNumber = arr[3];
                    ep.Title = $"Season {ep.SeasonNumber} Episode {ep.EpisodeNumber}";
                    if (ep.SeasonNumber.Contains("N")) continue;
                    episodeservice.Add(ep); //tar for lang tid;

                    await Task.Delay(1);
                }
                else if (name.Contains("name.basics"))
                {

                    // "nconst	primaryName	birthYear	deathYear	primaryProfession	knownForTitles"
                    throw new NotImplementedException(name);

                }
                else if (name.Contains("title.ratings"))
                {
                    Item item = _contr.FindItem(arr[0]);
                    if (item != null)
                    {
                        item.Rated = arr[1];
                        item.ImdbVotes = arr[2];
                        contr.Update(item);
                        if (progress != null)
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
                else if (name.Contains("title.basics"))
                {

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
                            if (progress != null)
                            {
                                SetStatusLabelText($"[INSERT] ({counter}/{totalLength} - {lastPercentage}%) {item.ImdbId} - {item.Title}  ");

                            }
                        }
                    }
                }
                #region oppdater
                if (progress != null)
                {

                    int percentage = percentCalc.CalculatePercent(counter, totalLength);

                    if (percentage > lastPercentage)
                    {
                        lastPercentage = percentage;
                        progress.Report(lastPercentage);

                        if (counter != 0)
                        {//https://stackoverflow.com/questions/473355/calculate-time-remaining
                            float elapsedMin = ((float)120/*ca tid i ms, kan beregnes bedre*/ / 1000) / 60;
                            float minLeft = (elapsedMin / counter) * (totalLength - counter); //see comment a
                                                                                              //         minLeft = (elapsedMin  ) * (totalLength - counter); //see comment a
                            TimeSpan ts = TimeSpan.FromMinutes(minLeft);

                            string formatted = ts.ToString("%h'h'");
                            formatted = string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes);

                            SetStatusLabelText($"[STATUS] {percentage}% ferdig. ETA for operationen {name}: {formatted}  ");
                        }
                        await Task.Delay(12);

                    }
                }
                #endregion

            }
            if (progress != null) progress.Report(100);
            return ret;
        }

        #region Paralell handling
        public async void ReadAndInsert(string filePath, IProgress<int> progress)
        {
            // Tellere
            long totalLines = File.ReadLines(filePath).LongCount(); // Get total lines
            long processedLines = 0;
            int lastPercent = -1;
            //      ItemService itemservice = new ItemService(_userSettings.DefaultConnection);
            EpisodeService episodeservice = new EpisodeService(_userSettings.DefaultConnection);
            //    SeasonService seasonservice = new SeasonService(_userSettings.DefaultConnection);
            //    SeasonEpisodeService seasonepisodeservice = new SeasonEpisodeService(_userSettings.DefaultConnection);


            var options = new CsvDataReaderOptions
            {
                Delimiter = '\t', // Use tab delimiter
                HasHeaders = true // Skip header automatically
            };

            using var reader = CsvDataReader.Create(filePath, options);

            // Process in parallel
            Parallel.ForEach(reader.Cast<IDataRecord>(), record =>
            {
                if (_cancel)
                {
                    SetStatusLabelText($"[CANCEL] Cancel button pressed at ({processedLines}/{totalLines}) for {Path.GetFileNameWithoutExtension(filePath)}  ");
                    _cancel = false;
                    return;
                }


                if (filePath.Contains("title.episode") && !record.GetString(2).Contains("N"))
                {
                    try
                    {
                        var ep = new Episode
                        {
                            ImdbId = record.GetString(0),
                            SeriesId = record.GetString(1),
                            SeasonNumber = record.GetString(2),
                            EpisodeNumber = record.GetString(3),
                            Title = $"Season {record.GetString(2)} Episode {record.GetString(3)}",
                        };
                        episodeservice.Add(ep);
                    }
                    catch (Exception ex)
                    {
                        SetStatusLabelText($"[ERROR] {Path.GetFileNameWithoutExtension(filePath)} -  {ex.Message} ({processedLines})");
                    }
                }

                else if (filePath.Contains("title.episode"))
                {
                    var episode = new Episode
                    {
                        //     Id = record.GetString(0),
                        Title = record.GetString(1),
                        //   Season = record.IsDBNull(2) ? (int?)null : record.GetInt32(2),
                        EpisodeNumber = record.GetString(3) //record.IsDBNull(3) ? (int?)null : record.GetInt32(3)
                    };

                    // Process each episode
                    Console.WriteLine($"Read Episode: {episode.Title}");
                    throw new NotImplementedException(Path.GetFileNameWithoutExtension(filePath));
                }
                Interlocked.Increment(ref processedLines);

                // Update progress

                int percentComplete = (int)((processedLines * 100) / totalLines);
                if (percentComplete > lastPercent)
                {
                    ((IProgress<int>)progress).Report(percentComplete); lastPercent = percentComplete;
                    SetStatusLabelText($"[STATUS] {Path.GetFileNameWithoutExtension(filePath)} -  {percentComplete}% processed.");
                }

            });

        }


        public async void button_DataFile_Click_new(object? sender, EventArgs e)
        {
            Button button = sender as Button;
            Progress<int> progress = percentCalc.InitProgressBar(toolStripProgressBar1);

            button.Enabled = false;
            groupBoxDataFiles.Enabled = button.Enabled;
            FileInfo info = GetDestination(button);
            await Task.Delay(4);
            ReadAndInsert(info.FullName, progress);

        }
        #endregion

        #endregion

        #region local button functions
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = (sender as LinkLabel).Text;
                if (sender.Equals(linkLabelIMDBdb)) { FileUtilities.OpenFolderHighlightFile(new FileInfo(url)); }
                else { FileUtilities.Start(new Uri(url)); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _cancel = true;
        }

        private async void buttonClearDB_Click(object sender, EventArgs e)
        {
            try
            {
                var info = new FileInfo(_contr.ConnectionString.Filename);
                var res = MessageBox.Show($"Are you sure you want to clear out {info.Name} ( {FileUtilities.GetFilesize(info.Length)})?\r\nThis action will delete all information stored in the TEMP Database, not the data from your repository.", "Clear out data from TEMP DB", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res == DialogResult.OK)
                {
                    if (info.Exists)
                    {
                        _contr.Dispose();
                        await Task.Delay(10);
                        info.Delete();
                        _contr = new LiteDBController(info, false, false);
                        Init();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {

            try
            {
                buttonClearDB_Click(null, null);
                FileUtilities.DeleteDirectory(_destinationDir.FullName);
                _lineDic.Clear();
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                Init();
            }
        }


        private void buttonCreateSchemas_Click(object sender, EventArgs e)
        {
            string commentLine = $"{Environment.NewLine}{"/".PadRight(60, '*')}{"/"}{Environment.NewLine}";
            try
            {
                richTextBox1.Clear();

                richTextBox1.AppendText("-- troubleshoot: https://dba.stackexchange.com/questions/8239/how-to-easily-convert-utf8-tables-to-utf8mb4-in-mysql-5-5/21684#21684" + Environment.NewLine);

                string user = @$"{commentLine}BEGIN
CREATE USER '{Environment.UserName}'@'localhost' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON *.* TO '{Environment.UserName}'@'localhost' WITH GRANT OPTION;
CREATE USER '{Environment.UserName}'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON *.* TO '{Environment.UserName}'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;
END{commentLine}";

                richTextBox1.AppendText($"{Environment.NewLine}{user}{Environment.NewLine}");

                ObjectToMySQLTableSchemaController schemacontr = new ObjectToMySQLTableSchemaController();
                var item = new Item();
                richTextBox1.AppendText(schemacontr.CreateSchema<Item>(item) + commentLine);

                var episode = new Episode();
                richTextBox1.AppendText(schemacontr.CreateSchema<Episode>(episode) + commentLine);

                OMDbApiNet.Model.Season season = new Season();
                richTextBox1.AppendText(schemacontr.CreateSchema<Season>(season) + commentLine);

                OMDbApiNet.Model.SeasonEpisode ep = new SeasonEpisode();
                richTextBox1.AppendText(schemacontr.CreateSchema<SeasonEpisode>(ep) + commentLine);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            TestConnection();
        }

        private bool TestConnection(bool verbose = false)
        {
            bool ret = false;
            try
            {
                string sql = "select 1 = 1;";
                var table = MySQLController.GetData(_userSettings.DefaultConnection, sql);
                if (table != null)
                {
                    MySqlConnection conn = new MySqlConnection(_userSettings.DefaultConnection);
                    string text = $"Successfully connected to {conn.Database} - {_userSettings.DefaultConnection}";
                    if (verbose)
                    {
                        //MessageBox.Show(text, $"Success");
                        SetStatusLabelText($"[SUCCESS] {text}");

                        Kolibri.net.Common.FormUtilities.Forms.SplashScreen.Splash(text, 2000, Common.FormUtilities.Forms.TypeOfMessage.Success);
                    }
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                string text = $"{ex.Message} - {_userSettings.DefaultConnection}";
                if (verbose) MessageBox.Show(text, $"{ex.GetType().Name} - is the DB connection correct?");
                SetStatusLabelText($"[ERROR] {text}");
            }
            return ret;
        }

        private void buttonCreateDapperClasses_Click(object sender, EventArgs e)
        {
            string ns = "ImdbDataFiles";
            try
            {
                DapperGenericRepository.Controller.ConvertDBTableToDapperClassesController conv = new ConvertDBTableToDapperClassesController(_userSettings.DefaultConnection);
                var list = conv.GetTableNames();
                var process = percentCalc.InitProgressBar(toolStripProgressBar1);

                var dest = new DirectoryInfo($@"c:\temp\{ns}");
                if (!dest.Exists) dest.Create();
                foreach (var name in list)
                {
                    conv.GetClassForTable(name, dest, ns);
                }
                Process.Start(new ProcessStartInfo(dest.FullName) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private async void buttonInsertFromLiteDBToMySQLDB_Click(object sender, EventArgs e)
        {

            try
            {
                IProgress<int> process = percentCalc.InitProgressBar(toolStripProgressBar1);

                int counter = 0;
                ItemService itemservice = new ItemService(_userSettings.DefaultConnection);
                DapperBulkInsertController episodeController = new DapperBulkInsertController(_userSettings.DefaultConnection);
                List<Item> list = _liteDB.FindAllItems().GetAwaiter().GetResult().ToList();  
                foreach (var item in list)
                {
                    try
                    {
                        counter++;
                        item.Ratings = null;
                        if (_cancel) return;
                        string cat = "[INSERT]";
                        if (!itemservice.Add(item)) { cat = "[ERROR]"; }

                        SetStatusLabelText($"{cat} Inserting {nameof(Item)} {item.ImdbId} - {item.Title} ({item.Type})");
                        await Task.Delay(4);
                        process.Report(percentCalc.CalculatePercent(counter, list.Count()));
                    }
                    catch (Exception ex)
                    {
                        SetStatusLabelText($"Inserting {nameof(Item)} complete, counted {counter} - {ex.GetType().Name} -  {ex.Message}");
                    }
                
                }

                // var res=          await episodeController.BulkInsert(list.ToList());
                SetStatusLabelText($"Inserting {nameof(Item)} complete, counted {counter};");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabelText($"Inserting complete, with error: {ex.GetType().Name} -  {ex.Message}");
            }
        }
        #endregion
    }
}