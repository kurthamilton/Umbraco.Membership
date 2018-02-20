using ODK.Umbraco.Members;
using ODK.Umbraco.Security;
using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Web.Mvc
{
    public abstract class OdkUmbracoTemplatePage : UmbracoTemplatePage
    {
        private RequestCacheItem<IUser> _adminUser = new RequestCacheItem<IUser>(nameof(_adminUser), () => SecurityHelper.CurrentAdminUser());
        private RequestCacheItem<IPublishedContent> _currentMember;
        private RequestCacheItem<MemberModel> _currentMemberModel;
        private RequestCacheItem<IPublishedContent> _homePage;
        private RequestCacheItem<OdkMemberService> _memberService;

        protected OdkUmbracoTemplatePage()
        {
            _currentMember = new RequestCacheItem<IPublishedContent>(nameof(_currentMember), () => Umbraco.MembershipHelper.GetCurrentMember());
            _currentMemberModel = new RequestCacheItem<MemberModel>(nameof(_currentMemberModel), () => new MemberModel(CurrentMember));
            _homePage = new RequestCacheItem<IPublishedContent>(nameof(_homePage), () => Model.Content.HomePage());
            _memberService = new RequestCacheItem<OdkMemberService>(nameof(_memberService),
                () => new OdkMemberService(CurrentMember, ApplicationContext.Services.MediaService, ApplicationContext.Services.MemberService, Umbraco));
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

        public IUser AdminUser => _adminUser.Value;

        public IPublishedContent CurrentMember => _currentMember.Value;

        public MemberModel CurrentMemberModel => _currentMemberModel.Value;

        public IDataTypeService DataTypeService => ApplicationContext.Services.DataTypeService;

        public IPublishedContent HomePage => _homePage.Value;

        public OdkMemberService MemberService => _memberService.Value;

        public OdkUmbracoTemplateModel<T> ModelFor<T>(T value)
        {
            return new OdkUmbracoTemplateModel<T>(value, Model.Content, Umbraco);
        }
    }
}
