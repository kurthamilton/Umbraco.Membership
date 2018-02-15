using ODK.Umbraco.Settings;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Web.Mvc
{
    public abstract class OdkUmbracoTemplatePage : UmbracoTemplatePage
    {
        protected OdkUmbracoTemplatePage()
        {
            HomePage = Model.Content.HomePage();
        }

        public IPublishedContent HomePage { get; }
    }
}
