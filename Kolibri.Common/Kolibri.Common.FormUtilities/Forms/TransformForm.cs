using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Controller;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class TransformForm : Form
    {
        /// <summary>
        /// The current users choosen default path for projects etc.
        /// </summary>
        private DirectoryInfo m_defaultpath;
        /// <summary>
        /// Applications roaming temp folder
        /// </summary>
        private DirectoryInfo m_tempPath;
        /// <summary>
        /// Default extension for files to work with
        /// </summary>
        private string m_sourceExtension = "xsl";

        private Dictionary<string, FileInfo> m_paths;
        [Obsolete("Kun for GUI ved arv", true)]
        public TransformForm() { }
        public TransformForm(DirectoryInfo path, string sourceExtension)
        {
            m_tempPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestBench", "TransformForm"));

            m_paths = new Dictionary<string, FileInfo>();
            m_sourceExtension = sourceExtension.ToUpper().Replace(".", string.Empty);
            m_defaultpath = path;
            if (!m_defaultpath.Exists)
                m_defaultpath = new DirectoryInfo(@"C:\Source\BizTalk\Main2013");
            InitializeComponent();
            InitGrid(m_defaultpath);
          Controller.TransFormController.UnpackXSLT();

        }

        private void InitGrid(DirectoryInfo path)
        {

            m_defaultpath = path;
            dataGridView1.DataSource = null;
            try
            {
                FileInfo[] infos = path.GetFiles(string.Format("*.{0}", m_sourceExtension), SearchOption.AllDirectories);
                DataTable table = DataSetUtilities.ConvertToDataTable(infos.ToList());
                dataGridView1.DataSource = table;

                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                dataGridView1.AllowUserToOrderColumns = true;
                dataGridView1.AllowUserToResizeColumns = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;


                string searchString = "TDocFlatFile_To_HovedboksBilag.xsl";
                SetDataGridSelectedItem(searchString);

            }
            catch (Exception)
            { }
            SetLabelPaths();
        }

        private void SetDataGridSelectedItem(string searchString)
        {
            try
            {
                int visibleColumnIndex = dataGridView1.Columns["Name"].Index;
                int rowIndex = -1;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(searchString))
                    {
                        rowIndex = row.Index;
                        m_paths["xslFile"] = new FileInfo(((DataRowView)dataGridView1.Rows[rowIndex].DataBoundItem).Row["FullName"].ToString()); ;
                        break;
                    }
                }
                if (rowIndex >= 0)
                {
                    dataGridView1.CurrentCell = dataGridView1[visibleColumnIndex, rowIndex];
                }
            }
            catch (Exception ex) { }
        }
        private FileInfo GetXSLT(FileInfo btmFile)
        {
            string destFolderName = "XSLT";
            FileInfo ret = null;

            string ext = btmFile.Extension.TrimStart('.').ToUpper();
            ret = new FileInfo(Path.Combine(m_tempPath.FullName, destFolderName, string.Format("transform{0}.xslt", ext.ToUpper())));//"BizTalkMapDocumenterHTML.xslt"));
            if (!ret.Directory.Exists) ret.Directory.Create();
            switch (ext)
            {
                case "CONFIG": File.WriteAllText(ret.FullName, ResourceController.GetResouceObject("treeview") as string); break;
                case "XML": Controller.TransFormController.UnpackXSLT(); ret = null; break;
                case "XSD": File.WriteAllText(ret.FullName, ResourceController.GetResouceObject("xs3p") as string); break;                
                case "BTM": File.WriteAllText(ret.FullName, ResourceController.GetResouceObject("BizTalkMapDocumenterHTML") as string); break;
                case "ODX":
                case "BTP":
                    if (m_tempPath != null && m_tempPath.Exists)
                    {
                        try
                        {
                            FileInfo info = null ;
                            if(ext.Equals("ODX"))
                            info = m_tempPath.GetFiles("OrchestrationCode.xslt", SearchOption.AllDirectories)[0];
                            if(ext.Equals("BTP"))
                                info = m_tempPath.GetFiles("PipelineDisplay.xslt", SearchOption.AllDirectories)[0];
                            if (info.Exists)
                                ret = info;
                            else ret = null;
                        }
                        catch (Exception)
                        { ret = null; break; }
                    }
                    break;

                default: ret = null; break;
            }
            return ret;
        }

        private void ShowFiles(FileInfo xmlFile)
        {
            FileInfo xslFile = GetXSLT(xmlFile);
            if (xslFile != null)
            {
                m_paths["xslFile"] = xslFile;
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = xmlFile.Name + " - Choose file to use for transformation";
                ofd.Filter = "Transform|*.xsl; *.xslt;|" + FileUtilities.GetFileDialogFilter(new string[] { "xsl", "xslt" });
                ofd.DefaultExt = ".xsl";
                #region set initial directory
                try
                {
                    if (xmlFile.Extension.EndsWith("XML", StringComparison.InvariantCultureIgnoreCase) && xslFile != null && xslFile.Exists)
                    {
                        ofd.FileName = xslFile.Name;
                        ofd.InitialDirectory = xslFile.Directory.FullName;
                    }
                    else
                    {
                        ofd.FileName = m_paths["xslFile"].Name;
                        ofd.InitialDirectory = m_paths["xslFile"].Directory.FullName;
                    }
                }
                catch (Exception)
                {
                    FileInfo info = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", "biztalk2013documenter", "biztalk2013documenter.zip"));
                    if (info.Exists) ofd.InitialDirectory = info.Directory.FullName;
                }
                #endregion
                ofd.ShowHelp = true;

                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                else
                    m_paths["xslFile"] = new FileInfo(ofd.FileName);
            }

            if (m_paths["xslFile"] != null)
            {
                m_paths["xmlFile"] = xmlFile;
                SetLabelPaths();
                //  ShowFiles(m_paths["xmlFile"], m_paths["xslFile"]);  
                Controller.TransFormController.ShowFiles(m_paths, m_paths["xmlFile"], m_paths["xslFile"]);
            }
        }
     
        private void SetLabelPaths()
        {
            textBox1.Text = m_defaultpath.FullName;
            label1.Text = string.Empty;
            labelExtension.Text = string.Empty;
            try
            {
                foreach (string productKey in m_paths.Keys)
                {
                    label1.Text += string.Format("{0}: {1}{2}", productKey, m_paths[productKey], Environment.NewLine);
                }
                labelExtension.Text = string.Format("Extension: {0}", m_sourceExtension);
            }
            catch (Exception)
            { }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                DataRow row = (dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row;
                FileInfo info = new FileInfo(row["FullName"].ToString());
                Process.Start(info.FullName);
                //   ShowFiles(info);
            }
            catch (Exception)
            { }
        }

        private void buttonPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (Directory.Exists(textBox1.Text))
                dlg.SelectedPath = textBox1.Text;
            if (File.Exists(textBox1.Text))
            {
                dlg.SelectedPath = Path.GetDirectoryName(textBox1.Text);
                SendKeys.Send("{TAB}{TAB}{RIGHT}");  // <<-- Workaround - SendKeys
            }

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                InitGrid(new DirectoryInfo(dlg.SelectedPath));
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                try
                {
                    DataRow row = (dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row;
                    FileInfo info = new FileInfo(row["FullName"].ToString());
                    FileUtilities.ShowOpenWithDialog(info.FullName);
                }
                catch (Exception)
                { }
            }
        }
        private void buttonTransform_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = (dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row;
                ShowFiles(new FileInfo(row["FullName"].ToString()));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.InnerException.Message);
                MessageBox.Show(msg);
            }
        }

        private void buttonXSLT_Click(object sender, EventArgs e)
        {
            try
            {
                object obj = m_sourceExtension.Equals("ODX", StringComparison.InvariantCultureIgnoreCase) ? "ODXViewer_02" : null;
                var liste = new List<FileInfo>(m_tempPath.GetFiles("*.xsl*", SearchOption.AllDirectories));
                DataTable table = FileUtilities.FileInfoAsDataSet(liste, false).Tables[0];

                if (FormUtilities.InputDialogs.ChooseListBox("Set a xslt sheet", "Choose a transformation defult", table, ref obj)
                    == DialogResult.OK)
                {
                    ListViewItem selItem = ((obj) as ListViewItem);
                    m_paths["xslFile"] = liste.FirstOrDefault(s => s.Name == selItem.Text);
                    SetLabelPaths();
                }
            }
            catch (Exception)
            {  }
        }

        private void buttonPasteText_Click(object sender, EventArgs e)
        {
            string text = string.Empty;

            try
            {
                if (FormUtilities.InputDialogs.InputRichTextBox(m_sourceExtension.ToUpper() + " - Paste and transform", "Paste text to be transformed here", ref text) == System.Windows.Forms.DialogResult.OK)
                {
                    FileInfo info = FileUtilities.GetTempFile(m_sourceExtension);
                    FileUtilities.WriteStringToFile(text, info.FullName);

                    try
                    {
                        if (!m_paths.ContainsKey("xslFile") && m_sourceExtension.EndsWith("XML", StringComparison.InvariantCultureIgnoreCase))
                            m_paths["xslFile"] = m_tempPath.GetFiles("GenericHTML*", SearchOption.AllDirectories)[0];
                    }
                    catch (Exception) { }
                    ShowFiles(info);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

