using Common.Entities;
using Common.Enums;
using System;

namespace PaymentGatewayDB.Entities
{
    public class PaymentRequestEntity : BaseEntity
    {
        public Guid PaymentId { get; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string CardDetails { get; set; } = null!;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.None;
    }
}
