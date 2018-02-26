using System;
using System.Web;

namespace ODK.Website.Models.Payments
{
    public class PaypalNotification
    {
        public PaypalNotification(HttpRequestBase request)
        {
            double.TryParse(request["mc_gross"], out double amount);
            Amount = amount;

            CurrencyCode = request["mc_currency"];

            Enum.TryParse(request["payment_status"], out PaymentStatus paymentStatus);
            PaymentStatus = paymentStatus;

            Guid.TryParse(request["custom"], out Guid token);
            Token = token;
        }

        public double Amount { get; }

        public string CurrencyCode { get; }

        public PaymentStatus PaymentStatus { get; }

        public Guid Token { get; }
    }
}