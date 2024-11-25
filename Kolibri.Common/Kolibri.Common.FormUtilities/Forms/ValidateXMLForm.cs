using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Extensions;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class ValidateXMLForm : Form
    {      private static string strCompName = "TESTFORM";
        private static FileInfo m_last = null;
        public ValidateXMLForm()
        {
            InitializeComponent();
        
            Init();
            InitComboBoxes(); 
            
        }
        private void Init()
        {
          richTextBox1.Language = FastColoredTextBoxNS.Language.XML;
            this.Text += $" ({Environment.UserName}) - [{Assembly.GetExecutingAssembly().GetName().Version}]";
            try
            {
                if (File.Exists(Properties.Settings.Default.LastOpened))
                    m_last = new FileInfo(Properties.Settings.Default.LastOpened);

            }
            catch (Exception ex)
            {
                string msg = ex.Message; toolStripStatusLabelXML.Text = ex.Message;

            }
        }
        private void InitComboBoxes()
        {
            try
            {
                comboBoxSchema.SelectedIndex = -1; comboBoxSchema.DataSource = null;
                comboBoxStyleSheet.SelectedIndex = -1; comboBoxStyleSheet.DataSource = null;
                comboBoxSchema.Items.Clear();
                comboBoxStyleSheet.Items.Clear();

                DirectoryInfo d = new DirectoryInfo(Path.Combine(Application.StartupPath, "Schemas"));
                FileInfo[] Files = d.GetFiles("*.xsd"); //Getting Text files

                comboBoxSchema.DataSource = Files;
                comboBoxSchema.DisplayMember = "Name";
            }
            catch (Exception ex) { toolStripStatusLabelXML.Text = ex.Message; }

            try
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(Application.StartupPath, "StyleSheets"));
                FileInfo[] Files = d.GetFiles("*.xsl*"); //Getting Text files

                comboBoxStyleSheet.DataSource = Files;
                comboBoxStyleSheet.DisplayMember = "Name";
            }
            catch (Exception) { }
            try
            {
                comboBoxStyleSheet.SelectedIndex = comboBoxStyleSheet.FindStringExact(new FileInfo(Properties.Settings.Default.LastStylesheet).Name);
            }
            catch (Exception ex) { toolStripStatusLabelXML.Text = ex.Message; }
            try
            {
                comboBoxSchema.SelectedIndex = comboBoxSchema.FindStringExact(new FileInfo(Properties.Settings.Default.LastSchema).Name);
            }
            catch (Exception ex) { toolStripStatusLabelXML.Text = ex.Message; }
            try
            {
                if (comboBoxSchema.SelectedIndex < 0)
                    comboBoxStyleSheet.SelectedIndex = comboBoxStyleSheet.FindStringExact("GenericHTML01.xsl");
                if (comboBoxSchema.SelectedIndex < 0)
                    comboBoxSchema.SelectedIndex = comboBoxSchema.FindStringExact("Rbd_ArticleMd_Response.xsd");

            }
            catch (Exception ex)
            { toolStripStatusLabelXML.Text = ex.Message; }
        }

        private void buttonSendSOAP_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(richTextBox1.Text);
                SendSOAP(xmlDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void SendSOAP(XmlDocument xmlDoc)
        {
            string text = string.Empty;
            text = "2021-09-22 - AEØ denne funksjonaliteten er fjernet fra applikasjonen, kjør den fra kildekoden isåfall";
            var test = "Fjernet pga dll peker";
            //var test = global::Hemit.BizTalk.RBD.Masterdata.BusinessComponents.Controller.SOAPSendController.Execute(
            //               Guid.NewGuid().ToString(), xmlDoc.OuterXml, out text);
            MessageBox.Show(text, test.ToString());
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelXML.Text = string.Empty;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                FileInfo filePath = new FileInfo(@"\\biztalkfil\BiztalkT\Fildropp\ERP\RBD\DebugVareregister\example.xml");
                if (Environment.UserName.Contains("asostb"))
                    filePath = new FileInfo(@"C:\Users\adm_asostb-he\AppData\Local\Temp\_MapData\Hemit.BizTalk.RBD.Masterdata.Maps\SAP_ZMATERIALMASTER_To_Rbd_ArticleMd_Response_output.xml");

                if (m_last != null && m_last.Exists)
                    filePath = m_last;

                if (filePath.Exists)
                {
                    ofd.FileName = filePath.Name;
                    ofd.InitialDirectory = filePath.Directory.FullName;
                }
                else if (filePath.Directory.Exists)
                {
                    ofd.InitialDirectory = filePath.Directory.FullName;
                }

                ofd.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                ofd.FilterIndex = 1;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    m_last = new FileInfo(ofd.FileName);
                    try
                    {
                           richTextBox1.OpenFile(m_last.FullName, Encoding.UTF8);


                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(m_last.FullName);

                        richTextBox1.Text = XDocument.Parse(xmlDoc.OuterXml).ToString();
                    }
                    catch (Exception)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(m_last.FullName);

                        richTextBox1.Text = XDocument.Parse(xmlDoc.OuterXml).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(richTextBox1.Text);

                try
                {
                    XDocument xDoc = XDocument.Parse(xmlDoc.OuterXml);
                    var tempEle = xDoc.Root.GetElement("Content");
                    if (tempEle == null && xDoc.Root.Name.LocalName == "Content")
                        tempEle = xDoc.Root;
                    xDoc = XDocument.Load(tempEle.CreateReader());
                    xmlDoc = xDoc.ToXmlDocument();

                }
                catch (Exception)
                { }


                var temp = RemoveIllegalValues(xmlDoc);
                xmlDoc = RemoveEmptyElements(temp);

                richTextBox1.Text = XDocument.Parse(xmlDoc.OuterXml).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            FileInfo schema = null; ;
            try
            {
                schema = comboBoxSchema.Items[comboBoxSchema.FindStringExact(comboBoxSchema.SelectedValue.ToString())] as FileInfo;
                Properties.Settings.Default.LastSchema = schema.FullName;
            }
            catch (Exception)
            { }

            try
            {

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(richTextBox1.Text);

                string text = xmlDoc.OuterXml;
                XDocument xDoc = XDocument.Parse(text);

                //Fjern schemaLocation siden vi har ett lokalt skjema
                var temp = xDoc.Root.Attributes().FirstOrDefault(x => x.Name.LocalName.Contains("schemaLocation"));
                if (temp != null)
                {
                    temp.Remove();

                    toolStripStatusLabelXML.Text = $"Removing {temp}";
                }
                xmlDoc = xDoc.ToXmlDocument();
                xmlDoc.Validate(schema);

                Color color = toolStripStatusLabelXML.BackColor;
                toolStripStatusLabelXML.Text = $"Success - the xml validates using {schema.Name}";
                toolStripStatusLabelXML.BackColor = Color.LimeGreen;
                Application.DoEvents();
                Thread.Sleep(500);
                toolStripStatusLabelXML.BackColor = color;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        public static XmlDocument RemoveIllegalValues(XmlDocument xmlDoc)
        {
            List<string> iVal = new List<string>() { "0.0", "0000-00-00", "000000" };
            XDocument xDoc = XDocument.Parse(xmlDoc.OuterXml);
            try
            {
                foreach (var item in iVal)
                {
                    xDoc.Descendants().Where(a => $"{a.Value}".Equals(item)).Remove();
                }

                XmlDocument ret = new XmlDocument();
                ret.LoadXml(xDoc.ToString());
                return ret;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(strCompName, $"{System.Reflection.MethodBase.GetCurrentMethod().Name} feilet - {ex.Message}");
                /*toolStripStatusLabelXML.Text = ex.Message; */
                return xmlDoc;
            }
        }

        /// <summary>
        /// RemoveEmptyElements - Felt som ikke har verdier i orginalfila (json-fila) bør ikke med,  
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static XmlDocument RemoveEmptyElements(XmlDocument xmlDoc)
        {
            try
            {
                var document = XDocument.Parse(xmlDoc.OuterXml);
                document.Descendants()
               .Where(a => a.IsEmpty || String.IsNullOrWhiteSpace(a.Value))
               .Remove();
                XmlDocument ret = new XmlDocument();
                ret.LoadXml(document.ToString());
                return ret;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(strCompName, $"{System.Reflection.MethodBase.GetCurrentMethod().Name} feilet - {ex.Message}");
                return xmlDoc; /*toolStripStatusLabelXML.Text = ex.Message*/;
            }
        }

        private void buttonFilelist_Click(object sender, EventArgs e)
        {

            bool log = string.IsNullOrWhiteSpace(richTextBox1.Text);
            string path = @"\\biztalkfil\BiztalkT\Fildropp\ERP\RBD\DebugVareregister";//@"\\ahsbxgws02\gid\OUT\GID708\Error"

            try
            {
                var tempstring = Properties.Settings.Default.FileListPath;
                if (!string.IsNullOrWhiteSpace(tempstring))
                {
                    DirectoryInfo dir = new DirectoryInfo(tempstring);
                    if (dir.Exists)
                        path = dir.FullName;
                }
            }
            catch (Exception) { }


            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();

                if (Directory.Exists(path))
                {
                    fbd.SelectedPath = path;
                    SendKeys.Send("{TAB}{TAB}{DOWN}{RIGHT}{UP}");


                }
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (log)
                    {
                        try
                        {
                            richTextBox1.AppendText("Please wait while preparing list...." + Environment.NewLine);
                            richTextBox1.Parent.Update(); richTextBox1.Parent.Refresh();
                            Application.DoEvents();
                        }
                        catch (Exception) { }
                    }



                    StringBuilder builder = new StringBuilder();
                    StringBuilder errBuilder = new StringBuilder();
                    var list = Directory.GetFiles(fbd.SelectedPath, "*.xml", SearchOption.TopDirectoryOnly);

                    try
                    {
                        richTextBox1.AppendText($"list is {list.Count()} items long...." + Environment.NewLine);
                        richTextBox1.Parent.Update(); richTextBox1.Parent.Refresh();
                        Application.DoEvents();
                        Properties.Settings.Default.FileListPath = fbd.SelectedPath;

                    }
                    catch (Exception) { }



                    var dic = CreateFileDictionary(list);
                    Dictionary<string, string> errDic = new Dictionary<string, string>();
                    if (dic.Keys.Count > 0)
                    {
                        foreach (var key in dic.Keys)
                        {
                            if (log)
                            {
                                try
                                {
                                    richTextBox1.AppendText(key + Environment.NewLine);
                                    richTextBox1.Parent.Update(); richTextBox1.Parent.Refresh();
                                    Application.DoEvents();

                                }
                                catch (Exception) { }
                            }
                            string errors = $"{dic[key]}".Replace(Environment.NewLine, " + ");
                            builder.AppendLine($"{key} | {errors}");
                            try
                            {
                                foreach (var item in errors.Split('+'))
                                {
                                    if (!string.IsNullOrEmpty(item.Trim()))
                                    {
                                        errDic[item.Trim()] = key;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }

                        try
                        {
                            foreach (var key in errDic.Keys)
                            {
                                errBuilder.AppendLine($"{key} | {errDic[key]}");
                            }
                        }
                        catch (Exception ex) { toolStripStatusLabelXML.Text = ex.Message; }


                        if (log)
                        {
                            try
                            {
                                richTextBox1.Clear(); Application.DoEvents();
                                richTextBox1.AppendText(Environment.NewLine + "Done. Opening folder in Explorer" + Environment.NewLine);
                                Application.DoEvents();
                                richTextBox1.Parent.Update(); richTextBox1.Parent.Refresh();
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(1000);
                            }
                            catch (Exception) { }
                        }

                        FileInfo info = new FileInfo(@"c:\temp\RBDErrors.txt");
                        FileInfo errInfo = new FileInfo(@"c:\temp\RBDErrorsUniqueList.txt");
                        if (!info.Directory.Exists) info.Directory.Create();

                        File.WriteAllText(info.FullName, builder.ToString());
                        File.WriteAllText(errInfo.FullName, errBuilder.ToString());
                        FileUtilities.OpenFolderHighlightFile(errInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }

            if (log) richTextBox1.Clear();

        }

        private Dictionary<string, string> CreateFileDictionary(string[] list)
        {
            Dictionary<string, string> valDic = new Dictionary<string, string>();
            FileInfo schema = comboBoxSchema.Items[comboBoxSchema.FindStringExact(comboBoxSchema.SelectedValue.ToString())] as FileInfo;
            foreach (var item in list)
            {
                valDic[item] = "OK";
                try
                {
                    FileInfo info = new FileInfo(item);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(info.FullName);

                    try
                    {
                        XDocument xDoc = XDocument.Parse(xmlDoc.OuterXml);
                        var tempEle = xDoc.Root.GetElement("Content");
                        if (tempEle == null && xDoc.Root.Name.LocalName == "Content")
                            tempEle = xDoc.Root;

                        xDoc = XDocument.Load(tempEle.CreateReader());
                        xmlDoc = xDoc.ToXmlDocument();
                        string text = xmlDoc.OuterXml;
                        //text = text.Replace("<Header>", @"<Header xmlns="""">");
                        //text = text.Replace("<Update>", @"<Update xmlns="""">");
                        xmlDoc = XDocument.Parse(text).ToXmlDocument();
                    }
                    catch (Exception)
                    { }

                    xmlDoc.Validate(schema);
                    valDic[item] = "OK";
                }
                catch (Exception ex)
                {
                    valDic[item] = ex.Message; toolStripStatusLabelXML.Text = ex.Message;
                }
            }
            return valDic;
        }

        private void buttonBeautify_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(richTextBox1.Text);
                richTextBox1.Text = XDocument.Parse(xmlDoc.OuterXml).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }

        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.Shift && e.Alt)
                {
                    if (e.KeyCode == Keys.B)
                    {
                        //string sourceCode = FastColoredTextBox1.Text;
                        //// not sure what's going on for you "location" but you need to do that logic here too
                        //File.WriteAllText(location, sourceCode);
                        buttonBeautify_Click(null, null);
                        e.SuppressKeyPress = true;
                    }
                    else if (e.KeyCode == Keys.V)
                    {
                        buttonValidate_Click(null, null);
                    }
                    else if (e.KeyCode == Keys.T)
                    {
                        buttonTransform_Click(null, null);
                    }
                    else if (e.KeyCode == Keys.C)
                    {
                        if (m_last == null || !m_last.Exists)
                            return;
                        if (MessageBox.Show("Watch for changes?", m_last.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CreateFileWatcher(m_last);
                        }
                    }
                    else if (e.KeyCode == Keys.L)
                    {
                        linarizeToolStripMenuItem_Click(null, null);
                    }
                    else if (e.KeyCode == Keys.I)
                    {
                        buttonRemove_Click(null, null);
                    }
                    else if (e.KeyCode == Keys.O)
                    {
                        FileUtilities.OpenFolderHighlightFile(m_last);
                    }
                }
                else if (e.Control && e.KeyCode == Keys.O) { buttonOpen_Click(null, null); }
                else if (e.Control && e.KeyCode == Keys.S) { buttonSave_Click(null, null); }
                   else if (e.Control && e.KeyCode == Keys.R && m_last != null) { richTextBox1.OpenFile(m_last.FullName, Encoding.UTF8); e.SuppressKeyPress = true; }
                else if (e.Control && e.KeyCode == Keys.L) { buttonFilelist_Click(null, null); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text)) return;

            SaveFileDialog sfd = new SaveFileDialog();
            if (m_last != null && m_last.Exists)
            {
                sfd.InitialDirectory = m_last.Directory.FullName;
                sfd.FileName = m_last.Name;
            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                  richTextBox1.SaveToFile(sfd.FileName, Encoding.UTF8);
            }
        }

        public void CreateFileWatcher(FileInfo info)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();


            watcher.Path = info.Directory.FullName;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = info.Name; //"*.txt";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                UserContrl1_LOadDataMethod(new FileInfo(e.FullPath));
            }
            catch (Exception)
            { }

            try
            {
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    this.toolStripStatusLabelXML.Text = $"{DateTime.Now.ToShortTimeString()} Udated {e.Name} ";
                });
                // Back on the worker thread
            }
            catch (Exception)
            {
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }
        private void UserContrl1_LOadDataMethod(FileInfo info)
        {
            string extension = "";
            if (richTextBox1.InvokeRequired)
            {
                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception) { }
                if (info.FullName.Equals(m_last.FullName))
                {//Hvis filen er den samme som vi jobber på så oppdater skjermbildet med fersk kopi
                     richTextBox1.Invoke(new MethodInvoker(delegate { richTextBox1.OpenFile(info.FullName, Encoding.UTF8); }));
                }
                else
                {
                    //Hvis filen er ett stilark vi lytter på, kopier det inn ved endringer så lokal kopi blir oppdatert.
                    DirectoryInfo dest = new DirectoryInfo(Path.Combine(Application.StartupPath, "StyleSheets"));
                    FileInfo destFile = new FileInfo(Path.Combine(dest.FullName, info.Name));
                    if (destFile.Exists)
                    {
                        File.Copy(info.FullName, destFile.FullName, true);
                    }
                }
            }
        }


        private void comboBoxSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.Visible) return;
            try
            {
                buttonSendSOAP.Visible = comboBoxSchema.SelectedValue.ToString().StartsWith("Rbd_ArticleMd_Response");

            }
            catch (Exception)
            { }
        }

        private void buttonTransform_Click(object sender, EventArgs e)
        {
            FileInfo stylesheet = null;
            try
            {
                stylesheet = comboBoxStyleSheet.Items[comboBoxStyleSheet.FindStringExact(comboBoxStyleSheet.SelectedValue.ToString())] as FileInfo;
                Properties.Settings.Default.LastStylesheet = stylesheet.FullName;

            }
            catch (Exception) { }

            try
            {

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(richTextBox1.Text);

                string text = xmlDoc.OuterXml;
                xmlDoc = XDocument.Parse(text).ToXmlDocument();

                var info = xmlDoc.Transform(stylesheet);

                if (info != null && info.Exists && info.Name.EndsWith("xml"))
                {
                    var res = MessageBox.Show($"* Yes opens file in current window,{Environment.NewLine}* No opens file in editor,{Environment.NewLine}* Cancel opens Explorer.{Environment.NewLine}{Environment.NewLine}{info.FullName}", "Open transformed file in current window?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                              richTextBox1.OpenFile(info.FullName, Encoding.UTF8);
                    }
                    else if (res == DialogResult.No)
                    {
                        Process.Start(info.FullName);
                    }
                    else
                    {
                        FileUtilities.OpenFolderHighlightFile(info);
                    }
                }
                else
                {
                    Process.Start(info.FullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void RBDXMLForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (m_last != null && m_last.Exists)
                    Properties.Settings.Default.LastOpened = m_last.FullName;

                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {

            }
        }

        private void buttonMySchema_Click(object sender, EventArgs e)
        {

            try
            {
                FileInfo info;
                string lastPath = Properties.Settings.Default.MySchema;
                if (!string.IsNullOrEmpty(lastPath))
                    info = FileUtilities.GetFile("Hent fil som legges til i " + (sender as Button).Text, new FileInfo(lastPath).Directory, "xsd");
                else
                    info = FileUtilities.GetFile("Hent fil som legges til i " + (sender as Button).Text, "xsd");
                if (info != null && info.Exists)
                {

                    DirectoryInfo dest = new DirectoryInfo(Path.Combine(Application.StartupPath, "Schemas"));
                    if (!dest.Exists) dest.Create();
                    lastPath = Path.Combine(dest.FullName, info.Name);
                    File.Copy(info.FullName, lastPath, true);
                    InitComboBoxes();
                    comboBoxSchema.SelectedIndex = comboBoxSchema.FindStringExact(info.Name);
                    Properties.Settings.Default.MySchema = info.FullName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void buttonMyStylesheet_Click(object sender, EventArgs e)
        {
            FileInfo info;
            string lastPath = Properties.Settings.Default.MyStylesheet;
            if (!string.IsNullOrEmpty(lastPath)) info = FileUtilities.GetFile("Hent fil som legges til i " + (sender as Button).Text, new FileInfo(lastPath).Directory, "xsl");
            else info = FileUtilities.GetFile("Hent fil som legges til i " + (sender as Button).Text, "xsl");
            if (info != null && info.Exists)
            {
                DirectoryInfo dest = new DirectoryInfo(Path.Combine(Application.StartupPath, "StyleSheets"));
                if (!dest.Exists) dest.Create();
                lastPath = Path.Combine(dest.FullName, info.Name);
                File.Copy(info.FullName, lastPath, true);
                InitComboBoxes();
                comboBoxStyleSheet.SelectedIndex = comboBoxStyleSheet.FindStringExact(info.Name);
                Properties.Settings.Default.MyStylesheet = info.FullName;
                var res = MessageBox.Show($"Update local copy when {info.Name} changes?", "Listen and update local copy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    CreateFileWatcher(info);
                }
            }

        }

        private void ApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start((sender as ToolStripMenuItem).Tag.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name); toolStripStatusLabelXML.Text = ex.Message;
            }
        }

        private void linarizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Parse(richTextBox1.Text);
            // Flat one line XML
            string s = doc.ToString(SaveOptions.DisableFormatting);
            richTextBox1.Text = s;
        }

        private void comboBoxStyleSheet_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}