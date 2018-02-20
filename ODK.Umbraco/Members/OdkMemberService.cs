using System.Collections.Generic;
using System.Linq;
using System.Web;
using ODK.Umbraco.Content;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class OdkMemberService
    {
        private readonly IPublishedContent _currentMember;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IMediaService _umbracoMediaService;
        private readonly IMemberService _umbracoMemberService;

        public OdkMemberService(IPublishedContent currentMember, IMediaService umbracoMediaService, IMemberService umbracoMemberService,
            UmbracoHelper umbracoHelper)
        {
            _currentMember = currentMember;
            _umbracoHelper = umbracoHelper;
            _umbracoMediaService = umbracoMediaService;
            _umbracoMemberService = umbracoMemberService;
        }

        public MemberModel GetCurrentMember()
        {
            return GetMember(_currentMember);
        }

        public MemberModel GetMember(int id)
        {
            if (_currentMember == null)
            {
                return null;
            }

            IPublishedContent member = _umbracoHelper.TypedMember(id);
            return GetMember(member);
        }

        public IReadOnlyCollection<MemberModel> GetMembers(MemberSearchCriteria criteria)
        {
            if (_currentMember == null)
            {
                return new MemberModel[] { };
            }

            IEnumerable<IMember> members = _umbracoMemberService.GetAllMembers();

            IEnumerable<MemberModel> models = members.Select(x => new MemberModel(x, _umbracoHelper))
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

        public ServiceResult Register(RegisterMemberModel model)
        {
            if (_umbracoMemberService.GetByUsername(model.Email) != null)
            {
                return new ServiceResult(nameof(model.Email), "Email already registered");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model, model.UploadedPicture);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            string memberType = _umbracoMemberService.GetDefaultMemberType();

            IMember member = _umbracoMemberService.CreateMember(model.Email, model.Email, model.FullName, memberType);

            member.SetValue(MemberPropertyNames.ChapterId, _umbracoHelper.GetPublishedContentAsPropertyValue(model.ChapterId));

            IMedia picture = SaveImage(model.UploadedPicture, model);

            UpdateMemberProperties(member, model, picture);

            _umbracoMemberService.Save(member);
            _umbracoMemberService.SavePassword(member, model.Password);

            return new ServiceResult(true);
        }

        public ServiceResult Update(int id, UpdateMemberModel model)
        {
            IMember member = _umbracoMemberService.GetById(id);
            if (member == null)
            {
                return new ServiceResult("", "Member not found");
            }

            IDictionary<string, string> validationMessages = ValidateModel(model, model.UploadedPicture);
            if (validationMessages.Any())
            {
                return new ServiceResult(validationMessages);
            }

            IMedia picture = null;
            if (model.UploadedPicture != null)
            {
                picture = SaveImage(model.UploadedPicture, model);

            }

            UpdateMemberProperties(member, model, picture);

            _umbracoMemberService.Save(member);

            return new ServiceResult(true);
        }

        private MemberModel GetMember(IPublishedContent member)
        {
            if (member == null)
            {
                return null;
            }

            return new MemberModel(member);
        }

        private static void UpdateMemberProperties(IMember member, MemberModel model, IMedia picture)
        {
            member.SetValue(MemberPropertyNames.FacebookProfile, model.FacebookProfile);
            member.SetValue(MemberPropertyNames.FavouriteBeverage, model.FavouriteBeverage);
            member.SetValue(MemberPropertyNames.FirstName, model.FirstName);
            member.SetValue(MemberPropertyNames.Hometown, model.Hometown);
            member.SetValue(MemberPropertyNames.KnittingExperience, model.KnittingExperience);
            member.SetValue(MemberPropertyNames.KnittingExperienceOther, model.KnittingExperienceOther);
            member.SetValue(MemberPropertyNames.LastName, model.LastName);
            member.SetValue(MemberPropertyNames.Neighbourhood, model.Neighbourhood);
            member.SetValue(MemberPropertyNames.Reason, model.Reason);

            if (picture != null)
            {
                member.SetValue(MemberPropertyNames.Picture, picture.ToPropertyValue());
            }
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

        private IDictionary<string, string> ValidateModel<T>(T model, HttpPostedFileBase image) where T : MemberModel, IMemberPictureUpload
        {
            Dictionary<string, string> messages = new Dictionary<string, string>();

            IEnumerable<string> knittingExperienceOptions = _umbracoHelper.GetKnittingExperienceOptions();
            if (!knittingExperienceOptions.Contains(model.KnittingExperience))
            {
                messages.Add(nameof(model.KnittingExperience), "Knitting know-how required");
            }

            if (model.KnittingExperience == knittingExperienceOptions.Last() && string.IsNullOrWhiteSpace(model.KnittingExperienceOther))
            {
                messages.Add(nameof(model.KnittingExperienceOther), "Knitting know-how 'other' required");
            }

            if (!image.ContentType.StartsWith("image/"))
            {
                messages.Add(nameof(model.UploadedPicture), "File type not allowed. Please upload an image.");
            }

            return messages;
        }
    }
}
