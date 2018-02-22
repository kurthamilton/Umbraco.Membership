using System.Threading.Tasks;
using System.Web.Mvc;
using ODK.Umbraco.Events;
using ODK.Umbraco.Web.Mvc;

namespace ODK.Website.Controllers
{
    public class EventsController : OdkSurfaceControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        public async Task<ActionResult> Respond(int eventId, EventResponseType responseType)
        {
            if (CurrentMemberModel == null)
            {
                return RedirectToCurrentUmbracoPage();
            }

            await _eventService.UpdateEventResponse(Umbraco.TypedContent(eventId), CurrentMember, responseType);

            return RedirectToCurrentUmbracoPage();
        }
    }
}