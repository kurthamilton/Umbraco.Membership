using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ODK.Payments.Stripe;
using ODK.Umbraco;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Controllers
{
    public class StripeController : OdkSurfaceControllerBase
    {
        private readonly EventService _eventService;
        private readonly OdkMemberService _memberService;
        private readonly PaymentService _paymentService;
        private readonly StripePaymentProvider _provider;

        public StripeController(StripePaymentProvider provider, PaymentService paymentService, EventService eventService, OdkMemberService memberService)
        {
            _eventService = eventService;
            _memberService = memberService;
            _paymentService = paymentService;
            _provider = provider;
        }

        [HttpPost]
        public async Task<ActionResult> Create(int id, string stripeToken)
        {
            IPublishedContent content = Umbraco.Content(id);
            ServiceResult result;

            switch (content.DocumentTypeAlias)
            {
                case "subscription":
                    result = await MakeSubscriptionPayment(content, stripeToken);
                    break;
                case "event":
                    result = await MakeEventPayment(content, stripeToken);
                    break;
                default:
                    AddFeedback("Invalid request", false);
                    return RedirectToCurrentUmbracoPage();
            }
            
            if (result.Success)
            {
                string message = content.GetPropertyValue<string>("successMessage");
                AddFeedback(message, result.Success);
            }
            else
            {
                AddFeedback(result.ErrorMessage, result.Success);
            }

            return RedirectToCurrentUmbracoPage();
        }

        private async Task<ServiceResult> MakeSubscriptionPayment(IPublishedContent content, string stripeToken)
        {
            SubscriptionPaymentModel payment = new SubscriptionPaymentModel(content, content.HomePage(), CurrentMemberModel);

            ServiceResult result = await _provider.MakePayment(CurrentMemberModel, payment, stripeToken);

            if (result.Success)
            {
                _paymentService.CreatePayment(null, CurrentMemberModel, payment.CurrencyCode, payment.Id, payment.Amount, true);

                if (payment.SubscriptionType.HasValue)
                {
                    _memberService.UpdateSubscription(CurrentMemberModel, payment.SubscriptionType.Value, DateTime.Today.AddYears(1) - DateTime.Today, payment.Amount);
                }
            }
            
            return result;
        }

        private async Task<ServiceResult> MakeEventPayment(IPublishedContent content, string stripeToken)
        {
            EventModel @event = new EventModel(content);

            ServiceResult ticketResult = _eventService.HasTicketsAvailable(@event);
            if (!ticketResult.Success)
            {
                return ticketResult;
            }

            EventPaymentModel payment = new EventPaymentModel(content, content.HomePage(), CurrentMemberModel, @event);

            ServiceResult result = await _provider.MakePayment(CurrentMemberModel, payment, stripeToken);

            if (result.Success)
            {
                _paymentService.CreatePayment(null, CurrentMemberModel, payment.CurrencyCode, payment.Id, payment.Amount, true);
                _eventService.UpdateEventResponse(content, CurrentMember, EventResponseType.Yes);
            }

            return result;
        }
    }
}