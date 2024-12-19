using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;


namespace Kolibri.net.Common.Utilities
{
    public class HTMLUtilities
    {
        public static string GetHTML(Uri url)
        {
            string ret = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    ret = client.DownloadString(url.AbsoluteUri);
                }
            }
            catch (Exception) { }
            return ret;
        }

        public static Uri GoogleSearchString(string lookUp)
        {
            string url = $"https://www.google.com/search?q={(lookUp.Replace(" ", "+"))}";
            return new Uri(url);
        }

        /// <summary>
        /// https://code-maze.com/how-to-create-a-url-query-string/
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public static string BuildUrlWithQueryStringUsingStringConcat(
            string basePath, Dictionary<string, string> queryParams)
        {
            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return $"{basePath}?{queryString}";
        }

        /// <summary>
        /// It allows us to parse an existing query string into a collection of key-value pairs, modify those pairs, and generate a new query string.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public static string BuildUrlWithQueryStringUsingParseQueryStringMethod(
    string basePath, Dictionary<string, string> queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var dict in queryParams)
            {
                query[dict.Key] = dict.Value;
            }
            return string.Join("?", basePath, query.ToString());
        }

        public static void OpenURLInBrowser(Uri url)
        {
            try
            {
                //             url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url.AbsoluteUri) { UseShellExecute = true });
            }
            catch (Exception)
            {
            }
        }

        public static bool ShowHTML(string html, DirectoryInfo dirInfo = null)
        {
            bool ret = true;
            try
            {
                FileInfo info = new FileInfo(@"c:\temp\html.html");
                if (dirInfo != null)
                    info = new FileInfo(Path.Combine(dirInfo.FullName, $"{dirInfo.Name}.html"));
                if (!info.Directory.Exists) info.Directory.Create();
                Kolibri.net.Common.Utilities.FileUtilities.WriteStringToFile(html, info.FullName, Encoding.UTF8);

                System.Diagnostics.Process.Start(info.FullName);
            }
            catch (Exception ex)
            { ret = false; }
            return ret;

        }

    }
}