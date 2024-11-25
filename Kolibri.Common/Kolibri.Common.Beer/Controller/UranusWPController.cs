 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kolibri.Common.Beer.Controller
{

    public class UranusWPController
    {
        private Uri baseUrl = new Uri("https://www.problembar.net/category/bryggelogg/");
        private string _uranusWebPage = string.Empty;

        public UranusWPController()
        {
            Init();
        }

        private void Init()
        {
            var url = baseUrl;

            WebRequest request = HttpWebRequest.Create(url);

            using (WebResponse response = request.GetResponse())
            {

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    _uranusWebPage = responseText;
                }
            }
        }

        public string GetWebPage() { return _uranusWebPage; }

        internal string ConvertHTML2WPxml()
        {
            //fjern alle lenker før blog'en
            string input = _uranusWebPage.Substring(_uranusWebPage.IndexOf(@"<div class=""site-bloglist"">"));
            var matches = Regex.Matches(input, @"<a\shref=""(?<url>.*?)"">(?<text>.*?)</a>");

            // Konstruer en xml. 
            var ret = new StringBuilder();
            ret.Append("<ArrayOfLinks>");
            // chanel er stedet lenkene ligger i orginal eksportfil
            ret.Append("<channel>");
            
            //Faen og, må sortere omvendt... blir snudd i grensesnittet.
            for (int i = matches.Count; i-- > 0;)
            {
                string url = (matches[i].Groups["url"].Value).Replace(@"target=""_blank", string.Empty).Replace(@"rel=""bookmark", string.Empty).Trim().TrimEnd('"');
                ret.Append(@"<item>");
                ret.Append($@"<title><![CDATA[{ matches[i].Groups["text"].Value}]]></title>");
                ret.Append($@"<link>{url.Trim()}</link>");
                ret.Append(@"</item>");
            } 

            ret.Append("</channel>");
            ret.Append("</ArrayOfLinks>");

            XDocument xDoc = XDocument.Parse(ret.ToString());
            return xDoc.ToString();
        }
    }
}