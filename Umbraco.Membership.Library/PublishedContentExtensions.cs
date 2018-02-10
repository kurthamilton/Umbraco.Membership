using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Umbraco.Membership
{
    public static class PublishedContentExtensions
    {
        
        public static IEnumerable<IPublishedContent> PermittedChildren(this IPublishedContent content, IPublishedContent member)
        {
            return content.Children.Where(x => x.IsVisible() && !x.IsRestricted(member));
        }

        public static bool IsRestricted(this IPublishedContent content, IPublishedContent member)
        {
            bool restricted = content.GetPropertyValue<bool>(Constants.Restricted);
            if (!restricted)
            {
                return false;
            }

            return member == null;
        }
    }
}