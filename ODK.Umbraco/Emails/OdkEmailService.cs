using System.Collections.Generic;
using System.Net.Mail;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Emails
{
    public class OdkEmailService
    {
        public void SendAdminEmail(IPublishedContent chapter, string subject, string body)
        {
            string[] toAddresses = chapter.GetPropertyValue<string>("adminEmailAddresses").Split(',');

            SendEmail(chapter, subject, body, toAddresses);
        }

        public void SendEmail(IPublishedContent chapter, string subject, string body, IEnumerable<string> toAddresses)
        {
            string fromAddress = chapter.GetPropertyValue<string>("emailFromAddress");
            using (SmtpClient client = new SmtpClient())
            {
                MailMessage message = new MailMessage
                {
                    Body = body,
                    From = new MailAddress(fromAddress),
                    IsBodyHtml = true,
                    Subject = subject
                };

                foreach (string address in toAddresses)
                {
                    message.To.Add(address);
                }

                client.Send(message);
            }
        }
    }
}
