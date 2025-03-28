using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace Kolibri.net.Common.Dal.Controller
{

    public class MySQLController
    {
        public string _dbConnectionString;

        public MySQLController(string dbConnectionString) { _dbConnectionString = dbConnectionString; }



 
    /// <summary>
    /// Get classes out of a table in MySQL
    /// </summary>
    /// <param name="dbConnection"></param>
    /// <returns></returns>
        public static string GetTableMySQL(string dbConnection)
        {    //   https://stackoverflow.com/questions/7771455/create-c-sharp-classes-based-of-mysql-table
            string ret = string.Empty;

            string sql = @"select concat('public ',tps.dest,' ',column_name,'{get;set;}') as code
from information_schema.columns c
join (
select 'char' as orign ,'string' as dest union all
select 'varchar' ,'string' union all
select 'longtext' ,'string' union all
select 'datetime' ,'DateTime' union all
select 'text' ,'string' union all
select 'bit' ,'int' union all
select 'bigint' ,'int' union all
select 'int' ,'int' union all
select 'double' ,'double' union all
select 'decimal' ,'double' union all
select 'date' ,'DateTime' union all
select 'tinyint' ,'bool'
) tps on c.data_type like tps.orign
where table_schema = 'your_schema' and table_name = 'your_table'
order by c.ordinal_position";

            ret = sql;
            return ret;
        }


        public static DataTable GetData(string connectionString, string sql)
        {
            DataTable dt = new DataTable();
            using (MySql.Data.MySqlClient.MySqlConnection c = new MySqlConnection(connectionString))
            {
                using (MySql.Data.MySqlClient.MySqlDataAdapter sda = new MySqlDataAdapter(sql, c))
                {
                    sda.Fill(dt);
                }
                return dt;
            }
        }
    }
}


