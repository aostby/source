using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
    using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml.Linq;
using static Kolibri.net.Common.Utilities.FolderUtilities.PInvoke;
namespace Kolibri.net.Common.Images
{
    public static partial class Icons
    {


        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHGetStockIconInfo(
            SHSTOCKICONID siid,
            uint uFlags,
            ref SHSTOCKICONINFO psii);



        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint ExtractIconEx(
        string szFileName,
        int nIconIndex,
        IntPtr[] phiconLarge,
        IntPtr[] phiconSmall,
        uint nIcons);




        static Dictionary<(string file, int index), string> KnownIcons = new Dictionary<(string file, int index), string>
        {

            {("imageres.dll",0) , "Explorer" },
{("imageres.dll",1) , "Explorer Default Document" },
{("imageres.dll",10) , "Drive Network Drive disconnected" },
{("imageres.dll",107) , "os drive folder icon" },
{("imageres.dll",11) , "Drive CD-ROM Drive" },
{("imageres.dll",12) , "Drive RAM Drive" },
{("imageres.dll",13) , "Global Entire network" },
{("imageres.dll",15) , "Explorer Networked Computer" },
{("imageres.dll",16) , "Explorer Printer(s)" },
{("imageres.dll",17) , "Desktop Network Neighborhood" },
{("imageres.dll",179) , "compressed file/folder overlay icon" },
{("imageres.dll",18) , "Explorer Workgroup" },
{("imageres.dll",19) , "Startmenu Programs" },
{("imageres.dll",2) , "Explorer Default Application" },
{("imageres.dll",20) , "Startmenu Recent documents" },
{("imageres.dll",21) , "Startmenu Settings" },
{("imageres.dll",22) , "Startmenu Find" },
{("imageres.dll",23) , "Startmenu Help" },
{("imageres.dll",24) , "Startmenu Run" },
{("imageres.dll",25) , "Startmenu Suspend" },
{("imageres.dll",26) , "Startmenu Docking" },
{("imageres.dll",27) , "Startmenu Shutdown" },
{("imageres.dll",28) , "Overlay Sharing" },
{("imageres.dll",29) , "Overlay Shortcut" },
{("imageres.dll",3) , "Folder" },
{("imageres.dll",31) , "Desktop Recycle bin empty" },
{("imageres.dll",32) , "Desktop Recycle bin full" },
{("imageres.dll",33) , "Explorer Dial-up Networking" },
{("imageres.dll",34) , "Explorer Desktop" },
{("imageres.dll",35) , "Startmenu Settings/Control Panel" },
{("imageres.dll",36) , "Startmenu Programs/Program folder" },
{("imageres.dll",37) , "Startmenu Settings/Printers" },
{("imageres.dll",39) , "Startmenu Settings/Taskbar" },
{("imageres.dll",4) ,"Open Folder" },
{("imageres.dll",40) , "Explorer Audio CD" },
{("imageres.dll",42) , "Explorer Saved search (.fnd)" },
{("imageres.dll",43) , "Explorer und Startmenu Favorites" },
{("imageres.dll",44) , "Startmenu Log Off" },
{("imageres.dll",5) , "Drive 5.25 inch floppy" },
{("imageres.dll",51   ) , "network folder icon" },
{("imageres.dll",6) , "Drive 3.5 inch floppy" },
{("imageres.dll",7) , "Drive Removable Drive" },
{("imageres.dll",77) , "UAC (administrator) overlay icon" },
{("imageres.dll",8) , "Drive Hard Drive" },
{("imageres.dll",9) , "Drive Network Drive" },
{("shell32.dll", 167) , "Warning" },
{("shell32.dll", 168) , "Error"}


        };
        public static void ApplyNames(List<SystemIconInfo> icons, bool largeIcons)
        {
            var stockMap = GetStockIconNames(largeIcons);

            foreach (var icon in icons)
            {
                // 1. Try known dictionary
                var key = (Path.GetFileName(icon.SourceFile).ToLower(), icon.Index);
                if (KnownIcons.TryGetValue(key, out var friendly))
                {
                    icon.FriendlyName = friendly;
                    continue;
                }

                // 2. Try stock icon match (by hash)
                string hash = GetIconHash(icon.Icon);

                if (stockMap.TryGetValue(hash, out var stockName))
                {
                    icon.StockName = stockName;
                }
            }
        }

        public static Dictionary<string, string> GetStockIconNames(bool large)
        {
            var dict = new Dictionary<string, string>();

            foreach (SHSTOCKICONID id in Enum.GetValues(typeof(SHSTOCKICONID)))
            {
                var info = new SHSTOCKICONINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);

                uint flags = SHGSI_ICON | (large ? SHGSI_LARGEICON : SHGSI_SMALLICON);

                SHGetStockIconInfo(id, flags, ref info);

                if (info.hIcon != IntPtr.Zero)
                {
                    using var icon = (Icon)Icon.FromHandle(info.hIcon).Clone();

                    string hash = GetIconHash(icon);
                    dict[hash] = id.ToString().Replace("SIID_", "");
                }
            }

            return dict;
        }


        /// <summary>
        /// Metode som legger ett icon oppå ett annet
        /// https://stackoverflow.com/questions/2599778/how-to-merge-two-icons-together-overlay-one-icon-on-top-of-another/2609475
        /// </summary>
        /// <param name="originalIcon"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        public static Icon AddIconOverlay(Icon originalIcon, Icon overlay)
        {
            Image a = originalIcon.ToBitmap();
            Image b = overlay.ToBitmap();
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics canvas = Graphics.FromImage(bitmap);

            canvas.DrawImage(a, new System.Drawing.Rectangle(0, 0, 16, 16));
            canvas.DrawImage(b, new System.Drawing.Rectangle(0, 0, 16, 16));
            //canvas.DrawImage(a, new Point(0, 0));
            //canvas.DrawImage(b, new Point(0, 0));
            canvas.Save();
            return Icon.FromHandle(bitmap.GetHicon());
        }


        /// <summary>
        /// Two constants extracted from the FileInfoFlags, the only that are
        /// meaningfull for the user of this class.
        /// </summary>
        public enum SystemIconSize : int
        {
            Large = (int)FileInfoFlags.SHGFI_LARGEICON, //0x000000000,
            Small = (int)FileInfoFlags.SHGFI_SMALLICON // 0x000000001
        }
        public enum FolderType
        {
            Closed,
            Open
        }

        /// <summary>
        /// Get the number of icons in the specified file.
        /// </summary>
        /// <param name="fileName">Full path of the file to look for.</param>
        /// <returns></returns>
        static int GetIconsCountInFile(string fileName)
        {
            return ExtractIconEx(fileName, -1, null, null, 0);
        }

        #region ExtractIcon-like functions

        public static void ExtractEx(string fileName, List<Icon> largeIcons,
            List<Icon> smallIcons, int firstIconIndex, int iconCount)
        {
            /*
             * Memory allocations
             */

            IntPtr[] smallIconsPtrs = null;
            IntPtr[] largeIconsPtrs = null;

            if (smallIcons != null)
            {
                smallIconsPtrs = new IntPtr[iconCount];
            }
            if (largeIcons != null)
            {
                largeIconsPtrs = new IntPtr[iconCount];
            }

            /*
             * Call to native Win32 API
             */

            int apiResult = ExtractIconEx(fileName, firstIconIndex, largeIconsPtrs, smallIconsPtrs, iconCount);
            if (apiResult != iconCount)
            {
                throw new UnableToExtractIconsException(fileName, firstIconIndex, iconCount);
            }

            /*
             * Fill lists
             */

            if (smallIcons != null)
            {
                smallIcons.Clear();
                foreach (IntPtr actualIconPtr in smallIconsPtrs)
                {
                    smallIcons.Add(Icon.FromHandle(actualIconPtr));
                }
            }
            if (largeIcons != null)
            {
                largeIcons.Clear();
                foreach (IntPtr actualIconPtr in largeIconsPtrs)
                {
                    largeIcons.Add(Icon.FromHandle(actualIconPtr));
                }
            }
        }

        public static List<Icon> ExtractEx(string fileName, SystemIconSize size, int firstIconIndex, int iconCount)
        {
            List<Icon> iconList = new List<Icon>();

            switch (size)
            {
                case SystemIconSize.Large:
                    ExtractEx(fileName, iconList, null, firstIconIndex, iconCount);
                    break;

                case SystemIconSize.Small:
                    ExtractEx(fileName, null, iconList, firstIconIndex, iconCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("size");
            }

            return iconList;
        }

        public static void Extract(string fileName, List<Icon> largeIcons, List<Icon> smallIcons)
        {
            int iconCount = GetIconsCountInFile(fileName);
            ExtractEx(fileName, largeIcons, smallIcons, 0, iconCount);
        }

        public static List<Icon> Extract(string fileName, SystemIconSize size)
        {
            int iconCount = GetIconsCountInFile(fileName);
            return ExtractEx(fileName, size, 0, iconCount);
        }

        public static Icon ExtractOne(string fileName, int index, SystemIconSize size)
        {
            try
            {
                List<Icon> iconList = ExtractEx(fileName, size, index, 1);
                return iconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(fileName, index);
            }
        }

        public static void ExtractOne(string fileName, int index,
            out Icon largeIcon, out Icon smallIcon)
        {
            List<Icon> smallIconList = new List<Icon>();
            List<Icon> largeIconList = new List<Icon>();
            try
            {
                ExtractEx(fileName, largeIconList, smallIconList, index, 1);
                largeIcon = largeIconList[0];
                smallIcon = smallIconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(fileName, index);
            }
        }

        #endregion

        internal static Icon GetFolderIcon(DirectoryInfo di, SystemIconSize size, FolderType folderType)
        {
            // Need to add size check, although errors generated at present!    
            uint flags = (int)FileInfoFlags.SHGFI_ICON | (int)FileInfoFlags.SHGFI_USEFILEATTRIBUTES;

            if (FolderType.Open == folderType)
            {
                flags += (int)FileInfoFlags.SHGFI_OPENICON;
            }
            if (SystemIconSize.Small == size)
            {
                flags += (int)FileInfoFlags.SHGFI_SMALLICON;
            }
            else
            {
                flags += (int)FileInfoFlags.SHGFI_LARGEICON;
            }
            // Get the folder icon    
            var shfi = new SHFILEINFO();

            var res = SHGetFileInfo(
                 di.FullName,
                 0,
                 out shfi,
                 Marshal.SizeOf(shfi),
               //  ((folderType == FolderType.Closed) ? FileInfoFlags.SHGFI_ICON : FileInfoFlags.SHGFI_OPENICON) | (FileInfoFlags)size
               flags
                 );

            //( FileInfoFlags.SHGFI_ICON )); // FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size);
            /*SHGetFileInfo(@"C:\Windows", (int)FileInfoFlags.FILE_ATTRIBUTE_DIRECTORY, out shfi (uint)Marshal.SizeOf(shfi), flags); */
            if (res == IntPtr.Zero)
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());

            // Load the icon from an HICON handle  
            Icon.FromHandle(shfi.hIcon);

            // Now clone the icon, so that it can be successfully stored in an ImageList
            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();

            DestroyIcon(shfi.hIcon);        // Cleanup    

            return icon;
        }

        private static Icon GetFolderIcon(SystemIconSize size, FolderType folderType)
        {

            // Need to add size check, although errors generated at present!    
            uint flags = (int)FileInfoFlags.SHGFI_ICON | (int)FileInfoFlags.SHGFI_USEFILEATTRIBUTES;

            if (FolderType.Open == folderType)
            {
                flags += (int)FileInfoFlags.SHGFI_OPENICON;
            }
            if (SystemIconSize.Small == size)
            {
                flags += (int)FileInfoFlags.SHGFI_SMALLICON;
            }
            else
            {
                flags += (int)FileInfoFlags.SHGFI_LARGEICON;
            }
            // Get the folder icon    
            var shfi = new SHFILEINFO();

            var res = SHGetFileInfo(
                 @"C:\Windows",
                 0,
                 out shfi,
                 Marshal.SizeOf(shfi),
                 ((folderType == FolderType.Closed) ? FileInfoFlags.SHGFI_ICON : FileInfoFlags.SHGFI_OPENICON) | (FileInfoFlags)size
                 );

            //( FileInfoFlags.SHGFI_ICON )); // FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size);
            /*SHGetFileInfo(@"C:\Windows", (int)FileInfoFlags.FILE_ATTRIBUTE_DIRECTORY, out shfi (uint)Marshal.SizeOf(shfi), flags); */
            if (res == IntPtr.Zero)
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());

            // Load the icon from an HICON handle  
            Icon.FromHandle(shfi.hIcon);

            // Now clone the icon, so that it can be successfully stored in an ImageList
            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();

            DestroyIcon(shfi.hIcon);        // Cleanup    

            return icon;
        }
        public static Icon GetFolderIcon() { return Icons.MakeTransperent(GetFolderIcon(SystemIconSize.Small, FolderType.Closed)); }
        public static Icon MakeTransperent(Icon ico)
        {
            return MakeTransperent(ico, System.Drawing.Color.Transparent);
        }
        public static Icon MakeTransperent(Icon ico, System.Drawing.Color color)
        {
            using (Bitmap bitmap = ico.ToBitmap())
            {
                bitmap.MakeTransparent(color);
                System.IntPtr icH = bitmap.GetHicon();
                return Icon.FromHandle(icH);
            }
        }

        public static Icon IconFromImage(Image bitmap)
        {
            Icon ret = null;
            using (Bitmap Cbitmap = new Bitmap(bitmap))
            {
                IntPtr icH = Cbitmap.GetHicon();
                Icon ico = Icon.FromHandle(icH);
                ret = ico;
            }
            return ret;
        }

        /// <summary>
        /// this will look throgh the registry 
        /// to find if the Extension have an icon.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Icon IconFromExtension(string extension, SystemIconSize size)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException(nameof(extension));

            // Ensure proper format
            if (!extension.StartsWith("."))
                extension = "." + extension;

            // Fake filename (this is the trick)
            string fakeFileName = "file" + extension;

            SHFILEINFO shinfo;

            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES |
                         (size == SystemIconSize.Small ? SHGFI_SMALLICON : SHGFI_LARGEICON);

            IntPtr result = SHGetFileInfo(
                fakeFileName,
                FILE_ATTRIBUTE_NORMAL,
                out shinfo,
                (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                flags);

            if (result == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
                return null;

            // Clone the icon to avoid handle issues
            Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();

            // Clean up native handle
            DestroyIcon(shinfo.hIcon);

            return icon;
        }

        public static Icon IconFromExtension_old(string extension, SystemIconSize size)
        {
            // Add the '.' to the extension if needed
            if (extension[0] != '.') extension = '.' + extension;
            extension = extension.Replace("..", ".");

            //opens the registry for the wanted key.
            RegistryKey Root = Registry.ClassesRoot;
            RegistryKey ExtensionKey = Root.OpenSubKey(extension);
            ExtensionKey.GetValueNames();
            RegistryKey ApplicationKey =
                Root.OpenSubKey(ExtensionKey.GetValue("").ToString());

            //gets the name of the file that have the icon.
            string IconLocation =
                ApplicationKey.OpenSubKey("DefaultIcon").GetValue("").ToString();
            string[] IconPath = IconLocation.Split(',');

            if (IconPath[1] == null) IconPath[1] = "0";
            IntPtr[] Large = new IntPtr[1], Small = new IntPtr[1];

            //extracts the icon from the file.
            ExtractIconEx(IconPath[0],
                Convert.ToInt16(IconPath[1]), Large, Small, 1);
            return size == SystemIconSize.Large ?
                Icon.FromHandle(Large[0]) : Icon.FromHandle(Small[0]);
        }

        public static Icon IconFromExtensionShell(string extension, SystemIconSize size)
        {
            //add '.' if nessesry
            if (extension[0] != '.') extension = '.' + extension;

            //temp struct for getting file shell info
            SHFILEINFO fileInfo = new SHFILEINFO();

            SHGetFileInfo(
                extension,
                0,
                out fileInfo,
                Marshal.SizeOf(fileInfo),
                FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size);

            return Icon.FromHandle(fileInfo.hIcon);
        }

        public static Icon IconFromResource(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            return new Icon(assembly.GetManifestResourceStream(resourceName));
        }

        /// <summary>
        /// Parse strings in registry who contains the name of the icon and
        /// the index of the icon an return both parts.
        /// </summary>
        /// <param name="regString">The full string in the form "path,index" as found in registry.</param>
        /// <param name="fileName">The "path" part of the string.</param>
        /// <param name="index">The "index" part of the string.</param>
        public static void ExtractInformationsFromRegistryString(string regString, out string fileName, out int index)
        {
            if (regString == null)
            {
                throw new ArgumentNullException("regString");
            }
            if (regString.Length == 0)
            {
                throw new ArgumentException("The string should not be empty.", "regString");
            }

            index = 0;
            string[] strArr = regString.Replace("\"", "").Split(',');
            fileName = strArr[0].Trim();
            if (strArr.Length > 1)
            {
                int.TryParse(strArr[1].Trim(), out index);
            }
        }

        public static Icon ExtractFromRegistryString(string regString, SystemIconSize size)
        {
            string fileName;
            int index;
            ExtractInformationsFromRegistryString(regString, out fileName, out index);
            return ExtractOne(fileName, index, size);
        }

        /// <summary>
        /// Returns an icon representation of an image contained in the specified file.
        /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, xcept this version works.
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        /// <exception cref="System.ArgumentException">filePath does not indicate a valid file.</exception>
        public static Icon ExtractAssociatedIcon(String filePath)
        {
            int index = 0;

            Uri uri;
            if (filePath == null)
            {
                throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
            }
            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }
            //if (uri.IsUnc)
            //{
            //  throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", filePath, "filePath"), "filePath");
            //}
            if (uri.IsFile)
            {
                if (!File.Exists(filePath))
                {
                    //IntSecurity.DemandReadFileIO(filePath);
                    throw new FileNotFoundException(filePath);
                }

                StringBuilder iconPath = new StringBuilder(260);
                iconPath.Append(filePath);

                IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                if (handle != IntPtr.Zero)
                {
                    //IntSecurity.ObjectFromWin32Handle.Demand();
                    return Icon.FromHandle(handle);
                }
            }
            return null;
        }
        public static Icon ImageToIcon(Bitmap image)
        {

            return ImageToIcon(image as Image);
        }

        public static Icon ImageToIcon(Image image)
        {
            var Hicon = (image as Bitmap).GetHicon();
            Icon newIcon = Icon.FromHandle(Hicon);
            return newIcon;
        }

        public static List<SystemIconInfo> GetAllSystemIcons(bool largeIcons = true)
        {
            var results = new List<SystemIconInfo>();

            string systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            string[] sources =
            {
            Path.Combine(systemDir, "shell32.dll"),
            Path.Combine(systemDir, "imageres.dll"),
            Path.Combine(systemDir, "moricons.dll"),
Path.Combine(systemDir, "wmploc.DLL"),
Path.Combine(systemDir, "netshell.dll"),
Path.Combine(systemDir, "mmcndmgr.dll"),
Path.Combine(systemDir, "ieframe.dll "),
Path.Combine(systemDir, "compstui.dll"),
Path.Combine(systemDir, "DDORes.dll"),
Path.Combine(systemDir, "pnidui.dll"),



        };

            foreach (var file in sources)
            {
                if (!File.Exists(file))
                    continue;

                uint count = (uint)ExtractIconEx(file, -1, null, null, 0);

                for (int i = 0; i < count; i++)
                {
                    IntPtr[] large = new IntPtr[1];
                    IntPtr[] small = new IntPtr[1];

                    uint extracted = (uint)ExtractIconEx(file, i, large, small, 1);

                    IntPtr handle = largeIcons ? large[0] : small[0];

                    if (extracted == 0 || handle == IntPtr.Zero)
                        continue;

                    try
                    {
                        Icon icon = (Icon)Icon.FromHandle(handle).Clone();

                        results.Add(new SystemIconInfo
                        {
                            SourceFile = file,
                            Index = i,
                            Icon = icon,
                            
                        });
                    }
                    finally
                    {
                        DestroyIcon(handle);
                    }
                }
            }
            ApplyNames(results, largeIcons);

            return results
                    .GroupBy(o => GetIconHash(o.Icon))
                    .Select(g => g.First())
                    .ToList();
        }
        public static string GetIconHash(Icon icon)
        {if (icon == null)                return null;          
            using var bmp = icon.ToBitmap();
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return Convert.ToBase64String(ms.ToArray());
        }


        /// <summary>
        /// This class suppresses stack walks for unmanaged code permission. 
        /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
        /// This class is for methods that are safe for anyone to call. 
        /// Callers of these methods are not required to perform a full security review to make sure that the 
        /// usage is secure because the methods are harmless for any caller.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
            internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
        }
        public class SystemIconInfo
        {
            public string SourceFile { get; set; }
            public int Index { get; set; }
            public Icon Icon { get; set; }

            public string FriendlyName { get; set; } // from dictionary
            public string StockName { get; set; }    // from SHGetStockIconInfo

            public string Name =>
                FriendlyName 
                ?? StockName
                ?? $"{Path.GetFileName(SourceFile)} #{Index}";

            public string Id => $"{Path.GetFileName(SourceFile)}:{Index}";
        }
    }
}