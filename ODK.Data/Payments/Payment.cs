using System;
using System.Collections.Generic;
using System.Linq;

namespace ODK.Data.Payments
{
    public class Payment
    {
        public Payment(Guid id, string identifier, int? memberId, string memberName, string currencyCode, DateTime date, IEnumerable<PaymentDetail> details)
        {
            CurrencyCode = currencyCode;
            Date = date;
            Details = details.ToArray();
            Id = id;
            Identifier = identifier;
            MemberId = memberId;
            MemberName = memberName;
        }

        public string CurrencyCode { get; }

        public DateTime Date { get; }

        public IReadOnlyCollection<PaymentDetail> Details { get; }

        public Guid Id { get; }

        public string Identifier { get; }

        public int? MemberId { get; }

        public string MemberName { get; }

        public double Total => Details.Sum(x => x.Amount);
    }
}
