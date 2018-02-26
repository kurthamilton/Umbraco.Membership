using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Models
{
    public class PersonThumbnailViewModel
    {
        public PersonThumbnailViewModel(MemberModel member, bool showFullName = true)
        {
            FullName = member.FullName;
            MemberId = member.Id;
            PersonPageUrl = member.Chapter.GetPropertyValue<IPublishedContent>("personPage").Url;
            Picture = member.Picture;
            ShowFullName = showFullName;
        }

        public string FullName { get; }

        public int MemberId { get; }

        public string PersonPageUrl { get; }

        public IPublishedContent Picture { get; }

        public bool ShowFullName { get; }
    }
}