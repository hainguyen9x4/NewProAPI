using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace CreateMail1s
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Take_Click(object sender, EventArgs e)
        {
            var number = 0;
            var mailHost = "@mail1s.edu.vn";
            var prefix = "h";
            if (int.TryParse(txtNumber.Text, out number))
            {
                txtMail.Text = prefix + number.ToString("D3") + mailHost;
                Clipboard.SetText(txtMail.Text);
                number++;
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendEmail("Test Errormessage");
        }
        private void SendEmail(string strMessage, string subject="ERROR")
        {
            string fromMail = "hainguyen9x7@gmail.com";
            string fromPassword = "pnfcewyxjydmdxwc";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress("taikhoancv1@gmail.com"));
            message.Body = $"<html><body>{strMessage}</body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}