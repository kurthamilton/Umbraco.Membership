using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ODK.Umbraco.Content;
using ODK.Umbraco.Emails;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class OdkMemberService
    {
        private readonly OdkEmailService _emailService = new OdkEmailService();
        private readonly IMediaService _umbracoMediaService;
        private readonly IMemberService _umbracoMemberService;

        public OdkMemberService(IMediaService umbracoMediaService, IMemberService umbracoMemberService)
        {
            _umbracoMediaService = umbracoMediaService;
            _umbracoMemberService = umbracoMemberService;
        }

        public ServiceResult ChangePassword(int id, ChangePasswordModel model)
        {
            IMember member = _umbracoMemberService.GetById(id);
            if (member == null)
            {
                return new ServiceResult("", "Member not found");
            }

            _umbracoMemberService.SavePassword(member, model.NewPassword);

            return new ServiceResult(true);
        }

        public MemberModel GetMember(int id, UmbracoHelper helper)
        {
            if (!IsAllowed(helper))
            {
                return null;
            }

            IPublishedContent member = helper.TypedMember(id);
            return GetMember(member);
        }

        public IReadOnlyCollection<MemberModel> GetMembers(MemberSearchCriteria criteria, UmbracoHelper helper)
        {
            if (!IsAllowed(helper))
            {
                return new MemberModel[] { };
            }

            IEnumerable<MemberModel> models = _umbracoMemberService
                .GetAllMembers()
                .Select(x => helper.TypedMember(x.Id))
                .Select(x => new MemberModel(x))
                .Where(x => x.ChapterId == criteria.ChapterId);

            if (!criteria.ShowAll)
            {
                models = models.Where(x => !x.Disabled);
            }

            if (criteria.Types != null)
            {
                models = models.Where(x => criteria.Types.Contains(x.Type));
            }

            if (criteria.Sort != null)
            {
                models = criteria.Sort(models);
            }

            if (criteria.MaxItems > 0)
            {
                models = models.Take(criteria.MaxItems.Value);
            }

            return models.ToArray();
        }

        public ServiceResult Register(RegisterMemberModel model, UmbracoHelper helper)
        {
            if (_umbracoMemberService.GetByUsername(model.Email) != null)
            {
                return new ServiceResult(nameof(model.Email), "Email already registered");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model, model.UploadedPicture, helper);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            string memberType = _umbracoMemberService.GetDefaultMemberType();

            IMember member = _umbracoMemberService.CreateMember(model.Email, model.Email, model.FullName, memberType);

            member.SetValue(MemberPropertyNames.ChapterId, helper.GetPublishedContentAsPropertyValue(model.ChapterId));

            IMedia picture = SaveImage(model.UploadedPicture, model);

            UpdateMemberProperties(member, model, helper, picture);

            int trialPeriodMonths = model.Chapter.GetPropertyValue<int>("trialPeriodMonths");
            UpdateMemberSubscriptionProperties(member, model, MemberTypes.Trial, DateTime.Today.AddMonths(trialPeriodMonths) - DateTime.Today);

            _umbracoMemberService.Save(member);
            _umbracoMemberService.SavePassword(member, model.Password);

            // Send emails
            SendNewMemberAdminEmail(model);
            SendNewMemberEmail(model);

            return new ServiceResult(true);
        }

        public ServiceResult Update(int id, UpdateMemberModel model, UmbracoHelper helper)
        {
            IMember member = _umbracoMemberService.GetById(id);
            if (member == null)
            {
                return new ServiceResult("", "Member not found");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model, model.UploadedPicture, helper);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            IMedia picture = null;
            if (model.UploadedPicture != null)
            {
                picture = SaveImage(model.UploadedPicture, model);
            }

            UpdateMemberProperties(member, model, helper, picture);

            _umbracoMemberService.Save(member);

            return new ServiceResult(true);
        }

        public ServiceResult UpdateSubscription(MemberModel model, MemberTypes type, TimeSpan length, double amount)
        {
            IMember member = _umbracoMemberService.GetById(model.Id);
            if (member == null)
            {
                return new ServiceResult("", "Member not found");
            }

            UpdateMemberSubscriptionProperties(member, model, type, length, amount);

            _umbracoMemberService.Save(member);

            return new ServiceResult(true);
        }

        private static string ReplaceMemberProperties(string text, MemberModel model)
        {
            return text
                .Replace($"{{{{{MemberPropertyNames.Email}}}}}", model.Email)
                .Replace($"{{{{{MemberPropertyNames.FavouriteBeverage}}}}}", model.FavouriteBeverage)
                .Replace($"{{{{{MemberPropertyNames.FirstName}}}}}", model.FirstName)
                .Replace($"{{{{{MemberPropertyNames.Hometown}}}}}", model.Hometown)
                .Replace($"{{{{{MemberPropertyNames.KnittingExperience}}}}}", model.KnittingExperience)
                .Replace($"{{{{{MemberPropertyNames.KnittingExperienceOther}}}}}", model.KnittingExperienceOther)
                .Replace($"{{{{{MemberPropertyNames.LastName}}}}}", model.LastName)
                .Replace($"{{{{{MemberPropertyNames.Neighbourhood}}}}}", model.Neighbourhood)
                .Replace($"{{{{{MemberPropertyNames.Reason}}}}}", model.Reason);
        }

        private static bool IsAllowed(UmbracoHelper helper)
        {
            return !string.IsNullOrEmpty(helper.MembershipHelper.CurrentUserName);
        }

        private static void UpdateMemberProperties(IMember member, MemberModel model, UmbracoHelper helper, IMedia picture)
        {
            IEnumerable<KeyValuePair<int, string>> knittingExperienceOptions = helper.GetKnittingExperienceOptions();

            member.SetValue(MemberPropertyNames.FacebookProfile, model.FacebookProfile);
            member.SetValue(MemberPropertyNames.FavouriteBeverage, model.FavouriteBeverage);
            member.SetValue(MemberPropertyNames.FirstName, model.FirstName);
            member.SetValue(MemberPropertyNames.Hometown, model.Hometown);
            member.SetValue(MemberPropertyNames.KnittingExperience, knittingExperienceOptions.First(x => x.Value == model.KnittingExperience).Key);
            member.SetValue(MemberPropertyNames.KnittingExperienceOther, model.KnittingExperienceOther);
            member.SetValue(MemberPropertyNames.LastName, model.LastName);
            member.SetValue(MemberPropertyNames.Neighbourhood, model.Neighbourhood);
            member.SetValue(MemberPropertyNames.Reason, model.Reason);

            if (picture != null)
            {
                member.SetValue(MemberPropertyNames.Picture, picture.ToPropertyValue());
            }
        }

        private static void UpdateMemberSubscriptionProperties(IMember member, MemberModel model, MemberTypes type, TimeSpan length, double amount = 0)
        {
            DateTime subscriptionEndDate = DateTime.Today.Add(length);
            if (model.SubscriptionEndDate != null)
            {
                subscriptionEndDate = model.SubscriptionEndDate.Value.Add(length);
            }

            if (amount > 0)
            {
                member.SetValue(MemberPropertyNames.LastPaymentAmount, amount);
                member.SetValue(MemberPropertyNames.LastPaymentDate, DateTime.Today.ToString("yyyy-MM-dd"));
            }

            member.SetValue(MemberPropertyNames.SubscriptionEndDate, subscriptionEndDate);
            member.SetValue(MemberPropertyNames.Type, (int)type);
        }

        private MemberModel GetMember(IPublishedContent member)
        {
            if (member == null)
            {
                return null;
            }

            return new MemberModel(member);
        }

        private IMedia SaveImage(HttpPostedFileBase file, MemberModel member)
        {
            IMedia peopleFolder = _umbracoMediaService.GetRootMedia().FirstOrDefault(x => x.Name == "People");

            IMedia chapterFolder = peopleFolder.Children().FirstOrDefault(x => x.Name == member.Chapter.Name);
            if (chapterFolder == null)
            {
                chapterFolder = _umbracoMediaService.CreateMedia(member.Chapter.Name, peopleFolder, "Folder");
            }

            string key = member.FullName;
            IMedia existing = chapterFolder.Children().FirstOrDefault(x => x.Name == member.FullName);
            if (existing != null)
            {
                existing.Name = member.FullName + "-deleted";
                _umbracoMediaService.Save(existing);
            }

            IMedia memberImage = _umbracoMediaService.CreateMedia(member.FullName, chapterFolder, "Image");
            memberImage.SetValue("umbracoFile", file);

            _umbracoMediaService.Save(memberImage);

            if (existing != null)
            {
                _umbracoMediaService.Delete(existing);
            }

            return memberImage;
        }

        private void SendNewMemberAdminEmail(MemberModel model)
        {
            string subject = model.Chapter.GetPropertyValue<string>("newMemberEmailSubjectAdmin");
            string bodyText = model.Chapter.GetPropertyValue<string>("newMemberEmailBodyAdmin");
            bodyText = ReplaceMemberProperties(bodyText, model);
            _emailService.SendAdminEmail(model.Chapter, subject, bodyText);
        }

        private void SendNewMemberEmail(MemberModel model)
        {
            string subject = model.Chapter.GetPropertyValue<string>("newMemberEmailSubject");
            string bodyText = model.Chapter.GetPropertyValue<string>("newMemberEmailBody");
            bodyText = ReplaceMemberProperties(bodyText, model);
            _emailService.SendEmail(model.Chapter, subject, bodyText, new string[] { model.Email });
        }

        private IDictionary<string, string> ValidateModel<T>(T model, HttpPostedFileBase image, UmbracoHelper helper) where T : MemberModel, IMemberPictureUpload
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            if (helper == null)
            {
                return messages;
            }

            IEnumerable<KeyValuePair<int, string>> knittingExperienceOptions = helper.GetKnittingExperienceOptions();
            if (!knittingExperienceOptions.Any(x => x.Value == model.KnittingExperience))
            {
                messages.Add(nameof(model.KnittingExperience), "Knitting know-how required");
            }

            if (model.KnittingExperience == knittingExperienceOptions.Last().Value && string.IsNullOrWhiteSpace(model.KnittingExperienceOther))
            {
                messages.Add(nameof(model.KnittingExperienceOther), "Knitting know-how 'other' required");
            }

            if (image != null && !image.ContentType.StartsWith("image/"))
            {
                messages.Add(nameof(model.UploadedPicture), "File type not allowed. Please upload an image.");
            }

            return messages;
        }
    }
}
