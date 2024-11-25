using System;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;

namespace MoviesFromImdb
{
    public partial class EmailForm : Form
    {
        public EmailForm()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbSendEmail.Text))
            {
                MessageBox.Show("Please enter friend's email!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("youremail@gmail.com");

                try
                {
                    mail.To.Add(tbSendEmail.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("E-mail address: " + tbSendEmail.Text + " is not in the form required for an e-mail address.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                mail.Subject = "My watchlist";
                mail.Body = "Look at my movies watchlist in the attachment";


                if (!File.Exists(@"C:\Users\your\Documents\My_Watchlist.xls"))
                {
                    MessageBox.Show("Please make wishlist excel file first!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Attachment attachment = new Attachment(@"C:\Users\your\Documents\My_Watchlist.xls");
                mail.Attachments.Add(attachment);


                try
                {
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("youremail@gmail.com", "yourpass");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
                catch (Exception)
                {
                    MessageBox.Show("E-mail not sent! Notice: It is possible that you need to enable access to third-party sites&&apps on your email account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                MessageBox.Show("E-mail successfully sent to address: " + tbSendEmail.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            finally
            {

                tbSendEmail.Clear();
                tbSendEmail.Focus();

            }


        }

        private void EmailForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) btnSubmit.PerformClick();
        }
    }
}
