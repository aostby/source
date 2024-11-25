using System;
using System.Data;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
//using System.Data.OracleClient;

namespace Kolibri.Data.Connection
{
    /// <summary>
    /// DbKobling
    /// </summary>
    public class DbAccess
    { 
        #region Public Methods

        /// <summary>
        /// Dersom eksternt kall, benytt heller GetDbConnection();
        /// Hent en connection basert på innparametere. benytt gjerne eksisterende connectionstring fra ett/samme connection objekt.
        /// </summary>
        /// <param name="connectionString">oppkoblingsstreng for databasen</param>
        /// <param name="dbConnectionType">type oppkobling. Dersom ODBC, skriv dette istedet for DbType</param>
        /// <returns>connection objekt av angitt type hvis mulig.</returns>
        internal IDbConnection GetDbConnection(string connectionString, string dbConnectionType)
        {
            IDbConnection dbConnection = null;
            string tempContype = dbConnectionType.ToUpper();

            switch (tempContype)
            {
                case "SQL":
                    dbConnection = new SqlConnection(connectionString);
                    break;
                case "OLEDB":
                    dbConnection = new OleDbConnection(connectionString);
                    break;
                case "ODBC":
                    dbConnection = new OdbcConnection(connectionString);
                    break;
                case "ORACLE":
                    throw new PlatformNotSupportedException(DbConnection.DBType.ORACLE.ToString());
                   // dbConnection = new OracleConnection(connectionString);
                    break;
                case "MYSQL":
                    throw new PlatformNotSupportedException(DbConnection.DBType.MYSQL.ToString());
                //    dbConnection = new MySqlConnection(connectionString);
                    break;
                case "SYBASE":
                    throw new PlatformNotSupportedException(DbConnection.DBType.SYBASE.ToString() + " er i denne versjonen kun støttet som dbConnectionType 'ODBC'.");
                    break;
                //case "ULTRALITE":
                //    dbConnection = new iAnywhere.Data.UltraLite.ULConnection (connectionString);
                //    break;    
                default:
                    throw new PlatformNotSupportedException(DbConnection.DBType.UNKNOWN.ToString());
                    break;
            }
            return dbConnection;
        }

        internal IDbCommand GetDbCommand(string dbConnectionType)
        {
            IDbCommand dbCommand = null;

            switch (dbConnectionType)
            {
                case "SQL":
                    dbCommand = new SqlCommand();
                    break;
                case "OLEDB":
                    dbCommand = new OleDbCommand();
                    break;
                case "ODBC":
                    dbCommand = new OdbcCommand();
                    break;
                case "ORACLE":
                 //   dbCommand = new OracleCommand();
                    break;
                //case "UltraLite":
                //    dbCommand = new iAnywhere.Data.UltraLite.ULCommand();// MySqlCommand();

                    break;
                //case "MYSQL":
                //    dbCommand = new MySqlCommand();
                //    break;
                default:
                    break;
            }
            return dbCommand;
        }

        internal IDbDataParameter GetDbDataParameter(string dbConnectionType)
        {
            IDbDataParameter dbDataParameter = null;

            switch (dbConnectionType)
            {
                case "SQL":
                    dbDataParameter = new SqlParameter();
                    break;
                case "OLEDB":
                    dbDataParameter = new OleDbParameter();
                    break;
                case "ODBC":
                    dbDataParameter = new OdbcParameter();
                    break;
                case "ORACLE":
                  //  dbDataParameter = new OracleParameter();
                    break;
                default:
                    break;
            }
            return dbDataParameter;
        }

        internal IDataReader GetDataReader(IDbCommand command, string dbConnectionType)
        {
            IDataReader dataReader = null;

            switch (dbConnectionType)
            {
                case "SQL":
                    SqlCommand sqlCommand = (SqlCommand)command;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    dataReader = (SqlDataReader)sqlDataReader;
                    break;
                case "OLEDB":
                    OleDbCommand oleDbCommand = (OleDbCommand)command;
                    OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
                    dataReader = (OleDbDataReader)oleDbDataReader;
                    break;
                case "ODBC":
                    OdbcCommand odbcCommand = (OdbcCommand)command;
                    OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
                    dataReader = (OdbcDataReader)odbcDataReader;
                    break;
                //case "UltraLite":
                //    ULCommand ULCommand = (iAnywhere.Data.UltraLite.ULCommand)command;
                //    ULDataReader ULDataReader = ULCommand.ExecuteReader();
                //    dataReader = (iAnywhere.Data.UltraLite.ULDataReader)ULDataReader;
                //    break;
                case "ORACLE":
                    //OracleCommand oracleCommand = (System.Data.OracleClient.OracleCommand)command;
                    //OracleDataReader oracleDataReader = oracleCommand.ExecuteReader();
                    //dataReader = (System.Data.OracleClient.OracleDataReader)oracleDataReader;
                    break;
                //case "MYSQL":
                //    MySqlCommand mySqlCommand = (MySql.Data.MySqlClient.MySqlCommand)command;
                //    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                //    dataReader = (MySql.Data.MySqlClient.MySqlDataReader)mySqlDataReader;
                //    break;
                default:
                    break;
            }
            return dataReader;
        }

        internal IDataAdapter GetDataAdapter(IDbCommand command, string dbConnectionType) 
        {
            IDataAdapter adapter = null;
            
            switch (dbConnectionType)
            {
                case "SQL":
                    SqlCommand sqlCommand = (SqlCommand)command;
                    adapter = new SqlDataAdapter(command as SqlCommand);
                    break;
                case "ODBC":
                    OdbcCommand odbcCommand = (OdbcCommand)command;
                    adapter = new OdbcDataAdapter(command as OdbcCommand);
                    break;
                //case "UltraLite":
                //    ULCommand ULCommand = (iAnywhere.Data.UltraLite.ULCommand)command;
                //    adapter = new ULDataAdapter(command as ULCommand);                   
                //    break;               
                case "OLEDB":
                    OleDbCommand oledbcommand = (OleDbCommand)command;
                    adapter = new OleDbDataAdapter(command as OleDbCommand);
                    break;
                case "ORACLE":
                    throw new PlatformNotSupportedException(DbConnection.DBType.ORACLE.ToString());
                    break;
                case "MYSQL":
                    throw new PlatformNotSupportedException(DbConnection.DBType.MYSQL.ToString());
                    break;
                default:
                    throw new PlatformNotSupportedException(DbConnection.DBType.UNKNOWN.ToString());                
                    break;
            }
            return adapter;
        }

        #endregion
    }
}
