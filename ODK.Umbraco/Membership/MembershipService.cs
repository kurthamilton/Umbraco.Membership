using System.Web.Security;
using ODK.Umbraco.Content;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Security;

namespace ODK.Umbraco.Membership
{
    public class MembershipService
    { 
        private readonly MembershipHelper _membershipHelper;

        public MembershipService(MembershipHelper membershipHelper)
        {
            _membershipHelper = membershipHelper;
        }

        public int? GetChapterId(string username)
        {
            IPublishedContent member = _membershipHelper.GetByUsername(username);
            if (member == null)
            {
                return null;
            }

            if (!member.HasProperty(MemberPropertyNames.ChapterId))
            {
                return null;
            }

            return member.GetPropertyValue<IPublishedContent>(MemberPropertyNames.ChapterId)?.Id;
        }

        public ServiceResult Register(RegisterMember model)
        {
            if (_membershipHelper.GetByUsername(model.Email) != null)
            {
                return new ServiceResult("That email address is already registered");
            }

            RegisterModel registerModel = _membershipHelper.CreateRegistrationModel();
            registerModel.Email = model.Email;
            registerModel.Name = model.FirstName;
            registerModel.Password = model.Password;
            registerModel.UsernameIsEmail = true;

            MembershipUser membershipUser = _membershipHelper.RegisterMember(registerModel, out MembershipCreateStatus status);
            if (membershipUser == null || status != MembershipCreateStatus.Success)
            {
                return new ServiceResult(status.ToString());                
            }                

            return new ServiceResult(true);            
        }

        public void UpdateChapter(IPublishedContent chapterContent)
        {
            ProfileModel profile = _membershipHelper.GetCurrentMemberProfileModel();
            if (profile == null)
            {
                return;
            }

            string chapterUdi = chapterContent.ToPropertyValue();
            profile.SetProperty(MemberPropertyNames.ChapterId, chapterUdi);

            _membershipHelper.UpdateMemberProfile(profile);
        }        
    }
}
