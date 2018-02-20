using ODK.Umbraco.Content;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public static class PublishedContentExtensions
    {
        public static bool IsRestricted(this IPublishedContent content, IPublishedContent member, bool recurse = true)
        {
            if (member == null)
            {
                return content.GetPropertyValue(PropertyNames.Restricted, recurse: recurse, defaultValue: false);
            }

            if (!content.GetPropertyValue<bool>(PropertyNames.Admin))
            {
                return false;
            }

            return member.GetPropertyValue<int?>(MemberPropertyNames.AdminUserId) == null;
        }

        public static IEnumerable<IPublishedContent> MenuItems(this IPublishedContent content, IPublishedContent member)
        {
            foreach (IPublishedContent child in content.Children)
            {
                if (ShowInMenu(child, member))
                {
                    yield return child;
                }
            }
        }

        private static bool ShowInMenu(IPublishedContent content, IPublishedContent member)
        {
            if (!content.IsPage())
            {
                return false;
            }

            if (content.GetPropertyValue(PropertyNames.UmbracoNaviHide, recurse: true, defaultValue: false))
            {
                return false;
            }

            if (content.IsRestricted(member))
            {
                return false;
            }

            return true;
        }
    }
}