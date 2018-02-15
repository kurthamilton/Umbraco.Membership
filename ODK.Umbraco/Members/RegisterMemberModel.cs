using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public class RegisterMemberModel : MemberModel
    {
        public RegisterMemberModel()
            : this(0, null)
        {
        }

        public RegisterMemberModel(int chapterId, UmbracoHelper helper)
            : base(null, helper)
        {
            ChapterId = chapterId;
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
    }
}
