using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using DevExpress.XtraGrid.Views.Grid;
using System.Xml.Xsl;
using System.Xml;
using Kolibri.Utilities;

namespace Kolibri.FormUtilities.Forms
{
    public partial class XmlViewerForm : Form
    {
        private string m_importfilsti;
        private DataSet m_ds;
        private string m_version;
        public XmlViewerForm()
        {
            InitializeComponent();
            richTextBox1.Language = FastColoredTextBoxNS.Language.HTML;
        }

        public XmlViewerForm(string filsti)
        {
            InitializeComponent();
            LastInnDokument(filsti);
            richTextBox1.Language = FastColoredTextBoxNS.Language.HTML;
        }
 
        private void LastInnDokument(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    m_importfilsti = fileName;
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(m_importfilsti);
                    if (doc.FirstChild.OuterXml.ToLower().Contains("version="))
                        m_version = doc.FirstChild.OuterXml;
                    else
                        m_version = string.Empty;

                    DataSet set = new DataSet();
                    set.ReadXml(m_importfilsti);
                    m_ds = set;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Feil ved lesing av fil: " + ex.Message);
                m_ds = null;
                m_importfilsti = string.Empty;
                comboBox1_SelectedIndexChanged(null, null);

            }
            OppdaterSkjerm();
        }

        private void buttonLesXml_Click(object sender, EventArgs e)
        {
            // this.Cursor = Cursors.WaitCursor;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            string fileName = dialog.FileName;
            LastInnDokument(fileName);
        }

        void OppdaterSkjerm()
        {
            toolStripStatusLabel1.Text = m_importfilsti;
            comboBox1.Items.Clear();
            gridControl1.DataSource = null;
            GridView view = gridControl1.DefaultView as GridView;
            if (view != null)
            {
                view.GroupPanelText = string.Empty;// "Dra kolonneoverskrift opp hit for å organisere på innhold";
                view.Columns.Clear();
            }
            try
            {

                if (m_ds != null)
                {
                    for (int i = 0; i < m_ds.Tables.Count; i++)
                    {
                        if (m_ds.Tables[i].Rows.Count > 0)
                            comboBox1.Items.Add(m_ds.Tables[i].TableName);
                    }
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception)
            { }

            if (tabControl1.SelectedIndex == 2)
            {
                webBrowser1.Navigate("");
                webBrowser1.Navigate(m_importfilsti);
            } if (tabControl1.SelectedIndex == 2)
            {
                OppdaterSkjermRTB();
            }
        }
        
        private void OppdaterSkjermRTB()
        {
            if (m_ds != null)
            {
                //StringBuilder builder = new System.Text.StringBuilder();// Create the StringWriter object with the StringBuilder object. 
                //StringWriter writer = new System.IO.StringWriter(builder);// Write the schema into the StringWriter. 
                //m_ds.WriteXml(writer);
                //richTextBox1.Text = writer.ToString();
                if (!string.IsNullOrEmpty(m_version))
                    richTextBox1.Text = m_version + Environment.NewLine + m_ds.GetXml();
                else
                {
                    string xml = m_ds.GetXml();
                    richTextBox1.Clear();
                    richTextBox1.Text = xml;
                }
            }
            else
                richTextBox1.Clear();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView view = gridControl1.DefaultView as GridView;
            if (view != null)
            {
                view.Columns.Clear();
            }
            if (m_ds != null && comboBox1.Items.Count > 0)
            {
                gridControl1.DataSource = m_ds.Tables[comboBox1.Text.ToString()];


            }
            else
            {
                gridControl1.DataSource = null;
            }
            gridControl1.RefreshDataSource();
            gridControl1.DefaultView.RefreshData();
            gridControl1.Update();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = m_importfilsti;
            comboBox1.Visible = (tabControl1.SelectedIndex == 0);
            groupBoxTransform.Visible = (tabControl1.SelectedIndex == 1);
           
            buttonPrintPreview.Enabled = (tabControl1.SelectedIndex <= 1);
            buttonSave.Enabled = (tabControl1.SelectedIndex == 2);
         //   button2.Enabled = button2.Visible;
            if (tabControl1.SelectedIndex == 2)
            {
                toolStripStatusLabel1.Text = "Xml er hentet fra 'Tabell' - endringer derfra er synlige her. (Endringer du gjør manuelt må lagres og lastes inn igjen.)";
                OppdaterSkjermRTB();
            }
            if (tabControl1.SelectedIndex == 1)
            {
                try
                { if (comboBox2.SelectedIndex < 0) comboBox2.SelectedIndex = 1; }
                catch (Exception)
                { }
                if (comboBox2.SelectedIndex < 0) comboBox2.SelectedIndex = 0;
                toolStripStatusLabel1.Text = "Nettleservinduet reflekterer " + m_importfilsti + " slik den var ved innlesning.";
            }
        }
               
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Process pros = new Process();

                pros.StartInfo.FileName = "explorer.exe";
                pros.StartInfo.Arguments = m_importfilsti;
                pros.Start();
            }
            catch (Exception)
            {
            }

        }
     
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                doc.LoadXml(richTextBox1.Text);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = new FileInfo(m_importfilsti).Name + "_bak";
                sfd.Filter = "xml |*.xml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    doc.Save(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void buttonPrintPreview_Click(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
                gridControl1.ShowPrintPreview();
            if (tabControl1.SelectedIndex == 1)
                webBrowser1.ShowPrintDialog();
        }

  
        
        private void validateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fDialog = new OpenFileDialog();

                fDialog.Title = "Open XSD File";
                fDialog.Filter = "XSD Files|*.xsd";

                if (fDialog.ShowDialog() == DialogResult.OK)
                {

                    if (!Utilities.XMLUtilities.ValidateXml(new FileInfo(m_importfilsti), new FileInfo(fDialog.FileName), true))
                        MessageBox.Show(Utilities.Logger.GetLastErrorLogLine(), fDialog.FileName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(m_importfilsti + Environment.NewLine + "The file is valid.", fDialog.FileName, MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), m_importfilsti);
            }
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            #region save
            if ((e.KeyData == (Keys.Control | Keys.S)))
            {
                try
                {

                //Pass på at man ikke driver og publiserer
                toolStripStatusLabel1.Text = "Save (CTRL+S)";
                FileUtilities.WriteStringToFile(richTextBox1.Text, m_importfilsti);
                m_ds.ReadXml(m_importfilsti);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    
                }
                e.Handled = true;
            }
            #endregion
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            string filename = m_importfilsti;

            filename = Path.Combine(Path.GetTempPath(), "transform.html");
            if (File.Exists(filename))
                File.Delete(filename);

            try
            {
                string value = (sender as ComboBox).Text;
                System.IO.MemoryStream sw = new System.IO.MemoryStream();
                XslCompiledTransform xslTrans = new XslCompiledTransform();
                XmlDocument InputXMLDocument = new XmlDocument();
                InputXMLDocument.LoadXml(m_ds.GetXml().Insert(0, "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>"));
                XsltArgumentList XSLTArgs = new XsltArgumentList();
                string temp;
                switch (value)
                {
                    case "Liste":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.UnorderedList.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        break;
                    case "Flat":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.configfiles.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        break;
                    case "Tabell":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.GenericHTML01.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        break;
                    case "Trestruktur":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.treeview.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        break;
                    case "AltInn":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.altinn.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        break;
                    case "Excel":
                        temp = new string(Kolibri.FormUtilities.Properties.Resources.excel.ToCharArray());
                        xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                        filename = filename.Replace(".html", ".xls");
                        break;
                    default:
                        filename = m_importfilsti;
                        break;
                }

               
                xslTrans.Transform(InputXMLDocument.CreateNavigator(), XSLTArgs, sw);
                Utilities.FileUtilities.WriteByteArrayToFile(sw.GetBuffer(), filename);
            }
            catch (Exception ex)
            {
            //   if(Utilities.SystemInfo.GetLoggedOnUser.Equals("aostby")) FormUtilities.SplashScreen.Splash(ex.Message,1000);
            }
            webBrowser1.Navigate("");
            webBrowser1.Focus();
            webBrowser1.Navigate(filename);
        }
          
    }
}
/*
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            string filename = m_importfilsti; 
            RadioButton button = sender as RadioButton;
                if (button == null)
                    button = radioButtonStandard;
                if (button != radioButtonStandard)
                {
                    filename = Path.Combine(Path.GetTempPath(), "transform.html");
                    if (File.Exists(filename))
                        File.Delete(filename);
                    try
                    {
                        System.IO.MemoryStream sw = new System.IO.MemoryStream();
                       // sw.Encoding = Encoding.GetEncoding("ISO-8859-1");
                        XslCompiledTransform xslTrans = new XslCompiledTransform();
                        XmlDocument InputXMLDocument = new XmlDocument();
                        
                        InputXMLDocument.LoadXml(m_ds.GetXml().Insert(0,  "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>"));
                        
                        XsltArgumentList XSLTArgs = new XsltArgumentList();
                  //   xslTrans.OutputSettings.Encoding = Encoding.GetEncoding("iso-8859-1");

                        string temp;
                        switch (button.Text)
                        {
                            case "Flat":
                                //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\DS2HTML_Div.xslt");
                                temp = new string(Kolibri.FormUtilities.Properties.Resources.configfiles.ToCharArray());
                                //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\ConfigFiles.xsl");
                                xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                                break;
                            case "Tabell":
                                //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\DS2HTML_Div.xslt");
                                temp = new string(Kolibri.FormUtilities.Properties.Resources.GenericHTML01.ToCharArray());
                                //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\ConfigFiles.xsl");
                                xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                                break;
                            case "Trestruktur":
                                temp = new string(Kolibri.FormUtilities.Properties.Resources.treeview.ToCharArray());
                                // xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\TreeView.xslt");                              
                                xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                                break;
                            case "AltInn":
                                temp = new string(Kolibri.FormUtilities.Properties.Resources.altinn.ToCharArray());
                               // xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\AltInn.xsl");                              
                                xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                                break;
                            case "Excel":
                                temp = new string(Kolibri.FormUtilities.Properties.Resources.excel.ToCharArray());
                             //   xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\Excel.xslt");
                                xslTrans.Load(new XmlTextReader(new StringReader(temp)));
                                //filename = filename.Replace(".html", ".xsl");
                                filename = filename.Replace(".html", ".xls");
                                break;
                            default:                              
                                break;
                        }

                        //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\html_w_headers.xslt");
                        //xslTrans.Load(@"D:\Test_og_eksempler\XML_XSLT\DS2HTML_Div.xslt");
                        //xslTrans.Load(@"D:\Source\Split\DEV\2011\Projects_10.00.00\Maritech.Startpanel\Maritech.Startpanel\bin\x86\Release\ConfigFiles.xsl");
               
                        xslTrans.Transform(InputXMLDocument.CreateNavigator(), XSLTArgs, sw);
                        Utilities.FileUtilities.WriteByteArrayToFile(sw.GetBuffer(), filename);
                    }


                    catch (Exception)
                    {
                    }
                } webBrowser1.Navigate("");
            webBrowser1.Navigate(filename);
                
           // OppdaterSkjermRTB();
            //return sw.ToString();  

        }
*/