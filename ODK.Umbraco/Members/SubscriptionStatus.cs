using System;

namespace ODK.Umbraco.Members
{
    [Flags]
    public enum SubscriptionStatus
    {
        None = 0,
        Current = 1,
        Expiring = 2,
        Expired = 4
    }
}
