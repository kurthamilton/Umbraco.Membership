using System.Collections.Generic;
using System.Linq;
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
        public ActionResult AddMemberGroup(string name)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            int chapterId = Umbraco.AssignedContentItem.HomePage().Id;
            _memberService.AddMemberGroup(chapterId, name);

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public ActionResult DeleteMemberGroup(int groupId)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            _memberService.DeleteMemberGroup(groupId);

            return RedirectToCurrentUmbracoPage();
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
        public ActionResult SendMemberEmail(string memberIdString, EmailViewModel email)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            int[] memberIds = memberIdString.Split(',').Select(x => int.Parse(x)).ToArray();
            IReadOnlyCollection<MemberModel> members = _memberService.GetMembers(new MemberSearchCriteria(chapter.Id), Umbraco);
            members = members.Where(x => memberIds.Contains(x.Id)).ToArray();

            _memberService.SendMemberEmails(members, email.Subject, email.Body);

            AddFeedback($"{members.Count} emails sent", true);

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

        [HttpPost]
        public void UpdateMemberGroups(int memberId, IEnumerable<int> groupIds)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return;
            }

            _memberService.UpdateMemberGroups(memberId, groupIds?.ToArray());
        }

        private ActionResult RedirectToHome()
        {
            IPublishedContent homePage = Umbraco.UmbracoContext.PublishedContentRequest.PublishedContent.HomePage();
            return RedirectToUmbracoPage(homePage.Id);
        }
    }
}