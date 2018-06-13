using System.Linq;
using System.Web.Mvc;
using ODK.Umbraco.Events;
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
            EventSidebarViewModel viewModel = new EventSidebarViewModel
            {
                EventId = eventId
            };

            if (CurrentMember != null)
            {
                viewModel.MemberId = CurrentMember.Id;
                viewModel.MemberResponses = _eventService.GetEventResponses(eventId, Umbraco);

                foreach (EventResponseType key in viewModel.MemberResponses.Keys)
                {
                    if (viewModel.MemberResponses[key].Any(x => x.Id == CurrentMember.Id))
                    {
                        viewModel.MemberResponse = key;
                        break;
                    }
                }
            }

            return PartialView(viewModel);
        }
    }
}