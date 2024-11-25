
using Kolibri.Common.FormUtilities;
using Kolibri.Common.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.FormUtilities.Forms
{
    public partial class RenameFoldersForm : Form
    {

        public RenameFoldersForm()
        {
            InitializeComponent();
            this.Text = "Rename folders";
            linkLabelSource.Tag = new DirectoryInfo(Environment.CurrentDirectory);
            linkLabelSource.Text = (linkLabelSource.Tag as DirectoryInfo).FullName;
            buttonExecute.Enabled = false;
        }

        private void buttonSource_Click(object sender, EventArgs e)
        {
            DirectoryInfo directoryInfo;
            buttonExecute.Enabled = false;
            directoryInfo = FolderUtilities.LetOppMappe((linkLabelSource.Tag as DirectoryInfo).FullName);

            if (directoryInfo != null)
            {

                if (directoryInfo.Exists)
                {
                    linkLabelSource.Tag = directoryInfo;
                    linkLabelSource.Text = directoryInfo.FullName;

                    buttonExecute.Enabled = true;
                }
            }
            else
            {
                return;
            }
        }

        private void linkLabelSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Kolibri.Common.Utilities.FileUtilities.Start(linkLabelSource.Tag as DirectoryInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = null;
            try
            {
                var test = $"{textBoxNew.Text}{textBoxOld.Text}";
                if (string.IsNullOrEmpty(test)) { throw new Exception($"Kan ikke bytte ut tom tekst: {test}"); }
                else if (string.IsNullOrEmpty(textBoxOld.Text))
                {
                    throw new Exception("Kan ikke bytte ut tom tekst");
                }
                //else if (string.IsNullOrEmpty(textBoxNew.Text))
                //{
                //    throw new Exception("Kan ikke bytte ut tom tekst");
                //}


                di = linkLabelSource.Tag as DirectoryInfo;
                var list = di.GetDirectories($"*{textBoxOld.Text}*", SearchOption.AllDirectories);
                if (list != null && list.Count() >= 1)
                {
                    foreach (var item in list)
                    {
                        var oldName = item.FullName;
                        var folderName = item.Name;

                        var newName = oldName.Substring(0, oldName.LastIndexOf('\\') + 1);
                        newName = newName + folderName.Replace(textBoxOld.Text, textBoxNew.Text);
                        if (!newName.Equals(oldName))
                        {
                            if (!Common.Utilities.FolderUtilities.RenameDir(oldName, newName))
                            {
                                throw new Exception($"Aborting, {oldName} --> {newName}");
                            }
                        }
                    }
                    Common.Utilities.FileUtilities.Start(di);
                }
                else { throw new Exception("Kan ikke utføre operasjon, ingen mapper funnet"); }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo di = linkLabelSource.Tag as DirectoryInfo;
                DirectoryInfo[] list = di.GetDirectories($"*{textBoxOld.Text}*", SearchOption.AllDirectories);
                DataTable table = new DataView(GetRenameValues(list)).ToTable(false, "Name", "NewName");
                if (table.DataSet == null)
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(table);
                }
                Visualizers.VisualizeDataSet($"{di.FullName} - Preview", table.DataSet, this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }
        }

        private DataTable GetRenameValues(DirectoryInfo[] list)
        {
            DataTable orgTable = DataSetUtilities.AutoGenererDataSet(list.ToList()).Tables[0];

            orgTable.Columns.Add("NewName");
            List<string> names = DataSetUtilities.ColumnNames(orgTable).ToList();
            names.Remove("NewName");
            names.Insert(0, "NewName");
            DataTable ret = new DataView(orgTable, "", "", DataViewRowState.CurrentRows).ToTable(false, names.ToArray());
            if (ret.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(ret);
            }

            foreach (DataRow row in ret.Rows)
            {
                try
                {
                    if (row["name"].ToString().Contains(textBoxOld.Text))
                    {
                        {
                            var text = row["name"].ToString().Replace(textBoxOld.Text, textBoxNew.Text);
                            if (text != row["name"].ToString())
                                row["newname"] = text;
                        }
                    }
                }
                catch (Exception)
                { }
            }

            return ret;
        }
    }
}
