namespace ODK.Payments
{
    public class PaymentDetails
    {
        public double Amount { get; set; }

        public string CancelUrl { get; set; }

        public string CurrencyCode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
