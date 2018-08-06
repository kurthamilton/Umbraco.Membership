using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ODK.Umbraco.Members;
using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Web.Mvc
{
    public abstract class OdkSurfaceControllerBase : SurfaceController
    {
        private readonly Lazy<IPublishedContent> _currentMember;
        private readonly Lazy<MemberModel> _currentMemberModel;
        private readonly Lazy<IPublishedContent> _currentPage;
        private readonly List<string> _feedbackMessages = new List<string>();
        private readonly List<bool> _feedbackSuccesses = new List<bool>();
        private readonly Lazy<IPublishedContent> _homePage;

        protected OdkSurfaceControllerBase()
        {
            _currentMember = new Lazy<IPublishedContent>(() => Umbraco.MembershipHelper.GetCurrentMember());
            _currentMemberModel = new Lazy<MemberModel>(() => CurrentMember != null ? new MemberModel(CurrentMember) : null);
            _currentPage = new Lazy<IPublishedContent>(() => Umbraco.AssignedContentItem);
            _homePage = new Lazy<IPublishedContent>(() => CurrentPage.HomePage());
        }

        protected IPublishedContent CurrentMember => _currentMember.Value;

        protected MemberModel CurrentMemberModel => _currentMemberModel.Value;

        protected IPublishedContent CurrentPage => _currentPage.Value;

        protected IPublishedContent HomePage => _homePage.Value;

        protected void AddFeedback(string message, bool success)
        {
            _feedbackMessages.Add(message);
            _feedbackSuccesses.Add(success);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (_feedbackMessages.Count > 0)
            {
                TempData["Feedback.Messages"] = _feedbackMessages.ToArray();
                TempData["Feedback.Successes"] = _feedbackSuccesses.ToArray();
            }

            base.OnResultExecuting(filterContext);
        }

        protected void SetInvalidModel<T>(T model) where T : class
        {
            TempData[typeof(T).Name] = model;
        }
    }
}
