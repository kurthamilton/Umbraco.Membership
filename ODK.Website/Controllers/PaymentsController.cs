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
        public ActionResult Start(double amount)
        {
            int memberId = CurrentMember.Id;
            Guid token = Guid.NewGuid();
            string secret = CurrentMemberModel.Chapter.GetPropertyValue<string>("paypalIpnSecret");
            _paymentService.CreatePaymentRequest(memberId, CurrentMemberModel.FullName, amount, token, secret);

            return Json(token);
        }
    }
}