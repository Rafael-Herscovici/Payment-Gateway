using System;

namespace PaymentGatewayDB.Entities
{
    public class PaymentRequest
    {
        public Guid PaymentId { get; set; }
        public Guid MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        // Dev note: Even encrypted, this probably should be stored in a more secure database then
        // storing it in the Apis database
        public string CardDetails { get; set; }
    }
}
