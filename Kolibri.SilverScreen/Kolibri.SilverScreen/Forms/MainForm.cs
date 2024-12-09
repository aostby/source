using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.Utilities;
using Microsoft.Extensions.Configuration;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.Utilities;
using static Kolibri.net.SilverScreen.Controls.Constants;
using Kolibri.net.SilverScreen.Forms;

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

        public static void  SetStatusLabel(string statusText)
        {
            try
            {
                toolStripStatusLabelStatus.Text = statusText;
            }
            catch (Exception)
            {
 
            }
        }

        private void InitUserSettings()
        {
            string dbPath = @"C:\TEMP\SilverScreen\SilverScreen.db";

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

            var userSettings = new UserSettings( dbPath);

            try
            {
                using (LiteDBController tmp = new(new FileInfo( userSettings.LiteDBFilePath), false, false))
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
                    if (_userSettings.LiteDBFilePath == null || !File.Exists( _userSettings.LiteDBFilePath))
                    { _userSettings.LiteDBFilePath = new FileInfo(dbPath).FullName; }

                    tmp.Upsert(_userSettings);
                }
            }
            catch (Exception ex) { }
            this.Text += $" - Current DB: ({userSettings.LiteDBFilePath})";
        }

        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void multiMedialocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newMDIChild = null;
            if (sender.Equals(movieslocalToolStripMenuItem))
            {
                newMDIChild = new MultiMediaForm(MultimediaType.Movies, _userSettings);
            }
            else if (sender.Equals(serieslocalToolStripMenuItem))
            {
                newMDIChild = new MultiMediaForm(MultimediaType.Series, _userSettings);
            }
            newMDIChild.MdiParent = this;
            newMDIChild.Show();
        }
        private void finnDuplikaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var ex = FileUtilities.LetOppMappe(null, "Velg folder du vil finne duplikate filmer fra:");

                if (ex != null)
                {
                    SameFileController contr = new SameFileController(ex);
                    var list = contr.GetDupes();
                    var ds = DataSetUtilities.AutoGenererDataSet(list);
                    Visualizers.VisualizeDataSet("Dupes", ds, this.Size);
                    var byFolder = contr.GetDupesByFolder();
                    if (list.Count() != byFolder.Count() && byFolder.Count >= 1)
                    {
                        if (MessageBox.Show("Do you want to see dupes by folders also?", "More choices awailable", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ds = DataSetUtilities.AutoGenererDataSet(list);
                            Visualizers.VisualizeDataSet("Dupes by folder", ds, this.Size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void liteDBFilepathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var tmp = FileUtilities.LetOppFil(new DirectoryInfo(_userSettings.LiteDBFilePath), "Velg fil du vil bruke tli å lese og skrive data til:");

                if (tmp != null)
                {
                    _userSettings.LiteDBFilePath = tmp.FullName;
                    using (LiteDBController ldbc = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                    {
                        ldbc.Upsert(_userSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }
    }
}