using System;

namespace ODK.Data.Payments
{
    public class Payment
    {
        public Payment(int memberId, string memberName, string currencyCode, double amount, Guid token)
            : this(0, memberId, memberName, currencyCode, amount, DateTime.MinValue)
        {
            Token = token;
        }

        public Payment(int id, int memberId, string memberName, string currencyCode, double amount, DateTime date)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
            Date = date;
            Id = id;
            MemberId = memberId;
            MemberName = memberName;
        }


        public double Amount { get; }

        public string CurrencyCode { get; }

        public DateTime Date { get; }

        public int Id { get; }

        public int MemberId { get; }

        public string MemberName { get; }

        public Guid Token { get; }
    }
}
