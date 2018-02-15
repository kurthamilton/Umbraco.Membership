using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class RegisterMemberModel : MemberModel
    {
        public RegisterMemberModel()
            : this(null, null)
        {
        }

        public RegisterMemberModel(IPublishedContent chapter, UmbracoHelper helper)
            : base(null, helper)
        {
            Chapter = chapter;
            Helper = helper;
        }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [DisplayName("Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords must match")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Photo required")]
        [DisplayName("Please upload your photo")]
        public HttpPostedFileBase UploadedPicture { get; set; }

        public void SetChapter(IPublishedContent chapter)
        {
            if (Chapter != null)
            {
                return;
            }

            Chapter = chapter;
        }
    }
}
