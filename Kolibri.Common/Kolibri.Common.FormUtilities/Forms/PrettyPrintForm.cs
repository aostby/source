using Kolibri.Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class PrettyPrintForm : Form
    {
        public PrettyPrintForm()
        {
            InitializeComponent();
        }

        public PrettyPrintForm(FileInfo info)
        {
            InitializeComponent();
            try
            {
                if (info.Exists)
                {
                    string temp = FileUtilities.ReadTextFile(info.FullName);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        textBoxREQUEST.Text = temp;
                        PrettyPrintForm_KeyDown(null, new KeyEventArgs(Keys.F5));
                    }
                }
            }
            catch (Exception ex)
            {
                textBoxREQUEST.Text += Environment.NewLine + ex.Message;
            }
        }

        public PrettyPrintForm(string text)
        {
            InitializeComponent();
            try
            {

                string temp = text;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        textBoxREQUEST.Text = temp;
                        PrettyPrintForm_KeyDown(null, new KeyEventArgs(Keys.F5));
                    }
                 
            }
            catch (Exception ex)
            {
                textBoxREQUEST.Text += Environment.NewLine + ex.Message;
            }
        }




        private void PrettyPrintForm_KeyDown(object sender, KeyEventArgs e)
        {
            string jalla = " true"; bool bHandled = false;
            Keys k = e.KeyCode;
            // switch case is the easy way, a hash or map would be better, 
            // but more work to get set up.
            switch (k)
            {
                case Keys.F5:

                    string formatted = string.Empty;
                    // do whatever
                    if (textBoxREQUEST.Text.Trim().Contains("<"))
                        try
                        {
                            formatted = textBoxREQUEST.Text.Replace("This XML file does not appear to have any style information associated with it. The document tree is shown below.", string.Empty).Trim();
                            formatted = XDocument.Parse(formatted.Trim()).ToString();
                           
                            textBoxREQUEST.Language = FastColoredTextBoxNS.Language.XML;
                        }
                        catch (Exception)
                        {
                        }
                    if (!!string.IsNullOrEmpty(formatted) && textBoxREQUEST.Text.Contains("{"))
                        try
                        {
                            //formatted = JsonConvert.SerializeObject(textBoxREQUEST.Text, Formatting.Indented);
                          formatted=  JsonFormatter.PrettyPrint(textBoxREQUEST.Text);
                            textBoxREQUEST.Language = FastColoredTextBoxNS.Language. JS;
                        }
                        catch (Exception)
                        { }

                    if (!string.IsNullOrEmpty(formatted))
                        textBoxREQUEST.Text = formatted;
                    bHandled = true;
                    break;
                default:
                    bHandled = true;
                    break;
            }
        }
    }


    public static  class JsonFormatter
    {
        public static string Indent = "    ";

        public static string PrettyPrint(string input)
        {
            var output = new StringBuilder(input.Length * 2);
            char? quote = null;
            int depth = 0;

            for (int i = 0; i < input.Length; ++i)
            {
                char ch = input[i];

                switch (ch)
                {
                    case '{':
                    case '[':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(++depth));
                        }
                        break;
                    case '}':
                    case ']':
                        if (quote.HasValue)
                            output.Append(ch);
                        else
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(--depth));
                            output.Append(ch);
                        }
                        break;
                    case '"':
                    case '\'':
                        output.Append(ch);
                        if (quote.HasValue)
                        {
                            if (!output.IsEscaped(i))
                                quote = null;
                        }
                        else quote = ch;
                        break;
                    case ',':
                        output.Append(ch);
                        if (!quote.HasValue)
                        {
                            output.AppendLine();
                            output.Append(Indent.Repeat(depth));
                        }
                        break;
                    case ':':
                        if (quote.HasValue) output.Append(ch);
                        else output.Append(" : ");
                        break;
                    default:
                        if (quote.HasValue || !char.IsWhiteSpace(ch))
                            output.Append(ch);
                        break;
                }
            }

            return output.ToString();
        }



        public static string Repeat(this string str, int count)
        {
            return new StringBuilder().Insert(0, str, count).ToString();
        }

        public static bool IsEscaped(this string str, int index)
        {
            bool escaped = false;
            while (index > 0 && str[--index] == '\\') escaped = !escaped;
            return escaped;
        }

        public static bool IsEscaped(this StringBuilder str, int index)
        {
            return str.ToString().IsEscaped(index);
        }
    }
}
 
