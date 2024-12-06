using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
using Kolibri.SilverScreen.Forms;
using Microsoft.Extensions.Configuration;
using Ookii.Dialogs.WinForms;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;


namespace SortPics.Forms
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
            Init();
        }

        private void Init()
        {
            string dbPath = new FileInfo(@"C:\TEMP\SilverScreen\SilverScreen.db").ToString();

            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();
                dbPath = configuration["LiteDBPath"].ToString();
            }
            catch (Exception) { }

            FileInfo info = new FileInfo(dbPath);
            if (!Directory.Exists(info.DirectoryName))
            {
                info.Create();
                if (!info.Exists) { info.Create(); }
            }
            using (Kolibri.net.Common.Dal.LiteDBController tmp = new Kolibri.net.Common.Dal.LiteDBController(info, false, false))
            {
                _userSettings = tmp.GetUserSettings();
                _userSettings.LiteDBFilePath = dbPath;
                tmp.Upsert(_userSettings);
            }
        }
        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void multiMedialocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newMDIChild = null;
            if (sender.Equals(movieslocalToolStripMenuItem)) {
                newMDIChild = new MultiMediaForm(MultiMediaForm.MultimediaType.Movies, _userSettings);
            } 
            newMDIChild.MdiParent = this;
            newMDIChild.Show();

        }
    }
}
 