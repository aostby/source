using com.sun.xml.@internal.fastinfoset.util;
using java.text;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace Kolibri.net.Common.Utilities
{


    public class ByteUtilities
        {
            public static byte[] ReadFile(string sPath)
            {
                byte[] data = null;
                try
                {
                    FileInfo fInfo = new FileInfo(sPath);
                    long numBytes = fInfo.Length;
                    FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fStream);
                    data = br.ReadBytes((int)numBytes);
                    br.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                return data;
            }

            /// <summary>
            /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
            /// </summary>
            /// <param name="characters">Unicode Byte Array to be converted to String</param>
            /// <returns>String converted from Unicode Byte Array</returns>
            public static String UTF8ByteArrayToString(Byte[] characters)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                String constructedString = encoding.GetString(characters);
                return (constructedString);
            }

            /// <summary>
            /// Converts the String to UTF8 Byte array and is used in Deserialization
            /// </summary>
            /// <param name="pXmlString"></param>
            /// <returns></returns>
            public static Byte[] StringToUTF8ByteArray(String pXmlString)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(pXmlString);
                return byteArray;
            }

            public static string ByteArrayToString(Byte[] characters, Encoding enc)
            {
                String constructedString = enc.GetString(characters);
                return (constructedString);
            }
            public static Byte[] StringToByteArray(String pXmlString, Encoding enc)
            {
                Byte[] byteArray = enc.GetBytes(pXmlString);
                return byteArray;
            }

            /// <summary>
            /// Metode som pakker ut en zippet byte[]
            /// </summary>
            /// <param name="arrayToDecompress"></param>
            /// <returns></returns>
            public static Byte[] GZipDecompress(Byte[] arrayToDecompress)
            {

                byte[] ret = arrayToDecompress;
                //    zipxml = Convert.ToBase64String( (Utilities.ByteUtilities.StringToByteArray( rapp.Rapportxml, Encoding.Unicode));
                using (MemoryStream stream = new MemoryStream())
                {
                    using (MemoryStream stream2 = new MemoryStream(ret))
                    {
                        using (GZipStream stream3 = new GZipStream(stream2, CompressionMode.Decompress))
                        {
                            byte[] buffer = new byte[0x2000];
                            for (int i = stream3.Read(buffer, 0, buffer.Length); i > 0; i = stream3.Read(buffer, 0, buffer.Length))
                            {
                                stream.Write(buffer, 0, i);
                            }
                        }
                    }
                    ret = stream.ToArray();
                }
                return ret;
            }

            public static Byte[] GZipCompress(Byte[] arrayToCompress)
            {
                byte[] ret = arrayToCompress;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress))
                    {
                        byte[] buffer = ret;//Maritech.Utils.XmlToBytes(new System.Xml.XmlDocument());
                        gz.Write(buffer, 0, buffer.Length);
                    }
                    byte[] compressed = stream.ToArray();
                    ret = compressed;
                }
                return ret;
            }
        /// <summary>
        /// Gets the number of bytes converted to it's closest textual representation
        /// </summary>
        /// <param name="byteCount">bytes to convert</param>
        /// <returns>string representation of the number of bytes</returns>
        public static string GetByteSize(long value) { 
  string suffix;
        double readable;
    switch (Math.Abs(value))
    {
        case >= 0x1000000000000000:
            suffix = "EiB";
            readable = value >> 50;
            break;
        case >= 0x4000000000000:
            suffix = "PiB";
            readable = value >> 40;
            break;
        case >= 0x10000000000:
            suffix = "TiB";
            readable = value >> 30;
            break;
        case >= 0x40000000:
            suffix = "GiB";
            readable = value >> 20;
            break;
        case >= 0x100000:
            suffix = "MiB";
            readable = value >> 10;
            break;
        case >= 0x400:
            suffix = "KiB";
            readable = value;
            break;
        default:
            return value.ToString("0 B");
    }

    return (readable / 1024).ToString("0.## ", CultureInfo.InvariantCulture) + suffix;
}

public static string GetByteSize_old(long byteCount)
            { //http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net

                try
                {
                    string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
                    if (byteCount == 0)
                        return "0" + suf[0];
                    long bytes = Math.Abs(byteCount);
                    int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                    double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                    return (Math.Sign(byteCount) * num).ToString() +$" {suf[place]}";
                }
                catch (Exception)
                {
                    return "0 KB";
                }
            }



            /// <summary>
            ///  Stream.Read doesn't guarantee that it will read everything it's asked for. 
            ///  If you're reading from a network stream, for example, it may read one packet's worth and then return, even if there will be 
            ///  more data soon. BinaryReader.Read will keep going until the end of the stream or your specified size, but you still have 
            ///  to know the size to start with.
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


            public static bool IsGZip(byte[] arr)
            {
                return arr.Length >= 2 && arr[0] == 31 && arr[1] == 139;
            }


            /// <summary>
            /// Encode
            /// </summary>
            /// <param name="plainText"></param>
            /// <returns></returns>
            public static string Base64Encode(string plainText)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }

            /// <summary>
            /// Decode
            /// </summary>
            /// <param name="base64EncodedData"></param>
            /// <returns></returns>
            public static string Base64Decode(string base64EncodedData)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }
    }
