using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Membership
{
    public static class ContentExtensions
    {                
        public static bool IsRestricted(this IPublishedContent content, IPublishedContent member)
        {
            if (member != null)
            {
                return false;
            }

            return content.GetPropertyValue(PropertyNames.Restricted, recurse: true, defaultValue: false);
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
            if (content.GetPropertyValue(PropertyNames.UmbracoNaviHide, recurse: true, defaultValue: false))
            {
                return false;
            }

            if (content.IsRestricted(member))
            {
                return false;
            }

            if (member != null)
            {
                return !content.GetPropertyValue(PropertyNames.HideWhenLoggedIn, recurse: true, defaultValue: false);
            }

            return true;
        }
    }
}