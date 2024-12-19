using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
 
using System.Drawing;
using System.Xml;
using FastColoredTextBoxNS;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
 
using System.Reflection;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Controller;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.FormUtilities.Forms;

namespace Kolibri.net.Common.FormUtilities
{
    public class Visualizers
    {
        /// <summary>
        /// button event som kan utvides for alle Vizializers
        /// </summary>  
        private static void buttonEvent_Clicked(object sender, EventArgs args)
        {
            Button button = sender as Button;
            if (button == null) return;
            try
            {
                #region buttonSearch
                if (!string.IsNullOrEmpty(button.Name) && button.Name.Equals("buttonSearch"))
                {
                    Form form = button.FindForm();
                    var list = form.Controls.Find("dgv", true);
                    foreach (var contr in list)
                    {
                        var text = "searchText";
                        if (InputDialogs.InputBox("Search for value", "Lookup value in " + form.Text, ref text) != DialogResult.OK)
                            return;
                        DataSet result = new DataSet("searchResult");
                        DataTable merged = null;
                        StringBuilder builder = new StringBuilder();
                        if (contr.GetType().Equals(typeof(DataGridView)))
                        {
                            DataSet ds = (contr as DataGridView).DataSource as DataSet;
                            foreach (DataTable dataTable in ds.Tables)
                            {
                                var test = dataTable
                                    .Rows
                                    .Cast<DataRow>()
                                    .Where(r => r.ItemArray.Any(
                                        c => c.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) > 0
                                    )).ToArray();
                                if (test != null && test.Length > 0)
                                {
                                    DataTable table = dataTable.Clone();
                                    test.ToList().ForEach(row => table.ImportRow(row));
                                    table.Columns.Add(new DataColumn("TableName") { DefaultValue = dataTable.TableName });
                                    builder.AppendLine(table.TableName);
                                    builder.Append(DataSetUtilities.DataTableToCSV(table));
                                    result.Tables.Add(table);

                                    try
                                    //forutsetter jo at alle tabellene i DS er like, da men....
                                    {
                                        if (merged == null)
                                            merged = table.Clone();
                                        merged.Merge(table);
                                    }
                                    catch (Exception)
                                    { }
                                }
                            }
                        }
                        //OutputDialogs.ShowRichTextBoxDialog(form.Name, builder.ToString(), new Size(800, 600));
                        if (merged != null)
                            OutputDialogs.ShowDataTableDialog(form.Name, merged, form.Size);
                        else
                            VisualizeDataSet(form.Name, result, form.Size);
                    }
                }
                #endregion // button search
            }
            catch (Exception)
            { }
        }

        private static void ShowDataTableDialog_SelectionChanged(object sender, EventArgs args)
        {
            DataGridView temp = sender as DataGridView;
            if (temp == null) return;
            if (!temp.Visible) return;

            else
            {
                try
                {
                    DataRowView currentDataRowView = (DataRowView)temp.CurrentRow.DataBoundItem;
                    DataRow row = currentDataRowView.Row;


                    if (temp.Tag!=null&& row[temp.Tag.ToString()].GetType() == typeof(Bitmap))
                    {
                        SplitterPanel split = temp.Parent as SplitterPanel;
                        (((split.Parent as SplitContainer).Panel2.Controls[0]) as PictureBox).Image = row[temp.Tag.ToString()] as Bitmap;
                    }
                    if (temp.Tag != null && row[temp.Tag.ToString()].GetType() == typeof(XmlDocument))
                    {
                        SplitterPanel split = temp.Parent as SplitterPanel;
                        (((split.Parent as SplitContainer).Panel2.Controls[0]) as FastColoredTextBox).Text =
              Utilities.XMLUtilities.        PrettyPrintXML((row[temp.Tag.ToString()] as XmlDocument).OuterXml, Encoding.UTF8);
                    }
                    else if(temp.Tag!=null)
                    {

                        string query = string.Format("{0}", row[temp.Tag.ToString()]);
                        SplitterPanel split = temp.Parent as SplitterPanel;
                        (split.Parent as SplitContainer).Panel2.Controls[0].Text = query;
                    }
                }
                catch (Exception)
                {
                }

            }
        }
        private static void ShowDataTableDialog_KeyDown(object sender, KeyEventArgs e)
        {
            DataTable table = null;
            try
            {
                if (e.KeyCode == Keys.S && (e.Control)) // || e.Control || e.Shift)) //   if (e.KeyCode == Keys.Control|| e.KeyCode > Keys.s)
                {
                    DataGridView view = sender as DataGridView;

                    if (view != null)
                    {
                        if (view.DataSource.GetType().Equals(typeof(DataTable))) ;
                        table = view.DataSource as DataTable;
                    

                        if (table == null)
                        {
                            if (view.DataSource.GetType().Equals(typeof(DataSet)))
                            {
                                if (view.DataMember == string.Empty)
                                    table = (view.DataSource as DataSet).Tables[0];
                                else

                                    table = (view.DataSource as DataSet).Tables[view.DataMember];
                            }
                        }
                        if (table.DataSet == null)
                        {
                            DataSet ds = new DataSet("DocumentElement");
                            ds.Tables.Add(table);
                        }
                    }
                }


                if (table != null)
                {/*
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = table.TableName + ".xml";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "")
                    {
                        table.DataSet.WriteXml(saveFileDialog.FileName);
                        Utilities.FileUtilities.OpenFolderHighlightFile(new FileInfo(saveFileDialog.FileName));
                    }
                    */
                    FileUtilities.ExportToFormats(table);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            } e.Handled = true;
        }
        private static void VisualizeDataSet_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox box = sender as ComboBox;
                SplitterPanel split = box.Parent as SplitterPanel;
                SplitContainer splitC = box.Parent.Parent as SplitContainer;
                DataGridView dgv = splitC.Controls.Find("dgv", true)[0] as DataGridView;
                dgv.DataMember = box.Text;
            }
            catch (Exception ex)
            {
            }
        }
        private static void VizualizeDBTables_SelectionChanged(object sender, EventArgs e)
        {
            try
            {//Tekst                //        string.Format("MachineName: {0} ({2}/{3}) table: {1} ", Environment.MachineName, table.TableName, liste.IndexOf(item), liste.Count)
                try
                {
                    ComboBox box = sender as ComboBox;
                    SplitterPanel split = box.Parent as SplitterPanel;
                    SplitContainer splitC = box.Parent.Parent as SplitContainer;
                    DataGridView dgv = splitC.Controls.Find("dgv", true)[0] as DataGridView;
                    //dgv.DataMember = box.Text;
                    string dbconnectionstring = dgv.Tag.ToString();
                    DataTable table = GetData(dbconnectionstring, string.Format("Select * from {0}", box.SelectedValue));
                    table.TableName = box.SelectedValue.ToString();
                    dgv.DataSource = table;


                    if (table != null && box.Tag != null && (box.Tag.GetType().Equals(typeof(Label))))
                    {
                        (box.Tag as Label).Text = "RowCount: " + table.Rows.Count;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, ex.GetType().Name); }
        }
        public static DialogResult VizualizeDBTables(string title, string dbconnectionstring, Size size, List<string> filter=null)
        {
            Form form = new Form();
            form.Size = size;
            form.Text = title;
        //    form.Icon = Icons.IconFromImage( ResourceController.GetResouceObject("Ssms") as Bitmap);

            SplitContainer split = new SplitContainer();
            split.Name = "split";
            split.Orientation = Orientation.Horizontal;
            split.Dock = DockStyle.Fill;

            string sql = @"SELECT TABLE_NAME from (
                                SELECT * FROM information_schema.tables 
                                WHERE TABLE_TYPE='BASE TABLE'
                                AND  TABLE_TYPE != 'VIEW' 
                                AND TABLE_NAME <> 'sysdiagrams'
                            ) t";

            if (filter != null) {
                string s =string.Format("'{0}'",   string.Join("','", filter.ToArray().Select(i => i.Replace("'", "''")).ToArray()));
                sql = string.Format("Select * from ({0} ) filtered where TABLE_NAME in ({1})", sql, s);
                    }


            DataTable table = GetData(dbconnectionstring, sql);
            table.TableName = "TableName";

            var list = table.Rows.OfType<DataRow>().Select(dr => dr.Field<string>(table.Columns[0].ColumnName)).ToList();
            var tablenames = list;

            ComboBox box = new ComboBox();
            box.DataSource = tablenames;
            //box.Dock =  DockStyle.Top;
            box.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            box.Dock = DockStyle.Top;
            Label lblRowCount = new Label();
            lblRowCount.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            lblRowCount.Dock = DockStyle.Top;
            lblRowCount.Text = "Rowcount:    0";
            lblRowCount.Name = "lblRowCount";

            box.Tag = lblRowCount;
            split.Panel1.Controls.Add(lblRowCount); split.Panel1.Controls.Add(box);

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgv";
            dgv.Tag = dbconnectionstring;
            //  dgv.DataSource = ds;
            //  dgv.DataMember = ds.Tables[0].TableName;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = true;
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(dgv);


            dgv.KeyDown += new KeyEventHandler(ShowDataTableDialog_KeyDown);
            //    dgv.SelectionChanged += VizualizeDBTables_SelectionChanged;
            box.SelectedValueChanged += VizualizeDBTables_SelectionChanged;
            form.Controls.Add(split);
            form.Show(); split.SplitterDistance = box.Height * 2;


            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }

        public static DialogResult VisualizeDataSet(string title, DataTable dt, Size size)
        {
            DataSet ds = dt.DataSet;
            if (dt != null && dt.DataSet == null)
            {
                ds = new DataSet();
                ds.Tables.Add(dt);
            }
           
            return VisualizeDataSet(title, ds, size);
        }

        public static DialogResult VisualizeDataSet(string title, DataSet ds, Size size)
        {
            Form form = new Form();
            form.Size = size;
            form.Text = title;
            //Icon ico =  Icons.IconFromImage(    ResourceController.GetResouceObject("dataset") as Bitmap);
            //form.Icon = Kolibri.net.Common.Utilities.Icons.MakeTransperent(ico, form.ForeColor);

            SplitContainer split = new SplitContainer();
            split.Name = "split";
            split.Orientation = Orientation.Horizontal;
            split.Dock = DockStyle.Fill;

            List<DataTable> tables = ds.Tables.Cast<DataTable>().ToList();
            var tablenames = tables.Select(dt => dt.TableName).ToList();

            ComboBox box = new ComboBox();
            box.DataSource = tablenames;
            //box.Dock =  DockStyle.Top;
            box.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            box.Dock = DockStyle.Top;
            split.Panel1.Controls.Add(box);

            Button buttonSearch = new Button();
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Text = "Search";
            buttonSearch.Top = box.Bottom + 2;
            buttonSearch.Click += buttonEvent_Clicked;
            split.Panel1.Controls.Add(buttonSearch);
            //http://stackoverflow.com/questions/10661361/find-a-string-in-all-datatable-columns
            /*var text = "whatever";
return dataTable
    .Rows
    .Cast<DataRow>()
    .Where(r => r.ItemArray.Any(
        c => c.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) > 0
    )).ToArray();*/

            DataGridView dgv = new DataGridView();
            dgv.Name = "dgv";
            dgv.DataSource = ds;
            dgv.DataMember = ds.Tables[0].TableName;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = true;
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.Dock = DockStyle.Fill;
            split.Panel2.Controls.Add(dgv);

            form.Controls.Add(split);
            form.Show(); split.SplitterDistance = box.Height;

            dgv.KeyDown += new KeyEventHandler(ShowDataTableDialog_KeyDown);
            dgv.SelectionChanged += ShowDataTableDialog_SelectionChanged;
            box.SelectedValueChanged += VisualizeDataSet_SelectionChanged;


            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }
        public static void VisualizeTransformedXML(string title, XmlDocument doc, Size size)
        {
            FileInfo info = FileUtilities.GetTempFile("xml");
            doc.Save(info.FullName);
            XSLTTransform.TransformAndShow(info, Controller.TransFormController.GetXSLTPath("GenericHTML01"), "HTML");
        }
        public static void VisualizeTransformedXML(string title,  string xml, Size size)
        {
            XmlDocument xdocument = new XmlDocument();
            xdocument.LoadXml(xml);
            VisualizeTransformedXML(title, xdocument, size);
        }

 
        public static DialogResult VisualizeTextFiles(string title, List<FileInfo> filelist, Size size)
        {
            Form form = new Form();
            form.Size = size;
            form.Text = title;

            SplitContainer split = new SplitContainer();
            split.Name = "split";
            split.Orientation = Orientation.Horizontal;
            split.Dock = DockStyle.Fill;


            DataSet fileset = FileUtilities.FileInfoAsDataSet(filelist, false);

            ComboBox box = new ComboBox();
            box.DataSource = fileset.Tables[0];
            box.ValueMember = "FullName";
            box.DisplayMember = "Name";
            box.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            box.Dock = DockStyle.Top;
            split.Panel1.Controls.Add(box);

            form.Controls.Add(split);
            form.Show();
            split.SplitterDistance = box.Height + 2;

            box.SelectedValueChanged += VisualizeTextFiles_SelectionChanged;
            VisualizeTextFiles_SelectionChanged(box, null);
            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }
        private static void VisualizeTextFiles_SelectionChanged(object sender, EventArgs e)
        {
            ComboBox box = sender as ComboBox;
            SplitterPanel split = box.Parent as SplitterPanel;
            SplitContainer splitC = box.Parent.Parent as SplitContainer;
            splitC.Panel2.Controls.Clear();

            string text = FileUtilities.ReadTextFile(box.SelectedValue.ToString());

            FastColoredTextBoxNS.FastColoredTextBox output = new FastColoredTextBoxNS.FastColoredTextBox();
            output.Language = Language.HTML;
            output.Text = text;
            output.Dock = DockStyle.Fill;
            output.KeyDown += FCTB_KeyDown;
            splitC.Panel2.Controls.Add(output);
        }
        private static void FCTB_KeyDown(object sender, KeyEventArgs e)
        {
            FastColoredTextBox textBoxQuery = sender as FastColoredTextBox;
            #region save
       
            #endregion

            #region Kjør querey
            //if (e.KeyData == Keys.F5)
            //{
            //    buttonTestQuery_Click(null, null);
            //    e.Handled = true;
            //}
            #endregion

            #region Show Plan for querey
            //if (e.KeyData == Keys.F6)
            //{
            //    try
            //    {
            //        string text = textBoxQuery.SelectedText;
            //        if (string.IsNullOrEmpty(text))
            //            text = textBoxQuery.Text;

            //        if (string.IsNullOrEmpty(text))
            //        { e.Handled = true; }
            //        else
            //        {
            //            string sql = m_controller.LagSqlSporring(text);
            //            using (SAClient client = new SAClient(m_dbConnection.DbConnectionString))
            //            {
            //                string plan = MetaDataController.GetPlan(client, sql);
            //                if (!string.IsNullOrEmpty(plan))
            //                    FormUtilities.OutputDialogs.ShowRichTextBoxDialog("Executionplan for " + textBoxMethodName.Text, plan, this.Size);
            //                else
            //                    MessageBox.Show("No plan available at this point for " + textBoxMethodName.Text, "No plan found");
            //            }
            //            e.Handled = true;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        e.Handled = true;
            //    }
            //}
            #endregion

            #region Kjør querey - selected region
            //if (e.KeyData == Keys.F9)
            //{
            //    try
            //    {
            //        string text = textBoxQuery.SelectedText;

            //        if (string.IsNullOrEmpty(text))
            //        { e.Handled = true; }
            //        else
            //        {
            //            //   textBoxQuery.Text = textBoxQuery.Text.Replace(textBoxQuery.Selection.Text, string.Format("/*{0}*/", textBoxQuery.Selection.Text));
            //            //buttonTestQuery_Click(null, null);
            //            string sql = m_controller.LagSqlSporring(text);
            //            DataTable table = DataDiggerDAL.GetQueryData(sql, m_dbConnection).Tables[0];
            //            table.TableName = textBoxMethodName.Text;
            //            FormUtilities.OutputDialogs.ShowDataTableDialog(table.TableName, table, this.Size);
            //            e.Handled = true;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        e.Handled = true;
            //    }
            //}
            #endregion

            #region insert tablename
            //if ((e.KeyData == (Keys.Control | Keys.OemPeriod)) | (e.KeyData == Keys.F7) | (e.KeyData == (Keys.Control | Keys.Space)))
            //{
            //    //Sett inn tabellnavn
            //    if (m_CodeInsighttables == null)
            //        InitCodeInsightTables();
            //    object value = "SS001Klient";

            //    if (Kolibri.FormUtilities.InputDialogs.InputBox("Velg tabell", "Velg en tabell:", m_CodeInsighttables, "table_name", "table_name", ref value) == DialogResult.OK)
            //    {
            //        try
            //        {
            //            value = (value as DataRowView).Row["table_name"].ToString();
            //            textBoxQuery.InsertText(value.ToString().Trim(), true);
            //        }
            //        catch (Exception)
            //        {
            //        }
            //    }

            //    e.Handled = true;
            //}
            #endregion

            #region insert fieldname
            //if (e.KeyData == Keys.F4)
            //{
            //    //Sett inn tabellnavn
            //    if (m_mappingTable == null)
            //    {
            //        textBoxInformation.Text = "Backgroundworker Initializing mapping table, please be patient";
            //        System.Threading.ThreadPool.QueueUserWorkItem(delegate
            //        {
            //            UpdateMappingThread();//DoSomethingThatDoesntInvolveAControl();
            //        }, null);

            //    }
            //    if (m_mappingTable == null)
            //        return;

            //    string fieldName = textBoxQuery.SelectedText;
            //    if (string.IsNullOrEmpty(fieldName))
            //    {
            //        textBoxInformation.Text = "F4 - map to fields pressed, no text selected";
            //        return;
            //    }

            //    DataTable table = new DataView(m_mappingTable.Copy(), string.Format("BaseColumnName = '{0}' AND DataPumpNavn <> '{1}'", fieldName, textBoxMethodName.Text), "DataPumpNavn", DataViewRowState.CurrentRows).ToTable(true, "FieldName", "BaseColumnName");
            //    textBoxInformation.Text = string.Format("{0} ({1}){2}", fieldName, textBoxMethodName.Text, Environment.NewLine);

            //    if (table.Rows.Count == 0)
            //    {
            //        table = new DataView(m_mappingTable.Copy(), string.Format("BaseColumnName Like '%{0}%' AND DataPumpNavn <> '{1}'", fieldName, textBoxMethodName.Text), "DataPumpNavn", DataViewRowState.CurrentRows).ToTable(true, "FieldName", "BaseColumnName");
            //        textBoxInformation.Text += string.Format("No sure match found, suggesting fields based on LIKE '%{0}%'. Tip: CTRL-Z to undo.{1}", fieldName, Environment.NewLine);
            //    }
            //    if (table.Rows.Count >= 1)
            //    {
            //        object value = table.Rows[0]["FieldName"].ToString();

            //        if (table.Rows.Count > 1)
            //        {
            //            textBoxInformation.Text += string.Format("'{0}' maps to {1} fieldnames. ({2})", fieldName, table.Rows.Count,
            //                 string.Join(", ", table
            //         .AsEnumerable()
            //         .Select(row => row.Field<string>("FieldName"))
            //         .ToArray()));

            //            //  table = table.AsEnumerable().GroupBy(r => new { FieldName = r["FieldName"]}).Last().CopyToDataTable();

            //            string.Format("'{0}' maps to {1} fieldnames", fieldName, table.Rows.Count);
            //            if (FormUtilities.InputDialogs.ChooseListBox(
            //                                 string.Format("'{0}' maps to {1} fieldnames", fieldName, table.Rows.Count)
            //                                 , "Please choose fieldname alias"

            //                                 , table, ref value) != System.Windows.Forms.DialogResult.OK)
            //                return;
            //            value = (value as ListViewItem).Text;
            //        }



            //        textBoxQuery.InsertText(string.Format("{0} as {1}", fieldName, value), true);




            //    }
            //    else textBoxInformation.Text += " No mapping found.";

            //    e.Handled = true;
            //}
            #endregion

            #region comment out text
            //Kommentarbehandling
            if ((e.KeyData == (Keys.Control | Keys.K)))
            {
                string text = textBoxQuery.SelectedText;//.Document.GetText(richEditControl1.Document.Selection).ToString();

                if (string.IsNullOrEmpty(text))
                { e.Handled = false; }
                else
                {
                    textBoxQuery.Text = textBoxQuery.Text.Replace(textBoxQuery.Selection.Text, string.Format("/*{0}*/", textBoxQuery.Selection.Text));
                    e.Handled = true;
                }
            }
            #endregion

            #region Copy with parameters
            //if (e.Control && e.Shift && e.KeyCode == Keys.C)
            //{
            //    try
            //    {
            //        string myString = m_controller.LagSqlSporring(textBoxQuery.Text);
            //        try
            //        {
            //            myString = MetaDataController.SQLFormatter(myString);
            //        }
            //        catch (Exception) { }

            //        if (!string.IsNullOrEmpty(myString))
            //        {
            //            Clipboard.SetDataObject(myString, true);
            //            toolStripStatusLabel1.Text = "Spørring lagt til utskriftstavlen.";
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        toolStripStatusLabel1.Text = "Kunne ikke plassere tekst til utskriftstavlen";

            //    }
            //    e.Handled = true;
            //}
            #endregion
        }
        public static DataTable GetData(string connectionString, string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(sql, c))
                {
                    sda.Fill(dt);
                }

                if(dt!=null&&dt.DataSet==null)
                {
                    DataSet ds =  ds = new DataSet();  
                        ds.Tables.Add(dt); 
                } 
                return dt;
            }
        }
    }
}