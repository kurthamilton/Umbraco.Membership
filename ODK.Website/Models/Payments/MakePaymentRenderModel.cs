using ODK.Umbraco.Payments;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace ODK.Website.Models.Payments
{
    public class RedirectToCheckoutModel : RenderModel
    {
        public RedirectToCheckoutModel(IPublishedContent content)
            : base(content)
        {
        }

        public PaymentModel Payment { get; set; }

        public string PaymentId { get; set; }
    }
}