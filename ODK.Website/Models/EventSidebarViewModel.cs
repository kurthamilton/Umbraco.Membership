﻿using System.Collections.Generic;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;

namespace ODK.Website.Models
{
    public class EventSidebarViewModel
    {
        public int EventId { get; set; }

        public int MemberId { get; set; }

        public EventResponseType MemberResponse { get; set; }

        public Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>> MemberResponses { get; set; }
            = new Dictionary<EventResponseType, IReadOnlyCollection<MemberModel>>();
    }
}