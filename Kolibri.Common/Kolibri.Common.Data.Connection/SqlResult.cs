using System;

namespace Kolibri.Data.Connection
{

    [Serializable]
    public class SqlResult
    {
        public SqlResultRow[] Rows = null;

        public SqlResult()
        {

        }

        public int Count { get { if (this.Rows == null) return 0; else	return this.Rows.GetUpperBound(0) + 1; } }
    }

    [Serializable]
    public class SqlField
    {
        private string _name = "";
        private string _value = "";

        public SqlField()
        {

        }

        public bool IsDbNull()
        {
            if (_value == "" || _value == null)
                return true;
            else
                return false;
        }

        public string Name { get { return _name; } set { _name = value; } }
        public string Value { get { return _value; } set { _value = value; } }
    }

    [Serializable]
    public class SqlResultRow
    {
        public SqlField[] Fields = null;

        public SqlResultRow()
        {
        }

        public int Count { get { return this.Fields.GetUpperBound(0) + 1; } }

        public string[] FieldsToStringArray()
        {

            string[] ret = new string[Fields.GetUpperBound(0) + 1];

            try
            {
                for (int i = 0; i < Fields.GetUpperBound(0) + 1; i++)
                {
                    ret[i] = Fields[i].Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }
    }
}
