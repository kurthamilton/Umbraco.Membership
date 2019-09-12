using System.Collections.Generic;
using System.Threading.Tasks;
using ODK.Umbraco.Members;
using ODK.Umbraco.Payments;
using Stripe;
using Stripe.Checkout;

namespace ODK.Payments.Stripe
{
    public class StripePaymentProvider
    {
        private readonly PaymentService _paymentService;

        public StripePaymentProvider(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<string> CreatePayment(MemberModel member, PaymentModel payment, string successUrl, string cancelUrl)
        {
            StripeConfiguration.ApiKey = payment.ApiSecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Name = payment.Title,
                        Description = payment.Description,
                        Amount = (int)(payment.Amount * 100),
                        Currency = payment.CurrencyCode,
                        Quantity = 1
                    },
                },
                CustomerEmail = member.Email,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Id;
        }              
    }
}
