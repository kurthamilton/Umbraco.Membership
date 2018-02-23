using System;
using ODK.Data.Payments;

namespace ODK.Umbraco.Payments
{
    public class PaymentService
    {
        private readonly PaymentsDataService _paymentsDataService;

        public PaymentService(PaymentsDataService paymentsDataService)
        {
            _paymentsDataService = paymentsDataService;
        }

        public void CreatePaymentRequest(int memberId, string memberName, double amount, Guid token, string secret)
        {
            _paymentsDataService.CreatePaymentRequest(new PaymentRequest
            {
                Amount = amount,
                MemberId = memberId,
                MemberName = memberName,
                Token = token,
                Secret = secret
            });
        }

        public void CompletePayment(Guid token, string secret)
        {
            PaymentRequest paymentRequest = _paymentsDataService.GetPaymentRequest(token, secret);
            if (paymentRequest == null)
            {
                return;
            }

            _paymentsDataService.LogPayment(new Payment
            {
                Amount = paymentRequest.Amount,
                Date = DateTime.UtcNow,
                MemberId = paymentRequest.MemberId,
                MemberName = paymentRequest.MemberName
            });

            _paymentsDataService.DeletePaymentRequest(paymentRequest);
        }
    }
}
