﻿using Kolibri.net.Common.Dal.Controller;
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
            if (_liteDB == null)
                _liteDB = new LiteDBController(_userSettings.LiteDBFileInfo, false, false);
            return _liteDB.WishListAdd(entity);



            //string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand("AddNewMovie", conn))
            //    {
            //        conn.Open();

            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.AddWithValue("@Title", item.Title);
            //        cmd.Parameters.AddWithValue("@Year", item.Year);
            //        cmd.Parameters.AddWithValue("@imdbRating", item.Rated);
            //        cmd.Parameters.AddWithValue("@Runtime", item.Runtime);
            //        cmd.Parameters.AddWithValue("@Genre", item.Genre);
            //        cmd.Parameters.AddWithValue("@Actors", item.Actors);
            //        cmd.Parameters.AddWithValue("@Plot", item.Plot);
            //        cmd.Parameters.AddWithValue("@Metascore", item.Metascore);
            //        cmd.Parameters.AddWithValue("@Poster", item.Poster);
            //        cmd.Parameters.AddWithValue("@Image", item.Picture);
            //        cmd.Parameters.AddWithValue("@Trailer", item.Trailer);

            //        var i = cmd.ExecuteNonQuery();

            //        if (i >= 1)
            //        {
            //            MessageBox.Show("Movie is added to watch list!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        }
            //        else
            //        {
            //            MessageBox.Show("Movie already exist in your watchlist!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        }


            // }
            //  }

        }

        public   bool ChangeMovieStatus(string movieId, string watched = "Y")
        {
         
            var entity = _liteDB.WishListGetItemByID(movieId);
            entity.Watched = watched;


            if (_liteDB.WishListUpsert(entity))
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
