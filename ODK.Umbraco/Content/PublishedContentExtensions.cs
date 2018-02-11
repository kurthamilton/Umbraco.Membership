using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using static Umbraco.Core.Constants;

namespace ODK.Umbraco.Content
{
    public static class PublishedContentExtensions
    {
        public static string ToPropertyValue(this IPublishedContent content)
        {
            return Udi.Create(UdiEntityType.Document, content.GetKey()).ToString();
        }
    }
}
