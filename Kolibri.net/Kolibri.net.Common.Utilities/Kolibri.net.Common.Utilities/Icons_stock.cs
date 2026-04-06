using System;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.CompilerServices; // [Extension()]
using System.Runtime.InteropServices;

namespace Kolibri.net.Common.Images
{
    public static partial class Icons
    {
        private const uint SHGSI_ICON = 0x000000100;
        private const uint SHGSI_LARGEICON = 0x000000000;
        private const uint SHGSI_SMALLICON = 0x000000001;
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_LARGEICON = 0x000000000;

        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
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

        public enum SHSTOCKICONID : uint
        {
            SIID_FOLDER = 3,
            SIID_FOLDEROPEN = 4,
            SIID_DRIVEFIXED = 8,
            SIID_DRIVECD = 11,
            SIID_WARNING = 78,
            SIID_ERROR = 80,
            SIID_INFO = 79,
            SIID_SHIELD = 77,
            SIID_RECYCLER = 31,
            SIID_RECYCLERFULL = 32,
            SIID_APPLICATION = 2,
            SIID_DOCNOASSOC = 0,
            SIID_DOCASSOC = 1
        }

        const Int32 MAX_PATH = 260;
        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">A handle to the icon to be destroyed. The icon must not be in use.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        /// <remarks>To get extended error information, call GetLastError.</remarks>
        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]

        private static extern bool DestroyIcon(IntPtr hIcon);
        /// <summary>
        /// Retrieves information about a stock icon.
        /// </summary>
        /// <param name="siid">One of the values from the SHSTOCKICONID enumeration that specifies which icon should be retrieved.</param>
        /// <param name="uFlags">A combination of zero or more of the following flags that specify which information is requested.</param>
        /// <param name="psii">A pointer to a SHSTOCKICONINFO structure. When this function is called, the cbSize member of this structure needs to be set to the size of the SHSTOCKICONINFO structure. When this function returns, contains a pointer to a SHSTOCKICONINFO structure that contains the requested information.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        /// <remarks>If this function returns an icon handle in the hIcon member of the SHSTOCKICONINFO structure pointed to by psii, you are responsible for freeing the icon with DestroyIcon when you no longer need it.</remarks>
        [DllImport("Shell32.dll", SetLastError = false)]
        private static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);


        #region SHGSI   SHSTOCKICONINFO
        /// <summary>
        /// Receives information used to retrieve a stock Shell icon. This structure is used in a call SHGetStockIconInfo.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHSTOCKICONINFO
        {
            /// <summary>
            /// The size of this structure, in bytes.
            /// </summary>
            public UInt32 cbSize;
            /// <summary>
            /// When SHGetStockIconInfo is called with the SHGSI_ICON flag, this member receives a handle to the icon.
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// When SHGetStockIconInfo is called with the SHGSI_SYSICONINDEX flag, this member receives the index of the image in the system icon cache.
            /// </summary>
            public Int32 iSysIconIndex;
            /// <summary>
            /// When SHGetStockIconInfo is called with the SHGSI_ICONLOCATION flag, this member receives the index of the icon in the resource whose path is received in szPath.
            /// </summary>
            public Int32 iIcon;
            /// <summary>
            /// When SHGetStockIconInfo is called with the SHGSI_ICONLOCATION flag, this member receives the path of the resource that contains the icon. The index of the icon within the resource is received in iIcon.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPath;
        }

        /// <summary>
        /// UInt Enumeration with Flags that specify which information is requested.
        /// </summary>
        [Flags()]
        public enum SHGSI : UInt32
        {

            /// <summary>
            /// The szPath and iIcon members of the SHSTOCKICONINFO structure receive the path and icon index of the requested icon, in a format suitable for passing to the ExtractIcon function. The numerical value of this flag is zero, so you always get the icon location regardless of other flags.
            /// </summary>
            SHGSI_ICONLOCATION = 0,

            /// <summary>
            /// The hIcon member of the SHSTOCKICONINFO structure receives a handle to the specified icon.
            /// </summary>
            SHGSI_ICON = 0x100,

            /// <summary>
            /// The iSysImageImage member of the SHSTOCKICONINFO structure receives the index of the specified icon in the system imagelist.
            /// </summary>
            SHGSI_SYSICONINDEX = 0x4000,

            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to add the link overlay to the file's icon.
            /// </summary>
            SHGSI_LINKOVERLAY = 0x8000,

            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to blend the icon with the system highlight color.
            /// </summary>
            SHGSI_SELECTED = 0x10000,

            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to retrieve the large version of the icon, as specified by the SM_CXICON and SM_CYICON system metrics.
            /// </summary>
            SHGSI_LARGEICON = 0x0,

            /// <summary>
            /// Modifies the SHGSI_ICON value by causing the function to retrieve the small version of the icon, as specified by the SM_CXSMICON and SM_CYSMICON system metrics.
            /// </summary>
            SHGSI_SMALLICON = 0x1,

            /// <summary>
            /// Modifies the SHGSI_LARGEICON or SHGSI_SMALLICON values by causing the function to retrieve the Shell-sized icons rather than the sizes specified by the system metrics.
            /// </summary>
            SHGSI_SHELLICONSIZE = 0x4

        }
        #endregion

        #region SHSTOCKICONID
        /// <summary>
        /// Used by SHGetStockIconInfo to identify which stock system icon to retrieve.
        /// </summary>
        /// <remarks>SIID_INVALID, with a value of -1, indicates an invalid SHSTOCKICONID value.</remarks>
       /* public enum SHSTOCKICONID : UInt32
        {
            /// <summary>
            /// Document of a type with no associated application.
            /// </summary>
            SIID_DOCNOASSOC = 0,
            /// <summary>
            /// Document of a type with an associated application.
            /// </summary>
            SIID_DOCASSOC = 1,
            /// <summary>
            /// Generic application with no custom icon.
            /// </summary>
            SIID_APPLICATION = 2,
            /// <summary>
            /// Folder (generic, unspecified state).
            /// </summary>
            SIID_FOLDER = 3,
            /// <summary>
            /// Folder (open).
            /// </summary>
            SIID_FOLDEROPEN = 4,
            /// <summary>
            /// 5.25-inch disk drive.
            /// </summary>
            SIID_DRIVE525 = 5,
            /// <summary>
            /// 3.5-inch disk drive.
            /// </summary>
            SIID_DRIVE35 = 6,
            /// <summary>
            /// Removable drive.
            /// </summary>
            SIID_DRIVEREMOVE = 7,
            /// <summary>
            /// Fixed drive (hard disk).
            /// </summary>
            SIID_DRIVEFIXED = 8,
            /// <summary>
            /// Network drive (connected).
            /// </summary>
            SIID_DRIVENET = 9,
            /// <summary>
            /// Network drive (disconnected).
            /// </summary>
            SIID_DRIVENETDISABLED = 10,
            /// <summary>
            /// CD drive.
            /// </summary>
            SIID_DRIVECD = 11,
            /// <summary>
            /// RAM disk drive.
            /// </summary>
            SIID_DRIVERAM = 12,
            /// <summary>
            /// The entire network.
            /// </summary>
            SIID_WORLD = 13,
            /// <summary>
            /// A computer on the network.
            /// </summary>
            SIID_SERVER = 15,
            /// <summary>
            /// A local printer or print destination.
            /// </summary>
            SIID_PRINTER = 16,
            /// <summary>
            /// The Network virtual folder (FOLDERID_NetworkFolder/CSIDL_NETWORK).
            /// </summary>
            SIID_MYNETWORK = 17,
            /// <summary>
            /// The Search feature.
            /// </summary>
            SIID_FIND = 22,
            /// <summary>
            /// The Help and Support feature.
            /// </summary>
            SIID_HELP = 23,

            // OVERLAYS...

            /// <summary>
            /// Overlay for a shared item.
            /// </summary>
            SIID_SHARE = 28,
            /// <summary>
            /// Overlay for a shortcut.
            /// </summary>
            SIID_LINK = 29,
            /// <summary>
            /// Overlay for items that are expected to be slow to access.
            /// </summary>
            SIID_SLOWFILE = 30,

            // MORE ICONS...

            /// <summary>
            /// The Recycle Bin (empty).
            /// </summary>
            SIID_RECYCLER = 31,
            /// <summary>
            /// The Recycle Bin (not empty).
            /// </summary>
            SIID_RECYCLERFULL = 32,
            /// <summary>
            /// Audio CD media.
            /// </summary>
            SIID_MEDIACDAUDIO = 40,
            /// <summary>
            /// Security lock.
            /// </summary>
            SIID_LOCK = 47,
            /// <summary>
            /// A virtual folder that contains the results of a search.
            /// </summary>
            SIID_AUTOLIST = 49,
            /// <summary>
            /// A network printer.
            /// </summary>
            SIID_PRINTERNET = 50,
            /// <summary>
            /// A server shared on a network.
            /// </summary>
            SIID_SERVERSHARE = 51,
            /// <summary>
            /// A local fax printer.
            /// </summary>
            SIID_PRINTERFAX = 52,
            /// <summary>
            /// A network fax printer.
            /// </summary>
            SIID_PRINTERFAXNET = 53,
            /// <summary>
            /// A file that receives the output of a Print to file operation.
            /// </summary>
            SIID_PRINTERFILE = 54,
            /// <summary>
            /// A category that results from a Stack by command to organize the contents of a folder.
            /// </summary>
            SIID_STACK = 55,
            /// <summary>
            /// Super Video CD (SVCD) media.
            /// </summary>
            SIID_MEDIASVCD = 56,
            /// <summary>
            /// A folder that contains only subfolders as child items.
            /// </summary>
            SIID_STUFFEDFOLDER = 57,
            /// <summary>
            /// Unknown drive type.
            /// </summary>
            SIID_DRIVEUNKNOWN = 58,
            /// <summary>
            /// DVD drive.
            /// </summary>
            SIID_DRIVEDVD = 59,
            /// <summary>
            /// DVD media.
            /// </summary>
            SIID_MEDIADVD = 60,
            /// <summary>
            /// DVD-RAM media.
            /// </summary>
            SIID_MEDIADVDRAM = 61,
            /// <summary>
            /// DVD-RW media.
            /// </summary>
            SIID_MEDIADVDRW = 62,
            /// <summary>
            /// DVD-R media.
            /// </summary>
            SIID_MEDIADVDR = 63,
            /// <summary>
            /// DVD-ROM media.
            /// </summary>
            SIID_MEDIADVDROM = 64,
            /// <summary>
            /// CD+ (enhanced audio CD) media.
            /// </summary>
            SIID_MEDIACDAUDIOPLUS = 65,
            /// <summary>
            /// CD-RW media.
            /// </summary>
            SIID_MEDIACDRW = 66,
            /// <summary>
            /// CD-R media.
            /// </summary>
            SIID_MEDIACDR = 67,
            /// <summary>
            /// A writeable CD in the process of being burned.
            /// </summary>
            SIID_MEDIACDBURN = 68,
            /// <summary>
            /// Blank writable CD media.
            /// </summary>
            SIID_MEDIABLANKCD = 69,
            /// <summary>
            /// CD-ROM media.
            /// </summary>
            SIID_MEDIACDROM = 70,
            /// <summary>
            /// An audio file.
            /// </summary>
            SIID_AUDIOFILES = 71,
            /// <summary>
            /// An image file.
            /// </summary>
            SIID_IMAGEFILES = 72,
            /// <summary>
            /// A video file.
            /// </summary>
            SIID_VIDEOFILES = 73,
            /// <summary>
            /// A mixed (media) file.
            /// </summary>
            SIID_MIXEDFILES = 74,


            /// <summary>
            /// Folder back. Represents the background Fold of a Folder.
            /// </summary>
            SIID_FOLDERBACK = 75,
            /// <summary>
            /// Folder front. Represents the foreground Fold of a Folder.
            /// </summary>
            SIID_FOLDERFRONT = 76,
            /// <summary>
            /// Security shield.
            /// </summary>
            /// <remarks>Use for UAC prompts only. This Icon doesn't work on all purposes.</remarks>
            SIID_SHIELD = 77,
            /// <summary>
            /// Warning (Exclamation mark).
            /// </summary>
            SIID_WARNING = 78,
            /// <summary>
            /// Informational (Info).
            /// </summary>
            SIID_INFO = 79,
            /// <summary>
            /// Error (X).
            /// </summary>
            SIID_ERROR = 80,
            /// <summary>
            /// Key.
            /// </summary>
            SIID_KEY = 81,
            /// <summary>
            /// Software.
            /// </summary>
            SIID_SOFTWARE = 82,
            /// <summary>
            /// A UI item, such as a button, that issues a rename command.
            /// </summary>
            SIID_RENAME = 83,
            /// <summary>
            /// A UI item, such as a button, that issues a delete command.
            /// </summary>
            SIID_DELETE = 84,
            /// <summary>
            /// Audio DVD media.
            /// </summary>
            SIID_MEDIAAUDIODVD = 85,
            /// <summary>
            /// Movie DVD media.
            /// </summary>
            SIID_MEDIAMOVIEDVD = 86,
            /// <summary>
            /// Enhanced CD media.
            /// </summary>
            SIID_MEDIAENHANCEDCD = 87,
            /// <summary>
            /// Enhanced DVD media.
            /// </summary>
            SIID_MEDIAENHANCEDDVD = 88,
            /// <summary>
            /// Enhanced DVD media.
            /// </summary>
            SIID_MEDIAHDDVD = 89,
            /// <summary>
            /// High definition DVD media in the Blu-ray Disc™ format.
            /// </summary>
            SIID_MEDIABLURAY = 90,
            /// <summary>
            /// Video CD (VCD) media.
            /// </summary>
            SIID_MEDIAVCD = 91,
            /// <summary>
            /// DVD+R media.
            /// </summary>
            SIID_MEDIADVDPLUSR = 92,
            /// <summary>
            /// DVD+RW media.
            /// </summary>
            SIID_MEDIADVDPLUSRW = 93,
            /// <summary>
            /// A desktop computer.
            /// </summary>
            SIID_DESKTOPPC = 94,
            /// <summary>
            /// A mobile computer (laptop).
            /// </summary>
            SIID_MOBILEPC = 95,
            /// <summary>
            /// The User Accounts Control Panel item.
            /// </summary>
            SIID_USERS = 96,
            /// <summary>
            /// Smart media.
            /// </summary>
            SIID_MEDIASMARTMEDIA = 97,
            /// <summary>
            /// CompactFlash media.
            /// </summary>
            SIID_MEDIACOMPACTFLASH = 98,
            /// <summary>
            /// A cell phone.
            /// </summary>
            SIID_DEVICECELLPHONE = 99,
            /// <summary>
            /// A digital camera.
            /// </summary>
            SIID_DEVICECAMERA = 100,
            /// <summary>
            /// A digital video camera.
            /// </summary>
            SIID_DEVICEVIDEOCAMERA = 101,
            /// <summary>
            /// An audio player.
            /// </summary>
            SIID_DEVICEAUDIOPLAYER = 102,
            /// <summary>
            /// Connect to network.
            /// </summary>
            SIID_NETWORKCONNECT = 103,
            /// <summary>
            /// The Network and Internet Control Panel item.
            /// </summary>
            SIID_INTERNET = 104,
            /// <summary>
            /// A compressed file with a .zip file name extension.
            /// </summary>
            SIID_ZIPFILE = 105,
            /// <summary>
            /// The Additional Options Control Panel item.
            /// </summary>
            SIID_SETTINGS = 106,
            /// <summary>
            /// Windows Vista with Service Pack 1 (SP1) and later. High definition DVD drive (any type - HD DVD-ROM, HD DVD-R, HD-DVD-RAM) that uses the HD DVD format.
            /// </summary>
            SIID_DRIVEHDDVD = 132,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition DVD drive (any type - BD-ROM, BD-R, BD-RE) that uses the Blu-ray Disc format.
            /// </summary>
            SIID_DRIVEBD = 133,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition DVD-ROM media in the HD DVD-ROM format.
            /// </summary>
            SIID_MEDIAHDDVDROM = 134,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition DVD-R media in the HD DVD-R format.
            /// </summary>
            SIID_MEDIAHDDVDR = 135,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition DVD-RAM media in the HD DVD-RAM format.
            /// </summary>
            SIID_MEDIAHDDVDRAM = 136,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition DVD-ROM media in the Blu-ray Disc BD-ROM format.
            /// </summary>
            SIID_MEDIABDROM = 137,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition write-once media in the Blu-ray Disc BD-R format.
            /// </summary>
            SIID_MEDIABDR = 138,
            /// <summary>
            /// Windows Vista with SP1 and later. High definition read/write media in the Blu-ray Disc BD-RE format.
            /// </summary>
            SIID_MEDIABDRE = 139,
            /// <summary>
            /// Windows Vista with SP1 and later. A cluster disk array.
            /// </summary>
            SIID_CLUSTEREDDRIVE = 140,

            /// <summary>
            /// The highest valid value in the enumeration. Values over 160 are Windows 7-only icons.
            /// </summary>
            SIID_MAX_ICONS = 175
        }
        */
        #endregion

        // copy DLL declarations above here...
        // ...

        // and copy again the XML summary here.
        public static bool DestroyIconH(IntPtr iconHandle)
        {
            return DestroyIcon(iconHandle);
        }
  

        /// <summary>
        /// Gets the Pointer to the (stock) Icon associated to the specified ID.
        /// </summary>
        /// <param name="StockIconID">Icon ID among the defined Stock ones.</param>
        /// <returns>The Pointer to the retrieved Icon. If no Icon were found, an empty Pointer is returned.</returns>
        private static IntPtr GetShellIconPointer(SHSTOCKICONID StockIconID, SHGSI IconOptions)
        {
            SHSTOCKICONINFO StkIconInfo = new SHSTOCKICONINFO();
            StkIconInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(SHSTOCKICONINFO)));

            if (SHGetStockIconInfo(StockIconID, IconOptions, ref StkIconInfo) == 0)
            {
                return StkIconInfo.hIcon;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the (stock) Icon associated to the specified ID.
        /// </summary>
        /// <param name="StockIconID">Icon ID among the defined Stock ones.</param>
        /// <returns>The (stock) Icon. If no Icon were found, Null is returned.</returns>
        /// <remarks>WARNING ! Caller is responsible of calling Dispose() on the returned Icon.</remarks>
        public static Icon GetSystemIcon(SHSTOCKICONID stockIconID, SHGSI iconOptions)
        {
            IntPtr iconPointer = GetShellIconPointer(stockIconID, iconOptions);

            if (iconPointer != IntPtr.Zero)
            {
                Icon actualIcon = Icon.FromHandle(iconPointer);
                Icon iconCopy = (System.Drawing.Icon)actualIcon.Clone();

                actualIcon.Dispose();
                DestroyIcon(iconPointer);
                /*
                    Honestly, I'm unsure of what I'm doing here :-(
                    If I get rid of either actualIcon or iconCopy, 
                    and don't make a copy of it, I get a 0x0 Icon.
                    If I don't call DestroyIcon(h), I get a memory leak.
                    I highly doubt a memory leak won't occur even with the trick above,
                    but heh! >:-D honestly I don't care since 
                    everything related to Icon retrieval in Windows 
                    appears to me a very bad design pattern in the first place.
                */

                return iconCopy;
            }
            else
            {
                return null;
            }
        }

        ///// <summary>
        ///// Creates a Thumbnail of the Bitmap.
        ///// </summary>
        ///// <param name="sourceBitmap">Source Bitmap.</param>
        ///// <param name="width">Width for the Tumbnail.</param>
        ///// <param name="height">Height for the Tumbnail.</param>
        ///// <returns>Returns the created Thumbnail.</returns>
        //[Extension()]
        //public Bitmap CreateThumbnail(this Bitmap sourceBitmap, Int32 width, Int32 height)
        //{
        //    if (sourceBitmap == null)
        //    {
        //        return null; // Eat Exception !
        //    }
        //    else
        //    {
        //        if ((width > 0) && (height > 0))
        //        {
        //            Rectangle layoutRectangle = new Rectangle(0, 0, width, height);
        //            Bitmap thumbnailBitmap = new Bitmap(width, height, sourceBitmap.PixelFormat);

        //            using (Graphics bmpGrphics = thumbnailBitmap.CreateGraphics())
        //            {
        //                bmpGrphics.DrawImage(sourceBitmap, layoutRectangle);
        //            }

        //            return thumbnailBitmap;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

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


    }
}