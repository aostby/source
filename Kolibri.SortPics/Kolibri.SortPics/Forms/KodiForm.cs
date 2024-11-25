using Kolibri.Common.FormUtilities.Forms;
using Kolibri.Common.MovieAPI.Controller;
using Kolibri.Common.MovieAPI.Entities;
using Kolibri.Common.Utilities;
using Kolibri.Utilities.Controller;
using Microsoft.VisualBasic.ApplicationServices;
using Sayaka.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;
using static Kolibri.Common.MovieAPI.Controller.LiteDBController;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SortPics.Forms
{
    public partial class KodiForm : Form
    {
        string _localText = string.Empty;
        private MALFileForm form;
        private OMDBController _OMDB;
        private LiteDBController _LITEDB;
        private TMDBController _TMDB;

        private Dictionary<string, string> _unsearchableDic = new Dictionary<string, string>();

        public KodiForm()
        {
            InitializeComponent();
            Init();
        }

        private void SetLabelText(string text)
        {
            try
            {
                _localText = text;

                toolStripStatusLabelFilnavn.TextAlign = ContentAlignment.MiddleLeft;
  
                try
                { 
                    try
                    {

                        try
                        {
                            toolStripStatusLabelFilnavn.Text = _localText;
                        }
                        catch (Exception ex1)
                        { }  
                        toolStripStatusLabelFilnavn.GetCurrentParent().Refresh();
                    }
                    catch (Exception) { }
                }
                catch (Exception) { }
            }
            catch (Exception)
            { }
        }

        private void Init()
        {
            this.Text = $"Search for KODI files to import ({Assembly.GetExecutingAssembly().GetName().Version.ToString()})";
            //      SetLabelText(this.Text);
            form = new MALFileForm();
            if (Directory.Exists(@"\\URANUSSHARE\Nyeste\Kodi_Cache\StuaCast"))
            { form.SourcePath = new DirectoryInfo(@"\\URANUSSHARE\Nyeste\Kodi_Cache\StuaCast"); }


            form.DestinationPath = form.SourcePath;
            form.FormBorderStyle = FormBorderStyle.None;
            form.TopLevel = false;
            form.Visible = true;

            splitContainer1.Panel1.Controls.Add(form);
            try
            {
                _LITEDB = new LiteDBController(true, false);
                //      linkLabelLiteDB.Text = _LITEDB.ConnectionString.Filename;
                //     linkLabelLiteDB.Tag = linkLabelLiteDB.Text;
            }
            catch (Exception)
            {
                _LITEDB = new LiteDBController(false, false);
            }

            try
            {
                FlowLayoutPanel linkPanel = new FlowLayoutPanel();
                linkPanel.HorizontalScroll.Enabled = false;
                linkPanel.FlowDirection = FlowDirection.TopDown;
                //         linkPanel.Height = groupBoxMovieLinks.Height;
                //         linkPanel.Width = groupBoxMovieLinks.Width;
                foreach (var key in Kolibri.Common.Utilities.MovieUtilites.MovieLinksDic.Keys)
                {
                    LinkLabel link = new LinkLabel();
                    link.Width = 5000;
                    link.Text = key;
                    link.Tag = Kolibri.Common.Utilities.MovieUtilites.MovieLinksDic[key];
                    link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
                    linkPanel.Controls.Add(link);
                }
                //    groupBoxMovieLinks.Controls.Add(linkPanel);
            }
            catch (Exception)
            { }
            var omdbkey = OMDBController.GetOmdbKey(false);
            _OMDB = new OMDBController(omdbkey, _LITEDB);
            _TMDB = new TMDBController(_LITEDB);
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (false)
            {
                //if (sender.Equals(linkLabelLiteDB))
                //{
                //    try
                //    {
                //        FileInfo info = new FileInfo(linkLabelLiteDB.Tag.ToString());

                //        Process.Start(info.Directory.FullName);
                //    }
                //    catch (Exception) { }
                //}
            }
            else
            {
                try
                {
                    if (sender as LinkLabel != null)
                    {
                        var s = sender as LinkLabel;

                        Process.Start($"{s.Tag}");
                    }
                }
                catch (Exception) { }
            }
        }

        private void buttonMapKodi2IMDB_Click(object sender, EventArgs e)
        {

            try
            {
                FileInfo file = FileUtilities.GetFiles(form.SourcePath, "*.xml", true).OrderByDescending(x => x.CreationTime).FirstOrDefault();

                if (file != null && (checkBoxMovies.Checked || checkBoxSeries.Checked))
                {

                    XDocument xDoc = XDocument.Load(file.FullName);

                    var res = XMLUtilities.SerializationHelper<videodb>.Deserialize(xDoc);

                    TMDbLib.Objects.Movies.Movie tmdbMov = new TMDbLib.Objects.Movies.Movie();
                    if (checkBoxMovies.Checked)
                    {
                        foreach (var kodi in res.movie)
                        {
                            try
                            {
        Movies(kodi);
                            }
                            catch (Exception ex)
                            { 
                            }
                    
                        
                           
                        }
                    }
                    if (checkBoxSeries.Checked)
                    {
                        foreach (var kodi in res.tvshow)
                        {
                            try
                            {   TVShow(kodi);

                            }
                            catch (Exception ex)
                            {

                                 
                            }
                         
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.ToString(), ex.GetType().Name);
                return;
            }
            SetLabelText($"KODI movies and/or series searched and possibly added to {_LITEDB.ConnectionString.Filename}.");
        }

        private void TVShow(videodbTvshow kodi)
        {
            if (kodi == null)
            {
                SetLabelText($"The show {kodi} was null");
                return;
            }

            // Get the type of the new object and the type of the old object passed in

            var test = MapperController.Map<TMDbLib.Objects.TvShows.TvShow>(kodi);
            test.FirstAirDate = Parse($"{kodi.premiered}");
            SetLabelText($"Inspecting {kodi.title} season: {kodi.season}  episode: {kodi.episode}.");
            if (kodi.uniqueid != null)
            {
                if (kodi.uniqueid.FirstOrDefault(x => x.type == "imdb") != null)
                {
                    if (test.ExternalIds == null)
                    {
                        test.ExternalIds = new TMDbLib.Objects.General.ExternalIdsTvShow();
                    }
                    test.ExternalIds.ImdbId = kodi.uniqueid.FirstOrDefault(x => x.type == "imdb").Value;
                }
                if (kodi.uniqueid.FirstOrDefault(x => x.type == "tmdb") != null)
                {
                    test.Id = kodi.uniqueid.FirstOrDefault(x => x.type == "tmdb").Value.ToInt32();
                }

                if (_LITEDB.FindItem(test.ExternalIds.ImdbId) == null)
                {
                    var movOMDB = _OMDB.GetItemByImdbId(test.ExternalIds.ImdbId);
                    if (movOMDB != null)
                    {
                        if (_LITEDB.Upsert(movOMDB))
                        {

                            SetLabelText($"{test.Name} as OMDB show added.");
                        }
                    }
                }
                if (_LITEDB.FindMovie(test.Id.ToString()) == null)
                {

                    var movTMDB = _TMDB.GetMovie(test.Id);
                    if (movTMDB != null)
                    {
                        if (_LITEDB.Upsert(movTMDB))
                        {
                            SetLabelText($"{test.Name} as TMDB movie added.");
                        }
                    }
                }
            }
            else
            {
                SetLabelText($"{test.Name} allready found!");
            }
        }


        private void Movies(videodbMovie kodi)
        {
            // Get the type of the new object and the type of the old object passed in

            var test = MapperController.Map<TMDbLib.Objects.Movies.Movie>(kodi);
            if (test.ImdbId == null)
            {
                try
                {     test.ImdbId = kodi.uniqueid.FirstOrDefault(x => x.type == "imdb").Value; 
                }
                catch (Exception ex)
                {
                    SetLabelText($"{test.Title} - {ex.Message}");
                }
           
            }


            test.ReleaseDate = Parse($"{kodi.premiered}");

            if (kodi.uniqueid != null)
            {
                if (kodi.uniqueid.FirstOrDefault(x => x.type == "imdb") != null)
                {
                    test.ImdbId = kodi.uniqueid.FirstOrDefault(x => x.type == "imdb").Value;
                }
                if (kodi.uniqueid.FirstOrDefault(x => x.type == "tmdb") != null)
                {
                    test.Id = kodi.uniqueid.FirstOrDefault(x => x.type == "tmdb").Value.ToInt32();
                }

                if (_LITEDB.FindItem(test.ImdbId) == null)
                {
                    var movOMDB = _OMDB.GetItemByImdbId(test.ImdbId);
                    if (movOMDB == null) {
                          movOMDB= _OMDB.GetItemById(test.Id);


                    }

                        if (movOMDB != null)
                    {
                        if (_LITEDB.Upsert(movOMDB))
                        {
                            SetLabelText($"{test.Title} as OMDB movie added.");
                        }
                        try
                        {
                            if (movOMDB.ImdbId != null)
                            {
                                var exists = _LITEDB.FindFile(movOMDB.ImdbId);
                                if (exists == null)
                                {
                                    string path = (kodi.path);
                                    path = path.Replace("/", "\\");
                                    path = path.Replace("smb:\\\\", @"\\");
                                    path = path.Replace(":445", string.Empty);

                                    FileInfo info = new FileInfo(path);
                                    FileItem item = new FileItem(movOMDB.ImdbId, info.FullName);
                                    _LITEDB.Upsert(item);//                                    _LITEDB.Update(item);
                                }
                            }
                        }
                        catch (Exception)
                        { }


                    }
                }
                if (_LITEDB.FindMovie(test.Id.ToString()) == null)
                {

                    var movTMDB = _TMDB.GetMovie(test.Id);
                    if (movTMDB != null)
                    {
                        if (_LITEDB.Upsert(movTMDB))
                        {
                            SetLabelText($"{test.Title} as TMDB movie added.");
                        }
                    }
                }
            }
            else
            {
                SetLabelText($"{test.Title} allready found!");
            }
        }

        public static DateTime? Parse(string text) =>
             DateTime.TryParse(text, out var date) ? date : (DateTime?)null;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                FileInfo file = FileUtilities.GetFiles(form.SourcePath, "*.xml", true).OrderByDescending(x => x.CreationTime).FirstOrDefault();

                if (file != null && (checkBoxMovies.Checked || checkBoxSeries.Checked))
                {
                    var xsltName = "GenericHTML01.xsl";

                    var filePath = file;
                    var joss = Kolibri.Common.FormUtilities.Controller.TransFormController.GetUnpackPath();
                    Kolibri.Common.FormUtilities.Controller.TransFormController.UnpackXSLT();
                    var xslt2 = Kolibri.Common.FormUtilities.Controller.TransFormController.FindXSLTFile(xsltName);

                    XSLTTransform.TransformAndShow(filePath, xslt2, "html");

                    var v = XSLTTransform.TransformFiles(xslt2, filePath);
                }

            }
            catch (Exception ex)
            { }
        }
    }
}