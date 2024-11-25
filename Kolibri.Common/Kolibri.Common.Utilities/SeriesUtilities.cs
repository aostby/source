using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kolibri.Common.Utilities
{
    public class SeriesUtilities
    {
        /// <summary>
        /// Based on a list of filenames, for each file find the name of the series, the season, episodenumber, and include the full file name in the search in a table
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static DataTable SeriesEpisode(FileInfo[] infos)
        {
            DataTable table = DataSetUtilities.ColumnNamesToDataTable("Name", "Season", "Episode", "FullName").Tables[0];
            Regex regex = new Regex(@"(?<season>[Ss]\d{1,2})((?<sep>[-+]?)(?<episode>[Ee]\d{1,2}))+");
            string title = string.Empty;
            foreach (FileInfo info in infos)
            {
                string file = Path.GetFileNameWithoutExtension(info.FullName);
                Match match = regex.Match(file);
                if (match.Success)
                {
                    title = MovieUtilites.GetMovieTitle(file);
                    Console.WriteLine(match.Value);
                    table.Rows.Add(title, match.Groups["season"].Value, match.Groups["episode"].Value, info.FullName);
                }
                else
                {
                    List<char> separators = new List<char>() { '-', '(', '[' };
                    if (file.IndexOfAny(separators.ToArray()) >= 0)
                    {
                        title = MovieUtilites.GetMovieTitle(file.Substring(0, file.IndexOfAny(separators.ToArray())));
                        var resten = file.Substring(file.IndexOfAny(separators.ToArray())).TrimStart(separators.ToArray()).Trim();
                        string episode = new String(resten.SkipWhile(c => !Char.IsDigit(c)).TakeWhile(Char.IsDigit).ToArray());
                        if (string.IsNullOrEmpty(episode))
                        {
                            if (title.IndexOf("Episode", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                episode = title.Substring(title.IndexOf("Episode", StringComparison.OrdinalIgnoreCase));
                                episode = Regex.Replace(episode, "Episode", string.Empty, RegexOptions.IgnoreCase).Trim();
                                episode = episode.Split(' ').FirstOrDefault().Trim();
                                if (!episode.IsNumeric())
                                    episode = string.Empty;
                            }
                            if (episode == string.Empty)
                                episode = "1000";
                            //throw new Exception($"RegEx failed ({file}) in method { System.Reflection.MethodBase.GetCurrentMethod().Name}");
                        }
                        else
                            table.Rows.Add(title, "1", episode, info.FullName);
                    }
                }
            }
            table.DefaultView.Sort = "Season asc, Episode ASC";
            return table.DefaultView.ToTable();
        }

        public static string GetEpisodeFromFilename(string filename)
        {
            string ret = string.Empty;

            Regex regex = new Regex(@"(?<season>[Ss]\d{1,2})((?<sep>[-+]?)(?<episode>[Ee]\d{1,2}))+");
            string title = string.Empty;
            FileInfo info = new FileInfo(filename);
            string file = Path.GetFileNameWithoutExtension(info.FullName);
            Match match = regex.Match(file);
            if (match.Success)
            {
                title = MovieUtilites.GetMovieTitle(file);
                Console.WriteLine(match.Value);
                ret = $"{match.Groups["season"].Value}{match.Groups["episode"].Value}";
            }
            else
            {
                List<char> separators = new List<char>() { '-', '(', '[' };
                if (file.IndexOfAny(separators.ToArray()) >= 0)
                {
                    title = MovieUtilites.GetMovieTitle(file.Substring(0, file.IndexOfAny(separators.ToArray())));
                    var resten = file.Substring(file.IndexOfAny(separators.ToArray())).TrimStart(separators.ToArray()).Trim();
                    string episode = new String(resten.SkipWhile(c => !Char.IsDigit(c)).TakeWhile(Char.IsDigit).ToArray());
                    if (string.IsNullOrEmpty(episode))
                    {
                        if (title.IndexOf("Episode", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            episode = title.Substring(title.IndexOf("Episode", StringComparison.OrdinalIgnoreCase));
                            episode = Regex.Replace(episode, "Episode", string.Empty, RegexOptions.IgnoreCase).Trim();
                            episode = episode.Split(' ').FirstOrDefault().Trim();
                            if (!episode.IsNumeric())
                                episode = string.Empty;
                        }
                        if (episode == string.Empty)
                            episode = "1000";
                        //throw new Exception($"RegEx failed ({file}) in method { System.Reflection.MethodBase.GetCurrentMethod().Name}");
                    }
                    else

                        ret = $"{"1"}{episode}";
                }
            }
            return ret;
        }
    }
}