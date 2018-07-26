using System.Web.Mvc;
using ODK.Umbraco.Emails;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using Umbraco.Core.Models;

namespace ODK.Website.Controllers
{
    public class AdminController : OdkSurfaceControllerBase
    {
        private readonly OdkEmailService _emailService = new OdkEmailService();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendTestEmail(string subject, string body)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            _emailService.SendAdminEmail(chapter, subject, body);

            return RedirectToCurrentUmbracoPage();
        }

        private ActionResult RedirectToHome()
        {
            IPublishedContent homePage = Umbraco.UmbracoContext.PublishedContentRequest.PublishedContent.HomePage();
            return RedirectToUmbracoPage(homePage.Id);
        }
    }
}