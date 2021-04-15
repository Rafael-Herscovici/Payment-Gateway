using System;

namespace CommonModels
{
    public class PaymentRequest : CardDetails
    {
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
