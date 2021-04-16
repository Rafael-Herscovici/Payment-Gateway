using PaymentGatewayDB.Migrations;
using System;
using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace PaymentGatewayDB.Entities
{
    public class PaymentRequestEntity : BaseEntity
    {
        public Guid PaymentId { get; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        // Dev note: there is not EF way to force a value
        [Required(AllowEmptyStrings = false)]
        public string CardDetails { get; set; } = null!;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.None;
    }
}
