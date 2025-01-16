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

        public static string BrokenImage()
        {
            var ret = "";
            ret= "PCFET0NUWVBFIGh0bWw+DQo8aHRtbCBsYW5nPSJlbiI+DQogIDxoZWFkPg0KICAgICAgPG1ldGEgY2hhcnNldD0idXRmLTgiPg0KDQogICAgICA8dGl0bGU+Q29ubmVjdG5pZ2VyaWEgfCBFcnJvciA0MDQ8L3RpdGxlPg0KDQogICAgICA8bWV0YSBuYW1lPSJyb2JvdHMiIGNvbnRlbnQ9Ik5PSU5ERVgsIE5PRk9MTE9XLCBOT0FSQ0hJVkUsIE5PU05JUFBFVCIgLz4NCiAgICAgIDxsaW5rIHJlbD0ic2hvcnRjdXQgaWNvbiIgaHJlZj0iaHR0cHM6Ly9kZXZjbm1haW4uY29ubmVjdG5pZ2VyaWEuY29tL2Fzc2V0cy9pY28vZmF2aWNvbi5pY28iIHR5cGU9ImltYWdlL2ljbyI+DQoNCiAgICAgIDxtZXRhIG5hbWU9InZpZXdwb3J0IiBjb250ZW50PSJ3aWR0aD1kZXZpY2Utd2lkdGgsIGluaXRpYWwtc2NhbGU9MS4wLCBtYXhpbXVtLXNjYWxlPTEuMCwgdXNlci1zY2FsYWJsZT1ubyIgLz4NCiAgICAgIDxsaW5rIHJlbD0ibWFuaWZlc3QiIGhyZWY9Imh0dHBzOi8vd3d3LmNvbm5lY3RuaWdlcmlhLmNvbS9hc3NldHMvbWFuaWZlc3QuanNvbiIgLz4NCg0KICAgICAgPGxpbmsgaHJlZj0iaHR0cHM6Ly9kZXZjbm1haW4uY29ubmVjdG5pZ2VyaWEuY29tL2Fzc2V0cy9wbHVnaW5zL2ZvdW5kcnkvY3NzL2ZvbnQtYXdlc29tZS5taW4uY3NzIiByZWw9InN0eWxlc2hlZXQiIHR5cGU9InRleHQvY3NzIiBtZWRpYT0iYWxsIj4NCiAgICAgIDxsaW5rIGhyZWY9Imh0dHBzOi8vZGV2Y25tYWluLmNvbm5lY3RuaWdlcmlhLmNvbS9hc3NldHMvcGx1Z2lucy9ib290c3RyYXAvY3NzL2Jvb3RzdHJhcC5taW4uY3NzIiByZWw9InN0eWxlc2hlZXQiIHR5cGU9InRleHQvY3NzIiBtZWRpYT0iYWxsIiAvPg0KICAgICAgPGxpbmsgaHJlZj0iaHR0cHM6Ly9kZXZjbm1haW4uY29ubmVjdG5pZ2VyaWEuY29tL2Fzc2V0cy9wbHVnaW5zL2FuaW1hdGVjc3MvY3NzL2FuaW1hdGUubWluLmNzcyIgcmVsPSJzdHlsZXNoZWV0IiB0eXBlPSJ0ZXh0L2NzcyIgbWVkaWE9ImFsbCIgLz4NCiAgICAgIDxsaW5rIGhyZWY9Imh0dHBzOi8vZGV2Y25tYWluLmNvbm5lY3RuaWdlcmlhLmNvbS9hc3NldHMvcGx1Z2lucy9pbWFnZWhvdmVyL2Nzcy9pbWFnZWhvdmVyLm1pbi5jc3MiIHJlbD0ic3R5bGVzaGVldCIgdHlwZT0idGV4dC9jc3MiIG1lZGlhPSJhbGwiIC8+DQogICAgICA8bGluayBocmVmPSJodHRwczovL2RldmNubWFpbi5jb25uZWN0bmlnZXJpYS5jb20vYXNzZXRzL2Nzcy92cTlkOVVEYVdoLmNzcyIgcmVsPSJzdHlsZXNoZWV0IiB0eXBlPSJ0ZXh0L2NzcyIgbWVkaWE9ImFsbCIgLz4NCiAgPC9oZWFkPg0KICA8Ym9keT4NCiAgICA8c2VjdGlvbiBjbGFzcz0iZnVsbHNjcmVlbiI+DQogICAgICA8ZGl2IGNsYXNzPSJjb250YWluZXIgdi1hbGlnbi10cmFuc2Zvcm0iPg0KICAgICAgICAgIDxkaXYgY2xhc3M9InJvdyI+DQogICAgICAgICAgICAgIDxkaXYgY2xhc3M9ImNvbC1zbS0xMCBjb2wtc20tb2Zmc2V0LTEiPg0KICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGV4dC1jZW50ZXIiPg0KICAgICAgICAgICAgICAgICAgICAgIDxpIGNsYXNzPSJ0aS1yZWNlaXB0IGljb24gaWNvbi1sZyBtYjI0IG1iLXhzLTAiPjwvaT4NCiAgICAgICAgICAgICAgICAgICAgICA8aDEgY2xhc3M9ImxhcmdlIj5QYWdlIE5vdCBGb3VuZDwvaDE+DQogICAgICAgICAgICAgICAgICAgICAgPHA+VGhlIHBhZ2UgeW91IHJlcXVlc3RlZCBjb3VsZG4ndCBiZSBmb3VuZCAtIHRoaXMgY291bGQgYmUgZHVlIHRvIGEgc3BlbGxpbmcgZXJyb3IgaW4gdGhlIFVSTCBvciBhIHJlbW92ZWQgcGFnZS48L3A+DQogICAgICAgICAgICAgICAgICAgICAgPGEgY2xhc3M9ImJ0biIgaHJlZj0iaHR0cHM6Ly93d3cuY29ubmVjdG5pZ2VyaWEuY29tLyI+R28gQmFjayBIb21lPC9hPg0KICAgICAgICAgICAgICAgICAgICAgIDxhIGNsYXNzPSJidG4iIGhyZWY9Imh0dHBzOi8vd3d3LmNvbm5lY3RuaWdlcmlhLmNvbS9wYWdlL2NvbnRhY3QtdXMiPkNvbnRhY3QgVXM8L2E+DQogICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgPC9kaXY+DQogICAgICAgICAgPCEtLWVuZCBvZiByb3ctLT4NCiAgICAgICAgICA8ZGl2IGNsYXNzPSJlbWJlbGlzaC1pY29ucyI+DQogICAgICAgICAgICAgIDxpIGNsYXNzPSJ0aS1oZWxwLWFsdCI+PC9pPg0KICAgICAgICAgICAgICA8aSBjbGFzcz0idGktY3Jvc3MiPjwvaT4NCiAgICAgICAgICAgICAgPGkgY2xhc3M9InRpLXN1cHBvcnQiPjwvaT4NCiAgICAgICAgICAgICAgPGkgY2xhc3M9InRpLWFubm91bmNlbWVudCI+PC9pPg0KICAgICAgICAgICAgICA8aSBjbGFzcz0idGktc2lnbmFsIj48L2k+DQogICAgICAgICAgICAgIDxpIGNsYXNzPSJ0aS1wdWxzZSI+PC9pPg0KICAgICAgICAgICAgICA8aSBjbGFzcz0idGktbWFya2VyIj48L2k+DQogICAgICAgICAgICAgIDxpIGNsYXNzPSJ0aS1wdWxzZSI+PC9pPg0KICAgICAgICAgICAgICA8aSBjbGFzcz0idGktYWxlcnQiPjwvaT4NCiAgICAgICAgICAgICAgPGkgY2xhc3M9InRpLWhlbHAtYWx0Ij48L2k+DQogICAgICAgICAgICAgIDxpIGNsYXNzPSJ0aS1hbGVydCI+PC9pPg0KICAgICAgICAgICAgICA8aSBjbGFzcz0idGktcHVsc2UiPjwvaT4NCiAgICAgICAgICA8L2Rpdj4NCiAgICAgIDwvZGl2Pg0KICAgICAgPCEtLWVuZCBvZiBjb250YWluZXItLT4NCiAgPC9zZWN0aW9uPg0KICAgICAgICAgIDxzY3JpcHQgdHlwZT0idGV4dC9qYXZhc2NyaXB0Ij4gDQoNCiAgICAgICAgICAgIHdpbmRvdy5jbl91cmwgPSAnaHR0cHM6Ly93d3cuY29ubmVjdG5pZ2VyaWEuY29tLyc7DQoNCiAgICAgICAgICAgIHdpbmRvdy5iYXNlX3VybCA9ICdodHRwczovL3d3dy5jb25uZWN0bmlnZXJpYS5jb20vJzsNCg0KICAgICAgICAgICAgd2luZG93LmNhcnNfdXJsID0gJ2h0dHBzOi8vY2Fycy5jb25uZWN0bmlnZXJpYS5jb20vJzsNCg0KICAgICAgICAgICAgd2luZG93LmRlYWxzX3VybCA9ICdodHRwczovL2RlYWxzLmNvbm5lY3RuaWdlcmlhLmNvbS8nOw0KDQogICAgICAgICAgICB3aW5kb3cuZXZlbnRzX3VybCA9ICdodHRwczovL2V2ZW50cy5jb25uZWN0bmlnZXJpYS5jb20vJzsNCg0KICAgICAgICAgICAgd2luZG93LnJlYWxlc3RhdGVfdXJsID0gJ2h0dHBzOi8vcmVhbGVzdGF0ZS5jb25uZWN0bmlnZXJpYS5jb20vJzsNCg0KICAgICAgICAgICAgd2luZG93LmpvYnNfdXJsID0gJ2h0dHBzOi8vam9icy5jb25uZWN0bmlnZXJpYS5jb20vJzsNCg0KDQogICAgICAgICAgICAvLyBjb25zdGFudCBnbG9iYWwgc291cmNlcyByZWZlcmVuY2UNCg0KICAgICAgICAgICAgIHdpbmRvdy5nbG9iYWxfc3JjID0gImh0dHBzOi8vZGV2Y25tYWluLmNvbm5lY3RuaWdlcmlhLmNvbS8iOw0KDQogICAgICAgICAgICAgd2luZG93LmxvY2FsX3NyYyA9ICJodHRwczovL3d3dy5jb25uZWN0bmlnZXJpYS5jb20vYXNzZXRzLyI7DQoNCiAgICAgICAgICAgICANCiAgICAgICAgICAgIHdpbmRvdy5wYWdlID0gJyc7DQogICAgICAgICAgICB3aW5kb3cuY3RybCA9ICcnOw0KDQogICAgICAgIDwvc2NyaXB0Pg0KICAgICAgICA8c2NyaXB0IGRlZmVyIHNyYz0iaHR0cHM6Ly93d3cuY29ubmVjdG5pZ2VyaWEuY29tL2Fzc2V0cy9hc3NldHMvanMvYm9vdC5taW4uanMiPjwvc2NyaXB0Pg0KICA8L2JvZHk+DQo8L2h0bWw+DQoNCg===";

            ret = @"R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==";

            return ret;
        }
        public static string SearchGlassImage() {
            string ret = "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";
            return ret;
        
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

        public Image GetImageFromBase64String(string base64)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String(base64);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
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
 