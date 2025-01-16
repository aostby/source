using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Images.Entities;
using Kolibri.net.Common.Utilities;
using LiteDB;
using OMDbApiNet.Model;
using System.Data;
using System.Reflection;
using System.Runtime.Caching;
using System.Xml;

namespace Kolibri.net.Common.Dal.Entities
{
    public class ImageCache
    {
        ImagePosterDBController _ImageDB;
        LiteDBController _LiteDB;

        private int _max =2000;
        
        private MemoryCache _cache = MemoryCache.Default;
        private readonly UserSettings _userSettings;

        //private Dictionary<string, byte[]> _imageCache;
         public string ApplicationName { get; private set; }

        public ImageCache(UserSettings userSettings)
        {
            this._userSettings = userSettings;
            Init();
        }

        internal string CachePath
        {
            get
            {
              
                FileInfo ret = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "images.xml"));
                if (!ret.Directory.Exists) ret.Directory.Create();
                return ret.FullName;
            }
        }

        public int NumElements
        {
            get
            {
                int ret = 0;
                try
                {
                    ret = _cache.Where(o => true).Select(o => o.Key).Count();

                }
                catch (Exception)
                { }
                return ret;

            }
        }

      
        private void Init()
        {
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            _ImageDB = new ImagePosterDBController(DefaultPath(), false, false);

            try
            {
                if (File.Exists(CachePath))
                {
                    using (XmlReader reader = XmlReader.Create(new StreamReader(CachePath)))
                    {
                        reader.ReadToFollowing("Images");
                        do
                        {
                            try
                            {
                                string key, b64;
                                reader.ReadToFollowing("Key");
                                key = reader.ReadElementContentAsString();
                                if (!_cache.Contains(key))
                                {
                                    reader.ReadToFollowing("Value");
                                    b64 = reader.ReadElementContentAsString();

                                    StoreItemsInCache(key, Convert.FromBase64String(b64));
                                }
                            }
                            catch (Exception ex)
                            { }

                        } while (reader.ReadToFollowing("Images"));
                    }
                }
            }
            catch (Exception ex) { }

            if (_LiteDB != null)
            {
                Task.Run(async () => await InitImages(_LiteDB));
            }
        }

        public FileInfo DefaultPath() {
         FileInfo ret = new FileInfo( Path.ChangeExtension(_userSettings.LiteDBFileInfo.FullName, ".imgdb"));
            return ret;
        }
        //Store Stuff in the cache  
        public void StoreItemsInCache(string key, byte[] arr)
        { 
            //Do what you need to do here. Database Interaction, Serialization,etc.
            var cacheItemPolicy = new CacheItemPolicy()
            {
                //Set your Cache expiration.
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            };
            //remember to use the above created object as third parameter.
            _cache.Add(key, arr, cacheItemPolicy);
        }

        //Get stuff from the cache
        public  byte[] GetItemsFromCache(string key)
        {
            if (!_cache.Contains(key))
            {      return null;
            } 
            return _cache.Get(key) as byte[];
        }

        //Remove stuff from the cache. If no key supplied, all data will be erased.
        public   void RemoveItemsFromCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                 _cache.Dispose();
            }
            else
            {
                _cache.Remove(key);
            }
        } 
    
        public bool InsertImage(string key, Bitmap bitmap)
        {
            bool ret = false;
            var cache = CacheImageExists(key);
            var db = _ImageDB.ImageExists(key);
            bool insert = (!cache|| !db);
            try
            {
                if (insert)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap bmp = new Bitmap(bitmap);
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                        StoreItemsInCache(key, (byte[])ms.ToArray());
                        _ImageDB.InsertImage(new ImagePoster(key, bitmap));
                        bmp.Dispose();
                    }
                }
                else { 
                
                }
            }
            catch (Exception ex)
            { }
            return ret;
        }

        public byte[] GetByteArray(string key)
        {
            return GetItemsFromCache(key); 
        }

        public ImagePoster FindImage(string key)
        {
            var ret = RetrieveImage(key);

            if (ret == null)
            { return null; }
            else
            {
                return new ImagePoster(key, ret);
            }
        }
        public Bitmap RetrieveImage(string key)
        {
            Bitmap ret = null;
            Byte[] arr = null;
            try
            {
                //var arr = GetItemsFromCache(key);
                if (arr == null)
                {
                    if (_ImageDB.ImageExists(key))
                    {
                        var imagePoster = _ImageDB.FindImage(key);
                        if (imagePoster == null) return null;
                        else
                        {
                            imagePoster.Image.Tag = key;
                            return imagePoster.Image;
                        }
                    }
                }
                else
                {
                    // When retrieving, create a new Bitmap based on the bytes retrieved from the cached array

                    using (MemoryStream ms = new MemoryStream((byte[])arr))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            ret = bmp;ret.Tag = key;
                        }
                    }
                }
            }
            catch (Exception ex)
            { ret = null; }
            return ret;
        }
        public bool CacheImageExists(string key)
        {
            try
            {
                return _cache.Contains(key);
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
                var items =await LITEDB.FindAllItems();
                var dic = items.OrderByDescending(mc => mc.ImdbRating)
                        .ToDictionary(mc => mc.ImdbId.ToString(),
                                      mc => mc.Poster.ToString(),
                                      StringComparer.OrdinalIgnoreCase).Distinct().Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var task = Task.Run(async () => await (GetImagesAsync(LITEDB, dic)));
                var taskNA = Task.Run(async () => await ( GetNAImagesAsync(LITEDB,  dic)));

            }
            catch (Exception ex)
            { ret = false; }
            return ret;
        }

        private async Task<bool> GetNAImagesAsync(LiteDBController LITEDB,  Dictionary<string, string> dic)
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
                        ImagePoster img = _ImageDB.FindImage(item.Key);
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
                                            _ImageDB.InsertImage(new ImagePoster(movie.ImdbId, image));
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

        private void InitImages(LiteDBController LITEDB,   IEnumerable<Item> items)
        {
            var dic = items.OrderByDescending(mc => mc.ImdbRating)
                      .ToDictionary(mc => mc.ImdbId.ToString(),
                                    mc => mc.Poster.ToString(),
                                    StringComparer.OrdinalIgnoreCase).Distinct().Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var task = Task.Run(async () => await (GetImagesAsync(LITEDB, dic)));
            var taskNA = Task.Run(async () => await (GetNAImagesAsync(LITEDB, dic)));
        }

        private async Task<bool> GetImagesAsync(LiteDBController LITEDB,  Dictionary<string, string> dic)
        {
            bool ret = true;
            try
            {
                foreach (var key in dic.Keys)
                {
                    ImagePoster img = null;
                    try
                    {
                        if (CacheImageExists(dic[key])) { continue; }
                        if (dic[key].Contains("N/A")) continue;


                        if (!CacheImageExists(dic[key]))
                        {
                            img =_ImageDB.FindImage(key);

                            if (img != null)
                            {
                                InsertImage(key, img.Image);
                                img.Image.Dispose();
                                continue;
                            }
                        }                    
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
                                    InsertImage(key, image);

                                    if (img == null)
                                    {
                                        _ImageDB.InsertImage(new ImagePoster(key, image));
                                        image.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception) { }
            return ret;
        }
    
        public async Task<bool> GetImagesAsync(Dictionary<string, string> dic)
        {
            bool ret = true;
            try
            {
                foreach (var key in dic.Keys)
                {
                    ImagePoster img = null;
                    try
                    {
                        if (dic[key].Contains("N/A")) continue;
                        if (CacheImageExists(dic[key])) { continue; }
                        if(_ImageDB.ImageExists(dic[key])) { continue; }



                        var url = dic[key];
                            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            System.Net.WebResponse webResponse = webRequest.GetResponse();
                            System.IO.Stream stream = webResponse.GetResponseStream();
                            var image = (Bitmap)System.Drawing.Image.FromStream(stream);
                            webResponse.Close(); 

                             InsertImage(key, image);  
                        
                    }
                    catch (Exception ex)
                    {
                         
                    }
                }
            }
            catch (Exception)
            {
            }
            return ret;

        }
     
        #endregion

         
        public bool Save()
        {
           FileUtilities.MoveCopy(new FileInfo(CachePath), new FileInfo(CachePath));
            try
            {    SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SaveAsync()
        {
            string errors = string.Empty;
            string SigBase64 = null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                List<string> list= new List<string>();
                SigBase64 = null;
                var keys = _cache.Where(o => true).Select(o => o.Key);
                foreach (var key in keys)
                {
                    list.Add(key);
                } 

                using (XmlWriter writer = XmlWriter.Create(CachePath))
                {
                    writer.WriteStartElement("Saratustra");
                    // Code to do the write here

                    foreach (var item in list)
                    {
                        try
                        {
                            byte[] byteImage = GetItemsFromCache(item);
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
            }
            catch (Exception ex)
            {
                errors += ex.Message;
            }
        }

        public void Dispose()
        {

            try
            {
                _ImageDB.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }

    internal class ImagePosterDBController
    {
        internal bool ExclusiveConnection { get; set; }

        internal LiteDatabase _litePosterDB;
        internal ConnectionString ConnectionString;

        internal ImagePosterDBController(FileInfo pahtToImageDB, bool exclusiveAccess = false, bool readOnly = true)
        {

            if (!pahtToImageDB.Directory.Exists)
                pahtToImageDB.Directory.Create();
            if(!pahtToImageDB.Exists)
                pahtToImageDB.Create(); 

            ExclusiveConnection = exclusiveAccess;
           


            ConnectionString = new ConnectionString()
            {
                Connection = ExclusiveConnection ? ConnectionType.Direct : ConnectionType.Shared,
                ReadOnly = readOnly,
                Upgrade = false,
                Password = null,
                Filename = pahtToImageDB.FullName
            };
            

            _litePosterDB = new LiteDatabase(ConnectionString);
        }
       
        internal void Dispose()
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
        internal bool ImageExists(string ImdbId)
        {
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
        {bool ret =true;
            try
            {
                if (ImageExists(imagePoster.ImdbId)) {
                    
                    
                    
                    return true; }
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

        internal ImagePoster FindImage(string imdbId)
        {
            ImagePoster ret = null;
            try
            {
                var imgbase = _litePosterDB.GetCollection<ImageBase>("ImageBase")
                    .FindById(imdbId);
                //     .Find(x => x.ImdbId == imdbId).FirstOrDefault();

                if (imgbase != null)
                {
                    ret = new ImagePoster(imgbase.ImdbId,ImageUtilities.Base64ToImage(imgbase.Base64) as Bitmap);
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