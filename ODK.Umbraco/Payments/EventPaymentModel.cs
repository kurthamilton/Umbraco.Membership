﻿using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ODK.Umbraco.Payments
{
    public class EventPaymentModel : PaymentModel
    {
        public EventPaymentModel(IPublishedContent content, IPublishedContent homePage, MemberModel member, EventModel @event)
            : base(content, homePage, member)
        {
            Amount = content.GetPropertyValue<double>("eventTicketCost");
            Description = @event.Name;
            Title = @event.Name;
        }

        public override double Amount { get; }

        public override string Description { get; }
        
        public override string Title { get; }
    }
}
