using Kolibri.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SortPics.Controller
{
    public class ImageCache_old
    {
        private Dictionary<string, byte[]> _imageCache;
        public string ApplicationName { get; set; }
        internal string CachePath
        {
            get
            { FileInfo ret = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "images.xml"));
                if (!ret.Directory.Exists) ret.Directory.Create();
                return ret.FullName;
            }
        }

        public ImageCache_old(bool init = true, string applicationName = null)
        {
            if (applicationName == null) ApplicationName = Assembly.GetCallingAssembly().GetName().Name;
            else ApplicationName = applicationName;
            if (_imageCache == null) _imageCache = new Dictionary<string, byte[]>();
            if (init)
                Init();
        }

        private void Init()
        {
            _imageCache = new Dictionary<string, byte[]>();

            try
            {
                if (File.Exists(CachePath))
                { 
                    XmlReader reader = XmlReader.Create(  new StreamReader(CachePath));
                    reader.ReadToFollowing("Images" );
                    do
                    {
                        try
                        {
                            string key,  b64;
                            reader.ReadToFollowing("Key");
                            key = reader.ReadElementContentAsString();
                            reader.ReadToFollowing("Value");
                            b64 = reader.ReadElementContentAsString();
                            _imageCache[key] = Convert.FromBase64String(b64);

                        }
                        catch (Exception ex)
                        { }

                    } while (reader.ReadToFollowing("Images"));
                }
            }
            catch (Exception ex) { }
        }

        //public Bitmap GetImage(string key)
        //{
        //    Bitmap ret = null;
        //    if (_imageCache.Keys.Contains(key))
        //        return _imageCache[key];
        //    else
        //        return ret;
        //}

        public bool InsertImage(string key, Bitmap bitmap)
        {
            bool ret = false;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    Bitmap bmp = new Bitmap(bitmap);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    _imageCache[key] = (byte[])ms.ToArray();

                    if (_imageCache.Keys.Count % 2 == 0&& _imageCache.Keys.Count> 600) SaveAsync();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    _imageCache[key] = Kolibri.Utilities.ImageUtilities.BitmapToBytes(bitmap);
                }
                catch (Exception ex2)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public byte[] GetByteArray(string key)
        {
            if (_imageCache[key] != null)
                return _imageCache[key];

            else
                return null;
        }

        public Bitmap RetrieveImage(string key)
        {
            Bitmap ret = null;
            try
            {
                // When retrieving, create a new Bitmap based on the bytes retrieved from the cached array
                if (_imageCache[key] != null)
                {
                    using (MemoryStream ms = new MemoryStream((byte[])_imageCache[key]))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            ret = bmp;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return ret;
        }
        public bool CacheImageExists(string key)
        {
            try
            {
                return _imageCache.Keys.Contains(key);
            }
            catch (Exception)
            {
                return false;
            }


        }

        #region from DB

        public async Task<bool> InitImages(LiteDBController LITEDB)
        {
            bool ret = true;
            try
            {
                var items = LITEDB.FindAllItems();
                var dic = items.OrderByDescending(mc => mc.ImdbRating)
                        .ToDictionary(mc => mc.ImdbId.ToString(),
                                      mc => mc.Poster.ToString(),
                                      StringComparer.OrdinalIgnoreCase).Distinct().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var task = Task.Run(async () => await (GetImagesAsync(LITEDB,dic)));
                var taskNA = Task.Run(async () => await (GetNAImagesAsync(LITEDB, dic)));

            }
            catch (Exception ex)
            { ret = false; }
            return ret;
        }

        public async Task<bool> GetNAImagesAsync(LiteDBController LITEDB, Dictionary<string, string> dic)
        {
            var items = dic.Where(item => item.Value == "N/A");
            if (items.Count() < 1)
                return false;

            foreach (var item in items)
            {
                if (CacheImageExists(dic[item.Key])) { continue; }
                else
                {
                    try
                    {
                        LiteDBController.ImagePoster img = LITEDB.FindImage(item.Key);
                        if (img != null) { continue; }

                        var movie = LITEDB.FindMovie(item.Key, true);
                        if (movie != null)
                        {
                            string path = string.Empty;
                            if (!string.IsNullOrWhiteSpace(movie.BackdropPath))
                                path = $"{movie.BackdropPath.TrimStart('/')}";
                            else
                                path = $"{movie.PosterPath.TrimStart('/')}";


                            var url = $"https://image.tmdb.org/t/p/w300/{path}";
                            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            using (System.Net.WebResponse webResponse = webRequest.GetResponse())
                            {
                                using (System.IO.Stream stream = webResponse.GetResponseStream())
                                {
                                    using (var image = (Bitmap)System.Drawing.Image.FromStream(stream))
                                    {
                                        webResponse.Close();
                                        InsertImage(url, image);

                                        if (img == null)
                                        {
                                            LITEDB.InsertImage(new LiteDBController.ImagePoster(movie.ImdbId, image));
                                            image.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            return true;
        }

        public async Task<bool> GetImagesAsync(LiteDBController LITEDB ,Dictionary<string, string> dic)
        {
            bool ret = true;
            try
            {
                foreach (var key in dic.Keys)
                {
                    LiteDBController.ImagePoster img = null;
                    try
                    {
                        if (dic[key].Contains("N/A")) continue;


                        if (!CacheImageExists(dic[key]))
                        {
                            img = LITEDB.FindImage(key);

                            if (img != null)
                            {
                                InsertImage(key, img.Image);
                                img.Image.Dispose();
                                continue;
                            }
                        }
                        if (CacheImageExists(dic[key])) { continue; }
                        else
                        {
                            var url = dic[key];
                            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url); 

                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            using (System.Net.WebResponse webResponse = webRequest.GetResponse())
                            {
                                using (System.IO.Stream stream = webResponse.GetResponseStream())
                                {
                                    var image = (Bitmap)System.Drawing.Image.FromStream(stream);
                                    webResponse.Close();
                                    InsertImage(url, image);

                                    if (img == null)
                                    {
                                        LITEDB.InsertImage(new LiteDBController.ImagePoster(key, image));
                                        image.Dispose();
                                    }
                                }
                            }
                        } 
                    }
                    catch (Exception ex)
                    {
                        try
                        {

                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception)
            {
            }
            return ret;

        }
          
        #endregion


        public bool Save(Dictionary<string, Bitmap> saveCache)
        {
            string errors = string.Empty;
            string SigBase64 = null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                SigBase64 = null;
                var list = saveCache.Keys.ToList();
                foreach (var item in list)
                {

                    byte[] byteImage = null;
                    try
                    {
                        using (Bitmap TEST = saveCache[item])
                        {
                            byteImage = ImageUtilities.BitmapToBytes((Bitmap)TEST.Clone());

                            if (byteImage != null && byteImage.Length > 0)
                            {
                                try
                                {
                                    _imageCache[item] = byteImage;

                                    //        SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
                                    //      dic[item] = SigBase64;
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += ex.Message;
                    }
                }

                //if (dic.Count() > 0)
                //{
                //    DataSet ds = new DataSet("imageCache");
                //    var datatable = dic.ToDataTable();
                //    if (datatable.DataSet == null)
                //    {
                //        datatable.TableName = "Images";
                //        ds.Tables.Add(datatable);
                //    }
                //    ds.WriteXml(CachePath);
                //}
            }
            catch (Exception ex)
            {
                errors += ex.Message;
            }

            //return string.IsNullOrEmpty($"{errors}");
            return Save();
        }
        public bool Save() {

            if (_imageCache != null && _imageCache.Count > 10)
            {
                Task.Run(() => SaveAsync());
                return true; }
            else return false;
        }

        private void SaveAsync()
        {
            string errors = string.Empty;
            string SigBase64 = null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                SigBase64 = null;
                var list = _imageCache.Keys.ToList();

                using (XmlWriter writer = XmlWriter.Create(CachePath))
                {
                    writer.WriteStartElement("Saratustra");
                    // Code to do the write here

                    foreach (var item in list)
                    {
                        try
                        {
                            byte[] byteImage = _imageCache[item];
                            {
                                if (byteImage != null && byteImage.Length > 0)
                                {
                                    try
                                    {
                                        SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
                                        writer.WriteStartElement("Images");

                                        writer.WriteStartElement("Key");
                                        writer.WriteValue(item);
                                        writer.WriteEndElement();

                                        writer.WriteStartElement("Value");
                                        writer.WriteValue(SigBase64);
                                        writer.WriteEndElement();

                                        writer.WriteEndElement();  // end Images
                                    }
                                    catch (Exception ex) { }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errors += ex.Message;
                        }
                    }


                    writer.WriteEndElement();  // end Saratustra
                }
    

                //if (dic.Count() > 0)
                //{
                //    DataSet ds = new DataSet("imageCache");
                //    var datatable = dic.ToDataTable();
                //    if (datatable.DataSet == null)
                //    {
                //        datatable.TableName = "Images";
                //        ds.Tables.Add(datatable);
                //    }
                //    ds.WriteXml(CachePath);
                //}
            }
            catch (Exception ex)
            {
                errors += ex.Message;
            }
        }
    }
}