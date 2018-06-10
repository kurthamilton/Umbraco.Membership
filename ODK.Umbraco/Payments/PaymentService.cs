using System;
using System.Collections.Generic;
using System.Linq;
using ODK.Data.Payments;
using ODK.Umbraco.Members;

namespace ODK.Umbraco.Payments
{
    public class PaymentService
    {
        private readonly PaymentsDataService _paymentsDataService;

        public PaymentService(PaymentsDataService paymentsDataService)
        {
            _paymentsDataService = paymentsDataService;
        }

        public void CreatePayment(Guid id, MemberModel currentMember, string currencyCode, string identifier, IEnumerable<KeyValuePair<int, double>> nodeAmounts)
        {
            List<PaymentDetail> paymentDetails = new List<PaymentDetail>();
            foreach (KeyValuePair<int, double> nodeAmount in nodeAmounts)
            {
                paymentDetails.Add(new PaymentDetail(nodeAmount.Value, nodeAmount.Key, id));
            }

            Payment payment = new Payment(id, identifier, currentMember?.Id ?? 0, currentMember?.FullName, currencyCode, DateTime.Now, paymentDetails);
            _paymentsDataService.CreatePayment(payment);
        }

        public void CompletePayment(string identifier, string currencyCode, double amount)
        {
            Payment payment = _paymentsDataService.GetIncompletePayment(identifier);
            if (payment == null)
            {
                return;
            }

            _paymentsDataService.CompletePayment(payment.Id, currencyCode, amount);
        }

        public MemberPayment GetLastPayment(int memberId)
        {
            IReadOnlyCollection<Payment> payments = _paymentsDataService.GetCompletePayments(memberId);

            Payment lastPayment = payments.OrderByDescending(x => x.Date).FirstOrDefault();
            return Map(lastPayment);
        }

        public MemberPayment GetIncompletePayment(string identifier)
        {
            Payment payment = _paymentsDataService.GetIncompletePayment(identifier);
            return Map(payment);
        }

        private static MemberPayment Map(Payment payment)
        {
            if (payment == null)
            {
                return null;
            }

            return new MemberPayment
            {
                Amount = payment.Total,
                CurrencyCode = payment.CurrencyCode,
                Date = payment.Date
            };
        }
    }
}
