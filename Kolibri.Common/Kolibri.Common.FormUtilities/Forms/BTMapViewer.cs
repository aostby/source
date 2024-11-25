
using Kolibri.Common.Utilities.Controller;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
 
namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class BTMapViewer : Form
    {
        static string m_currentMapFile = string.Empty;

        public BTMapViewer(FileInfo currentMapFile)
        {
            m_currentMapFile = currentMapFile.FullName;
            if(Directory.Exists(m_currentMapFile))
                try
                {
                    m_currentMapFile = Directory.GetFiles(m_currentMapFile, "*.btm", SearchOption.AllDirectories)[0];
                }
                catch (Exception)
                { 
                }
            InitializeComponent();
        }

        private void FileMenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(m_currentMapFile))
                webBrowser1.DocumentText = LoadAndTransformMap(m_currentMapFile);
            statusStripCurrentMapFile.Text = Path.GetFileName(m_currentMapFile);
        }

        private static string LoadAndTransformMap(string path)
        {
            string output = string.Empty;
            try
            {
                string ext = Path.GetExtension(path).ToUpper().TrimStart('.');
                XPathDocument xpathDoc = new XPathDocument(path);
                XslTransform xslTrans = new XslTransform();


                FileInfo msxslInfo = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", string.Format( "transform{0}.xslt", ext.ToUpper())));//"BizTalkMapDocumenterHTML.xslt"));
                if (!msxslInfo.Exists)
                {
                    if (!msxslInfo.Directory.Exists) msxslInfo.Directory.Create();
                    switch (ext)
                    {
                        case "BTM": File.WriteAllText(msxslInfo.FullName, ResourceController.GetResouceObject("BizTalkMapDocumenterHTML") as string); break;

                        case "XSD": File.WriteAllText(msxslInfo.FullName, ResourceController.GetResouceObject("xs3p") as string); break;
                        default:
                            break;
                    }
                }

                xslTrans.Load(msxslInfo.FullName);

                MemoryStream buffer = new MemoryStream();
                StreamWriter sw = new StreamWriter(buffer);

                xslTrans.Transform(xpathDoc, null, sw);

                byte[] chars = buffer.ToArray();
                buffer.Dispose();
                  output = Encoding.UTF8.GetString(chars);

                m_currentMapFile = path;
            }
            catch (Exception)
            {

            }
            return output;
        }

        private void FileMenuOpen_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                webBrowser1.DocumentText = LoadAndTransformMap(openFileDialog1.FileName);
                statusStripCurrentMapFile.Text = Path.GetFileName(m_currentMapFile);
            }
        }

        private void FileMenuSaveAsHtml_Click(object sender, EventArgs e)
        {
            if (m_currentMapFile != string.Empty)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "html page|*.html";
                saveFileDialog1.Title = "Save as html page";
                saveFileDialog1.FileName =
                    Path.GetFileNameWithoutExtension(m_currentMapFile) + "_MapDocumentation.html";
                DialogResult dr = saveFileDialog1.ShowDialog();

                if (dr == DialogResult.OK && saveFileDialog1.FileName != string.Empty)
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog1.OpenFile();

                    StreamWriter wText = new StreamWriter(fs);
                    wText.Write(webBrowser1.DocumentText);

                    fs.Close();
                }

                saveFileDialog1.Dispose();
            }
        }

        private void FileMenuPrint_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void FileMenuPageSetup_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPageSetupDialog();
        }

        private void ContextMenu_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Copy", false, null);
        }

        private void SettingsMenuRegister_Click(object sender, EventArgs e)
        {
            string extensionhandler;

            RegistryKey btmKey = Registry.ClassesRoot.OpenSubKey(".btm");

            if (btmKey != null)
            {
                extensionhandler = btmKey.GetValue("").ToString();
            }
            else
            {
                extensionhandler = "BizTalkMapViewer.btm";

                Registry.LocalMachine.CreateSubKey(@"Software\Classes\.btm");
                btmKey = Registry.LocalMachine.OpenSubKey(@"Software\Classes\.btm", true);
                btmKey.SetValue("", extensionhandler);
            }

            // Register our context menu extension
            string menuCommand = string.Format("\"{0}\" \"%L\"",
                                   Application.ExecutablePath);

            FileShellExtension.Register(extensionhandler, "Simple Context Menu",
                            "Open with BizTalk Map Viewer", menuCommand);

        }

        private void SettingsMenuUnregister_Click(object sender, EventArgs e)
        {
            RegistryKey btmKey = Registry.ClassesRoot.OpenSubKey(".btm");

            if (btmKey != null)
            {
                string extensionhandler = btmKey.GetValue("").ToString();
                FileShellExtension.Unregister(extensionhandler, "Simple Context Menu");
            }
        }

        private void FileMenuPrintPreview_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Copy", false, null);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            FileMenuOpen_Click(null, null);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            FileMenuSaveAsHtml_Click(null, null);
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            FileMenuPrint_Click(null, null);
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            copyToolStripButton_Click(null, null);
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
                if (e.Url.IsFile) 
                {       
                    // Prevent navigation  
                    e.Cancel = true;

                    if (Path.GetExtension(e.Url.LocalPath).Equals(".btm", StringComparison.CurrentCultureIgnoreCase))
                    {
                    webBrowser1.DocumentText = LoadAndTransformMap(e.Url.LocalPath);
                    statusStripCurrentMapFile.Text = Path.GetFileName(m_currentMapFile);
                    }
                } 
        }
    }

    static class FileShellExtension
    {
        public static void Register(string fileType,
               string shellKeyName, string menuText, string menuCommand)
        {
            // create path to registry location
            string regPath = string.Format(@"Software\Classes\{0}\shell\{1}", fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key =
                Registry.CurrentUser.CreateSubKey(string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }

        public static void Unregister(string fileType, string shellKeyName)
        {
            // path to the registry location
            string regPath = string.Format(@"Software\Classes\{0}\shell\{1}", fileType, shellKeyName);

            // remove context menu from the registry
            Registry.CurrentUser.DeleteSubKeyTree(regPath);
        }
    }
}
