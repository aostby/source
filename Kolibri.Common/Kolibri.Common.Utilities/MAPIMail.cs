using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using System.IO;

namespace Kolibri.Common.Utilities
{
    public class MAPIMail
    {
        /// <summary>
        /// Metode som bruker MAPI (Outlook) til å sende mail
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attatchment"></param>
        /// <returns></returns>
        public static bool SendMail(string to, string from, string cc, string bcc, string subject, string message, string attatchment)
        {
            bool ret = true;
            FileInfo fi;

            try
            {
                string toMail = to.Replace(",", ";");
                string fromMail = from;
                string ccMail;
                if (string.IsNullOrEmpty(cc))
                    ccMail = string.Empty;
                else
                    ccMail = cc.Replace(",", ";");
                string bccMail;
                if (string.IsNullOrEmpty(bcc))
                    bccMail = string.Empty;
                else
                    bccMail = bcc.Replace(",", ";");
                string attatchmentMail;
                if (string.IsNullOrEmpty(attatchment))
                    attatchmentMail = string.Empty;
                else
                    attatchmentMail = attatchment.Replace(",", ";");


                //Create the object of ms-Microsoft.Office.Interop.Outlook.

                Microsoft.Office.Interop.Outlook.Application objOutlookApp = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook._MailItem objMail = default(Microsoft.Office.Interop.Outlook._MailItem);

                //Outlook._NameSpace oNS = (Outlook._NameSpace)oApp.GetNamespace("MAPI");
                //oNS.Logon(Missing.Value, Missing.Value, true, true); 

                objMail = (MailItem)objOutlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                objMail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                //Subject of the mail
                objMail.Subject = subject;
                //Content of the mail 
                objMail.HTMLBody = message;
                //The target mail id.
                objMail.To = toMail; ;
                objMail.BCC = bccMail;
                objMail.CC = ccMail;
                if (!string.IsNullOrEmpty(attatchmentMail))
                {
                    string[] attachamenta = attatchmentMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in attachamenta)
                    {

                        {

                            //Add the attachment if file is present in the specified path.

                            if (File.Exists(item))
                            {
                                string path = item;
                                int attachType = (int)Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue;
                                fi = new FileInfo(path);
                                string displayName = fi.Name;
                                objMail.Attachments.Add(path, attachType, 1, displayName);
                            }
                        }
                    }
                }

                //The person receving this mail should not send receipt 
                //about receiving the mail.For this ReadReceiptRequested 
                //is set to false. 
                objMail.ReadReceiptRequested = false;

                //******************************************************************
                /*Diverse ang Security beskeden:
                 http://www.add-in-express.com/docs/outlook-security-manager-automate.php
                 http://www.add-in-express.com/outlook-security/object-model-guard.php                  
                 */

                //Send the mail.                 
                objMail.Send();
                //Destroy the object of the mail.
                objMail = null;


            }
            catch (System.Exception ex)
            {
                Logger.Logg(Logger.LoggType.Feil, ex.Message + " " + ex.InnerException);
                ret = false;
            }
            return ret;

        }

        public static bool test()
        {



            ////     public static string SendMail(string server, string from, string to, string cc, string msg, string subject)
            ////{
            //try
            //{
            //    if (server.Length == 0)
            //    {
            //        server = Config.GetConfig("MailServer").Value;
            //        if (server.Length == 0)
            //            return "E-post server ikke angitt!";
            //    }

            //    System.Net.Mail.MailAddressCollection toAdr = new System.Net.Mail.MailAddressCollection();
            //    toAdr.Add(to);
            //    if (cc.Length != 0)
            //        toAdr.Add(cc);
            //    //System.Net.Mail.SmtpStatusCode = System.Net.Mail.SmtpStatusCode.

            //    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, to, subject, msg);
            //    System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient(server);
            //    mail.IsBodyHtml = false;
            //    mail.Body = msg;

            //    mailClient.Send(mail);

            //    return "";
            //}
            //catch (Exception e)
            //{
            //    return "Feil ved sending av e-post: " + e.Message;
            //}

            return false;


        }
    }
}
