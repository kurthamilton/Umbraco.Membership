using System.Collections.Generic;
using System.Web.Mvc;
using ODK.Umbraco.Emails;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using ODK.Website.Models;
using Umbraco.Core.Models;

namespace ODK.Website.Controllers
{
    public class AdminController : OdkSurfaceControllerBase
    {
        private readonly OdkEmailService _emailService;
        private readonly EventService _eventService;
        private readonly OdkMemberService _memberService;

        public AdminController(OdkEmailService emailService, EventService eventService, OdkMemberService memberService)
        {
            _emailService = emailService;
            _eventService = eventService;
            _memberService = memberService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEventInvite(int eventId, MemberTypes[] memberTypes, EmailViewModel email)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            MemberSearchCriteria memberSearchCriteria = new MemberSearchCriteria(chapter.Id) { Types = memberTypes };
            IReadOnlyCollection<MemberModel> members = _memberService.GetMembers(memberSearchCriteria, Umbraco);

            _memberService.SendMemberEmails(members, email.Subject, email.Body);
            _eventService.LogSentEventInvite(eventId, Umbraco);

            AddFeedback($"{members.Count} invites sent", true);

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendTestEmail(string to, EmailViewModel email)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            _emailService.SendEmail(chapter, email.Subject, email.Body, new[] { to });

            return RedirectToCurrentUmbracoPage();
        }

        private ActionResult RedirectToHome()
        {
            IPublishedContent homePage = Umbraco.UmbracoContext.PublishedContentRequest.PublishedContent.HomePage();
            return RedirectToUmbracoPage(homePage.Id);
        }
    }
}