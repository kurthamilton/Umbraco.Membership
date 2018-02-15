using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public static class PublishedContentExtensions
    {
        public static IPublishedContent HomePage(this IPublishedContent content)
        {
            foreach (IPublishedContent ancestor in content.AncestorsOrSelf())
            {
                string name = ancestor.GetPropertyValue<string>(PropertyNames.SiteName);

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                return ancestor;
            }

            return null;
        }
    }
}
