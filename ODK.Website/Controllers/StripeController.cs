using System.Threading.Tasks;
using System.Web.Mvc;
using ODK.Payments.Stripe;
using ODK.Umbraco;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Controllers
{
    public class StripeController : OdkSurfaceControllerBase
    {
        private readonly StripePaymentProvider _provider;

        public StripeController(StripePaymentProvider provider)
        {
            _provider = provider;
        }

        [HttpPost]
        public async Task<ActionResult> Create(int id, string stripeToken)
        {
            IPublishedContent paymentContent = Umbraco.Content(id);
            PaymentModel payment = new PaymentModel(paymentContent, paymentContent.HomePage(), CurrentMemberModel);
            ServiceResult result = await _provider.MakePayment(CurrentMemberModel, payment, stripeToken);

            if (result.Success)
            {
                string message = paymentContent.GetPropertyValue<string>("successMessage");
                AddFeedback(message, result.Success);
            }
            else
            {
                AddFeedback(result.ErrorMessage, result.Success);
            }

            return RedirectToCurrentUmbracoPage();
        }
    }
}