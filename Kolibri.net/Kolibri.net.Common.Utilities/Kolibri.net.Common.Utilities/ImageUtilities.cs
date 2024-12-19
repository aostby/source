using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Drawing.Drawing2D;


namespace Kolibri.net.Common.Utilities
{
    public class ImageUtilities
    {
        public static Bitmap Crop(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path"> Path to which the image would be saved. </param> 
        /// <param name="quality"> An integer from 0 to 100, with 100 being the highest quality. </param> 
        public static void ReduceJpegSize(string path, Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        public static Image ImageZoom(Image img, Size size)
        {
            Bitmap bm = new Bitmap(img);

            bm = new Bitmap(img, new Size(Convert.ToInt32(img.Width * size.Width), Convert.ToInt32(img.Height * size.Height)));
            Graphics grap = Graphics.FromImage(bm);
            //grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
            return bm;
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }


      

    
      
        public static Size AspectSize(Image imageResource)
        {
            Size ret = new Size(500, 150);

            if ((imageResource.Width > ret.Width) || (imageResource.Height > ret.Height))
            {
                double num = Convert.ToDouble(imageResource.Height) / Convert.ToDouble(ret.Height);
                double num2 = Convert.ToDouble(imageResource.Width) / Convert.ToDouble(ret.Width);
                double num3 = Math.Max(num, num2);
                ret.Width = Convert.ToInt32((double)(Convert.ToDouble(imageResource.Width) / num3));
                ret.Height = Convert.ToInt32((double)(Convert.ToDouble(imageResource.Height) / num3));
            }
            else
                ret = imageResource.Size;
            return ret;
        }
        /// <summary>
        /// Gets an online image as a stream via an HttpRequest
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }


        /// <summary>
        /// The fileread causes memoryerror. use this method to avoid that 
        /// Also you can open it in read mode, (if you want to use it in two place same time)
        /// </summary>
        /// <param name="previewFile"></param>
        /// <returns></returns>
        public static Image OpenImage(string previewFile)
        {
            FileStream fs = new FileStream(previewFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Image.FromStream(fs);
        }

 

        public static string MakeImageSrcData(FileInfo file)
        {

            FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            byte[] filebytes = new byte[fs.Length];
            fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));

            return Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
        }

        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        public static DateTime DateTaken(string path)
        {
            DateTime ret = new DateTime();
            try
            {
                Regex r = new Regex(":");

                //retrieves the datetime WITHOUT loading the whole image

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    try
                    {
                        System.Drawing.Imaging.PropertyItem propertyItem = myImage.PropertyItems.FirstOrDefault(i => i.Id == 36867);
                        if (propertyItem != null)
                        {

                            PropertyTagId bar = (PropertyTagId)0x9003;
                            PropertyItem propItem = myImage.GetPropertyItem(36867);
                            string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                            /*
                             PropertyTagType foo = (PropertyTagType)propertyItem.Id;
                             PropertyTagId bar = (PropertyTagId)propertyItem.Type;
                             PropertyTagId too = (PropertyTagId)36867;
                             */
                            return DateTime.Parse(dateTaken);
                        }
                        else if (myImage.PropertyItems.FirstOrDefault(i => i.Id == 0x132) != null)
                        {
                            // Get the Date Created property 
                            //System.Drawing.Imaging.PropertyItem propertyItem = image.GetPropertyItem( 0x132 );
                            propertyItem = myImage.PropertyItems.FirstOrDefault(i => i.Id == 0x132);
                            if (propertyItem != null)
                            {
                                // Extract the property value as a String. 
                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                string text = encoding.GetString(propertyItem.Value, 0, propertyItem.Len - 1);

                                // Parse the date and time. 
                                System.Globalization.CultureInfo provider = CultureInfo.InvariantCulture;
                                DateTime dateCreated = DateTime.ParseExact(text, "yyyy:MM:d H:m:s", provider);
                                return dateCreated;
                            }
                        }
                        else
                        {
                            //DateTime fileCreatedDate = File.GetCreationTime(path);
                            DateTime LastWriteTime = File.GetLastWriteTime(path);
                            return LastWriteTime;
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return ret;
        }

        public static byte[] BitmapToBytes(Bitmap bitmap)
        {


            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

        }


            //byte[] bytes = null;
            //using (var ms = new MemoryStream())
            //{
            //    Bitmap bmp =  bitmap.Clone() as Bitmap;//Bitmap(bitmap);
            //    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    bytes = ms.ToArray();
            //}
            //return bytes;


            //Image img = new Bitmap(bitmap, 300, 400);
            //    byte[] arr;
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        //Here, you should change the ImageFormat depending on the format of your actually loaded file
            //        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //        arr = ms.ToArray();
            //    }
            //    using (MemoryStream ms = new MemoryStream(arr, 0, arr.Length))
            //    {
            //        ms.Position = 0;
            //        Image resim;
            //        resim = Image.FromStream(ms, true);
            //    }
            //    return arr;
            //}
        }


    ///<summary>
    /// Specifies the data type of the values stored in the value data member of that same PropertyItem object.
    ///</summary>
    public enum PropertyTagType : short
    {
        ///<summary>Specifies that the format is 4 bits per pixel, indexed.</summary>
        PixelFormat4bppIndexed = 0,
        ///<summary>Specifies that the value data member is an array of bytes.</summary>
        Byte = 1,
        ///<summary>Specifies that the value data member is a null-terminated ASCII string. If you set the type data member of a PropertyItem object to PropertyTagTypeASCII, you should set the length data member to the length of the string including the NULL terminator. For example, the string HELLO would have a length of 6.</summary>
        ASCII = 2,
        ///<summary>Specifies that the value data member is an array of unsigned short (16-bit) integers.</summary>
        Int16 = 3,
        ///<summary>Specifies that the value data member is an array of unsigned long (32-bit) integers.</summary>
        Int32 = 4,
        ///<summary>Specifies that the value data member is an array of pairs of unsigned long integers. Each pair represents a fraction; the first integer is the numerator and the second integer is the denominator.</summary>
        Rational = 5,
        ///<summary>Specifies that the value data member is an array of bytes that can hold values of any data type.</summary>
        Undefined = 7,
        ///<summary>Specifies that the value data member is an array of signed long (32-bit) integers.</summary>
        SLONG = 9,
        ///<summary>Specifies that the value data member is an array of pairs of signed long integers. Each pair represents a fraction; the first integer is the numerator and the second integer is the denominator.</summary>
        SRational = 10
    }

    ///<summary>
    /// The following Enumeration gives list (and descriptions) of the property items supported in EXIF format.
    ///</summary>	
    public enum PropertyTagId : int
    {
        ///<summary>Null-terminated character string that specifies the name of the person who created the image.</summary>
        Artist = 0x013B,
        ///<summary>Number of bits per color component. See also SamplesPerPixel.</summary>
        BitsPerSample = 0x0102,
        ///<summary></summary>
        CellHeight = 0x0109,
        ///<summary></summary>
        CellWidth = 0x0108,
        ///<summary></summary>
        ChrominanceTable = 0x5091,
        ///<summary></summary>
        ColorMap = 0x0140,
        ///<summary></summary>
        ColorTransferFunction = 0x501A,
        ///<summary></summary>
        Compression = 0x0103,
        ///<summary></summary>
        Copyright = 0x8298,
        ///<summary></summary>
        DateTime = 0x0132,
        ///<summary></summary>
        DocumentName = 0x010D,
        ///<summary></summary>
        DotRange = 0x0150,
        ///<summary></summary>
        Camera_Make = 0x010F,
        ///<summary></summary>
        Camera_Model = 0x0110,
        ///<summary></summary>
        ExifAperture = 0x9202,
        ///<summary></summary>
        ExifBrightness = 0x9203,
        ///<summary></summary>
        ExifCfaPattern = 0xA302,
        ///<summary></summary>
        ExifColorSpace = 0xA001,
        ///<summary></summary>
        ExifCompBPP = 0x9102,
        ///<summary></summary>
        ExifCompConfig = 0x9101,
        ///<summary></summary>
        ExifDTDigitized = 0x9004,
        ///<summary></summary>
        ExifDTDigSS = 0x9292,
        ///<summary></summary>
        ExifDTOrig = 0x9003,
        ///<summary></summary>
        ExifDTOrigSS = 0x9291,
        ///<summary></summary>
        ExifDTSubsec = 0x9290,
        ///<summary></summary>
        ExifExposureBias = 0x9204,
        ///<summary></summary>
        ExifExposureIndex = 0xA215,
        ///<summary></summary>
        ExifExposureProg = 0x8822,
        ///<summary></summary>
        ExifExposureTime = 0x829A,
        ///<summary></summary>
        ExifFileSource = 0xA300,
        ///<summary></summary>
        ExifFlash = 0x9209,
        ///<summary></summary>
        ExifFlashEnergy = 0xA20B,
        ///<summary></summary>
        ExifFNumber = 0x829D,
        ///<summary></summary>
        ExifFocalLength = 0x920A,
        ///<summary></summary>
        ExifFocalResUnit = 0xA210,
        ///<summary></summary>
        ExifFocalXRes = 0xA20E,
        ///<summary></summary>
        ExifFocalYRes = 0xA20F,
        ///<summary></summary>
        ExifFPXVer = 0xA000,
        ///<summary></summary>
        ExifIFD = 0x8769,
        ///<summary></summary>
        ExifInterop = 0xA005,
        ///<summary></summary>
        ExifISOSpeed = 0x8827,
        ///<summary></summary>
        ExifLightSource = 0x9208,
        ///<summary></summary>
        ExifMakerNote = 0x927C,
        ///<summary></summary>
        ExifMaxAperture = 0x9205,
        ///<summary></summary>
        ExifMeteringMode = 0x9207,
        ///<summary></summary>
        ExifOECF = 0x8828,
        ///<summary></summary>
        ExifPixXDim = 0xA002,
        ///<summary></summary>
        ExifPixYDim = 0xA003,
        ///<summary></summary>
        ExifRelatedWav = 0xA004,
        ///<summary></summary>
        ExifSceneType = 0xA301,
        ///<summary></summary>
        ExifSensingMethod = 0xA217,
        ///<summary></summary>
        ExifShutterSpeed = 0x9201,
        ///<summary></summary>
        ExifSpatialFR = 0xA20C,
        ///<summary></summary>
        ExifSpectralSense = 0x8824,
        ///<summary></summary>
        ExifSubjectDist = 0x9206,
        ///<summary></summary>
        ExifSubjectLoc = 0xA214,
        ///<summary></summary>
        ExifUserComment = 0x9286,
        ///<summary></summary>
        ExifVer = 0x9000,
        ///<summary></summary>
        ExtraSamples = 0x0152,
        ///<summary></summary>
        FillOrder = 0x010A,
        ///<summary></summary>
        FrameDelay = 0x5100,
        ///<summary></summary>
        FreeByteCounts = 0x0121,
        ///<summary></summary>
        FreeOffset = 0x0120,
        ///<summary></summary>
        Gamma = 0x0301,
        ///<summary></summary>
        GlobalPalette = 0x5102,
        ///<summary></summary>
        GpsAltitude = 0x0006,
        ///<summary></summary>
        GpsAltitudeRef = 0x0005,
        ///<summary></summary>
        GpsDestBear = 0x0018,
        ///<summary></summary>
        GpsDestBearRef = 0x0017,
        ///<summary></summary>
        GpsDestDist = 0x001A,
        ///<summary></summary>
        GpsDestDistRef = 0x0019,
        ///<summary></summary>
        GpsDestLat = 0x0014,
        ///<summary></summary>
        GpsDestLatRef = 0x0013,
        ///<summary></summary>
        GpsDestLong = 0x0016,
        ///<summary></summary>
        GpsDestLongRef = 0x0015,
        ///<summary></summary>
        GpsGpsDop = 0x000B,
        ///<summary></summary>
        GpsGpsMeasureMode = 0x000A,
        ///<summary></summary>
        GpsGpsSatellites = 0x0008,
        ///<summary></summary>
        GpsGpsStatus = 0x0009,
        ///<summary></summary>
        GpsGpsTime = 0x0007,
        ///<summary></summary>
        GpsIFD = 0x8825,
        ///<summary></summary>
        GpsImgDir = 0x0011,
        ///<summary></summary>
        GpsImgDirRef = 0x0010,
        ///<summary></summary>
        GpsLatitude = 0x0002,
        ///<summary></summary>
        GpsLatitudeRef = 0x0001,
        ///<summary></summary>
        GpsLongitude = 0x0004,
        ///<summary></summary>
        GpsLongitudeRef = 0x0003,
        ///<summary></summary>
        GpsMapDatum = 0x0012,
        ///<summary></summary>
        GpsSpeed = 0x000D,
        ///<summary></summary>
        GpsSpeedRef = 0x000C,
        ///<summary></summary>
        GpsTrack = 0x000F,
        ///<summary></summary>
        GpsTrackRef = 0x000E,
        ///<summary></summary>
        GpsVer = 0x0000,
        ///<summary></summary>
        GrayResponseCurve = 0x0123,
        ///<summary></summary>
        GrayResponseUnit = 0x0122,
        ///<summary></summary>
        GridSize = 0x5011,
        ///<summary></summary>
        HalftoneDegree = 0x500C,
        ///<summary></summary>
        HalftoneHints = 0x0141,
        ///<summary></summary>
        HalftoneLPI = 0x500A,
        ///<summary></summary>
        HalftoneLPIUnit = 0x500B,
        ///<summary></summary>
        HalftoneMisc = 0x500E,
        ///<summary></summary>
        HalftoneScreen = 0x500F,
        ///<summary></summary>
        HalftoneShape = 0x500D,
        ///<summary></summary>
        HostComputer = 0x013C,
        ///<summary></summary>
        ICCProfile = 0x8773,
        ///<summary></summary>
        ICCProfileDescriptor = 0x0302,
        ///<summary></summary>
        ImageDescription = 0x010E,
        ///<summary></summary>
        ImageHeight = 0x0101,
        ///<summary></summary>
        ImageTitle = 0x0320,
        ///<summary></summary>
        ImageWidth = 0x0100,
        ///<summary></summary>
        IndexBackground = 0x5103,
        ///<summary></summary>
        IndexTransparent = 0x5104,
        ///<summary></summary>
        InkNames = 0x014D,
        ///<summary></summary>
        InkSet = 0x014C,
        ///<summary></summary>
        JPEGACTables = 0x0209,
        ///<summary></summary>
        JPEGDCTables = 0x0208,
        ///<summary></summary>
        JPEGInterFormat = 0x0201,
        ///<summary></summary>
        JPEGInterLength = 0x0202,
        ///<summary></summary>
        JPEGLosslessPredictors = 0x0205,
        ///<summary></summary>
        JPEGPointTransforms = 0x0206,
        ///<summary></summary>
        JPEGProc = 0x0200,
        ///<summary></summary>
        JPEGQTables = 0x0207,
        ///<summary></summary>
        JPEGQuality = 0x5010,
        ///<summary></summary>
        JPEGRestartInterval = 0x0203,
        ///<summary></summary>
        LoopCount = 0x5101,
        ///<summary></summary>
        LuminanceTable = 0x5090,
        ///<summary></summary>
        MaxSampleValue = 0x0119,
        ///<summary></summary>
        MinSampleValue = 0x0118,
        ///<summary></summary>
        NewSubfileType = 0x00FE,
        ///<summary></summary>
        NumberOfInks = 0x014E,
        ///<summary></summary>
        Orientation = 0x0112,
        ///<summary></summary>
        PageName = 0x011D,
        ///<summary></summary>
        PageNumber = 0x0129,
        ///<summary></summary>
        PaletteHistogram = 0x5113,
        ///<summary></summary>
        PhotometricInterp = 0x0106,
        ///<summary></summary>
        PixelPerUnitX = 0x5111,
        ///<summary></summary>
        PixelPerUnitY = 0x5112,
        ///<summary></summary>
        PixelUnit = 0x5110,
        ///<summary></summary>
        PlanarConfig = 0x011C,
        ///<summary></summary>
        Predictor = 0x013D,
        ///<summary></summary>
        PrimaryChromaticities = 0x013F,
        ///<summary></summary>
        PrintFlags = 0x5005,
        ///<summary></summary>
        PrintFlagsBleedWidth = 0x5008,
        ///<summary></summary>
        PrintFlagsBleedWidthScale = 0x5009,
        ///<summary></summary>
        PrintFlagsCrop = 0x5007,
        ///<summary></summary>
        PrintFlagsVersion = 0x5006,
        ///<summary></summary>
        REFBlackWhite = 0x0214,
        ///<summary></summary>
        ResolutionUnit = 0x0128,
        ///<summary></summary>
        ResolutionXLengthUnit = 0x5003,
        ///<summary></summary>
        ResolutionXUnit = 0x5001,
        ///<summary></summary>
        ResolutionYLengthUnit = 0x5004,
        ///<summary></summary>
        ResolutionYUnit = 0x5002,
        ///<summary></summary>
        RowsPerStrip = 0x0116,
        ///<summary></summary>
        SampleFormat = 0x0153,
        ///<summary></summary>
        SamplesPerPixel = 0x0115,
        ///<summary></summary>
        SMaxSampleValue = 0x0155,
        ///<summary></summary>
        SMinSampleValue = 0x0154,
        ///<summary></summary>
        SoftwareUsed = 0x0131,
        ///<summary></summary>
        SRGBRenderingIntent = 0x0303,
        ///<summary></summary>
        StripBytesCount = 0x0117,
        ///<summary></summary>
        StripOffsets = 0x0111,
        ///<summary></summary>
        SubfileType = 0x00FF,
        ///<summary></summary>
        T4Option = 0x0124,
        ///<summary></summary>
        T6Option = 0x0125,
        ///<summary></summary>
        TargetPrinter = 0x0151,
        ///<summary></summary>
        ThreshHolding = 0x0107,
        ///<summary></summary>
        ThumbnailArtist = 0x5034,
        ///<summary></summary>
        ThumbnailBitsPerSample = 0x5022,
        ///<summary></summary>
        ThumbnailColorDepth = 0x5015,
        ///<summary></summary>
        ThumbnailCompressedSize = 0x5019,
        ///<summary></summary>
        ThumbnailCompression = 0x5023,
        ///<summary></summary>
        ThumbnailCopyRight = 0x503B,
        ///<summary></summary>
        ThumbnailData = 0x501B,
        ///<summary></summary>
        ThumbnailDateTime = 0x5033,
        ///<summary></summary>
        ThumbnailEquipMake = 0x5026,
        ///<summary></summary>
        ThumbnailEquipModel = 0x5027,
        ///<summary></summary>
        ThumbnailFormat = 0x5012,
        ///<summary></summary>
        ThumbnailHeight = 0x5014,
        ///<summary></summary>
        ThumbnailImageDescription = 0x5025,
        ///<summary></summary>
        ThumbnailImageHeight = 0x5021,
        ///<summary></summary>
        ThumbnailImageWidth = 0x5020,
        ///<summary></summary>
        ThumbnailOrientation = 0x5029,
        ///<summary></summary>
        ThumbnailPhotometricInterp = 0x5024,
        ///<summary></summary>
        ThumbnailPlanarConfig = 0x502F,
        ///<summary></summary>
        ThumbnailPlanes = 0x5016,
        ///<summary></summary>
        ThumbnailPrimaryChromaticities = 0x5036,
        ///<summary></summary>
        ThumbnailRawBytes = 0x5017,
        ///<summary></summary>
        ThumbnailRefBlackWhite = 0x503A,
        ///<summary></summary>
        ThumbnailResolutionUnit = 0x5030,
        ///<summary></summary>
        ThumbnailResolutionX = 0x502D,
        ///<summary></summary>
        ThumbnailResolutionY = 0x502E,
        ///<summary></summary>
        ThumbnailRowsPerStrip = 0x502B,
        ///<summary></summary>
        ThumbnailSamplesPerPixel = 0x502A,
        ///<summary></summary>
        ThumbnailSize = 0x5018,
        ///<summary></summary>
        ThumbnailSoftwareUsed = 0x5032,
        ///<summary></summary>
        ThumbnailStripBytesCount = 0x502C,
        ///<summary></summary>
        ThumbnailStripOffsets = 0x5028,
        ///<summary></summary>
        ThumbnailTransferFunction = 0x5031,
        ///<summary></summary>
        ThumbnailWhitePoint = 0x5035,
        ///<summary></summary>
        ThumbnailWidth = 0x5013,
        ///<summary></summary>
        ThumbnailYCbCrCoefficients = 0x5037,
        ///<summary></summary>
        ThumbnailYCbCrPositioning = 0x5039,
        ///<summary></summary>
        ThumbnailYCbCrSubsampling = 0x5038,
        ///<summary></summary>
        TileByteCounts = 0x0145,
        ///<summary></summary>
        TileLength = 0x0143,
        ///<summary></summary>
        TileOffset = 0x0144,
        ///<summary></summary>
        TileWidth = 0x0142,
        ///<summary></summary>
        TransferFunction = 0x012D,
        ///<summary></summary>
        TransferRange = 0x0156,
        ///<summary></summary>
        WhitePoint = 0x013E,
        ///<summary></summary>
        XPosition = 0x011E,
        ///<summary></summary>
        XResolution = 0x011A,
        ///<summary></summary>
        YCbCrCoefficients = 0x0211,
        ///<summary></summary>
        YCbCrPositioning = 0x0213,
        ///<summary></summary>
        YCbCrSubsampling = 0x0212,
        ///<summary></summary>
        YPosition = 0x011F,
        ///<summary></summary>
        YResolution = 0x011B
    }
}
 