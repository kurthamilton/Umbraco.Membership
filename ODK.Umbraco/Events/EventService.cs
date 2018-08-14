using System;
using System.Collections.Generic;
using System.Linq;
using ODK.Data.Events;
using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Umbraco.Events
{
    public class EventService
    {
        private readonly IContentService _contentService;
        private readonly EventsDataService _eventDataService;

        public EventService(EventsDataService eventsDataService, IContentService contentService)
        {
            _contentService = contentService;
            _eventDataService = eventsDataService;
        }

        public ServiceResult CreateEvent(IPublishedContent chapter, int userId, string name, string location, DateTime date, string time, string address,
            string mapQuery, string description)
        {
            IPublishedContent eventsPage = chapter.GetPropertyValue<IPublishedContent>("eventsPage");

            IContent @event = _contentService.CreateContent(name, eventsPage.Id, "event", userId);

            @event.SetValue(EventPropertyNames.Location, location);
            @event.SetValue(EventPropertyNames.Date, date);

            if (!string.IsNullOrEmpty(time))
            {
                @event.SetValue(EventPropertyNames.Time, time);
            }

            if (!string.IsNullOrEmpty(address))
            {
                @event.SetValue(EventPropertyNames.Address, address);
            }

            if (!string.IsNullOrEmpty(mapQuery))
            {
                @event.SetValue(EventPropertyNames.MapQuery, mapQuery);
            }

            if (!string.IsNullOrEmpty(description))
            {
                @event.SetValue(EventPropertyNames.Description, description);
            }

            @event.SetValue(EventPropertyNames.Public, false);

            _contentService.Publish(@event, userId);

            return new ServiceResult(true);
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

        public IEnumerable<EventModel> GetNextEvents(IPublishedContent eventsPage, IPublishedContent currentMember, int maxItems = 0)
        {
            EventSearchCriteria criteria = new EventSearchCriteria
            {
                FutureOnly = true,
                MaxItems = maxItems
            };

            IEnumerable<EventModel> nextEvents = SearchEvents(eventsPage, currentMember, criteria);
            return nextEvents;
        }

        public void LogSentEventInvite(int eventId, UmbracoHelper helper)
        {
            IContent @event = _contentService.GetById(eventId);
            @event.SetValue(EventPropertyNames.InviteSentDate, DateTime.Now);
            _contentService.SaveAndPublishWithStatus(@event);
        }

        public IEnumerable<EventModel> SearchEvents(IPublishedContent eventsPage, IPublishedContent member, EventSearchCriteria criteria)
        {
            IEnumerable<EventModel> events = eventsPage.Children
                                                       .Select(x => new EventModel(x, ReplaceEventProperties))
                                                       .Where(x => member != null || x.Public)
                                                       .OrderBy(x => x.Date);

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

            EventModel eventModel = new EventModel(@event, ReplaceEventProperties);
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

        private static string ReplaceEventProperties(string text, EventModel eventModel)
        {
            return text
                .Replace("{{Name}}", eventModel.Name)
                .Replace("{{eventUrl}}", eventModel.Url)
                .Replace($"{{{{{EventPropertyNames.Address}}}}}", eventModel.Address)
                .Replace($"{{{{{EventPropertyNames.Date}}}}}", eventModel.Date.ToString("dddd dd MMMM, yyyy"))
                .Replace($"{{{{{EventPropertyNames.Description}}}}}", eventModel.Description)
                .Replace($"{{{{{EventPropertyNames.Location}}}}}", eventModel.Location)
                .Replace($"{{{{{EventPropertyNames.Time}}}}}", eventModel.Time);

        }
    }
}
