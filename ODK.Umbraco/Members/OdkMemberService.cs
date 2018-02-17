﻿using System.Collections.Generic;
using System.Linq;
using ODK.Umbraco.Content;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class OdkMemberService
    {
        private readonly UmbracoHelper _umbracoHelper;

        private readonly IMemberService _umbracoMemberService;

        public OdkMemberService(IMemberService umbracoMemberService, UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
            _umbracoMemberService = umbracoMemberService;
        }

        public MemberModel GetMember(string username)
        {
            IMember umbracoMember = _umbracoMemberService.GetByUsername(username);
            if (umbracoMember == null)
            {
                return null;
            }

            return new MemberModel(umbracoMember, _umbracoHelper);
        }

        public MemberModel GetMember(int id)
        {
            IMember umbracoMember = _umbracoMemberService.GetById(id);
            if (umbracoMember == null)
            {
                return null;
            }

            return new MemberModel(umbracoMember, _umbracoHelper);
        }

        public IReadOnlyCollection<MemberModel> GetMembers(int chapterId)
        {
            IEnumerable<IMember> members = _umbracoMemberService.GetAllMembers();
            return members.Select(x => new MemberModel(x, _umbracoHelper))
                          .Where(x => x.ChapterId == chapterId)
                          .ToArray();
        }

        public ServiceResult Register(RegisterMemberModel model)
        {
            if (_umbracoMemberService.GetByUsername(model.Email) != null)
            {
                return new ServiceResult(nameof(model.Email), "That email address is already registered");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            string memberType = _umbracoMemberService.GetDefaultMemberType();

            IMember member = _umbracoMemberService.CreateMember(model.Email, model.Email, model.FirstName + " " + model.LastName, memberType);

            member.SetValue(MemberPropertyNames.ChapterId, _umbracoHelper.GetPublishedContentAsPropertyValue(model.ChapterId));

            UpdateMemberProperties(member, model);

            _umbracoMemberService.Save(member);
            _umbracoMemberService.SavePassword(member, model.Password);

            return new ServiceResult(true);
        }

        public ServiceResult Update(int id, MemberModel model)
        {
            IMember member = _umbracoMemberService.GetById(id);
            if (member == null)
            {
                return new ServiceResult("", "Member not found");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            UpdateMemberProperties(member, model);

            _umbracoMemberService.Save(member);

            return new ServiceResult(true);
        }

        private static void UpdateMemberProperties(IMember member, MemberModel model)
        {
            member.SetValue(MemberPropertyNames.FacebookProfile, model.FacebookProfile);
            member.SetValue(MemberPropertyNames.FavouriteBeverage, model.FavouriteBeverage);
            member.SetValue(MemberPropertyNames.FirstName, model.FirstName);
            member.SetValue(MemberPropertyNames.Hometown, model.Hometown);
            member.SetValue(MemberPropertyNames.KnittingExperience, model.KnittingExperienceId);
            member.SetValue(MemberPropertyNames.KnittingExperienceOther, model.KnittingExperienceOther);
            member.SetValue(MemberPropertyNames.LastName, model.LastName);
            member.SetValue(MemberPropertyNames.Neighbourhood, model.Neighbourhood);
            member.SetValue(MemberPropertyNames.Reason, model.Reason);
        }

        private IDictionary<string, string> ValidateModel(MemberModel model)
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            IDictionary<int, string> knittingExperienceOptions = _umbracoHelper.GetKnittingExperienceOptions();
            if (!knittingExperienceOptions.ContainsKey(model.KnittingExperienceId))
            {
                messages.Add(nameof(model.KnittingExperienceId), "Knitting know-how required");
            }

            if (model.KnittingExperienceId == knittingExperienceOptions.Last().Key && string.IsNullOrWhiteSpace(model.KnittingExperienceOther))
            {
                messages.Add(nameof(model.KnittingExperienceOther), "Knitting know-how 'other' required");
            }

            return messages;
        }
    }
}