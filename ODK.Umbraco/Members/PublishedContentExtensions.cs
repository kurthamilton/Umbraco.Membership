using ODK.Umbraco.Content;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Members
{
    public static class PublishedContentExtensions
    {
        public static bool IsRestricted(this IPublishedContent content, IPublishedContent member, bool recurse = true)
        {
            bool isAdmin = content.GetPropertyValue<bool>(PropertyNames.Admin);
            if (member == null)
            {
                return isAdmin || content.GetPropertyValue(PropertyNames.Restricted, recurse: recurse, defaultValue: false);
            }

            return isAdmin ? member.GetPropertyValue<int?>(MemberPropertyNames.AdminUserId) == null : false;
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

        public static IEnumerable<IEnumerable<IPublishedContent>> MenuItemGroups(this IPublishedContent content, IPublishedContent member, int groupSize = 5)
        {
            List<IEnumerable<IPublishedContent>> groups = new List<IEnumerable<IPublishedContent>>();

            IPublishedContent[] menuItems = content.MenuItems(member).ToArray();
            for (int i = 0; i < menuItems.Length; i += groupSize)
            {
                groups.Add(menuItems.Skip(i).Take(groupSize));
            }

            return groups;
        }

        private static bool ShowInMenu(IPublishedContent content, IPublishedContent member)
        {
            if (content.TemplateId <= 0)
            {
                return false;
            }

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