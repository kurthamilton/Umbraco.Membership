using System;
using System.Web.Mvc;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Web.Mvc;
using Umbraco.Web;

namespace ODK.Website.Controllers
{
    public class PaymentsController : OdkSurfaceControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentervice)
        {
            _paymentService = paymentervice;
        }

        [HttpPost]
        public ActionResult Start(double amount, string currencyCode)
        {
            int memberId = CurrentMember.Id;
            Guid token = Guid.NewGuid();
            _paymentService.CreatePayment(memberId, CurrentMemberModel.FullName, currencyCode, amount, token);

            return Json(token);
        }
    }
}