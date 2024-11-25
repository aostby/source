using System;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace Kolibri.Common.Utilities
{

    //ConnectionString:       Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\myFolder\myAccess2007file.accdb;Jet OLEDB:Database Password=MyDbPassword;
    public class AccessUtilities
    {
        public static string m_password;

        //ressurs : // http://www.codeproject.com/KB/miscctrl/Excel_data_access.aspx#create
        public static DataTable GetOleDbSchemaTable(string fullfilsti)
        {
            DataTable dtSchema = null;
            string connectionstring = GetAccessConnectionString(fullfilsti, m_password);

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
        public static bool CreateTable(string fullFilsti, DataTable table)
        {
            bool ret = true;
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = GetAccessConnectionString(fullFilsti, m_password);
            RunSql(con, string.Format("DROP TABLE {0}", table.TableName));
            return RunSql(con, "CREATE TABLE aaTable1 (id counter primary key, theDate DateTime, num decimal(18, 4))");
        }

        public static bool RunSql(OleDbConnection con, String sqlString)
        {
            bool ret = true;
            try
            {
                OleDbCommand com = new OleDbCommand(sqlString, con);
                com.Connection.Open();
                try
                {
                    com.ExecuteNonQuery();
                }
                catch
                {
                    // Always dies here with the error message above. 
                }
                finally
                {
                    com.Connection.Close();
                }
            }
            catch (System.Exception)
            {

                ret = false;
            }
            return ret;
        }

        


        /// <summary>
        /// Metode som returnerer en connectionstring for access basert på filnavnet
        /// </summary>
        /// <param name="fullfilsti"></param>
        /// <returns></returns>
        public static string GetAccessConnectionString(string filename, string password)
        {

            string ext = System.IO.Path.GetExtension(filename);
            if (string.IsNullOrEmpty(password))
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", filename);
            }
            else
            {
                return string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1};", filename, password);
            }
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
            using (OleDbConnection connection = new OleDbConnection(GetAccessConnectionString(m_filepath, m_filepath)))
            {
                foreach (DataRow item in table.Rows)
                {
                    connection.Open();
                    string kolonner = string.Join(", ", array);
                    string verdier = "@" + string.Join(",@", array);
                    string query = "INSERT INTO [" + table.TableName +  /*Sheet1*/"$](" + kolonner + ")  VALUES  (" + verdier + ")";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
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
                string sql = "Drop Table [" + sheetname + "]";


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


        // Creates Create Table Statement and runs it.
        public static bool WriteTable(string filename, DataTable table)
        {
            bool ret;
            string connectionString = ExcelUtilities.GetExcelConnectionString(filename);
            OleDbConnection conObj = new OleDbConnection(connectionString);

            try
            {
                string sql = GenerateCreateTable(table);
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

            if (ret)
            {
                //InjectWorkbook(filename, table);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    ret = ret && AddNewRow(table.Rows[i], conObj);
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
 
        /// <summary>
        /// http://support.microsoft.com/kb/325938
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="FieldList"></param>
        /// <returns></returns>
        private static  DataTable CreateTable(string TableName, string FieldList)
        {	
            DataSet ds = null;
	        DataTable dt = new DataTable(TableName);
	        DataColumn dc;
	        string[] Fields= FieldList.Split(',');    
	        string[] FieldsParts;
	        string Expression;
	        foreach(string Field in Fields)
	        {
		        FieldsParts = Field.Trim().Split(" ".ToCharArray(), 3); // allow for spaces in the expression
		        // add fieldname and datatype			
		        if (FieldsParts.Length == 2)
		        {	
			        dc = dt.Columns.Add(FieldsParts[0].Trim(), Type.GetType("System." + FieldsParts[1].Trim(),true,true));
			        dc.AllowDBNull = true;
		        }
		        else if (FieldsParts.Length == 3)  // add fieldname, datatype, and expression
		        {
			        Expression = FieldsParts[2].Trim();
			        if (Expression.ToUpper() == "REQUIRED")
			        {				
				        dc = dt.Columns.Add(FieldsParts[0].Trim(), Type.GetType("System." + FieldsParts[1].Trim(), true, true));
				        dc.AllowDBNull = false;
			        }
			        else
			        {
				        dc = dt.Columns.Add(FieldsParts[0].Trim(), Type.GetType("System." + FieldsParts[1].Trim(), true, true), Expression);
			        }
		        }
		        else
		        {
			        throw new ArgumentException("Invalid field definition: '" + Field + "'.");
		        }
	        }
	        if (ds != null) 
	        {
	                ds.Tables.Add(dt);
	        }
	        return dt;
       }

        // Create Table Generation based on Table Defination
        private static string GenerateCreateTable(DataTable table)
        {
            return null;
            StringBuilder sb = new StringBuilder();
            bool firstcol = true;
            sb.AppendFormat("CREATE TABLE [{0}](", table.TableName);



            int teller = 0;
            string[] arrayNames = null;
            //arrayNames= (from DataColumn x in table.Columns select x.ColumnName).ToArray();

            foreach (DataRow row in table.Rows)
            {
                //verdier
                string[] array = Array.ConvertAll<object, string>(table.Rows[teller].ItemArray,
                                      delegate(object obj) { return obj.ToString(); });
                string[] Fields = arrayNames;//.Split(',');    
                string[] FieldsParts;

                foreach (object item in row.ItemArray)
                {
                }
            }
        }
	

            /*
         //
	// Get the first row and loop over its ItemArray.
	//
	DataTable table = GetTable();
	DataRow row = table.Rows[0];
	foreach (object item in row.ItemArray)
	{
	    if (item is int)
	    {
		Console.WriteLine("Int: {0}", item);
	    }
	    else if (item is string)
	    {
		Console.WriteLine("String: {0}", item);
	    }
	    else if (item is DateTime)
	    {
		Console.WriteLine("DateTime: {0}", item);
	    }
	}

         */

            /*//
        // Loop over DataTable rows and call the Field extension method.
        //
        foreach (DataRow row in GetTable().Rows)
        {
            // Get first field by column index.
            int weight = row.Field<int>(0);

            // Get second field by column name.
            string name = row.Field<string>("Name");

            // Get third field by column index.
            string code = row.Field<string>(2);

            // Get fourth field by column name.
            DateTime date = row.Field<DateTime>("Date");

            // Display the fields.
            Console.WriteLine("{0} {1} {2} {3}", weight, name, code, date);
        }
    */



          /*  firstcol = true;
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
           * */
     //       return sb.ToString();
    


        //Generates InsertStatement from a DataRow.
        private 
  
            static string GenerateInsertStatement(DataRow dr)
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
                string value = (dr[i] == null) ? string.Empty : dr[i].ToString();
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

    }
}


