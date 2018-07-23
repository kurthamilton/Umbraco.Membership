using System;
using ODK.Payments;
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
            CurrencyString = PaymentsHelper.ToCurrencyString(CurrencyCode, Amount);
            Description = content.GetPropertyValue<string>("description");
            Email = member.Email;
            Id = content.Id;
            Provider = homePage.GetPropertyValue<string>("paymentsProvider");
            SiteName = homePage.GetPropertyValue<string>(PropertyNames.SiteName);

            if (content.HasProperty("subscriptionType"))
            {
                SubscriptionType = (MemberTypes)Enum.Parse(typeof(MemberTypes), content.GetPropertyValue<string>("subscriptionType"));
            }

            Title = content.GetPropertyValue<string>("title");
        }

        public double Amount { get; }

        public string ApiPublicKey { get; }

        public string ApiSecretKey { get; }

        public string CurrencyCode { get; }

        public string CurrencyString { get; }

        public string Description { get; }

        public string Email { get; }

        public int Id { get; }

        public string Provider { get; }

        public string SiteName { get; }

        public MemberTypes? SubscriptionType { get; }

        public string Title { get; }
    }
}
