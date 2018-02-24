using System;

namespace ODK.Umbraco.Payments
{
    public class MemberPayment
    {
        public double Amount { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime Date { get; set; }
    }
}
