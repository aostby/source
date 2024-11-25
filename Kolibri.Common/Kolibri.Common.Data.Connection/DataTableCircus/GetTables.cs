using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Data.Connection.DataTableCircus
{
   public class GetTables
    {


        //mySQL
        //   https://stackoverflow.com/questions/7771455/create-c-sharp-classes-based-of-mysql-table
        public static string  GetTableMySQL(string dbConnection) {
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
    }
}
