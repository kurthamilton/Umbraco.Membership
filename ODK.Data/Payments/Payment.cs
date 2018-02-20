using System;

namespace ODK.Data.Payments
{
    public class Payment
    {
        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }
    }
}
