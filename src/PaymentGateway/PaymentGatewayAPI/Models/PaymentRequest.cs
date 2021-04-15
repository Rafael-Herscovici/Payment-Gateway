using System;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A payment request model
    /// </summary>
    public class PaymentRequest
    {
        /// <inheritdoc cref="PaymentRequest" />
        public PaymentRequest(Guid merchantId)
        {
            MerchantId = merchantId;
        }

        /// <summary>
        /// The merchant id of the payment request
        /// </summary>
        public Guid MerchantId { get; }
        /// <summary>
        /// The amount to charge
        /// </summary>
        public decimal? Amount { get; set; } = null!;

        /// <summary>
        /// The card details used in this transaction
        /// </summary>
        public virtual CardDetails CardDetails { get; set; } = null!;
    }
}
