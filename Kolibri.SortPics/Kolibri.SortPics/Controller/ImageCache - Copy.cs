﻿using Kolibri.Utilities;
using OMDbApiNet.Model;
using SortPics.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SortPics.Controller
{
    public class ImageCache
    {
        private int _max = 500;
        
        private MemoryCache _cache = MemoryCache.Default;
        //private Dictionary<string, byte[]> _imageCache;
        public string ApplicationName { get; set; }
        internal string CachePath
        {
            get
            {
                FileInfo ret = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName, "images.xml"));
                if (!ret.Directory.Exists) ret.Directory.Create();
                return ret.FullName;
            }
        }

        internal int NumElements
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

        public ImageCache (bool init = true, string applicationName = null)
        {
            if (applicationName == null) ApplicationName = Assembly.GetCallingAssembly().GetName().Name;
            else ApplicationName = applicationName;
            //     if (_imageCache == null) _imageCache = new Dictionary<string, byte[]>();
            if (init)
                Init();
        }

        private void Init()
        {
     //       _imageCache = new Dictionary<string, byte[]>();

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
        } 
    

        //Store Stuff in the cache  
        public   void StoreItemsInCache(string key, byte[] arr)
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
                return null;

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

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {  Bitmap bmp = new Bitmap(bitmap);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); 
                    StoreItemsInCache(key, (byte[])ms.ToArray());
                    bmp.Dispose();
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

        public Bitmap RetrieveImage(string key)
        {
            Bitmap ret = null;
            try
            {
                // When retrieving, create a new Bitmap based on the bytes retrieved from the cached array
                 
                    using (MemoryStream ms = new MemoryStream((byte[])GetItemsFromCache(key)))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            ret = bmp;
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
                var items = LITEDB.FindAllItems();
                var dic = items.OrderByDescending(mc => mc.ImdbRating)
                        .ToDictionary(mc => mc.ImdbId.ToString(),
                                      mc => mc.Poster.ToString(),
                                      StringComparer.OrdinalIgnoreCase).Distinct().Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var task = Task.Run(async () => await (GetImagesAsync(LITEDB, dic)));
                var taskNA = Task.Run(async () => await ( GetNAImagesAsync(LITEDB, new ImagePosterDBController(false, false),dic)));

            }
            catch (Exception ex)
            { ret = false; }
            return ret;
        }

        public async Task<bool> GetNAImagesAsync(LiteDBController LITEDB, ImagePosterDBController ImageDB, Dictionary<string, string> dic)
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
                        ImagePoster img = ImageDB.FindImage(item.Key);
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
                                            ImageDB.InsertImage(new ImagePoster(movie.ImdbId, image));
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

        internal void InitImages(LiteDBController LITEDB, ImagePosterDBController ImageDB, IEnumerable<Item> items)
        {
            var dic = items.OrderByDescending(mc => mc.ImdbRating)
                      .ToDictionary(mc => mc.ImdbId.ToString(),
                                    mc => mc.Poster.ToString(),
                                    StringComparer.OrdinalIgnoreCase).Distinct().Take(_max).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var task = Task.Run(async () => await (GetImagesAsync(LITEDB, dic)));
            var taskNA = Task.Run(async () => await (GetNAImagesAsync(LITEDB, dic)));
        }

        public async Task<bool> GetImagesAsync(LiteDBController LITEDB, Dictionary<string, string> dic)
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
                            img = ImageDB.FindImage(key);

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
                                    InsertImage(key, image);

                                    if (img == null)
                                    {
                                        LITEDB.InsertImage(new ImagePoster(key, image));
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
        #endregion


        //public bool Save(Dictionary<string, Bitmap> saveCache)
        //{
        //    string errors = string.Empty;
          
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    try
        //    {
        //        var list = saveCache.Keys.ToList();
        //        foreach (var item in list)
        //        { byte[] byteImage = null;
        //            try
        //            {
        //                using (Bitmap TEST = saveCache[item])
        //                {
        //                    byteImage = ImageUtilities.BitmapToBytes((Bitmap)TEST.Clone());

        //                    if (byteImage != null && byteImage.Length > 0)
        //                    {
        //                        try
        //                        {
        //                            StoreItemsInCache(item,byteImage);
        //                            TEST.Dispose();
        //                        }
        //                        catch (Exception ex) { }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                errors += ex.Message;
        //            }
        //        } 
        
        //    }
        //    catch (Exception ex)
        //    {
        //        errors += ex.Message;
        //    }

        //    //return string.IsNullOrEmpty($"{errors}");
        //    return Save();
        //}
        public bool Save()
        {
            Kolibri.Utilities.FileUtilities.MoveCopy(new FileInfo(CachePath), new FileInfo(CachePath));
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
    }
}