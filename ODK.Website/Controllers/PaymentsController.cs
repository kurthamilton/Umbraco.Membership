using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using ODK.Payments;
using ODK.Umbraco.Payments;
using ODK.Umbraco.Settings;
using ODK.Umbraco.Web.Mvc;
using ODK.Website.Models.PayPal;
using PayPal.Api;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Website.Controllers
{
    public class PaymentsController : OdkSurfaceControllerBase
    {
        private static readonly PayPalPaymentService _payPalPaymentService = new PayPalPaymentService();
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentervice)
        {
            _paymentService = paymentervice;
        }

        [HttpPost]
        public ActionResult Create(PayPalButtonViewModel viewModel)
        {
            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            string currencyCode = chapter.GetPropertyValue<string>("payPalCurrencyCode");

            KeyValuePair<int, double>[] amounts = GetNodeAmounts(viewModel).ToArray();
            double total = amounts.Sum(x => x.Value);

            string returnUrl = Request.Url + "?success=true";
            string cancelUrl = Request.Url + "?cancelled=true";

            Guid id = Guid.NewGuid();

            Payment payment = CreatePayPalPayment(chapter, id, currencyCode, total, returnUrl, cancelUrl);
            if (payment == null)
            {
                return Json(null);
            }

            PayPalCredentials credentials = GetPayPalCredentials(chapter);

            JObject response = _payPalPaymentService.CreatePayment(credentials, payment);

            string identifier = response.GetValue("id").Value<string>();
            _paymentService.CreatePayment(id, CurrentMemberModel, currencyCode, identifier, amounts);

            return Content(response.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult Execute(string paymentId, string payerId)
        {
            IPublishedContent chapter = Umbraco.AssignedContentItem.HomePage();

            PayPalCredentials credentials = GetPayPalCredentials(chapter);

            MemberPayment memberPayment = _paymentService.GetIncompletePayment(paymentId);
            if (memberPayment == null)
            {
                return new JsonResult();
            }

            PaymentExecution payment = new PaymentExecution
            {
                payer_id = payerId
            };

            _payPalPaymentService.ExecutePayment(credentials, paymentId, payment);

            return new JsonResult();
        }

        private static Payment CreatePayPalPayment(IPublishedContent chapter, Guid id, string currencyCode, double total, string returnUrl, string cancelUrl)
        {
            if (total == 0)
            {
                return null;
            }

            return new Payment
            {
                intent = "sale",
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                },
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = CreateTransactions(total, currencyCode, id)
            };
        }

        private static List<Transaction> CreateTransactions(double total, string currencyCode, Guid id)
        {
            return new List<Transaction>
            {
                new Transaction
                {
                    custom = id.ToString(),
                    amount = new Amount
                    {
                        total = total.ToString(),
                        currency = currencyCode
                    }
                }
            };
        }

        private IEnumerable<KeyValuePair<int, double>> GetNodeAmounts(PayPalButtonViewModel viewModel)
        {
            foreach (PayPalOptionViewModel optionViewModel in viewModel.Options)
            {
                PayPalOptionViewModel option = new PayPalOptionViewModel(Umbraco.TypedContent(optionViewModel.Id));

                for (int i = 0; i < optionViewModel.Quantity; i++)
                {
                    yield return new KeyValuePair<int, double>(option.Id, option.Amount);
                }
            }
        }

        private PayPalCredentials GetPayPalCredentials(IPublishedContent chapter)
        {
            return new PayPalCredentials
            {
                ClientId = chapter.GetPropertyValue<string>("payPalApiUsername"),
                ClientSecret = chapter.GetPropertyValue<string>("payPalApiPassword")
            };
        }
    }
}