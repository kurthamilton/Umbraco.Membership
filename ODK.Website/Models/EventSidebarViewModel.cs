using System;
using System.Collections.Generic;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using ODK.Umbraco.Payments;

namespace ODK.Website.Models
{
    public class EventSidebarViewModel
    {
        public int EventId { get; set; }

        public int MemberId { get; set; }

        public EventResponseType MemberResponse { get; set; }

        public Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>> MemberResponses { get; set; }
            = new Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>>();

        public EventPaymentModel EventPaymentModel { get; set; }

        public double? TicketCost { get; set; }

        public int? TicketCount { get; set; }

        public DateTime? TicketDeadline { get; set; }

        public int? TicketsRemaining { get; set; }
    }
}