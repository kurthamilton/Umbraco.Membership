using System.Collections.Generic;
using System.Net.Mail;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Emails
{
    public class OdkEmailService
    {
        public ServiceResult SendAdminEmail(string siteUrl, IPublishedContent chapter, string subject, string body)
        {
            string[] toAddresses = chapter.GetPropertyValue<string>("adminEmailAddresses").Split(',');

            return SendEmail(siteUrl, chapter, subject, body, toAddresses);
        }

        public ServiceResult SendEmail(string siteUrl, IPublishedContent chapter, string subject, string body, IEnumerable<string> toAddresses)
        {
            body = ReplaceBodyProperties(body, siteUrl);

            string fromAddress = chapter.GetPropertyValue<string>("emailFromAddress");

#if DEBUG
            return new ServiceResult(true, $"The following email would have been sent to {string.Join(", ", toAddresses)}: {body}");
#endif

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

                return new ServiceResult(true);
            }
        }

        private string ReplaceBodyProperties(string body, string siteUrl)
        {
            return body.Replace("http://{{siteUrl}}", siteUrl);
        }
    }
}
