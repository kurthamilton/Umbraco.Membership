using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Web.Mvc
{
    public class OdkUmbracoTemplateModel<T>
    {
        public OdkUmbracoTemplateModel(T value, IPublishedContent content, UmbracoHelper helper)
        {
            Content = content;
            Helper = helper;
            Value = value;
        }

        public IPublishedContent Content { get; }

        public UmbracoHelper Helper { get; }

        public T Value { get; }
    }
}
