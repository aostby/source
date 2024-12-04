using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Kolibri.net.Common.Utilities
{
    public class ZipUtilities
    {

        public static void UnZipFile(FileInfo zipArchive, DirectoryInfo destinationPath)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(zipArchive.FullName, destinationPath.FullName);
        }

        public static void UnZipFile(Stream zipArchive, DirectoryInfo destinationPath)
        {
            using (var zip = new System.IO.Compression.ZipArchive(zipArchive, System.IO.Compression.ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    using (var stream = entry.Open())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            FileInfo info = new FileInfo(Path.Combine(destinationPath.FullName, entry.FullName));
                            if (!info.Directory.Exists)
                                info.Directory.Create();
                            FileUtilities.WriteByteArrayToFile(memoryStream.ToArray(), info.FullName);
                        }
                    }
                }
            }
        }

        public static void ZipFile(FileInfo inputFilePath)
        {
            string outPath = inputFilePath.FullName.Replace(inputFilePath.Extension, ".zip");// File.inputFolderPath + @"\" + outputPathAndFile;


            using (FileStream fs = new FileStream(outPath, FileMode.Create))
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                arch.CreateEntryFromFile(inputFilePath.FullName, inputFilePath.Name);
            }
        }


        #region ICSHarpLib Zip-related methods
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

        /*     [Obsolete("Use the overload with exact outputfilename as FileInfo instead")]
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
               }*/


        /// <summary>
        /// Metode som zipper en fil ved å ta inn en full filsti. filen blir laget med ekstenssjonen .zip som output
        /// </summary>
        /// <param name="inputFilePath">filen som skal zippes</param>
        /// <param name="password">Hvis null krypteres ikke filen, ellers blir oppgitt string brukt til passord</param>
        /// <returns></returns>
        /*      public static bool ZipFile(string inputFilePath, string password)
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
      */
        /*   public static void ZipFiles(DirectoryInfo inputFolderPath, FileInfo outputFullName, string password)
           {
               FileInfo[] arr = inputFolderPath.GetFiles("*.*", SearchOption.TopDirectoryOnly);
               FileStream ostream;
               byte[] obuffer;
               string outPath = outputFullName.FullName;
               ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
               if (!String.IsNullOrEmpty(password)) oZipStream.Password = password;    // set password if provided
               oZipStream.SetLevel(9); // maximum compression
               ZipEntry oZipEntry;
               foreach (FileInfo Fil in arr) // for each file, generate a zipentry
               {
                   oZipEntry = new ZipEntry(Fil.FullName);
                   oZipStream.PutNextEntry(oZipEntry);
                       ostream = File.OpenRead(Fil.FullName);
                       obuffer = new byte[ostream.Length];
                       ostream.Read(obuffer, 0, obuffer.Length);
                       oZipStream.Write(obuffer, 0, obuffer.Length);
               }
               oZipStream.Finish();
               oZipStream.Close();
           }*/




        /*public static bool UnZipFiles(string zipPathAndFile, string outputFolder, string password)
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
        */
        /*  public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
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
          }*/
        #endregion


         
    }
}
