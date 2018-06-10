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

            Identifier = request["custom"];

            Enum.TryParse(request["payment_status"], out PaymentStatus paymentStatus);
            PaymentStatus = paymentStatus;
        }

        public double Amount { get; }

        public string CurrencyCode { get; }

        public string Identifier { get; }

        public PaymentStatus PaymentStatus { get; }
    }
}