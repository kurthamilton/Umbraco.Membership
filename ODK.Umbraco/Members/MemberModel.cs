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

        private readonly Lazy<int?> _adminUserId;
        private readonly Lazy<bool> _disabled;
        private readonly MutableLazy<string> _favouriteBeverage;
        private readonly MutableLazy<string> _facebookProfile;
        private readonly MutableLazy<string> _firstName;
        private readonly MutableLazy<string> _hometown;
        private readonly MutableLazy<string> _knittingExperience;
        private readonly MutableLazy<string> _knittingExperienceOther;
        private readonly MutableLazy<string> _lastName;
        private readonly MutableLazy<string> _neighbourhood;
        private readonly Lazy<IPublishedContent> _picture;
        private readonly MutableLazy<string> _reason;
        private readonly Lazy<MemberTypes> _type;

        public MemberModel()
            : this(null)
        {
        }

        public MemberModel(IMember member, UmbracoHelper helper)
            : this(helper.TypedMember(member.Id))
        {
        }

        public MemberModel(IPublishedContent member)
        {
            if (member != null)
            {
                Chapter = member.GetPropertyValue<IPublishedContent>(MemberPropertyNames.ChapterId);
                Email = member.GetPropertyValue<string>(MemberPropertyNames.Email);
                Id = member.Id;
                Joined = member.CreateDate;
            }

            _adminUserId = new Lazy<int?>(() => member?.GetPropertyValue<int?>(MemberPropertyNames.AdminUserId));
            _disabled = new Lazy<bool>(() => member != null ? !member.GetPropertyValue<bool>(MemberPropertyNames.Approved) : false);
            _facebookProfile = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.FacebookProfile));
            _favouriteBeverage = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.FavouriteBeverage));
            _firstName = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.FirstName));
            _hometown = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.Hometown));
            _knittingExperience = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.KnittingExperience));
            _knittingExperienceOther = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.KnittingExperienceOther));
            _lastName = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.LastName));
            _neighbourhood = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.Neighbourhood));
            _picture = new Lazy<IPublishedContent>(() => member?.GetPropertyValue<IPublishedContent>(MemberPropertyNames.Picture));
            _reason = new MutableLazy<string>(() => member?.GetPropertyValue<string>(MemberPropertyNames.Reason));
            _type = new Lazy<MemberTypes>(() => member != null ? (MemberTypes)Enum.Parse(typeof(MemberTypes), member.GetPropertyValue<string>(MemberPropertyNames.Type)) : MemberTypes.Trial);
        }

        public int? AdminUserId => _adminUserId.Value;

        public IPublishedContent Chapter { get; private set; }

        public int ChapterId => Chapter?.Id ?? 0;

        public bool Disabled => _disabled.Value;

        [Required]
        [EmailAddress]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string Email { get; set; }

        [DisplayName("Facebook profile")]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string FacebookProfile
        {
            get { return _facebookProfile?.Value; }
            set { _facebookProfile.Value = value; }
        }

        [DisplayName("Your favourite alcoholic beverage")]
        [Required]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string FavouriteBeverage
        {
            get { return _favouriteBeverage?.Value; }
            set { _favouriteBeverage.Value = value; }
        }

        [DisplayName("First Name")]
        [Required]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string FirstName
        {
            get { return _firstName?.Value; }
            set { _firstName.Value = value; }
        }

        public string FullName => FirstName + " " + LastName;

        [DisplayName("Where are you from?")]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string Hometown
        {
            get { return _hometown?.Value; }
            set { _hometown.Value = value; }
        }

        public int Id { get; }

        [DisplayName("Date joined")]
        public DateTime? Joined { get; }

        [DisplayName("What's your level of knitting know-how?")]
        public string KnittingExperience
        {
            get { return _knittingExperience?.Value ?? ""; }
            set { _knittingExperience.Value = value; }
        }

        [DisplayName("Please specify")]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string KnittingExperienceOther
        {
            get { return _knittingExperienceOther?.Value; }
            set { _knittingExperienceOther.Value = value; }
        }

        [DisplayName("Last Name")]
        [Required]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string LastName
        {
            get { return _lastName?.Value; }
            set { _lastName.Value = value; }
        }

        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string Neighbourhood
        {
            get { return _neighbourhood?.Value; }
            set { _neighbourhood.Value = value; }
        }

        public IPublishedContent Picture => _picture?.Value;

        [DisplayName("Why are you a Drunken Knitwit?")]
        [Required]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters in length")]
        public string Reason
        {
            get { return _reason?.Value; }
            set { _reason.Value = value; }
        }

        [DisplayName("Membership type")]
        public MemberTypes Type => _type.Value;

        public void SetChapter(IPublishedContent chapter)
        {
            if (Chapter != null)
            {
                return;
            }

            Chapter = chapter;
        }

        protected void CopyFrom(MemberModel other)
        {
            if (other == null)
            {
                return;
            }

            Email = other.Email;
            FacebookProfile = other.FacebookProfile;
            FavouriteBeverage = other.FavouriteBeverage;
            FirstName = other.FirstName;
            Hometown = other.Hometown;
            KnittingExperience = other.KnittingExperience;
            KnittingExperienceOther = other.KnittingExperienceOther;
            LastName = other.LastName;
            Neighbourhood = other.Neighbourhood;
            Reason = other.Reason;
        }
    }
}
