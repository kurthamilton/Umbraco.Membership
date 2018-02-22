using System;
using System.Collections.Generic;
using System.Linq;
using ODK.Data.Events;
using ODK.Umbraco.Members;
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

        public Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>> GetEventResponses(int eventId, UmbracoHelper helper)
        {
            IReadOnlyCollection<EventResponse> responses = _eventDataService.GetEventResponses(eventId);

            Dictionary<EventResponseType, List<MemberModel>> dictionary = new Dictionary<EventResponseType, List<MemberModel>>();
            foreach (EventResponse response in responses)
            {
                EventResponseType responseType = (EventResponseType)response.ResponseTypeId;
                if (!dictionary.ContainsKey(responseType))
                {
                    dictionary.Add(responseType, new List<MemberModel>());
                }

                IPublishedContent member = helper.TypedMember(response.MemberId);
                if (member != null)
                {
                    MemberModel memberModel = new MemberModel(member);
                    dictionary[responseType].Add(memberModel);
                }
            }

            return dictionary.ToDictionary(x => x.Key, x => (IReadOnlyCollection<MemberModel>)x.Value.ToArray());
        }

        public Dictionary<int, EventResponseType> GetMemberResponses(int memberId, IEnumerable<int> eventIds)
        {
            IReadOnlyCollection<EventResponse> responses = _eventDataService.GetMemberResponses(memberId, eventIds);
            return responses.ToDictionary(x => x.EventId, x => (EventResponseType)x.ResponseTypeId);
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

        public void UpdateEventResponse(IPublishedContent @event, IPublishedContent member, EventResponseType responseType)
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

            _eventDataService.UpdateEventResponse(new EventResponse
            {
                EventId = @event.Id,
                MemberId = member.Id,
                ResponseTypeId = (int)responseType
            });
        }
    }
}
