using com.sun.org.apache.bcel.@internal.generic;
using com.sun.xml.@internal.bind.v2.model.core;
using Dapper;
using java.awt.print;
using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using LiteDB;
using MySql;
using MySql.Data.MySqlClient;
using OMDbApiNet.Model;
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

        public string GetDBName { get {return    new MySqlConnection(_connectionString).Database; } }

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
                DropDBTable("titles");
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
        {
            bool ret = false;  
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

        internal async Task<bool> TitlesLangCreateTable(bool create, bool keys)
        {
            bool ret = false;
            if (!create && !keys)
                return ret;

            string tableName = "titles_lang";

            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");
            if (create)
            {
                wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");

                wr.AppendLine(@$"CREATE TABLE {tableName}   (title_id varchar(20) NOT NULL, 
                    ordering int, title text NOT NULL, 
                    region varchar(5), language varchar(5), 
                    types varchar(40),  attributes varchar(255), isOriginalTitle boolean); ");
                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");
            }     if (keys)
            {
                wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (title_id, ordering);");
                wr.AppendLine("COMMIT;");
                wr.AppendLine($"ALTER TABLE {tableName} ADD CONSTRAINT fk_titles_lang_title_id FOREIGN KEY (title_id) REFERENCES titles(id) ON DELETE CASCADE;");
                wr.AppendLine("COMMIT;");
            }

            ret = await Execute(wr.ToString());
            return ret;
        }

        internal async Task<bool> EpisodesCreateTable(bool create, bool keys)
        {
            bool ret = false;
            if (!create && !keys)
                return ret;

            string tableName = "episodes";

            StringBuilder wr = new StringBuilder();
            wr.AppendLine("SET autocommit=0;");
            if (create)
            {
                 
                wr.AppendLine("SET autocommit=0;");
                wr.AppendLine($"DROP TABLE IF EXISTS {tableName};");

                wr.AppendLine($"CREATE TABLE {tableName} (id varchar(20) NOT NULL, parentId varchar(20) NOT NULL, seasonNumber int, episodeNumber int);");
                wr.AppendLine(Environment.NewLine);

                wr.AppendLine(Environment.NewLine);
                wr.AppendLine("COMMIT;");  

            }
            if (keys)
            {
                wr.AppendLine($"ALTER TABLE {tableName} ADD PRIMARY KEY (id);");
                wr.AppendLine("COMMIT;"); 
            }

            ret = await Execute(wr.ToString());
            return ret;
        }

        internal async Task<Item> GetTVShow(string imdbId) {
            Item ret = null;    
            string sql = $@"SELECT 
st.primarytitle as Title,
CAST(st.startYear as int) as Year ,
convert( st.startYear, varchar (4))+'01-01' as Released ,
st.runtimeMinutes as Runtime, 
st.genres as Genre,
n.primaryName as Director, 
n.primaryName  as Writer,  
'TV-MA' as Rated, 
r.averageRating as ImdbRating,
r.numVotes as ImdbVotes,
st.id as ImdbId,
/* st.titleType  as Type,*/
'series' as type,
max(e.seasonNumber) as TotalSeasons
FROM  titles AS st
INNER JOIN       episodes  e ON ( e.parentId  = st.id )
LEFT  OUTER JOIN ratings   r ON ( st.id = r.id )
LEFT  OUTER JOIN principals   p ON ( st.id = p.title_id and category = 'writer' )
LEFT  OUTER JOIN names   n ON ( p.name_id  = n.name_id  )
-- WHERE st.primarytitle = 'Better Off Ted'
AND   st.titleType  = 'tvSeries'
where st.Id = '{imdbId}'";
            DataTable table = await GetData(sql);

            if (table != null && table.Rows.Count >= 1)
            {
                ret = DataSetUtilities.Cast<Item>(table.Rows[0]);
            }
            return ret;
        }
    }
}
