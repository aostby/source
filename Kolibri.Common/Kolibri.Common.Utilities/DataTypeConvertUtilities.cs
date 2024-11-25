using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kolibri.Common.Utilities
{

    /// <summary>
    /// This class converts the .Net reflected Built-In Types ( passed as strings to different string codes ) 
    /// </summary>

    class DataTypeConvertUtilities
    {
        /// <summary>
        /// Converts the reflected .Net type passed as string to a C# built-in type as string
        /// </summary>
        /// <param name="strDotNetTypeString">The .Net type passed as string</param>
        /// <returns>the converted built-in C# type </returns>
        public static string FromDotNetTypeToCSharpType(string strDotNetTypeString)
        {

            string strConvertTo = strDotNetTypeString;

            #region DefineTheConvertedType

            //<source>http://msdn.microsoft.com/en-us/library/ya5y69ds.aspx</source>
            //<source>http://ysgitdiary.blogspot.com/2010/02/c-types-vs-net-types-code-generation.html</source>
            switch (strDotNetTypeString)
            {
                case "System.Boolean":
                    strConvertTo = "bool";
                    break;

                case "System.Byte":
                    strConvertTo = "byte";
                    break;

                case "System.SByte":
                    strConvertTo = "sbyte";
                    break;

                case "System.Char":
                    strConvertTo = "char";
                    break;

                case "System.Decimal":
                    strConvertTo = "decimal";
                    break;

                case "System.Double":
                    strConvertTo = "double";
                    break;

                case "System.Single":
                    strConvertTo = "float";
                    break;

                case "System.Int32":
                    strConvertTo = "int";
                    break;

                case "System.UInt32":
                    strConvertTo = "uint";
                    break;

                case "System.Int64":
                    strConvertTo = "long";
                    break;

                case "System.UInt64":
                    strConvertTo = "ulong";
                    break;

                case "System.Object":
                    strConvertTo = "object";
                    break;

                case "System.Int16":
                    strConvertTo = "short";
                    break;

                case "System.UInt16":
                    strConvertTo = "ushort";
                    break;

                case "System.String":
                    strConvertTo = "string";
                    break;

                case "System.Nullable`1[System.Boolean]":
                    strConvertTo = "bool?";
                    break;

                case "System.Nullable`1[System.Byte]":
                    strConvertTo = "byte?";
                    break;

                case "System.Nullable`1[System.SByte]":
                    strConvertTo = "sbyte?";
                    break;

                case "System.Nullable`1[System.Char]":
                    strConvertTo = "char?";
                    break;

                case "System.Nullable`1[System.Decimal]":
                    strConvertTo = "decimal?";
                    break;

                case "System.Nullable`1[System.Double]":
                    strConvertTo = "double?";
                    break;

                case "System.Nullable`1[System.Single]":
                    strConvertTo = "float?";
                    break;

                case "System.Nullable`1[System.Int32]":
                    strConvertTo = "int?";
                    break;

                case "System.Nullable`1[System.UInt32]":
                    strConvertTo = "uint?";
                    break;

                case "System.Nullable`1[System.Int64]":
                    strConvertTo = "long?";
                    break;

                case "System.Nullable`1[System.UInt64]":
                    strConvertTo = "ulong?";
                    break;

                case "System.Nullable`1[System.Object]":
                    strConvertTo = "object?";
                    break;

                case "System.Nullable`1[System.Int16]":
                    strConvertTo = "short?";
                    break;

                case "System.Nullable`1[System.UInt16]":
                    strConvertTo = "ushort?";
                    break;

                case "System.Nullable`1[System.String]":
                    strConvertTo = "string?";
                    break;

            }//eof switch 

            return strConvertTo;

            #endregion DefineTheConvertedType
        } //eof method 

        /// <summary>
        /// Convertes a .Net reflected Build-In type passed as string to a string code to convert this type in 
        /// to basic Convert.ToTypeOfType sting code
        /// </summary>
        /// <param name="strDotNetTypeString">the basic built-in type as string </param>
        /// <returns>the basic built-in type </returns>
        public static string FromDotNetTypeToConvertToDotNetTypeCode(string strDotNetTypeString)
        {

            string strConvertTo = strDotNetTypeString;

            #region DefineTheConvertedType

            //<source>http://msdn.microsoft.com/en-us/library/ya5y69ds.aspx</source>
            //<source>http://ysgitdiary.blogspot.com/2010/02/c-types-vs-net-types-code-generation.html</source>
            switch (strDotNetTypeString)
            {
                case "System.Boolean":
                    strConvertTo = "Convert.ToBoolean";
                    break;

                case "System.Byte":
                    strConvertTo = "Convert.ToByte";
                    break;

                case "System.SByte":
                    strConvertTo = "Convert.ToSByte";
                    break;

                case "System.Char":
                    strConvertTo = "Convert.ToChar";
                    break;

                case "System.Decimal":
                    strConvertTo = "Convert.ToDecimal";
                    break;

                case "System.Double":
                    strConvertTo = "Convert.ToDouble";
                    break;

                case "System.Single":
                    strConvertTo = "Convert.ToSingle";
                    break;

                case "System.Int32":
                    strConvertTo = "Convert.ToInt32";
                    break;

                case "System.UInt32":
                    strConvertTo = "Convert.ToUInt32";
                    break;

                case "System.Int64":
                    strConvertTo = "Convert.ToInt64";
                    break;

                case "System.UInt64":
                    strConvertTo = "Convert.ToUInt64";
                    break;

                case "System.Object":
                    strConvertTo = "Convert.ToObject";
                    break;

                case "System.Int16":
                    strConvertTo = "Convert.ToInt16";
                    break;

                case "System.UInt16":
                    strConvertTo = "Convert.ToUint16";
                    break;

                case "System.String":
                    strConvertTo = "Convert.ToString";
                    break;

                case "System.Nullable`1[System.Boolean]":
                    strConvertTo = "Convert.ToBool";
                    break;

                case "System.Nullable`1[System.Byte]":
                    strConvertTo = "Convert.ToByte";
                    break;

                case "System.Nullable`1[System.SByte]":
                    strConvertTo = "Convert.ToSByte";
                    break;

                case "System.Nullable`1[System.Char]":
                    strConvertTo = "Convert.ToChar";
                    break;

                case "System.Nullable`1[System.Decimal]":
                    strConvertTo = "Convert.ToDecimal";
                    break;

                case "System.Nullable`1[System.Double]":
                    strConvertTo = "Convert.ToDouble";
                    break;

                case "System.Nullable`1[System.Single]":
                    strConvertTo = "Convert.ToSingle";
                    break;

                case "System.Nullable`1[System.Int32]":
                    strConvertTo = "Convert.ToInt32";
                    break;

                case "System.Nullable`1[System.UInt32]":
                    strConvertTo = "Convert.ToUInt32";
                    break;

                case "System.Nullable`1[System.Int64]":
                    strConvertTo = "Convert.ToInt64";
                    break;

                case "System.Nullable`1[System.UInt64]":
                    strConvertTo = "Convert.ToUInt64";
                    break;

                case "System.Nullable`1[System.Object]":
                    strConvertTo = "Convert.ToObject";
                    break;

                case "System.Nullable`1[System.Int16]":
                    strConvertTo = "Convert.ToInt16";
                    break;

                case "System.Nullable`1[System.UInt16]":
                    strConvertTo = "Convert.ToUInt16";
                    break;

                case "System.Nullable`1[System.String]":
                    strConvertTo = "Convert.ToString";
                    break;

            }//eof switch 

            return strConvertTo;

            #endregion DefineTheConvertedType
        } //eof method 

    } //eof class 
} //eof 
 



