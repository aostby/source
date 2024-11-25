using System;
using System.Collections.Generic;
using System.Text;

namespace Kolibri.Data.Connection
{
    public class DbFelt
    {
        private string tableName;
        private string name;
        private Type type;
        private string length;
        private string offset;
        private string usertype;
        private string operand;
        private string valueSet;

        public string TableName { get { return tableName; } set { tableName = value; } }
        public string Name { get { return name; } set { name = value; } }
        public Type Type { get { return type; } set { type = value; } }
        public string Length { get { return length; } set { length = value; } }
        public string Offset { get { return offset; } set { offset = value; } }
        public string Usertype { get { return usertype; } set { usertype = value; } }
        public string Operand { get { return operand; } set { operand = value; } }
        public string Value { get { return valueSet; } set { valueSet = value; } }
    }
}
