using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Settings
{
    public class HomePageSettings
    {
        private Lazy<IPublishedContent> _logo;
        private Lazy<string> _themeColour;
        private Lazy<string> _themeFontColour;

        public HomePageSettings(string name, IPublishedContent content)
        {
            Content = content;
            Name = name;

            _logo = new Lazy<IPublishedContent>(() => content.GetPropertyValue<IPublishedContent>(PropertyNames.SiteLogo));
            _themeColour = new Lazy<string>(() => content.GetPropertyValue<string>(PropertyNames.ThemeColour));
            _themeFontColour = new Lazy<string>(() => content.GetPropertyValue<string>(PropertyNames.ThemeFontColour));
        }

        public IPublishedContent Content { get; private set; }

        public IPublishedContent Logo => _logo.Value;

        public string Name { get; private set; }

        public string ThemeColour => _themeColour.Value;

        public string ThemeFontColour => _themeFontColour.Value;
    }
}
