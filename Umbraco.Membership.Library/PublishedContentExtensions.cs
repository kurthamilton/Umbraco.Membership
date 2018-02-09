using Umbraco.Core.Models;

namespace Umbraco.Membership
{
    public static class PublishedContentExtensions
    {
        public static bool IsRestricted(this IPublishedContent content, IPublishedContent member)
        {
            string value = content.GetProperty("restricted")?.Value?.ToString();
            if (!bool.TryParse(value, out bool restricted) || !restricted)
            {
                return false;
            }

            return member == null;
        }
    }
}