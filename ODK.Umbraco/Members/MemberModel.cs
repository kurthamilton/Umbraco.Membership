using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class MemberModel
    {
        public const int DefaultKnittingExperienceOptionId = 0;

        private readonly MutableLazy<string> _favouriteBeverage;
        private readonly MutableLazy<string> _facebookProfile;
        private readonly MutableLazy<string> _firstName;
        private readonly MutableLazy<string> _hometown;
        private readonly MutableLazy<int> _knittingExperienceId;
        private readonly MutableLazy<string> _knittingExperienceOther;
        private readonly MutableLazy<string> _lastName;
        private readonly MutableLazy<string> _neighbourhood;
        private readonly Lazy<IPublishedContent> _picture;
        private readonly MutableLazy<string> _reason;

        public MemberModel(IMember member, UmbracoHelper helper)
        {
            Helper = helper;

            if (member != null)
            {
                Chapter = member.GetPublishedContentPropertyValue(MemberPropertyNames.ChapterId, helper);
                Email = member.Email;
                Id = member.Id;
                Joined = member.CreateDate;
            }

            _facebookProfile = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.FacebookProfile));
            _favouriteBeverage = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.FavouriteBeverage));
            _firstName = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.FirstName));
            _hometown = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.Hometown));
            _knittingExperienceId = new MutableLazy<int>(() => member?.GetIntegerPropertyValue(MemberPropertyNames.KnittingExperience) ?? DefaultKnittingExperienceOptionId);
            _knittingExperienceOther = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.KnittingExperienceOther));
            _lastName = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.LastName));
            _neighbourhood = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.Neighbourhood));
            _picture = new Lazy<IPublishedContent>(() => member?.GetPublishedMediaPropertyValue(MemberPropertyNames.Picture, helper));
            _reason = new MutableLazy<string>(() => member?.GetStringPropertyValue(MemberPropertyNames.Reason));
        }

        public IPublishedContent Chapter { get; protected set; }

        public int ChapterId => Chapter?.Id ?? 0;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Facebook profile")]
        public string FacebookProfile
        {
            get { return _facebookProfile?.Value; }
            set { _facebookProfile.Value = value; }
        }

        [DisplayName("Your favourite alcoholic beverage")]
        [Required]
        public string FavouriteBeverage
        {
            get { return _favouriteBeverage?.Value; }
            set { _favouriteBeverage.Value = value; }
        }

        [DisplayName("First Name")]
        [Required]
        public string FirstName
        {
            get { return _firstName?.Value; }
            set { _firstName.Value = value; }
        }

        public UmbracoHelper Helper { get; set; }

        [DisplayName("Where are you from?")]
        public string Hometown
        {
            get { return _hometown?.Value; }
            set { _hometown.Value = value; }
        }

        public int Id { get; }

        [DisplayName("Date joined")]
        public DateTime? Joined { get; }

        [DisplayName("What's your level of knitting know-how?")]
        public int KnittingExperienceId
        {
            get { return _knittingExperienceId?.Value ?? -1; }
            set { _knittingExperienceId.Value = value; }
        }

        [DisplayName("Please specify")]
        public string KnittingExperienceOther
        {
            get { return _knittingExperienceOther?.Value; }
            set { _knittingExperienceOther.Value = value; }
        }

        [DisplayName("Last Name")]
        [Required]
        public string LastName
        {
            get { return _lastName?.Value; }
            set { _lastName.Value = value; }
        }

        public string Neighbourhood
        {
            get { return _neighbourhood?.Value; }
            set { _neighbourhood.Value = value; }
        }

        public IPublishedContent Picture => _picture?.Value;

        [DisplayName("Why are you a Drunken Knitwit?")]
        [Required]
        public string Reason
        {
            get { return _reason?.Value; }
            set { _reason.Value = value; }
        }
    }
}
