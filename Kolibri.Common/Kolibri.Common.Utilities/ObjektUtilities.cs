using System;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kolibri.Common.Utilities
{
    public class ObjektUtilities
    {
        #region Private Members

        // We are interested in non-static, public properties with getters and setters   
        private  const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        #endregion  

        /// <summary>
        /// Konverter ett array av objekter til string verdier av objektene.
        /// nyttig ved f.eks å sende inn dataView.ToTable().Rows[0].ItemArray
        /// </summary>
        /// <param name="itemArr"></param>
        /// <returns></returns>
        public static string[] ObjectArrayToStringArray(object[] itemArr)
        {
            string[] ret = null;
            try
            {
                string[] array = Array.ConvertAll<object, string>(itemArr, delegate(object obj) { return obj.ToString().Trim(); });
                ret = array;
                //string[] array = Array.ConvertAll<object, string>(dataView.ToTable().Rows[0].ItemArray,    delegate(object obj) { return obj.ToString(); });

            }
            catch (Exception)
            { }
            return ret;
        }

        public static string GenerellBeskrivelse(Object obj)
        {
            string ret = string.Empty;
            //ret += "Generell beskrivelse for :;" + obj.GetType().ToString() + ", kalt fra :;" + Assembly.GetCallingAssembly().FullName + ";";

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.GetValue(obj, null) != null)
                    ret += " Entitet: " + property.Name + " Verdi: " + property.GetValue(obj, null).ToString() + ";";
                else
                    ret += " Entitet: " + property.Name + " Verdi: null;";

                ret += Environment.NewLine;
            }
            return ret;
        }

        #region ReadResourceFile
        /// <summary>
        /// method for reading a value from a resource file (.resx file)
        /// </summary>
        /// <param name="file">file to read from</param>
        /// <param name="key">key to get the value for</param>
        /// <returns>a string value</returns>
        public static string ReadResourceValue(string file, string key)
        {
            //value for our return value
            string resourceValue = string.Empty;
            try
            {
                // specify your resource file name 
                string resourceFile = file;
                // get the path of your file
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                // create a resource manager for reading from
                //the resx file
                ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(resourceFile, filePath, null);
                // retrieve the value of the specified key
                resourceValue = resourceManager.GetString(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resourceValue = string.Empty;
            }
            return resourceValue;
        }

        public static string ReadResource(string name)
        {
            UnpackResource();
            return GetUnpackResourcePath().GetFiles($"*{name}*").FirstOrDefault().FullName;
           
        }

        public static DirectoryInfo GetUnpackResourcePath()
        {
            DirectoryInfo ret = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestBench", "Utilities"));
            return ret;
        }

        private static void UnpackResource()
        {
            //https://msdn.microsoft.com/en-us/library/system.environment.specialfolder(v=vs.110).aspx
            //  m_tempPath = new DirectoryInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", "TransformForm"));
            //m_tempPath = new DirectoryInfo(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), "TestBench", "TransformForm"));
            //DirectoryInfo m_tempPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestBench", "TransformForm"));
            DirectoryInfo m_tempPath = GetUnpackResourcePath();
            DirectoryInfo m_images = m_tempPath.Parent;

            FileInfo info;
            string destFolderName = destFolderName = "XSLT";
            foreach (var item in Controller.ResourceController.GetResourceNames())
            {
                string[] nameArr = Path.GetFileName(item).Replace(Path.GetExtension(item), string.Empty).Split(".".ToCharArray()); //
                if (item.EndsWith(".xsl", StringComparison.OrdinalIgnoreCase) || item.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase) || item.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    if (Path.GetExtension(item).EndsWith("ZIP", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (Array.IndexOf(nameArr, "KITH") >= 0)
                        {
                            destFolderName = "KITH";
                        }

                        if (Array.IndexOf(nameArr, "FHIR") >= 0)
                        {
                            destFolderName = "FHIR";
                        }

                        if (Array.IndexOf(nameArr, "biztalk2013documenter") >= 0
                            || Array.IndexOf(nameArr, "biztalk2013images") >= 0
                            )
                        { destFolderName = "biztalk2013documenter"; }
                        try
                        {
                            info = new FileInfo(Path.Combine(m_tempPath.FullName, destFolderName, nameArr[nameArr.Length - 1]) + ".zip");
                            if (!info.Directory.Exists) info.Directory.Create();
                            byte[] arr = Controller.ResourceController.GetResourceBytes(Assembly.GetAssembly(typeof(Controller.ResourceController)), item);
                            File.WriteAllBytes(info.FullName, arr);
                            if (Path.GetFileNameWithoutExtension(info.FullName).Equals("biztalk2013images"))
                            {
                                FileInfo imageInfo = new FileInfo(Path.Combine(info.Directory.Parent.FullName, info.Name));
                                File.WriteAllBytes(info.FullName, arr);
                            }


                        }
                        catch (Exception)
                        { }

                    }
                    else
                    {
                        destFolderName = "XSLT";
                        try
                        {
                            string fullName = Path.GetFileName(item);
                            info = new FileInfo(Path.Combine(m_tempPath.FullName, destFolderName, nameArr[nameArr.Length - 1]) + ".xsl");
                            if (!info.Directory.Exists) info.Directory.Create();
                            File.WriteAllText(info.FullName, Controller.ResourceController.GetResourceString(Assembly.GetAssembly(typeof(Controller.ResourceController)), fullName));
                        }
                        catch (Exception)
                        { }
                    }
                }
            }
            try
            {
                foreach (FileInfo zFile in new DirectoryInfo(Path.Combine(m_tempPath.FullName)).GetFiles("*.zip", SearchOption.AllDirectories))
                {
                    try
                    {
                        ZipUtilities.UnZipFile(zFile, zFile.Directory);
                    }
                    catch (Exception ex)
                    { string jall = ex.Message; }
                }
            }
            catch (Exception) { }
            info = null;
        }
         
        #endregion

        /// <summary>   
        /// Copies all public properties from one object to another.   
        /// </summary>   
        /// <param name="fromType">The type of the from object, preferably an interface. We could infer this using reflection, but this allows us to contrain the copy to an interface.</param>   
        /// <param name="from">The object to copy from</param>   
        /// <param name="toType">The type of the to object, preferably an interface. We could infer this using reflection, but this allows us to contrain the copy to an interface.</param>   
        /// <param name="to">The object to copy to</param>   
        public static void Copy( Type fromType, object from, Type toType,  object to )
        {   // http://www.alteridem.net/2008/07/09/method-to-copy-data-between-objects-of-different-types/
            if ( fromType == null )   
                throw new ArgumentNullException( "fromType", "The type that you are copying from cannot be null" );   
  
            if ( from == null )   
                throw new ArgumentNullException( "from", "The object you are copying from cannot be null" );   
  
            if ( toType == null )   
                throw new ArgumentNullException( "toType", "The type that you are copying to cannot be null" );   
  
            if ( to == null )   
                throw new ArgumentNullException( "to", "The object you are copying to cannot be null" );   
  
            // Don't copy if they are the same object   
            if ( !ReferenceEquals( from, to ) )   
            {   
                // Get all of the public properties in the toType with getters and setters   
                Dictionary<string, PropertyInfo> toProperties = new Dictionary<string, PropertyInfo>();   
                PropertyInfo[] properties = toType.GetProperties( flags );   
                foreach ( PropertyInfo property in properties )   
                {   
                    toProperties.Add( property.Name, property );   
                }   
  
                // Now get all of the public properties in the fromType with getters and setters   
                properties = fromType.GetProperties( flags );   
                foreach ( PropertyInfo fromProperty in properties )   
                {   
                    // If a property matches in name and type, copy across   
                    if ( toProperties.ContainsKey( fromProperty.Name ) )   
                    {   
                        PropertyInfo toProperty = toProperties[fromProperty.Name];   
                        if ( toProperty.PropertyType == fromProperty.PropertyType )   
                        {   
                            object value = fromProperty.GetValue( from, null );   
                            toProperty.SetValue( to, value, null );   
                        }   
                    }   
                }   
            }   
        }

        /// <summary>
        /// Metode som tar inn en liste som parameter, og returnerer listen uten duplikater. forutsetter at objektene i listen kan sammenlignes
        /// </summary>
        /// <typeparam name="T">objekttype</typeparam>
        /// <param name="list">liste det skal fjernes dublikater fra</param>
        /// <returns>liste uten duplikater</returns>
        public static List<T> RemoveDuplicates<T>( List<T> list) {
            var ret = new List<T>();
            var hs = new HashSet<T>();
            foreach (T t in list)
                if (hs.Add(t))
                    ret.Add(t);
            return ret;
        }

        
        /// <summary>
        /// Metode som tar inn en enumerator, og henter beskrivelsen for denne
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            fi.GetOptionalCustomModifiers();
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

       

    }   
}  



