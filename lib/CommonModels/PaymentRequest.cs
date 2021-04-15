using System;

namespace CommonModels
{
    public class PaymentRequest : CardDetails
    {
        public Guid MerchantId { get; } = Guid.NewGuid();
        public decimal? Amount { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
    }
}
