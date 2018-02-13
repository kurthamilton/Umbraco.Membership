using System;
using Umbraco.Core.Models;

namespace ODK.Umbraco.Members
{
    public class MemberModel
    {
        private readonly Lazy<int?> _chapterId;
        private readonly Lazy<string> _firstName;
        private readonly Lazy<string> _lastName;
        private readonly Lazy<IPublishedContent> _picture;

        public MemberModel(IMember member)
        {
            _chapterId = new Lazy<int?>(() => (member.GetPropertyValue(MemberPropertyNames.ChapterId) as IPublishedContent)?.Id);
            _firstName = new Lazy<string>(() => member.GetPropertyValue(MemberPropertyNames.FirstName)?.ToString());
            _lastName = new Lazy<string>(() => member.GetPropertyValue(MemberPropertyNames.LastName)?.ToString());
            _picture = new Lazy<IPublishedContent>(() => member.GetPropertyValue(MemberPropertyNames.Picture) as IPublishedContent);
        }

        public int? ChapterId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public IPublishedContent Picture { get; set; }
    }
}
