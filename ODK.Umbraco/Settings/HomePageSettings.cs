using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public class HomePageSettings
    {
        private Lazy<string> _loginButtonText;
        private Lazy<string> _loginUrl;
        private Lazy<IPublishedContent> _logo;
        private Lazy<string> _logoutButtonText;
        private Lazy<string> _registerButtonText;
        private Lazy<string> _registerUrl;
        private Lazy<string> _themeColour;
        private Lazy<string> _themeFontColour;

        public HomePageSettings(string name, IPublishedContent content)
        {
            Content = content;
            Name = name;

            _loginButtonText = new Lazy<string>(() => GetPropertyValue<string>(PropertyNames.LoginButtonText));
            _loginUrl = new Lazy<string>(() => GetPropertyValue<IPublishedContent>(PropertyNames.LoginPage)?.Url);
            _logo = new Lazy<IPublishedContent>(() => GetPropertyValue<IPublishedContent>(PropertyNames.SiteLogo));
            _logoutButtonText = new Lazy<string>(() => GetPropertyValue<string>(PropertyNames.LogoutButtonText));
            _registerButtonText = new Lazy<string>(() => GetPropertyValue<string>(PropertyNames.RegisterButtonText));
            _registerUrl = new Lazy<string>(() => GetPropertyValue<IPublishedContent>(PropertyNames.RegisterPage)?.Url);
            _themeColour = new Lazy<string>(() => GetPropertyValue<string>(PropertyNames.ThemeColour));
            _themeFontColour = new Lazy<string>(() => GetPropertyValue<string>(PropertyNames.ThemeFontColour));
        }

        public IPublishedContent Content { get; private set; }

        public string LoginButtonText => _loginButtonText.Value;

        public string LoginUrl => _loginUrl.Value;

        public IPublishedContent Logo => _logo.Value;

        public string LogoutButtonText => _logoutButtonText.Value;

        public string Name { get; private set; }

        public string RegisterButtonText => _registerButtonText.Value;

        public string RegisterUrl => _registerUrl.Value;

        public string ThemeColour => _themeColour.Value;

        public string ThemeFontColour => _themeFontColour.Value;

        private T GetPropertyValue<T>(string alias)
        {
            return Content.GetPropertyValue<T>(alias, recurse: true);
        }
    }
}
