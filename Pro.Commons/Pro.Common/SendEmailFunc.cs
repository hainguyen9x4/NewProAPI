using System.Net;
using System.Net.Mail;

namespace Pro.Common
{
    public static class SendEmailFunc
    {
        public static void SendEmail(string strMessage, string subject = "ERROR")
        {
            try
            {
                string fromMail = "hainguyen9x7@gmail.com";
                string fromPassword = "pnfcewyxjydmdxwc";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = subject + DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss tt");
                message.To.Add(new MailAddress("taikhoancv1@gmail.com"));
                message.Body = $"<html><body>{strMessage}</body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };

                smtpClient.SendAsync(message, null);
            }
            catch (Exception ex)
            {
                LogHelper.Error("SendEmail", ex);
            }
        }
    }
}
