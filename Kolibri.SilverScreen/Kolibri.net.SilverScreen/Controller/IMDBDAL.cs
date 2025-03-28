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
    public class IMDBDAL
    {
        private UserSettings _userSettings;
        private   LiteDBController _liteDB = null;

        public IMDBDAL(LiteDBController liteDB) {
            _liteDB = liteDB;
        }
        public IMDBDAL(UserSettings userSettings)
        {
            _userSettings = userSettings;
            _liteDB = new LiteDBController(userSettings.LiteDBFileInfo, false, false);
        }


        public DataSet GetAllMovies(string wishListName = null)
        {
            IEnumerable<WatchList> list = _liteDB.WishListFindAll(watchListName: wishListName).ToList(); 

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
                        var pic = ImageUtilities.GetImageFromUrl(row[pos].ToString());
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

        public   bool AddMovie( WatchList entity)
        {
            try
            {
                if (_liteDB == null)
                    _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
                _liteDB.WishListAdd(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            } 
        }

        public   bool ChangeMovieStatus(string movieId, string watched = "Y")
        {
         
            var entity = _liteDB.WishListGetItemByID(movieId);
            entity.Watched = watched;


            if (_liteDB.WishListUpsert(entity).GetAwaiter().GetResult())
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
        public   bool DeleteMovie(string movieId)
        {
        



            if (_liteDB.DeleteWishListItem(movieId) >= 1)
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
