﻿using OMDbApiNet.Model;
namespace Kolibri.net.Common.Dal.Entities
{
    public class WatchList : Item
    {
        public string WatchListName { get; set; } = "MyMovies";

        public byte[] Picture { get; set; }
        public string Trailer { get; set; }
        public string Watched { get; set; }
        public string FilePath { get; set; }

        /*public class ImdbEntity
 {
     public string Title { get; set; }
     public string Year { get; set; }
     public string Rated { get; set; }
     public string Released { get; set; }
     public string Genre { get; set; }
     public string Actors { get; set; }
     public string Plot { get; set; }
     public string Poster { get; set; }
     public string Metascore { get; set; }
     public string imdbRating { get; set; }
     public string Response { get; set; }*/

        /*
        public string Runtime { get; set; } 
         public string imdbVotes { get; set; }
        public string imdbID { get; set; }
         public string Type { get; set; }
         public string Language { get; set; }
         public string Country { get; set; }
         public string Awards { get; set; }
         public string Director { get; set; }
         public string Writer { get; set; }
*/
    }
}
