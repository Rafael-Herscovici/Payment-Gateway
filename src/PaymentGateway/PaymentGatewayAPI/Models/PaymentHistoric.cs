using System;
using PaymentGatewayDB.Enums;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A historic payment response
    /// Dev note: This is a response only model and does not require validation
    /// </summary>
    public class PaymentHistoric : PaymentBase, IPaymentResponse
    {
        /// <inheritdoc cref="IPaymentResponse"/>
        public Guid PaymentId { get; set; }
        /// <inheritdoc cref="IPaymentResponse"/>
        public PaymentStatus Status { get; set; }
        /// <summary>
        /// Masked values of card details
        /// </summary>
        public CardDetails CardDetails { get; set; } = null!;
    }
}
