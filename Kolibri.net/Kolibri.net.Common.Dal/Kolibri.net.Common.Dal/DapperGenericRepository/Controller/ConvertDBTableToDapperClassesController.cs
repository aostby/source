using Microsoft.CSharp;
using MySql.Data.MySqlClient;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Globalization;
using System.Text;

namespace DapperGenericRepository.Controller
{
    public class ConvertDBTableToDapperClassesController
    {
        public string ConnectionString { get; }
        public ConvertDBTableToDapperClassesController(string connectionstring)
        {
            this.ConnectionString = connectionstring;
        }

        public List<string> GetTableNames()
        {
            using (var con = new MySqlConnection(ConnectionString))
            {
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    var dt = Execute($"SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{con.Database}' order by 1 ASC");
                    List<string> list = dt.AsEnumerable()
                                   .Select(r => r.Field<string>("TABLE_NAME"))
                                   .ToList();
                    return list;
                }
            }
        }
        public DataTable GetClassForTable(string tableName)
        {
            string sql = $@" 
SELECT 
table_name, tps.dest as dataType,column_name as columnName,
Concat('public ', tps.dest, ' ', column_name, '{{get;set;}}') AS code
,   concat('public ',tps.dest, IF(tps.dest = 'string', '', IF(is_nullable = 'NO', '', '?'))) as nullable
, CASE WHEN COLUMN_KEY = 'PRI' THEN 'y' ELSE 'n' END AS isKey      
FROM   information_schema.columns c
       JOIN(SELECT 'char'   AS orign,
                   'string' AS dest
            UNION ALL
            select 'tinytext' ,'string' union all
            SELECT 'varchar',
                   'string'
            UNION ALL
            SELECT 'longtext',
                   'string'
            UNION ALL
            SELECT 'datetime',
                   'DateTime'
            UNION ALL
            SELECT 'text',
                   'string'
            UNION ALL
            SELECT 'bit',
                   'int'
            UNION ALL
            SELECT 'bigint',
                   'int'
            UNION ALL
            SELECT 'int',
                   'int'
            UNION ALL
            SELECT 'double',
                   'double'
            UNION ALL
            SELECT 'decimal',
                   'double'
            UNION ALL
            SELECT 'date',
                   'DateTime'
            UNION ALL
            SELECT 'tinyint',
                   'bool') tps
         ON c.data_type LIKE tps.orign
WHERE  table_name = '{tableName}'
ORDER  BY c.ordinal_position 
";
            var ret = Execute(sql);
            ret.TableName = tableName;
            return ret;
        }
        public void GetClassForTable(string tableName, DirectoryInfo savePath, string nameSpaceName = null)
        {
            string ns = "DapperGenericRepository";
            string modelNS = string.Empty;
            string repositoryNS = string.Empty;
            string serviceNS = string.Empty;

            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                ns = nameSpaceName;
            }
            if (!ns.Contains(".Model", StringComparison.OrdinalIgnoreCase))
            { modelNS = ns + (".Model"); }
            repositoryNS = modelNS.Replace(".Model", ".Repository");
            serviceNS = modelNS.Replace(".Model", ".Service");
            var tbl = GetClassForTable(tableName);
            tbl.TableName = ConvertToPascalCase(tbl.TableName);
            StringBuilder Model = new StringBuilder();
            Model.AppendLine($@"                
                using System.ComponentModel.DataAnnotations;
                using System.ComponentModel.DataAnnotations.Schema;

                namespace {modelNS}
                {{
                    [Table(""{tbl.Rows[0]["Table_Name"]}"")]
                    public class {tbl.TableName}
                    {{" + Environment.NewLine);
            foreach (DataRow row in tbl.Rows)
            {
                //Model.AppendLine(row["code"].ToString());
                string key = row["iskey"].ToString().Equals("y", StringComparison.CurrentCultureIgnoreCase) ? "[Key]" : string.Empty;
                string colName = row["COLUMNNAME"].ToString();
                string datatype = row["nullable"].ToString();
                string prop = ConvertToPascalCase(colName);
                string dec = $@"{key}[Column(""{colName}"")]{datatype} {ConvertToPascalCase(colName)} {"{get;set;}"}";
                Model.AppendLine(dec);
            }
            Model.AppendLine($@"    }} }}");

            StringBuilder Repository = new StringBuilder();
            Repository.AppendLine($@"
                using DapperGenericRepository.Repository;
                using {modelNS};
                namespace {repositoryNS}
                                    {{ public class {tbl.TableName}Repository : GenericRepository<{tbl.TableName}>    {{ }}}}");

            StringBuilder Service = new StringBuilder();
            Service.AppendLine(GetServiceSkeleton(tbl.TableName, modelNS, repositoryNS, serviceNS));

            #region saveFiles
            FileInfo fileInfo = new FileInfo(Path.Combine(savePath.FullName, nameof(Model), tbl.TableName + ".cs"));
            if (!fileInfo.Directory.Exists) { fileInfo.Directory.Create(); }
            File.WriteAllText(fileInfo.FullName, Model.ToString());

            fileInfo = new FileInfo(Path.Combine(savePath.FullName, nameof(Repository), tbl.TableName + nameof(Repository) + ".cs"));
            if (!fileInfo.Directory.Exists) { fileInfo.Directory.Create(); }
            File.WriteAllText(fileInfo.FullName, Repository.ToString());

            fileInfo = new FileInfo(Path.Combine(savePath.FullName, nameof(Service), tbl.TableName + nameof(Service) + ".cs"));
            if (!fileInfo.Directory.Exists) { fileInfo.Directory.Create(); }
            File.WriteAllText(fileInfo.FullName, Service.ToString());
            #endregion

        }

        public string GetClassForQuery(string query, string classname)
        {
            string sql = query;
            DataTable dt = Execute(sql);
            dt.TableName = classname;
            return DataTableToCode(dt);
        }


        private DataTable Execute(string query)
        {
            var dataTable = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();

                    var _da = new MySqlDataAdapter(cmd);
                    _da.Fill(dataTable);
                    conn.Close();
                }
            }
            return dataTable;
        }
        private string? GetServiceSkeleton(string tableName, string modelNS, string repositoryNS, string serviceNS)
        {
            string ret = $@"using {modelNS};
using {repositoryNS};

namespace {serviceNS}
{{
    public class {tableName}Service
    {{
        public bool Add({tableName} {tableName})
        {{
            bool isAdded = false;
            try
            {{
                {tableName}Repository {tableName}Repository = new {tableName}Repository();
                isAdded = {tableName}Repository.Insert({tableName});
            }}
            catch (Exception ex)
            {{
            }}
            return isAdded;
        }}

        public List<{tableName}> GetAll()
        {{
            List<{tableName}> {tableName}s = new List<{tableName}>();
            try
            {{
                {tableName}Repository {tableName}Repository = new {tableName}Repository();
                {tableName}s = {tableName}Repository.GetAll().ToList();
            }}
            catch (Exception ex)
            {{
            }}

            return {tableName}s;
        }}

        public {tableName} Get(int Id)
        {{
            {tableName} {tableName} = new {tableName}();
            try
            {{
                {tableName}Repository {tableName}Repository = new {tableName}Repository();
                {tableName} = {tableName}Repository.GetById(Id);
            }}
            catch (Exception ex)
            {{
            }}

            return {tableName};
        }}

        public bool Update({tableName} {tableName})
        {{
            bool isUpdated = false;
            try
            {{
                {tableName}Repository {tableName}Repository = new {tableName}Repository();
                isUpdated = {tableName}Repository.Update({tableName});
            }}
            catch (Exception ex)
            {{
            }}

            return isUpdated;
        }}

        public bool Delete({tableName} {tableName})
        {{
            bool isDeleted = false;
            try
            {{
                {tableName}Repository {tableName}Repository = new {tableName}Repository();
                isDeleted = {tableName}Repository.Delete({tableName});
            }}
            catch (Exception ex)
            {{
            }}
            return isDeleted;
        }}
    }}
}}";
            return ret;
        }

        public static string ConvertToPascalCase(string snakeCase)
        {
            if (string.IsNullOrEmpty(snakeCase))
                return snakeCase;

            var words = snakeCase.Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0], CultureInfo.InvariantCulture) + words[i].Substring(1);
                }
            }
            return string.Join(string.Empty, words);
        }

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