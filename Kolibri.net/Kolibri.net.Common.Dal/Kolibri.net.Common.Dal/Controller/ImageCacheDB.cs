using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Images.Entities;
using Kolibri.net.Common.Utilities;
using LiteDB;
using OMDbApiNet.Model;
using sun.security.util;
using sun.util.resources.cldr.zh;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Xml;

namespace Kolibri.net.Common.Dal.Controller
{
    public class ImageCacheDB
    {
      internal  ImagePosterDBController _ImageDB;
        LiteDBController _liteDB;

   

        private int _max = 2000;

      //  private MemoryCache _cache = MemoryCache.Default;
        //private readonly UserSettings _userSettings;

        //private Dictionary<string, byte[]> _imageCache;
        public string ApplicationName { get; private set; }

        public ImageCacheDB(LiteDBController liteDB)
        {

            _liteDB = liteDB;
            
            Init();
        }

        //internal string CachePath
        //{
        //    get
        //    {

        //        FileInfo ret = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "images.xml"));
        //        if (!ret.Directory.Exists) ret.Directory.Create();
        //        return ret.FullName;
        //    }
        //}

        //public int CacheNumElementsInCache
        //{
        //    get
        //    {
        //        int ret = 0;
        //        try
        //        {
        //            ret = _cache.Where(o => true).Select(o => o.Key).Count();

        //        }
        //        catch (Exception)
        //        { }
        //        return ret;

        //    }
        //}


        private void Init()
        {
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            _ImageDB = new ImagePosterDBController(DefaultDBPath(), false, false);

            //try
            //{
            //    if (File.Exists(CachePath))
            //    {
            //        using (XmlReader reader = XmlReader.Create(new StreamReader(CachePath)))
            //        {
            //            reader.ReadToFollowing("Images");
            //            do
            //            {
            //                try
            //                {
            //                    string key, b64;
            //                    reader.ReadToFollowing("Key");
            //                    key = reader.ReadElementContentAsString();
            //                    if (!_cache.Contains(key))
            //                    {
            //                        reader.ReadToFollowing("Value");
            //                        b64 = reader.ReadElementContentAsString();

            //                        StoreItemsInCache(key, Convert.FromBase64String(b64));
            //                    }
            //                }
            //                catch (Exception ex)
            //                { }

            //            } while (reader.ReadToFollowing("Images"));
            //        }
            //    }
            //}
            //catch (Exception ex) { }

            if (_liteDB != null)
            {
                Task.Run(async () => await InitImagesAsync(_liteDB));
            }

        }

        public FileInfo DefaultDBPath()
        {
            string path = Path.ChangeExtension(_liteDB.ConnectionString.Filename, ".imgdb");
            FileInfo ret = new FileInfo(path);
            return ret;
        }
        //Store Stuff in the cache  
        //public void CasheImage(string key, byte[] arr)
        //{
        //    //Do what you need to do here. Database Interaction, Serialization,etc.
        //    var cacheItemPolicy = new CacheItemPolicy()
        //    {
        //        //Set your Cache expiration.
        //        AbsoluteExpiration = DateTime.Now.AddDays(1)
        //    };
        //    //remember to use the above created object as third parameter.
        //    _cache.Add(key, arr, cacheItemPolicy);
        //}

        //Get stuff from the cache
        //public byte[] CasheGetItemFromCache(string key)
        //{
        //    if (!_cache.Contains(key))
        //    {
        //        return null;
        //    }
        //    return _cache.Get(key) as byte[];
        //}

        //Remove stuff from the cache. If no key supplied, all data will be erased.
        //public void CasheRemoveItemsFromCache(string key)
        //{
        //    if (string.IsNullOrEmpty(key))
        //    {
        //        _cache.Dispose();
        //    }
        //    else
        //    {
        //        _cache.Remove(key);
        //    }
        //}

        public async Task<bool> InsertImageAsync(string key, Bitmap bitmap)
        {
            bool ret = false;
            
            var db = await _ImageDB.ImageExists(key);
            bool insert =  !db;
            try
            {
                if (insert)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap bmp = new Bitmap(bitmap);
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                        //CasheImage(key, ms.ToArray());
                        await _ImageDB.InsertImage(new ImagePoster(key, bitmap));
                        bmp.Dispose();
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            { }
            return ret;
        }

  
        public async Task<ImagePoster> FindImageAsync(string key)
        {
            var ret = await RetrieveImageAsync(key);

            if (ret == null)
            { return null; }
            else
            {
                return new ImagePoster(key, ret);
            }
        }
        public async Task<Bitmap> RetrieveImageAsync(string key)
        {
            Bitmap ret = null;
            try
            {
                if (await _ImageDB.ImageExists(key))
                {
                    var imagePoster = await _ImageDB.FindImage(key);
                    if (imagePoster == null) return null;
                    else
                    {
                        imagePoster.Image.Tag = key;
                        return imagePoster.Image;
                    }
                }
                else {
               ret = await  Task.FromResult((Bitmap)  ImageUtilities.GetImageFromUrl(key));

                    _ImageDB.InsertImage(new ImagePoster(key, ret));
                }
            }
            catch (Exception ex) { ret = (Bitmap)ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage());   }
            
            return ret;
        }

        //public bool CacheImageExists(string key)
        //{
        //    try
        //    {
        //        return _cache.Contains(key);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        #region from DB

        public async Task<bool> InitImagesAsync(LiteDBController LITEDB)
        {
            bool ret = true;
            try
            {
                var items = await LITEDB.FindAllItems();
                var dic = items.OrderByDescending(mc => mc.ImdbRating)
                        .ToDictionary(mc => mc.ImdbId.ToString(),
                                      mc => mc.Poster?.ToString(),
                                      StringComparer.OrdinalIgnoreCase).Distinct().Where(lnk => lnk.Value != "N/A").Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var task = Task.Run(async () => await GetImagesAsync(LITEDB, dic));
                var taskNA = Task.Run(async () => await GetNAImagesAsync(LITEDB, dic));

            }
            catch (Exception ex)
            { ret = false; }
            return ret;
        }

        private async Task<bool> GetNAImagesAsync(LiteDBController LITEDB, Dictionary<string, string> dic)
        {
            var items = dic.Where(item => item.Value == "N/A");
            if (items.Count() < 1)
                return false;

            foreach (var item in items)
            {
                try
                {
                    ImagePoster img = await _ImageDB.FindImage(item.Key);
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
                        System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                        webRequest.AllowWriteStreamBuffering = true;
                        webRequest.Timeout = 30000;
                        using (System.Net.WebResponse webResponse = webRequest.GetResponse())
                        {
                            using (Stream stream = webResponse.GetResponseStream())
                            {
                                using (var image = (Bitmap)Image.FromStream(stream))
                                {
                                    webResponse.Close();
                                    await InsertImageAsync(url, image);

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
            return true;
        }

        private void InitImages(LiteDBController LITEDB, IEnumerable<Item> items)
        {
            var dic = items.OrderByDescending(mc => mc.ImdbRating)
                      .ToDictionary(mc => mc.ImdbId.ToString(),
                                    mc => mc.Poster.ToString(),
                                    StringComparer.OrdinalIgnoreCase).Distinct().Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var task = Task.Run(async () => await GetImagesAsync(LITEDB, dic));
            var taskNA = Task.Run(async () => await GetNAImagesAsync(LITEDB, dic));
        }

        private async Task<bool> GetImagesAsync(LiteDBController LITEDB, Dictionary<string, string> dic)
        {
            bool ret = true;
            try
            {
                foreach (var key in dic.Keys)
                {
                    ImagePoster img = null;
                    try
                    {
                        //if (CacheImageExists(dic[key])) { continue; }
                        if (dic[key].Contains("N/A")) continue;


                        
                            img = await _ImageDB.FindImage(key);

                        if (img != null)
                        {
                            await InsertImageAsync(key, img.Image);
                            img.Image.Dispose();
                            continue;
                        }
                        else
                        {
                            var url = dic[key];
                            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                            webRequest.AllowWriteStreamBuffering = true;
                            webRequest.Timeout = 30000;
                            using (System.Net.WebResponse webResponse = webRequest.GetResponse())
                            {
                                using (Stream stream = webResponse.GetResponseStream())
                                {
                                    var image = (Bitmap)Image.FromStream(stream);
                                    webResponse.Close();
                                    return await InsertImageAsync(url, image);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { ret = false; }
                }
            }
            catch (Exception) { ret = false; }
            return ret;
        }

        /*public async Task<bool> GetImagesAsync(Dictionary<string, string> dic)
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
                      
                        if (await _ImageDB.ImageExists(dic[key])) { continue; }

                        var url = dic[key];
                        System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                        webRequest.AllowWriteStreamBuffering = true;
                        webRequest.Timeout = 30000;
                        System.Net.WebResponse webResponse = webRequest.GetResponse();
                        Stream stream = webResponse.GetResponseStream();
                        var image = (Bitmap)Image.FromStream(stream);
                        webResponse.Close();

                        await InsertImageAsync(key, image);

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

        }*/

        #endregion


        //public bool Save()
        //{
        //   FileUtilities.MoveCopy(new FileInfo(CachePath), new FileInfo(CachePath));
        //    try
        //    {    SaveAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private void SaveAsync()
        //{
        //    string errors = string.Empty;
        //    string SigBase64 = null;
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    try
        //    {
        //        List<string> list= new List<string>();
        //        SigBase64 = null;
        //        var keys = _cache.Where(o => true).Select(o => o.Key);
        //        foreach (var key in keys)
        //        {
        //            list.Add(key);
        //        } 

        //        using (XmlWriter writer = XmlWriter.Create(CachePath))
        //        {
        //            writer.WriteStartElement("Saratustra");
        //            // Code to do the write here

        //            foreach (var item in list)
        //            {
        //                try
        //                {
        //                    byte[] byteImage = GetItemsFromCache(item);
        //                    {
        //                        if (byteImage != null && byteImage.Length > 0)
        //                        {
        //                            try
        //                            {
        //                                SigBase64 = Convert.ToBase64String(byteImage); // Get Base64
        //                                writer.WriteStartElement("Images");

        //                                writer.WriteStartElement("Key");
        //                                writer.WriteValue(item);
        //                                writer.WriteEndElement();

        //                                writer.WriteStartElement("Value");
        //                                writer.WriteValue(SigBase64);
        //                                writer.WriteEndElement();

        //                                writer.WriteEndElement();  // end Images
        //                            }
        //                            catch (Exception ex) { }
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errors += ex.Message;
        //                }
        //            }  
        //            writer.WriteEndElement();  // end Saratustra
        //        } 
        //    }
        //    catch (Exception ex)
        //    {
        //        errors += ex.Message;
        //    }
        //}

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
        internal Hashtable _ht = new Hashtable();
        internal bool ExclusiveConnection { get; set; }

        internal LiteDatabase _litePosterDB;
        internal ConnectionString ConnectionString;

        internal ImagePosterDBController(FileInfo pahtToImageDB, bool exclusiveAccess = false, bool readOnly = true)
        {

            if (!pahtToImageDB.Directory.Exists)
                pahtToImageDB.Directory.Create();
            if (!pahtToImageDB.Exists)
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
        internal async Task<bool> ImageExists(string ImdbId)
        {int hash = ImdbId.GetHashCode();
            bool ret = false;
            
           if( _ht.ContainsKey(hash))return true;
            try
            {
                //  var count =   _litePosterDB.GetCollection<ImageBase>("ImageBase").Count(Query.EQ("_id", ImdbId));
                var count = _litePosterDB.GetCollection<ImageBase>("ImageBase").Count(x => x.ImdbId == ImdbId);
                ret = count > 0;
            }
            catch (AggregateException ex) {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            { ret = false; }
            if (ret) { _ht.Add(hash, ImdbId); }
              return ret;
        }

        internal async Task<bool> InsertImage(ImagePoster imagePoster)
        {
            bool ret = true;
            try
            {
                if (await ImageExists(imagePoster.ImdbId))
                {
                    return true;
                }

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

        internal Task<ImagePoster> FindImage(string imdbId)
        {
            ImagePoster ret = null;

            try
            {
                var imgbase = _litePosterDB.GetCollection<ImageBase>("ImageBase")
                    .FindById(imdbId);
                //     .Find(x => x.ImdbId == imdbId).FirstOrDefault();

                if (imgbase != null)
                {
                    ret = new ImagePoster(imgbase.ImdbId, ImageUtilities.Base64ToImage(imgbase.Base64) as Bitmap);
                }
                
            return Task.FromResult(ret);
            }
            catch (AggregateException ex) 
            {
                ret = null;
            }

            catch (Exception ex)
            {
                ret = null;
            }
            return null;
        }

        #endregion
    }
}