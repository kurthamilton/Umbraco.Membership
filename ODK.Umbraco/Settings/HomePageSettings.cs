using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public class HomePageSettings
    {
        public HomePageSettings(string name, IPublishedContent content)
        {
            Content = content;
            Name = name;
        }

        public IPublishedContent Content { get; private set; }

        public string Name { get; private set; }

        public T GetPropertyValue<T>(string alias)
        {
            return Content.GetPropertyValue<T>(alias, recurse: true);
        }
    }
}
