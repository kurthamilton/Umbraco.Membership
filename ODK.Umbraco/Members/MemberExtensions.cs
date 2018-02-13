using Umbraco.Core.Models;

namespace ODK.Umbraco.Members
{
    public static class MemberExtensions
    {
        public static object GetPropertyValue(this IMember member, string name)
        {
            return member.HasProperty(name) ? member.Properties[name] : null;
        }
    }
}
