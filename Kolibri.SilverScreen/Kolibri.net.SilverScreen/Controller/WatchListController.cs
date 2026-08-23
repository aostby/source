using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoviesFromImdb.Controller
{
    public class WatchListController
    {
        private UserSettings _userSettings;
        private LiteDBController _liteDB = null;
        private ImageCacheDB _imageCache;

        public WatchListController(LiteDBController liteDB)
        {
            _liteDB = liteDB;
            Init();
        }  
        public WatchListController(UserSettings userSettings)
        {
            _userSettings = userSettings;
            _liteDB = new LiteDBController(userSettings.LiteDBFileInfo, false, false);
            Init();
        }
        private void Init()
        {
            if (_userSettings == null) { _userSettings = _liteDB.GetUserSettings(); }
            _imageCache = new ImageCacheDB(_userSettings);
        }
        public DataSet GetAllMoviesFromWatchLists(string watchListName = null)
        {
            IEnumerable<WatchListItem> list = _liteDB.WatchListFindAll(watchListName: watchListName).ToList(); 

            DataSet ret = Kolibri.net.Common.Utilities.DataSetUtilities.AutoGenererDataSet(list.ToList());
            if (ret.Tables.Count != 0)
            {
                ret.Tables[0].Columns.Remove("Picture");
                ret.Tables[0].Columns.Add("Image", typeof(Bitmap));
                foreach (DataRow row in ret.Tables[0].Rows)
                {
                    try
                    {
                        int pos = row.Table.Columns.IndexOf("Poster");
                        var id = row.Table.Columns.IndexOf("ImdbId");
                        var url = string.Empty;// _imageCache.GetPosterUrlAsync($"{row[id]}").Result;
                        if (string.IsNullOrWhiteSpace(url)) {
                            url = row[pos].ToString();
                        }
                        var pic = ImageUtilities.GetImageFromUrl(url);
                        pos = row.Table.Columns.IndexOf("Image");
                        row[pos] = pic;

                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return ret;
        }

        public bool AddMovieToLiteDBWatchList( WatchListItem entity)
        {
            try
            {
                if (_liteDB == null)
                {
                    _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
                    if (_userSettings == null)
                    {
                        _userSettings = _liteDB.GetUserSettings();
                    }
                }
                _liteDB.WatchListAdd(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            } 
        }

        public   bool ChangeMovieStatus(string movieId, string watched = "Y")
        {
         
            var entity = _liteDB.WatchListGetItemByID(movieId);
            entity.Watched = watched;


            if (_liteDB.WatchListUpsert(entity).GetAwaiter().GetResult())
            {
                MessageBox.Show("Movie status changed from not watched to watched!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Movie status cannot change!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public   bool DeleteMovieFromWatchLists(string movieId)
        { 

            if (_liteDB.DeleteWatchListItem(movieId) >= 1)
            {
                MessageBox.Show("Movie deleted from watchlist!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Movie delete failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
