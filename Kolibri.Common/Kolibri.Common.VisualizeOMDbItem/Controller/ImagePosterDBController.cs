using LiteDB;
using Kolibri.Common.VisualizeOMDbItem.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using Kolibri.Common.Utilities;
 
using Kolibri.Common.VisualizeOMDbItem.Entities;
using Kolibri.Common.MovieAPI.Controller;

namespace Kolibri.Common.VisualizeOMDbItem.Controller
{
    public class ImagePosterDBController
    {
        private bool ExclusiveConnection { get; set; }

        private LiteDatabase _litePosterDB;
        public ConnectionString ConnectionString;

        private string DefaultFilePath
        {
            get
            {
                string filepath = @"C:\inetpub\wwwroot\TMDB\ImagePoster.db";
                if (!File.Exists(filepath))
                    filepath = @"ImagePoster.db";
                return new FileInfo(filepath).FullName;
            }
        }

        public ImagePosterDBController(bool exclusiveAccess = false, bool readOnly = true)
        {
            ExclusiveConnection = exclusiveAccess;
            readOnly = false;
            ConnectionString = new ConnectionString()
            {
                Connection = ExclusiveConnection ? ConnectionType.Direct : ConnectionType.Shared,
                ReadOnly = readOnly,
                Upgrade = false,
                Password = null,
                Filename = DefaultFilePath,
            };
            try
            {
                if (!string.IsNullOrEmpty(  AccessConfig.LastImageCacheConnectionString))
                    if (File.Exists(AccessConfig.LastImageCacheConnectionString))
                    {
                        ConnectionString.Filename = AccessConfig.LastImageCacheConnectionString;
                    }
            }
            catch (Exception) { }

            try
            {
                AccessConfig.LastImageCacheConnectionString = ConnectionString.Filename;
                Properties.Settings.Default.Save();
            }
            catch (Exception) { }

            _litePosterDB = new LiteDatabase(ConnectionString);
        }
        public void LiteDBController_old(bool exclusiveAccess = false)
        {
            #region dette veriker

            if (exclusiveAccess)
                _litePosterDB = new LiteDatabase(DefaultFilePath);
            else
                _litePosterDB = new LiteDatabase(new ConnectionString(DefaultFilePath) { Connection = ConnectionType.Shared });
            return;

            #endregion
        }
        public void Dispose()
        { 
            try
            {
                _litePosterDB.Dispose();
            }
            catch (Exception)
            {
            }
        }

        #region ImagePoster
        public bool ImageExists(string ImdbId) {
            bool ret = false;
            try
            {
                var count = _litePosterDB.GetCollection<ImageBase>("ImageBase").Count(Query.EQ("_id", ImdbId));
                return (count > 0);
            }
            catch (Exception)
            { }
            return ret;
        }

        internal bool InsertImage(ImagePoster imagePoster)
        {
            try
            {
                if (ImageExists(imagePoster.ImdbId)) return true;
                ImageBase imgbase = null;

                using (var ms = new MemoryStream())
                {
                    using (var bitmap = new Bitmap(imagePoster.Image))
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        var SigBase64 = Convert.ToBase64String(ms.GetBuffer()); //Get Base64
                        imgbase = new ImageBase(imagePoster.ImdbId, SigBase64);
                    }
                }
                _litePosterDB.GetCollection<ImageBase>("ImageBase")
                    .Insert(imagePoster.ImdbId, imgbase);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ImagePoster FindImage(string imdbId)
        {
            ImagePoster ret = null;
            try
            { 
                var imgbase = _litePosterDB.GetCollection<ImageBase>("ImageBase")
                    .FindById(imdbId);
           //     .Find(x => x.ImdbId == imdbId).FirstOrDefault();

                if (imgbase != null)
                {
                    ret = new ImagePoster(imgbase.ImdbId,  ImageUtilities.Base64ToImage(imgbase.Base64) as Bitmap);
                }
                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

    }
}
