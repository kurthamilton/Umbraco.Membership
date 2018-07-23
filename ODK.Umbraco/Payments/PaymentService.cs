using System;
using System.Collections.Generic;
using System.Linq;
using ODK.Data.Payments;
using ODK.Umbraco.Members;

namespace ODK.Umbraco.Payments
{
    public class PaymentService
    {
        private readonly OdkMemberService _memberService;
        private readonly PaymentsDataService _paymentsDataService;

        public PaymentService(PaymentsDataService paymentsDataService, OdkMemberService memberService)
        {
            _memberService = memberService;
            _paymentsDataService = paymentsDataService;
        }

        public void CreatePayment(Guid? id, MemberModel currentMember, string currencyCode, int nodeId, double amount,
            bool complete = false, MemberTypes? subscriptionType = null)
        {
            CreatePayment(id, currentMember, currencyCode, new Dictionary<int, double> { { nodeId, amount } }, complete, subscriptionType);
        }

        public void CreatePayment(Guid? id, MemberModel currentMember, string currencyCode, IEnumerable<KeyValuePair<int, double>> nodeAmounts,
            bool complete = false, MemberTypes? subscriptionType = null)
        {
            id = id ?? Guid.NewGuid();

            List<PaymentDetail> paymentDetails = new List<PaymentDetail>();
            foreach (KeyValuePair<int, double> nodeAmount in nodeAmounts)
            {
                paymentDetails.Add(new PaymentDetail(nodeAmount.Value, nodeAmount.Key, id.Value));
            }

            Payment payment = new Payment(id.Value, id.Value.ToString(), currentMember?.Id ?? 0, currencyCode, DateTime.Now, paymentDetails);
            _paymentsDataService.CreatePayment(payment);

            if (complete)
            {
                _paymentsDataService.CompletePayment(payment.Id);

                if (subscriptionType != null)
                {
                    double amount = nodeAmounts.Sum(x => x.Value);
                    _memberService.UpdateSubscription(currentMember, subscriptionType.Value, DateTime.Today.AddYears(1) - DateTime.Today, amount);
                }
            }
        }

        public void CompletePayment(string identifier)
        {
            Payment payment = _paymentsDataService.GetIncompletePayment(identifier);
            if (payment == null)
            {
                return;
            }

            _paymentsDataService.CompletePayment(payment.Id);
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
