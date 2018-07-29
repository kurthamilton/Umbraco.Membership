using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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

        public AccountController(OdkMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            HandleLoggedOffUser();

            if (!ModelState.IsValid)
            {
                return OnError(model, null);
            }

            ServiceResult result = _memberService.ChangePassword(CurrentMember.Id, model);
            if (!result.Success)
            {
                return OnError(model, result);
            }

            AddFeedback("Password changed", true);

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            HandleLoggedOnUser();

            if (ModelState.IsValid)
            {
                if (LogUserIn(model.Email, model.Password))
                {
                    return RedirectToCurrentUmbracoPage();
                }
            }

            SetInvalidModel(model);

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

            ServiceResult result = _memberService.Register(model, Umbraco);
            if (!result.Success)
            {
                return OnError(model, result);
            }

            if (!LogUserIn(model.Email, model.Password))
            {
                return OnError(model, new ServiceResult(false, "An error has occurred"));
            }

            AddFeedback("Welcome!", true);

            return RedirectToChapter(model.ChapterId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UpdateMemberModel model)
        {
            HandleLoggedOffUser();

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();
            model.SetChapter(chapter);

            ServiceResult result = _memberService.Update(CurrentMember.Id, model, Umbraco);
            if (!result.Success)
            {
                return OnError(model, result);
            }

            AddFeedback("Profile updated", true);

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportMemberPictures(IEnumerable<HttpPostedFileBase> files)
        {
            if (CurrentMemberModel.AdminUserId == null)
            {
                return RedirectToHome();
            }

            List<string> notFound = new List<string>();
            List<string> updated = new List<string>();

            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            IReadOnlyCollection<MemberModel> memberModels = _memberService.GetMembers(new MemberSearchCriteria(chapter.Id), Umbraco);
            foreach (HttpPostedFileBase file in files)
            {
                FileInfo fileInfo = new FileInfo(file.FileName);
                string fullName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                MemberModel memberModel = memberModels.FirstOrDefault(x => x.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase));
                if (memberModel == null)
                {
                    notFound.Add(file.FileName);
                    continue;
                }

                IPublishedContent member = Members.GetById(memberModel.Id);

                UpdateMemberModel updateMemberModel = new UpdateMemberModel(member);
                updateMemberModel.UploadedPicture = file;

                _memberService.Update(member.Id, updateMemberModel, Umbraco);

                updated.Add(file.FileName);
            }

            if (notFound.Count > 0)
            {
                AddFeedback("Members not found: " + string.Join(", ", notFound), false);
            }

            if (updated.Count > 0)
            {
                AddFeedback("Members updated: " + string.Join(", ", updated), true);
            }

            return RedirectToCurrentUmbracoPage();
        }

        private ActionResult OnError<T>(T model, ServiceResult result) where T : class
        {
            if (result != null)
            {
                foreach (string key in result.Errors.Keys)
                {
                    ModelState.AddModelError(key, result.Errors[key]);
                }
            }

            SetInvalidModel(model);
            return CurrentUmbracoPage();
        }

        private void HandleLoggedOffUser()
        {
            if (CurrentMember == null)
            {
                RedirectToHome();
            }
        }

        private void HandleLoggedOnUser()
        {
            if (Umbraco.MemberIsLoggedOn())
            {
                RedirectToChapter(CurrentMemberModel?.ChapterId);
            }
        }

        private bool LogUserIn(string email, string password, bool createPersistentCookie = true)
        {
            if (!Umbraco.MembershipHelper.Login(email, password))
            {
                return false;
            }

            FormsAuthentication.SetAuthCookie(email, createPersistentCookie);
            return true;
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