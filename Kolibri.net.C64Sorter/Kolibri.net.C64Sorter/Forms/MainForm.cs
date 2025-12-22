 
using File_Organizer;
using FTP_Connect;
 
using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
 
using System.Data;
 

namespace Kolibri.net.C64Sorter
{
    public partial class MainForm : Form
    {
        private DirectoryInfo _sSelectedFolder;
        private string _searchText = string.Empty;
        public MainForm()
        {
            InitializeComponent();
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
                    D64Controller ctrl = new D64Controller(new DirectoryInfo(_sSelectedFolder.FullName));
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

                    var list = _sSelectedFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    if (sender.Equals(pNGFilesToolStripMenuItem))
                    {
                        var infos = list.Where(file => new string[] { ".png" }.Contains(file.Extension)).ToList();
                        SetStatusLabel($"Printing {_sSelectedFolder.Name}");
                        PrinterController.PrintImage(infos);
                    }
                }
                else throw new FileNotFoundException("file wasnt found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            SetStatusLabel($"Printing completed.");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form newMDIChild = new frmFTP();
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
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
                    D64Controller ctrl = new D64Controller(new DirectoryInfo(_sSelectedFolder.FullName));

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

        private void pRGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string hostname = string.Empty;
            try
            {
                try { hostname = File.ReadAllText(@".\Resources\hostname.txt"); } catch (Exception) { }

                var fbd = FileUtilities.LetOppFil(_sSelectedFolder, "PRG file to search for ");



                if (fbd.Exists)
                {

                    var choice = InputDialogs.InputBox("IP or HostName", "Please input a value", ref hostname);
                    Controllers.UltmateEliteClient client = new UltmateEliteClient(hostname);
                    client.UploadAndRunPrg(hostname, fbd.FullName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private async void d64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string hostname = string.Empty;
            try
            {
                try { hostname = File.ReadAllText(@".\Resources\hostname.txt"); } catch (Exception) { }
                if (string.IsNullOrEmpty(hostname))
                {
                    toolStripMenuItemHostname_Click(null, null);
                    return;
                }



                var fbd = FileUtilities.LetOppFil(_sSelectedFolder, "PRG file to search for ");

                if (fbd.Exists)
                {
                    SetStatusLabel($"Mounting {fbd.Name} remotely to {hostname}");
                    Controllers.UltmateEliteClient client = new UltmateEliteClient(hostname);
                    var existing = client.FtpUpload(hostname, fbd.FullName);
                    client.MountAndRunExistingTempFile(hostname, Path.GetFileName(fbd.Name));

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
            try
            {
                string hostname = string.Empty;

                try { hostname = File.ReadAllText(@".\Resources\hostname.txt"); } catch (Exception) { }

                var choice = InputDialogs.InputBox("IP or HostName", "Please input a value", ref hostname);
                if (choice == DialogResult.OK)
                {
                    File.WriteAllText(@".\Resources\hostname.txt", hostname);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void aboutC64SorterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplashScreen.Splash("PCUtil for the Commodore Ultimate Elite II",4000, this.Icon.ToBitmap());
        }
    }
}