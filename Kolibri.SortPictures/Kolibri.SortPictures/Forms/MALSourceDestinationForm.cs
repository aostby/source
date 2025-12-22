using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
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
namespace Kolibri.SortPictures.Forms
{
    public partial class MALSourceDestinationForm : Form
    {
        private string m_formtype = "Picture";

        public string Formtype
        {
            get { return m_formtype; }
            set { m_formtype = value; }
        }
        public DirectoryInfo SourcePath { get { return new DirectoryInfo(textBoxSource.Text); } }
        public DirectoryInfo DestinationPath { get { return new DirectoryInfo(textBoxDestination.Text); } }

        public MALSourceDestinationForm()
        {
            InitializeComponent();
            Init();
        }
        internal void Init (string formType=null)
        {if (formType != null) m_formtype = formType;

            string sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string destPath = Environment.CurrentDirectory;

            toolStripStatusLabelFilnavn.Text = string.Empty;
       
            if (m_formtype.IndexOf("pic", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                sourcePath = Properties.Settings.Default.PicturesSourcePath;
                destPath = Properties.Settings.Default.PicturesDestinationPath;
            } 
 
            if (!Directory.Exists(sourcePath))
                sourcePath =  Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!Directory.Exists(destPath))
                destPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

            SetSource(sourcePath);
            SetDestination(destPath);

            this.StartPosition = FormStartPosition.CenterParent;
            //Lets make it nasty (some forms aren't rendered properly otherwise)
            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;
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
                    buttonOpenDirD.Text = string.Empty;

                    buttonOpenDirS.BackgroundImage = image;
                    buttonOpenDirS.BackgroundImageLayout = ImageLayout.Stretch;
                    buttonOpenDirD.BackgroundImage = image;
                    buttonOpenDirD.BackgroundImageLayout = ImageLayout.Stretch;
                }
                #endregion 
            }
            catch (Exception) { }

            

                }

        private void button_Click(object sender, EventArgs e)
        {
            string text = null;
            if (sender.Equals(buttonSource))
                                text = textBoxSource.Text;
            else
                text = textBoxDestination.Text;     

            DirectoryInfo folder = null;
            //if (checkBoxMTP.Checked)
            //{

            //    try
            //    {
            //        //CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            //        //cofd.IsFolderPicker = true;
            //        //cofd.ShowDialog();
            //        //folder = new DirectoryInfo(cofd.FileName);

            //        var dlg1 = new FolderUtilities.FolderBrowserDialogEx();
            //        dlg1.Description = "Select a folder to extract to:";
            //        dlg1.ShowNewFolderButton = true;
            //        dlg1.ShowEditBox = true;
            //        //dlg1.NewStyle = false;
            //        dlg1.SelectedPath = textBoxSource.Text;
            //        dlg1.ShowFullPathInEditBox = true;
            //        dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            //        // Show the FolderBrowserDialog.
            //        DialogResult result = dlg1.ShowDialog();
            //        if (result == DialogResult.OK)
            //        {
            //            textBoxSource.Text = dlg1.SelectedPath;
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        return;
            //    }

            //}
            //else
            {
                folder = FileUtilities.LetOppMappe(text);////.Replace(@"\\", @"\"));
            }
            if (folder != null && folder.Exists)
            {
                if (sender.Equals(buttonSource))
                {
                    textBoxSource.Text = folder.FullName.ToString();
                    if (string.IsNullOrWhiteSpace(textBoxDestination.Text))
                    {
                        textBoxDestination.Text = new DirectoryInfo(textBoxSource.Text).Parent.FullName;
                    }
                }
                else
                {
                    textBoxDestination.Text = folder.FullName.ToString();
                }
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

                if (textBoxSource.Text.Equals(textBoxDestination.Text))
                    textBoxDestination.BackColor = Color.LightYellow;

                if (box == textBoxSource)
                    SetSource(textBoxSource.Text);
                if (box == textBoxDestination)
                    SetDestination (textBoxDestination.Text);


            }
            catch (Exception)
            { }

        }

        public virtual void buttonExecute_Click(object sender, EventArgs e)
        {
            try
            {
                throw new NotImplementedException("Needs to be overridden");

            }
            catch (Exception ex)
            { string msg = ex.Message; SetLabelText(ex.Message); }
        }

        public void SetLabelText(string text)
        {
           
            toolStripStatusLabelFilnavn.Spring = true;
            toolStripStatusLabelFilnavn.TextAlign = ContentAlignment.MiddleLeft;
            toolStripStatusLabelFilnavn.Text = text;

        }

        public void SetSource(string sourcePath) {

            if (Directory.Exists(sourcePath))
            {
                textBoxSource.Text = sourcePath;
                if (m_formtype.IndexOf("pic", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    Properties.Settings.Default.PicturesSourcePath = sourcePath;
                else 
                    Properties.Settings.Default.PicturesSourcePath = sourcePath;
                Properties.Settings.Default.Save();
            }
        }
        public void SetDestination(string destPath )
        { 
            if (Directory.Exists(destPath))
            {
                textBoxDestination.Text = destPath;
                if (m_formtype.IndexOf("pic", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    Properties.Settings.Default.PicturesDestinationPath = destPath;
                else
                    Properties.Settings.Default.PicturesDestinationPath = destPath;
                Properties.Settings.Default.Save();
            }
        }  
        private void buttonOpenDir_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox textBox = null;
                if (sender.Equals(buttonOpenDirS)) { textBox = textBoxSource; }
                else if (sender.Equals(buttonOpenDirD)) { textBox = textBoxDestination; }

                Process.Start(textBox.Text);
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabelFilnavn.Text = ex.Message;
            }
        }

        public Panel GetCusfomPanel() 
        {
            return this.panelCustom;
        }


    }
}