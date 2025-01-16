using Kolibri.Common.Beer.Forms;
using Kolibri.Common.FormUtilities;
using Kolibri.Common.MovieAPI.Controller;
using Kolibri.Common.MovieAPI.Entities;
using Kolibri.Common.MovieAPI.Forms;
using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Forms;
using Kolibri.Common.Vinmonopolet;
using MoviesFromImdb;
using SortPics.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SortPics.Forms
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-determine-the-active-mdi-child
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected void MDIChildNew_Click(object sender, EventArgs e)
        {
            Form newMDIChild = null;
            try
            {
                if (sender.Equals(sorterBilderToolStripMenuItem)) { newMDIChild = new SortPicsDesktopForm(); }
                else if (sender.Equals(watchlistToolStripMenuItem)) { newMDIChild = new MoviesFromImdb.MovieForm(); }
                else if (sender.Equals(kODIImportToolStripMenuItem)) { newMDIChild = new KodiForm(); }
                else if (sender.Equals(browseMoviesToolStripMenuItem)) { newMDIChild = new BrowseMoviesForm(); }
                else if (sender.Equals(reduserBildestørrelseToolStripMenuItem)) { newMDIChild = new SchrinkImagesForm(); }
                else if (sender.Equals(sorterFilmMapperToolStripMenuItem)) { newMDIChild = new SortMoviesDesktopForm(); }
                else if (sender.Equals(oMDBLocalMoviesToolStripMenuItem)) { newMDIChild = new MoviesForm(); }
                else if (sender.Equals(liteDBMovesToolStripMenuItem)) { try { newMDIChild = new LiteDBMovieForm(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }
                else if (sender.Equals(beerXMLDisplayFilesToolStripMenuItem)) { newMDIChild = new BeerXmlDisplayForm(); }
                else if (sender.Equals(createBarcodeToolStripMenuItem)) { newMDIChild = new BeerCreateBarcodeForm(); }
                else if (sender.Equals(xMLValidatorToolStripMenuItem)) { newMDIChild = new Kolibri.Common.FormUtilities.Forms.ValidateXMLForm(); }
                else if (sender.Equals(seriesToolStripMenuItem)) { newMDIChild = new SeriesForm(); }

                else if (sender.Equals(navngiFilerToolStripMenuItem)) { newMDIChild = new RenameFilesForm(); }
                else if (sender.Equals(navngiMapperToolStripMenuItem)) { newMDIChild = new Kolibri.FormUtilities.Forms.RenameFoldersForm(); }
                else if (sender.Equals(browseSeriesToolStripMenuItem))
                {

                    newMDIChild = new Kolibri.Common.VisualizeOMDbItem.ShowLocalSeries(Settings.Default.SeriesForm_Search,
                    new LiteDBController(false, true, true).FindAllItems("series"));
                }
                else if (sender.Equals(oMDBOnlineSearchToolStripMenuItem))
                {
                    newMDIChild = new Kolibri.Common.VisualizeOMDbItem.ShowOnlineMovies();
                }
                else if (sender.Equals(profilesToolStripMenuItem)) { newMDIChild = new BeerProfileGeneratorForm(new FileInfo(@"C:\Users\asoes\source\repos\Kolibri.SortPics\Kolibri.SortPics\uranusgarage.WordPress.2023-09-21.xml")); }
                else if (sender.Equals(fermentablesToolStripMenuItem)) { newMDIChild = new BeerMetaDataForm(); }
                newMDIChild.MdiParent = this;
                newMDIChild.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void lukkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cleanOutEmptyFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx ex = new Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx("Velg folder du vil fjerne tomme mapper fra:");

                DialogResult res = ex.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    Kolibri.Common.FormUtilities.SplashScreen.Splash($"Sletter tomme mapper fra {ex.SelectedPath}", 2000);
                    Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(ex.SelectedPath);
                    Kolibri.Common.FormUtilities.SplashScreen.Splash($"Fullførst - Sletting av tomme mapper fra {ex.SelectedPath}", 2000);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);

            }


        }

        //private void fjernTommeMapperToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    DirectoryInfo folder = null;
        //    try
        //    {
        //        //CommonOpenFileDialog cofd = new CommonOpenFileDialog();
        //        //cofd.IsFolderPicker = true;
        //        //cofd.ShowDialog();
        //        //folder = new DirectoryInfo(cofd.FileName);

        //        var dlg1 = new Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx();
        //        dlg1.Description = "Select a folder to extract to:";
        //        dlg1.ShowNewFolderButton = true;
        //        dlg1.ShowEditBox = true;
        //        dlg1.ShowFullPathInEditBox = true;
        //        dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

        //        // Show the FolderBrowserDialog.
        //        DialogResult result = dlg1.ShowDialog();
        //        if (result == DialogResult.OK)
        //        {
        //            DirectoryInfo dinfo = new DirectoryInfo(dlg1.SelectedPath);
        //            Kolibri.Common.Utilities.FileUtilities.DeleteEmptyDirs(dinfo);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //}

        private void createLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            var mappe = Kolibri.Common.Utilities.FolderUtilities.LetOppMappe("Let opp mappe med alle bildene i - lag HTMLfil i mappen som viser alle filene");
            string fileExt = ".jpg";
            string filenamehtml = "all.htm";
            if (sender.Equals(createIMGLinksToolStripMenuItem))
                fileExt = ".jpg";
            else if (sender.Equals(createMOVIELinksToolStripMenuItem))
            {
                fileExt = ".mov";
                filenamehtml = "mov.html";
            }
            try
            {

                if (mappe.Exists)
                {
                    FileInfo[] liste = Kolibri.Common.Utilities.FileUtilities.GetFiles(mappe, $"*{fileExt}", false);

                    Array.Sort(liste, delegate (FileInfo x, FileInfo y)
                    {
                        return string.Compare(x.Name, y.Name);
                    });

                    foreach (var item in liste)
                    {
                        if (sender.Equals(createIMGLinksToolStripMenuItem))
                            builder.AppendLine($@"<img src=""{item.Name}"" alt=""{item.Name}""  title=""{item.Name}"" width=""410"" height=""400"" />");
                        else
                            builder.AppendLine($@"<img src=""{item.Name}"" alt=""{item.Name}""  title=""{item.Name}"" width=""410"" height=""400"" />");

                    }
                    FileInfo filename = new FileInfo(Path.Combine(mappe.FullName, filenamehtml));

                    Kolibri.Common.Utilities.FileUtilities.WriteStringToFile(builder.ToString(), filename.FullName);
                    Kolibri.Common.Utilities.FileUtilities.OpenFolderHighlightFile(filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void APIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {


                string ret = string.Empty;
                if (sender.Equals(getOMDBKeyToolStripMenuItem))
                {
                    ret = OMDBController.GetOmdbKey(false, true);
                }
                else if (sender.Equals(ObtainOMDBKeyToolStripMenuItem))
                {
                    OMDBController.GetOmdbKey(true);
                }
                else if (sender.Equals(getTMDBKeyToolStripMenuItem))
                {
                    ret = TMDBController.GetTMDBKey(false, true);

                }
                else if (sender.Equals(obtainTMDBKeyToolStripMenuItem)) { ret = TMDBController.GetTMDBKey(true); }
                else if (sender.Equals(settingsToolStripMenuItem))
                {


                    var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                    Process.Start(path);
                }

                else { throw new Exception($"{sender.ToString()} - no such command mapped in {System.Reflection.MethodBase.GetCurrentMethod().Name}"); }
                if (!string.IsNullOrEmpty(ret))
                {
                    MessageBox.Show(ret, (sender as ToolStripMenuItem).Text);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private async void Vinmonopolet_Click(object sender, EventArgs e)
        {

            try
            {
                if (sender.Equals(getVMProductDetailsToolStripMenuItem))
                {
                    DirectoryInfo dirInfo = FolderUtilities.LetOppMappe(@"c:\Temp", "Velg mappe for å lagre vinmonopol fil til");
                    if (null != dirInfo)
                    {

                        Kolibri.Common.Vinmonopolet.VinmonopoletConroller contr = new Kolibri.Common.Vinmonopolet.VinmonopoletConroller("d4eb0d6218fc449f8dc793ae44cc7fba");
                        var strText = await contr.DetailsNormal();
                        FileUtilities.WriteStringToFile(strText, Path.Combine(dirInfo.FullName, $@"{"Vinmonopolet"}_DetailsNormal.json"));
                        string imgId = "18546201";
                        var jall = contr.GetImage(imgId);
                        jall.Save(Path.Combine(dirInfo.FullName, $"{imgId}.jpg"), ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void finnDuplikaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx ex = new Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx("Velg folder du vil finne duplikate filmer fra:");

                DialogResult res = ex.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    Kolibri.Common.MovieAPI.Controller.SameFileController contr = new SameFileController(new DirectoryInfo(ex.SelectedPath));
                    var list = contr.GetDupes();
                    var ds = DataSetUtilities.AutoGenererDataSet(list);
                    Visualizers.VisualizeDataSet("Dupes", ds, this.Size);
 
                 var byFolder = contr.GetDupesByFolder();


                    if (list.Count() != byFolder.Count() && byFolder.Count >= 1)
                    {
                        if (MessageBox.Show("Do you want to see dupes by folders also?", "More choices awailable", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            ds = DataSetUtilities.AutoGenererDataSet(list);
                            Visualizers.VisualizeDataSet("Dupes by folder", ds, this.Size);
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void convertMovieToMP4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx ex = new Kolibri.Common.Utilities.FolderUtilities.FolderBrowserDialogEx("Velg folder du vil konvertere filmer fra:");

                DialogResult res = ex.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    Kolibri.Common.MovieAPI.Controller.ConvertMoveToMP4Controller contr = new ConvertMoveToMP4Controller(new DirectoryInfo(ex.SelectedPath));
                    contr.ExecuteConversion();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void getBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Velg bildefil du vil konvertere til Base64";

                DialogResult res = ofd.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    Image img = ImageUtilities.OpenImage(ofd.FileName);
                    string b = ImageUtilities.ImageToBase64(img, ImageFormat.Png);

                    OutputDialogs.ShowRichTextBoxDialog(ofd.FileName, b, this.Size);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }
    }
}
 