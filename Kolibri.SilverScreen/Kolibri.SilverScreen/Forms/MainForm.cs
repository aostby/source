using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.FormUtilities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
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
        public MainForm()
        {
            InitializeComponent();
        }
        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

     
        private void finnDuplikaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               FolderUtilities.FolderBrowserDialogEx ex = new  FolderUtilities.FolderBrowserDialogEx("Velg folder du vil finne duplikate filmer fra:");

                DialogResult res = ex.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    SameFileController contr = new SameFileController(new DirectoryInfo(ex.SelectedPath));
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

    }
}
 