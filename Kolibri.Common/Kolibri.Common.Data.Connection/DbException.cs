using System;
using System.Collections.Generic;

using System.Text;

namespace Kolibri.Data.Connection
{
    /// <summary>
    /// Klasse for håndtering av feil oppstått i DbConnection.
    /// Inneholder både exceptionet som har oppstått og hvilken type exception samt funksjonsnavn og evt kommentar som er lagt inn.	
    /// </summary>
    public class DbException
    {

        #region Private Members

        private Exception m_exception;
        private string m_functionName;
        private string m_comment;
        private System.Type m_exceptionType;

        #endregion

        #region Constructors
        public DbException()
        {
        }

        public DbException(Exception e)
        {
            m_exception = e;
            m_exceptionType = e.GetType();
        }

        public DbException(Exception e, string functionName)
        {
            m_exception = e;
            m_exceptionType = e.GetType();
            m_functionName = functionName;
        }

        public DbException(Exception e, string functionName, string comment)
        {
            m_exception = e;
            m_exceptionType = e.GetType();
            m_functionName = functionName;
            m_comment = comment;
        }
        #endregion

        #region Properties
        public Exception Exception { get { return m_exception; } }
        public System.Type Type { get { return m_exceptionType; } }
        public string FunctionName { get { return m_functionName; } }
        public string Comment { get { return m_comment; } }
        #endregion

        public override string ToString()
        {
            String ret = "";
            String feilmelding = m_exception.ToString();

            if (m_exceptionType == typeof(System.Data.SqlClient.SqlException))
            {
                if (feilmelding.IndexOf(m_exceptionType.ToString() + ": " + "Cannot insert duplicate key row in object") == 0)
                    ret = "Kan ikke legge inn flere rader med samme nøkkel.";
            }

            if (ret.Length == 0)
            {
                ret = feilmelding;
            }

            return ret;
        }
    }
}


