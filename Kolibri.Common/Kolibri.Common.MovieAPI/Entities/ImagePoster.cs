using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.MovieAPI.Entities
{
    public class ImagePoster
    {
        public ImagePoster() { }

        public ImagePoster(string imdbid, Bitmap image)
        {
            ImdbId = imdbid;
            Image = image;
        }

        /// <summary>
        /// References the imdb id
        /// </summary>
        public string ImdbId { get; set; }
        /// <summary>
        /// References the image
        /// </summary>
        public Bitmap Image { get; set; }

    }

    public class ImageBase
    {
        public ImageBase(string imdbid, string base64)
        {
            ImdbId = imdbid;
            Base64 = base64;
        }

        /// <summary>
        /// References the imdb id
        /// </summary>
        public string ImdbId { get; set; }
        /// <summary>
        /// References the image
        /// </summary>
        public string Base64 { get; set; }
    }
}