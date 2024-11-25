using Kolibri.Common.Utilities;
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

namespace SortPics.Forms
{
    public partial class MALFileForm : Form
    {
        internal enum FormType { MOV, PIC, TV };
        
        private FormType Type { get; set; } 
         
        public DirectoryInfo SourcePath { get { return new DirectoryInfo(textBoxSource.Text); } set { textBoxSource.Text = value.FullName; } }
        public DirectoryInfo DestinationPath { get { return new DirectoryInfo(textBoxDestination.Text); } set { textBoxDestination.Text = value.FullName; } }
        /// <summary>
        /// Ved å angi formtype finner en rett filsti som ble brukt sist
        /// </summary>
        /// <param name="formType">MOV, PIC, TV </param>
        public MALFileForm(string formType = null)
        {
            InitializeComponent();
            Type = FormType.MOV;
            Init(formType);
        } 
      
        internal void Init(string formType = null)
        {
            if (!string.IsNullOrEmpty(formType)){
                try
                {
                    Type = (FormType)Enum.Parse(typeof(FormType), formType, true);

                }
                catch (Exception) { }
            }
         
            string sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string destPath = Environment.CurrentDirectory;

         //   toolStripStatusLabelFilnavn.Text = string.Empty;
            if(Type.Equals(FormType.MOV))
            {
                sourcePath = Properties.Settings.Default.MoviesSourcePath;
                destPath = Properties.Settings.Default.MoviesDestinationPath;
            }
            if (Type.Equals(FormType.PIC))
                {
                sourcePath = Properties.Settings.Default.PicturesSourcePath;
                destPath = Properties.Settings.Default.PicturesDestination;
            }
            if (Type.Equals(FormType.TV))
            {
                sourcePath = Properties.Settings.Default.SeriesSourcePath;
                destPath = Properties.Settings.Default.SeriesDestination;
            }

            if (!Directory.Exists(sourcePath))
                sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
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
                {
                    folder = Kolibri.Common.Utilities.FolderUtilities.LetOppMappe(text);
               
                }
                if (folder != null && folder.Exists)
                {
                    if (sender.Equals(buttonSource))
                    {
                        textBoxSource.Text = folder.FullName.ToString();
                    //if (string.IsNullOrWhiteSpace(textBoxDestination.Text))
                    //{
                    textBoxDestination.Text = folder.FullName.ToString();// new DirectoryInfo(textBoxSource.Text).Parent.FullName;
                        //}
                    }
                    else
                    {
                        textBoxDestination.Text = folder.FullName.ToString();
                    }
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

                if (textBoxSource.Text.Equals(textBoxDestination.Text))
                    textBoxDestination.BackColor = Color.LightYellow;

                if (box == textBoxSource)
                    SetSource(textBoxSource.Text);
                if (box == textBoxDestination)
                    SetDestination(textBoxDestination.Text);


            }
            catch (Exception)
            { }

        }
         
        public void SetSource(string sourcePath)
        {

            if (Directory.Exists(sourcePath))
            {
                textBoxSource.Text = sourcePath;
                if (Type.Equals(FormType.MOV)) 
                    Properties.Settings.Default.MoviesSourcePath = sourcePath;
                else if(Type.Equals(FormType.PIC))
                    Properties.Settings.Default.PicturesSourcePath = sourcePath;
                else if (Type.Equals(FormType.TV))
                    Properties.Settings.Default.SeriesSourcePath = sourcePath;
                Properties.Settings.Default.Save();
            }
        }
        public void SetDestination(string destPath)
        {
            if (Directory.Exists(destPath))
            {
                textBoxDestination.Text = destPath;

                if (Type.Equals(FormType.MOV))
                    Properties.Settings.Default.MoviesDestinationPath = destPath;
                else if (Type.Equals(FormType.PIC))
                    Properties.Settings.Default.PicturesDestination = destPath;
                else if (Type.Equals(FormType.TV))
                    Properties.Settings.Default.SeriesDestination = destPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
