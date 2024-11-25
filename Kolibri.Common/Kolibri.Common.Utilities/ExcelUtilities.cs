using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Kolibri.Common.Utilities
{
    public class ExcelUtilities
    {
        //ressurs : // http://www.codeproject.com/KB/miscctrl/Excel_data_access.aspx#create

        /// <summary>
        /// Method that locates Excel in registry, finds filepath and starts excel with filename as parameter
        /// </summary>
        /// <param name="filename"></param>
        public static void OpenFileInExcel(string filename)
        {
            string path = "";
            RegistryKey key = Registry.LocalMachine;
            RegistryKey excelKey = key.OpenSubKey(@"SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\APP PATHS\EXCEL.EXE");

            if (excelKey != null)
            {
                path = excelKey.GetValue("").ToString();
            }
            if (path != "")
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = path;
                startInfo.Arguments = "\"" + filename + "\"";
                startInfo.UseShellExecute = false;

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    try
                    {
                        process.Start();
                    }
                    catch (Exception ex)
                    { }
                }
            }
            else
            {
                try
                {
                    OpenInExcelProgramFiles(filename);
                }
                catch (System.Exception)
                {
                    Logger.Logg(Logger.LoggType.Feil, "Can't Open in excel because excel is not installed.");
                }
            }
        }
        private static void OpenInExcelProgramFiles(string filename)
        {
            try
            {
                string prog32 = Environment.GetEnvironmentVariable("ProgramFiles");
                string prog64 = Environment.GetEnvironmentVariable("ProgramW6432");

                string excel = string.Empty;
                for (int i = 10; i < 22; i++)
                {
                    string executablePath = string.Format(@"Microsoft Office\Office{0}\excel.exe", i);
                    FileInfo info = new FileInfo(Path.Combine(prog64, executablePath));
                    if (info.Exists)
                    {
                        excel = info.FullName;
                        break;
                    }
                    else
                    {
                        info = new FileInfo(Path.Combine(prog32, executablePath));
                        if (info.Exists)
                        {
                            excel = info.FullName;
                            break;
                        }
                    }
                }
                // UNDONE: don't hardcode this path unless it is truly machine independent. Instead use environment variables to construct the path similar to above.

                string xlsFile = string.Format("\"{0}\"", filename);

                ProcessStartInfo startInfo = new ProcessStartInfo(excel, xlsFile);

                Process.Start(startInfo);

            }
            catch (System.Exception ex)
            { }
        }

        public static void GenerateExcel2007(FileInfo p_strPath, DataSet p_dsSrc)
        {
            using (ExcelPackage objExcelPackage = new ExcelPackage())
            {
                foreach (DataTable dtSrc in p_dsSrc.Tables)
                {
                    //Create the worksheet    
                    ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add(dtSrc.TableName);
                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1    
                    objWorksheet.Cells["A1"].LoadFromDataTable(dtSrc, true);
                    //objWorksheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                    objWorksheet.Cells.AutoFitColumns();
                    //Format the header     
                    using (var objRange = objWorksheet.Cells[1, 1, 1, dtSrc.Columns.Count])
                    {
                        objRange.Style.Font.Bold = true;
                        objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        objRange.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                        objRange.Style.Font.Color.SetColor(Color.Black);
                    }
                    if (dtSrc.Columns[0].ColumnName.Equals("Title", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var objRange = objWorksheet.Cells[2, 1, dtSrc.Rows.Count, 1])
                        {
                            objRange.Style.Font.Bold = true;
                        }
                    }

                    try
                    {
                        //if (p_dsSrc.Tables[0].Columns.Contains("Image")&&false) {

                        //    int rowIndex = 0;int cellIndex = 0;
                        //        Image img = p_dsSrc.Tables[0].Rows[rowIndex][cellIndex] as Image;

                        //    ExcelPicture pic = objWorksheet.Drawings.AddPicture("Sample", img);
                        //    pic.SetPosition(rowIndex, 0, cellIndex, 0);
                        //    //pic.SetPosition(PixelTop, PixelLeft);  
                        //    pic.SetSize(10, 10);
                        //    //pic.SetSize(40);  
                        //    objWorksheet.Protection.IsProtected = false;
                        //    objWorksheet.Protection.AllowSelectLockedCells = false;

                        //}  
                        ////https://stackoverflow.com/questions/11588704/adding-images-into-excel-using-epplus   og https://stackoverflow.com/questions/15634709/epplus-setposition-picture-issue

                        // foreach (ExcelPicture drawing in objWorksheet.Drawings)
                        // { drawing.SetSize(10); }

                    }
                    catch (Exception)
                    { }
                }


                try
                {
                    //Write it back to the client    
                    if (p_strPath.Exists) p_strPath.Delete();

                    //Create excel file on physical disk    
                    FileStream objFileStrm = File.Create(p_strPath.FullName);
                    objFileStrm.Close();

                    //Write content to excel file
                    File.WriteAllBytes(p_strPath.FullName, objExcelPackage.GetAsByteArray());
                }
                catch (Exception)
                {
                    FileInfo newInfo = new FileInfo(FileUtilities.AutomaticFilenameIfExists(p_strPath.FullName));

                    //Write it back to the client    
                    if (newInfo.Exists) p_strPath.Delete();

                    //Create excel file on physical disk    
                    FileStream objFileStrm = File.Create(newInfo.FullName);
                    objFileStrm.Close();

                    //Write content to excel file
                    File.WriteAllBytes(newInfo.FullName, objExcelPackage.GetAsByteArray());
                }

            }
        }

        /// <summary>
        /// Hent et ark fra Excel som dataset
        /// </summary>
        /// <param name="fullFilsti"></param>
        /// <param name="arknavn"></param>
        /// <returns></returns>
        public static DataSet HentSomDataSet(string fullFilsti, string arknavn)
        {
            DataSet ret = null;
            try
            {
                ret = HentExcelSomDataset(fullFilsti, arknavn);

            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;
        }

        private static DataSet HentExcelSomDataset(string fullFilsti, string sheetname)
        {
            //http://bytes.com/groups/net-c/517696-reading-excel-sheet-into-db-table
            string strConn;
            strConn = GetExcelConnectionString(fullFilsti); //"Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fullFilsti + ";" + "Extended Properties=Excel 8.0;"; //You must use the $ after the object you reference in the spreadsheet
            System.Data.OleDb.OleDbConnection myCon = new System.Data.OleDb.OleDbConnection(strConn);
            myCon.Open();
            System.Data.OleDb.OleDbDataAdapter myCommand = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + sheetname + "]", myCon);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, FileUtilities.SafeFileName(sheetname));// "Deltakere");
            myCon.Close();
            return myDataSet;
        }

        /// <summary>
        /// This mehtod retrieves the excel sheet names from 
        /// an excel workbook.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <returns>String[]</returns>
        public static String[] GetExcelSheetNames(string fullfilsti)
        {
            //http://forums.asp.net/t/1088515.aspx
            string excelFile = fullfilsti;
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            try
            {
                string connString = GetExcelConnectionString(fullfilsti);

                // connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFile + ";Extended Properties=Excel 10.0;";
                // Create connection object by using the preceding connection string.
                objConn = new OleDbConnection(connString);
                // Open connection with the database.
                objConn.Open();
                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Loop through all of the sheets if you want too...
                for (int j = 0; j < excelSheets.Length; j++)
                {
                    // Query each excel sheet.
                }
                return excelSheets;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Clean up.
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        public static DataTable GetOleDbSchemaTable(string fullfilsti)
        {
            DataTable dtSchema = null;
            string connectionstring = GetExcelConnectionString(fullfilsti);

            using (OleDbConnection conObj = new OleDbConnection(connectionstring))
            {

                if (conObj.State != ConnectionState.Open)
                    conObj.Open();
                dtSchema = conObj.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                if (conObj.State == ConnectionState.Open)
                    conObj.Close();

                conObj.Dispose();

            }
            return dtSchema;
        }


        /// <summary>
        /// Metode som returnerer en connectionstring for excel basert på filnavnet
        /// </summary>
        /// <param name="fullfilsti"></param>
        /// <returns></returns>
        public static string GetExcelConnectionString(string filename)
        {
            // Connection String. Change the excel file to the file you           // will search.
            //String ret;

            //if (new FileInfo(filename).Extension.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))              //hvis  xlsx fil 
            //    ret = string.Format(@"Data Source={0};Provider=Microsoft.ACE.OLEDB.12.0; Extended Properties=Excel 12.0;", filename);
            //else                //hvis xls fil -- må testes
            //    ret = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", filename);

            string ext = System.IO.Path.GetExtension(filename);
            // Support for Excel 2007 XML format   
            if (ext.ToLower() == ".xlsx")
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", filename);
            // Support for Excel 2007 binary format          
            if (ext.ToLower() == ".xlsb")
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\";", filename);
            // Support for Excel 2007 binary format 
            if (ext.ToLower() == ".xlsm")
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Macro;HDR=YES\";", filename);
            // Basic support for Excel 97 through 2003.     
            return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES\";", filename);


            //return ret;



        }
             

        /// <summary>
        /// Ikje verskja
        /// </summary>
        /// <param name="m_filepath"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool InjectWorkbook(string m_filepath, DataTable table)
        {
            //array  = Array.ConvertAll<object, string>(array, delegate(object obj) { return obj.ToString(); });
            //http://stackoverflow.com/questions/3283204/convert-datatable-to-excel-2007-xlsx

            bool ret = true;

            DataColumnCollection kolonneHeadere = table.Columns;
             string[] array = new string[kolonneHeadere.Count];
            for (int i = 0; i < array.Length; i++)
			{
			 array[i] = kolonneHeadere[i].ColumnName;
			}

           
            using (OleDbConnection connection = new     OleDbConnection(GetExcelConnectionString(m_filepath)))
            {
                foreach (DataRow item in table.Rows)
                {
                    connection.Open();
                    //  string query = "INSERT INTO [Sheet1$] (Column1, Column2, Column3, Column4, Column5) VALUES  (@Column1, @Column2, @Column3, 
                    string kolonner = string.Join(", ", array);
                    string verdier = "@" + string.Join(",@", array);
                    string query = "INSERT INTO [" + table.TableName +  /*Sheet1*/"$](" + kolonner + ")  VALUES  (" + verdier + ")";
                    query = "INSERT INTO [Table1$](tall) Values ('2')";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                     //   command.Parameters.AddRange(item.ItemArray);
                        /*command.Parameters.AddWithValue("@Column1", item["Column1"].ToString());
                        command.Parameters.AddWithValue("@Column2", item["Column2"].ToString());
                        command.Parameters.AddWithValue("@Column3", item["Column3"].ToString());
                        command.Parameters.AddWithValue("@Column4", item["Column4"].ToString());
                        command.Parameters.AddWithValue("@Column5", item["Column5"].ToString());*/
            //            for (int i = 0; i < array.Length; i++)
            //{
            //                object value =  (item.ItemArray[i] == null)? string.Empty: item.ItemArray[i];
            //                command.Parameters.AddWithValue("@"+array[i],value.ToString());
            //}
                        

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            return ret;
        }

                // Creates Create Table Statement and runs it.
        public static bool DeleteTable(string filename, string sheetname)
        {
            bool ret;
            string connectionString = ExcelUtilities.GetExcelConnectionString(filename);
            OleDbConnection conObj = new OleDbConnection(connectionString);

            try
            {
                string sql = "Drop Table ["+sheetname+"]";


                using (OleDbCommand cmd = new OleDbCommand(sql, conObj))
                {
                    if (conObj.State != ConnectionState.Open)
                        conObj.Open();
                    cmd.ExecuteNonQuery();

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                ret = false;
            }

           

            if (conObj.State == ConnectionState.Open)
                conObj.Close();

            return ret;
        }

        public static string LegalSheetName(string name)
        {


            string ret = name;
            if (string.IsNullOrEmpty(name))
            {
                ret = "Table1";
                return ret;
            }
            ret = StringUtilities.FirstToUpper(ret);
            ret = StringUtilities.FirstToUpperCamelCasing(ret).Replace(" ", "");
            if (ret.Length > 31) { ret = ret.Substring(0, 30); }
            return ret;
        }



        // Creates Create Table Statement and runs it.
        public static  bool WriteTable(string filename, DataTable table)
        {
            bool ret;
            string connectionString = ExcelUtilities.GetExcelConnectionString(filename);
            OleDbConnection conObj = new OleDbConnection(connectionString);

            try
            {
                string sql = GenerateCreateTable(table);
                using (OleDbCommand cmd = new OleDbCommand(sql ,conObj))
                {
                    if (conObj.State != ConnectionState.Open)
                        conObj.Open();
                    cmd.ExecuteNonQuery();
                  
                    ret = true;
                }
            }
            catch(Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message);
                ret= false;
            }

            if (ret)
            {
                //InjectWorkbook(filename, table);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                 ret = ret &&   AddNewRow(table.Rows[i], conObj);
                }
            
            }

            if (conObj.State == ConnectionState.Open)
                conObj.Close();

            return ret;

        }
    
        // Generates Insert Statement and executes it
        private static bool AddNewRow(DataRow dr, OleDbConnection conObj)
        {

            try
            {

                using (OleDbCommand cmd = new OleDbCommand(
                              GenerateInsertStatement(dr), conObj))
                {
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        
        // Create Table Generation based on Table Defination
        private static string GenerateCreateTable(DataTable table)
        {

            StringBuilder sb = new StringBuilder();
            bool firstcol = true;
            sb.AppendFormat("CREATE TABLE [{0}](", table.TableName);
            firstcol = true;
            for (int i = 0; i < table.Columns.Count; i++)
            {

                if (!firstcol)
                {
                    sb.Append(",");
                }
                firstcol = false;

                string colType = "TEXT";
                //colType = GetExcelType(table.Columns[i]);

                sb.AppendFormat("{0} {1}", table.Columns[i].ColumnName.Replace("$", ""), colType);// keyvalue.Key, keyvalue.Value);
            }

            sb.Append(")");
            return sb.ToString();
        }

        //Generates InsertStatement from a DataRow.
        private static string GenerateInsertStatement(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();
            bool firstcol = true;
            sb.AppendFormat("INSERT INTO [{0}](", dr.Table.TableName);


            foreach (DataColumn dc in dr.Table.Columns)
            {
                if (!firstcol)
                {
                    sb.Append(",");
                }
                firstcol = false;

                sb.Append(dc.ColumnName);//.Caption);
            }

            sb.Append(") VALUES(");
            firstcol = true;
            for (int i = 0; i <= dr.Table.Columns.Count - 1; i++)
            {
                //Dette kan effektiviseres med array og join....
                string  value = (dr[i] == null) ? string.Empty : dr[i].ToString();
                value = string.Format("'{0}'", value.Replace("'", ""));

                sb.Append(value);
                //}
                if (i != dr.Table.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append(")");
            return sb.ToString();
        }


        private static string GetExcelType(DataColumn column)
        {

            /*
            CURRENCY    SQL_NUMERIC
            DATETIME    SQL_TIMESTAMP
            LOGICAL     SQL_BIT
            NUMBER      SQL_DOUBLE
            TEXT        SQL_VARCHAR*/


            string ret = "TEXT";
            string jalla = GetCsType(column);
            switch (jalla)
            {
                case "string":
                case "Guid":
                    ret = "TEXT";
                    break;
                case "byte":
                case "short":
                case "long":
                case "int":
                case "float":
                case "decimal":
                    ret = "NUMBER";
                    break;
                case "byte[]":
                
                    ret = "TEXT";
                    break;

                case "DateTime":
                    ret = "DATETIME";
                    break;

                case "bool":
                    ret = "LOGICAL";
                    break;

                default:
                    ret = "TEXT";
                    break;
            }

            return ret;
        }

        private static string GetCsType(DataColumn column)
        {
                //switch (column.DataType.ToString().ToLower())
                
            switch(column.DataType.ToString().ToLower())
            {
                        case "binary":
                                return "byte[]";
                        case "bigint":
                                return "long";
                        case "bit":
                                return "bool";
                        case "char":
                                return "string";
                        case "datetime":
                                return "DateTime";
                        case "decimal":
                                return "decimal";
                        case "float":
                                return "float";
                        case "image":
                                return "byte[]";
                        case "int":
                        //case "system.int16":
                        //case "system.int32":
                                return "int";
                        case "money":
                                return "decimal";
                        case "nchar":
                                return "string";
                        case "ntext":
                                return "string";
                        case "nvarchar":
                                return "string";
                        case "numeric":
                                return "decimal";
                        case "real":
                                return "decimal";
                        case "smalldatetime":
                                return "DateTime";
                        case "smallint":
                                return "short";
                        case "smallmoney":
                                return "float";
                        case "sql_variant":
                                return "byte[]";
                        case "sysname":
                                return "string";
                        case "text":
                                return "string";
                        case "timestamp":
                                return "DateTime";
                        case "tinyint":
                                return "byte";
                        case "varbinary":
                                return "byte[]";
                        case "varchar":
                                return "string";
                        case "uniqueidentifier":
                                return "Guid";
                        default:  // Unknow data type
                    //forsøk noe annet
                           return DataTypeConvertUtilities.FromDotNetTypeToConvertToDotNetTypeCode(column.DataType.ToString());
                          
                     //           throw (new Exception("Invalid SQL Server data type specified: " + column.DataType.ToString()));
                }
        }

    }
}

