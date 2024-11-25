using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static net.sf.saxon.expr.Component;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Kolibri.Common.Utilities
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

            FolderBrowserDialogEx fbdEX = new FolderBrowserDialogEx(text);
            fbdEX.RootFolder = Environment.SpecialFolder.MyComputer;
            fbdEX.ShowFullPathInEditBox = true;
            fbdEX.SelectedPath = initalPath;
            //  SendKeys.SendWait("{TAB}{TAB}{DOWN}{UP}");
            SendKeys.Send("{TAB}{TAB}{RIGHT}");
            if (fbdEX.ShowDialog() == DialogResult.OK)
            {
                return new DirectoryInfo(fbdEX.SelectedPath);

            }
            else return null;
        }

        // FolderBrowserDialogEx.cs
        //
        // This class comes from the DotNetZip project: http://dotnetzip.codeplex.com
        // It is licensed under the MS-PL: http://dotnetzip.codeplex.com/license
        //
        // A replacement for the builtin System.Windows.Forms.FolderBrowserDialog class.
        // This one includes an edit box, and also displays the full path in the edit box. 
        //
        // based on code from http://support.microsoft.com/default.aspx?scid=kb;[LN];306285 
        // 
        // 20 Feb 2009
        //
        // ========================================================================================
        // Example usage:
        // 
        // string _folderName = "c:\\dinoch";
        // private void button1_Click(object sender, EventArgs e)
        // {
        //     _folderName = (System.IO.Directory.Exists(_folderName)) ? _folderName : "";
        //     var dlg1 = new Ionic.Utils.FolderBrowserDialogEx
        //     {
        //         Description = "Select a folder for the extracted files:",
        //         ShowNewFolderButton = true,
        //         ShowEditBox = true,
        //         //NewStyle = false,
        //         SelectedPath = _folderName,
        //         ShowFullPathInEditBox= false,
        //     };
        //     dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;
        // 
        //     var result = dlg1.ShowDialog();
        // 
        //     if (result == DialogResult.OK)
        //     {
        //         _folderName = dlg1.SelectedPath;
        //         this.label1.Text = "The folder selected was: ";
        //         this.label2.Text = _folderName;
        //     }
        // }
        //





        //[Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultEvent("HelpRequest"), SRDescription("DescriptionFolderBrowserDialog"), DefaultProperty("SelectedPath")]
        public class FolderBrowserDialogEx : System.Windows.Forms.CommonDialog
        {
            private static readonly int MAX_PATH = 260;

            // Fields
            private PInvoke.BrowseFolderCallbackProc _callback;
            private string _descriptionText;
            private Environment.SpecialFolder _rootFolder;
            private string _selectedPath;
            private bool _selectedPathNeedsCheck;
            private bool _showNewFolderButton;
            private bool _showEditBox;
            private bool _showBothFilesAndFolders;
            private bool _newStyle = true;
            private bool _showFullPathInEditBox = true;
            private bool _dontIncludeNetworkFoldersBelowDomainLevel;
            private int _uiFlags;
            private IntPtr _hwndEdit;
            private IntPtr _rootFolderLocation;

            // Events
            //[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public new event EventHandler HelpRequest
            {
                add
                {
                    base.HelpRequest += value;
                }
                remove
                {
                    base.HelpRequest -= value;
                }
            }

            // ctor
            public FolderBrowserDialogEx(string descriptionText = null)
            {
                this.Reset();
                if (descriptionText != null)
                {
                    this._descriptionText = descriptionText;
                }
            }

            // Factory Methods
            public static FolderBrowserDialogEx PrinterBrowser()
            {
                FolderBrowserDialogEx x = new FolderBrowserDialogEx();
                // avoid MBRO comppiler warning when passing _rootFolderLocation as a ref:
                x.BecomePrinterBrowser();
                return x;
            }

            public static FolderBrowserDialogEx ComputerBrowser()
            {
                FolderBrowserDialogEx x = new FolderBrowserDialogEx();
                // avoid MBRO comppiler warning when passing _rootFolderLocation as a ref:
                x.BecomeComputerBrowser();
                return x;
            }


            // Helpers
            private void BecomePrinterBrowser()
            {
                _uiFlags += BrowseFlags.BIF_BROWSEFORPRINTER;
                Description = "Select a printer:";
                PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.PRINTERS, ref this._rootFolderLocation);
                ShowNewFolderButton = false;
                ShowEditBox = false;
            }

            private void BecomeComputerBrowser()
            {
                _uiFlags += BrowseFlags.BIF_BROWSEFORCOMPUTER;
                Description = "Select a computer:";
                PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, CSIDL.NETWORK, ref this._rootFolderLocation);
                ShowNewFolderButton = false;
                ShowEditBox = false;
            }


            private class CSIDL
            {
                public const int PRINTERS = 4;
                public const int NETWORK = 0x12;
            }

            private class BrowseFlags
            {
                public const int BIF_DEFAULT = 0x0000;
                public const int BIF_BROWSEFORCOMPUTER = 0x1000;
                public const int BIF_BROWSEFORPRINTER = 0x2000;
                public const int BIF_BROWSEINCLUDEFILES = 0x4000;
                public const int BIF_BROWSEINCLUDEURLS = 0x0080;
                public const int BIF_DONTGOBELOWDOMAIN = 0x0002;
                public const int BIF_EDITBOX = 0x0010;
                public const int BIF_NEWDIALOGSTYLE = 0x0040;
                public const int BIF_NONEWFOLDERBUTTON = 0x0200;
                public const int BIF_RETURNFSANCESTORS = 0x0008;
                public const int BIF_RETURNONLYFSDIRS = 0x0001;
                public const int BIF_SHAREABLE = 0x8000;
                public const int BIF_STATUSTEXT = 0x0004;
                public const int BIF_UAHINT = 0x0100;
                public const int BIF_VALIDATE = 0x0020;
                public const int BIF_NOTRANSLATETARGETS = 0x0400;
            }

            private static class BrowseForFolderMessages
            {
                // messages FROM the folder browser
                public const int BFFM_INITIALIZED = 1;
                public const int BFFM_SELCHANGED = 2;
                public const int BFFM_VALIDATEFAILEDA = 3;
                public const int BFFM_VALIDATEFAILEDW = 4;
                public const int BFFM_IUNKNOWN = 5;

                // messages TO the folder browser
                public const int BFFM_SETSTATUSTEXT = 0x464;
                public const int BFFM_ENABLEOK = 0x465;
                public const int BFFM_SETSELECTIONA = 0x466;
                public const int BFFM_SETSELECTIONW = 0x467;
            }

            private int FolderBrowserCallback(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
            {
                switch (msg)
                {
                    case BrowseForFolderMessages.BFFM_INITIALIZED:
                        if (this._selectedPath.Length != 0)
                        {
                            PInvoke.User32.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_SETSELECTIONW, 1, this._selectedPath);
                            if (this._showEditBox && this._showFullPathInEditBox)
                            {
                                // get handle to the Edit box inside the Folder Browser Dialog
                                _hwndEdit = PInvoke.User32.FindWindowEx(new HandleRef(null, hwnd), IntPtr.Zero, "Edit", null);
                                PInvoke.User32.SetWindowText(_hwndEdit, this._selectedPath);
                            }
                        }
                        break;

                    case BrowseForFolderMessages.BFFM_SELCHANGED:
                        IntPtr pidl = lParam;
                        if (pidl != IntPtr.Zero)
                        {
                            if (((_uiFlags & BrowseFlags.BIF_BROWSEFORPRINTER) == BrowseFlags.BIF_BROWSEFORPRINTER) ||
                                ((_uiFlags & BrowseFlags.BIF_BROWSEFORCOMPUTER) == BrowseFlags.BIF_BROWSEFORCOMPUTER))
                            {
                                // we're browsing for a printer or computer, enable the OK button unconditionally.
                                PInvoke.User32.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_ENABLEOK, 0, 1);
                            }
                            else
                            {
                                IntPtr pszPath = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
                                bool haveValidPath = PInvoke.Shell32.SHGetPathFromIDList(pidl, pszPath);
                                String displayedPath = Marshal.PtrToStringAuto(pszPath);
                                Marshal.FreeHGlobal(pszPath);
                                // whether to enable the OK button or not. (if file is valid)
                                PInvoke.User32.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_ENABLEOK, 0, haveValidPath ? 1 : 0);

                                // Maybe set the Edit Box text to the Full Folder path
                                if (haveValidPath && !String.IsNullOrEmpty(displayedPath))
                                {
                                    if (_showEditBox && _showFullPathInEditBox)
                                    {
                                        if (_hwndEdit != IntPtr.Zero)
                                            PInvoke.User32.SetWindowText(_hwndEdit, displayedPath);
                                    }

                                    if ((_uiFlags & BrowseFlags.BIF_STATUSTEXT) == BrowseFlags.BIF_STATUSTEXT)
                                        PInvoke.User32.SendMessage(new HandleRef(null, hwnd), BrowseForFolderMessages.BFFM_SETSTATUSTEXT, 0, displayedPath);
                                }
                            }
                        }
                        break;
                }
                return 0;
            }

            private static PInvoke.IMalloc GetSHMalloc()
            {
                PInvoke.IMalloc[] ppMalloc = new PInvoke.IMalloc[1];
                PInvoke.Shell32.SHGetMalloc(ppMalloc);
                return ppMalloc[0];
            }

            public override void Reset()
            {
                this._rootFolder = (Environment.SpecialFolder)0;
                this._descriptionText = string.Empty;
                this._selectedPath = string.Empty;
                this._selectedPathNeedsCheck = false;
                this._showNewFolderButton = true;
                this._showEditBox = true;
                this._newStyle = true;
                this._dontIncludeNetworkFoldersBelowDomainLevel = false;
                this._hwndEdit = IntPtr.Zero;
                this._rootFolderLocation = IntPtr.Zero;
            }

            protected override bool RunDialog(IntPtr hWndOwner)
            {
                bool result = false;
                if (_rootFolderLocation == IntPtr.Zero)
                {
                    PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int)this._rootFolder, ref _rootFolderLocation);
                    if (_rootFolderLocation == IntPtr.Zero)
                    {
                        PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, 0, ref _rootFolderLocation);
                        if (_rootFolderLocation == IntPtr.Zero)
                        {
                            throw new InvalidOperationException("FolderBrowserDialogNoRootFolder");
                        }
                    }
                }
                _hwndEdit = IntPtr.Zero;
                //_uiFlags = 0;
                if (_dontIncludeNetworkFoldersBelowDomainLevel)
                    _uiFlags += BrowseFlags.BIF_DONTGOBELOWDOMAIN;
                if (this._newStyle)
                    _uiFlags += BrowseFlags.BIF_NEWDIALOGSTYLE;
                if (!this._showNewFolderButton)
                    _uiFlags += BrowseFlags.BIF_NONEWFOLDERBUTTON;
                if (this._showEditBox)
                    _uiFlags += BrowseFlags.BIF_EDITBOX;
                if (this._showBothFilesAndFolders)
                    _uiFlags += BrowseFlags.BIF_BROWSEINCLUDEFILES;


                if (Control.CheckForIllegalCrossThreadCalls && (Application.OleRequired() != ApartmentState.STA))
                {
                    throw new ThreadStateException("DebuggingException: ThreadMustBeSTA");
                }
                IntPtr pidl = IntPtr.Zero;
                IntPtr hglobal = IntPtr.Zero;
                IntPtr pszPath = IntPtr.Zero;
                try
                {
                    PInvoke.BROWSEINFO browseInfo = new PInvoke.BROWSEINFO();
                    hglobal = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
                    pszPath = Marshal.AllocHGlobal(MAX_PATH * Marshal.SystemDefaultCharSize);
                    this._callback = new PInvoke.BrowseFolderCallbackProc(this.FolderBrowserCallback);
                    browseInfo.pidlRoot = _rootFolderLocation;
                    browseInfo.Owner = hWndOwner;
                    browseInfo.pszDisplayName = hglobal;
                    browseInfo.Title = this._descriptionText;
                    browseInfo.Flags = _uiFlags;
                    browseInfo.callback = this._callback;
                    browseInfo.lParam = IntPtr.Zero;
                    browseInfo.iImage = 0;
                    pidl = PInvoke.Shell32.SHBrowseForFolder(browseInfo);
                    if (((_uiFlags & BrowseFlags.BIF_BROWSEFORPRINTER) == BrowseFlags.BIF_BROWSEFORPRINTER) ||
                    ((_uiFlags & BrowseFlags.BIF_BROWSEFORCOMPUTER) == BrowseFlags.BIF_BROWSEFORCOMPUTER))
                    {
                        this._selectedPath = Marshal.PtrToStringAuto(browseInfo.pszDisplayName);
                        result = true;
                    }
                    else
                    {
                        if (pidl != IntPtr.Zero)
                        {
                            PInvoke.Shell32.SHGetPathFromIDList(pidl, pszPath);
                            this._selectedPathNeedsCheck = true;
                            this._selectedPath = Marshal.PtrToStringAuto(pszPath);
                            result = true;
                        }
                    }
                }
                finally
                {
                    PInvoke.IMalloc sHMalloc = GetSHMalloc();
                    sHMalloc.Free(_rootFolderLocation);
                    _rootFolderLocation = IntPtr.Zero;
                    if (pidl != IntPtr.Zero)
                    {
                        sHMalloc.Free(pidl);
                    }
                    if (pszPath != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pszPath);
                    }
                    if (hglobal != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(hglobal);
                    }
                    this._callback = null;
                }
                return result;
            }

            // Properties
            //[SRDescription("FolderBrowserDialogDescription"), SRCategory("CatFolderBrowsing"), Browsable(true), DefaultValue(""), Localizable(true)]

            /// <summary>
            /// This description appears near the top of the dialog box, providing direction to the user.
            /// </summary>
            public string Description
            {
                get
                {
                    return this._descriptionText;
                }
                set
                {
                    this._descriptionText = (value == null) ? string.Empty : value;
                }
            }

            //[Localizable(false), SRCategory("CatFolderBrowsing"), SRDescription("FolderBrowserDialogRootFolder"), TypeConverter(typeof(SpecialFolderEnumConverter)), Browsable(true), DefaultValue(0)]
            public Environment.SpecialFolder RootFolder
            {
                get
                {
                    return this._rootFolder;
                }
                set
                {
                    if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(Environment.SpecialFolder));
                    }
                    this._rootFolder = value;
                }
            }

            //[Browsable(true), SRDescription("FolderBrowserDialogSelectedPath"), SRCategory("CatFolderBrowsing"), DefaultValue(""), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Localizable(true)]

            /// <summary>
            /// Set or get the selected path.  
            /// </summary>
            public string SelectedPath
            {
                get
                {
                    if (((this._selectedPath != null) && (this._selectedPath.Length != 0)) && this._selectedPathNeedsCheck)
                    {
                        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this._selectedPath).Demand();
                        this._selectedPathNeedsCheck = false;
                    }
                    return this._selectedPath;
                }
                set
                {
                    this._selectedPath = (value == null) ? string.Empty : value;
                    this._selectedPathNeedsCheck = true;
                }
            }

            //[SRDescription("FolderBrowserDialogShowNewFolderButton"), Localizable(false), Browsable(true), DefaultValue(true), SRCategory("CatFolderBrowsing")]

            /// <summary>
            /// Enable or disable the "New Folder" button in the browser dialog.
            /// </summary>
            public bool ShowNewFolderButton
            {
                get
                {
                    return this._showNewFolderButton;
                }
                set
                {
                    this._showNewFolderButton = value;
                }
            }

            /// <summary>
            /// Show an "edit box" in the folder browser.
            /// </summary>
            /// <remarks>
            /// The "edit box" normally shows the name of the selected folder.  
            /// The user may also type a pathname directly into the edit box.  
            /// </remarks>
            /// <seealso cref="ShowFullPathInEditBox"/>
            public bool ShowEditBox
            {
                get
                {
                    return this._showEditBox;
                }
                set
                {
                    this._showEditBox = value;
                }
            }

            /// <summary>
            /// Set whether to use the New Folder Browser dialog style.
            /// </summary>
            /// <remarks>
            /// The new style is resizable and includes a "New Folder" button.
            /// </remarks>
            public bool NewStyle
            {
                get
                {
                    return this._newStyle;
                }
                set
                {
                    this._newStyle = value;
                }
            }


            public bool DontIncludeNetworkFoldersBelowDomainLevel
            {
                get { return _dontIncludeNetworkFoldersBelowDomainLevel; }
                set { _dontIncludeNetworkFoldersBelowDomainLevel = value; }
            }

            /// <summary>
            /// Show the full path in the edit box as the user selects it. 
            /// </summary>
            /// <remarks>
            /// This works only if ShowEditBox is also set to true. 
            /// </remarks>
            public bool ShowFullPathInEditBox
            {
                get { return _showFullPathInEditBox; }
                set { _showFullPathInEditBox = value; }
            }

            public bool ShowBothFilesAndFolders
            {
                get { return _showBothFilesAndFolders; }
                set { _showBothFilesAndFolders = value; }
            }
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

 