using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public class SiteSettings
    {
        private Lazy<string> _loginUrl;
        private Lazy<string> _registerUrl;

        public SiteSettings(IPublishedContent content)
        {
            _loginUrl = new Lazy<string>(() => content.GetPropertyValue<IPublishedContent>(PropertyNames.LoginPage)?.Url);
            _registerUrl = new Lazy<string>(() => content.GetPropertyValue<IPublishedContent>(PropertyNames.RegisterPage)?.Url);
        }

        public string LoginUrl => _loginUrl.Value;

        public string RegisterUrl => _registerUrl.Value;
    }
}
