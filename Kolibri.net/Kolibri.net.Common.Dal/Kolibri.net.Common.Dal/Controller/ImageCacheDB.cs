using com.sun.xml.@internal.bind.v2.model.core;
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

        //LiteDBController _liteDB;
      //  private Hashtable _ht = new Hashtable(); 

        private int _max = 2000;
        private UserSettings _userSettings;

        public string ApplicationName { get; private set; }

        public ImageCacheDB(UserSettings userSettings)
        {
          //  _liteDB = liteDB;
          _userSettings = userSettings;
            Init();
        } 

        private void Init()
        {
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            _ImageDB = new ImagePosterDBController(DefaultDBPath(), false, false); 

            
        }

        public FileInfo DefaultDBPath()
        {
            string path = Path.ChangeExtension(_userSettings.LiteDBFilePath, ".imgdb");
            FileInfo ret = new FileInfo(path);
            return ret;
        }

        public async Task<bool> InsertImageAsync(string key, Bitmap bitmap)
        {
            bool ret = false;

            var db = await _ImageDB.ImageExists(key);
            bool insert = !db;
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
                { }
            }
            catch (Exception ex)
            { }
            return ret;
        }

        public async Task<ImagePoster> FindImageAsync(string key)
        {
            var ret = await RetrieveImageAsync(key);

            if (ret == null||ret.PhysicalDimension.Width<=1)
            { return null; }
            else
            {
                var hash = key.GetHashCode();
                var imgPoster = new ImagePoster(key, ret);
                //_ht[hash] = imgPoster;
                return imgPoster;
            }
        }
        public async Task<Bitmap> RetrieveImageAsync(string key)
        {
            Bitmap ret = null;
            try
            {
                var hash = key.GetHashCode();
                //if (_ht.ContainsKey(hash))
                //{
                //    var tmp = ((ImagePoster)_ht[hash]).Image;
                //    return tmp;
                //}

                if (await _ImageDB.ImageExists(key))
                {
                    var imagePoster = await _ImageDB.FindImage(key);
                    if (imagePoster == null) return null;
                    else
                    {
                        imagePoster.Image.Tag = key;

                   //     _ht.Add(hash, imagePoster);
                        return imagePoster.Image;
                    }
                }
                else
                {
                    ret = await Task.FromResult((Bitmap)ImageUtilities.GetImageFromUrl(key));
                    _ImageDB.InsertImage(new ImagePoster(key, ret));
                }
            }
            catch (Exception ex) { ret = (Bitmap)ImageUtilities.Base64ToImage(ImageUtilities.BrokenImage()); }

            return ret;
        } 
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
        {
            int hash = ImdbId.GetHashCode();
            bool ret = false; 
            try
            {
                //  var count =   _litePosterDB.GetCollection<ImageBase>("ImageBase").Count(Query.EQ("_id", ImdbId));
                var collection = _litePosterDB.GetCollection<ImageBase>("ImageBase");//.Count(x => x.ImdbId == ImdbId);
                  ret = collection.FindById(hash)!=null;
               
            }
            catch (AggregateException ex) {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            { ret = false; }
           
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
                    .Insert(imagePoster.ImdbId.GetHashCode(), imgbase);
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
                    .FindById(imdbId.GetHashCode());
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