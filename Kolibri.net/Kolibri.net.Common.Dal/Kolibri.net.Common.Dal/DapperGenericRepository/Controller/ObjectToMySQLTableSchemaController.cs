
using LiteDB;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Kolibri.net.Common.Dal.DapperGenericRepository.Controller
{
    public class ObjectToMySQLTableSchemaController
    {
        private Dictionary<string, string> datatypeDic = new Dictionary<string, string>()
        {   { "varchar", "string" }
            ,{ "char" , "string" }
            ,{ "tinytext" ,"string"  }
            ,{ "text", "string" }
            ,{ "longtext", "string" }
            ,{ "datetime", "DateTime" }
            ,{ "int", "int" }
            ,{ "bit", "int" }
            ,{ "bigint", "int" }
            ,{ "double", "double" }
            ,{ "decimal", "double" }
            ,{ "date", "DateTime" }
            ,{ "tinyint", "bool" }
        };

        /// <summary>
        /// Svært forenklet skjemabygger, krever mer ved avanserte datatyper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string CreateSchema<T>(T obj)
        {
            int totalLength = 0;
            List<string> longLength = new List<string>() {"Plot", "TomatoUrl", "Writer", "Poster" };
            int length = 150;//"(65532)"; //https://stackoverflow.com/questions/332798/equivalent-of-varcharmax-in-mysql
            string ret = $"CREATE TABLE IF NOT EXISTS {obj.GetType().Name} {Environment.NewLine}";
           
            string primaryKey = null;
            StringBuilder sb = new StringBuilder();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            int counter = 0;
            foreach (PropertyDescriptor prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                
                var myKey = datatypeDic.FirstOrDefault(x => x.Value.Equals(type.Name, StringComparison.OrdinalIgnoreCase)).Key;
                if (myKey==null) 
                {
                    myKey = "varchar";
                }


                //trenger ikke lengde dersom ikke varchar, finn ut
                string col = $"{prop.Name} {myKey}({(length)})";
                if (longLength.Contains(prop.Name)) {
                    col = $"{prop.Name} TEXT ";
                }
                if (prop.Name.Equals("ImdbId"))
                    col += " UNIQUE ";

                if (counter == 0)
                {  
                    primaryKey = prop.Name;
                    col += " NOT NULL";
                }
                
                sb.AppendLine(col+ ",");
                counter++;
            }
            sb.Insert(0,  $" _Id int(11) NOT NULL AUTO_INCREMENT,{Environment.NewLine}"); 
            ret += $"({sb.ToString().Trim().TrimEnd(',')} {$", PRIMARY KEY(_Id)"});";
            if (totalLength >= 65532) throw new Exception($"Exceeds totallength: {totalLength}");
            return ret;
        }
    }
}
