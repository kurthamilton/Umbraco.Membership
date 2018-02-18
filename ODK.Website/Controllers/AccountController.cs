using System.Web;
using System.Web.Mvc;
using ODK.Umbraco;
using ODK.Umbraco.Members;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using Umbraco.Core.Models;

namespace ODK.Website.Controllers
{
    public class AccountController : OdkSurfaceControllerBase
    {
        private readonly OdkMemberService _memberService;

        public AccountController()
        {
            _memberService = new OdkMemberService(Umbraco.MembershipHelper.GetCurrentMember(), Services.MediaService, Services.MemberService, Umbraco);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            HandleLoggedOnUser();

            if (ModelState.IsValid)
            {
                if (Umbraco.MembershipHelper.Login(model.Email, model.Password))
                {
                    return RedirectToCurrentUmbracoPage();
                }
            }

            SetModel(model);

            return CurrentUmbracoPage();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Umbraco.MembershipHelper.Logout();
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterMemberModel model)
        {
            HandleLoggedOnUser();

            if (!ModelState.IsValid)
            {
                return OnError(model, null);
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();
            model.SetChapter(chapter);

            ServiceResult result = _memberService.Register(model);
            if (!result.Success)
            {
                return OnError(model, result);
            }

            Umbraco.MembershipHelper.Login(model.Email, model.Password);

            AddFeedback("Welcome!", true);

            return RedirectToChapter(model.ChapterId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UpdateMemberModel model)
        {
            IPublishedContent member = Umbraco.MembershipHelper.GetCurrentMember();
            if (member == null)
            {
                return RedirectToHome();
            }

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();
            model.SetChapter(chapter);

            ServiceResult result = _memberService.Update(member.Id, model);
            if (!result.Success)
            {
                return OnError(model, result);
            }

            AddFeedback("Profile updated", true);

            return RedirectToCurrentUmbracoPage();
        }

        private ActionResult OnError(object model, ServiceResult result)
        {
            if (result != null)
            {
                foreach (string key in result.Errors.Keys)
                {
                    ModelState.AddModelError(key, result.Errors[key]);
                }
            }

            SetModel(model);
            return CurrentUmbracoPage();
        }

        private void HandleLoggedOnUser()
        {
            if (Umbraco.MemberIsLoggedOn())
            {
                MemberModel member = _memberService.GetMember(Umbraco.MembershipHelper.GetCurrentMemberId());
                RedirectToChapter(member?.ChapterId);
            }
        }

        private ActionResult RedirectToChapter(int? chapterId)
        {
            return chapterId > 0
                    ? RedirectToUmbracoPage(chapterId.Value)
                    : RedirectToHome();
        }

        private ActionResult RedirectToHome()
        {
            IPublishedContent homePage = Umbraco.UmbracoContext.PublishedContentRequest.PublishedContent.HomePage();
            return RedirectToUmbracoPage(homePage.Id);
        }
    }
}