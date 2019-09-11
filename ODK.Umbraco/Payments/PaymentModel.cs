using ODK.Payments;
using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Payments
{
    public abstract class PaymentModel
    {
        protected PaymentModel(IPublishedContent content, IPublishedContent homePage, MemberModel member)
        {
            ApiPublicKey = homePage.GetPropertyValue<string>("paymentsApiPublicKey");
            ApiSecretKey = homePage.GetPropertyValue<string>("paymentsApiSecretKey");
            CurrencyCode = homePage.GetPropertyValue<string>("paymentsCurrencyCode");            
            Email = member.Email;
            Id = content.Id;
            Provider = homePage.GetPropertyValue<string>("paymentsProvider");
            SiteName = homePage.GetPropertyValue<string>(PropertyNames.SiteName);
        }

        public abstract double Amount { get; }

        public string ApiPublicKey { get; }

        public string ApiSecretKey { get; }

        public string CurrencyCode { get; }

        public string CurrencyString => PaymentsHelper.ToCurrencyString(CurrencyCode, Amount);

        public abstract string Description { get; }

        public string Email { get; }

        public int Id { get; }

        public string Provider { get; }

        public string SiteName { get; }        

        public abstract string Title { get; }
    }
}
