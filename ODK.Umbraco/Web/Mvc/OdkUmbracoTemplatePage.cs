using System.Web.Mvc;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Web.Mvc
{
    public abstract class OdkUmbracoTemplatePage : UmbracoTemplatePage
    {
        private RequestCacheItem<IPublishedContent> _currentMember;
        private RequestCacheItem<MemberModel> _currentMemberModel;
        private RequestCacheItem<IPublishedContent> _homePage;

        protected OdkUmbracoTemplatePage()
        {
            _currentMember = new RequestCacheItem<IPublishedContent>(nameof(_currentMember), () => Umbraco.MembershipHelper.GetCurrentMember());
            _currentMemberModel = new RequestCacheItem<MemberModel>(nameof(_currentMemberModel), () => new MemberModel(CurrentMember));
            _homePage = new RequestCacheItem<IPublishedContent>(nameof(_homePage), () => Model.Content.HomePage());

            EventService = DependencyResolver.Current.GetService<EventService>();
            MemberService = DependencyResolver.Current.GetService<OdkMemberService>();
        }

        public bool IsRestricted { get; set; }

        protected override void InitializePage()
        {
            if (Model.Content.IsRestricted(CurrentMember))
            {
                IsRestricted = true;
                Response.Redirect(Model.Content.Parent.Url);
                return;
            }

            base.InitializePage();
        }

        public IPublishedContent CurrentMember => _currentMember.Value;

        public MemberModel CurrentMemberModel => _currentMemberModel.Value;

        public IDataTypeService DataTypeService => ApplicationContext.Services.DataTypeService;

        public EventService EventService { get; }

        public IPublishedContent HomePage => _homePage.Value;

        public OdkMemberService MemberService { get; }

        public OdkUmbracoTemplateModel<T> ModelFor<T>(T value)
        {
            return new OdkUmbracoTemplateModel<T>(value, Model.Content, Umbraco);
        }
    }
}
