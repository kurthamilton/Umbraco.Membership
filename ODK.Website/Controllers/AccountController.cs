using System.Web.Mvc;
using ODK.Umbraco;
using ODK.Umbraco.Members;
using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace ODK.Website.Controllers
{
    public class AccountController : SurfaceController
    {
        private readonly MemberService _memberService;

        public AccountController()
        {
            _memberService = new MemberService(Services.MemberService, Umbraco);
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

            ModelState.AddModelError("", "The username or password provided is incorrect.");

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
                return OnError(model);
            }

            model.ChapterId = Umbraco.AssignedContentItem.HomePageSettings().Content.Id;

            ServiceResult result = _memberService.Register(model);
            if (!result.Success)
            {
                return OnError(model);
            }

            Umbraco.MembershipHelper.Login(model.Email, model.Password);

            return RedirectToChapter(model.ChapterId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MemberModel model)
        {
            IPublishedContent member = Umbraco.MembershipHelper.GetCurrentMember();
            if (member == null)
            {
                return RedirectToHome();
            }

            ServiceResult result = _memberService.Update(member.Id, model);
            if (!result.Success)
            {
                return OnError(model);
            }

            return RedirectToCurrentUmbracoPage();
        }

        private ActionResult OnError(object model)
        {
            TempData["Model"] = model;
            return CurrentUmbracoPage();
        }

        private void HandleLoggedOnUser()
        {
            if (Umbraco.MemberIsLoggedOn())
            {
                MemberModel member = _memberService.GetMember(Umbraco.MembershipHelper.CurrentUserName);
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
            HomePageSettings homePageSettings = Umbraco.UmbracoContext.PublishedContentRequest.PublishedContent.HomePageSettings();
            return RedirectToUmbracoPage(homePageSettings.Content.Id);
        }
    }
}