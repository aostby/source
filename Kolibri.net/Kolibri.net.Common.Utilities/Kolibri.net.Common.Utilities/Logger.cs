using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Kolibri.net.Common.Utilities
{
    public class Logger
        {
            public enum LoggType { Feil, Varsel, Informasjon };
            private static ArrayList m_loggListe;

            public static void Logg(LoggType type, string loggtekst)
            { /*
           * Ved å endre på rekkefølgen i arrayet her, må hele denne klassen sjekkes siden arrayet benyttes i alle metodene!
         */
                /*
             // 0   Bruker			
             // 1   LoggType		
             // 2   Tidspunkt	
             // 3   LoggTekst		
             // 4   Klassenavn		
             // 5   Funksjonsnavn	
             // 6   Versjon			
             // 7   TraceTekst		
             // 8   Filsti			
                    */
                try
                {
                    string[] tekstArr = new string[9];
                    CallingMethod cm = new CallingMethod(typeof(CallingMethod));

                    tekstArr[0] = Environment.UserName; //bruker
                    tekstArr[1] = type.ToString(); //Type logglinje  {Feil,Varsel,Informasjon};
                    tekstArr[2] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"); //tidspunkt for loggingen
                    if (string.IsNullOrEmpty(loggtekst))
                        tekstArr[3] = "";
                    else
                        tekstArr[3] = loggtekst;//Tekst som skal logges

                    tekstArr[4] = cm.TypeName;// 4   Klassenavn
                    tekstArr[5] = cm.Text; // 5   Funksjonsnavn
                    if (Assembly.GetEntryAssembly() == null)
                        tekstArr[6] = Assembly.GetCallingAssembly().GetName().Version.ToString();//Versjon
                    else
                        tekstArr[6] = Assembly.GetEntryAssembly().GetName().Version.ToString(); // 6   Versjon	
                    if (type == LoggType.Feil) // tracer bare feil.
                        tekstArr[7] = TracedStack();
                    else
                        tekstArr[7] = string.Empty;
                    tekstArr[8] = cm.FilePath;

                    if (m_loggListe == null)
                    {
                        m_loggListe = new ArrayList();
                    }
                    for (int i = 0; i < tekstArr.Length; i++)
                    {
                        if (tekstArr[i] != null)
                        {
                            tekstArr[i] = tekstArr[i].Replace("'", "");
                        }
                    }
                    Thread.Sleep(2); //Dersom vi logger for tett, får vi en feil ved lagring!
                    m_loggListe.Add(tekstArr);
                }
                catch (Exception ex)
                {

                }
            }
            public static string GetLastLogLine()
            {
                String ret = string.Empty;
                try
                {
                    if (m_loggListe != null && m_loggListe.Count > 0)
                    {
                        ret = ((string[])m_loggListe[m_loggListe.Count - 1])[3];
                    }
                }
                catch (Exception)
                { }
                return ret;
            }

            public static string GetLastErrorLogLine()
            {
                String ret = string.Empty;
                try
                {
                    if (m_loggListe != null && m_loggListe.Count > 0)
                    {
                        for (int i = m_loggListe.Count - 1; i >= 0; i--)
                        {
                            string[] temp = ((string[])m_loggListe[i]);
                            if (temp[1] != null && temp[3] != null && temp[1].Equals("Feil"))
                            {
                                ret = temp[3];
                                break;
                            }
                        }

                    }
                }
                catch (Exception)
                { }
                return ret;
            }

            public static DataSet GetLog()
            {
                DataSet ret = null;
                DataSet ds = null;
                DataTable regTabell = null;
                try
                {
                    ds = new DataSet();
                    if (m_loggListe != null && m_loggListe.Count > 0)
                    {

                        foreach (string[] info in m_loggListe)
                        {

                            if (regTabell == null)
                            {
                                regTabell = ds.Tables.Add("Logg");    //regTabell = ds.Tables.Add(System.Reflection.Assembly.GetAssembly(Assembly.GetCallingAssembly().GetType() as Type).GetName().Name);

                                //regTabell.Columns.Add(prop.Name);
                                regTabell.Columns.Add("Bruker");
                                regTabell.Columns.Add("LoggType");
                                regTabell.Columns.Add("Tidspunkt");
                                regTabell.Columns.Add("LoggTekst");

                                regTabell.Columns.Add("Klassenavn");
                                regTabell.Columns.Add("Funksjonsnavn");
                                regTabell.Columns.Add("Versjon");
                                regTabell.Columns.Add("TraceTekst");
                                regTabell.Columns.Add("Filsti");

                            }
                            DataRow row = regTabell.NewRow();
                            for (int i = 0; i < info.Length; i++)
                            {
                                row[i] = info[i];
                            }


                            regTabell.Rows.Add(row);
                        }
                    }
                }
                catch (Exception) { }
                ret = ds;
                return ret;
            }

            /// <summary>
            ///  Metode som tømmer loggen uten å skrive
            /// </summary>
            public static void ClearLog()
            {
                if (m_loggListe != null)
                    m_loggListe.Clear();
            }

            /// <summary>
            ////Metode som skriver logg til eventlog og tømmer loggen til slutt
            /// </summary>
            public static void WriteAllLogs()
            {
                WriteAllLogs(null);
            }

            /// <summary>
            /// Creates subdir Log, and appends loggtext for each date to logfile based on calling assembly name
            /// </summary>
            /// <param name="di">Directory o</param>
            public static void WriteLogToFile(System.IO.DirectoryInfo di)
            {
                try
                {
                    string ext = ".txt";

                    string filename = Path.Combine(di.FullName, "Log\\" + FileUtilities.SafeFileName(DateTime.Now.ToString("yyyy-MM-dd") + "_Log_" + Assembly.GetCallingAssembly().GetName().Name));
                    filename += ext;

                    string logg = DataSetUtilities.DataSetToCSV(Logger.GetLog(), ";");

                    FileInfo info = new FileInfo(filename);
                    DirectoryInfo dinfo = info.Directory;
                    if (!dinfo.Exists)
                        dinfo.Create();

                    using (StreamWriter outfile = new StreamWriter(info.FullName, true))
                    {
                        outfile.Write(logg);
                    }
                }
                catch (Exception)
                {
                }
            }


            /// <summary>
            ////Metode som skriver logg til eventlog, db systemlog og tømmer loggen til slutt
            /// </summary>
            /// <param name="dbConnection"></param>
            public static void WriteAllLogs(string dbConnection= null)
            {
                WriteEventLog();
                //WriteSystemLog(dbConnection);
                ClearLog();
            }

            private /* public*/ static void WriteEventLog()
            {
                if (m_loggListe != null && m_loggListe.Count > 0)
                {
                    string source = "Application";
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog();
                    myLog.Source = source;
                    for (int i = 0; i < m_loggListe.Count; i++)
                    {  // Write an informational entry to the event log.   
                        EventLogEntryType type = EventLogEntryType.Error;

                        switch ((LoggType)Enum.Parse(typeof(LoggType), ((string[])m_loggListe[i])[1]))
                        {
                            case LoggType.Feil:
                                type = EventLogEntryType.Error;
                                break;
                            case LoggType.Varsel:
                                type = EventLogEntryType.Warning;
                                break;
                            case LoggType.Informasjon:
                                type = EventLogEntryType.Information;
                                break;
                        }

                        try
                        {

                            myLog.WriteEntry(String.Join(" ; ", ((string[])m_loggListe[i])), type);
                        }
                        catch (Exception ex)
                        {
                        }


                    }
                }
            }
          
            private static string TracedStack()
            {

                int nStackCnt = 0;

                StringBuilder ret = new StringBuilder();

                StackTrace oStack = new StackTrace(true);

                nStackCnt = oStack.FrameCount;

                // I've decided to only show the last few items of the stack. 
                // They are displayed in order by most recent frame. 

                if (nStackCnt > 5) { nStackCnt = 5; }

                for (int i = 0; i < nStackCnt; i++)
                {
                    // The first frame (0) is always this method (SetOnError), 
                    // so we don't want to display it. 

                    if (i > 0)
                    {
                        try
                        {
                            ret.Append(oStack.GetFrame(i).GetMethod().ReflectedType.FullName + " ");
                            ret.Append(" " + oStack.GetFrame(i).GetMethod().ToString() + " ");
                        }
                        catch (Exception)
                        { }

                        // If it is the first frame we want to view, it will contain info from the 
                        // class/method that generated the error 

                        if (i == 1) { ret.Append(" "); }
                    }
                }

                // Flag our class to react in RedirectOnError when it is called. 

                //this.ErrorNumber = 2; 
                return ret.ToString();
            }


        }

    }
