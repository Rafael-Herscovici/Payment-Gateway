using System;

namespace CommonModels
{
    public class PaymentRequest : CardPayment
    {
        public Guid CorrelationId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
