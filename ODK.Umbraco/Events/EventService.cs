using System;
using System.Collections.Generic;
using System.Linq;
using ODK.Data.Events;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Events
{
    public class EventService
    {
        private readonly EventsDataService _eventDataService;

        public EventService(EventsDataService eventsDataService)
        {
            _eventDataService = eventsDataService;
        }

        public IEnumerable<EventModel> SearchEvents(IPublishedContent eventsPage, IPublishedContent member, EventSearchCriteria criteria)
        {
            IEnumerable<EventModel> events = eventsPage.Children
                                                       .Select(x => new EventModel(x))
                                                       .Where(x => member != null || x.Public);

            if (criteria.FutureOnly == true)
            {
                events = events.Where(x => x.Date >= DateTime.Today);
            }

            if (criteria.Month != null)
            {
                events = events.Where(x => x.Date.Month == criteria.Month.Value);
            }

            if (criteria.MaxItems > 0)
            {
                events = events.Take(criteria.MaxItems.Value);
            }

            return events;
        }
    }
}
