using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODK.Data.Events;
using Umbraco.Core.Models;
using Umbraco.Web;
using Task = System.Threading.Tasks.Task;

namespace ODK.Umbraco.Events
{
    public class EventService
    {
        private readonly EventsDataService _eventDataService;

        public EventService(EventsDataService eventsDataService)
        {
            _eventDataService = eventsDataService;
        }

        public Dictionary<int, EventResponseType> GetEventResponses(int eventId)
        {
            // TODO - make async
            IReadOnlyCollection<EventResponse> responses = Task.Run(() => _eventDataService.GetEventResponses(eventId)).Result;
            return responses.ToDictionary(k => k.MemberId, v => (EventResponseType)v.ResponseTypeId);
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

        public async Task UpdateEventResponse(IPublishedContent @event, IPublishedContent member, EventResponseType responseType)
        {
            if (@event == null)
            {
                return;
            }

            if (!Enum.IsDefined(typeof(EventResponseType), responseType) || responseType == EventResponseType.None)
            {
                return;
            }

            EventModel eventModel = new EventModel(@event);
            if (eventModel.Date == DateTime.MinValue)
            {
                return;
            }

            await _eventDataService.UpdateEventResponse(new EventResponse
            {
                EventId = @event.Id,
                MemberId = member.Id,
                ResponseTypeId = (int)responseType
            });
        }
    }
}
