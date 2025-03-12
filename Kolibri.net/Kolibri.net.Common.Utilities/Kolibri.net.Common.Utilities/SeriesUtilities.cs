using Kolibri.net.Common.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace Kolibri.net.Common.Utilities
{
    public class SeriesUtilities
    {
        /// <summary>
        /// Based on a list of filenames, for each file find the name of the series, the season, episodenumber, and include the full file name in the search in a table
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static DataTable SeriesEpisode(List<string> infos, bool useTitle=true)
        {
            DataTable table = DataSetUtilities.ColumnNamesToDataTable((useTitle?"Title":"Name"), "Season", "Episode", "FullName").Tables[0];
            Regex regex = new Regex(@"(?<season>[Ss]\d{1,2})((?<sep>[-+]?)(?<episode>[Ee]\d{1,2}))+");
            string title = string.Empty;
            foreach (string info in infos)
            {
                string file = Path.GetFileNameWithoutExtension(info);
                Match match = regex.Match(file);
                if (match.Success)
                {
                    title = MovieUtilites.GetMovieTitle(file);
                    Console.WriteLine(match.Value);
                    table.Rows.Add(title, match.Groups["season"].Value.ToInt32().ToString(), match.Groups["episode"].Value.ToInt32().ToString(), info);
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
                            table.Rows.Add(title, "1", episode, info);
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

        /// <summary>
        /// usual columnnames =  "Name", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" 
        /// </summary>
        /// <param name="formatTable"></param>
        /// <returns></returns>
        public static  DataTable SortAndFormatSeriesTable(DataTable? formatTable)
        { string sortString = string.Empty;
            string name = formatTable.Columns.Contains("Name") ? "name" : "Title";
            string season = formatTable.Columns.Contains("Season") ? "Season" : "SeasonNumber";
            string episode = formatTable.Columns.Contains("Episode") ? "Episode" : "EpisodeNumber";

           sortString    = $"{season} ASC, {episode} ASC, {name} ASC";

            DataTable resultTable = new DataView(formatTable, "", sortString, DataViewRowState.CurrentRows).ToTable();

            List<string> columns = new List<string>() { "Name", "Title", "ImdbRating", "Year", "Rated", "Runtime", "Genre", "Plot" };
            if ((!resultTable.Columns.Contains("Year") || !resultTable.Columns.Contains("Name")))
                columns = DataSetUtilities.ColumnNames(resultTable).ToList();//series

            resultTable = new DataView(resultTable, "", sortString, DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());

            if (resultTable.Columns.Contains("Response"))
            {
                resultTable.Columns["Response"].SetOrdinal(0);
                resultTable = new DataView(resultTable, "", $"Response ASC, {sortString}", DataViewRowState.CurrentRows).ToTable(false, columns.ToArray());
            }
            if (resultTable.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(resultTable);
            }

            return resultTable;
        }

    }
}