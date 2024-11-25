using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
 
using System.Collections.Generic;
using Kolibri.Common.Utilities;

namespace Kolibri.Common.FormUtilities.Forms
{
    /// <summary>
    /// Vis fil og directoryinfo
    /// http://www.java2s.com/Code/CSharp/GUI-Windows-Form/UseListViewtodisplayFileinfonamesizeanddate.htm
    /// </summary>
    public partial class ListViewForm : System.Windows.Forms.Form
    {
        private System.Collections.Specialized.StringCollection folderCol;

        private System.Windows.Forms.ImageList ilLarge;
        private System.Windows.Forms.ImageList ilSmall;
        private System.Windows.Forms.ListView lwFilesAndFolders;
        private System.Windows.Forms.Label lblCurrentPath;

        public ListViewForm(DirectoryInfo info)
        {
            InitializeComponent();

            // Init ListView and folder collection
            folderCol = new System.Collections.Specialized.StringCollection();
            CreateHeadersAndFillListView();

            if (System.IO.Directory.Exists(info.FullName))
            {
                PaintListView(info.FullName);
                folderCol.Add(info.FullName);
            } 

            this.lwFilesAndFolders.ItemActivate += new System.EventHandler(this.lwFilesAndFolders_ItemActivate);
            lblCurrentPath.Text = lblCurrentPath.Text + info.GetType().Name;
        }
       
        private void CreateHeadersAndFillListView()
        {
            ColumnHeader colHead;

            colHead = new ColumnHeader();
            colHead.Text = "Filename";
            this.lwFilesAndFolders.Columns.Add(colHead);

            colHead = new ColumnHeader();
            colHead.Text = "Size";
            this.lwFilesAndFolders.Columns.Add(colHead);

            colHead = new ColumnHeader();
            colHead.Text = "Last accessed";
            this.lwFilesAndFolders.Columns.Add(colHead);
        }

        private void PaintListView(string root)
        {
            try
            {
                ListViewItem lvi;
                ListViewItem.ListViewSubItem lvsi;
              
                this.lblCurrentPath.Text = root + "    (Double click to display the path name)";

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(root);

                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();

                this.lwFilesAndFolders.Items.Clear();

                this.lwFilesAndFolders.BeginUpdate();

                foreach (System.IO.FileInfo fi in files)
                {
                    lvi = new ListViewItem();
                    lvi.Text = fi.Name;
                    lvi.ImageIndex = 1;
                    lvi.Tag = fi.FullName;

                    lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = fi.Length.ToString();
                    lvi.SubItems.Add(lvsi);

                    lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = fi.LastAccessTime.ToString();
                    lvi.SubItems.Add(lvsi);

                    this.lwFilesAndFolders.Items.Add(lvi);
                }
        

                this.lwFilesAndFolders.EndUpdate();
            }
            catch (System.Exception err)
            {
                MessageBox.Show("Error: " + err.Message);
            }

            this.lwFilesAndFolders.View = View.Details;
            this.lwFilesAndFolders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.lwFilesAndFolders.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.lwFilesAndFolders.Refresh();
        }

        private void lwFilesAndFolders_ItemActivate(object sender, System.EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            System.Windows.Forms.ListView lw = (System.Windows.Forms.ListView)sender;

            try
            {
                string filename = lw.SelectedItems[0].Tag.ToString();
                label1.Text = Path.GetFileName(filename);
                FileInfo info = new FileInfo(filename);
                if (info.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                { richTextBox1.Text = DataSetUtilities.DataTableToCSV(FileUtilities.GetFileVersionInfo(info)).ToString(); }
                else
                {
                    richTextBox1.Font = new Font("Comic Sans MS", 10, FontStyle.Bold);
                    
                    DataTable table = FileUtilities.FileInfoAsDataSet( new List<FileInfo> { info }, true).Tables[0];
               
                    foreach (DataColumn dc in table.Columns)
                    {
                       string value = string.Format("{0}", dc.Table.Rows[0][dc.ColumnName]);
                        if(!string.IsNullOrEmpty(value))
                            richTextBox1.Text += string.Format("{0}: {1}", dc.ColumnName, dc.Table.Rows[0][dc.ColumnName]) + Environment.NewLine;
                    }
                    try
                    {
                        var ico = ImageUtilities.GetIconFromFile(info);
                        if (ico != null)
                        {
                            Clipboard.SetImage(ico);
                            richTextBox1.Text = richTextBox1.Text.Insert(0, Environment.NewLine);
                            this.richTextBox1.Paste();
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception)
            {
            }          
        }
    }
}
