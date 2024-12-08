using Ookii.Dialogs.WinForms;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Kolibri.net.Common.FormUtilities.Forms
{
    public class FolderUtilities
    {
        public static bool RenameDir(string originalName, string newName)
        {
            bool ret = true;
            try
            {
                System.IO.Directory.Move(originalName, newName);
                ret = true;
            }

            catch (Exception e)
            {
                ret = false;
                Console.WriteLine(e.ToString());
            }
            return ret;
        }


        public static DirectoryInfo GetCurrentDirectory()
        {
            //This will give us the full name path of the executable file:
            //i.e. C:\Program Files\MyApplication\MyApplication.exe
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //This will strip just the working path name:
            //C:\Program Files\MyApplication
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            return new DirectoryInfo(strWorkPath);
        }


        #region copy
        /********************************************************************/

        public static void CopyDirectoryAndFiles(string sourceDirectory, string destinationDirectory, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDirectory);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found:{dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDirectory);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDirectory = Path.Combine(destinationDirectory, subDir.Name);
                    CopyDirectoryAndFiles(subDir.FullName, newDestinationDirectory, true);
                }
            }
        }

        /// <summary>
        /// Use This class to Perform Xcopy
        /// </summary>
        public static bool XCopy(DirectoryInfo source, DirectoryInfo destination, bool deleteSource)
        {
            bool ret = false;
            try
            {
                if (!destination.Exists)
                    destination.Create();
                ProcessXcopy(source.FullName, destination.FullName);

                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            if (ret && deleteSource)
            {
                destination.Refresh();
                if (destination.Exists)
                {
                    //Skummel! - sjekk om filene eksisterer!
                    source.Delete(true);
                }
            }
            return ret;
        }



        /// <summary>
        /// Method to Perform Xcopy to copy files/folders from Source machine to Target Machine
        /// </summary>
        /// <param name="SolutionDirectory"></param>
        /// <param name="TargetDirectory"></param>
        private static void ProcessXcopy(string SolutionDirectory, string TargetDirectory)
        {

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I /z";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }


        /********************************************************************/
        #endregion
        public static void DeleteDirectory(DirectoryInfo dir)
        {
            DeleteDirectory(dir.FullName);
        }


        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        /// <summary>
        /// Flytter en directory fra Source til Destination vha RoboCopy
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <returns></returns>
        public static bool MoveDirectory(DirectoryInfo sourceDirName, DirectoryInfo destDirName)
        {
            var args = "/COPYALL /E /dcopy:T /SECFIX"; //https://adamtheautomator.com/robocopy/
            if (!sourceDirName.FullName.StartsWith("\\\\") && !destDirName.FullName.StartsWith("\\\\"))
            { args = "/MIR /MOVE"; }
            else { return XCopy(sourceDirName, destDirName, true); }

            string errors = string.Empty;
            int ret = 0;
            //https://learn.microsoft.com/en-us/answers/questions/226496/c-how-copy-multiple-files-in-multiple-thread-to-sp
            using (Process p = new Process())
            {
                p.StartInfo.Arguments = string.Format($@"/C ROBOCOPY ""{sourceDirName.FullName}"" ""{destDirName.FullName}"" {args}");
                p.StartInfo.FileName = "CMD.EXE";
                p.StartInfo.UseShellExecute = false;
                //      p.StartInfo.RedirectStandardError = true;
                p.Start();
                //      errors = p.StandardError.ReadToEnd();
                p.WaitForExit();
                ret = p.ExitCode;

            }
            return ret == 1;
        }

        public static DirectoryInfo LetOppMappe(string initalPath, string text = "Browse for folder")
        {

            var fbdEX = new VistaFolderBrowserDialog();
            fbdEX.RootFolder = Environment.SpecialFolder.MyComputer;
            
            fbdEX.SelectedPath = initalPath;
            //  SendKeys.SendWait("{TAB}{TAB}{DOWN}{UP}");
            SendKeys.Send("{TAB}{TAB}{RIGHT}");
            if (fbdEX.ShowDialog() == DialogResult.OK)
            {
                return new DirectoryInfo(fbdEX.SelectedPath);

            }
            else return null;
        }

  
        internal static class PInvoke
        {
            static PInvoke() { }

            public delegate int BrowseFolderCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

            internal static class User32
            {
                [DllImport("user32.dll", CharSet = CharSet.Auto)]
                public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

                [DllImport("user32.dll", CharSet = CharSet.Auto)]
                public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

                [DllImport("user32.dll", SetLastError = true)]
                //public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
                //public static extern IntPtr FindWindowEx(HandleRef hwndParent, HandleRef hwndChildAfter, string lpszClass, string lpszWindow);
                public static extern IntPtr FindWindowEx(HandleRef hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

                [DllImport("user32.dll", SetLastError = true)]
                public static extern Boolean SetWindowText(IntPtr hWnd, String text);
            }

            [ComImport, Guid("00000002-0000-0000-c000-000000000046"), SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IMalloc
            {
                [PreserveSig]
                IntPtr Alloc(int cb);
                [PreserveSig]
                IntPtr Realloc(IntPtr pv, int cb);
                [PreserveSig]
                void Free(IntPtr pv);
                [PreserveSig]
                int GetSize(IntPtr pv);
                [PreserveSig]
                int DidAlloc(IntPtr pv);
                [PreserveSig]
                void HeapMinimize();
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public class BROWSEINFO
            {
                public IntPtr Owner;
                public IntPtr pidlRoot;
                public IntPtr pszDisplayName;
                public string Title;
                public int Flags;
                public BrowseFolderCallbackProc callback;
                public IntPtr lParam;
                public int iImage;
            }



            [SuppressUnmanagedCodeSecurity]
            internal static class Shell32
            {
                // Methods
                [DllImport("shell32.dll", CharSet = CharSet.Auto)]
                public static extern IntPtr SHBrowseForFolder([In] PInvoke.BROWSEINFO lpbi);
                [DllImport("shell32.dll")]
                public static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] PInvoke.IMalloc[] ppMalloc);
                [DllImport("shell32.dll", CharSet = CharSet.Auto)]
                public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);
                [DllImport("shell32.dll")]
                public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);
            }
        }
        #region search extensions    /**********************************************/
        /// <summary>
        /// and first time call looks like this: var extensionsList = SearchAccessibleFilesNoDistinct("rootAddress", null);        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static IEnumerable<string> SearchAccessibleFiles(string root, string searchTerm)
        {
            var files = new List<string>();

            foreach (var file in Directory.EnumerateFiles(root).Where(m => m.Contains(searchTerm)))
            {
                string extension = Path.GetExtension(file);
                files.Add(extension);
            }
            foreach (var subDir in Directory.EnumerateDirectories(root))
            {
                try
                {
                    files.AddRange(SearchAccessibleFiles(subDir, searchTerm));
                }
                catch (UnauthorizedAccessException ex)
                {
                    // ...
                }
            }
            return files.Distinct().ToList();
        }
        /// <summary>
        ///   you can simply remove.Where(m => m.Contains(searchTerm)) part for searching without a search term.
        /// Edit If you don't want to use .Distict() and want to check duplicates on the go you can try this method:
        /// </summary>

        public static IEnumerable<string> SearchAccessibleFilesNoDistinct(string root, List<string> files = null)
        {
            if (files == null)
                files = new List<string>();

            foreach (var file in Directory.EnumerateFiles(root))
            {
                string extension = Path.GetExtension(file);
                if (!files.Contains(extension))
                    files.Add(extension);
            }
            foreach (var subDir in Directory.EnumerateDirectories(root))
            {
                try
                {
                    SearchAccessibleFilesNoDistinct(subDir, files);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw ex;
                }
            }
            return files;
        }
        #endregion /***********************************************/


        public static string CompareFolders(DirectoryInfo source, DirectoryInfo destination)
        {
            StringBuilder ret = new StringBuilder();
            System.IO.DirectoryInfo dir1 = source;
            System.IO.DirectoryInfo dir2 = destination;

            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories).OrderBy (f => f.FullName);

            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*",System.IO.SearchOption.AllDirectories).OrderBy (f => f.FullName);    

            bool IsInDestination = false;
            bool IsInSource = false;

            foreach (System.IO.FileInfo s in list1)
            {
                IsInDestination = true;

                foreach (System.IO.FileInfo s2 in list2)
                {
                    if (s.Name == s2.Name)
                    {
                        IsInDestination = true;
                        break;
                    }
                    else
                    {
                        IsInDestination = false;
                    }
                }

                if (!IsInDestination)
                {
                    //System.IO.File.Copy(s.FullName, System.IO.Path.Combine(destination.FullName, s.Name), true);
                    ret.AppendLine($"{s.FullName} is not in {destination.FullName}");
                }
            }

            list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            bool areIdentical = list1.SequenceEqual(list2, new FileCompare());

            if (!areIdentical)
            {
                foreach (System.IO.FileInfo s in list2)
                {
                    IsInSource = true;

                    foreach (System.IO.FileInfo s2 in list1)
                    {
                        if (s.Name == s2.Name)
                        {
                            IsInSource = true;
                            break;
                        }
                        else
                        {
                            IsInSource = false;
                        }
                    }

                    if (!IsInSource)
                    {
                        //System.IO.File.Copy(s.FullName, System.IO.Path.Combine(source.FullName, s.Name), true);
                        ret.AppendLine($"{s.Name} is not in {source.FullName}");
                    }
                }
            }
            return ret.ToString();
        }

    }
    internal class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            return (f1.Name == f2.Name);
        }
        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s = fi.Name;
            return s.GetHashCode();
        }
    }

} 

 