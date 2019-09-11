using System.Threading.Tasks;
using ODK.Umbraco;
using ODK.Umbraco.Members;
using ODK.Umbraco.Payments;
using Stripe;

namespace ODK.Payments.Stripe
{
    public class StripePaymentProvider
    {
        private readonly PaymentService _paymentService;

        public StripePaymentProvider(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<ServiceResult> MakePayment(MemberModel member, PaymentModel payment, string stripeToken)
        {
            var options = new StripeChargeCreateOptions
            {
                Amount = (int)(payment.Amount * 100),
                Currency = payment.CurrencyCode,
                Description = payment.Title,
                ReceiptEmail = member.Email,
                SourceTokenOrExistingSourceId = stripeToken
            };

            var service = new StripeChargeService();
            StripeCharge charge = await service.CreateAsync(options, new StripeRequestOptions
            {
                ApiKey = payment.ApiSecretKey
            });

            bool success = charge.Paid;            
            return new ServiceResult(success, charge.FailureMessage);
        }
    }
}
