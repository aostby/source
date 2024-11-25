using Kolibri.Common.Utilities;
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

namespace SortPics.Forms
{
    public partial class SortMoviesDesktopForm : MALSourceDestinationForm
    {
        public SortMoviesDesktopForm()
        {
            InitializeComponent();
            Init("Movies");
            CheckBox xcopy = new CheckBox() { Checked = false, Text = "RoboCopy and Delete from source folder", Name = "xcopyanddelete", Width = 500 };
            GetCusfomPanel().Controls.Add(xcopy);
        }


        public override void buttonExecute_Click(object sender, EventArgs e)
        {
            try
            {
                var extList = Kolibri.Common.Utilities.FileUtilities.MoviesCommonFileExt();

                var allowedExtensions = extList.ConvertAll(d => d.ToUpper()); ;
                /*var files = Directory
                    .GetFiles(folder)
                    .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                    .ToList();
                */
                var jall = SourcePath.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                FileInfo[] collection = (from fi in jall
                                         where allowedExtensions.Contains(fi.Extension.Replace(".", string.Empty).ToUpper())
                                         select fi)
                                         .ToArray();

                bool xcopy = false;
                foreach (Control c in GetCusfomPanel().Controls)
                {
                    if ((c is CheckBox) && ((CheckBox)c).Checked)
                        xcopy = true;
                }


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
                    int year = Kolibri.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
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
                    Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(file.Directory.Parent);
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
                    int year = Kolibri.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                 
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
                    Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(file.Directory.Parent);
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
                    int year = Kolibri.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                    var destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, year.ToString(), file.Directory.Name));
                    try
                    {
                        String info = $"Moving {file.Directory.FullName} --> {destination.FullName} -- please wait";
                        // Directory.Move(file.Directory.FullName, destination .FullName);
                        SetLabelText(info);
                        // Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(file.Directory.FullName, destination.FullName, UIOption.AllDialogs);
                        Kolibri.Common.Utilities.FolderUtilities.CopyDirectoryAndFiles(file.Directory.FullName, destination.FullName, true);
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
                    Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(new DirectoryInfo(item));
                }
                
            }
            catch (Exception)
            { }

            SetLabelText($"Moving operation is completed, number of accumilated errors: {numerrors} ");
        }
        #endregion

    }
}