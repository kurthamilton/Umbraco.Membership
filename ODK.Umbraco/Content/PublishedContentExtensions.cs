using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using static Umbraco.Core.Constants;

namespace ODK.Umbraco.Content
{
    public static class PublishedContentExtensions
    {
        public static IEnumerable<T> GetPropertyValues<T>(this IEnumerable<IPublishedContent> contents, string alias)
        {
            foreach (IPublishedContent content in contents)
            {
                if (content.HasProperty(alias))
                {
                    yield return content.GetPropertyValue<T>(alias);
                }
            }
        }

        public static string ToPropertyValue(this IPublishedContent content)
        {
            return Udi.Create(UdiEntityType.Document, content.GetKey()).ToString();
        }
    }
}
