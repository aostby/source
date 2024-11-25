using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kolibri.Common.Utilities
{

    public static class DataSetUtilities
    {

        #region datatabeller til entiteter
        /// <summary>
        /// Konverterer en liste til DataTable. Forutsetter Public Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }


        /// <summary> Metode som tar inn en tabell og returnerern en Dictionary   </summary>
        /// <param name="ds">DataSet</param>
        /// <param name="keyColumn">key i dictionary</param>
        /// <param name="valueColumn">value i dictionary</param>
        /// <returns></returns>
        public static Dictionary<string, string> DataTableToDictionary(DataTable dt, string keyColumn, string valueColumn)
        {
            Dictionary<string, string> ret = dt.AsEnumerable()
            .ToDictionary<DataRow, string, string>(
            row => row.Field<string>($"{keyColumn}"),
            row => row.Field<string>($"{valueColumn}"));
            return ret;
        }
        public static Dictionary<string, string> DataTableToDictionary(DataSet ds, string keyColumn, string valueColumn)
        {
            return DataTableToDictionary(ds.Tables[0], keyColumn, valueColumn);
        }
        public static DataTable DynamicObjectToDataTable(dynamic dynItem)
        {
            string json = JsonConvert.SerializeObject(dynItem);

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            DataTable dt = dict.ToDataTable();
            return dt;
        }
        public static DataSet ColumnNamesToDataTable(params string[] arr)
        {
            return ColumnNamesToDataTable(arr.ToList());
        }
        public static DataSet ColumnNamesToDataTable(List<string> list)
        {
            DataTable table = new DataTable();
            table.Columns.AddRange(list.Select(a => new DataColumn() { ColumnName = a }).ToArray());
            table.TableName = "Table1";

            DataSet ds = new DataSet("DocumentElement");
            ds.Tables.Add(table);
            return ds;

        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        /// <summary>
        /// Konverterer en HASHTable til datatable
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="hashtable"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TKey, TValue>(
                this Dictionary<TKey, TValue> hashtable
            )
        {
            var dataTable = new DataTable(hashtable.GetType().Name);
            dataTable.Columns.Add("Key", typeof(object));
            dataTable.Columns.Add("Value", typeof(object));
            foreach (KeyValuePair<TKey, TValue> var in hashtable)
            {
                dataTable.Rows.Add(var.Key, var.Value);
            }
            return dataTable;
        }

        /// <summary>
        /// Konverterer en HashTable tli en DataTable
        /// </summary>
        /// <param name="hashtable"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this Hashtable hashtable)
        {
            var dataTable = new DataTable(hashtable.GetType().Name);
            dataTable.Columns.Add("Key", typeof(object));
            dataTable.Columns.Add("Value", typeof(object));

            foreach (DictionaryEntry var in hashtable)
            {
                dataTable.Rows.Add(var.Key, var.Value);
            }
            return dataTable;
        }

        public static List<T> ConvertToList<T>(this DataTable table) where T : new()
        {
            Type t = typeof(T);

            // Create a list of the entities we want to return
            List<T> returnObject = new List<T>();

            // Iterate through the DataTable's rows
            foreach (DataRow dr in table.Rows)
            {
                // Convert each row into an entity object and add to the list
                T newRow = dr.ConvertToEntity<T>();
                returnObject.Add(newRow);
            }

            // Return the finished list
            return returnObject;
        }

        public static T ConvertToEntity<T>(this DataRow tableRow) where T : new()
        {
            // Create a new type of the entity I want
            Type t = typeof(T);
            T returnObject = new T();

            foreach (DataColumn col in tableRow.Table.Columns)
            {
                string colName = col.ColumnName;

                // Look for the object's property with the columns name, ignore case
                PropertyInfo pInfo = t.GetProperty(colName.ToLower(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // did we find the property ?
                if (pInfo != null)
                {


                    object val = tableRow[colName];

                    // is this a Nullable<> type
                    bool IsNullable = (Nullable.GetUnderlyingType(pInfo.PropertyType) != null);
                    if (IsNullable)
                    {
                        if (val is System.DBNull)
                        {
                            val = null;
                        }
                        else
                        {
                            // Convert the db type into the T we have in our Nullable<T> type
                            val = Convert.ChangeType(val, Nullable.GetUnderlyingType(pInfo.PropertyType));
                        }
                    }
                    else
                    {
                        // Convert the db type into the type of the property in our entity
                        try
                        {
                            val = Convert.ChangeType(val, pInfo.PropertyType);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    // Set the value of the property with the value from the db
                    try
                    {
                        pInfo.SetValue(returnObject, val, null);
                    }
                    catch (Exception ex)
                    { }

                }
            }

            // return the entity object with values
            return returnObject;
        }
        #endregion

        #region autogenerer dataset utfra lister og objekter

        /// <summary>
        /// Reads an XML and converts it to a dataset
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns>Converted dataset</returns>
        public static DataSet ConvertXMLStringToDataSet(string xmlString)
        {
            DataSet ds = new DataSet();
            using (StringReader strReader = new StringReader(xmlString))
            {
                ds.ReadXml(strReader);
            }
            return ds;
        }



        /// <summary>
        /// Metode som autogenerer DataSet utifra arrayList objekter, og returnerer ett dataset med en tabell med objektenes verdi dersom listen ikke er tom, null ellers
        /// Forutsettning: Objektene i listen inneholder properties. Endres rekkefølgen på disse, endres også datasettet.
        /// Tips: dersom det kommer kollonner som du ikke vil benytte, bruk remove på tabellen, eks: ds.Tables[0].Columns.Remove("StillingsID");
        /// Tips: konverter en List T på denne måten  new ArrayList(data) der data er List of  T
        /// OBS: Semikolon i value kan påvirke utskriften om en bruker CSV
        /// </summary>
        /// <returns></returns>
        public static DataSet AutoGenererDataSet(ArrayList arrayList)
        {
            DataSet ret = null;
            DataSet ds = null;
            DataTable regTabell = null;
            try
            {
                ds = new DataSet();
                if (arrayList != null)
                {

                    foreach (Object info in arrayList)
                    {
                        Type objectType = info.GetType();
                        PropertyInfo[] properties = objectType.GetProperties();
                        if (regTabell == null)
                        {
                            regTabell = ds.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                            foreach (PropertyInfo prop in properties)
                            {
                                regTabell.Columns.Add(prop.Name);
                            }
                        }
                        DataRow row = regTabell.NewRow();
                        foreach (PropertyInfo prop in properties)
                        {
                            row[prop.Name] = prop.GetValue(info, null);
                        }
                        regTabell.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex) { }
            ret = ds;
            return ret;
        }
        public static DataSet AutoGenererDataSet<T>(List<T> liste)
        {
            ArrayList listeArr = new ArrayList();
            listeArr.AddRange(liste.ToArray());
            return AutoGenererDataSet(listeArr);

        }

        /// <summary>
        /// Samme som autogenerer dataset 
        /// </summary>
        /// <returns></returns>
        public static DataSet AutoGenererTypedDataSet(ArrayList arrayList, bool skipErrors)
        {
            DataSet ret = null;
            DataSet ds = new DataSet();
            DataTable regTabell = null;
            try
            {
                if (arrayList != null)
                {

                    foreach (Object info in arrayList)
                    {
                        Type objectType = info.GetType();
                        PropertyInfo[] properties = objectType.GetProperties();
                        if (regTabell == null)
                        {
                            regTabell = ds.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                            foreach (PropertyInfo prop in properties)
                            {
                                regTabell.Columns.Add(prop.Name, prop.PropertyType);
                            }
                        }
                        DataRow row = regTabell.NewRow();
                        foreach (PropertyInfo prop in properties)
                        {
                            try
                            {
                                row[prop.Name] = prop.GetValue(info, null);
                            }
                            catch (Exception ex)
                            {
                                if (!skipErrors)
                                    throw ex;
                            }
                        }
                        regTabell.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            ret = ds;
            return ret;
        }

        // this is the method I have been using
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    //   row[prop.Name] = prop.GetValue(item);
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                // HERE IS WHERE THE ERROR IS THROWN FOR NULLABLE TYPES // table.Columns.Add(prop.Name, prop.PropertyType);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            return table;
        }

        #endregion

        #region file related operations on datasets
        /// <summary>
        /// Metode som leser en xmlfil, og en tilhørende xsd fil for å returnere typet dataset. tvinger inn verdier i felter som er tomme hvis nødvendig (desimal og dato felter)
        /// </summary>
        /// <param name="fullpath">string med full filsti til xml. xsd må finnes med samme navn i stien</param>
        /// <returns>DataSet basert på xml, eller null hvis xsd ikke finns.</returns>
        /// <exception cref="Exception">kaster Exception hvis noe går galt</exception>
        public static DataSet ReadTypedDataSetFromXML(string fullpath)
        {
            DataSet ret = null;
            FileInfo info = new FileInfo(fullpath);
            if (File.Exists(info.FullName.Replace(info.Extension, ".xsd")))
            {

                try
                {
                    DataSet ds;

                    if (XMLUtilities.ValidateXml(info, new FileInfo(info.FullName.Replace(info.Extension, ".xsd")), false))
                    {
                        ds = new DataSet();
                        ds = new DataSet();
                        ds.ReadXmlSchema(info.FullName.Replace(info.Extension, ".xsd"));
                        ds.ReadXml(info.FullName);
                    }
                    else
                    {  // vi tvinger den inn

                        string evenmorejalla = FileUtilities.ReadTextFile(info.FullName);

                        ds = new DataSet();
                        DataSet temp = new DataSet();
                        temp.ReadXmlSchema(info.FullName.Replace(info.Extension, ".xsd"));
                        if (temp.Tables.Count > 0)
                        {
                            foreach (DataColumn col in temp.Tables[temp.Tables.Count - 1].Columns)
                            {
                                //Desimalkolonner tryner om de er tomme, eller har , istedet for punktuom
                                if (col.DataType.Equals(typeof(System.Decimal)))
                                {
                                    evenmorejalla = evenmorejalla.Replace(",", "."); // i hele stringen... kan forbedres?
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}/>", col.ColumnName), string.Format("<{0}>0.0</{0}>", col.ColumnName));
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}></{0}>", col.ColumnName), string.Format("<{0}>0.0</{0}>", col.ColumnName));
                                }
                                else if (col.DataType.Equals(typeof(System.DateTime)))
                                {
                                    //datetime tryner om de er tomme
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}/>", col.ColumnName), string.Format("<{0}>0001-01-01</{0}>", col.ColumnName));
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}></{0}>", col.ColumnName), string.Format("<{0}>0001-01-01</{0}>", col.ColumnName));
                                }
                                else if (col.DataType.Equals(typeof(System.Int32)))
                                {
                                    //Heltall tryner også                                 <alder/>
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}/>", col.ColumnName), string.Format("<{0}>0</{0}>", col.ColumnName));
                                    evenmorejalla = evenmorejalla.Replace(string.Format("<{0}></{0}>", col.ColumnName), string.Format("<{0}>0</{0}>", col.ColumnName));
                                }

                            }
                        }

                        ds = new DataSet();
                        ds.ReadXmlSchema(info.FullName.Replace(info.Extension, ".xsd"));
                        ds.GetXmlSchema();
                        evenmorejalla = evenmorejalla.Substring(evenmorejalla.IndexOf("<"));
                        evenmorejalla = evenmorejalla.Replace("encoding=\"UTF-16LE\"", "encoding=\"iso-8859-1\"");
                        ds.ReadXml(new StringReader(evenmorejalla));

                    }
                    ret = ds;
                }
                catch (Exception ex)
                { throw new Exception("Feil ved lesning av dataset xml: ", ex); }
            }
            return ret;
        }

        public static DataSet CSVToDataSet(string ascii, string separator, bool firstIsHeader)
        {
            DataSet ret = null;
            string[] header;
            string[] source = ascii.Split(Environment.NewLine.ToCharArray());
            source = StringUtilities.RemoveItem(source, "");
            char[] sep = separator.ToCharArray();

            if (firstIsHeader)
            {
                header = source[0].Split(sep);
            }
            else
            {
                header = new string[source[0].Split(sep).Length];
                for (int i = 0; i < header.Length; i++)
                {
                    header[i] = "Column" + i;
                }
            }

            ret = new DataSet();
            DataTable regTabell = null;
            bool headerHandeled = false;
            foreach (string info in source)
            {
                string[] array = info.Split(sep);
                if (firstIsHeader && !headerHandeled)//; && array.Equals(header))// skip header hvis første er kolonnenavn
                {
                    headerHandeled = true;
                    continue;
                }
                if (regTabell == null)
                {
                    regTabell = ret.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                    foreach (string prop in header)
                    {
                        regTabell.Columns.Add(prop);
                    }
                }
                DataRow row = regTabell.NewRow();
                int index = 0;
                foreach (string value in array)
                {
                    row[index] = value;
                    index++;
                }
                regTabell.Rows.Add(row);
            }
            return ret;
        }

        public static DataSet CSVToDataSet(FileInfo fullpath, string separator, bool firstIsHeader)
        {
            DataSet ret = null;
            string[] header;
            try
            {
                char[] sep = separator.ToCharArray();

                string[] source = File.ReadAllLines(fullpath.FullName, Encoding.GetEncoding("iso-8859-1"));
                if (firstIsHeader)
                {
                    header = source[0].Split(sep);
                }
                else
                {
                    header = new string[source[0].Split(sep).Length];
                    for (int i = 0; i < header.Length; i++)
                    {
                        header[i] = "Column" + i;
                    }
                }

                ret = new DataSet();
                DataTable regTabell = null;
                bool headerHandeled = false;
                foreach (string info in source)
                {
                    string[] array = info.Split(sep);
                    if (firstIsHeader && !headerHandeled)//; && array.Equals(header))// skip header hvis første er kolonnenavn
                    {
                        headerHandeled = true;
                        continue;
                    }
                    if (regTabell == null)
                    {
                        regTabell = ret.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                        foreach (string prop in header)
                        {
                            regTabell.Columns.Add(prop);
                        }
                    }
                    try
                    {

                        DataRow row = regTabell.NewRow();
                        int index = 0;
                        foreach (string value in array)
                        {
                            row[index] = value;
                            index++;
                        }
                        regTabell.Rows.Add(row);
                    }
                    catch (Exception ex)
                    { string text = ex.Message; }
                }
            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;
        }

        private static DataTable FixedPositionToDataTable(string text, int[] posArray)
        {
            //  string pos =  string.Join(",", Array.ConvertAll(posArray, x => x.ToString()));

            string s = "Pos" + string.Join(",Pos", Array.ConvertAll(posArray, x => x.ToString()));
            DataTable dt = new DataTable();
            dt.TableName = "FixedPosition";

            string[] tableData = s.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var col = from cl in tableData[0].Split(",".ToCharArray())
                      select new DataColumn(cl);

            dt.Columns.AddRange(col.ToArray());

            foreach (string item in text.Split(Environment.NewLine.ToArray()))
            {
                if (item.Equals(string.Empty))
                    continue;

                int position = 0;
                DataRow row = dt.NewRow();

                for (int i = 0; i < posArray.Length; i++)
                {
                    row[i] = item.Substring(position, posArray[i] - position).Trim();
                    position = posArray[i];
                }
                dt.Rows.Add(row);

            }


            //   (from st in tableData.Skip(1)
            //    select dt.Rows.Add(st.Split(",".ToCharArray()))).ToList();

            return dt;





        }

        public static XmlDocument DataSetToXML(DataSet ds)
        {
            XmlDocument ret = null;
            try
            {
                // This is the final document
                XmlDocument Data = new XmlDocument();

                // Create a string writer that will write the Xml to a string
                StringWriter stringWriter = new StringWriter();

                // The Xml Text writer acts as a bridge between the xml stream and the text stream
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

                // Now take the Dataset and extract the Xml from it, it will write to the string writer
                ds.WriteXml(xmlTextWriter, XmlWriteMode.IgnoreSchema);

                // Write the Xml out to a string
                string contentAsXmlString = stringWriter.ToString();

                // load the string of Xml into the document
                Data.LoadXml(contentAsXmlString);
                ret = Data;

            }
            catch (Exception) { }
            return ret;
        }
        public static string DataSetToCSV(DataTable table, string separator)
        {
            if (table.DataSet == null)
            {
                DataSet ds = new DataSet();
                ds.DataSetName = "DocumentElement";
                ds.Tables.Add(table);
                return DataSetToCSV(ds, separator);
            }
            else

                return DataSetToCSV(table.DataSet, separator);
        }
        public static string DataSetToCSV(DataSet ds, string separator)
        {
            string ret = string.Empty;
            string sep = separator;
            if (string.IsNullOrEmpty(sep))
                sep = Convert.ToChar(9).ToString(); //  sep = ";"; sep = 
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable table = ds.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow rad = table.Rows[i];
                    string[] array = Array.ConvertAll<object, string>(rad.ItemArray,
                                    delegate (object obj) { return obj.ToString(); });

                    ret += String.Join(sep, array) + Environment.NewLine;
                }


            }
            return ret;
        }

        /// <summary>
        /// Very simple DataTable to CSV converter, includes headerline, uses tab as delimiter
        /// </summary>
        /// <param name="table">DataTable to be converted</param>
        /// <returns></returns>
        public static StringBuilder DataTableToCSV(DataTable table)
        {
            StringBuilder sb = new StringBuilder();

            // var columnNames = table.Columns.Cast<DataColumn>().Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"").ToArray();
            var columnNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join("\t", columnNames));

            foreach (DataRow row in table.Rows)
            {
                var csvfields = row.ItemArray.Select(field => field.ToString()).ToArray();
                sb.AppendLine(string.Join("\t", csvfields));
            }
            return sb;
        }


        #endregion

        #region webrelated operations json, html
        /// <summary>
        /// Metode som tar inn en DataTable og konverter den til json array
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableTojson(DataTable dt)
        {      /****************************************************************************
             * Without goingin to the depth of the functioning of this Method, i will try to give an overview
             * As soon as this method gets a DataTable it starts to convert it into JSON String,
             * it takes each row and ineach row it creates an array of cells and in each cell is having its data
             * on the client side it is very usefull for direct binding of object to  TABLE.
             * Values Can be Access on clien in this way. OBJ.TABLE[0].ROW[0].CELL[0].DATA 
             * NOTE: One negative point. by this method user will not be able to call any cell by its name.
             * *************************************************************************/

            StringBuilder JsonString = new StringBuilder();
            JsonString.Append("{ ");
            JsonString.Append("\"TABLE\":[{ ");
            JsonString.Append("\"ROW\":[ ");

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                JsonString.Append("{ ");
                JsonString.Append("\"COL\":[ ");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j < dt.Columns.Count - 1)
                    {
                        JsonString.Append("{" + "\"DATA\":\"" + dt.Rows[i][j].ToString() + "\"},");
                    }
                    else if (j == dt.Columns.Count - 1)
                    {
                        JsonString.Append("{" + "\"DATA\":\"" + dt.Rows[i][j].ToString() + "\"}");
                    }
                }
                /*end Of String*/
                if (i == dt.Rows.Count - 1)
                {
                    JsonString.Append("]} ");
                }
                else
                {
                    JsonString.Append("]}, ");
                }
            }

            JsonString.Append("]}]}");
            return JsonString.ToString();
        }

        //http://www.west-wind.com/weblog/posts/471835.aspx

        public static string CreateJsonParameters(DataTable dt)
        {
            /* /****************************************************************************
             * Without goingin to the depth of the functioning of this Method, i will try to give an overview
             * As soon as this method gets a DataTable it starts to convert it into JSON String,
             * it takes each row and in each row it grabs the cell name and its data.
             * This kind of JSON is very usefull when developer have to have Column name of the .
             * Values Can be Access on clien in this way. OBJ.HEAD[0].<ColumnName>
             * NOTE: One negative point. by this method user will not be able to call any cell by its index.
             * *************************************************************************/
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        

            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"Head\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }

                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }

                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// Read unordered list into dataset
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>



        public static DataSet HTMLULToDataSet(string HTML)

        {
            DataSet ret = new DataSet("UnorderedLists");
            string ulExpression = "<ul[^>]*>(.*?)</ul>";
            string liExpression = "<li[^>]*>(.*?)</li>";
            Regex htmltagreg = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:" + @"\"".*?\""|'.*?'|[^'\"">\s]+))?)+\s*|\s*)/?>");
            MatchCollection unorderedLists = Regex.Matches(HTML, ulExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            int listCounter = 0;
            foreach (Match list in unorderedLists)
            {
                int itemCounter = 0;
                DataTable table = ColumnNamesToDataTable(new List<string>() { "Key", "Value" }).Tables[0];

                table.TableName = ($"Table{listCounter}");
                listCounter++;
                MatchCollection elements = Regex.Matches(list.Value, liExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                foreach (Match Row in elements)
                {
                    string value = string.Empty;
                    try
                    {
                        value = htmltagreg.Replace(Row.Groups[0].ToString(), "").Replace("&nbsp;", " ");
                        table.Rows.Add(itemCounter, value);
                        itemCounter++;
                    }
                    catch (Exception ex) { }
                }
                ret.Tables.Add(table.Copy());

            }
            return ret;
        }



        public static DataSet HTMLTablesToDataSet(string HTML)
        {
            // Declarations    
            DataSet ds = new DataSet();
            DataTable dt = null;
            DataRow dr = null;
            DataColumn dc = null;
            string TableExpression = "<table[^>]*>(.*?)</table>";
            string HeaderExpression = "<th[^>]*>(.*?)</th>";
            string RowExpression = "<tr[^>]*>(.*?)</tr>";
            string ColumnExpression = "<td[^>]*>(.*?)</td>";
            bool HeadersExist = false;
            int iCurrentColumn = 0;
            int iCurrentRow = 0;

            Regex htmltagreg = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:" + @"\"".*?\""|'.*?'|[^'\"">\s]+))?)+\s*|\s*)/?>");

            // Get a match for all the tables in the HTML    
            MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Loop through each table element    
            foreach (Match Table in Tables)
            {

                // Reset the current row counter and the header flag    
                iCurrentRow = 0;
                HeadersExist = false;

                // Add a new table to the DataSet    
                dt = new DataTable();

                // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)    
                if (Table.Value.Contains("<th"))
                {
                    // Set the HeadersExist flag    
                    HeadersExist = true;

                    // Get a match for all the rows in the table    
                    MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Loop through each header element    
                    foreach (Match Header in Headers)
                    {
                        //dt.Columns.Add(Header.Groups[0].ToString());
                        dt.Columns.Add(htmltagreg.Replace(Header.Groups[0].ToString(), string.Empty));
                    }
                }
                else
                {
                    for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                    {
                        dt.Columns.Add("Column " + iColumns);
                    }
                }

                // Get a match for all the rows in the table    
                MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each row element    
                foreach (Match Row in Rows)
                {

                    // Only loop through the row if it isn't a header row    
                    if (!(iCurrentRow == 0 & HeadersExist == true))
                    {

                        // Create a new row and reset the current column counter    
                        dr = dt.NewRow();
                        iCurrentColumn = 0;

                        // Get a match for all the columns in the row    
                        MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);


                        // Loop through each column element    
                        foreach (Match Column in Columns)
                        {

                            // Add the value to the DataRow    
                            // dr[iCurrentColumn] = Column.Groups[0].ToString();                            
                            try
                            {
                                dr[iCurrentColumn] = htmltagreg.Replace(Column.Groups[0].ToString(), "").Replace("&nbsp;", " ");
                            }
                            catch (Exception)
                            {
                                //  dr[iCurrentColumn] = DBNull.Value;
                            }
                            // Increase the current column     
                            iCurrentColumn += 1;
                        }

                        // Add the DataRow to the DataTable    
                        dt.Rows.Add(dr);

                    }

                    // Increase the current row counter    
                    iCurrentRow += 1;
                }

                // Add the DataTable to the DataSet    
                ds.Tables.Add(dt);

            }

            return ds;

        }
        #endregion

        #region get column names

        /// <summary>
        /// Get the columnnames from a datasets table
        /// </summary>
        /// <param name="ds">dataset</param>
        /// <param name="tableName">table</param>
        /// <returns>string array of columnnames</returns>
        public static string[] ColumnNames(DataSet ds, string tableName)
        {
            DataTable table = ds.Tables[tableName];
            return ColumnNames(table);
        }
        /// <summary>
        /// Get the columnames from a datatable
        /// </summary>
        /// <param name="table">table</param>
        /// <returns>string array of columnnames</returns>
        public static string[] ColumnNames(DataTable table)
        {
            string[] ret = new string[table.Columns.Count];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = table.Columns[i].ColumnName;
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// Metode som bygger opp en tekst som kan benyttes til å lage klasser ut fra en datatabell
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToEntityClass(DataTable table)
        {
            try
            {
                StringBuilder ret = new StringBuilder();
                ret.Append(string.Format(@"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;

                namespace Kolibri.Entities
                {{
                    public class {0}
                    {{                                    ", table.TableName) + Environment.NewLine);
                ret.Append("#region private members" + Environment.NewLine);
                foreach (DataColumn col in table.Columns)
                {
                    ret.Append(string.Format("private {0} m_{1};", col.DataType, col.ColumnName.ToLower()) + Environment.NewLine);
                }
                ret.Append("#endregion" + Environment.NewLine);

                ret.Append("#region public properties" + Environment.NewLine);
                foreach (DataColumn col in table.Columns)
                {
                    ret.Append(string.Format(@"public {0} {1}
		            {{
				        get {{ return m_{2}; }}
				        set {{ m_{2} = value; }}
		            }}", col.DataType, StringUtilities.FirstToUpper(col.ColumnName), col.ColumnName.ToLower()) + Environment.NewLine);

                }

                ret.Append("#endregion" + Environment.NewLine);
                ret.Append("}}");

                return ret.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Samme som autogenerer dataset 
        /// </summary>
        /// <returns></returns>
        public static DataSet AutoGenererTypedDataSet(ArrayList arrayList)
        {
            DataSet ret = null;
            DataSet ds = new DataSet();
            DataTable regTabell = null;
            try
            {
                if (arrayList != null)
                {

                    foreach (Object info in arrayList)
                    {
                        Type objectType = info.GetType();
                        PropertyInfo[] properties = objectType.GetProperties();
                        if (regTabell == null)
                        {
                            regTabell = ds.Tables.Add("Table1");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);
                            foreach (PropertyInfo prop in properties)
                            {
                                regTabell.Columns.Add(prop.Name, prop.PropertyType);
                            }
                        }
                        DataRow row = regTabell.NewRow();
                        foreach (PropertyInfo prop in properties)
                        {
                            row[prop.Name] = prop.GetValue(info, null);
                        }
                        regTabell.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            ret = ds;
            return ret;
        }


        /// <summary>    
        ///    string filterExp = "Status = 'Active'";
        ///    string sortExp = "City";
        /// http://msdn.microsoft.com/en-us/library/zk13kdh0%28VS.71%29.aspx
        /// </summary>
        /// <returns></returns>
        //public static DataSet Sort(filter,sort)
        //{
        //    DataSet dataSet1 = null;


        //    DataRow[] drarray = null;
        //    //drarray = dataSet1.Customers.Select(filter, sort, DataViewRowState.CurrentRows);
        //    for (int i = 0; i < drarray.Length; i++)
        //    {
        //        //listBox1.Items.Add(drarray[i][sort].ToString());
        //    }
        //    return null;
        //}

        //public static DataSet AutoGenererDataSet(Object obj)
        //{
        //    DataSet ret = null;
        //    if (obj != null)
        //    {
        //        ArrayList liste = new ArrayList();
        //        liste.Add(obj);
        //        ret = AutoGenererDataSet(liste);
        //    }
        //    return ret;
        //}




        //http://www.west-wind.com/weblog/posts/471835.aspx  


        #region Create CS Classes


        public static string DataTableToCode(DataTable Table)
        {
            string className = Table.TableName;
            if (string.IsNullOrWhiteSpace(className))
            {   // Default name
                className = "Unnamed";
            }


            // Create the class
            CodeTypeDeclaration codeClass = CreateClass(className);

            // Add public properties
            foreach (DataColumn column in Table.Columns)
            {
                codeClass.Members.Add(CreateProperty(column.ColumnName, column.DataType));
            }

            // Add Class to Namespace
            string namespaceName = $"Dapper.Models.{Table.TableName}";
            CodeNamespace codeNamespace = new CodeNamespace(namespaceName);
            codeNamespace.Types.Add(codeClass);

            // Generate code
            string filename = string.Format("{0}.{1}.cs", namespaceName, className);
            return CreateCodeFile(filename, codeNamespace);

            // Return filename
            return filename;
        }

        private static CodeTypeDeclaration CreateClass(string name)
        {
            CodeTypeDeclaration result = new CodeTypeDeclaration(name);
            result.Attributes = MemberAttributes.Public;
            result.Members.Add(CreateConstructor(name)); // Add class constructor
            return result;
        }

        private static CodeConstructor CreateConstructor(string className)
        {
            CodeConstructor result = new CodeConstructor();
            result.Attributes = MemberAttributes.Public;
            result.Name = className;
            return result;
        }

        private static CodeMemberField CreateProperty(string name, Type type)
        {
            // This is a little hack. Since you cant create auto properties in CodeDOM,
            //  we make the getter and setter part of the member name.
            // This leaves behind a trailing semicolon that we comment out.
            //  Later, we remove the commented out semicolons.
            string memberName = name + "\t{ get; set; }//";

            CodeMemberField result = new CodeMemberField(type, memberName);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }

        private static string CreateCodeFile(string filename, CodeNamespace codeNamespace)
        {
            // CodeGeneratorOptions so the output is clean and easy to read
            CodeGeneratorOptions codeOptions = new CodeGeneratorOptions();
            codeOptions.BlankLinesBetweenMembers = false;
            codeOptions.VerbatimOrder = true;
            codeOptions.BracingStyle = "C";
            codeOptions.IndentString = "\t";

            string ret = string.Empty;

            // Create the code file
            using (TextWriter textWriter = new StringWriter())
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                codeProvider.GenerateCodeFromNamespace(codeNamespace, textWriter, codeOptions);
                ret = textWriter.ToString().Replace("//;", "");
            }
            return ret;
            // Correct our little auto-property 'hack'
            File.WriteAllText(filename, File.ReadAllText(filename).Replace("//;", ""));
        }

        #endregion
    }
}