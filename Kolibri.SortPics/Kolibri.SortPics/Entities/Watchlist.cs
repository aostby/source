﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using OMDbApiNet.Model;
namespace SortPics.Entities
{  
        public class WatchList : Item
        {

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
            public byte[] Picture { get; set; }
            public string Trailer { get; set; }
            public string Watched { get; set; }
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
