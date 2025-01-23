using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.SilverScreen.Controls
{
    public class Constants
    {
        public enum MultimediaType { movie, Movies, Series, Audio, Pictures }

        public static List<string> VisibleTMDBColumns
        {
            get
            {
                List<string> visibleColumns = new List<string>(){
                    "Title"
                    ,"Year"
                    ,"ImdbRating"
                    ,"Rated"
                    ,"Runtime"
                    ,"Genre"
                    ,"Plot"
                    ,"ImdbId"
                };
                return visibleColumns;
            }
        }

        public static List<string> VisibleSeasonEpisodeColumns
        {
            get
            {
                List<string> visibleColumns = new List<string>(){
                    "Title",
                    "Released",
                    "Episode",
                    "ImdbRating",
                    "ImdbId",
                };

                return visibleColumns;
            }
        }

       

    }
}
