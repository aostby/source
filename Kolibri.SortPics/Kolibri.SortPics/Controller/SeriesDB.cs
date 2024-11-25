using LiteDB;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using TvSeriesLogsDb.Helper;
using System.Linq.Expressions;

namespace SortPics.Controller
{
    public class SeriesDb : IDisposable
    {
        private LiteDatabase db;
        private ILiteCollection<Item> collection;
        private const string FILE = "series.db";
        private const string APP_NAME = "TvSeriesLogs";
        private const string COLL_SERIES = "series";
        private string PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public SeriesDb()
        {
            if (!Debugger.IsAttached)
                Directory.CreateDirectory(Path.Combine(PATH, APP_NAME));
            db = new LiteDatabase(Debugger.IsAttached ? FILE : Path.Combine(PATH, APP_NAME, FILE));
            collection = db.GetCollection<Item>(COLL_SERIES);
            collection.EnsureIndex(c => c.Title);
        }

        public Item FindById(string id)
        {
            var series = collection.FindById(id);
            return series;
        }


        /// <returns>Number of items that were removed</returns>
        public bool Remove(string id) => collection.Delete(id);
        public int RemoveAll() => collection.DeleteAll();
        public bool ExistsById(string id) => collection.Exists(s => s.ImdbId == id);
        public bool Exists(Expression<Func<Item, bool>> func) => collection.Exists(func);
        /// <returns>Id of the added series</returns>
        public int Add(Item series) => collection.Insert(series);
        /// <summary>updates considering the id</summary>
        /// <returns>false if document wasn't found in collection</returns>
        public bool Update(Item series) => collection.Update(series);
        /// <summary>updates considering the id</summary>
        /// <returns>false if document wasn't found in collection</returns>
        public bool Update(string id, Item series) => collection.Update(id, series);
        public IEnumerable<Item> GetAll() => collection.FindAll();
        public IEnumerable<Item> Find(Expression<Func<Item, bool>> exp) => collection.Find(exp);
        public IEnumerable<Item> Filter(string title, ushort limit, bool caseSensitive, bool orderByIndwx = true)
        {
            var items = GetAll();
            var filtered = items.Where(FilterPredicate);
            if (orderByIndwx)
                filtered = filtered.OrderBy(i => i.Title.IndexOf(title));

            bool FilterPredicate(Item series) =>
                caseSensitive
                ? series.Title.Contains(title)
                : series.Title.ToLower().Contains(title.ToLower());

            return filtered.Take(limit);
        }
        public void Dispose() => db.Dispose();
    }
}