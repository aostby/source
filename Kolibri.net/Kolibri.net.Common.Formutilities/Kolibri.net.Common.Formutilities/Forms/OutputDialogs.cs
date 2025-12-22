using FastColoredTextBoxNS;
using global::Kolibri.net.Common.Utilities;
using Kolibri.net.Common.FormUtilities.Controller;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.net.Common.FormUtilities.Forms
{
    public class OutputDialogs
    {
        public static DialogResult ShowRichTextBox(string title, string text, System.Drawing.Size size)
        {
            Form form = OutputFormController.RichTextBoxForm(title, text, size);
            form.Show();
            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }
        public static DialogResult ShowRichTextBoxDialog(string title, string text, System.Drawing.Size size)
        {
            Form form = OutputFormController.RichTextBoxForm(title, text, size);
            form.ShowDialog();
            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="language"> Custom = 0,CSharp = 1,VB = 2,HTML = 3,XML = 4,SQL = 5,PHP = 6,JS = 7,Lua = 8,</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static DialogResult ShowRichTextBoxDialog(string title, string text, int language, System.Drawing.Size size)
        {
            return ShowRichTextBoxDialog(title, text, (FastColoredTextBoxNS.Language)language, size);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="language"> Custom = 0,CSharp = 1,VB = 2,HTML = 3,XML = 4,SQL = 5,PHP = 6,JS = 7,Lua = 8,</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static DialogResult ShowRichTextBoxDialog(string title, string text, string language, System.Drawing.Size size)
        {
            FastColoredTextBoxNS.Language lang = Language.Custom;
            Enum.TryParse(language, out lang);
            return ShowRichTextBoxDialog(title, text, lang, size);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="lang">  Custom = 0 CSharp = 1  VB = 2  HTML = 3  SQL = 4  PHP = 5 JS = 6 </param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static DialogResult ShowRichTextBoxDialog(string title, string text, FastColoredTextBoxNS.Language lang, System.Drawing.Size size)
        {
            return ShowRichTextBoxDialog(title, text, lang, size, true);
        }
        public static DialogResult ShowRichTextBox(string title, string text, FastColoredTextBoxNS.Language lang, System.Drawing.Size size)
        {
            return ShowRichTextBoxDialog(title, text, lang, size, false);
        }
        private static DialogResult ShowRichTextBoxDialog(string title, string text, FastColoredTextBoxNS.Language lang, System.Drawing.Size size, bool dialog)
        {
            if (text.Length > 100000 && lang == Language.XML)
                lang = Language.HTML;
            Form form = OutputFormController.RichTexBoxForm(title, text, lang, size);
            if (dialog)
                form.ShowDialog();
            else
                form.Show();
            form.DialogResult = DialogResult.OK;
            return form.DialogResult;
        }

        public static DialogResult ShowDataTableDialog(string title, DataTable table, System.Drawing.Size size)
        {
            Form form = new Form();
            form.Size = size;
            form.Text = title;
            //form.Icon = Icons.IconFromImage(Kolibri.net.Common.Utilities.Controller.ResourceController.GetResouceObject("table") as Bitmap);

            DataGridView dgv = new DataGridView();
            dgv.DataSource = table;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            form.Controls.Add(dgv);
            dgv.Dock = DockStyle.Fill;
            dgv.KeyDown += new KeyEventHandler(ShowDataTableDialog_KeyDown);

            form.ShowDialog();
            form.DialogResult = DialogResult.OK;

            return form.DialogResult;
        }
        public static DialogResult ShowDataTableDialog(string title, DataTable table, DataColumn presentInDetail, System.Drawing.Size size)
        {
            Form form = new Form();
            form.Size = size;
            form.Text = title;
            DataGridView dgv = new DataGridView();
            dgv.DataSource = table;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.Dock = DockStyle.Fill;
            dgv.Tag = presentInDetail.ColumnName;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.KeyDown += new KeyEventHandler(ShowDataTableDialog_KeyDown);
            dgv.SelectionChanged += ShowDataTableDialog_SelectionChanged;


            /*
             * // Set property values appropriate for read-only - display and limited interactivity
dgv.AllowUserToAddRows = false;
dgv.AllowUserToDeleteRows = false;
dgv.AllowUserToOrderColumns = true;
dgv.ReadOnly = true;
dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dgv.MultiSelect = false;
dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
dgv.AllowUserToResizeColumns = false;
dgv.ColumnHeadersHeightSizeMode = 
dgv.DisableResizing;
dgv.AllowUserToResizeRows = false;
dgv.RowHeadersWidthSizeMode = 
dgv.DisableResizing;*/


            SplitContainer split = new SplitContainer();
            split.Orientation = Orientation.Vertical;
            split.Panel1.Controls.Add(dgv);
            split.Dock = DockStyle.Fill;
            if (presentInDetail.DataType == typeof(Bitmap))
            {
                split.Panel2.AutoScroll = true;
                PictureBox box = new PictureBox();
                box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;

                split.Panel2.Controls.Add(box);
            }
            if (presentInDetail.DataType == typeof(XmlDocument))
            {
                split.Panel2.AutoScroll = true;
                FastColoredTextBox textbox = new FastColoredTextBox();
                textbox.Language = Language.XML;
                textbox.Dock = DockStyle.Fill;
                split.Panel2.Controls.Add(textbox);
            }
            else
            {
                RichTextBox displaytext = new RichTextBox();
                displaytext.Dock = DockStyle.Fill;
                split.Panel2.Controls.Add(displaytext);
            }
            form.Controls.Add(split);

            form.Show();

            try
            {
                foreach (DataGridViewColumn item in dgv.Columns)
                {
                    if (item.Name.Equals(presentInDetail.ColumnName))
                    {
                        item.Visible = false;
                        break;
                    }
                }
            }
            catch (Exception)
            { }


            form.DialogResult = DialogResult.OK;

            return form.DialogResult;
        }
        internal static void ShowDataTableDialog_SelectionChanged(object sender, EventArgs args)
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


                    if (row[temp.Tag.ToString()].GetType() == typeof(Bitmap))
                    {
                        SplitterPanel split = temp.Parent as SplitterPanel;
                        (((split.Parent as SplitContainer).Panel2.Controls[0]) as PictureBox).Image = row[temp.Tag.ToString()] as Bitmap;
                    }
                    if (row[temp.Tag.ToString()].GetType() == typeof(XmlDocument))
                    {
                        SplitterPanel split = temp.Parent as SplitterPanel;
                        (((split.Parent as SplitContainer).Panel2.Controls[0]) as FastColoredTextBox).Text =
                    XMLUtilities.PrettyPrintXML((row[temp.Tag.ToString()] as XmlDocument).OuterXml, Encoding.UTF8);
                    }
                    else
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
        public static void ShowDataTableDialog_KeyDown(object sender, KeyEventArgs e)
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
                        if (view.Name.Equals("filter", StringComparison.OrdinalIgnoreCase))
                        {
                            table = new DataView(table).ToTable(false, "title", "ImdbRating", "Year", "Released", "Genre", "Runtime");
                        }


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
                {

                    string filter = FileUtilities.ExportToFormats(table);
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

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
        internal static void FCTB_KeyDown(object sender, KeyEventArgs e)
        {
            FastColoredTextBox textBoxQuery = sender as FastColoredTextBox;
            #region save
            if ((e.KeyData == (Keys.Control | Keys.S)))
            {
                FileUtilities.ExportToFormats(textBoxQuery.Text);
            }
            #endregion

            #region Kjør querey
            if (e.KeyData == Keys.F5)
            {
                //  buttonTestQuery_Click(null, null);
                //  e.Handled = true;

                try
                {
                    XDocument xdoc = XDocument.Parse(textBoxQuery.Text);
                    var Xele = Kolibri.net.Common.Utilities.Extensions.XmlExt.GetElement(xdoc.Root, "xml");
                    if (Xele != null)
                    {
                        string xml = Xele.FirstNode.ToString();
                        try
                        {
                            xml = WebUtility.HtmlDecode(xml);
                            xml = XDocument.Parse(xml).ToString();
                        }
                        catch (Exception)
                        { }

                        //    FormUtilities.OutputDialogs.ShowRichTextBoxDialog("Xml xml", xml, new Size(600, 400));
                        FormUtilities.Visualizers.VisualizeTransformedXML(xdoc.Root.Name.LocalName + " xml", xml, new Size(600, 400));
                    }
                }
                catch (Exception exF5)
                {
                    string msg = exF5.Message;
                }

            }
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
                return dt;
            }
        }
    }
}