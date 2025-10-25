using com.sun.org.apache.bcel.@internal.generic;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.FormUtilities.Tools;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.SilverScreen.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using percentCalc = Kolibri.net.Common.FormUtilities.Tools.ProgressBarHelper;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class IMDbDataFilesSubForm : Form
    {

        private FileInfo info;
        private MySqlTableOperationsController _mySqlTableOperationsController;
        public IMDbDataFilesSubForm(string filePath, UserSettings userSettings)
        {
            InitializeComponent();
            info = new FileInfo(filePath);
             _mySqlTableOperationsController = new MySqlTableOperationsController(userSettings.DefaultConnection);
            
            Init();
        }

        private async void Init()
        {
            this.Text = $"{info.Name}";            
            this.Visible = true;            
            pictureBox1.Visible = true;
            Thread.Sleep(5);

            string size = FileUtilities.GetFilesize(info.Length);
            bool huge = size.Contains("gib", StringComparison.OrdinalIgnoreCase);
            this.Text = $"{info.Name} ({size})";
            SetStatusLabelText($"[INITIALIZING] [{DateTime.Now.ToShortTimeString()}] {info.Name} ({size}) - Please wait.");

            bool ret = await StartProcessingLargeFile(huge);
            SetStatusLabelText($"[DONE] 100%  Errors: {ret}");
            this.Text = $"{(ret ? "Success!" : "Failed!")} Operation completed. {this.Text}"; 
        }

        private async Task<bool> StartProcessingLargeFile(bool huge)
        { 
            bool ret = true;
            Progress<int> progress = percentCalc.InitProgressBar(toolStripProgressBar1);

            if (!huge)
            {
                var sb = await info.ReadAllLinesAsyncStringBuilder(progress);
                ret = await InsertIntoMySQL(Path.GetFileNameWithoutExtension(info.FullName), sb.ToString().Split(Environment.NewLine).ToList(), progress);
            }
            else
            {
                int counter = 0;

                var hfCounter = new HugeFileCounter { totalCount = FileUtilities.GetLineCountInHugeFile(info.FullName), file = info };
                var hugeList = await File.ReadAllLinesAsync(info.FullName);
                int chunksize = 10000;
                foreach (var lines in hugeList.Chunk(chunksize)) //File.ReadLines(info.FullName))
                {
                    ret = ret & await InsertIntoMySQL(Path.GetFileNameWithoutExtension(info.FullName), lines.ToList(), progress, hfCounter);
                    hfCounter.currentCount += lines.Length;
                    Thread.Sleep(4);
                } 
            }
            return ret;
        }

        #region process data 
        private async Task<bool> InsertIntoMySQL(string name, List<string> orglines, IProgress<int> progress, HugeFileCounter hugeFileCounter = null)
        {
            GC.Collect(); // Forces collection of all generations

            bool ret = false;
            ProgressBarHelper.InitProgressBar(toolStripProgressBar1);
            switch (name)
            {
                //https://github.com/kalaspuffar/imdb2mysql/blob/main/src/main/java/org/example/TSVToSQL.java
                case "title.basics": ret = await processTitle(3100, name, orglines, progress); break;
                case "title.episode": ret = await processEpisodes(7500, name, orglines, progress); break;
                case "title.akas": ret = await processTitleLanguage(4000, name, orglines, progress, hugeFileCounter); break;
                case "name.basics": ret = await processNames(1500, name, orglines, progress); break;

                case "title.ratings": ret = await processRatings(7500, name, orglines, progress); break;
                case "title.principals": ret = await processPrincipals(4500, name, orglines, progress, hugeFileCounter); break;
                case "title.crew": ret = await processCrew(6500, name, orglines, progress); break;
                default:
                    break;
            }

            return ret;
        }

        public async Task<bool> processTitle(int maxLines, string name, List<string> orglines, IProgress<int> progress = null)
        {
            string insertSql = "INSERT INTO titles (id, titleType, primaryTitle, originalTitle, isAdult, startYear, endYear, runtimeMinutes, genres) VALUES ";
            bool ret = false;
            await _mySqlTableOperationsController.TitleCreateTable(create: true, keys: false);

            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");

            orglines.RemoveAt(0);
            int totalCount = 0;
            int counter = 0;

            wr.AppendLine(insertSql);
            foreach (var line in orglines)
            {
                counter++;

                String[] lineArr = line.Split("\t");
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("(");
                    sb.Append(wrapOrNull(lineArr[0]));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[1]));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[2]));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[3]));
                    sb.Append(", ");
                    sb.Append(lineArr[4].Replace("\\N", "NULL"));
                    sb.Append(", ");
                    sb.Append(lineArr[5].Replace("\\N", "NULL"));
                    sb.Append(", ");
                    sb.Append(lineArr[6].Replace("\\N", "NULL"));
                    sb.Append(", ");
                    sb.Append(lineArr[7].Replace("\\N", "NULL"));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[8]));
                    sb.Append("),");

                    wr.AppendLine(sb.ToString());

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)//3200 går ikke
                {
                    ret = ret && await Execute(wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "; COMMIT;");
                    totalCount = totalCount + counter;
                    counter = 0;
                    wr = new StringBuilder(insertSql);
                    if (progress != null)
                    {
                        var percent = percentCalc.CalculatePercent(totalCount, orglines.Count());
                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name} , maxLines: {maxLines}");
                        try
                        {
                            if (progress != null) progress.Report(percent);
                        }
                        catch (Exception ex)
                        { }
                        await Task.Delay(2);
                    }
                    if (!ret)
                    {
                        return ret;
                    }
                }

            }
            string last = wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',');
            wr = new StringBuilder(last);
            wr.AppendLine("; COMMIT;");
            wr.AppendLine("SET autocommit=1;");
            await _mySqlTableOperationsController.TitleCreateTable(false, true);
            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");
            return ret;
        }

        public async Task<bool> processEpisodes(int maxLines, string name, List<string> orglines, IProgress<int> progress = null)
        {
            string tableName = "episodes";
            bool ret = false;
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");

            wr.AppendLine($"CREATE TABLE {tableName} (id varchar(20) NOT NULL,   parentId int NOT NULL, seasonNumber int, episodeNumber int);");
            wr.AppendLine(Environment.NewLine);

            wr.AppendLine(Environment.NewLine);
            wr.AppendLine("COMMIT;");
            //   wr.AppendLine("SET autocommit=1;");

            ret = await Execute(wr.ToString());
            wr = new StringBuilder();
            /// wr.AppendLine("SET autocommit=0;");

            orglines.RemoveAt(0);
            int totalCount = 0;
            int counter = 0;
            foreach (var line in orglines)
            {
                counter++;

                String[] lineArr = line.Split("\t");
                try
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"INSERT INTO {tableName} (id, parentId, seasonNumber, episodeNumber) VALUES (");
                    sb.AppendLine(wrapOrNull(lineArr[0]));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[1]));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[2].Replace("\\N", "NULL"));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[3].Replace("\\N", "NULL"));
                    sb.AppendLine(");");

                    wr.Append(sb.ToString());
                    wr.Append(Environment.NewLine);

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)
                {
                    wr.AppendLine("COMMIT;");
                    ret = ret && await Execute(wr.ToString());
                    totalCount = totalCount + counter;
                    counter = 0;
                    wr = new StringBuilder();
                    if (progress != null)
                    {
                        var percent = percentCalc.CalculatePercent(totalCount, orglines.Count());
                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name}, maxLines: {maxLines}");
                        try
                        {
                            if (progress != null) progress.Report(percent);
                        }
                        catch (Exception ex)
                        { }
                        await Task.Delay(1);
                    }
                    if (!ret)
                    {
                        return ret;
                    }
                }

            }

            wr.AppendLine("COMMIT;");
            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (id);");
            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");
            return ret;
        }

        public async Task<bool> processNames(int maxLines, string name, List<string> orglines, IProgress<int> progress = null)
        {
            string tableName = "names";
            bool ret = false;
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");
            wr.AppendLine("DROP TABLE IF EXISTS knownfor;");

            wr.AppendLine($"CREATE TABLE {tableName} (name_id varchar(20) NOT NULL, primaryName varchar(255), " +
            "birthYear int, deathYear int, primaryProfession varchar(255));");
            wr.AppendLine(Environment.NewLine);

            wr.AppendLine($"CREATE TABLE knownfor (name_id varchar(20) NOT NULL, title_id varchar(20) NOT NULL);");

            wr.AppendLine("COMMIT;");
            //   wr.AppendLine("SET autocommit=1;");

            ret = await Execute(wr.ToString());
            wr = new StringBuilder();
            /// wr.AppendLine("SET autocommit=0;");

            orglines.RemoveAt(0);
            int totalCount = 0;
            int counter = 0;
            foreach (var line in orglines)
            {
                counter++;

                String[] lineArr = line.Split("\t");
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("INSERT INTO names (title_id, primaryName, birthYear, deathYear, primaryProfession) VALUES (");
                    var nameId = wrapOrNull(lineArr[0]);
                    sb.AppendLine(nameId);
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[1]));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[2].Replace("\\N", "NULL"));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[3].Replace("\\N", "NULL"));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[4]));
                    sb.AppendLine(");");

                    wr.Append(sb.ToString());
                    wr.Append(Environment.NewLine);


                    if (!lineArr[5].Equals("\\N"))
                    {
                        var s = lineArr[5].Split(',');
                        foreach (var item in s)
                        {

                            sb = new StringBuilder();
                            sb.AppendLine("INSERT IGNORE INTO knownfor (title_id, name_id) VALUES (");
                            var titleId = (wrapOrNull(item));
                            sb.AppendLine(titleId);
                            sb.AppendLine(", ");
                            sb.AppendLine(nameId);
                            sb.AppendLine(");");
                            wr.Append(sb.ToString());
                            wr.Append(Environment.NewLine);
                        }
                    }

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)
                {
                    wr.AppendLine("COMMIT;");
                    ret = ret && await Execute(wr.ToString());
                    totalCount = totalCount + counter;
                    counter = 0;
                    wr = new StringBuilder();
                    if (progress != null)
                    {
                        var percent = percentCalc.CalculatePercent(totalCount, orglines.Count());
                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name} , maxLines: {maxLines} ");
                        try
                        {
                            if (progress != null) progress.Report(percent);
                        }
                        catch (Exception ex)
                        { }
                        await Task.Delay(1);
                    }
                    if (!ret)
                    {
                        return ret;
                    }
                }

            }

            wr.AppendLine("COMMIT;");
            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (title_id);");
            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");
            return ret;


        }
        public async Task<bool> processCrew(int maxLines, string name, List<string> orglines, IProgress<int> progress = null)
        {
            bool ret = false;
            SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(orglines.Count)} {name}, maxLines: {maxLines} is going to ble slow, estimating Total  : {StringUtilities.FormatMinutesAsHoursAndMinutes((int)(orglines.Count * 0.5) / 60)}");

            //Opprett tabeller, men ikke primary/foregin keys
            ret = await _mySqlTableOperationsController.CrewCreateTable(create: true, keys: false);

            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");

            int totalCount = 0;
            int counter = 0;
            int longestLine = 0;
            int lastPercent = 0;
            orglines.RemoveAt(0);//ta bort header linjen

            foreach (var chunkLines in orglines.Chunk(maxLines))
            {
                counter += maxLines;
                HashSet<TitlePerson> directorsList = new();
                HashSet<TitlePerson> writersList = new();
                foreach (var line in chunkLines)
                {
                    String[] lineArr = line.Split("\t");
                    string titleId = lineArr[0];
                    var personList = lineArr[1].Split(",").Take(15).ToList();
                    foreach (var s in personList)
                    {
                        if (s.Length < 3) continue;
                        directorsList.Add(new TitlePerson() { ImdbId = titleId, NameId = s });

                    }
                    personList = lineArr[2].Split(",").Take(15).ToList();
                    foreach (var s in personList)
                    {
                        if (s.Length < 3) continue;
                        writersList.Add(new TitlePerson() { ImdbId = titleId, NameId = s });
                    }

                }
                StringBuilder sb = new StringBuilder();
                wr.AppendLine("INSERT   INTO directors (title_id,name_id) VALUES ");
                foreach (var tp in directorsList)
                {
                    try
                    {
                        sb = new StringBuilder();
                        sb.Append(" (");
                        sb.Append(wrapOrNull((tp.ImdbId)));
                        sb.Append(", ");
                        sb.Append(wrapOrNull(tp.NameId));
                        sb.AppendLine("),");
                        wr.Append(sb.ToString().Trim());

                    }
                    catch (Exception ex)
                    {
                        SetStatusLabelText(ex.Message);
                    }
                }
                // Remove the last character 
                ret = ret && await Execute(wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "; COMMIT;");
                if (!ret) SetStatusLabelText($"Error on line {wr.ToString()}");
                wr = new StringBuilder();
                wr.AppendLine("INSERT   INTO writers (title_id, name_id) VALUES ");

                foreach (var wl in writersList)
                {
                    sb = new StringBuilder();
                    sb.Append(" (");
                    sb.Append(wrapOrNull(wl.ImdbId));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(wl.NameId));
                    sb.Append("),");
                    wr.AppendLine(sb.ToString());

                }
                // Remove the last character
                ret = ret && await Execute(wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "; COMMIT;");
                if (!ret) SetStatusLabelText($"Error on line {wr.ToString()}");
                wr = new StringBuilder();

                totalCount = totalCount + counter;
                counter = 0;

                if (progress != null)
                {
                    var percent = percentCalc.CalculatePercent(totalCount, orglines.Count());

                    if (percent <= 0)
                    {

                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name}, maxLines: {maxLines} size = {StringUtilities.FormatBigNumbers(totalCount)}");
                    }
                    if (lastPercent < percent)
                    {
                        SetStatusLabelText($"[Info] Now {percent}%. Commited totally {StringUtilities.FormatBigNumbers(totalCount)} lines. From now on only reporting percent changes. ( Estimated time left: {StringUtilities.FormatMinutesAsHoursAndMinutes((int)((orglines.Count - totalCount) * 0.3) / 60)})");
                        GC.Collect(); //kan ikke skade?


                        if (progress != null) progress.Report(percent);
                        Thread.Sleep(2);
                        lastPercent = percent;
                    }
                }
                if (!ret)
                {
                    break;
                    //     return ret;
                }
            }

            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine("COMMIT;");

            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");

            //Opprett primary/foregin keys
            ret = ret && await _mySqlTableOperationsController.CrewCreateTable(create: false, keys: true);
            return ret;
        }

        public async Task<bool> processPrincipals(int maxLines, string name, List<string> orglines, IProgress<int> progress = null, HugeFileCounter hugeFileCounter = null)
        {
            string tableName = "principals";
            bool ret = true;
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            if (orglines.First().Contains("tconst"))
            {
                orglines.RemoveAt(0);
                await _mySqlTableOperationsController.PrincipalsCreateTable(true, false);
            }
            string insertSql = $"INSERT INTO {tableName} (title_id,  ordering, name_id, category,job, characters) VALUES ";
            wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");
            wr.AppendLine(insertSql);

            int totalCount = 0;
            int counter = 0;


            foreach (var line in orglines)
            {
                counter++;
                if (string.IsNullOrEmpty(line)) continue;
                String[] lineArr = line.Split("\t");
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("(");
                    sb.Append(wrapOrNull(lineArr[0]));
                    sb.Append(", ");
                    sb.Append(lineArr[1]);
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[2]));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[3]));
                    sb.Append(", ");
                    sb.Append(wrapOrNull(lineArr[4]));
                    sb.Append(", ");
                    String character = lineArr[5];
                    character = character.Replace("[", "");
                    character = character.Replace("\\", "");
                    character = character.Replace("\"", "");
                    character = character.Replace("]", "");
                    sb.Append(wrapOrNull(character));
                    sb.AppendLine("),");

                    wr.Append(sb.ToString());

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)
                {
                    // Remove the last character
                    ret = ret && await Execute(wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "; COMMIT;");
                    if (!ret) SetStatusLabelText($"Error in {tableName} {line}");
                    totalCount = totalCount + counter;

                    wr = new StringBuilder(insertSql);

                    if (!ret)
                    {
                        return ret;
                    }
                }

            }
            ret = await Execute(wr.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + "; COMMIT;");


            var percent = percentCalc.CalculatePercent(hugeFileCounter.currentCount, hugeFileCounter.totalCount);
            if (progress != null)
            {
                SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(hugeFileCounter.currentCount)}/{StringUtilities.FormatBigNumbers(hugeFileCounter.totalCount)} - {percent}%) {name}");
                try
                {
                    if (progress != null) progress.Report(percent); await Task.Delay(2);
                }
                catch (Exception ex)
                { }
                counter = 0;
            }
            if (percentCalc.CalculatePercent(hugeFileCounter.currentCount, hugeFileCounter.totalCount) >= 99)
            {
                wr = new StringBuilder();
                wr.AppendLine("COMMIT;");
                wr.AppendLine("SET autocommit=1;");
                ret = ret && await Execute(wr.ToString());
                SetStatusLabelText($"[DONE] (100%) {name}");
            }
            return ret;
        }

        public async Task<bool> processTitleLanguage(int maxLines, string name, List<string> orglines, IProgress<int> progress = null, HugeFileCounter hugeFileCounter = null)
        {
            string tableName = "titles_lang";
            bool ret = false;
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            if (orglines.First().Contains("titleId"))
            {
                orglines.RemoveAt(0);
                wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");

                wr.AppendLine(@$"CREATE TABLE {tableName}   (title_id varchar(20) NOT NULL, 
                    ordering int, title text NOT NULL, 
                    region varchar(5), language varchar(5), 
                    types varchar(40),  attributes varchar(255), isOriginalTitle boolean); ");
                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");

                wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (title_id, ordering);");

                //      wr.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT fk_titles_lang_title_id FOREIGN KEY (title_id) REFERENCES titles(id) ON DELETE CASCADE;");

                wr.AppendLine("COMMIT;");

                ret = await Execute(wr.ToString());
            }
            wr = new StringBuilder();
            /// wr.AppendLine("SET autocommit=0;");


            int totalCount = 0;
            int counter = 0;
            foreach (var line in orglines)
            {
                counter++;

                String[] lineArr = line.Split("\t");
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("INSERT IGNORE INTO titles_lang (title_id, ordering, title, region, ");
                    sb.AppendLine("language, types, attributes, isOriginalTitle) VALUES (");
                    sb.AppendLine(wrapOrNull(lineArr[0]));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[1]);
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[2]));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[3]));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[4]));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[5]));
                    sb.AppendLine(", ");
                    sb.AppendLine(wrapOrNull(lineArr[6]));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[7]);
                    sb.AppendLine(");");


                    wr.Append(sb.ToString());
                    wr.Append(Environment.NewLine);

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)
                {
                    wr.AppendLine("COMMIT;");
                    ret = ret && await Execute(wr.ToString());
                    totalCount = totalCount + counter;

                    wr = new StringBuilder();
                    if (progress != null)
                    {
                        var percent = percentCalc.CalculatePercent(hugeFileCounter.currentCount, hugeFileCounter.totalCount);
                        //     SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name}");
                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(hugeFileCounter.currentCount)}/{StringUtilities.FormatBigNumbers(hugeFileCounter.totalCount)} - {percent}%) {name}");
                        try
                        {
                            if (progress != null) progress.Report(percent); await Task.Delay(5);
                        }
                        catch (Exception ex)
                        { }
                        counter = 0;
                    }
                    if (!ret)
                    {
                        return ret;
                    }
                }

            }

            wr.AppendLine("COMMIT;");
            wr.AppendLine("SET autocommit=1;");
            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");
            return ret;


        }


        public async Task<bool> processRatings(int maxLines, string name, List<string> orglines, IProgress<int> progress = null)
        {
            string tableName = "ratings";
            bool ret = false;
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");
            wr.AppendLine($"CREATE TABLE {tableName} (id varchar(20) NOT NULL, averageRating float NOT NULL, numVotes int);");
            wr.AppendLine(Environment.NewLine);
            wr.AppendLine("COMMIT;");

            ret = await Execute(wr.ToString());
            wr = new StringBuilder();
            /// wr.AppendLine("SET autocommit=0;");

            orglines.RemoveAt(0);
            int totalCount = 0;
            int counter = 0;
            foreach (var line in orglines)
            {
                counter++;

                String[] lineArr = line.Split("\t");
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("INSERT INTO ratings (id, averageRating, numVotes) VALUES (");
                    sb.AppendLine(wrapOrNull(lineArr[0]));
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[1]);
                    sb.AppendLine(", ");
                    sb.AppendLine(lineArr[2]);
                    sb.AppendLine(");");

                    wr.Append(sb.ToString());
                    wr.Append(Environment.NewLine);

                }
                catch (Exception ex)
                {
                    SetStatusLabelText(ex.Message);
                }

                if (counter == maxLines)
                {
                    wr.AppendLine("COMMIT;");
                    ret = ret && await Execute(wr.ToString());
                    totalCount = totalCount + counter;
                    counter = 0;
                    wr = new StringBuilder();
                    if (progress != null)
                    {
                        var percent = percentCalc.CalculatePercent(totalCount, orglines.Count());
                        SetStatusLabelText($"[INSERT] ({StringUtilities.FormatBigNumbers(totalCount)}/{StringUtilities.FormatBigNumbers(orglines.Count())} - {percent}%) {name}");
                        try
                        {
                            if (progress != null) progress.Report(percent);
                        }
                        catch (Exception ex)
                        { }
                        await Task.Delay(1);
                    }
                    if (!ret)
                    {
                        return ret;
                    }
                }

            }

            wr.AppendLine("COMMIT;");
            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (id);");
            ret = ret && await Execute(wr.ToString());
            SetStatusLabelText($"[DONE] (100%) {name}");
            return ret;


        }
        private static String wrapOrNull(String s)
        {
            if (s.Equals("\\N"))
            {
                return "NULL";
            }
            else
            {
                return "\"" + s.Replace("\"", "'") + "\"";
            }
        }
        #endregion
        private void SetStatusLabelText(string message)
        {
            try
            {
                Task.Delay(1).GetAwaiter().GetResult();
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetStatusLabelText(message); }
                    ));
                else
                {

                    toolStripLabel1.Text = message;
                    //   richTextBox1.AppendText($"{Environment.NewLine}[{DateTime.Now.ToString()}] {message}");
                }
            }
            catch (Exception ex)
            { }
        }

        public async Task<bool> Execute(string sql, bool verbose = false)
        {
            try
            {
                return await _mySqlTableOperationsController.Execute(sql);
            }
            catch (Exception ex)
            {
                if (verbose)
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                return false;
            }  
        }

    }
}