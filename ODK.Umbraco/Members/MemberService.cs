using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class MemberService
    {
        private readonly IMemberService _umbracoMemberService;

        public MemberService(IMemberService umbracoMemberService)
        {
            _umbracoMemberService = umbracoMemberService;
        }

        public MemberModel GetMember(string username, UmbracoHelper helper)
        {
            IMember umbracoMember = _umbracoMemberService.GetByUsername(username);
            if (umbracoMember == null)
            {
                return null;
            }

            return new MemberModel(umbracoMember, helper);
        }

        public MemberModel GetMember(int id, UmbracoHelper helper)
        {
            IMember umbracoMember = _umbracoMemberService.GetById(id);
            if (umbracoMember == null)
            {
                return null;
            }

            return new MemberModel(umbracoMember, helper);
        }

        public IReadOnlyCollection<MemberModel> GetMembers(int chapterId, UmbracoHelper helper)
        {
            IEnumerable<IMember> members = _umbracoMemberService.GetAllMembers();
            return members.Select(x => new MemberModel(x, helper))
                          .Where(x => x.ChapterId == chapterId)
                          .ToArray();
        }

        public ServiceResult Register(RegisterMemberModel model)
        {
            if (_umbracoMemberService.GetByUsername(model.Email) != null)
            {
                return new ServiceResult("That email address is already registered");
            }

            string memberType = _umbracoMemberService.GetDefaultMemberType();

            IMember member = _umbracoMemberService.CreateMember(model.Email, model.Email, model.FirstName + " " + model.LastName, memberType);
            member.RawPasswordValue = model.Password;
            member.SetValue(MemberPropertyNames.ChapterId, model.ChapterId);
            member.SetValue(MemberPropertyNames.FirstName, model.FirstName);
            member.SetValue(MemberPropertyNames.LastName, model.LastName);

            _umbracoMemberService.Save(member);

            return new ServiceResult(true);
        }
    }
}
