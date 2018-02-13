using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Events
{
    public class EventService
    {
        private readonly IPublishedContent _eventsPage;
        private readonly IPublishedContent _member;

        public EventService(IPublishedContent eventsPage, IPublishedContent member)
        {
            _eventsPage = eventsPage;
            _member = member;
        }

        public IEnumerable<EventModel> SearchEvents(EventSearchCriteria criteria)
        {
            IEnumerable<EventModel> events = _eventsPage.Children
                                                        .Select(x => new EventModel(x))
                                                        .Where(x => _member != null || x.Public);

            if (criteria.FutureOnly == true)
            {
                events = events.Where(x => x.Date >= DateTime.Today);
            }

            if (criteria.Month != null)
            {
                events = events.Where(x => x.Date.Month == criteria.Month.Value);
            }

            if (criteria.PageSize > 0)
            {
                events = events.Take(criteria.PageSize.Value);
            }

            return events;
        }
    }
}
