using ODK.Umbraco.Settings;
using Umbraco.Web.Mvc;

namespace ODK.Umbraco.Web.Mvc
{
    public abstract class OdkUmbracoTemplatePage : UmbracoTemplatePage
    {
        protected OdkUmbracoTemplatePage()
        {
            HomePageSettings = Model.Content.HomePageSettings();
        }

        public HomePageSettings HomePageSettings { get; }
    }
}
