using System.Web.Mvc;
using ODK.Umbraco;
using ODK.Umbraco.Membership;
using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using ODK.Website.ViewModels.Account;

namespace ODK.Website.Controllers
{
    public class AccountController : SurfaceController
    {
        private readonly MembershipService _membershipService;

        public AccountController()
        {
            _membershipService = new MembershipService(Umbraco.MembershipHelper);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel)
        {
            HandleLoggedOnUser();

            if (ModelState.IsValid)
            {
                if (Umbraco.MembershipHelper.Login(viewModel.Email, viewModel.Password))
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
        public ActionResult Register(RegisterMember member)
        {
            HandleLoggedOnUser();

            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            member.ChapterId = Umbraco.AssignedContentItem.HomePageSettings().Content.Id;

            ServiceResult result = _membershipService.Register(member);

            if (!result.Success)
            {
                return CurrentUmbracoPage();
            }

            return RedirectToRoute(new { Controller = "Account", Action = nameof(SetChapter), id = member.ChapterId });
        }

        [HttpGet]
        public ActionResult SetChapter(int id)
        {
            IPublishedContent chapterContent = Umbraco.ContentQuery.TypedContent(id);

            _membershipService.UpdateChapter(chapterContent);

            return RedirectToChapter(id);
        }

        private void HandleLoggedOnUser()
        {
            if (Umbraco.MemberIsLoggedOn())
            {
                int? chapterId = _membershipService.GetChapterId(Umbraco.MembershipHelper.CurrentUserName);
                RedirectToChapter(chapterId);
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