using System;
using System.Collections.Generic;
using System.Linq;
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

        public void CreatePayment(int memberId, string memberName, string currencyCode, double amount, Guid token)
        {
            Payment payment = new Payment(memberId, memberName, currencyCode, amount, token);
            _paymentsDataService.CreatePayment(payment);
        }

        public void CompletePayment(Guid token, string currencyCode, double amount)
        {
            Payment payment = _paymentsDataService.GetIncompletePayment(token);
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
            if (lastPayment == null)
            {
                return null;
            }

            return new MemberPayment
            {
                Amount = lastPayment.Amount,
                CurrencyCode = lastPayment.CurrencyCode,
                Date = lastPayment.Date
            };
        }
    }
}
