using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;

namespace Kolibri.Common.Utilities.Forms
{
    public partial class RenameFilesForm : Form
    {
        string sourcePath = Environment.CurrentDirectory; //Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string destPath = Environment.CurrentDirectory;

        public RenameFilesForm()
        {
            InitializeComponent();
            Init();
        }

        public RenameFilesForm(DirectoryInfo source, DirectoryInfo destination)
        {
            InitializeComponent();            

            sourcePath = source.FullName;
            destPath = destination.FullName;

            Init();
        }

        public DirectoryInfo SourcePath { get { return new DirectoryInfo(textBoxSource.Text); } set { textBoxSource.Text = value.FullName; } }
        public DirectoryInfo DestinationPath { get { return new DirectoryInfo(textBoxDestination.Text); } set { textBoxDestination.Text = value.FullName; } }
        /// <summary>
        /// Ved å angi formtype finner en rett filsti som ble brukt sist
        /// </summary>
        /// <param name="formType">MOV, PIC, TV </param>



        internal void Init()
        {
            textBoxRename_TextChanged(textBoxFra, null);

            if (!Directory.Exists(sourcePath))
                sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!Directory.Exists(destPath))
                destPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

            SetSource(sourcePath);
            SetDestination(destPath);

            this.StartPosition = FormStartPosition.CenterParent;
            //Lets make it nasty (some forms aren't rendered properly otherwise)
            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
            #region foldericon
            try
            {
                // You're looking for a 16x16 opened folder icon : Icon icon16 = IconUtils.GetShellIcon(SHSTOCKICONID.SIID_FOLDEROPEN, SHGSI.SHGSI_ICON Or SHGSI.SHGSI_SMALLICON);
                // You're looking for a 32x32 default state folder icon :Icon icon32 = IconUtils.GetShellIcon(SHSTOCKICONID.SIID_FOLDER, SHGSI.SHGSI_ICON Or SHGSI.SHGSI_LARGEICON);
                Bitmap image = Icons.GetSystemIcon(Icons.SHSTOCKICONID.SIID_FOLDEROPEN, Icons.SHGSI.SHGSI_ICON | Icons.SHGSI.SHGSI_SMALLICON).ToBitmap();

                if (image != null)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    buttonOpenDirS.Text = string.Empty;
                    buttonOpenDirD.Text = string.Empty;

                    buttonOpenDirS.BackgroundImage = image;
                    buttonOpenDirS.BackgroundImageLayout = ImageLayout.Stretch;
                    buttonOpenDirD.BackgroundImage = image;
                    buttonOpenDirD.BackgroundImageLayout = ImageLayout.Stretch;
                }
                #endregion 
            }
            catch (Exception) { }
        }

        private void button_Click(object sender, EventArgs e)
        {
            string text = null;
            if (sender.Equals(buttonSource))
                text = textBoxSource.Text;
            else
                text = textBoxDestination.Text;

            DirectoryInfo folder = null;
            {
                folder = FolderUtilities.LetOppMappe(text);

            }
            if (folder != null && folder.Exists)
            {
                if (sender.Equals(buttonSource))
                {
                    textBoxSource.Text = folder.FullName.ToString();
                    //if (string.IsNullOrWhiteSpace(textBoxDestination.Text))
                    //{
                    textBoxDestination.Text = folder.FullName.ToString();// new DirectoryInfo(textBoxSource.Text).Parent.FullName;
                                                                         //}
                }
                else
                {
                    textBoxDestination.Text = folder.FullName.ToString();
                }
            }

        }

        private void buttonOpenDir_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox textBox = null;
                if (sender.Equals(buttonOpenDirS)) { textBox = textBoxSource; }
                else if (sender.Equals(buttonOpenDirD)) { textBox = textBoxDestination; }

                Process.Start(textBox.Text);
            }
            catch (Exception ex)
            {
                string jalla = ex.Message;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var box = sender as TextBox;
                DirectoryInfo info = new DirectoryInfo(box.Text);
                if (info.Exists)
                    box.BackColor = Color.White;
                else
                    box.BackColor = Color.LightSalmon;

                if (textBoxSource.Text.Equals(textBoxDestination.Text))
                    textBoxDestination.BackColor = Color.LightYellow;

                if (box == textBoxSource)
                    SetSource(textBoxSource.Text);
                if (box == textBoxDestination)
                    SetDestination(textBoxDestination.Text);


            }
            catch (Exception)
            { }

        }

        public void SetSource(string sourcePath)
        {

            if (Directory.Exists(sourcePath))
            {
                textBoxSource.Text = sourcePath;
            }
        }
        public void SetDestination(string destPath)
        {
            if (Directory.Exists(destPath))
            {
                textBoxDestination.Text = destPath;
            }
        }

        private void textBoxRename_TextChanged(object sender, EventArgs e)
        {
            buttonExecute.Enabled = !(textBoxFra.Text == string.Empty);
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {

            try
            {
                FileInfo[] list = GetListOfFiles();
                DataTable table = new DataView( GetRenameValues(list)).ToTable(false, "NewName", "Name", "Extension");
                ShowGridView(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }


        }

        private FileInfo[] GetListOfFiles()
        {
            List<string> common = FileUtilities.MoviesCommonFileExt();
            var searchStr = "*." + string.Join("|*.", common);
            var list = FileUtilities.GetFiles(new DirectoryInfo(textBoxSource.Text), searchStr, true);
            if (list.Count() == 0)
            {
                common = new List<string>() { "srt", "csproj" };
                searchStr = "*." + string.Join("|*.", common);
                list = FileUtilities.GetFiles(new DirectoryInfo(textBoxSource.Text), searchStr, true);
            }
            return list;
        }

        private DataTable GetRenameValues(FileInfo[] list)
        {
            DataTable orgTable = DataSetUtilities.AutoGenererDataSet(list.ToList()).Tables[0];

            orgTable.Columns.Add("NewName");
            List<string> names = DataSetUtilities.ColumnNames(orgTable).ToList();
            names.Remove("NewName");
            names.Insert(0, "NewName");
            DataTable ret = new DataView(orgTable, "", "", DataViewRowState.CurrentRows).ToTable(false, names.ToArray());

            if (textBoxFra.Text.Length > 0)
            {
                foreach (DataRow row in ret.Rows)
                {
                    try
                    {
                        if (row["name"].ToString().Contains(textBoxFra.Text))
                        {
                            {
                                var text = row["name"].ToString().Replace(textBoxFra.Text, textBoxTil.Text);
                                if (text != row["name"].ToString())
                                    row["newname"] = text;
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            return ret;
        }

        private void ShowGridView(DataTable table)
        {
            try
            {
                if (panel1.Controls.Count > 0)
                    panel1.Controls.Clear();

                DataGridView dgv = new DataGridView();
                dgv.DataSource = table;
                dgv.Dock = DockStyle.Fill;
                panel1.Controls.Add(dgv);

                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AllowUserToResizeColumns = true;
                dgv.SelectionChanged += new EventHandler(DataGridView_SelectionChanged);
                dgv.CellContentDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellContentDoubleClick);

                try
                {
                    List<string> fixedLength = new List<string>() { "Title","NewName","Name"};
                    foreach (string fix in fixedLength)
                    { if (dgv.Columns.Contains(fix))
                                dgv.Columns[fix].Width = 450;
                    }
                        
                    DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                    lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                catch (Exception) { }

            }
            catch (Exception ex)
            { }
        }
        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {

            string title = null; int year = 0; int index = 0;
            string titleColumn = "Title";

            try
            {
                DataGridView DataGridView1 = sender as DataGridView;

                if (DataGridView1.SelectedRows.Count > 0)
                    index = DataGridView1.SelectedRows[0].Index;
                else
                    try
                    { index = DataGridView1.CurrentRow.Index; }
                    catch (Exception) { }

                if (!DataGridView1.Columns.Contains(titleColumn))
                    titleColumn = "Name";

                if (DataGridView1.Rows[index].Cells[titleColumn].Value
                          != null)
                {
                    if (DataGridView1.Rows[index].
                        Cells[titleColumn].Value.ToString().Length != 0)
                    {
                        title = DataGridView1.Rows[index].Cells[titleColumn].Value.ToString();
                        if (DataGridView1.Columns.Contains("Year"))
                            year = int.Parse(DataGridView1.Rows[index].Cells["Year"].Value.ToString());
                        else if (DataGridView1.Columns.Contains("Released"))
                        {
                            DateTime dt = new DateTime();
                            if (DateTime.TryParse(DataGridView1.Rows[index].Cells["Released"].Value.ToString(), out dt))
                                year = dt.Year;
                            else
                            {
                                if (DataGridView1.Columns.Contains("ImdbId"))
                                {
                                    if (string.IsNullOrEmpty(DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString()))
                                    {

                                    }
                                }
                            }
                        }
                        else if (DataGridView1.Columns.Contains("AirDate"))
                            year = DateTime.Parse(DataGridView1.Rows[index].Cells["AirDate"].Value.ToString()).Year;
                    }
                }

                try
                {
                    string seasonColumn = "Season";
                    if (!DataGridView1.Columns.Contains(seasonColumn))
                        seasonColumn = "SeasonNumber";

                    string episodeColumn = "Episode";
                    if (!DataGridView1.Columns.Contains(episodeColumn))
                        episodeColumn = "EpisodeNumber";


                }
                catch (Exception ex)
                {
                }

                if (!DataGridView1.Columns.Contains("ImdbId"))
                {


                    return;
                }

                var imdbId = DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString();

                var name = DataGridView1.Rows[index].Cells["Name"].Value.ToString();



            }

            catch (Exception ex)
            {

            }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridView dgv = sender as DataGridView;


        }
        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                try
                {
                    DataGridView dgv = sender as DataGridView;
                    var test = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView)[0];//dgv.CurrentCell.Value;  


                    if (MessageBox.Show($"Delete move\r\r {test}, \r\n which will include parent folder and all its content?", "Delete from both file and database?",
                                     System.Windows.Forms.MessageBoxButtons.YesNoCancel,
                                     System.Windows.Forms.MessageBoxIcon.Question,
                                     System.Windows.Forms.MessageBoxDefaultButton.Button3,
                                    MessageBoxOptions.DefaultDesktopOnly) == DialogResult.Yes)
                    {
                        DataRow row = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView).Row;
                        string id = $"{row["ImdbId"]}";
                        if (!string.IsNullOrEmpty(id))
                        {

                        }
                        else
                        {

                            e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }

                }
                catch (Exception ex)
                {
                    e.Handled = true;
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }

            }
        } 

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Enabled = progressBar1.Visible;
            progressBar1.Value = 0;
            try
            {
                
                var list =  GetListOfFiles();
                DataTable table = GetRenameValues(list);
                table = new DataView(table, "newname <>'' ", "", DataViewRowState.CurrentRows).ToTable();
                progressBar1.Maximum = table.Rows.Count;
                foreach (DataRow row in table.Rows)
                {
               
                    FileInfo source = new FileInfo(row["FullName"].ToString());
                    FileInfo dest = new FileInfo(Path.Combine(source.Directory.FullName, row["NewName"].ToString()));
                    if (dest.Name != row["Name"].ToString())
                    {
                         //FileUtilities.MoveCopy(source, dest);
                        File.Move(source.FullName, dest.FullName);
                        progressBar1.Increment(1);
                    }

                    if (File.Exists(source.FullName.Replace(source.Extension, ".srt")))
                        {
                        string s = source.FullName.Replace(source.Extension, ".srt");
                        string d = dest.FullName.Replace(source.Extension, ".srt");
                        File.Move(s, d);
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }
            progressBar1.Enabled = false;
      
        }

        private void buttonCompare_Click(object sender, EventArgs e)
        {
            var source = new DirectoryInfo(textBoxSource.Text);
            var destination = new DirectoryInfo(textBoxDestination.Text);   

            var res = FolderUtilities.CompareFolders(source, destination);

            if (panel1.Controls.Count > 0)
                panel1.Controls.Clear();
            RichTextBox rtb = new RichTextBox(); ;
            rtb.Text = res.ToString();  
            rtb.Dock = DockStyle.Fill;
            panel1.Controls.Add(rtb);
                }
    }
}
