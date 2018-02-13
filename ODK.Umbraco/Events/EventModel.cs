﻿using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Events
{
    public class EventModel
    {
        private readonly Lazy<DateTime> _date;
        private readonly Lazy<string> _description;
        private readonly Lazy<string> _location;
        private readonly Lazy<string> _mapQuery;

        public EventModel(IPublishedContent content)
        {
            Name = content.Name;
            Public = content.GetPropertyValue<bool>(EventPropertyNames.Public);
            Url = content.Url;

            _date = new Lazy<DateTime>(() => content.GetPropertyValue<DateTime>(EventPropertyNames.Date));
            _description = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Description));
            _location = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.Location));
            _mapQuery = new Lazy<string>(() => content.GetPropertyValue<string>(EventPropertyNames.MapQuery));
        }

        public DateTime Date => _date.Value;

        public string Description => _description.Value;

        public string Location => _location.Value;

        public string MapQuery => _mapQuery.Value;

        public string Name { get; }

        public bool Public { get; }

        public string Url { get; }
    }
}
