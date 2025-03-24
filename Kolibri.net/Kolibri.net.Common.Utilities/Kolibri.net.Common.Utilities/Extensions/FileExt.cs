using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using java.nio.file;

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

        public static Task<List<string>> ReadAllLinesAsync(this FileInfo info, List<string> imdbIds , IProgress <int> progress = null)
        {
            return ReadAllLinesAsync(info.FullName, Encoding.UTF8, imdbIds, progress);
        }
        //arr_lines = System.Text.RegularExpression.Regex.Matches(str_text, "^.*word1.*", RegexOptions.Multiline).Cast<Match>().Select(m => m.ToString()).ToArray()

        private static async Task<List<string>> ReadAllLinesAsync(string path, Encoding encoding, List<string> imdbIds, IProgress<int> progress=null)
        {
            int counter = 0;
            long currentLength = 0;
            long totalLength = new FileInfo(path).Length;    
            int lastPercentage = 0;
            var lines = new StringBuilder();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {counter++;
                    currentLength += line.Length;

                    if (imdbIds == null)
                    { lines.AppendLine(line); }
                    else
                    {
                        lines.AppendLine(line);
                    }
                    if (progress != null)
                    {

                        int percentage = (int)((currentLength / (double)totalLength) * 100.0);
                        if (percentage > lastPercentage)
                        {
                            lastPercentage = percentage;
                            progress.Report(lastPercentage);
                            await Task.Delay(1);
                        }
                    }


                }
            }

            return lines.ToString().Split(Environment.NewLine).ToList();
        }
    }
}
