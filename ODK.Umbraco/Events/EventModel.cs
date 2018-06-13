using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Events
{
    public class EventModel
    {
        private readonly Lazy<string> _address;
        private readonly Lazy<DateTime> _date;
        private readonly Lazy<string> _description;
        private readonly Lazy<IPublishedContent> _image;
        private readonly Lazy<string> _location;
        private readonly Lazy<string> _mapQuery;
        private readonly Lazy<string> _time;

        public EventModel(IPublishedContent content)
        {
            Name = content.Name;
            Id = content.Id;
            Public = content.GetPropertyValue<bool>(EventPropertyNames.Public);
            Url = content.Url;

            _address = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Address));
            _date = new Lazy<DateTime>(() => content.GetPropertyValue<DateTime>(EventPropertyNames.Date));
            _description = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Description));
            _image = new Lazy<IPublishedContent>(() => content.GetPropertyValue<IPublishedContent>(EventPropertyNames.Image));
            _location = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Location));
            _mapQuery = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.MapQuery));
            _time = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Time));
        }

        public string Address => _address.Value;

        public DateTime Date => _date.Value;

        public string Description => _description.Value;

        public int Id { get; }

        public IPublishedContent Image => _image.Value;

        public string Location => _location.Value;

        public string MapQuery => _mapQuery.Value;

        public string Name { get; }

        public bool Public { get; }

        public string Time => _time.Value;

        public string Url { get; }
    }
}
