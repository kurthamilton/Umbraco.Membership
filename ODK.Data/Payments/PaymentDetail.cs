﻿using System;

namespace ODK.Data.Payments
{
    public class PaymentDetail
    {
        public PaymentDetail(double amount, int nodeId, Guid paymentId)
        {
            Amount = amount;
            NodeId = nodeId;
            PaymentId = paymentId;
        }

        public double Amount { get; }

        public int NodeId { get; }

        public Guid PaymentId { get; }
    }
}
