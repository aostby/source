using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Utilities;
using OMDbApiNet.Model;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Kolibri.net.SilverScreen.Controls
{
    public class DataGrivViewControls
    {
        private LiteDBController _LITEDB;

        public Item CurrentItem { get; internal set; }

        public DataGrivViewControls(LiteDBController contr)
        {
            _LITEDB = contr;
        }

        public Form GetMovieItemDataGridViewAsForm(DataTable table)
        {
            var view = GetMovieItemDataGridView(table);
            Form form = new Form();
            form.TopLevel = false;
            form.AutoScroll = true;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            form.Controls.Add(view);
            view.Dock = DockStyle.Fill;
            return form;
        }
        public DataGridView GetMovieItemDataGridView(DataTable tableItem)
        {
            DataGridView ret = null;
            try
            {
                List<string> visibleColumns = new List<string>(){ "Title"
                    ,"ImdbRating"
                    ,"Year"
            ,"Rated"
            ,"Runtime"
            ,"Genre"
            ,"Plot" };



                DataGridView dgv = new DataGridView();
                dgv.DataSource = tableItem;
                refresh(dgv, tableItem);
                dgv.Dock = DockStyle.Fill;

                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => col.Visible = false);
                // dataGridView1.Rows.OfType<DataGridViewRow>().ToList().ForEach(row => { if (!row.IsNewRow) row.Visible = false; });
                dgv.Columns.OfType<DataGridViewColumn>().ToList().ForEach(col => { if (visibleColumns.Contains(col.Name)) col.Visible = true; });
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AllowUserToResizeColumns = true;
                dgv.SelectionChanged += new EventHandler(DataGridView_SelectionChanged);
                dgv.CellContentDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellContentDoubleClick);
                dgv.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(DataGridView_RowPrePaint);
                dgv.KeyDown += Dgv_KeyDown;
                dgv.CellValueChanged += Dgv_CellValueChanged;
                dgv.RowPrePaint += Dgv_RowPrePaint;

                try
                {
                    dgv.Columns["Title"].DisplayIndex = 0; // or 1, 2, 3 etc dersom dgv ikke er added to panel 2 funker ikke dette 
                    dgv.Columns["Title"].Width = 150;
                    dgv.Columns["ImdbRating"].DisplayIndex = 1; // or 1, 2, 3 etc

                    dgv.Sort(dgv.Columns["ImdbRating"], ListSortDirection.Descending);
                    DataGridViewColumn lastVisibleColumn = dgv.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                    lastVisibleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                }
                catch (Exception) { }
                ret = dgv;           }
            catch (Exception)
            { }

            return ret;
        }
        public static void refresh(DataGridView view, DataTable table)
        {
            bool origAuto = view.AutoGenerateColumns;
            view.AutoGenerateColumns = true;
            view.DataSource = table;
            view.BindingContext = new BindingContext();
            view.AutoGenerateColumns = origAuto;
        }

        private void Dgv_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            { }
        }

        private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                var columnName = dgv.Columns[e.ColumnIndex].Name;
                if (columnName == "ImdbRating")
                {
                    var newValue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    var old = (dgv.Rows[dgv.CurrentCell.RowIndex].DataBoundItem as DataRowView)[e.ColumnIndex];//dgv.CurrentCell.Value;  
                    if (newValue != old)
                    {

                        var id = dgv.Rows[e.RowIndex].Cells["ImdbId"].Value;
                        var movie = _LITEDB.FindItem(id.ToString());
                        movie.ImdbRating = newValue.ToString().Replace(",", ".");
                        _LITEDB.Upsert(movie);

                    }
                }
            }
            catch (Exception) { }
        }


        private void Dgv_KeyDown(object sender, KeyEventArgs e)
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
                            var fileitem = _LITEDB.FindFile(id);
                            if (fileitem != null)
                            {
                                _LITEDB.DeleteItem(id);

                                FileInfo info = new FileInfo(fileitem.FullName);
                                if (info.Exists)
                                {
                                    try
                                    {
                                        FolderUtilities.DeleteDirectory(info.FullName);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, ex.GetType().Name);
                                    }
                                    info.Refresh();
                                    if (info.Exists)
                                    {
                                        info.Delete();
                                    }
                                }
                            }
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

        private void DataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals("del")) { }// == Keys.Delete) {

            //Then find which item/ row to delete by getting the SelectedRows property if your
            //    DataGridView is on FullRowSelect or RowHeaderSelect mode, else you can determine the row with something like this:

            //i = SelectedCells[0].RowIndex

        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        { 
            string title = null; int year = 0; int index = 0;
            try
            {
                DataGridView DataGridView1 = sender as DataGridView;

                if (DataGridView1.SelectedRows.Count > 0)
                    index = DataGridView1.SelectedRows[0].Index;
                else
                    try
                    {
                        index = DataGridView1.CurrentRow.Index;

                    }
                    catch (Exception) { index = 0; DataGridView1.ClearSelection(); }
                CurrentItem = _LITEDB.FindItem(DataGridView1.Rows[index].Cells["ImdbId"].Value.ToString());
                if (CurrentItem != null) return;
                if (DataGridView1.Rows[index].Cells["Title"].Value
                          != null)
                {
                    if (DataGridView1.Rows[index].
                        Cells["Title"].Value.ToString().Length != 0)
                    {
                        title = DataGridView1.Rows[index].Cells["Title"].Value.ToString();

                        year = int.Parse(DataGridView1.Rows[index].Cells["Year"].Value.ToString());

                        CurrentItem = _LITEDB.FindItemByTitle(title, year);
                    }
                }

                if (CurrentItem == null)
                {
                    CurrentItem = new OMDbApiNet.Model.Item() { Title = title, Year = $"{year}", ImdbRating = "unknown" };
                }
            }
            catch (Exception ex) { }
        }

        private void DataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

            if (e.State != DataGridViewElementStates.Visible) return;
            try
            {
                DataGridView dgv = sender as DataGridView;

                string imdbid = $"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";

                if (string.IsNullOrEmpty(imdbid))
                {
                    if (dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor != System.Drawing.Color.Beige)
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Beige;
                    }
                }
                else
                {
                    var info = _LITEDB.FindFile(imdbid);
                    if (info != null)
                    {
                        string ext = Path.GetExtension(info.FullName).ToLower();
                        FileInfo srtFile = new FileInfo(info.FullName.Replace(ext, ".srt"));
                        DirectoryInfo dInfo = new DirectoryInfo(Path.Combine(srtFile.Directory.FullName, "Subs"));
                        if (!srtFile.Exists && !dInfo.Exists)
                        {
                            var searchStr = "*.srt|*.sub";
                            var test = FileUtilities.GetFiles(srtFile.Directory, searchStr, true);
                            if (test.Length >= 0)
                            {
                                foreach (var item in test)
                                {
                                    var jall = test.Where(f => Path.GetFileNameWithoutExtension(item.FullName).Equals(Path.GetFileNameWithoutExtension(info.FullName)));
                                    if (jall != null && jall.Count() >= 0 && jall.FirstOrDefault() != null)
                                    {
                                        srtFile = jall.FirstOrDefault();
                                        break;
                                    }
                                }
                            }

                        }
                        if (!srtFile.Exists && !dInfo.Exists)
                        {
                            dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Yellow;

                        }
                        else
                            if (srtFile.Length <= 1) { dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Red; }
                        else
                        {
                            if (dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor != System.Drawing.Color.White)
                            {
                                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                            }
                        }

                    }

                }
                try
                {
                    if ($"{dgv.Rows[e.RowIndex - 1].Cells["ImdbId"].Value}".Equals($"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}"))
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Salmon;
                    }
                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception)
            { }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                DataGridView dgv = sender as DataGridView;

                string imdbid = $"{dgv.Rows[e.RowIndex].Cells["ImdbId"].Value}";


                if (!string.IsNullOrEmpty(imdbid))
                {
                    var info = _LITEDB.FindFile(imdbid);
                    if (info != null)
                    {
                        FileInfo file = new FileInfo(info.FullName);
                        FileUtilities.OpenFolderMarkFile(file);
                    }
                }
                else
                {

                    try
                    {
                        string path = Path.GetDirectoryName(dgv.Rows[e.RowIndex].Cells["TomatoUrl"].Value.ToString());
                        System.Diagnostics.Process.Start(path);

                    }
                    catch (Exception)
                    {
                    }


                }
            }
            catch (Exception) { }
        }
    }
}