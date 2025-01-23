using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Forms;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Kolibri.net.SilverScreen.Controls.Constants;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class SortMultimediaDesktopForm 
    {
        MultimediaType _type;
        UserSettings _settings;

        public DirectoryInfo SourcePath { get { return new DirectoryInfo(textBoxSource.Text); } }
        public DirectoryInfo DestinationPath { get { return new DirectoryInfo(textBoxDestination.Text); } }


        public SortMultimediaDesktopForm(MultimediaType type, UserSettings settings)
        {
            _type = type;
            _settings = settings;
            InitializeComponent();
            Init();
        }
   
      
        internal void Init(string formType = null)
        {
            string sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string destPath = Environment.CurrentDirectory;

            toolStripStatusLabelFilnavn.Text = string.Empty;
            switch (_type)
            {
                case MultimediaType.movie:                    
                case MultimediaType.Movies:
                    sourcePath = _settings.UserFilePaths.MoviesSourcePath;
                    destPath = _settings.UserFilePaths.MoviesDestinationPath;
                    break;
                case MultimediaType.Series:
                    break;
                case MultimediaType.Audio:
                    break;
                case MultimediaType.Pictures:
                    sourcePath = _settings.UserFilePaths.MoviesSourcePath;
                    destPath = _settings.UserFilePaths.MoviesDestinationPath;
                    break;
                default:
                    break;
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

        public void buttonExecute_Click(object sender, EventArgs e)
        {
            try
            {
                var extList = Kolibri.net.Common.Utilities.MovieUtilites.MoviesCommonFileExt();

                var allowedExtensions = extList.ConvertAll(d => d.ToUpper());
                var jall = SourcePath.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                FileInfo[] collection = (from fi in jall
                                         where allowedExtensions.Contains(fi.Extension.Replace(".", string.Empty).ToUpper())
                                         select fi)
                                         .ToArray();

                bool xcopy = checkBoxXCopyAndDelete.Checked;


                if (collection.Length > 0 && xcopy)
                {
                    if (MessageBox.Show($"https://adamtheautomator.com/robocopy/\r\nDet er noe bøll med nettverksdrev, så Xcopy blir brukt for det.\r\nDestination folder will be purged if it exists!!!!\r\n The files will be moved to location {DestinationPath.FullName}. \r\n Continue?", "USE WITH CARE - Move files", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                        // Kolibri.Common.Utilities.FolderUtilities.XCopy(SourcePath, DestinationPath, true);
                        MoveFilesRoboCopy(collection, DestinationPath);
                    else
                        SetLabelText("Aborted XCopy files");
                }
                else
                {
                    MoveFiles(collection, DestinationPath);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message; SetLabelText(ex.Message);
            }
        }
        private void MoveFilesRoboCopy(FileInfo[] collection, DirectoryInfo destinationPath)
        {

            var numerrors = 0;


            foreach (var file in collection)
            {
                if (file.Exists && file.Directory.Exists)
                {
                    int year = Kolibri.net.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                    var destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, year.ToString(), file.Directory.Name));

                    try
                    {
                        String info = $"Moving {file.Directory.FullName} --> {destination.FullName} -- please wait";
                        SetLabelText(info);

                        // Directory.Move(file.Directory.FullName, destination .FullName);                     
                        // Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(file.Directory.FullName, destination.FullName, UIOption.AllDialogs);
                        if (FolderUtilities.MoveDirectory(file.Directory, destination))
                        {

                            try
                            {
                                //foreach (var source in file.Directory.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                                //{
                                //    FileInfo infoTest = (destination.GetFiles(source.Name, System.IO.SearchOption.AllDirectories)).FirstOrDefault();

                                //    if (infoTest.Exists)
                                //    {
                                //        source.Delete();
                                //        SetLabelText($"File moved {info}");
                                //    }
                                //}

                            }

                            catch (Exception ex)
                            {
                                numerrors++;
                                SetLabelText($"error: {ex.Message}");
                                Thread.Sleep(10);
                            }
                        }
                        else { throw new Exception("ExitCode fail"); }
                        Application.DoEvents();
                        SetLabelText($"Done: {info}");
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message; SetLabelText(ex.Message);
                        numerrors++;
                    }
                }

                try
                {
                    Kolibri.net.Common.Utilities.FileUtilities.DeleteEmptyDirs(file.Directory.Parent);
                }
                catch (Exception)
                { }
            }
            SetLabelText($"Moving operation is completed, number of accumilated errors: {numerrors} ");
        }

        private void MoveFiles(FileInfo[] collection, DirectoryInfo destinationPath)
        {

            var numerrors = 0;


            foreach (var file in collection)
            {
                if (file.Exists && file.Directory.Exists)
                {
                    int year = Kolibri.net.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                 
                    var destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, year.ToString(), file.Directory.Name));

                    try
                    {
                        String info = $"Moving {file.Directory.FullName} --> {destination.FullName} -- please wait";
                        SetLabelText(info);

                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(file.Directory.FullName, destination.FullName, UIOption.AllDialogs);

                        try
                        {
                            foreach (var source in file.Directory.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                            {
                                FileInfo infoTest = (destination.GetFiles(source.Name, System.IO.SearchOption.AllDirectories)).FirstOrDefault();

                                if (infoTest.Exists)
                                {
                                    Thread.Sleep(10);
                                    source.Delete();
                                    SetLabelText($"File moved {info}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            numerrors++;
                            SetLabelText($"error: {ex.Message}");

                        }
                        Application.DoEvents();
                        SetLabelText($"Done: {info}");
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message; SetLabelText(ex.Message);
                        numerrors++;
                    }
                }
                try
                {
                    Kolibri.net.Common.Utilities.FileUtilities.DeleteEmptyDirs(file.Directory.Parent);
                }
                catch (Exception)
                { }
            }
            SetLabelText($"Moving operation is completed, number of accumilated errors: {numerrors} ");
        }
        #region Fast? finn ut
        
        

        #endregion


        #region ikke så effektiv MoveFilesRecursive
        private void MoveFilesRecursive(FileInfo[] collection, DirectoryInfo destinationPath)
        {

            var numerrors = 0;

            var list = new List<string>();
            foreach (var file in collection)
            {
                if (file.Exists && file.Directory.Exists)
                {
                    int year = Kolibri.net.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                    var destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, year.ToString(), file.Directory.Name));
                    try
                    {
                        String info = $"Moving {file.Directory.FullName} --> {destination.FullName} -- please wait";
                        // Directory.Move(file.Directory.FullName, destination .FullName);
                        SetLabelText(info);
                        // Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(file.Directory.FullName, destination.FullName, UIOption.AllDialogs);
                        Kolibri.net.Common.FormUtilities.Forms.FolderUtilities.CopyDirectoryAndFiles(file.Directory.FullName, destination.FullName, true);
                        try
                        {
                            foreach (var source in file.Directory.GetFiles("*.*", System.IO.SearchOption.AllDirectories))
                            {
                                FileInfo infoTest = (destination.GetFiles(source.Name, System.IO.SearchOption.AllDirectories)).FirstOrDefault();

                                if (infoTest.Exists)
                                {
                                    try
                                    {
                                        if(!list.Contains(file.Directory.FullName)) 
                                            list.Add(file.Directory.Parent.FullName);

        Thread.Sleep(100) ;

                                        if (source.Exists)
                                        {
                                            File.SetAttributes(source.FullName, FileAttributes.Normal);
                                            File.Delete(source.FullName);
                                        }
 
                                    SetLabelText($"File moved {info}");
                                    }
                                    catch (Exception)
                                    { 
                                    }
                            
                                }
                            } 
                        }
                        catch (Exception ex)
                        {
                            numerrors++;
                            SetLabelText($"error: {ex.Message}");
                            Thread.Sleep(10);
                        }
                        Application.DoEvents();
                        SetLabelText($"Done: {info}");
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message; SetLabelText(ex.Message);
                        numerrors++;
                    }
                }
            }
            try
            {
                foreach (var item in list)
                {
                    Kolibri.net.Common.Utilities.FileUtilities.DeleteEmptyDirs(new DirectoryInfo(item));
                }
                
            }
            catch (Exception)
            { }

            SetLabelText($"Moving operation is completed, number of accumilated errors: {numerrors} ");
        }
        #endregion

        private void button_Click(object sender, EventArgs e)
        {
            string text = null;
            if (sender.Equals(buttonSource))
                text = textBoxSource.Text;
            else
                text = textBoxDestination.Text;

            DirectoryInfo folder = null; 


            folder = FileUtilities.LetOppMappe(text);////.Replace(@"\\", @"\"));


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

                if (_type.Equals(MultimediaType.movie) || _type.Equals(MultimediaType.Movies))
                    _settings.UserFilePaths.MoviesSourcePath = sourcePath;
                else if (_type.Equals(MultimediaType.Pictures))
                    _settings.UserFilePaths.PicturesSourcePath = sourcePath;
                else if (_type.Equals(MultimediaType.Series))
                    _settings.UserFilePaths.SeriesSourcePath = sourcePath;
                _settings.Save();
            }
        }
        public void SetDestination(string destPath)
        {
            if (Directory.Exists(destPath))
            {
                textBoxDestination.Text = destPath;

                if (_type.Equals(MultimediaType.movie)||_type.Equals(MultimediaType.Movies))
                    _settings.UserFilePaths.MoviesDestinationPath = destPath;
                else if (_type.Equals(MultimediaType.Pictures))
                    _settings.UserFilePaths.PicturesDestination = destPath;
                else if (_type.Equals(MultimediaType.Series))
                    _settings.UserFilePaths.SeriesDestination = destPath;
                _settings.Save();
            }
        }
        private   void  SetLabelText(string s) {
            try
            {
               toolStripStatusLabel1.Text = s;
            }
            catch (Exception ex)
            {
            }

        
        }
    }
}