using Kolibri.Common.Utilities;
using Kolibri.Common.FormUtilities;
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
using System.Xml.Linq;
using Kolibri.Common.Beer.Controller;
using System.Security.Policy;
using DevExpress.XtraReports;

namespace Kolibri.Common.Beer.Forms
{
    public partial class BeerXmlDisplayForm : Form
    {
        private MALFileForm form;
        public BeerXmlDisplayForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //https://blogg.bryggselv.no/category/oppskrifter/

            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                textBoxSource.Text = Properties.Settings.Default.BeerXMLPath;
            }
            catch (Exception)
            { }

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
                    buttonOpenDirS.BackgroundImage = image;
                    buttonOpenDirS.BackgroundImageLayout = ImageLayout.Stretch;

                }


                image = Icons.GetSystemIcon(Icons.SHSTOCKICONID.SIID_RECYCLER, Icons.SHGSI.SHGSI_ICON | Icons.SHGSI.SHGSI_SMALLICON).ToBitmap();
                if (image != null)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    buttonRefresh.Text = string.Empty;
                    buttonRefresh.BackgroundImage = image;
                    buttonRefresh.BackgroundImageLayout = ImageLayout.Stretch;

                }
                #endregion
            }
            catch (Exception ex) { }
        }

        private void button_Click(object sender, EventArgs e)
        {
            string text = null;
            if (sender.Equals(buttonSource))
                text = textBoxSource.Text;

            DirectoryInfo folder = null;
            {
                folder = FolderUtilities.LetOppMappe(text);
            }
            if (folder != null && folder.Exists)
            {
                if (sender.Equals(buttonSource))
                {
                    textBoxSource.Text = folder.FullName.ToString();

                }
            }
        }

        private void buttonOpenDir_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox textBox = null;
                if (sender.Equals(buttonOpenDirS)) { textBox = textBoxSource; }


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
                if (box == textBoxSource)
                    SetSource(textBoxSource.Text);
            }
            catch (Exception)
            { }
        }

        public void SetSource(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                textBoxSource.Text = sourcePath;
                try
                {
                    Properties.Settings.Default.BeerXMLPath = sourcePath; ;
                }
                catch (Exception)
                { }
                Properties.Settings.Default.Save();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                var xsltName = "UranusBEER01.xsl";

                var filePath = (comboBox1.SelectedItem as FileInfo);
                var xslt2 = Common.FormUtilities.Controller.TransFormController.FindXSLTFile(xsltName);
                if (radioButtonExternal.Checked)
                    XSLTTransform.TransformAndShow(filePath, xslt2, "html");
                if (radioButtonLocal.Checked)
                    webBrowser1.DocumentText = XSLTTransform.TransformFiles(xslt2, filePath);
      
            }
            catch (Exception ex)
            { }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox1.DataSource = new DirectoryInfo(textBoxSource.Text).GetFiles("*.xml");
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "FullName";
            }
            catch (Exception)
            { }
        }

        private void BeerXmlDisplayForm_Shown(object sender, EventArgs e)
        {
            try
            {
                buttonRefresh.PerformClick();
            }
            catch (Exception)
            { }
        }


        private void buttonShowFile_Click(object sender, EventArgs e)
        {
            var filePath = (comboBox1.SelectedItem as FileInfo);
            Utilities.FileUtilities.OpenFolderHighlightFile(filePath);
        }
        private void buttonBeerXMLIssuesReport_Click(object sender, EventArgs e)
        {
            var filePath = (comboBox1.SelectedItem as FileInfo);
            var report = CreateReports(new List<FileInfo>() { filePath });
            FormUtilities.OutputDialogs.ShowRichTextBoxDialog("XML issues Report", report.ToString(), this.Size);
        }
        public string CreateReports(List<FileInfo> fileList)
        {
            StringBuilder report = new StringBuilder();

            foreach (var item in fileList)
            {
                StringBuilder locRep = new StringBuilder();
                BeerXMLParser parser = new BeerXMLParser(XDocument.Load(item.FullName));
                locRep.AppendLine(parser.CheckFermentables());
                locRep.AppendLine(parser.CheckHops());
                locRep.AppendLine(parser.CheckYeasts());
                if (!string.IsNullOrWhiteSpace(locRep.ToString()))
                { 
                    locRep.AppendLine(item.FullName); 
                    locRep.AppendLine($"The root element is ok: {parser.RootIsOK()}");

                    if (fileList.Count() > 1)
                    {
                        locRep.AppendLine(string.Empty.PadRight(250, '-'));
                    }
                    report.AppendLine(locRep.ToString());
                }
            }
            return report.ToString();
        }

        private void buttonIssuesReportsAll_Click(object sender, EventArgs e)
        {
            var filePath = (comboBox1.SelectedItem as FileInfo);
            var report = CreateReports(  filePath.Directory.GetFiles("*.xml").ToList());
            FormUtilities.OutputDialogs.ShowRichTextBoxDialog("XML issues Report", report.ToString(), this.Size);
        }
    }
}