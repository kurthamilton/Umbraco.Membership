using System.ComponentModel;
using System.Web;
using Umbraco.Core.Models;

namespace ODK.Umbraco.Members
{
    public class UpdateMemberModel : MemberModel, IMemberPictureUpload
    {
        public UpdateMemberModel()
            : this(null)
        {
        }

        public UpdateMemberModel(IPublishedContent member)
            : base(member)
        {
        }

        public UpdateMemberModel(IPublishedContent member, UpdateMemberModel other)
            : base(member)
        {
            CopyFrom(other);
        }

        [DisplayName("Upload new photo")]
        public HttpPostedFileBase UploadedPicture { get; set; }
    }
}
