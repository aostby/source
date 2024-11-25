using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Odbc;


namespace Kolibri.Data.Connection
{
    /// <summary>
    /// Generisk klasse som arver DbAccess og som kan koble seg til hva det skal være dersom connectionStringen er støttet.
    /// </summary>
    public class DbConnection : DbAccess //, IClient //må legges til i ALLE klasser som benytter DbConnection, utsetter dette.
    {
        // http://www.codeproject.com/KB/database/DBaseFactGenerics.aspx 
        #region public enums
        /// <summary>
        /// Databasetype for oppkobling.
        /// </summary>
        public   enum DBType 
               {
            /// <summary>
            /// Database av ukjent type, connection kan fungere uansett - men med redusert funksjonalitet.
            /// </summary>
            UNKNOWN = 0, 
            /// <summary>
            /// Microsoft SQL Server 
            /// </summary>
               MSSQL,
            /// <summary>
               /// Oracle Provider - From Oracle Provider - From Microsoft Oracle Provider- dersom dette skal virke må .dll filer legges til i DbConnection, altså ny versjon
            /// </summary>
               ORACLE , 
            /// <summary>
            ///  ASE, 
            /// </summary>
               SYBASE ,
            /// <summary>
            /// Sybase ultralite
            /// </summary>
            UltraLite , 
            /// <summary>
            /// From CoreLab - dersom dette skal virke må .dll filer legges til i DbConnection, altså ny versjon            /// 
            /// </summary>
               MYSQL,
			   
			   } 
        #endregion

        #region Private Members
        #region typer av connectionstrings
        private string[] oraclearray = new string[] { "Data Source=", "User ID=", "Password=" };
        private string[] sqlarray = new string[] { "Data Source=", "Database=", "User ID=", "Password=" };
        private string[] sqlarray2 = new string[] { "Data Source=", "Initial Catalog=", "User ID=", "Password=" };
        private string[] mysqlarray = new string[] { "Server=", "Database=", "Uid=", "Pwd=" };
        #endregion
        
        private IDbTransaction m_dbTrans;   // Transaction object som tar seg av commit og rollback
        private IDbConnection m_dbConn;     // Connection object som holder connection mot databasen
        private DbException m_Status = null; // Objekt som sier noe om hvilken feil som har oppstått og hva som er evt er galt
        
        private string m_dbConnType = "";
        private DBType m_dbType = DBType.UNKNOWN;

        private string m_dbConnectionstring = "";
        private string m_dbPassword;
        private bool m_KeepAlive;

        public string StatusMelding = "";
        private int m_timeOutCount = 0;
        #endregion

        #region Constructor

        public DbConnection(string DbConnectionString)
        {
            m_dbConnectionstring = DbConnectionString;
            m_dbConnType = HentDbConnectionType(m_dbConnectionstring);
            SetPassword();
            if (m_dbType == DBType.UNKNOWN)
            {
                m_dbType = FinnDBType();
            }
            
        }

        /// <summary>
        /// Metode som aldri kan bli ferdig men som forsøker å si noe om DBTypen til en ODBC / eller ADO whatnot utifra 
        /// drivernavnet. 
        /// </summary>
        /// <returns>dbtype hvis funnet, dbtype unknown hvis ikke</returns>
        private DBType FinnDBType()
        {
            DBType ret = DBType.UNKNOWN;
            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if ( m_dbConn.State == ConnectionState.Closed)
                {
                    m_dbTrans = null;
                    m_dbConn.Open();
                    if (m_dbConn.GetType().Equals(typeof(System.Data.Odbc.OdbcConnection)))
                    {
                        string driver = ((System.Data.Odbc.OdbcConnection)(m_dbConn)).Driver.ToUpper();
                        switch (driver)
                        {
                            case "SYBDRVODB.DLL":
                            case "SYBDRVODB64.DLL":
                            case "SYODASE.DLL":
                            case "SYSYB95.DLL":
                            case "SYSYBNT.DLL":
                            case "DBODBC9.DLL":
                            case "DBODBC11.DLL":
                            case "DBODBC12.DLL":
                                ret = DBType.SYBASE;
                                break;
                            case "DRIVERNAVN1":
                                ret = DBType.MSSQL;
                                break;
                            case "DRIVERNAVN2":
                                ret = DBType.ORACLE;
                                break;
                            case "DRIVERNAVN3":
                                ret = DBType.SYBASE;
                                break;
                            case "DRIVERNAVN4":
                                ret = DBType.MYSQL;
                                break;
                            default:
                                if (driver.StartsWith("DBODBC1"))
                                    ret = DBType.SYBASE;
                            break;
                        }
                    }
                    m_dbConn.Close();
                }
            }
            catch (Exception ex)
            {
                m_Status = new DbException(new System.Exception("FinnDBType feiler"), ex.Message);
                //throw ex;
            }
            return ret;
        }

        #endregion

        #region Destructor
        /// <summary>
        /// Destructor som rydder opp i verdier og lukker connection mot databasen
        /// </summary>
        ~DbConnection()
        {
            try
            {
                m_dbTrans = null;
                m_dbConn = null;
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                m_Status = null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Dersom oppkoblingen er uspesifikk (ODBC) finnes dette her, ellers benytt DbType
        /// </summary>
         private  string DbConnectionType
        {
            get { return m_dbConnType; }
            set { m_dbConnType = value; }
        }
        /// <summary>
        /// Passord som benyttes for oppkobling
        /// </summary>
        public string DbPassword
        {
            get { return m_dbPassword; }
          //  set { m_dbPassword = value; }
        }

        /// <summary>
        /// Oppkoblingens tilkoblingsstreng
        /// </summary>
        public string DbConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace( m_dbConnectionstring))
                    m_Status = new DbException(new System.Exception("Connectionstring mangler i config"), "DbConnectionString");
                return m_dbConnectionstring;
            }
            set
            {
                m_dbConnectionstring = value;
                SetPassword();
            }
        }

        /// <summary>
        /// Oppkoblingens DbType
        /// </summary>
        public DBType DbType 
        {
            get { return m_dbType; }
            set {  m_dbType = value; }
        }
        /// <summary>
        /// Status for oppkoblingen
        /// </summary>
        public DbException Status
        {
            get {return m_Status; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            try
            {
                if (m_dbConn != null)
                    m_dbConn.Close();
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "Close");
                throw e;
            }
        }

        /// <summary>
        /// Prøve å åpne DB
        /// </summary>
        /// <returns>True dersom åpen; ellers false.</returns>
        public bool Open()
        {
            bool ret = false;
            IDbConnection dbConnLokal;
            try
            {
                dbConnLokal = this.GetDbConnection(DbConnectionString, DbConnectionType);
                dbConnLokal.Open();

                if (dbConnLokal.State == ConnectionState.Open)
                {
                    ret = true;
                    dbConnLokal.Close();
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "Open");
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int SqlExecute(string sql)
        {

            int value = 0;
            m_Status = null;

            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                {
                    m_dbTrans = null; // Må nullstille transaksjonen slik at den blir opprettet på nytt når connection detter ned
                    m_dbConn.Open();
                }

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                value = dbCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Rollback();
                m_Status = new DbException(e, "SqlExecute");
                throw e;
            }
            return value;
        }

        /// <summary>
        /// Funksjon som kjører en samling av sqler. 
        /// Her må alle gå ok. Om en feiler returerer funksjonen feil.
        /// </summary>
        /// <param name="sql">Arraylist</param>
        /// <returns></returns>
        public bool SqlExecute(ArrayList sql)
        {
            bool value = true;
            m_Status = null;

            try
            {
                foreach (object s in sql)
                {
                    SqlExecute(s.ToString());
                    if (m_Status != null)
                    {
                        value = false;
                        break; // En feil har oppstått. Ingen vits i å fullføre.
                    }
                }

            }
            catch (Exception e)
            {
                Rollback();
                m_Status = new DbException(e, "SqlExecute");
                value = false;
                throw e;
            }

            return value;
        }

        /// <summary>
        /// Ikke testet enda!
        /// </summary>
        /// <param name="StoreProcName">SP som skal kjøres</param>
        /// <param name="Params">liste over parameter som skal gies inn til SP</param>
        /// <returns></returns>
        protected int ExecuteNonQuery(string StoreProcName, List<DbParameter> Params)
        {
            bool internalOpen = false;
            IDbCommand cmd;
            try
            {
                m_Status = null;
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);



                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();
                
                cmd =   this.GetDbCommand(DbConnectionType);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoreProcName;
                cmd.Connection = m_dbConn;

                if (Params != null || Params.Count > 0)
                {
                    foreach (DbParameter param in Params) 
                        cmd.Parameters.Add(param);
                }
                if (m_dbConn.State == ConnectionState.Closed)
                {
                    m_dbTrans = null; // Må nullstille transaksjonen slik at den blir opprettet på nytt når connection detter ned
                    m_dbConn.Open();
               
                    internalOpen = true;
                }
                return cmd.ExecuteNonQuery();
            }
            catch (Exception DbEx)
            {
                m_Status = new DbException(DbEx, "ExecuteNonQuery");
                throw DbEx;
            }           
            finally
            {
                if (internalOpen) 
                    m_dbConn.Close();
            }
        }

        /// <summary>
        /// Returnerer det antall rader som er spesifisert
        /// </summary>
        /// <param name="sql">sqlen som skal kjøres</param>
        /// <param name="sqlRes">resultatet som blir returnert</param>
        /// <param name="rowCount">det antall rader en vil returnere</param>
        /// <returns>true om alt går bra</returns>
        public bool SqlSelect(string sql, ref SqlResult sqlRes, long rowCount)
        {
            bool value = false;
            try
            {
                if (DbConnectionType == "ORACLE")
                {

                }
                else
                {
                    SqlExecute("SET ROWCOUNT " + rowCount);
                    if (SqlSelect(sql, ref sqlRes))
                        value = true;
                    //Må sette tilbake
                    SqlExecute("SET ROWCOUNT 0");
                }
            }
            catch (Exception e)
            {
                value = false;
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlRes"></param>
        /// <returns></returns>
        public bool SqlSelect(string sql, ref SqlResult sqlRes)
        {
            ////Trace.Entry(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,SqlResult)");
            // //Trace.TraceLine(TraceItem.SQL, sql);
            bool value = false;
            m_Status = null;

            try
            {
                sqlRes = null; //Må nullstille siden en kan bruke samme resultset til flere operasjoner
                ArrayList list = new ArrayList();
                int nFieldCount = 0;

                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                //Setter timout til 1 minutt, men mulighet for overstyring via config
                // if (Config.GetConfig("DbConnectionTimeout").Value != null)
                // dbCommand.CommandTimeout = Convert.ToInt32(Config.GetConfig("DbConnectionTimeout").Value);
                // else
                dbCommand.CommandTimeout = 60;

                IDataReader dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                SqlResultRow SqlTmp = null;
                //Leser fra dbReader inn i ArrayList midlertidig
                while (dbReader.Read())
                {
                    SqlTmp = new SqlResultRow();
                    SqlTmp.Fields = new SqlField[dbReader.FieldCount];

                    for (int i = 0; i < dbReader.FieldCount; i++)
                    {
                        SqlTmp.Fields[i] = new SqlField();
                        if (!dbReader.IsDBNull(i))
                        {
                            SqlTmp.Fields[i].Name = dbReader.GetName(i).Trim();
                            SqlTmp.Fields[i].Value = dbReader.GetValue(i).ToString().Trim();
                        }
                        else
                        {
                            SqlTmp.Fields[i].Name = dbReader.GetName(i).Trim();
                            SqlTmp.Fields[i].Value = null;
                        }
                    }
                    //Legger resultatet inn i arraylisten
                    list.Add(SqlTmp);
                }
                value = true;

                dbReader.Close();
                //dbConn.Close();


                if (list.Count > 0)
                {
                    sqlRes = new SqlResult();
                    sqlRes.Rows = new SqlResultRow[list.Count];
                    int nRes = 0;
                    foreach (SqlResultRow res in list)
                    {
                        nFieldCount = res.Fields.GetUpperBound(0) + 1;
                        sqlRes.Rows[nRes] = new SqlResultRow();
                        sqlRes.Rows[nRes].Fields = new SqlField[nFieldCount];
                        for (int i = 0; i < nFieldCount; i++)
                        {
                            sqlRes.Rows[nRes].Fields[i] = res.Fields[i];
                        }
                        nRes += 1;
                    }
                }
            }
            catch (TimeoutException t)
            {
                m_timeOutCount++; //Teller opp timeoutcount

                m_Status = new DbException(t, "SqlSelect");
                if (m_timeOutCount > 2)
                {
                    m_timeOutCount = 0;
                    throw t;
                }
                else
                {
                    value = SqlSelect(sql, ref sqlRes);
                }
            }
            catch (Exception e)
            {
                value = false;
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            //Trace.Exit(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,SqlResult)", value);
            return value;
        }

        public bool SqlSelect(string sql, ref DataSet ds)
        {
            //Trace.Entry(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,DataSet)");
            //Trace.TraceLine(TraceItem.SQL, sql);
            bool value = false;
            m_Status = null;

            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();                

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                IDataReader dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                //ds = DataReaderToDataSet(dbReader);
                ds = DataReaderToDataSetTest(dbReader);
                dbReader.Close();

                value = true;
            }
            catch (TimeoutException t)
            {
                m_timeOutCount++; //Teller opp timeoutcount

                m_Status = new DbException(t, "SqlSelect");
                if (m_timeOutCount > 2)
                {
                    m_timeOutCount = 0;
                    throw t;
                }
                else
                {
                    value = SqlSelect(sql, ref ds);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            //Trace.Exit(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,DataSet)", value);
            return value;
        }

        public bool SqlSelect(string sql, ref DataSet ds, long rowCount)
        {
            //Trace.Entry(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,DataSet,long)");
            //Trace.TraceLine(TraceItem.SQL, sql);
            bool value = false;
            m_Status = null;

            try
            {
                if (DbConnectionType == "ORACLE")
                {

                }
                else
                {
                    SqlExecute("SET ROWCOUNT " + rowCount);
                    if (SqlSelect(sql, ref ds))
                        value = true;
                    //Må sette tilbake
                    SqlExecute("SET ROWCOUNT 0");
                }
            }
            catch (TimeoutException t)
            {
                m_timeOutCount++; //Teller opp timeoutcount

                m_Status = new DbException(t, "SqlSelect");
                if (m_timeOutCount > 2)
                {
                    m_timeOutCount = 0;
                    throw t;
                }
                else
                {
                    value = SqlSelect(sql, ref ds, rowCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            //Trace.Exit(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,DataSet,long)", value);
            return value;
        }

        public bool SqlSelect(string sql, ref IDataReader dbReader)
        {
            //Trace.Entry(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,IDataReader)");
            //Trace.TraceLine(TraceItem.SQL, sql);
            bool value = false;
            m_Status = null;

            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                value = true;
            }
            catch (TimeoutException t)
            {
                m_timeOutCount++; //Teller opp timeoutcount

                m_Status = new DbException(t, "SqlSelect");
                if (m_timeOutCount > 2)
                {
                    m_timeOutCount = 0;
                    throw t;
                }
                else
                {
                    value = SqlSelect(sql, ref dbReader);
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            //Trace.Exit(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,IDataReader)", value);
            return value;
        }

        public bool SqlSelectTest(string sql, ref DataSet ds) {
        
             bool value = false;
            m_Status = null;

            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();                

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                IDataReader dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                ds = DataReaderToDataSetTest(dbReader);
                dbReader.Close();

                value = true;
            }
            catch (TimeoutException t)
            {
                m_timeOutCount++; //Teller opp timeoutcount

                m_Status = new DbException(t, "SqlSelect");
                if (m_timeOutCount > 2)
                {
                    m_timeOutCount = 0;
                    throw t;
                }
                else
                {
                    value = SqlSelect(sql, ref ds);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                m_Status = new DbException(e, "SqlSelect");
                throw e;
            }

            //Trace.Exit(TraceItem.PERFORMANCE | TraceItem.SQL, this, "SqlSelect(string,DataSet)", value);
            return value;        
        }

        public bool UpdateDataTableSimple(DataTable table, string tableName)
        {

            bool value = false;
            m_Status = null;
            string sql = "SELECT top 1 * from " + tableName;
            DataSet dataSet = new DataSet();
            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                // dbCommand.CommandText = sql;
                IDataAdapter IAdapter = this.GetDataAdapter(dbCommand, DbConnectionType);
                int tall; switch (DbType)
                {

                    case DBType.MSSQL:
                        SqlDataAdapter sqladapter = (SqlDataAdapter)IAdapter;
                        sqladapter.SelectCommand = new SqlCommand(sql, m_dbConn as SqlConnection);
                        SqlCommandBuilder sqlbuilder = new SqlCommandBuilder(sqladapter);
                        sqladapter.Fill(dataSet);
                        sqladapter.UpdateCommand = sqlbuilder.GetUpdateCommand();
                        sqladapter.InsertCommand = sqlbuilder.GetInsertCommand();
                        sqladapter.DeleteCommand = sqlbuilder.GetDeleteCommand();
                        tall = sqladapter.Update(table);                        
                        value = tall > 0;
                        break;
                    case DBType.ORACLE:
                        throw new PlatformNotSupportedException(DBType.ORACLE.ToString());
                        break;
                    case DBType.SYBASE:
                        OdbcDataAdapter odbcadapter = (OdbcDataAdapter)IAdapter;
                        odbcadapter.SelectCommand = new OdbcCommand(sql, m_dbConn as OdbcConnection);
                        OdbcCommandBuilder odbcbuilder = new OdbcCommandBuilder(odbcadapter);
                        odbcadapter.Fill(dataSet);
                        odbcadapter.UpdateCommand = odbcbuilder.GetUpdateCommand();
                        odbcadapter.InsertCommand = odbcbuilder.GetInsertCommand();
                        odbcadapter.DeleteCommand = odbcbuilder.GetDeleteCommand();
                        tall = odbcadapter.Update(table);                     
                        value = tall > 0;
                        break;
                    case DBType.MYSQL:
                        throw new PlatformNotSupportedException(DBType.MYSQL.ToString());
                        break;
                    //case DBType.UltraLite:
                    //    ULDataAdapter uladapter = (ULDataAdapter)IAdapter;
                    //    uladapter.SelectCommand = new ULCommand(sql, m_dbConn as ULConnection);
                    //    ULCommandBuilder ulbuilder = new ULCommandBuilder(uladapter);
                    //    uladapter.Fill(dataSet);
                    //    uladapter.UpdateCommand = ulbuilder.GetUpdateCommand();
                    //    uladapter.InsertCommand = ulbuilder.GetInsertCommand();
                    //    uladapter.DeleteCommand = ulbuilder.GetDeleteCommand();
                    //    tall = uladapter.Update(table);                     
                    //    value = tall > 0;
                    //    break;
                    case DBType.UNKNOWN:
                        throw new PlatformNotSupportedException(DBType.UNKNOWN.ToString());
                    default:
                        break;
                }

                m_dbConn.Close();
            }
            catch (Exception)
            {
                value = false;
            }

            return value;
        }

        public bool Commit()
        {
            //Trace.Entry(TraceItem.SQL, this, "Commit");
            bool bRetur = false;
            m_Status = null;

            try
            {
                if (m_dbTrans != null)
                {
                    m_dbTrans.Commit();
                    bRetur = true;
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "SqlSelect", "An exception of type " + e.GetType() + " was encountered while attempting to commit the transaction");
                try
                {
                    m_dbTrans.Rollback();
                }
                catch (DataException ex)
                {
                    if (m_dbTrans.Connection != null)
                    {
                        m_Status = new DbException(ex, "Commit", "An exception of type " + ex.GetType() + " was encountered while attempting to roll back the transaction.");
                    }
                    throw ex;
                }
            }
            finally
            {
                m_dbTrans = null;
            }

            //Trace.Exit(TraceItem.SQL, this, "Commit", bRetur);
            return bRetur;
        }

        public bool Rollback()
        {
            //Trace.Entry(TraceItem.SQL, this, "Rollback");
            bool bRetur = false;
            m_Status = null;

            try
            {
                if (m_dbTrans != null)
                {
                    m_dbTrans.Rollback();
                }
                bRetur = true;
            }
            catch (Exception e)
            {
                if (m_dbTrans.Connection != null)
                {
                    m_Status = new DbException(e, "Rollback");
                }
                throw e;
            }
            finally
            {
                m_dbTrans = null;
            }

            //Trace.Exit(TraceItem.SQL, this, "Rollback", bRetur);
            return bRetur;
        }

        public bool DoExist(string sql, bool CloseConnection)
        {
            bool retValue = false;
            m_Status = null;

            try
            {
                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                IDataReader dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                if (dbReader.Read())
                    retValue = true;

                dbReader.Close();

                if (CloseConnection)
                {
                    m_dbConn.Close();
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "DoExist");
                throw e;
            }

            return retValue;
        }

        public bool DoExist(string sql)
        {

            return DoExist(sql, false);
        }

        public long CountRows(string sql)
        {
            long value = 0;
            try
            {
                ArrayList list = new ArrayList();

                if (m_dbConn == null)
                    m_dbConn = this.GetDbConnection(DbConnectionString, DbConnectionType);

                if (m_dbConn.State == ConnectionState.Closed)
                    m_dbConn.Open();

                if (m_dbTrans == null)
                    m_dbTrans = m_dbConn.BeginTransaction();

                IDbCommand dbCommand = this.GetDbCommand(DbConnectionType);
                dbCommand.Connection = m_dbConn;
                dbCommand.Transaction = m_dbTrans;
                dbCommand.CommandText = sql;
                IDataReader dbReader = this.GetDataReader(dbCommand, DbConnectionType);

                //Leser fra dbReader inn i ArrayList midlertidig
                while (dbReader.Read())
                {
                    value += 1;
                }
                dbReader.Close();

            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "CountRows");
                throw e;
            }
            return value;
        }

        /// <summary>
        /// Metode som henter brukernavnet oppgitt i dbconnectionstringen
        /// </summary>
        /// <returns>brukerid hvis den finnes, hvis ikke tom streng</returns>
        public string GetUsername()
        {
            string ret = "";
            string[] connectionarray = m_dbConnectionstring.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (m_dbConnectionstring.ToUpper().Contains("USER ID") || m_dbConnectionstring.ToUpper().Contains("UID"))
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("User ID", StringComparison.OrdinalIgnoreCase) || item.StartsWith("UID", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=") + 1);
                            return ret;
                        }
                    }
                }
                else
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("Pwd", StringComparison.OrdinalIgnoreCase))
                            ret = item.Substring(item.IndexOf("=") + 1);
                        return ret;
                    }
                }
            }

            catch (Exception e)
            {
                m_Status = new DbException(e, "GetUsername");
            }
            return ret;
        }
        public string GetDBname() {

            string ret = "";
            string[] connectionarray = m_dbConnectionstring.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (DbConnectionType.Equals("ODBC"))
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("DSN", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=") + 1);
                            return ret;
                        }
                    }
                }
                if (m_dbType.Equals(DBType.UltraLite))
                {
                    try
                    {
                        if (m_dbConnectionstring.StartsWith("DBF", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = m_dbConnectionstring.Substring(m_dbConnectionstring.IndexOf("=") + 1);
                            return ret;
                        }
                    }
                    catch (Exception)
                    { }
                }

                if (m_dbConnectionstring.ToUpper().Contains("DATABASE"))
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("DATABASE", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=") + 1);
                            return ret;
                        }
                    }
                }
                else
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("Pwd", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=") + 1);
                            return ret;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "GetUserPassword");
            }
            return ret;
        
        }
        public IDbConnection GetDbConnection()
        {
            return GetDbConnection(DbConnectionString, DbConnectionType);
        }

        public string GetDbConnectionType()
        {
            return DbConnectionType;
        }
        #endregion

        #region Private Methods

        private void SetPassword()
        {
           string temppassword = GetUserPassword();
           if (!string.IsNullOrWhiteSpace(temppassword))
               m_dbPassword = temppassword;
        }

        /// <summary>
        /// Metode som henter passordet som ligger i dbConnectionString
        /// </summary>
        /// <returns>passordet hvis det finnes, hvis ikke tom streng</returns>
        private string GetUserPassword()
        {
            string ret = "";
            string[] connectionarray = m_dbConnectionstring.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {                
                if (m_dbConnectionstring.ToUpper().Contains("PASSWORD"))
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=")+1);
                            return ret;
                        }
                    }
                }
                else
                {
                    foreach (string item in connectionarray)
                    {
                        if (item.StartsWith("Pwd", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = item.Substring(item.IndexOf("=")+1);
                            return ret;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "GetUserPassword");
            }
            return ret;
        }
        
        private DataSet DataReaderToDataSet(IDataReader reader)
        {
            DataSet ds = null;
            try
            {
                DataTable table = new DataTable();
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                }
                table.BeginLoadData();
                object[] values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();
                ds = new DataSet();
                ds.Tables.Add(table);
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "DataReaderToDataSet");
                throw e;
            }
            return ds;
        }

        private DataSet DataReaderToDataSetTest(IDataReader reader) {
            DataSet ds = null;
            try
            {
       // And this is how to use it:
            MyDataAdapter dataAdapter = new MyDataAdapter();
            DataTable table = new DataTable();
            dataAdapter.FillFromReader( table, reader );
            ds = new DataSet();
            ds.Tables.Add(table);
            }
            catch (Exception e)
            {
                m_Status = new DbException(e, "DataReaderToDataSet");
                throw e;
            }
            return ds;
     



        }

        /// <summary>
        /// metode som henter type utifra de vanligste connectionsstrings
        /// kilde: http://www.connectionstrings.com/
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private string HentDbConnectionType(string connectionString)
        {
            string ret = "";
           
            string[] connectionarray = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < connectionarray.Length; i++)
            {
                string entitet = connectionarray[i].Substring(0, connectionarray[i].IndexOf("=") + 1);
                entitet = entitet.Trim();
                if (entitet.StartsWith("Password") || entitet.ToUpper().Equals("PWD="))
                   m_dbPassword = connectionarray[i].Replace(entitet, "");
                connectionarray[i] = entitet;
            }

            if (SjekkDbConnectionTypeArray(mysqlarray, connectionarray)) { ret = "MYSQL"; m_dbType = DBType.MSSQL; }
            else if (SjekkDbConnectionTypeArray(oraclearray, connectionarray)) { ret = "ORACLE"; m_dbType = DBType.ORACLE; }
            else if (SjekkDbConnectionTypeArray(sqlarray, connectionarray)) { ret = "SQL"; m_dbType = DBType.MSSQL; }
            else if (SjekkDbConnectionTypeArray(sqlarray2, connectionarray)){ ret = "SQL"; m_dbType = DBType.MSSQL; }
            else if (connectionString.ToUpper().Contains("DSN=") && connectionString.ToUpper().Contains("UID=") && connectionString.ToUpper().Contains("PWD="))
            {
                ret = "ODBC";
            }
  
            else if (connectionString.ToLower().StartsWith("dbf"))
            {
                ret = Enum.GetName(typeof(DBType), (int) DBType.UltraLite);
                m_dbType = DBType.UltraLite;
            }

            //if (connectionString.ToUpper().Contains("SQLEXPRESS"))
            //{
            //    ret = "SQL";
            //    m_dbType = DBType.MSSQL;
            //}

            if (string.IsNullOrWhiteSpace(ret))
            {
                if (connectionString.ToUpper().Contains("SQLEXPRESS") | connectionString.ToUpper().Contains("INTEGRATED SECURITY=") | connectionString.ToUpper().Contains("INITIAL CATALOG"))
                {
                    ret = "SQL";
                    m_dbType = DBType.MSSQL;
                }
            }

         
            return ret;
        }

        private bool SjekkDbConnectionTypeArray(string[] typearray, string[] connectionarray)
        {
            bool ret = false;
            if (typearray.Length != connectionarray.Length)
                return false;

            foreach (string item in typearray)
            {
                if (Array.IndexOf(connectionarray, item) >= 0)
                    ret = true;
                else
                {
                    ret = false;
                    return ret;
                }
                
            }return ret;
        }
        #endregion
                
        #region IDisposable Members
        public void Dispose()
        {
            if (m_dbConn != null)
            {   //Remarks
                //The Close method rolls back any pending transactions. It then releases the connection to the connection pool,
                // or closes the connection if connection pooling is disabled.
                // An application can call Close more than one time without generating an exception.
                
                //if(m_dbConn.State == ConnectionState.Open)
                m_dbConn.Close();
                m_dbConn.Dispose();
            }
        }
        #endregion

     

    }
    #region helper classes
    /// <summary>
    /// Helper-class to fill a DataTable from an IDataReader-Instance
    /// </summary>
    public class MyDataAdapter : DbDataAdapter
    {
        public MyDataAdapter()
        {
        }

        public int FillFromReader(System.Data.DataTable dataTable,
            System.Data.IDataReader dataReader)
        {

            return base.Fill(dataTable, dataReader);

        }
    }

}
    #endregion
