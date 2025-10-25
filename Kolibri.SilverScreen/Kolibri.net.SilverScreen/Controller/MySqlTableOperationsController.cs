using com.sun.org.apache.bcel.@internal.generic;
using Dapper;
using java.awt.print;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.net.SilverScreen.Controller
{
    public class MySqlTableOperationsController
    {
        private readonly string _connectionString;
        private long innodb_buffer_pool_chunk_size = 16;

        public MySqlTableOperationsController(string connectionString)
        {
            _connectionString = connectionString;
            GetInnoDBBufferSize();
        }

        public async Task<bool> Execute( string sql)
        {
            bool ret = false;
            string connectionString = _connectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    int rowsAffected =await  command.ExecuteNonQueryAsync();                    
                    ret = rowsAffected >= 0;
                }
            }
            return ret;
        }
        public async Task<DataTable> GetData(string query)
        {
            return MySQLController.GetData(_connectionString, query);
        }

        private void GetInnoDBBufferSize()
        {
            try
            {
                string sql = " SELECT VARIABLE_VALUE FROM information_schema.GLOBAL_VARIABLES WHERE VARIABLE_NAME = 'innodb_buffer_pool_chunk_size';";
                innodb_buffer_pool_chunk_size = Convert.ToInt64(GetData(sql).GetAwaiter().GetResult().Rows[0][0]);
            }
            catch (AggregateException aex) { }
            catch (Exception) { }
            
        }

        private async Task<bool> DropDBTable(string tablename)
        {bool ret = false;
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            {
                StringBuilder wr = new StringBuilder();
                wr.AppendLine("SET autocommit=0;");
                wr.AppendLine($"DROP TABLE IF EXISTS {tablename};");
                wr.AppendLine("COMMIT;");
                wr.AppendLine("SET autocommit=1;");

                connection.Open();

                ret = connection.Execute(wr.ToString()) >= 0;
                Console.WriteLine($"Table {tablename} created successfully.");
            }
            return ret;
        }
        public async Task<bool> CrewCreateTable(bool create=false, bool keys=false)
        {
            if(!create&&!keys)
                return false;
         
            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");
            if (create)
            { 
                await DropDBTable("directors");
                await DropDBTable("writers");

                wr.AppendLine($"CREATE TABLE {"directors"} ( name_id varchar(20)  NOT NULL, title_id varchar(20)  NOT NULL);");
                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");

                wr.AppendLine($"CREATE TABLE {"writers"} (name_id varchar(20)  NOT NULL, title_id varchar(20)  NOT NULL);");
                wr.AppendLine("COMMIT;");
            }
            if (keys)
            {
                wr.AppendLine("ALTER TABLE directors ADD PRIMARY KEY (name_id, title_id);");
                wr.AppendLine("COMMIT;");
                wr.AppendLine("ALTER TABLE writers ADD PRIMARY KEY (name_id, title_id);");
                wr.AppendLine("COMMIT;");
            }
            
            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine("COMMIT;");
            bool ret = await Execute(wr.ToString());

            return ret;
        }
        public async Task<bool> TitleCreateTable(bool create = false, bool keys = false)
        {
            if (!create && !keys)
                return false;

            
            StringBuilder wr = new StringBuilder();

            wr.AppendLine("SET autocommit=0;");
            if (create)
            {
                DropDBTable("title");
                wr.AppendLine(
                "CREATE TABLE titles (id varchar(20) NOT NULL, titleType varchar(20) NOT NULL, primaryTitle text NOT NULL, " +
                    "originalTitle text, isAdult boolean, startYear int, endYear int, runtimeMinutes int, " +
                    "genres varchar(255));"
                );
                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");
            }
            if (keys) {
                wr.AppendLine("ALTER TABLE titles ADD PRIMARY KEY (id);");
            }

            wr.AppendLine("SET autocommit=1;");
            wr.AppendLine("COMMIT;");

            bool ret = await Execute(wr.ToString());
            return ret;
        }

        internal async Task<bool> PrincipalsCreateTable(bool create, bool keys)
        {bool ret = false;  
            if (!create && !keys)
                return ret;
            
            string tableName = "principals";

            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");
            if (create)
            {
                wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");

                wr.AppendLine($"CREATE TABLE {tableName} (title_id varchar(20) NOT NULL,ordering int, name_id varchar(20) NOT NULL,  category varchar(255), job text, characters text);");
                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");
            }
            if (keys)
            {

                wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (title_id, ordering);");
                //     wr.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT fk_principals_title_id FOREIGN KEY (title_id) REFERENCES titles(id) ON DELETE CASCADE;");
                //     wr.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT fk_principals_name_id FOREIGN KEY (name_id) REFERENCES names(id) ON DELETE CASCADE;");
                wr.AppendLine("COMMIT;");
            }

            ret = await Execute(wr.ToString());
            return ret;
        }

        internal async Task<int> GetRowCountFromTable(string tableName)
        {
            var number = await GetData($"SHOW TABLE STATUS LIKE '{tableName}';");
            //  var rowCount = StringUtilities.FormatBigNumbers(number.Rows[0]["Rows"]);
            return Convert.ToInt32(number.Rows[0]["Rows"]);
        }
    }
}
