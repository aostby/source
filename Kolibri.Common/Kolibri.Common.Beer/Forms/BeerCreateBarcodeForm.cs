using System.Drawing.Printing;
using Kolibri.Common.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using Kolibri.Common.Utilities;
using System.Web;

namespace Kolibri.Common.Beer.Forms
{
    public partial class BeerCreateBarcodeForm : Form
    {
        private XDocument _wordPressXDoc;
        private FileInfo _wordpressLinks;
        private FileInfo _uranusLogo;
        private DirectoryInfo _etikettPath;
        private Dictionary<string, string> storyDic;
        private Dictionary<string, string> dateDic;
        private ColorDialog _colorDialog;
        Color backColor = Color.White;
        Color foreColor = Color.Black;
        FileInfo _lastSavedFilename;
        public BeerCreateBarcodeForm()
        {
            try
            {
                _colorDialog = new ColorDialog();
                _etikettPath = new DirectoryInfo(@"C:\Users\asoes\Documents\BeerXML\Etiketter\");
                if(!_etikettPath.Exists)
                    _etikettPath = new DirectoryInfo(@"E:\DEV\Etiketter\");
                if (_wordpressLinks == null)
                    _wordpressLinks = new FileInfo(Path.Combine(Application.StartupPath, "uranusgarage.WordPress.2021.xml"));
                try
                {
                    if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.BeerWordPressExport))
                        if (File.Exists(Properties.Settings.Default.BeerWordPressExport))
                        {
                            _wordpressLinks = new FileInfo(Properties.Settings.Default.BeerWordPressExport);
                        }
                }
                catch (Exception) { }
                _uranusLogo = new FileInfo(Path.Combine(Application.StartupPath, "uranus_garage_logo.jpg"));

                InitializeComponent();
                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().GetType().Name);
            }
        }

        private void Init()
        {
            try
            {
                comboBoxName.DataSource = null;
                comboBoxDirectory.DataSource = null;

                _wordPressXDoc = XDocument.Load(_wordpressLinks.FullName);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                storyDic = new Dictionary<string, string>();
                dateDic = new Dictionary<string, string>();

                foreach (XElement element in _wordPressXDoc.Root.GetElement("channel").GetElements("link").Reverse())
                {
                    string title = element.Parent.GetElement("title").Value;
                    string url = element.Value;

                    dic[url] = title;

                    try
                    {
                        storyDic[element.Value] = element.Parent.GetElement("encoded").Value;
                        try
                        {
                            foreach (var item in element.Parent.GetElements("comment"))
                            {
                                storyDic[element.Value] += $"{Environment.NewLine}Comment: {Environment.NewLine}{item.GetElementValue("comment_author")}: {item.GetElementValue("comment_content")}";
                            }
                        }
                        catch (Exception ex)
                        { }  
                        dateDic[title] = element.Parent.GetElement("pubDate").Value; 
                    }
                    catch (Exception)
                    { }
                }

                comboBoxName.DataSource = new BindingSource(dic, null);
                comboBoxName.DisplayMember = "Value";
                comboBoxName.ValueMember = "Key";

                comboBoxDirectory.DataSource = _etikettPath.GetDirectories().OrderByDescending(f => f.LastWriteTime).ToArray();
                comboBoxDirectory.DisplayMember = "Name";
                comboBoxDirectory.ValueMember = "FullName";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            } 
        }

        private void comboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelDate.Text = string.Empty;
            try
            {
                try
                {
                    //if (!this.Visible) return;

                    radioButton_CheckedChanged(null, null);
                }
                catch (Exception) { }

                try
                {
                    var jall = (System.Collections.Generic.KeyValuePair<string, string>)comboBoxName.SelectedItem;

                    linkLabelUrl.Text = jall.Key; //jall.Value;
                    linkLabelUrl.Tag = jall.Key;
                    #region tekst for ølet
                    try
                    {
                        richTextBox1.Clear();

                        if (storyDic.Keys.Contains(jall.Key))
                        {
                            WebBrowser browser = new WebBrowser();

                            browser.Navigate("about:blank");
                            HtmlDocument doc = browser.Document;
                            doc.Write(String.Empty);
                            browser.DocumentText = $"<html><body>{storyDic[jall.Key]}</body></html>";
                            while (browser.ReadyState != WebBrowserReadyState.Complete)
                            {
                                Thread.Sleep(200);
                                Application.DoEvents();
                            }
                            browser.Document.ExecCommand("SelectAll", false, null);
                            browser.Document.ExecCommand("Copy", false, null);
                            richTextBox1.Paste();
                            richTextBox1.Parent.Update();
                        }
                    }
                    catch (Exception)
                    { }
                    #endregion

                    #region velg folder
                    try
                    {
                        labelDate.Text = dateDic[comboBoxName.Text];
                    }
                    catch (Exception)
                    { }
                    int test = -1;
                    string value = comboBoxName.Text;
                    bool match = false;
                    try
                    { 
                        foreach (var item in comboBoxDirectory.Items)
                        {if (match) break;
                            var arr = comboBoxName.Text.Split(' ');

                            foreach (var del in arr)
                            {
                                if (del.ToUpper().Equals(item.ToString()))
                                {
                                    value = item.ToString();match = true; break;
                                }
                                else
                                {

                                    var tall = StringUtilities.LevenshteinDistance(del, item.ToString());
                                    if (tall > test)
                                    {
                                        test = tall;
                                        value = item.ToString();
                                    }
                                }
                            }
                        }
                        comboBoxDirectory.SelectedIndex = comboBoxDirectory.FindStringExact(value); 
                    }
                    catch (Exception)
                    {   }

                    #endregion
                }
                catch (Exception ex)
                { }
            }
            catch (Exception)
            { }
        }

        private void linkLabelOpenTagUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start((sender as LinkLabel).Tag.ToString());
            }
            catch (Exception ex)
            {
            }
        }
        private void linkLabelChooseColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (_colorDialog.ShowDialog() == DialogResult.OK)
                {

                    (sender as LinkLabel).LinkColor = _colorDialog.Color;

                    if (sender.Equals(linkLabelBackColor))
                        backColor = _colorDialog.Color;
                    if (sender.Equals(linkLabelForeColor))
                        foreColor = _colorDialog.Color;
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pictureBoxCode.Image = null;
                var beer = (System.Collections.Generic.KeyValuePair<string, string>)comboBoxName.SelectedItem;
                #region type PDF147
                if (radioButtonPDF417.Checked)
                {
                    var img = ImageUtilities.GetBarcodePDF417(new Uri(beer.Key), foreColor, backColor);
                    if (radioButtonPlain.Checked || (radioButtonLogo.Checked) || (radioButtonStyled.Checked))
                    {
                        pictureBoxCode.Image =  ImageUtilities.Crop(img as Bitmap);
                        if (checkBoxVertical.Checked)
                            pictureBoxCode.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                }
                #endregion
                #region type QR
                if (radioButtonQR.Checked)
                {
                    if (radioButtonPlain.Checked)
                    {
                        pictureBoxCode.Image = ImageUtilities.GetBarcodeQR(new Uri(beer.Key));
                    }
                    else if (radioButtonLogo.Checked)
                    {
                        pictureBoxCode.Image = ImageUtilities.GetBarcodeQR(new Uri(beer.Key), _uranusLogo);
                    }
                    else if (radioButtonStyled.Checked)
                    {
                        pictureBoxCode.Image = ImageUtilities.GetBarcodeQR(new Uri(beer.Key), beer.Value);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void ImageWithQR_Click(bool lookUp = false, Image img=null)
        {

            try
            {
                string source = (comboBoxFile.SelectedItem as FileInfo).FullName;
                if (lookUp)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    var filetypes = FileUtilities.PictureFileExt().ToArray();
                    ofd.Filter = FileUtilities.GetFileDialogFilter(filetypes);
                    ofd.FilterIndex = 2;
                    ofd.InitialDirectory = (comboBoxDirectory.SelectedItem as DirectoryInfo).FullName;
                    ofd.FileName = (comboBoxFile.SelectedItem as FileInfo).FullName;
                    ofd.InitialDirectory = (comboBoxFile.SelectedItem as FileInfo).Directory.FullName;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        source = ofd.FileName;
                    }
                    else throw new FileNotFoundException("Ingen fil valgt fra valgt filsti!");
                }
                //Sett bildet til ønsket strekkode
                radioButton_CheckedChanged(null, null);

                //Dersom det er PDF417, kan du beholde layout, hvis noe annet, sett size til 100x100
                using (Image qr = radioButtonPDF417.Checked ? (pictureBoxCode.Image.Clone() as Image) : ImageUtilities.FixedSize((pictureBoxCode.Image.Clone() as Image), 100, 100))
                {
                    Image si;
                    if (img == null) {
                    si = ImageUtilities.OpenImage(source).Clone() as Image; }
                    else { si = img; }

                    //Hvis strekoden er er vertikalt, må en endre litt på plasseringen så alt blir med i bildet
                    if (radioButtonPDF417.Checked && checkBoxVertical.Checked)
                    {
                        Image result = WaterMark.CreateWaterMark(si, qr, si.Width - qr.Width, si.Height - qr.Height); //Convert.ToInt32( si.Width/2), Convert.ToInt32(si.Height - qr.Height));
                        pictureBoxCode.Image = result.Clone() as Image;
                        return;
                    }


                    Size newSize = new Size((si.Width / qr.Height) * 100, (si.Height / qr.Height) * 100);
                    if (newSize.Width < qr.Width)
                        newSize = si.Size;

                    using (Image pre = ImageUtilities.FixedSize(si, newSize.Width, newSize.Height))
                    {
                        Size size = pre.Size;
                        Image result = WaterMark.CreateWaterMark(pre, qr, (size.Width - qr.Width), (size.Height - qr.Height));
                        pictureBoxCode.Image = result.Clone() as Image;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void comboBoxDirectory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBoxFile.DataSource = null;

                DirectoryInfo dir = comboBoxDirectory.SelectedItem as DirectoryInfo;
                List<string> filter = Kolibri.Common.Utilities.FileUtilities.PictureFileExt().Take(4).ToList();

                var files = Kolibri.Common.Utilities.FileUtilities.GetFiles(dir, "*." + String.Join("|*.", filter.ToArray()), true);
                comboBoxFile.DataSource = files;
                comboBoxFile.DisplayMember = "Name";
                comboBoxFile.ValueMember = "FullName";
                int test = 0;
                string searchFor = comboBoxName.Text;
                string fileName = string.Empty;
                try
                {
                    foreach (var item in files)
                    {
                        var first = searchFor.Split(' ').FirstOrDefault();
                        if (item.Name.ToUpper().Contains(first.ToUpper()))
                        {
                            comboBoxFile.SelectedIndex = comboBoxFile.FindStringExact(item.Name);
                            break;
                        }
                    }
                }
                catch (Exception)
                { }
            }
            catch (Exception ex)
            { }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();

                var filetypes = new List<string>() { "jpg" }.ToArray();
                sfd.Filter = Kolibri.Common.Utilities.FileUtilities.GetFileDialogFilter(filetypes);
                sfd.FilterIndex = 1;
                sfd.InitialDirectory = comboBoxDirectory.Text;
                sfd.FileName = new DirectoryInfo(sfd.InitialDirectory).Name + "_QR.jpg";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxCode.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    _lastSavedFilename = new FileInfo(sfd.FileName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void buttonUpload_Click(object sender, EventArgs e)
        {
            string username = "problembar";
            string password = "BMWa-V64";
            try
            {
                if (_lastSavedFilename==null || !_lastSavedFilename.Exists)
                    throw new FileNotFoundException($"Ingen fil er funnet som kan lastes opp. Klikk {buttonSave.Text} to upload a file");

                var contents = ByteUtilities.ReadFile(_lastSavedFilename.FullName);
                string url = $@"sftp://{username}:{password}@ftp.domeneshop.no/www/uranus/BeerXML/Images/" + _lastSavedFilename.Name;
                Ftp.Upload(url, contents);
                DialogResult res = MessageBox.Show("File uploaded. Do you want to open webpage?", _lastSavedFilename.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    Process.Start("https://www.problembar.net/uranus/BeerXML/Images/");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        #region simple printpreview

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // rævva dritt  -- Kolibri.Utilities.Forms.PrintPreviewForm.PrintPreview(pictureBox1.Image as Image);

                string fileName = Kolibri.Common.Utilities.FileUtilities.GetTempFile("jpg").FullName;
                pictureBoxCode.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                Kolibri.Common.Utilities.Forms.PrintPreviewForm.PrintPreview(new FileInfo(fileName));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }

        }
        #endregion;

        private void comboBoxFile_SelectedIndexChanged(object sender, EventArgs e)
        {bool uranus = false;
            if (comboBoxFile.SelectedItem != null)
            {
                {
                    if ((comboBoxFile.SelectedItem as FileInfo).FullName.EndsWith("uranus_garage_logo.jpg")
                        || checkBoxHeader.Checked) 
                    {
                        var temp = Image.FromFile((comboBoxFile.SelectedItem as FileInfo).FullName);
                        pictureBoxPreview.Image = WaterMark.AppendHeaderAndFooterText(temp, HttpUtility.HtmlDecode( comboBoxName.Text), "5% ABV");
                        pictureBoxCode.Image = pictureBoxPreview.Image; 
                        uranus = true;  
                        
                    }
                    else
                        pictureBoxPreview.Image = Image.FromFile((comboBoxFile.SelectedItem as FileInfo).FullName);
                    pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                    
                    try
                    {
                        ImageWithQR_Click(false, uranus?pictureBoxCode.Image:  null );

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().Name);
                    }
                }
            }
        }

        private void buttonSourcePath_Click(object sender, EventArgs e)
        {
            try
            {
                var temp = Kolibri.Common.Utilities.FileUtilities.LetOppMappe(_etikettPath.FullName);
                if (temp.Exists)
                {
                    _etikettPath = temp;
                    Init();
                }
                else throw new FileNotFoundException("Ingen fil valgt fra valgt filsti!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }



        }

        private void radioButtonTypeCode_CheckedChanged(object sender, EventArgs e)
        {
            //if (sender.Equals(radioButtonQR))
            //{
            //    groupBoxAppearance.Enabled = true;
            //}
            //else
            //{
            //    //radioButtonPlain.Checked = true;
            //    //groupBoxAppearance.Enabled = false;
            //    radioButton_CheckedChanged(radioButtonPDF417, null);
            //}

        }

        private void richTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                string text = richTextBox1.Text;
                Kolibri.Common.FormUtilities.OutputDialogs.ShowRichTextBoxDialog(comboBoxName.Text, text, this.Size);
            }
            catch (Exception)
            {
            }
        }

        private void buttonOpenXMLPath_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog sfd = new OpenFileDialog();

                var filetypes = new List<string>() { "xml" }.ToArray();
                sfd.Filter = Kolibri.Common.Utilities.FileUtilities.GetFileDialogFilter(filetypes);
                sfd.FilterIndex = 1;
                sfd.InitialDirectory = _wordpressLinks.Directory.FullName;
                sfd.FileName = _wordpressLinks.Name;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _wordpressLinks = new FileInfo(sfd.FileName);
                    try
                    {
                        Properties.Settings.Default.BeerWordPressExport = _wordpressLinks.FullName;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception) { }
                    Init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }
        }

        private void buttonGetLatest_Click(object sender, EventArgs e)
        {
            try
            {
                Controller.UranusWPController uContr = new Controller.UranusWPController();
                var test = uContr.GetWebPage();
                test = uContr.ConvertHTML2WPxml();

                FileInfo info = new FileInfo(Path.Combine(_wordpressLinks.Directory.FullName, DateTime.Now.ToString("yyyy-MM-dd") + "_UranusWP.xml"));
                FileUtilities.WriteStringToFile(test, info.FullName, Encoding.UTF8);
                _wordpressLinks = info;
                try
                {
                    Properties.Settings.Default.BeerWordPressExport = _wordpressLinks.FullName;
                    Properties.Settings.Default.Save();
                }
                catch (Exception) { }
                Init();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);

            }
        }

        private void buttonUploadPDF_Click(object sender, EventArgs e)
        {

            string username = "problembar";
            string password = "BMWa-V64";
            try
            {

                OpenFileDialog ofd = new OpenFileDialog();

                var filetypes = new List<string>() { "pdf" }.ToArray();
                ofd.Filter =  FileUtilities.GetFileDialogFilter(filetypes);
                ofd.FilterIndex = 1;
                ofd.InitialDirectory = comboBoxDirectory.Text;
                if (_lastSavedFilename.Exists)
                    ofd.FileName = _lastSavedFilename.Name.Replace(_lastSavedFilename.Extension, ".pdf");

                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    FileInfo info = new FileInfo(ofd.FileName);

                    if (info == null || !info.Exists)
                        throw new FileNotFoundException($"Ingen fil er funnet som kan lastes opp. Klikk {buttonPrint.Text} to creata an upload pdf file");

                    var contents = ByteUtilities.ReadFile(info.FullName);
                    string url = $@"ftp://{username}:{password}@scp.domeneshop.no/www/uranus/BeerXML/Images/" + info.Name;
                    Ftp.Upload(url, contents);
                    DialogResult res = MessageBox.Show("File uploaded. Do you want to open webpage?", info.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        Process.Start("https://www.problembar.net/uranus/BeerXML/Images/");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
