using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Kolibri.Common.Vinmonopolet
{
    /*
Standard small image in Vinmonopolet's online store is 300x300, e.g. https://bilder.vinmonopolet.no/cache/300x300-0/146101-1.jpg
Standard image on product page is 515x515, and 1200x1200, e.g.: https://bilder.vinmonopolet.no/cache/515x515-0/1698101-1.jpg 
                                                        and https://bilder.vinmonopolet.no/cache/1200x1200-0/187701-1.jpg
High resolution images(for press) is 5000x5000, e.g.: https://bilder.vinmonopolet.no/cache/5000x5000-0/1712701-1.jpg
    */

    public class VinmonopoletImages
    {

        private static string _baseUrl = "https://bilder.vinmonopolet.no/cache";
        private static string smallSize = "/300x300-0";
        private static string standardSize = "/515x515-0";
        private static string standardBig = "/1200x1200-0";
        private static string hiRes = "/5000x5000-0";

        public static System.Drawing.Image GetImage(string id)
        {
            string url = $"{_baseUrl}{standardSize}/{id}-1.jpg";
            return Utilities.ImageUtilities.GetImageFromUrl(url);
        }

    }
}
