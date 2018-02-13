using System.Net.Mail;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace ODK.Website.Controllers
{
    public class ContactController : SurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string message)
        {
            IPublishedContent content = Umbraco.AssignedContentItem;

            string contactEmailAddress = content.GetPropertyValue<string>("contactEmailAddress");

            string body =
                "A message has been sent through the drunkenknitwits.com website from " + email + "." +
                "Message:" +
                "" + message;

            MailMessage mailMessage = new MailMessage("noreply@drunkenknitwits.com", contactEmailAddress, "Website contact message", body);

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Send(mailMessage);
            }

            TempData["Feedback"] = "Message sent";

            return CurrentUmbracoPage();
        }
    }
}