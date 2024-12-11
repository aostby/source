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

        private void InitUserSettings(FileInfo liteDBPath=null)
        {
            string dbPath = @"C:\TEMP\SilverScreen\SilverScreen.db";
            if (liteDBPath != null) {
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
            }

        }

        private void Button_Click(object? sender, EventArgs e)
        {
            (sender as Button ) .DialogResult = System.Windows.Forms.DialogResult.OK;
            ((sender as Button).Parent as Form).DialogResult = DialogResult.OK;
            ((sender as Button).Parent as Form).Close(); 
        }
    }
}