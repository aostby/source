using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Forms;
using Kolibri.SortPictures.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.SortPictures.Forms
{
    public partial class MainForm : Form
    {
        private DirectoryInfo searchPath;
        public MainForm()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = $"{Assembly.GetExecutingAssembly().GetName().Name} ({version.Major}.{version.Minor}.{version.Build})";
            this.Icon = Resources.redsortpictures;
        }

        private void MDIChildNew_Click(object sender, EventArgs e)
        {

            Form newMDIChild = null;
            try
            {
                if (sender.Equals(sorterBilderToolStripMenuItem)) { newMDIChild = new SortPicsDesktopForm(); }
                else if (sender.Equals(reduserBildestørrelseToolStripMenuItem)) { newMDIChild = new SchrinkImagesForm(); }
                newMDIChild.MdiParent = this;
                newMDIChild.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MappeUtilsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DirectoryInfo folder = null;
            try
            {
                var dlg1 = new Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx();
                dlg1.Description = $"Select a folder for your operattion ({(sender as ToolStripMenuItem).Text}):";
                dlg1.ShowNewFolderButton = true;
                dlg1.ShowEditBox = true;
                dlg1.ShowFullPathInEditBox = true;
                dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

                if (searchPath != null && searchPath.Exists) { dlg1.SelectedPath = searchPath.FullName;}

                   
                // Show the FolderBrowserDialog.
                DialogResult result = dlg1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    searchPath = new DirectoryInfo(dlg1.SelectedPath);

                    if (sender.Equals(fjernTommeMapperToolStripMenuItem))
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(dlg1.SelectedPath);
                        Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(dinfo);
                    }
                    else if (sender.Equals(listExtensionsToolStripMenuItem))
                    {
                        var liste = FolderUtilities.SearchAccessibleFilesNoDistinct(dlg1.SelectedPath);

                        ListBox mylist = new ListBox();
                        mylist.DataSource = liste;
                        mylist.Dock = DockStyle.Fill;
                        mylist.MouseDoubleClick += listBoxOpenFileExtension_MouseDoubleClick;
                     
                        Form form = new Form(); form.Text = (sender as ToolStripMenuItem).Text + ",  right clic to delete";
                        form.Size = new Size(300, 300);
                        form.Controls.Add(mylist);
                        form.MdiParent = this;
                        form.Show();

                    }
                }
                else MessageBox.Show("Operation cancelled.", "No action taken");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                Cursor = Cursors.Default;   
                return;
            }

            Cursor = Cursors.Default;
        }
        void listBoxOpenFileExtension_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                int index = (sender as ListBox).IndexFromPoint(e.Location);
                if (index != System.Windows.Forms.ListBox.NoMatches)
                {Cursor = Cursors.WaitCursor;   
                    var liste = FileUtilities.GetFiles(searchPath, $"*{(sender as ListBox).Items[index].ToString()}", true);
                    Cursor = Cursors.Default;
                    ListBox mylist = new ListBox();
                    mylist.DataSource = liste;
                    mylist.Dock = DockStyle.Fill;
                    mylist.MouseDoubleClick += listBoxOpenfile_MouseDoubleClick;                   
                    Form form = new Form();form.Text = $"*{(sender as ListBox).Items[index].ToString()} - doubleclick to open";
                    form.Size = new Size(300, this.Width-10);
                    form.Controls.Add(mylist);
                    form.MdiParent = this;
                    form.Show();
                }
            }
            catch (Exception)
            {
            }
        } 

        private void listBoxOpenfile_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            int index = (sender as ListBox).IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                FileInfo info = new FileInfo($"{(sender as ListBox).Items[index].ToString()}");
                if (info.Exists) { FileUtilities.OpenFolderHighlightFile(info);
                }
                else {
                    MessageBox.Show($"File {info.FullName} does not exist!", "Missing filereference", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            } 
        }
        private void lagIconFilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = null;
            try
            {
                var dlg1 = new OpenFileDialog();
                dlg1.Title = "Select a file to create icon from:";

                dlg1.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);

                // Show the FolderBrowserDialog.
                DialogResult result = dlg1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileInfo = new FileInfo(dlg1.FileName);

                    Icon ico = Kolibri.Common.Utilities.Icons.IconFromImage(Image.FromFile(fileInfo.FullName));
                    fileInfo = new FileInfo(Path.ChangeExtension(fileInfo.FullName, ".ico"));
                    using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Create))
                    {
                        ico.Save(fs);
                    }
                    if (MessageBox.Show($"{fileInfo.Name} created.\r\nOpen folder to show file?", "Operation complete.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(fileInfo);
                    }
                }
                else MessageBox.Show("Operation cancelled.", "No action taken");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
                return;
            }

        }
    }
}