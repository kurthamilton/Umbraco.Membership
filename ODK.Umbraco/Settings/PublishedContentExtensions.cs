using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public static class PublishedContentExtensions
    {
        public static HomePageSettings HomePageSettings(this IPublishedContent content)
        {
            foreach (IPublishedContent ancestor in content.AncestorsOrSelf())
            {
                string name = ancestor.GetPropertyValue<string>(PropertyNames.SiteName);

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                return new HomePageSettings(name, ancestor);
            }

            return null;
        }
    }
}
