using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.net.Common.Utilities
{
    public class FileUtilities
    {
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        private static extern long FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);
        public static DataSet FileInfoAsDataSet(List<FileInfo> infos, bool fileVersion)
        {
            DataSet ret = null;

            FileInfo[] files = infos.ToArray();
            DataTable regTabell = null;

            foreach (FileInfo info in files)
            {
                if (regTabell == null)
                {
                    regTabell = FileInfoTable(fileVersion);
                };

                regTabell.Rows.Add(FileInfoAsDataRow(info, fileVersion));
            }
            if (regTabell != null)
            {
                ret = new DataSet();
                ret.Tables.Add(regTabell.Copy());
            }
            return ret;
        }





        /// <summary>
        /// Metode som henter ett array av FileInfo fra en oppgitt sti
        /// </summary>
        /// <param name="path">sti filene skal søkes etter i </param>
        /// <param name="filefilter">ett filter det skal søkes etter, f.eks *.exe</param>
        /// <param name="searchAllDirs">om en skal søke i underkataloger i tillegg til oppgitt sti</param>
        /// <returns></returns>
        public static FileInfo[] HentFiler(string path, string filefilter, bool searchAllDirs = true)
        {
            SearchOption opt = SearchOption.TopDirectoryOnly;

            if (searchAllDirs)
                opt = SearchOption.AllDirectories;
            DirectoryInfo directoryInfoApplikasjon = new System.IO.DirectoryInfo(path);
            //            Awesome.File.????????_??????.txt


            FileInfo[] fileInfoListe = directoryInfoApplikasjon.GetFiles(filefilter, opt);
            return fileInfoListe;
        }
        /// <summary>
        /// Metode som henter ett array av FileInfo fra en oppgitt sti
        /// </summary>
        /// <param name="path">sti filene skal søkes etter i </param>
        /// <param name="filefilter">ett filter det skal søkes etter, f.eks *.exe</param>
        /// <param name="searchAllDirs">om en skal søke i underkataloger i tillegg til oppgitt sti</param>
        /// <returns></returns>
        public static FileInfo[] HentFiler(DirectoryInfo path, string filefilter, bool searchAllDirs = true)
        {
            SearchOption opt = SearchOption.TopDirectoryOnly;

            if (searchAllDirs)
                opt = SearchOption.AllDirectories;
            DirectoryInfo directoryInfoApplikasjon = new System.IO.DirectoryInfo(path.FullName);
            //            Awesome.File.????????_??????.txt


            FileInfo[] fileInfoListe = directoryInfoApplikasjon.GetFiles(filefilter, opt);
            return fileInfoListe;
        }
        /// <summary>
        /// Metode som henter ett array av FileInfo fra en oppgitt sti
        /// </summary>
        /// <param name="path">sti filene skal søkes etter i </param>
        /// <param name="filefilter">ett filter det skal søkes etter, f.eks *.exe</param>
        /// <param name="searchAllDirs">om en skal søke i underkataloger i tillegg til oppgitt sti</param>
        /// <returns></returns>
        public static FileInfo[] HentFiler(DirectoryInfo path, string[] filefilter, bool searchAllDirs = true)
        {
            SearchOption opt = SearchOption.TopDirectoryOnly;

            if (searchAllDirs)
                opt = SearchOption.AllDirectories;
            DirectoryInfo directoryInfoApplikasjon = new System.IO.DirectoryInfo(path.FullName);
            //            Awesome.File.????????_??????.txt


            List<FileInfo> fileInfoListe = new List<FileInfo>();
            for (int i = 0; i < filefilter.Length; i++)
            {


                foreach (var item in directoryInfoApplikasjon.GetFiles(filefilter[i], opt))
                {
                    fileInfoListe.Add(item);
                }
            }

            return fileInfoListe.ToArray();
        }



        #region FileInfo datatable with FileVersionInfo
        /// <summary>
        /// Creates an empty fileInfo table
        /// </summary>
        /// <returns></returns>
        public static DataTable FileInfoTable()
        {
            return FileInfoTable(false);
        }
        public static DataTable FileInfoTable(bool fileVersion)
        {
            DataTable regTabell = null;

            Type objectType = typeof(FileInfo);
            PropertyInfo[] properties = objectType.GetProperties();

            if (regTabell == null)
            {
                regTabell = new DataTable(objectType.Name);
                foreach (PropertyInfo prop in properties)
                {
                    regTabell.Columns.Add(prop.Name);
                }
                if (fileVersion)
                {
                    properties = typeof(FileVersionInfo).GetProperties();
                    foreach (PropertyInfo prop in properties)
                    {
                        regTabell.Columns.Add(prop.Name);
                    }
                }

            }
            return regTabell;
        }

        public static object[] FileInfoAsDataRow(FileInfo info)
        {
            return FileInfoAsDataRow(info, false);
        }

        public static object[] FileInfoAsDataRow(FileInfo info, bool fileVersion)
        {

            Type objectType = info.GetType();
            //ConstructorInfo[] info = deltaker..GetConstructors();
            //MethodInfo[] methods = objectType.GetMethods();
            PropertyInfo[] properties = objectType.GetProperties();


            DataTable regTabell = FileInfoTable(fileVersion);

            DataRow row = regTabell.NewRow();
            foreach (PropertyInfo prop in properties)
            {
                row[prop.Name] = prop.GetValue(info, null);
            }
            if (fileVersion)
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(info.FullName);
                properties = fileInfo.GetType().GetProperties();
                foreach (PropertyInfo prop in properties)
                {
                    row[prop.Name] = prop.GetValue(fileInfo, null);
                }
            }
            //  regTabell.Rows.Add(row);
            return row.ItemArray;

        }

        public static DataSet FileInfoAsDataSet(DirectoryInfo directory)
        {
            return FileInfoAsDataSet(directory, false);
        }
        public static DataSet FileInfoAsDataSet(DirectoryInfo directory, bool fileVersion)
        {
            DataSet ret = null;


            DirectoryInfo m_dir = directory;
            FileInfo[] files = m_dir.GetFiles("*.*", SearchOption.AllDirectories);
            DataSet ds = FileInfoAsDataSet(new List<FileInfo>(files), fileVersion);
            ds.DataSetName = FileUtilities.SafeFileName(m_dir.Name).Replace(" ", string.Empty);

            DataTable regTabell = null;

            foreach (FileInfo info in files)
            {
                if (regTabell == null)
                {
                    regTabell = FileInfoTable(fileVersion);
                };

                regTabell.Rows.Add(FileInfoAsDataRow(info, fileVersion));
            }
            ret = ds;
            return ret;
        }
        #endregion
        public static string FilePathToFileUrl(string filePath)
        {
            StringBuilder uri = new StringBuilder();
            foreach (char v in filePath)
            {
                if ((v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9') ||
                  v == '+' || v == '/' || v == ':' || v == '.' || v == '-' || v == '_' || v == '~' ||
                  v > '\xFF')
                {
                    uri.Append(v);
                }
                else if (v == Path.DirectorySeparatorChar || v == Path.AltDirectorySeparatorChar)
                {
                    uri.Append('/');
                }
                else
                {
                    uri.Append(String.Format("%{0:X2}", (int)v));
                }
            }
            if (uri.Length >= 2 && uri[0] == '/' && uri[1] == '/') // UNC path
                uri.Insert(0, "file:");
            else
                uri.Insert(0, "file:///");
            return uri.ToString();
        }
        public static string relativeFilePath(FileInfo fullFilePath, DirectoryInfo relativeRoot)
        {
            string ret = string.Empty;
            Uri fullPath = new Uri(fullFilePath.FullName, UriKind.Absolute);
            Uri relRoot = new Uri(relativeRoot.FullName, UriKind.Absolute);

            ret = relRoot.MakeRelativeUri(fullPath).ToString();
            if (ret.Contains("20")) ret = Uri.UnescapeDataString(ret); //hersens whitespaces...
            return ret;
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            try
            {
                using (StreamReader reader = new StreamReader(filename, Encoding.Default, true))
                {
                    reader.Peek(); // you need this!
                    var encoding = reader.CurrentEncoding;
                    return encoding;
                }
            }
            catch (Exception)
            { }

            return Encoding.ASCII;
        }

        public static string ReadTextFile(string fullfilepath)
        {
            string ret = string.Empty;
            try
            {
                // create reader & open file
                StreamReader tr = new StreamReader(fullfilepath);
                ret = tr.ReadToEnd();
                tr.Close();
            }

            catch (Exception)
            { }
            return ret;
        }

        public static Byte[] ReadFile(string sPath)
        {
            return ByteUtilities.ReadFile(sPath);
        }

        /// <summary>
        /// Metode som leser en flatfil
        /// </summary>
        /// <param name="fullfilepath">filens filbane</param>
        /// <param name="encoding">filens encoding, kan være null</param>
        /// <param name="columnWidths">array med bredde på hver kolonne</param>
        /// <returns></returns>
        public static DataSet ReadFlatTextFile(string fullfilepath, Encoding encoding, int[] columnWidths)
        {
            DataSet ret = null;
            string[] header;
            Encoding enc = encoding;
            if (enc == null)
                enc = Encoding.GetEncoding("iso-8859-1");

            try
            {
                if (!File.Exists(fullfilepath))
                    return ret;

                string[] source = File.ReadAllLines(fullfilepath, enc);

                header = new string[columnWidths.Length];

                for (int i = 0; i < columnWidths.Length; i++)
                {
                    header[i] = "Column" + i;
                }

                ret = new DataSet();
                DataTable regTabell = null;
                foreach (string info in source)
                {
                    if (regTabell == null)
                    {
                        regTabell = ret.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                        foreach (string prop in header)
                        {
                            regTabell.Columns.Add(prop);
                        }
                    }
                    DataRow row = regTabell.NewRow();
                    int index = 0;
                    int pos = 0;
                    foreach (int width in columnWidths)
                    {
                        row[index] = info.Substring(pos, width);
                        pos += width;
                        index++;
                    }
                    regTabell.Rows.Add(row);
                }
            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;

        }


        public static string GetByteSize(FileInfo info)
        {
            return ByteUtilities.GetByteSize(info.Length);
        }


        private static void DeleteFiles(FileInfo[] fileInfoListe)
        {
            string useridAD = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];

            if (fileInfoListe.Length > 0)
            {
                foreach (FileInfo fileInfo in fileInfoListe)
                {
                    string tempDir = Path.GetTempPath().TrimEnd(new char[] { '\\' });
                    if (fileInfo.Directory.ToString().TrimEnd(new char[] { '\'' }).Equals(tempDir))
                    {

                        try
                        {
                            fileInfo.Delete(); // sletter uten VB og dermed uten recyclebin
                        }
                        catch (Exception)
                        { }
                        break;

                    }
                    else
                    {
                        try
                        {
                            try
                            { //use this code to move file to recycle bin
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(fileInfo.FullName
                                    , Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs
                                    , Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                            }
                            catch (Exception)
                            {
                                fileInfo.Delete(); // sletter  uten recyclebin }
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Metode som skriver ett bytearray til fil
        /// </summary>
        /// <param name="buff">bytearray som skal skrives</param>
        /// <param name="fileName">filnavn med fullstendig sti og ekstensjon.</param>
        /// <returns></returns>
        public static bool WriteByteArrayToFile(byte[] buff, string fileName)
        {
            bool response = false;

            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(buff);
                bw.Close();
                response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Metode som sjekker en string for ulovlige filsystem tegn, og returnerer en trygg string der ulovlige tegn er fjernet.
        /// </summary>
        /// <param name="filename">string som skal sjekkes for ulovlige tegn</param>
        /// <returns>samme string som ble sendt inn, eller samme men fjernet ulovlige tegn</returns>
        public static string SafeFileName(string filename)
        {
            string safe = filename;
            try
            {
                foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars())
                {
                    safe = safe.Replace(lDisallowed.ToString(), "");
                }
            }
            catch (Exception)
            {
            }
            return safe;
        }

        public static string AutomaticFilenameIfExists(string fullPath) {
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }


        /// <summary>
        /// Writes (owerwrites an existing) file
        /// </summary>
        /// <param name="output">What to write</param>
        /// <param name="fullFilePath">path to write to </param>
        /// <param name="enc">encoding to use</param>
        /// <returns></returns>
        public static bool WriteStringToFile(object output, string fullFilePath, Encoding enc)
        {
            bool ret = true;


            try
            {
                using (StreamWriter writer = new StreamWriter(fullFilePath, false, enc))
                {
                    writer.Write(output.ToString());
                    writer.Close();
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }

        public static bool WriteStringToFile(Object output, string fullFilePath)
        {
            bool ret = true;
            try
            {
                using (StreamWriter outfile = new StreamWriter(fullFilePath, false))
                {
                    outfile.Write(output.ToString());
                }
            }
            catch (Exception)
            {

                ret = false;
            }
            return ret;
        }

        public static bool StartExecuteCommandFile(string commandfile)
        {
            return StartExecuteCommandFile(commandfile, false);
        }

        /// <summary>
        /// Metode som eksekverer en batch fil. Den setter arbeidssti og venter på at prosessen blir ferdig, og fanger opp feilmeldinger
        /// </summary>
        /// <param name="commandfile">full filsti til batchfilen</param>
        public static bool StartExecuteCommandFile(string commandfile, bool waitForExit)
        {
            bool ret = true;
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + commandfile);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.WorkingDirectory = Path.GetDirectoryName(commandfile);
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;


            process = Process.Start(processInfo);
            if (waitForExit)
            {
                process.WaitForExit();

                // *** Read the streams ***
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                exitCode = process.ExitCode;

                Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
                ret = string.IsNullOrEmpty(error);
            }
            return ret;
        }

        /// <summary>
        /// Enkel process start, lager en batch fil av processtostart og kaller StartExecuteCommandFile 
        /// </summary>
        /// <param name="processToStart">Innholdet i en batch fil som skal kjøres. Livsfarlig å eksponere til andre ennn Dev gjengen som er kjekke folk.</param>
        public static void Start(string processToStart)
        {
            FileInfo batchInfo = FileUtilities.GetTempFile("bat");
            if (!batchInfo.Directory.Exists) batchInfo.Create();
            File.WriteAllText(batchInfo.FullName, processToStart);
            FileUtilities.StartExecuteCommandFile(batchInfo.FullName);
        }

        public static void Start(Uri url) {
            Process.Start(new ProcessStartInfo(url.AbsoluteUri) { UseShellExecute = true });
        }
        public static void Start(FileInfo info)
        {
            Process.Start(new ProcessStartInfo(info.FullName) { UseShellExecute = true });
        }
        public static void Start(DirectoryInfo directoryInfo)
        {
            Process.Start(new ProcessStartInfo(directoryInfo.FullName) { UseShellExecute = true });
        }

        public static FileInfo GetTempFile(string extension = null)
        {
            FileInfo ret = new FileInfo(System.IO.Path.GetTempFileName());
            if (extension == null) return ret;
            string temp = extension.TrimStart('.');
            return new FileInfo(ret.FullName.Replace(".tmp", "." + temp));
        }

        /// <summary>
        /// Enkelt kall for å starte ett program med en fil som parameter.
        /// Alternative kall for de ulike metodene:
        /// string command = string.Format(@"""{0}"" ""{1}""", @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" , info.FullName);
        /// FileUtilities.Start(new FileInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"), 
        /// string.Format(@"""{0}""", info.FullName));
        /// FileUtilities.Start(command);//admin
        /// </summary>
        /// <param name="executable">programmet som skal startes</param>
        /// <param name="fileAsParameter">filen som skal brukes som parameter ved oppstart av programmet.</param>
        public static void Start(FileInfo executable, FileInfo fileAsParameter)
        {

            FileUtilities.Start(executable, string.Format(@"""{0}""", fileAsParameter.FullName));
        }

        /// <summary>
        /// Enkel Process.Start, som tar inn en bunch med parametere og skiller dem med mellomrom.
        /// </summary>
        /// <param name="executable">filen som skal kjøres</param>
        /// <param name="args">parametere, skilt med mellomrom</param>
        public static void Start(FileInfo executable, params string[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = string.Format(@"""{0}""", executable.FullName);
            if (args != null && args.Length > 0)
                startInfo.Arguments = string.Join(" ", args);
            Process.Start(startInfo);
        }

        /// <summary>
        /// Enkel process start , returnerer når prosessen er ferdig
        /// </summary>
        /// <param name="processToStart"></param>
        public static void StartWaitForExit(string processToStart)
        {
            var process = Process.Start(processToStart);
            process.WaitForExit();
        }
        public static bool DownloadFile(string url, string path)
        {
            try
            {
                FileInfo info = new FileInfo(path);
                if (!info.Directory.Exists) { info.Directory.Create(); }

                System.Net.WebClient client = new WebClient();
                
                client.DownloadFile(url, path);
            }
            catch (Exception e)
            { }
            return File.Exists(path);
        }

        public static string DownloadUrlToString(string url, Encoding enc)
        {
            string ret = string.Empty;
            //http://www.devsource.com/c/a/Languages/Pulling-Data-From-Internet-URLs-in-C/
            try
            {
                WebClient wc = new WebClient();
                byte[] data = wc.DownloadData(url);
                string strData = enc.GetString(data);

                ret = strData;
            }
            catch (Exception e)
            { }
            return ret;

        }

        public static bool DownloadPostnrfile(string url, string path)
        {
            try
            {
                string dl = string.Empty;
                Encoding enc = Encoding.GetEncoding("iso-8859-1");
                dl = DownloadUrlToString(url, enc);
                //dl = StringUtilities.RemoveControlChars(dl);
                if (!string.IsNullOrEmpty(dl))
                {
                    string content = Regex.Replace(dl, ";", "\t");
                    // Utilities.FileUtilities.WriteStringToFile(content, path);

                    StreamWriter writer = new StreamWriter(path, false, enc);
                    writer.Write(content);
                    writer.Close();

                }
            }
            catch (Exception e)
            { }
            return File.Exists(path);
        }

        public static string GetFilesize(long totalbytes)
        {
            return ByteUtilities.GetByteSize(totalbytes);
        }

        /// <summary>
        /// Henter filer fra en oppgitt sti, basert på ett eller flere  søkekriterie,  searchpattern kan splittes opp vha pipe ("|")
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">Split('|')</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> GetFiles(DirectoryInfo path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(System.IO.Directory.GetFiles(path.FullName, sp, searchOption));
            files.Sort();
            return files;
        }

        /// <summary>
        /// Metode som henter ett array av FileInfo fra en oppgitt sti, searchpattern kan splittes opp vha pipe ("|"), eksempel   var searchStr = "*." + string.Join("|*.", Utilities.FileUtilities.MoviesFileExt());
        /// </summary>
        /// <param name="path">sti filene skal søkes etter i </param>
        /// <param name="filefilter">ett filter det skal søkes etter, f.eks *.exe</param>
        /// <param name="searchAllDirs">om en skal søke i underkataloger i tillegg til oppgitt sti</param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(DirectoryInfo path, string searchPattern, bool searchAllDirs)
        {
            DirectoryInfo directoryInfoApplikasjon = path;

            ArrayList fileInfoListe = new ArrayList();
            string[] searchPatterns = searchPattern.Split('|');
            foreach (string sp in searchPatterns)
            {
                var temp = directoryInfoApplikasjon.GetFiles(sp, searchAllDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                if (temp != null)
                {
                    foreach (var item in temp)
                    {
                        fileInfoListe.Add(item);
                    }
                }
            }
            try
            { fileInfoListe.Sort(); }
            catch (Exception) { }
            return (FileInfo[])fileInfoListe.ToArray(typeof(FileInfo));
        }


        /// <summary>
        /// Get the Filter string for all extensions in string array.
        /// This can be used directly to the FileDialog class Filter Property.
        /// </summary>
        /// <returns>A Filter formated string to be used as filter for File dialogs (open/save) </returns>
        public static string GetFileDialogFilter(string[] extensions)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string item in extensions)
            {
                dic.Add(item.ToUpper(), item.ToLower());
            }
            return GetFileDialogFilter(dic);

        }

        public static string GetFileDialogFilter(Dictionary<string, string> extensions)
        {
            StringBuilder allImageExtensions = new StringBuilder();
            string separator = "";

            Dictionary<string, string> images = extensions;

            StringBuilder sb = new StringBuilder();
            images.Add("All", "*");
            foreach (KeyValuePair<string, string> image in images)
            {
                sb.AppendFormat("|{0} Files|*.{1}", image.Key, image.Value);
            }
            return sb.ToString().TrimStart('|');
        }

        public static string MimeTypeToFileExtension(string mimetype)
        {
            return MimeTypes.FindExtension(mimetype);
        }



        public static DataTable GetFileVersionInfo(FileInfo info)
        {
            DataTable ret = new DataTable();
            ret.Columns.AddRange(


                new DataColumn[] {
                                new DataColumn("assemblyVersion"),
                                new DataColumn("fileVersion"),
                                new DataColumn("productVersion")
                                });
            ret.Rows.Add(Assembly.LoadFile(info.FullName).GetName().Version.ToString()
                , FileVersionInfo.GetVersionInfo(info.FullName).FileVersion
                , FileVersionInfo.GetVersionInfo(info.FullName).ProductVersion);
            return ret;
        }

        public static void ShowOpenWithDialog(string path)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            Process.Start("rundll32.exe", args);
        }


        public static void OpenFolderHighlightFile(FileInfo info)
        {
            // suppose that we have a test.txt at E:\
            string filePath = info.FullName;
            if (!File.Exists(filePath))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select, \"" + filePath + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);


        }


        public static bool IsDangerousFileExtension(string ext)
        {
            return (DangerousFileExtensions().FindIndex(x => x.Equals(ext.TrimStart(".".ToCharArray()), StringComparison.OrdinalIgnoreCase)) != -1);
        }


        private static List<string> DangerousFileExtensions()
        {
            List<string> ret = new List<string>() { "386", "9",
                                    "aepl", "aru", "atm", "aut",
                                    "bat", "bhx", "bin", "bkd", "blf", "bll", "bmw", "boo", "bps", "bqf", "buk", "bup", "bxz",
                                    "cc", "ce0", "ceo", "cfxxe", "chm", "cih", "cla", "class", "cmd", "com", "cpl", "ctbl", "cxq", "cyw",
                                    "dbd", "delf", "dev", "dlb", "dli", "dll", "dllx", "dom", "drv", "dx", "dxz", "dyv", "dyz",
                                    "exe", "exe1", "exe_renamed", "ezt", "fag", "fjl", "fnr", "fuj", "gzquar",
                                    "hlp", "hlw", "hsq", "hts", "iva", "iws", "jar", "js",
                                    "kcd", "let", "lik", "lkh", "lnk", "lok", "lpaq5",
                                    "mcq" ,"mfu", "mjg", "mjz", "nls", "oar", "ocx", "osa", "ozd",
                                    "pcx", "pgm", "php3", "pid", "pif", "plc", "pr",
                                    "qit", "qrn", "rhk", "rna", "rsc_tmp",
                                    "s7p", "scr", "scr", "shs", "ska", "smm", "smtmp", "sop", "spam", "ssy", "swf", "sys",
                                    "tko", "tps", "tsa", "tti", "txs", "upa", "uzy",
                                    "vb", "vba", "vbe", "vbs", "vbx", "vexe", "vxd", "vzr",
                                    "wlpginstall", "wmf", "ws", "wsc", "wsf", "wsh",
                                    "xdu", "xir", "xlm", "xlv", "xnt", "xnxx", "xtbl",
                                    "zix", "zvz",};
            return ret;
        }

        public static bool HasExecutable(string path)
        {
            var executable = FindExecutable(path);
            return !string.IsNullOrEmpty(executable);
        }

        public static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            FindExecutable(path, string.Empty, executable);
            return executable.ToString();
        }

        
       


        public static void OpenFolderMarkFile(FileInfo info)
        {

            string filePath = info.FullName;
            if (!info.Exists)
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select, \"" + filePath + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        public static DataSet LagRapport(string directory)
        {
            DirectoryInfo m_dir = new DirectoryInfo(directory);
            DataSet ret = null;
            DataSet ds = new DataSet("Filinfo");
            DataTable regTabell = null;

            FileInfo[] files = m_dir.GetFiles("*", SearchOption.AllDirectories);
            //Console.WriteLine("Total number of bmp files", bmpfiles.Length);
            foreach (FileInfo info in files)
            {
                Type objectType = info.GetType();
                //ConstructorInfo[] info = deltaker..GetConstructors();
                //MethodInfo[] methods = objectType.GetMethods();
                PropertyInfo[] properties = objectType.GetProperties();
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(info.FullName);

                if (regTabell == null)
                {
                    regTabell = ds.Tables.Add("filer");
                    foreach (PropertyInfo prop in properties)
                    {
                        regTabell.Columns.Add(prop.Name);
                    }
                    properties = fileInfo.GetType().GetProperties();
                    foreach (PropertyInfo prop in properties)
                    {
                        regTabell.Columns.Add(prop.Name);
                    }
                    properties = objectType.GetProperties();
                }
                DataRow row = regTabell.NewRow();
                foreach (PropertyInfo prop in properties)
                {
                    row[prop.Name] = prop.GetValue(info, null);
                }
                properties = fileInfo.GetType().GetProperties();
                foreach (PropertyInfo prop in properties)
                {
                    row[prop.Name] = prop.GetValue(fileInfo, null);
                }
                regTabell.Rows.Add(row);
            }
            ret = ds;
            return ret;
        }
        public static List<string> MoviesCommonFileExt()
        {
            var ret = new List<string>() { "avi", "mkv", "mp4", "mpg", "mpeg" };
            return ret.Distinct().ToList();
        }
        public static List<string> MoviesFileExt()
        {
            return new List<string>() { "3g2", "3gp", "amv", "asf", "avi", "drc", "flv", "flv", "flv", "f4v", "f4p", "f4a", "f4b", "gif", "gifv", "m4v", "mkv", "mng", "mov", "qt", "mp4", "m4p", "m4v", "mpg", "mp2", "mpeg", "mpe", "mpv", "mpg", "mpeg", "m2v", "MTS", "M2TS", "TS", "mxf", "nsv", "ogv", "ogg", "rm", "rmvb", "roq", "svi", "vob", "webm", "wmv", "yuv","ts" };
        }
        public static List<string> PictureFileExt()
        {

            return new List<string>() { "JPEG", "JPG", "PNG", "GIF", "WEBP", "TIFF", "PSD", "RAW", "BMP", "HEIF", "INDD", "SVG", "AI", "EPS", "PDF" };
        }




        /// <summary>
        ///  Stream.Read doesn't guarantee that it will read everything it's asked for. If you're reading from a network stream, for example, it may read one packet's worth and then return, even if there will be more data soon. BinaryReader.Read will keep going until the end of the stream or your specified size, but you still have to know the size to start with.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Metode som tar inn en filsti, og sletter alle PDF filer i denne som er laget av samme bruker som er pålogget.
        /// </summary>
        /// <param name="directory">Stien en vil slette fra</param>
        /// <returns></returns>
        public static bool SlettBrukerPDFFiler(string directory)
        {

            bool ret = false;
            try
            {
                DirectoryInfo directoryInfoApplikasjon = new DirectoryInfo(directory);
                FileInfo[] fileInfoListe = directoryInfoApplikasjon.GetFiles("*.PDF", SearchOption.TopDirectoryOnly);
                DeleteFiles(fileInfoListe);
                fileInfoListe = directoryInfoApplikasjon.GetFiles("*.TMP", SearchOption.TopDirectoryOnly);
                DeleteFiles(fileInfoListe);

                //slett fra temp også hvis ulik 
                directoryInfoApplikasjon = new DirectoryInfo(Path.GetTempPath());
                if (!directory.Equals(directoryInfoApplikasjon.ToString()))
                {
                    fileInfoListe = directoryInfoApplikasjon.GetFiles("*.TMP", SearchOption.TopDirectoryOnly);
                    DeleteFiles(fileInfoListe);
                    fileInfoListe = directoryInfoApplikasjon.GetFiles("*.PDF", SearchOption.TopDirectoryOnly);
                    DeleteFiles(fileInfoListe);
                    //slett annen drit også
                    if (Environment.UserName.Equals("aostby"))
                    {
                        fileInfoListe = directoryInfoApplikasjon.GetFiles("*.tmp.cvr", SearchOption.TopDirectoryOnly);
                        DeleteFiles(fileInfoListe);
                        fileInfoListe = directoryInfoApplikasjon.GetFiles("*.od", SearchOption.TopDirectoryOnly);
                        DeleteFiles(fileInfoListe);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                ret = false;
            }
            return ret;

        }

        public static DirectoryInfo LetOppMappe(string initialpath = null)
        {
            DirectoryInfo ret = null;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrWhiteSpace(initialpath) && new DirectoryInfo(initialpath).Exists)
            {
                dialog.SelectedPath = initialpath;

                SendKeys.Send("{TAB}{TAB}{RIGHT}");
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SendKeys.Send("{TAB}{TAB}{RIGHT}");


                ret = new DirectoryInfo(dialog.SelectedPath);

            }
            return ret;
        }
        public static void DeleteEmptyDirs(DirectoryInfo dir)
        {
            DeleteEmptyDirs(dir.FullName);
        }

        public static void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException("Starting directory is a null reference or an empty string", "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        public static FileInfo GetFile(string title, DirectoryInfo dir, string filter)
        {
            FileInfo ret = null;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = $"{title}";
            ofd.Filter = $"{filter} files (*.{filter})|*.{filter}|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            if (dir != null && dir.Exists)
                ofd.InitialDirectory = dir.FullName;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ret = new FileInfo(ofd.FileName);
            }
            return ret;
        }

        public static FileInfo GetFile(string title, string filter)
        {
            FileInfo ret = null;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = $"{title}";
            ofd.Filter = $"{filter} files (*.{filter})|*.{filter}|All files (*.*)|*.*";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ret = new FileInfo(ofd.FileName);
            }
            return ret;
        }

        public static FileInfo GetFile(string filter)
        {
            FileInfo ret = null;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = $"{filter} files (*.{filter})|*.{filter}|All files (*.*)|*.*";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ret = new FileInfo(ofd.FileName);
            }
            return ret;
        }

        public static void CopyFolderToFolder(DirectoryInfo sourcePath, DirectoryInfo targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath.FullName, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath.FullName, targetPath.FullName));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath.FullName, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath.FullName, targetPath.FullName), true);
            }
        }


        public static void MoveCopy(FileInfo source, FileInfo target)
        {
            // assuming that target directory exists
            if (!target.Directory.Exists)
                target.Directory.Create();

            if (!target.Exists)
                File.Copy(source.FullName, target.FullName);
            else
                for (int i = 1; ; ++i)
                {
                    String name = Path.Combine(
                      Path.GetDirectoryName(target.FullName),
                      Path.GetFileNameWithoutExtension(target.FullName) + String.Format(" - Copy({0})", i) +
                      Path.GetExtension(target.FullName));

                    if (!File.Exists(name))
                    {
                        File.Copy(source.FullName, name);

                        break;
                    }
                }
        }



        #region Zip-related methods
        /// <summary>
        /// Metode som zipper en fil ved å ta inn en full filsti. filen blir laget med ekstenssjonen .zip som output
        /// </summary>
        /// <param name="inputFilePath">filen som skal zippes</param>
        /// <param name="password">Hvis null krypteres ikke filen, ellers blir oppgitt string brukt til passord</param>
        /// <returns></returns>
        public static bool ZipFile(string inputFilePath, string password)
        {
            bool ret = true;
            try
            {
                FileStream ostream;
                byte[] obuffer;

                FileInfo filinfo = new FileInfo(inputFilePath);
                string outPath = inputFilePath.Replace(filinfo.Extension, ".zip");// File.inputFolderPath + @"\" + outputPathAndFile;
                ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
                if (!string.IsNullOrEmpty(password))
                    oZipStream.Password = password;
                oZipStream.SetLevel(9); // maximum compression
                ZipEntry oZipEntry;
                oZipEntry = new ZipEntry(filinfo.Name);
                oZipStream.PutNextEntry(oZipEntry);
                ostream = File.OpenRead(filinfo.FullName);
                obuffer = new byte[ostream.Length];
                ostream.Read(obuffer, 0, obuffer.Length);
                oZipStream.Write(obuffer, 0, obuffer.Length);

                oZipStream.Finish();
                oZipStream.Close();

            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static void ZipFiles(string inputFolderPath, string outputPathAndFile, string password)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            string outPath = inputFolderPath + @"\" + outputPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
            if (password != null && password != String.Empty)
                oZipStream.Password = password;
            oZipStream.SetLevel(9); // maximum compression
            ZipEntry oZipEntry;
            foreach (string Fil in ar) // for each file, generate a zipentry
            {
                oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                oZipStream.PutNextEntry(oZipEntry);

                if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                {
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[ostream.Length];
                    ostream.Read(obuffer, 0, obuffer.Length);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                }
            }
            oZipStream.Finish();
            oZipStream.Close();
        }

        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                if (Directory.GetDirectories(Dir).Length == 0)
                // if directory is completely empty, add it
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils; // return file list
        }

        public static bool UnZipFiles(string zipPathAndFile, string outputFolder, string password)
        {
            bool ret = true;
            try
            {
                UnZipFiles(zipPathAndFile, outputFolder, password, false);
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipPathAndFile));
            if (password != null && password != String.Empty)
                s.Password = password;
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();
            if (deleteZipFile)
                File.Delete(zipPathAndFile);
        }

        public static dynamic FileInfoJsonAsDynamicObject(FileInfo info)
        {
            string json = System.IO.File.ReadAllText(info.FullName);
            var dynamic = JsonConvert.DeserializeObject(json);
            return dynamic;
        }


        #endregion

        public static string ExportToFormats(object data)
        {
            return ExportToFormats(data, null);
        }
        public static string ExportToFormats(object data, FileInfo initialFileInfo)
        {
            if (data.Equals(typeof(byte[])))
            { throw new Exception("Binary files not supported yet"); }

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            // sfd.Filter = Kolibri.Utilities.FileUtilities.GetFileDialogFilter(new string[] { "txt", "xml", "cs", "xlsx" });
            sfd.Filter = @"
JSON TEXT files (*.JSON)|*.JSON|
JSON Parameters files (*.JSON)|*.JSON|
Plain xml files (*.xml)|*.xml|
Nested xml files (*.xml)|*.xml|
XLXS files (*.xlsx)|*.xlsx|
C# cs files (*.cs)|*.cs|
UNLOAD to CSV files (*.csv)|*.csv|
SQL query files (*.sql)|*.sql|
Text files (*.txt)|*.txt|
All files (*.*)|*.*";


            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("JSON Text", "JSON");
            dic.Add("xml", "xml");
            dic.Add("C# Class library (cs)", "cs");
            dic.Add("Excel", "xml");
           
            if (initialFileInfo != null)
            {
                sfd.FileName = Path.GetFileName(initialFileInfo.Name);//   Path.GetFileNameWithoutExtension(initialFileInfo.Name);
                if (initialFileInfo.Directory.Exists) sfd.InitialDirectory = initialFileInfo.Directory.FullName;

                sfd.FilterIndex = 9;
            }
            else if (data.GetType().Equals(typeof(DataTable)))
            {
                if (!string.IsNullOrEmpty((data as DataTable).TableName))
                    sfd.FileName = (data as DataTable).TableName;

            }
            else
            {
                sfd.FilterIndex = 9;
                try
                {
                    XDocument xdoc = XDocument.Parse(data.ToString());
                    sfd.FileName = xdoc.Root.Name.LocalName;
                }
                catch (Exception)
                { }
            }

            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return null;
            try
            {
                string result = null;
                DataTable table = null;
                DataSet ds = null;
                if (data.GetType().Equals(typeof(DataTable)) || data.GetType().Equals(typeof(DataSet)))
                {
                    if (data.GetType().Equals(typeof(DataTable)))
                    {
                        table = data as DataTable;
                        if (table.DataSet == null)
                        { ds = new DataSet(); ds.Tables.Add(table); }
                        else ds = table.DataSet;
                    }
                    else if (data.GetType().Equals(typeof(DataSet)))
                    {
                        ds = data as DataSet;
                        table = ds.Tables[ds.Tables.Count - 1];
                    }
                }

                if (sfd.FilterIndex != 4 & sfd.FilterIndex != 7 & sfd.FilterIndex != 9)
                {

               
                    if (ds == null)
                    {
                        ds = new DataSet();
                        if (data.GetType().Equals(typeof(XmlDocument)))
                            ds.ReadXml(new StringReader((data as XmlDocument).OuterXml));
                        else
                            ds.ReadXml(new StringReader(data.ToString()));

                        table = ds.Tables[ds.Tables.Count - 1];
                    }
                }

                switch (sfd.FilterIndex)
                {
                    case 1: result = DataSetUtilities.DataTableTojson(table); break;
                    case 2: result = DataSetUtilities.CreateJsonParameters(table); break;
                    case 3: result = DataSetUtilities.DataSetToXML(ds).OuterXml; break;
                    case 4: result = XDocument.Parse(data.ToString()).ToString(); break;

                  
                    case 5:


                        //  result = Utilities.ExcelUtilities.ExcelXMLSpreadsheet(ds); break;
                        //     ExcelUtilities.GenerateExcel2007(new FileInfo(sfd.FileName), ds);
                        throw new NotImplementedException(System.Reflection.MethodBase.GetCurrentMethod().Name);

                   
                        break;

                    default:
                        //  result = Utilities.DataSetUtilities.DataSetToCSV(table, Convert.ToChar(9).ToString()); break;
                        result = data.ToString();
                        break;
                }
                if (sfd.FilterIndex != 5)
                {
                    if ((sfd.FilterIndex != 7) && string.IsNullOrEmpty(result))
                    {
                        //    toolStripStatusLabel1.Text = "Could not write file - no value found.";
                        throw new Exception("Fallthrough - no string value to write");
                    }
                }
                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    if (sfd.FilterIndex == 5)
                    {
                        //ExcelUtilities.OpenFileInExcel(sfd.FileName);
                        throw new NotImplementedException(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                    else if (sfd.FilterIndex != 7)
                    {
                        StreamWriter SW;
                        SW = File.CreateText(sfd.FileName);

                        SW.Write(result);
                        SW.Flush();
                        SW.Close();

                        FileUtilities.OpenFolderHighlightFile(new FileInfo(sfd.FileName));
                    }

                    else
                        // Process.Start(sfd.FileName);
                        FileUtilities.OpenFolderHighlightFile(new FileInfo(sfd.FileName));

                    //  toolStripStatusLabel1.Text = sfd.FileName + " written.";
                    //  MetaDataController.SetFilterIndexToRegistry(m_controller.Systemnr, sfd.FilterIndex);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);// throw;
                                            // toolStripStatusLabel1.Text = ex.Message;
            }
            return sfd.FileName;
        }
    }
}    

