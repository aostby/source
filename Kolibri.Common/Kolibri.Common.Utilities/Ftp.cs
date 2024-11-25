using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
namespace Kolibri.Common.Utilities
{

    public static class Ftp
    {
        private const bool UsePassive = true;

        // http://www.albahari.com/nutshell/ch14.html
        public static long GetSize(string url)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(url);
                req.KeepAlive = false;
                req.UsePassive = UsePassive;
                req.Method = WebRequestMethods.Ftp.GetFileSize;

                using (WebResponse resp = req.GetResponse())
                {
                    long len = resp.ContentLength;
                    resp.Close();
                    return len;
                }
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        ///   Bruker og passord må ligge i ftp-url på denne måten: ftp://bruker:passord@ftp.maritech.no/upload/
        ///   ftp.Credentials = new NetworkCredential("kunder", "rednuk");
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contents"></param>
        public static void Upload(string url, byte[] contents)
        {
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(url);
            ftp.KeepAlive = false;
            ftp.UsePassive = UsePassive;
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            ftp.ContentLength = contents.Length;

            ftp.EnableSsl = true;

            // Bruker og passord må ligge i ftp-url på denne måten: ftp://bruker:passord@ftp.maritech.no/upload/
            //ftp.Credentials = new NetworkCredential("kunder", "rednuk");

            using (System.IO.Stream s = ftp.GetRequestStream())
            {
                s.Write(contents, 0, contents.Length);
                s.Close();
            }

            using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                string msg = string.Format("{0}\n{1}", response.StatusCode, response.StatusDescription);
                bool ok = response.StatusCode == FtpStatusCode.ClosingData;
                response.Close();

                if (!ok)
                    throw new ApplicationException(msg);
            }
        }

        public static void Rename(string fromUrl, string toFile)
        {
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(fromUrl);
            ftp.KeepAlive = false;
            ftp.UsePassive = UsePassive;
            ftp.Method = WebRequestMethods.Ftp.Rename;
            ftp.RenameTo = toFile;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftp.GetResponse())
            {
                using (Stream responseStream = ftpResponse.GetResponseStream())
                {
                    responseStream.Close();
                }
                string msg = string.Format("{0}\n{1}", ftpResponse.StatusCode, ftpResponse.StatusDescription);
                bool ok = ftpResponse.StatusCode == FtpStatusCode.FileActionOK;
                ftpResponse.Close();

                if (!ok)
                    throw new ApplicationException(msg);
            }
        }

        public static void DeleteFile(string fromUrl)
        {
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(fromUrl);
            ftp.KeepAlive = false;
            ftp.UsePassive = UsePassive;
            ftp.Method = WebRequestMethods.Ftp.DeleteFile;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftp.GetResponse())
            {
                using (Stream responseStream = ftpResponse.GetResponseStream())
                {
                    responseStream.Close();
                }

                string msg = string.Format("{0}\n{1}", ftpResponse.StatusCode, ftpResponse.StatusDescription);
                bool ok = ftpResponse.StatusCode == FtpStatusCode.FileActionOK;
                ftpResponse.Close();

                if (!ok)
                    throw new ApplicationException(msg);
            }
        }

        public static string[] GetFiles(string uri)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
            req.KeepAlive = false;
            req.UsePassive = UsePassive;
            req.Method = WebRequestMethods.Ftp.ListDirectory;

            using (WebResponse resp = req.GetResponse())
            {
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    string tmp = reader.ReadToEnd();
                    return tmp.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        public static string DownloadText(string uri, System.Text.Encoding encoding)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = UsePassive;
            ftpRequest.KeepAlive = false;

            string xml;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (Stream responseStream = ftpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream, encoding))
                    {
                        xml = reader.ReadToEnd();
                    }
                    responseStream.Close();
                }
                ftpResponse.Close();
            }
            return xml;
        }

        public static bool DownloadToFile(string uri, string localFileName)
        {
            bool ret = true;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = UsePassive;
                ftpRequest.KeepAlive = false;
                WebClient wc = new WebClient();
                wc.Credentials = ftpRequest.Credentials;
                wc.DownloadFile(uri, localFileName);
                if (File.Exists(localFileName))
                    ret = true;
                else
                    ret = false;

            }
            catch (Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                ret = false;
            }
            return ret;
        }

        public static byte[] DownloadFile(string uri)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = UsePassive;
            ftpRequest.KeepAlive = false;

            int size = 0;
            using(MemoryStream output = new MemoryStream())
            {
                using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    using (Stream responseStream = ftpResponse.GetResponseStream())
                    {
                        byte[] buffer = new byte[65536];
                        int len = responseStream.Read(buffer, 0, buffer.Length);
                        while (len > 0)
                        {
                            size += len;
                            output.Write(buffer, 0, len);
                            len = responseStream.Read(buffer, 0, buffer.Length);
                        }
                        responseStream.Close();
                    }
                    ftpResponse.Close();
                }
                output.Close();
                return output.ToArray();
            }
        }
    }
}