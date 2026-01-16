using FastColoredTextBoxNS;
using File_Organizer;
 

using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.C64Sorter.Forms;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kolibri.net.C64Sorter
{
    public partial class MainForm : Form
    {
        private UE2LogOn _ue2logon = new UE2LogOn();
        private DirectoryInfo _sSelectedFolder;
        private string _searchText = string.Empty;
        private Controllers.UltmateEliteClient _client = null;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private async void Init()
        {
            this.KeyPreview = true; // This makes the form receive key events first.

            try { _ue2logon = JsonConvert.DeserializeObject<UE2LogOn>(File.ReadAllText(UltmateEliteClient.AppsettingsPath.FullName)); } catch (Exception) { }
            if (string.IsNullOrWhiteSpace(_ue2logon.Hostname))
            {
                SetStatusLabel($"NB! - no IP or Hostname for the Ultimate Elite II is set! Please fill it in in the {fileToolStripMenuItem.Text} menu.");
            }
            else
            { 
                try
                {
                    _client = new UltmateEliteClient(_ue2logon.Hostname);
                    var sysInfo = await _client.GetSystemInformationAsync();
                    SetStatusLabel($"{sysInfo.Product} - Host: {sysInfo.Hostname}, Firmware version: {sysInfo.FirmwareVersion}");
                }
                catch (AggregateException aex)
                {
                    MessageBox.Show(aex.Message, aex.GetType().Name);
                    SetStatusLabel($"Getting SystemInformation failed: {aex.Message}");
                }
                catch (Exception ex)
                {
                    SetStatusLabel($"SystemInformation failed: {ex.Message}");
                }
            }

            this.AllowDrop = true;
            this.Text = $" {Assembly.GetExecutingAssembly().GetName().Name} ({_ue2logon.Hostname})";
            try
            {
                this.Text += $" Version: {Assembly.GetExecutingAssembly()?
                                 .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                 .InformationalVersion.Substring(0, 5)}";

            }
            catch (Exception) { }

            try
            {
                InitToolsMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().Name, ex.Message);
            }

          

        }

        private void InitToolsMenu()
        {
            if (toolStripMenuItemTools.HasDropDownItems)
                toolStripMenuItemTools.DropDownItems.Clear();
            List<FileItemDetail> items = new List<FileItemDetail>();
            try
            {

                // Read the JSON file content
                string jsonString = File.ReadAllText(Path.Combine(UltmateEliteClient.ResourcesPath.FullName, "tools.json"));
                toolStripMenuItemTools.Visible = true;

                items = JsonConvert.DeserializeObject<List<FileItemDetail>>(jsonString);

            }
            catch (Exception ex) { toolStripMenuItemTools.Visible = false; }

            foreach (var item in items)
            {
                if (File.Exists($"{item.FullPath}"))
                    try
                    {
                        item.Icon = Icon.ExtractAssociatedIcon(item.FullPath).ToBitmap();
                    }
                    catch (Exception)
                    { }
                ToolStripMenuItem tmi = new ToolStripMenuItem()
                {
                    ToolTipText = $"{item.Description}",
                    Image = item.Icon ?? null,
                    Text = item.Name,
                    Tag = item.FullPath
                };
                tmi.Tag = item;
                tmi.Click += toolsMenuItem_Click;
                toolStripMenuItemTools.DropDownItems.Add(tmi);
            }
        }

        public void SetStatusLabel(string statusText)
        {
            try { toolStripStatusLabelStatus.Text = statusText; }
            catch (Exception) { }
        }

        private void organizeFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form newMDIChild = null;
                if (sender.Equals(singleFileCategorizerToolStripMenuItem))
                {
                    newMDIChild = new FileOrganizer();
                }
                else if (sender.Equals(extensionOrganizerToolStripMenuItem))
                {
                    newMDIChild = new ExtensionOrganizer();
                }
                newMDIChild.MdiParent = this;
                newMDIChild.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception)
            {

            }

            Application.Exit();
        }

        private void windowsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                if (sender.Equals(closeAllToolStripMenuItem)) { foreach (Form frm in this.MdiChildren) { frm.Dispose(); return; } }
                else if (sender.Equals(cascadeWindowsToolStripMenuItem)) { this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade); }
                else if (sender.Equals(tileVerticalToolStripMenuItem)) { this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical); }
                else if (sender.Equals(tileHorizontalToolStripMenuItem)) { this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal); }
                else if (sender.Equals(arrangeIconsToolStripMenuItem)) { this.LayoutMdi(System.Windows.Forms.MdiLayout.ArrangeIcons); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void toolStripMenuItemRemoveEmptyDirs_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo folder = FileUtilities.LetOppMappe(null, "Let opp mappen med tomme undermapper");
                if (folder != null && folder.Exists)
                {
                    SetStatusLabel($"Starting recursive removal of empty directories from ...\\{folder.Name}");
                    Kolibri.net.Common.Utilities.FileUtilities.DeleteEmptyDirs(folder);
                    SetStatusLabel($"Operation complete. Name: \\{folder.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabel($"Exception occured when trying to remove directories from folder.");
            }
        }

        private void d64FoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = $"Look up folder with with files in it";

                if (fbd.ShowDialog() == DialogResult.OK)
                {

                    _sSelectedFolder = new DirectoryInfo(fbd.SelectedPath);
                    D64ImagesController ctrl = new D64ImagesController(new DirectoryInfo(_sSelectedFolder.FullName));
                    var jall = ctrl.GetDiskInfos();
                    FileUtilities.OpenFolderHighlightFile(jall);
                }
                else throw new FileNotFoundException("file wasnt found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void PrintFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = $"Look up folder with with files in it";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _sSelectedFolder = new DirectoryInfo(fbd.SelectedPath);
                    var filetype = $"{(sender as ToolStripMenuItem).Tag}";

                    var list = _sSelectedFolder.GetFiles($"*.{filetype}", SearchOption.TopDirectoryOnly);

                    var infos = list.Where(file => file.Extension.Contains(filetype, StringComparison.OrdinalIgnoreCase)).ToList();
                    SetStatusLabel($"Printing {_sSelectedFolder.Name}");
                    if (infos.Count >= 1)
                    {
                        PrinterController.PrintImage(infos);
                    }
                    else SetStatusLabel($"No files found ({filetype}).");

                }
                else throw new FileNotFoundException("file wasnt found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            SetStatusLabel($"Printing completed.");
        }

        private void toolStripMenuItemFTP_Click(object sender, EventArgs e)
        {
            try
            {
                Form newMDIChild = null;

                if (sender.Equals(toolStripMenuItemFTPClient))
                {
                    //newMDIChild = new frmFTP(_ue2logon.Hostname, _ue2logon.Username, _ue2logon.Password);
                    var json = Path.Combine(UltmateEliteClient.ResourcesPath.FullName, $"tools.json");
                    if (File.Exists(json))
                    {
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(json));

                        foreach (var item in data)
                        {
                            FileInfo info = new FileInfo($"{item.FullPath}");
                            if (info.Exists && (info.Name.EndsWith("filezilla.exe") || info.Name.Contains("FTP")))
                            {
                                FileUtilities.Start(info);
                                SetStatusLabel($"Staring FTP: {info.Name}");
                                return;
                            }
                        }
                    }
                }
                else if (sender.Equals(toolStripMenuItemFTPTreeView))
                {
                    newMDIChild = new FTPTreeviewForm(_ue2logon);
                }
                newMDIChild.MdiParent = this;
                newMDIChild.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabel($"FTP: {ex.Message}");
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {

            object displayChoice = null;
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (_sSelectedFolder != null && _sSelectedFolder.Exists)
                {
                    fbd.InitialDirectory = _sSelectedFolder.FullName;
                }
                fbd.UseDescriptionForTitle = true;
                fbd.Description = $"Look up folder with with .d64 files with whilename to search for.";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _sSelectedFolder = new DirectoryInfo(fbd.SelectedPath);
                    D64ImagesController ctrl = new D64ImagesController(new DirectoryInfo(_sSelectedFolder.FullName));

                    var choice = InputDialogs.InputBox("Part of filename to search for", "Please input a search string", ref _searchText);
                    if (choice == DialogResult.Cancel) return;
                    Cursor.Current = Cursors.WaitCursor;
                    var result = ctrl.GetSearchResults(_searchText);
                    Cursor.Current = Cursors.Default;
                    DataSet ds = DataSetUtilities.CSVToDataSet(result, ",", true);
                    DataTable table = ds.Tables[0];
                    table.TableName = FileUtilities.SafeFileName(_searchText);
                    // Convert the DataTable to an EnumerableRowCollection, select the column values, and get distinct ones
                    IEnumerable<string> distinctValues = table.AsEnumerable()
                                                             .Select(row => row.Field<string>(table.Columns[0].ColumnName)) // Use Field<T> for type safety and handling nulls
                                                             .Distinct();


                    choice = InputDialogs.ChooseListBox($"Choose action for {table.TableName}", "Please choose how to present the result",
                                      new List<string>() { "1 - Just show me the CSV file",
                                             "2 - Table with result, so I can sort and such, with the possibility to 'SAVE as...' ( CTRL+s )  ",
                                             $"3 - Open each {distinctValues.Count()} .d64 file with whatever my OS says is the program for those extensions. (result has rowcount: {table.Rows.Count})" },
                                      ref displayChoice, false);
                    if (choice == DialogResult.Cancel) return;


                    switch (((displayChoice as ListViewItem).Text.ToString())[0])
                    {
                        case '1': FileUtilities.OpenFolderHighlightFile(result); break;
                        case '2': OutputDialogs.ShowDataTableDialog(table.TableName, table, this.Size); break;
                        case '3':

                            foreach (var item in distinctValues)
                            {
                                FileUtilities.Start(new FileInfo(item.Trim('"')));
                                Thread.Sleep(500);
                            }
                            break;
                        default:
                            OutputDialogs.ShowDataTableDialog(table.TableName, table, this.Size);
                            break;
                    }

                }
                else throw new FileNotFoundException("file wasnt found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }


        private async void toolsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var info = new FileInfo(((sender as ToolStripMenuItem).Tag as FileItemDetail).FullPath);
                FileUtilities.Start(info);

            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }

        }

        private async void mountersMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_ue2logon.Hostname)) { toolStripMenuItemHostname_Click(null, null); return; }
              
                var text = ((sender as ToolStripMenuItem).Text.Replace("or", " ").Replace(",", " ").Split(" ")).ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray();
                string filter = FileUtilities.GetFileDialogFilter(text, true);

                FileInfo fbd = FileUtilities.LetOppFil(_sSelectedFolder, $"{text} file to search for ", filter = filter);
                if (fbd != null && fbd.Exists
                          && fbd.Name.Contains(".D64", StringComparison.OrdinalIgnoreCase)
                          || fbd.Name.Contains(".G64", StringComparison.OrdinalIgnoreCase)
                          || fbd.Name.Contains(".D81", StringComparison.OrdinalIgnoreCase)
                          || fbd.Name.Contains(".D71", StringComparison.OrdinalIgnoreCase)
                          || fbd.Name.Contains(".G71", StringComparison.OrdinalIgnoreCase)
                          )
                {
                    SetStatusLabel($"Mounting {fbd.Name} remotely to {_ue2logon.Hostname}");
                    var existing = _client.FtpUpload(_ue2logon.Hostname, fbd.FullName);
                    if (fbd.Extension.Substring(fbd.Extension.Length - 2, 2).ToInt32() > 41)
                    { Thread.Sleep(1000); }
                    _client.MountAndRunExistingTempFile(_ue2logon.Hostname, Path.GetFileName(fbd.Name), false);
                    SetStatusLabel($"Mounting {fbd.Name} remotely to {existing}");
                }
                else SetStatusLabel($"Wrong filetype: {fbd.Name} - not a {string.Join(" or ", text)}");
            }
            catch (HttpRequestException hex)
            {
                MessageBox.Show(hex.Message, hex.GetType().Name);
                SetStatusLabel($"Mounting failed: {hex.Message}");
            }
            catch (AggregateException aex)
            {
                MessageBox.Show(aex.Message, aex.GetType().Name);
                SetStatusLabel($"Mounting failed: {aex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabel($"Mounting failed: {ex.Message}");
            }
        }
        private async void runnersMenuItem_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(_ue2logon.Hostname)) { toolStripMenuItemHostname_Click(null, null); return; }
            try
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
              //  Controllers.UltmateEliteClient client = new UltmateEliteClient(_hostname.Hostname);
                var text = ((sender as ToolStripMenuItem).Text.Replace("or", " ").Replace(",", " ").Split(" ")).ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray();
                string filter = FileUtilities.GetFileDialogFilter(text, true);

                if (sender.Equals(allFilesToolStripMenuItem))
                {
                    filter = FileUtilities.GetFileDialogFilter(new List<string>() { "*.*" }.ToArray(), true);
                }


                FileInfo fbd = FileUtilities.LetOppFil(_sSelectedFolder, $"{item.Text} file to search for ", filter = filter);
                if (fbd != null && fbd.Exists)
                {
                    if (fbd.Exists && (fbd.Name.Contains(".PRG", StringComparison.OrdinalIgnoreCase) || fbd.Name.Contains(".CRT", StringComparison.OrdinalIgnoreCase)))
                    {
                        SetStatusLabel($"Mounting {fbd.Name} remotely to {_ue2logon}");
                        _client.UploadAndRunPrgOrCrt(_ue2logon.Hostname, new FileInfo(fbd.FullName));

                    }
                    else if (fbd.Exists && fbd.Name.Contains(".D64", StringComparison.OrdinalIgnoreCase)
                        || fbd.Name.Contains(".G64", StringComparison.OrdinalIgnoreCase)
                        || fbd.Name.Contains(".D81", StringComparison.OrdinalIgnoreCase)
                        || fbd.Name.Contains(".D71", StringComparison.OrdinalIgnoreCase)
                        || fbd.Name.Contains(".G71", StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        SetStatusLabel($"Mounting {fbd.Name} remotely to {_ue2logon.Hostname}");
                        var existing = _client.FtpUpload(_ue2logon.Hostname, fbd.FullName);
                        if (fbd.Extension.Substring(fbd.Extension.Length - 2, 2).ToInt32() > 41)
                        { await _client.MachineReset(); Thread.Sleep(3500); }
                        _client.MountAndRunExistingTempFile(_ue2logon.Hostname, Path.GetFileName(fbd.Name));
                        SetStatusLabel($"Mounting {fbd.Name} remotely to {existing}");
                    }
                    else if (fbd.Exists && fbd.Name.Contains(".SID", StringComparison.OrdinalIgnoreCase)
                        || fbd.Name.Contains(".MOD", StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        SetStatusLabel($"Mounting {fbd.Name} remotely to {_ue2logon.Hostname}");
                        var existing = _client.FtpUpload(_ue2logon.Hostname, fbd.FullName);
                        _client.UploadAndRunPrgOrCrt(_ue2logon.Hostname, fbd);
                        SetStatusLabel($"Mounting {fbd.Name} remotely to {existing}");
                    }
                    else { throw new Exception($"{fbd.Extension.ToUpper()} files not supported!"); }
                }
            }
            catch (HttpRequestException hex)
            {
                MessageBox.Show(hex.Message, hex.GetType().Name);
                SetStatusLabel($"Mounting failed: {hex.Message}");
            }
            catch (AggregateException aex)
            {
                MessageBox.Show(aex.Message, aex.GetType().Name);
                SetStatusLabel($"Mounting failed: {aex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                SetStatusLabel($"Mounting failed: {ex.Message}");
            }
        }






        private void toolStripMenuItemHostname_Click(object sender, EventArgs e)
        {
            string filename = UltmateEliteClient.AppsettingsPath.FullName;
            try
            {
                UE2LogOn logon = new UE2LogOn();
                string hostname = _ue2logon.Hostname;
                string username = _ue2logon.Username;
                string password = _ue2logon.Password;

                try
                {
                    logon = JsonConvert.DeserializeObject<UE2LogOn>(File.ReadAllText(filename));
                }
                catch (Exception) { }

                var choice = InputDialogs.InputBox("IP or HostName", "Please input a value", ref hostname);
                if (choice == DialogResult.OK)
                {
                    var res = InputDialogs.PasswordDialog("Set username / password (blank if none)", "Leave blank for anonymous", ref username, ref password);
                    if (res == DialogResult.OK)
                    {
                        logon.Username = username;
                        logon.Password = password;
                        logon.Hostname = hostname;
                        _ue2logon = logon;
                        File.WriteAllText(filename, logon.JsonSerializeObject());

                        SetStatusLabel($"IP or Hostname for the Ultimate Elite II is set! {_ue2logon.Hostname}.");
                        Init();
                    }
                    else { SetStatusLabel($"No changes to config for {_ue2logon.Hostname}."); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private async void aboutC64SorterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form form = new net.Common.FormUtilities.Forms.WebBrowserForm("https://www.problembar.net/uranus/BeerXML/uploads/APPS/Kolibri.net.C64Sorter_UserManual.pdf");
                form.MdiParent = this;
                form.Show();

                //   Controllers.UltmateEliteClient client = new UltmateEliteClient(_hostname.Hostname);
                var version = "1.0.0.0";// await _client.GetVersionAsync().ToString();

                try
                {
                    version = Assembly.GetExecutingAssembly()?
                                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                      .InformationalVersion.Substring(0, 5).ToString();
                }
                catch (Exception)
                { }


                string text = $"Version: {version} - {GetIPv4AddressForInterface("en0")}";

                SplashScreen.Splash(text, 2000, this.Icon.ToBitmap());



            }
            catch (Exception ex)
            {

                SplashScreen.Splash("PCUtil for the Commodore Ultimate Elite II", 4000, this.Icon.ToBitmap());
                SetStatusLabel(ex.Message);
            }

        }

        private void browseLocalFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender.Equals(toolStripMenuItemOpenResoursesfolder))
                { FileUtilities.OpenWindowsExplorer(Controllers.UltmateEliteClient.ResourcesPath.FullName); }
                else
                {
                    FileUtilities.OpenWindowsExplorer(_sSelectedFolder == null ? null : _sSelectedFolder.FullName);
                }
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
        }

        private void machineToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
             //   Controllers.UltmateEliteClient client = new UltmateEliteClient(_hostname.Hostname);
                if (sender.Equals(resetToolStripMenuItem))
                { _client.MachineReset(); }
                else if (sender.Equals(rebootToolStripMenuItem))
                    _client.MachineReboot();

                else if (sender.Equals(pauseToolStripMenuItem))
                    _client.MachinePause();
                else if (sender.Equals(resumeToolStripMenuItem))
                    _client.MachineResume();
                else if (sender.Equals(powerOffToolStripMenuItem))
                    _client.MachinePowerOff();
                else if (sender.Equals(ue2MenutoolStripMenuItem))
                    _client.MachineMenu();
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
        } 

        public string? GetIPv4AddressForInterface(string interfaceName)
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.Name == interfaceName);

                if (networkInterface == null)
                {
                    Console.WriteLine($"No network interface found with name: {interfaceName}");
                    return null;
                }

                var ipProperties = networkInterface.GetIPProperties();
                var ipv4Address = ipProperties.UnicastAddresses
                    .FirstOrDefault(ua => ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                return ipv4Address?.Address.ToString();
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
            return string.Empty;
        }

        private void linksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo info = new FileInfo(@".\Resources\links.rss");

                var rssfeed = File.ReadAllText(info.FullName);
                Form form = new Kolibri.net.Common.FormUtilities.Forms.RSSForm(info.FullName);
                form.MdiParent = this;
                form.Show();
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
        }

        private void ConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //     Controllers.UltmateEliteClient client = new UltmateEliteClient(_hostname.Hostname);
                if (sender.Equals(volumeToolStripMenuItem))
                {
                    Form form = new Forms.VolumeControlForm(_client);
                    form.MdiParent = this;
                    form.Show();
                }
                else if (sender.Equals(configsToolStripMenuItem))
                {
                    Form form = new Forms.ConfigsTreeviewForm(_ue2logon);
                    form.MdiParent = this;
                    form.Show();
                }
                else if (sender.Equals(videostreamToolStripMenuItem))
                {
                    Form form = new Forms.VideoStreamForm(_ue2logon);
                    form.MdiParent = this;
                    form.Show();
                }
                else if (sender.Equals(displayCfgFilesToolStripMenuItem)) {
                    string filter = FileUtilities.GetFileDialogFilter(new List<string>() { "cfg" }.ToArray(), true);
                    FileInfo fileInfo = FileUtilities.LetOppFil(title: "Let opp mappen med tomme undermapper", filter: filter);
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        Form form = Common.FormUtilities.Controller.OutputFormController.RichTexBoxForm(fileInfo.Name, "",
                            //FastColoredTextBoxNS.Language.PHP,
                            //FastColoredTextBoxNS.Language.Lua,
                            FastColoredTextBoxNS.Language.Custom,
                            new Size(600, 300));

                   var fctb=     form.Controls.Find("dgv", false).FirstOrDefault() as FastColoredTextBoxNS.FastColoredTextBox;
                        fctb.TextChanged += (s, e) =>
                        {
                            HighlightIni(fctb);
                        }; 

                        form.MdiParent = this;
                        form.Show();
                        fctb.Text = FileUtilities.ReadTextFile(fileInfo.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
        }
        Style sectionStyle = new TextStyle(Brushes.DarkBlue, null, FontStyle.Bold);
        Style keyStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        Style valueStyle = new TextStyle(Brushes.DarkOliveGreen, null, FontStyle.Regular);
        Style commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private      void HighlightIni(FastColoredTextBoxNS.FastColoredTextBox tb)
        {
            //tb.ClearStyle( (sectionStyle, keyStyle, valueStyle, commentStyle);

            // Sections [Section]
            tb.Range.SetStyle(sectionStyle, @"^\s*\[.+?\]\s*$", RegexOptions.Multiline);

            // Comments ; or #
            tb.Range.SetStyle(commentStyle, @"^\s*[;#].*$", RegexOptions.Multiline);

            // Keys (left side of =)
            tb.Range.SetStyle(keyStyle, @"^\s*[^=;\[#]+(?=\=)", RegexOptions.Multiline);

            // Values (right side of =)
            tb.Range.SetStyle(valueStyle, @"(?<=\=).*", RegexOptions.Multiline);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("DragEnter!");


            // Check if the data being dragged is a file drop (e.g., from Explorer)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // If it is, show the 'Copy' cursor icon (or other effect)
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Otherwise, show the 'None' (no-drop) cursor icon
                e.Effect = DragDropEffects.None;
            }
        }

        // Event handler for when the user releases the mouse button over the form
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the file paths from the data object into a string array
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Process the dropped files (example: display in a MessageBox)
            if (files != null && files.Length > 0)
            {
                string text = "Dropped files:\n" + string.Join("\n", files);
                SetStatusLabel(text);

                // You can add further logic here, such as loading the file contents
                // into a control or performing file operations.


                Controllers.UltmateEliteClient client = new UltmateEliteClient(_ue2logon.Hostname);
                //client.UploadAndRunPrgOrCrt(_hostname.Hostname, files[0].ToString());
                var rt = string.Empty;
                foreach (var item in files)
                {
                    text = "Uploading file:\n" + item; SetStatusLabel(text);
                    rt = client.FtpUpload(_ue2logon.Hostname, item);
                }


                Uri uri = new Uri(rt);
                string ext = Path.GetExtension(Path.GetFileName(uri.LocalPath));
                if ((ext.ToLower().Contains("mod") || ext.ToLower().Contains("sid")))
                {
                    var retur = client.UploadAndRunPrgOrCrt(_ue2logon.Hostname, new FileInfo(files[0].ToString()));
                }
                else client.MountAndRunExistingTempFile(_ue2logon.Hostname, Path.GetFileName(uri.LocalPath));

            }
        }

        private void MainForm_DragOver(object sender, DragEventArgs e)
        {
            // Check if the data being dragged is something your control can use (e.g., files)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //   DragDropEffects.Copy; // Allow copying files
                //   e.Handled = true; // Tell the system we handled it
            }
            else
            {
                // e.Effects = DragDropEffects.None; // Show the red icon (not allowed)
                //  e.Handled = true;
            }
        }

        private async void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
              //  Controllers.UltmateEliteClient client = new UltmateEliteClient(_hostname.Hostname);
                var key = e.KeyValue;
                switch (key)
                {
                    case 173/*mute*/:
                        var tmp = await _client.ConfigurationGetSpeakerEnable();
                        switch (tmp)
                        {
                            case "Enabled": await _client.ConfigurationSpeakerEnable("Disabled"); break;
                            case "Disabled": await _client.ConfigurationSpeakerEnable("Enabled"); break;
                            default:
                                break;
                        }
                        e.Handled = true; break;
                    case 174/*down*/:
                        var current = await _client.ConfigurationGetVolumeLevel();

                        await _client.ConfigurationVolumeLevel(current - 5);
                        SetStatusLabel($"Volume down pressed. ({current})");
                        e.Handled = true; break;
                    case 175/*up*/:
                        var level = await _client.ConfigurationGetVolumeLevel();
                        int value = level;
                        if (level < -18)
                        {
                            if (level > -27)
                                value = -24 + 4;
                            else if (level > -30)
                                value = -27 + 5;
                            else if (level > -36)
                                value = -30 + 5;
                            else if (level > -36)
                                value = -30 + 5;
                            else value = -5;
                        }

                        value = value + 5;

                        SetStatusLabel($"Volume up pressed. ({value})");
                        await _client.ConfigurationVolumeLevel(value + 1);
                        e.Handled = true; break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        [Obsolete("LibVLC er for komprimerte formater, finn ut hvordan proprietær udp for UE2 kan løses")]
        private void uE2VideoStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form form = new Forms.VideoStreamForm(_ue2logon);
                form.MdiParent = this;
                form.Show();
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
            }
        }

        private async void commandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = DialogResult.Cancel;
                string script = string.Empty;
                if (sender.Equals(commandToolStripMenuItem))
                {
                    res = InputDialogs.InputBox($"Send a command to {_ue2logon.Hostname} (no PETSCII)", "Command to send (run, load, poke etc.)", ref script);
                }
                else if (sender.Equals(scriptToolStripMenuItem))
                {
                    res = InputDialogs.InputRichTextBox($"Send a script to {_ue2logon.Hostname} (no PETSCII or special commands)", "Command to send (10 print MyName 20 Goto 10; etc.)", ref script);
                }
                if (res == DialogResult.OK)
                {

                    using (Controllers.UltmateEliteClient client = new UltmateEliteClient(_ue2logon.Hostname))
                    {
                        foreach (var line in script.Split(Environment.NewLine))
                        {
                            var test = await client.SendCommand(line);
                            Thread.Sleep(100);
                        }
                        client.Dispose();
                    }
                }
                else SetStatusLabel($"No command given!");
            }
            catch (Exception ex)
            {
                SetStatusLabel($"{ex.GetType().Name} - {ex.Message}");
            }
        }

        private void toolStripMenuItemHTTPServer_Click(object sender, EventArgs e)
        {
            try
            {
                Form form = new Kolibri.net.Common.FormUtilities.Forms.WebBrowserForm($"http://{_ue2logon.Hostname}");
                form.MdiParent = this;
                form.Show();
            }
            catch (Exception ex)
            {
                SetStatusLabel($"{ex.GetType().Name} - {ex.Message}");
            }
        }

       
    }
}