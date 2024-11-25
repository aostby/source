using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
 
//using Maritech;
//using Maritech.DBClient;
namespace Kolibri.Common.Utilities
{
    /// <summary>
    /// Klasse for sending av mail vha SMTP. NB! Må initialiseres med "SetMailSettings"
    /// </summary>
    public class SMTPMail
    {
        internal static string SmtpServerName = string.Empty;
        internal static int SmtpServerPort = 25;
        internal static bool SmtpEnableSSL = false;
        internal static string SmtpUserName = string.Empty;
        internal static string SmtpPassword = string.Empty;
        
        /// <summary>
        /// Sett parametere for mailsending (SMTP oppsett)
        /// </summary>
        /// <param name="ServerName">server, f.eks smtp.gmail.com</param>
        /// <param name="ServerPort">port, f.eks 587</param>
        /// <param name="EnableSSL">Vanligst at denne er 'true'</param>
        /// <param name="UserName">brukernavn for pålogging server</param>
        /// <param name="Password">passord for pålogging server</param>
        public static void SetMailSettings(string ServerName, int ServerPort, bool EnableSSL,
           string UserName, string Password)
        {
            SmtpServerName = ServerName;
            SmtpServerPort = ServerPort;
            SmtpEnableSSL = EnableSSL;
            SmtpUserName = UserName;
            SmtpPassword = Password;

        }
        
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="to">One or more recipients, separated with semi-colon or comma</param>
        /// <param name="subject">Subject of mail</param>
        /// <param name="body">Mail body</param>
        public static bool SendMail(string to, string subject, string body)
        {
            bool ret = true;
            try
            {

                ret= SendMail(null, to, subject, body, new List<object>());

            }
            catch (Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message + " " + ex.InnerException);
                ret = false;
            }
            return ret;
        }
        
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to">One or more recipients, separated with semi-colon or comma</param>
        /// <param name="subject">Subject of mail</param>
        /// <param name="body">Mail body</param>
        /// <param name="attatchment">semikolon eller kommaseparert liste med vedlegg</param>
        /// <returns></returns>
        public static bool SendMail(string from, string to, string subject, string body, string attatchment)
        {
            bool ret = true;
            try
            {

                if (string.IsNullOrEmpty(attatchment))
                    ret = SendMail(from, to, subject, body, new List<object>());
                else
                {
                    object[] obj;
                    if(attatchment.Contains(","))
                        obj = attatchment.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    else
                        obj = attatchment.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    ret = SendMail(from, to, subject, body, obj.ToList());
                }


            }
            catch (Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message + " " + ex.InnerException);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to">One or more recipients, separated with semi-colon or comma</param>
        /// <param name="subject">Subject of mail</param>
        /// <param name="body">Mail body</param>
        /// <param name="attatchment">semikolon eller kommaseparert liste med vedlegg</param>
        /// <returns></returns>
        public static bool SendMail(string from, string to, string subject, string body, List<object> attachments)
        {
            bool ret = true;
            if (SmtpServerName == string.Empty || SmtpServerPort == 0)
                return false;

            string toMail = to.Replace(",", ";");

            MailMessage mail = new MailMessage();

            if (!string.IsNullOrEmpty(from))
                mail.From = new MailAddress(from);

            foreach (string email in toMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (email.Trim() == string.Empty)
                    continue;

                if (mail.From == null)
                    mail.From = new MailAddress(email.Trim());
                mail.To.Add(new MailAddress(email.Trim()));
            }
            mail.Subject = subject;
            mail.Body = body;

            Stream contentStream = null;
            if (attachments != null)
            {
                for (int i = 0; i < attachments.Count; i++)
                {
                 

                    if (attachments[i].GetType() == typeof(string))
                        contentStream = new FileStream(attachments[i].ToString(), FileMode.Open);
                    else if (attachments[i].GetType() == typeof(byte[]))
                        contentStream = new MemoryStream((byte[])attachments[i]);
                    else
                        throw new Exception(string.Format("Unknown attachment format: {0}", attachments[i].GetType()));

                    string filename = attachments[i].ToString();
                    mail.Attachments.Add(new Attachment(contentStream, new FileInfo(filename).Name));
                   
                }
            }

            SmtpClient smtp = new SmtpClient(SmtpServerName, SmtpServerPort);
            smtp.EnableSsl = SmtpEnableSSL;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (SmtpUserName != string.Empty && SmtpPassword != string.Empty)
                smtp.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);
            
                smtp.Send(mail);
                if (contentStream != null)
                {
                    try
                    { contentStream.Close(); }
                    catch (Exception ex) 
                    {
                    }
                }
                return ret;
        }

             
    }
}
