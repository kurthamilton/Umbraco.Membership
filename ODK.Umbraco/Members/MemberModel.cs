using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class MemberModel
    {
        private readonly Lazy<int?> _chapterId;
        private readonly Lazy<string> _favouriteBeverage;
        private readonly Lazy<string> _facebookProfile;
        private readonly Lazy<string> _firstName;
        private readonly Lazy<string> _hometown;
        private readonly Lazy<string> _knittingExperience;
        private readonly Lazy<string> _lastName;
        private readonly Lazy<string> _neighbourhood;
        private readonly Lazy<IPublishedContent> _picture;
        private readonly Lazy<string> _reason;

        public MemberModel(IMember member, UmbracoHelper helper)
        {
            Id = member.Id;
            Joined = member.CreateDate;

            _chapterId = new Lazy<int?>(() => member.GetPublishedContentPropertyValue(MemberPropertyNames.ChapterId, helper)?.Id);
            _facebookProfile = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.FacebookProfile));
            _favouriteBeverage = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.FavouriteBeverage));
            _firstName = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.FirstName));
            _hometown = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.Hometown));
            _knittingExperience = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.KnittingExperience));
            _lastName = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.LastName));
            _neighbourhood = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.Neighbourhood));
            _picture = new Lazy<IPublishedContent>(() => member.GetPublishedMediaPropertyValue(MemberPropertyNames.Picture, helper));
            _reason = new Lazy<string>(() => member.GetStringPropertyValue(MemberPropertyNames.Reason));
        }

        public int? ChapterId => _chapterId.Value;

        public string FacebookProfile => _facebookProfile.Value;

        public string FavouriteBeverage => _favouriteBeverage.Value;

        public string FirstName => _firstName.Value;

        public string Hometown => _hometown.Value;

        public int Id { get; }

        public DateTime Joined { get; }

        public string KnittingExperience => _knittingExperience.Value;

        public string LastName => _lastName.Value;

        public string Neighbourhood => _neighbourhood.Value;

        public IPublishedContent Picture => _picture.Value;

        public string Reason => _reason.Value;
    }
}
