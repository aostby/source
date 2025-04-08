using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Kolibri.net.Common.Utilities.Extensions
{
    public static class FileExt
    {

        /// <summary>
        /// This is the same default buffer size as
        /// <see cref="StreamReader"/> and <see cref="FileStream"/>.
        /// </summary>
        private const int DefaultBufferSize = 4096;

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static async Task<List<string>> ReadAllLinesAsync(this FileInfo info, IProgress<int> progress = null)
        {
            var ret = await ReadAllLinesAsync(info.FullName, Encoding.UTF8, progress);
            return ret.ToString().Split(Environment.NewLine).ToList();
        }
        public static async Task<StringBuilder> ReadAllLinesAsyncStringBuilder(this FileInfo info, IProgress<int> progress = null)
        {
            var ret = await ReadAllLinesAsync(info.FullName, Encoding.UTF8, progress);
            return ret;
        }

        private static async Task<StringBuilder> ReadAllLinesAsync(string path, Encoding encoding, IProgress<int> progress = null)
        {
            int itemIndex = 0;
            if (path.Contains("title.episode"))
                itemIndex = 1;
            int counter = 0;
            long currentLength = 0;
            long totalLength = new FileInfo(path).Length;
            int lastPercentage = 0;
            var lines = new StringBuilder();
            var arr = new string[0];
            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    counter++;
                    currentLength += line.Length;


                    { lines.AppendLine(line); }

                    if (progress != null)
                    {

                        int percentage = (int)((currentLength / (double)totalLength) * 100.0);

                        if (percentage == 0) percentage = 1;
                        if (percentage > lastPercentage)
                        {
                            lastPercentage = percentage;
                            progress.Report(lastPercentage);
                            await Task.Delay(1);
                        }
                    }
                }

                //   return lines.ToString().Split(Environment.NewLine).ToList();
                return lines;
            }
        }

        public static void SaveHashTable(this Hashtable ht, FileInfo path)
        {
            string json = JsonConvert.SerializeObject(ht);
            File.WriteAllText(path.FullName, json);
        }

        public static Hashtable GetHashTable(this Hashtable ht, FileInfo path)
        {
            string json = File.ReadAllText(path.FullName);

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var dict = JsonConvert.DeserializeObject<Dictionary<int, object>>(json, deserializeSettings);

            Hashtable hashtable = new Hashtable(dict);

            return hashtable;
        }

        /// <summary>
        /// Plukk en imdb id fra navnet til en directory
        /// </summary>
        /// <param name="path">full filsti for en directory</param>
        /// <returns>imdbid hvis funnet, null ellers</returns>
        public static string ImdbIdFromDirectoryName(this string path) 
        {
            string ret = null;
            try
            {
                string pattern = @"{(.*?)}";
                Regex rex = new Regex(pattern);
                var t = rex.Match(path);
                if (t.Success && t.Value.Contains("imdb-"))
                    ret= t.Value.Split("-").Last().TrimEnd('}');  

            }
            catch (Exception)
            {
                ret = null;    
            }

            return ret;
        }

    }
}