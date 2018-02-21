namespace ODK.Data.Payments
{
    public class PaymentRequest
    {
        public double Amount { get; set; }

        public int MemberId { get; set; }

        public string Token { get; set; }
    }
}
