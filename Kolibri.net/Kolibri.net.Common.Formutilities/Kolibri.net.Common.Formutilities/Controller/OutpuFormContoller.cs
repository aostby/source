using FastColoredTextBoxNS;
using global::Kolibri.net.Common.Formutilities.Forms;
using Kolibri.net.Common.Utilities;
using System.Data;
using System.Text;
using System.Xml;

namespace Kolibri.net.Common.FormUtilities.Controller
{

    public class OutputFormController
        {
            public static System.Windows.Forms.Form RichTextBoxForm(string title, string text, System.Drawing.Size size)
            {
                return RichTexBoxForm(title, text, null, size);
            }
            public static Form RichTexBoxForm(string title, string text, FastColoredTextBoxNS.Language? lang, System.Drawing.Size size)
            {
                Form form = new Form();
                form.Size = size;
                form.Text = title;
                FastColoredTextBoxNS.FastColoredTextBox dgv = new FastColoredTextBoxNS.FastColoredTextBox();
                if (lang != null) dgv.Language = (FastColoredTextBoxNS.Language)lang;
                dgv.Text = text;
                form.Controls.Add(dgv);
                dgv.Dock = DockStyle.Fill;

                dgv.KeyDown += OutputDialogs.FCTB_KeyDown;
                return form;
            }

            public static Form DataTableForm(string title, DataTable table, DataColumn presentInDetail, System.Drawing.Size size)
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

                return form;
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

            internal static void ShowDataTableDialog_KeyDown(object sender, KeyEventArgs e)
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
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = table.TableName + ".xml";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "")
                        {
                            table.DataSet.WriteXml(saveFileDialog.FileName);
                            FileUtilities.OpenFolderHighlightFile(new FileInfo(saveFileDialog.FileName));
                        }

                        // Utilities.FileUtilities.ExportToFormats(textBoxQuery.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
                e.Handled = true;
            }


        }
    }
