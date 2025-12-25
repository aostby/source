using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Kolibri.net.Common.Images
{
    public static partial class Icons
    {
        #region Custom exceptions class

        public class IconNotFoundException : Exception
        {
            public IconNotFoundException(string fileName, int index)
                : base(string.Format("Icon with Id = {0} wasn't found in file {1}", index, fileName))
            {
            }
        }

        public class UnableToExtractIconsException : Exception
        {
            public UnableToExtractIconsException(string fileName, int firstIconIndex, int iconCount)
                : base(string.Format("Tryed to extract {2} icons starting from the one with id {1} from the \"{0}\" file but failed", fileName, firstIconIndex, iconCount))
            {
            }
        }

        #endregion

        #region DllImports

        /// <summary>
        /// Contains information about a file object. 
        /// </summary>
        struct SHFILEINFO
        {
            /// <summary>
            /// Handle to the icon that represents the file. You are responsible for
            /// destroying this handle with DestroyIcon when you no longer need it. 
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// Index of the icon image within the system image list.
            /// </summary>
            public IntPtr iIcon;

            /// <summary>
            /// Array of values that indicates the attributes of the file object.
            /// For information about these values, see the IShellFolder::GetAttributesOf
            /// method.
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            /// String that contains the name of the file as it appears in the Microsoft
            /// Windows Shell, or the path and file name of the file that contains the
            /// icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>
            /// String that describes the type of file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [Flags]
        enum FileInfoFlags : int
        {
            /// <summary>
            /// Retrieve the handle to the icon that represents the file and the index 
            /// of the icon within the system image list. The handle is copied to the 
            /// hIcon member of the structure specified by psfi, and the index is copied 
            /// to the iIcon member.
            /// </summary>
            SHGFI_ICON = 0x000000100,
            /// <summary>
            /// Indicates that the function should not attempt to access the file 
            /// specified by pszPath. Rather, it should act as if the file specified by 
            /// pszPath exists with the file attributes passed in dwFileAttributes.
            /// </summary>
            SHGFI_USEFILEATTRIBUTES = 0x000000010,

            SHGFI_OPENICON = 0x000000002,
            SHGFI_SMALLICON = 0x000000001,
            SHGFI_LARGEICON = 0x000000000,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
        }

        /// <summary>
        ///     Creates an array of handles to large or small icons extracted from
        ///     the specified executable file, dynamic-link library (DLL), or icon
        ///     file. 
        /// </summary>
        /// <param name="lpszFile">
        ///     Name of an executable file, DLL, or icon file from which icons will
        ///     be extracted.
        /// </param>
        /// <param name="nIconIndex">
        ///     <para>
        ///         Specifies the zero-based index of the first icon to extract. For
        ///         example, if this value is zero, the function extracts the first
        ///         icon in the specified file.
        ///     </para>
        ///     <para>
        ///         If this value is �1 and <paramref name="phiconLarge"/> and
        ///         <paramref name="phiconSmall"/> are both NULL, the function returns
        ///         the total number of icons in the specified file. If the file is an
        ///         executable file or DLL, the return value is the number of
        ///         RT_GROUP_ICON resources. If the file is an .ico file, the return
        ///         value is 1. 
        ///     </para>
        ///     <para>
        ///         Windows 95/98/Me, Windows NT 4.0 and later: If this value is a 
        ///         negative number and either <paramref name="phiconLarge"/> or 
        ///         <paramref name="phiconSmall"/> is not NULL, the function begins by
        ///         extracting the icon whose resource identifier is equal to the
        ///         absolute value of <paramref name="nIconIndex"/>. For example, use -3
        ///         to extract the icon whose resource identifier is 3. 
        ///     </para>
        /// </param>
        /// <param name="phIconLarge">
        ///     An array of icon handles that receives handles to the large icons
        ///     extracted from the file. If this parameter is NULL, no large icons
        ///     are extracted from the file.
        /// </param>
        /// <param name="phIconSmall">
        ///     An array of icon handles that receives handles to the small icons
        ///     extracted from the file. If this parameter is NULL, no small icons
        ///     are extracted from the file. 
        /// </param>
        /// <param name="nIcons">
        ///     Specifies the number of icons to extract from the file. 
        /// </param>
        /// <returns>
        ///     If the <paramref name="nIconIndex"/> parameter is -1, the
        ///     <paramref name="phIconLarge"/> parameter is NULL, and the
        ///     <paramref name="phiconSmall"/> parameter is NULL, then the return
        ///     value is the number of icons contained in the specified file.
        ///     Otherwise, the return value is the number of icons successfully
        ///     extracted from the file. 
        /// </returns>
        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static int ExtractIconEx(
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpszFile,
            int nIconIndex,
            IntPtr[] phIconLarge,
            IntPtr[] phIconSmall,
            int nIcons);

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            FileInfoFlags uFlags);

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            uint uFlags);



        #endregion

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
            // Add the '.' to the extension if needed
            if (extension[0] != '.') extension = '.' + extension;

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
    }
}