using System;

namespace ODK.Data.Payments
{
    public class PaymentRequest
    {
        public double Amount { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public string Secret { get; set; }

        public Guid Token { get; set; }
    }
}
