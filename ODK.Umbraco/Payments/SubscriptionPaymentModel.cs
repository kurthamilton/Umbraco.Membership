using System;
using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Payments
{
    public class SubscriptionPaymentModel : PaymentModel
    {
        public SubscriptionPaymentModel(IPublishedContent content, IPublishedContent homePage, MemberModel member)
            : base(content, homePage, member)
        {
            Amount = content.GetPropertyValue<double>("amount");
            Description = content.GetPropertyValue<string>("description");
            Title = content.GetPropertyValue<string>("title");

            if (content.HasProperty("subscriptionType"))
            {
                SubscriptionType = (MemberTypes)Enum.Parse(typeof(MemberTypes), content.GetPropertyValue<string>("subscriptionType"));
            }            
        }

        public override double Amount { get; }

        public override string Description { get; }
        
        public MemberTypes? SubscriptionType { get; }

        public override string Title { get; }
    }
}
