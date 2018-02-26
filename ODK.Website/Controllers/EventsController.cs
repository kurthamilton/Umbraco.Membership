using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using ODK.Umbraco.Web.Mvc;
using ODK.Website.Models;

namespace ODK.Website.Controllers
{
    public class EventsController : OdkSurfaceControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        [ChildActionOnly]
        [HttpGet]
        public ActionResult EventSidebar(int eventId)
        {
            return EventSidebarView(eventId);
        }

        [HttpPost]
        public ActionResult EventSidebar(int eventId, EventResponseType responseType)
        {
            if (CurrentMemberModel == null)
            {
                return RedirectToCurrentUmbracoPage();
            }

            _eventService.UpdateEventResponse(Umbraco.TypedContent(eventId), CurrentMember, responseType);

            return EventSidebarView(eventId);
        }

        private ActionResult EventSidebarView(int eventId)
        {
            if (CurrentMemberModel == null)
            {
                return RedirectToCurrentUmbracoPage();
            }

            Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>> responses = _eventService.GetEventResponses(eventId, Umbraco);

            EventResponseType memberResponse = EventResponseType.None;
            foreach (EventResponseType key in responses.Keys)
            {
                if (responses[key].Any(x => x.Id == CurrentMember.Id))
                {
                    memberResponse = key;
                    break;
                }
            }

            EventSidebarViewModel viewModel = new EventSidebarViewModel
            {
                EventId = eventId,
                MemberResponse = memberResponse,
                MemberResponses = responses
            };

            return PartialView(viewModel);
        }
    }
}