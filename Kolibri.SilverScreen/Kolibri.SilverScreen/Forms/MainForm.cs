using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.Utilities;
using Microsoft.Extensions.Configuration;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.Utilities;
using static Kolibri.net.SilverScreen.Controls.Constants;
using Kolibri.net.SilverScreen.Forms;
using sun.tools.tree;
using java.awt.print;
using Kolibri.Common.VisualizeOMDbItem;
using OMDbApiNet.Model;
using java.nio.file;
using com.sun.org.apache.bcel.@internal;
using Kolibri.net.SilverScreen.Controller;
using Microsoft.VisualBasic;
using System;

namespace Kolibri.SilverScreen.Forms
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-determine-the-active-mdi-child
    /// </summary>
    public partial class MainForm : Form
    {
        UserSettings _userSettings;
        public MainForm()
        {
            InitializeComponent();
            InitUserSettings();
        }

        public void SetStatusLabel(string statusText)
        {
            try { toolStripStatusLabelStatus.Text = statusText; }
            catch (Exception) { }
        }

        private void InitUserSettings(FileInfo liteDBPath = null)
        {
            string dbPath = @"C:\TEMP\SilverScreen\SilverScreen.db";
            if (liteDBPath != null)
            {
                dbPath = liteDBPath.FullName;
            }
            else
            {
                try
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();
                    dbPath = configuration["LiteDBPath"].ToString();

                    FileInfo info = new FileInfo(dbPath);
                    if (!Directory.Exists(info.DirectoryName))
                    {
                        info.Create();
                        if (!info.Exists) { info.Create(); }
                    }
                }
                catch (Exception) { }
            }
            var userSettings = new UserSettings(dbPath);

            try
            {
                using (LiteDBController tmp = new(new FileInfo(userSettings.LiteDBFilePath), false, false))
                {
                    var dbSettings = tmp.GetUserSettings();
                    if (dbSettings != null)
                    {

                        _userSettings = dbSettings;

                    }
                    else
                    {
                        _userSettings = userSettings;
                    }
                    if (_userSettings.LiteDBFilePath == null || !File.Exists(_userSettings.LiteDBFilePath))
                    { _userSettings.LiteDBFilePath = new FileInfo(dbPath).FullName; }

                    tmp.Upsert(_userSettings);
                }
            }
            catch (Exception ex) { }
            this.Text = $"[{this.GetType().Name}] {_userSettings.UserName} - Current DB: ({userSettings.LiteDBFilePath})";
        }

        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void multiMedialocalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                Form newMDIChild = null;
                if (sender.Equals(movieslocalToolStripMenuItem))
                {
                    newMDIChild = new ShowLocalMoviesForm(MultimediaType.Movies, _userSettings);
                }
                else if (sender.Equals(serieslocalToolStripMenuItem))
                {
                    newMDIChild = new ShowLocalSeriesForm(_userSettings);
                }
                else if (sender.Equals(flyttFilmerToolStripMenuItem))
                {
                    newMDIChild = new SortMultimediaDesktopForm(MultimediaType.Movies, _userSettings);
                }
                else if (sender.Equals(searchToolStripMenuItem))
                {
                    newMDIChild = new Kolibri.net.SilverScreen.IMDBForms.MovieForm(_userSettings);
                }
                else if (sender.Equals(genreSearchToolStripMenuItem))
                {
                    newMDIChild = new net.Common.MovieAPI.Forms.BrowseMoviesForm(_userSettings);
                }
                else if (sender.Equals(iMDbDataFilesToolStripMenuItem))
                {
                    newMDIChild = new IMDbDataFilesForm(_userSettings);
                }
                else if (sender.Equals(testCircusToolStripMenuItem))
                {
                    newMDIChild = new TestCircusForm(_userSettings);
                }

                newMDIChild.MdiParent = this;
                newMDIChild.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void finnDuplikaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var ex = FileUtilities.LetOppMappe(null, "Velg folder du vil finne duplikate filmer fra:");

                if (ex != null)
                {
                    SetStatusLabel("Searching for dupes.... please wait");
                    SameFileController contr = new SameFileController(ex);
                    var list = contr.GetDupes();
                    var ds = DataSetUtilities.AutoGenererDataSet(list);
                    Visualizers.VisualizeDataSet($"Dupes {ds.Tables[0].Rows.Count}", ds, this.Size);
                    var byFolder = contr.GetDupesByFolder();
                    if (list.Count() != byFolder.Count() && byFolder.Count >= 1)
                    {
                        if (MessageBox.Show("Do you want to see dupes by folders also?", "More choices awailable", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            var filterList = Kolibri.net.Common.Utilities.MovieUtilites.MulitpartFilter();
                            var liste = byFolder.FindAll(x => !x.FilePath.FullName.Contains("CD"));

                            foreach (string item in filterList)
                            {
                                liste = liste.FindAll(x => !x.FilePath.FullName.Contains(item, StringComparison.OrdinalIgnoreCase));
                            }



                            ds = DataSetUtilities.AutoGenererDataSet(liste);
                            Visualizers.VisualizeDataSet($"Dupes by folder ({ds.Tables[0].Rows.Count}) - fiter: {string.Join(' ', filterList.ToArray())}", ds, this.Size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
                SetStatusLabel(ex.Message);
            }
            SetStatusLabel("Dupe search completed.");
        }

        private void liteDBFilepathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tmp = FileUtilities.LetOppFil(new DirectoryInfo(_userSettings.LiteDBFilePath), "Velg fil du vil bruke tli å lese og skrive data til:");

                if (tmp != null)
                {
                    _userSettings.LiteDBFilePath = tmp.FullName;
                    _userSettings.Save();
                    InitUserSettings(new FileInfo(_userSettings.LiteDBFilePath));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void innstillingerToolStripMenuItem_Click(object sender, EventArgs e)
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
            propertyGrid1.SelectedObject = _userSettings;
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            propertyGrid1.Size = new Size(495, 480);
            // propertyGrid1.Dock = DockStyle.Top;
            form.Controls.Add(propertyGrid1);

            var res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                _userSettings.Save();
                InitUserSettings(new FileInfo(_userSettings.LiteDBFilePath));
            }


            /*   Form form = new Form();
               form.Size = new Size(500, 500);
               PropertyGrid propertyGrid1 = new PropertyGrid();
               propertyGrid1.CommandsVisibleIfAvailable = true;
               propertyGrid1.Location = new Point(10, 20);
               propertyGrid1.Size = new System.Drawing.Size(400, 300);
               propertyGrid1.TabIndex = 1;
               propertyGrid1.Text = "Innstillinger";
               this.Controls.Add(propertyGrid1);
               propertyGrid1.SelectedObject = _userSettings;
               propertyGrid1.Dock = DockStyle.Top;
               propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Right|AnchorStyles.Left|AnchorStyles.Bottom;

               form.Controls.Add(propertyGrid1);

               Button button = new Button();
               button.DialogResult = DialogResult.OK;
               button.Text = "Lagre";
               button.Click += Button_Click;
               button.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
               propertyGrid1.Dock = DockStyle.Bottom;
               button.BringToFront();
               form.Controls.Add(button);
               var res = form.ShowDialog();


               if (res == DialogResult.OK)
               {
                   _userSettings.Save();
                   InitUserSettings(new FileInfo(_userSettings.LiteDBFilePath));
               }*/

        }

        private void Button_Click(object? sender, EventArgs e)
        {
            (sender as Button).DialogResult = System.Windows.Forms.DialogResult.OK;
            ((sender as Button).Parent as Form).DialogResult = DialogResult.OK;
            ((sender as Button).Parent as Form).Close();
        }

        private async void filmerToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var dInfo = FileUtilities.LetOppMappe(_userSettings.UserFilePaths.MoviesSourcePath, $"Let opp mappe ({Kolibri.net.SilverScreen.Controls.Constants.MultimediaType.Movies})");
            if (dInfo != null && dInfo.Exists)
            {
                MoviesSearchController searchController = new MoviesSearchController(_userSettings, updateTriState: false);
                searchController.ProgressUpdated += OnProgressUpdated;
                try
                {

                    var t = await searchController.SearchForMovies(dInfo);

                    Kolibri.net.Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog("Async search for movies", searchController.CurrentLog.ToString(), this.Size);
                }
                catch (AggregateException aex)
                {
                    string agg = aex.Message;
                    foreach (var innerEx in aex.InnerExceptions)
                    {
                        agg += innerEx.Message;
                    }
                    MessageBox.Show(agg, aex.GetType().FullName);
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message, ex.GetType().FullName); }
            }
        }
        private void OnProgressUpdated(object sender, string progress)
        {
            try
            {
                SetStatusLabel(progress);
            }
            catch (Exception ex)
            { SetStatusLabel(ex.Message); }
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
       
    }
}