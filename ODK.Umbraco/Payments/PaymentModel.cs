using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Payments
{
    public class PaymentModel
    {
        public PaymentModel(IPublishedContent content, IPublishedContent homePage, MemberModel member)
        {
            Amount = content.GetPropertyValue<double>("amount");
            ApiPublicKey = homePage.GetPropertyValue<string>("paymentsApiPublicKey");
            ApiSecretKey = homePage.GetPropertyValue<string>("paymentsApiSecretKey");
            CurrencyCode = homePage.GetPropertyValue<string>("paymentsCurrencyCode");
            CurrencySymbol = GetCurrencySymbol(CurrencyCode);
            Description = content.GetPropertyValue<string>("description");
            Email = member.Email;
            Id = content.Id;
            Provider = homePage.GetPropertyValue<string>("paymentsProvider");
            SiteName = homePage.GetPropertyValue<string>(PropertyNames.SiteName);
            Title = content.GetPropertyValue<string>("title");
        }

        public double Amount { get; }

        public string ApiPublicKey { get; }

        public string ApiSecretKey { get; }

        public string CurrencyCode { get; }

        public string CurrencySymbol { get; }

        public string Description { get; }

        public string Email { get; }

        public int Id { get; }

        public string Provider { get; }

        public string SiteName { get; }

        public string Title { get; }

        private static string GetCurrencySymbol(string currencyCode)
        {
            switch (currencyCode)
            {
                case "AUD":
                case "USD":
                    return "$";
                case "EUR":
                    return "€";
                case "GBP":
                    return "£";
            }

            return null;
        }
    }
}
