using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
   using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Text;
    using System.Windows.Forms;


namespace Kolibri.net.Common.Utilities
{

    public class XMLValidator
    {
        // Validation Error Count
        static int ErrorsCount = 0;

        // Validation Error Message
        static string ErrorMessage = "";

        public static void ValidationHandler(object sender,
                                             ValidationEventArgs args)
        {
            ErrorMessage = ErrorMessage + args.Message + "\r\n";
            ErrorsCount++;
        }

        public void Validate(string strXMLDoc, FileInfo schema)
        {
            ErrorsCount = 0;
            ErrorMessage = string.Empty;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;

                // Declare local objects
                XmlTextReader tr = null;
                XmlSchemaCollection xsc = null;
                XmlValidatingReader vr = null;

                // Text reader object
                tr = new XmlTextReader(schema.FullName);
                //xsc = new XmlSchemaCollection();
                //xsc.Add(null, tr);

                // XML validator object

                vr = new XmlValidatingReader(strXMLDoc,
                             XmlNodeType.Document, null);

                vr.Schemas.Add(null, schema.FullName);// //xsc);  

                // Add validation event handler

                vr.ValidationType = ValidationType.Schema;
                vr.ValidationEventHandler +=
                         new ValidationEventHandler(ValidationHandler);

                // Validate XML data

                while (vr.Read()) ;

                vr.Close();

                // Raise exception, if XML validation fails
                if (ErrorsCount > 0)
                {
                    throw new Exception(ErrorMessage);
                }

                // XML Validation succeeded
                Console.WriteLine("XML validation succeeded.\r\n");
            }
            catch (Exception error)
            {
                // XML Validation failed
                Console.WriteLine("XML validation failed." + "\r\n" +
                "Error Message: " + error.Message);

                throw error;
            }
        }


        [Obsolete("En må oppgi sti til skjema for validering", true)]
        public void Validate(string strXMLDoc)
        {
            ErrorsCount = 0;
            ErrorMessage = string.Empty;
            try
            {
                // Declare local objects
                XmlTextReader tr = null;
                XmlSchemaCollection xsc = null;
                XmlValidatingReader vr = null;

                // Text reader object
                tr = new XmlTextReader(Path.Combine(Application.StartupPath, "Schemas", "Rbd_ArticleMd_Response.xsd"));
                xsc = new XmlSchemaCollection();
                xsc.Add(null, tr);

                // XML validator object

                vr = new XmlValidatingReader(strXMLDoc,
                             XmlNodeType.Document, null);

                vr.Schemas.Add(xsc);

                // Add validation event handler

                vr.ValidationType = ValidationType.Schema;
                vr.ValidationEventHandler +=
                         new ValidationEventHandler(ValidationHandler);

                // Validate XML data

                while (vr.Read()) ;

                vr.Close();

                // Raise exception, if XML validation fails
                if (ErrorsCount > 0)
                {
                    throw new Exception(ErrorMessage);
                }

                // XML Validation succeeded
                Console.WriteLine("XML validation succeeded.\r\n");
            }
            catch (Exception error)
            {
                // XML Validation failed
                Console.WriteLine("XML validation failed." + "\r\n" +
                "Error Message: " + error.Message);

                throw error;
            }
        }
    }
}