using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A payment request model
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// The merchant id of the payment request
        /// </summary>
        [Required]
        public Guid MerchantId { get; set; }

        /// <summary>
        /// The amount to charge
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// The currency to charge in (3 letter ISO 4217 code)
        /// </summary>
        [Required, StringLength(maximumLength:3, MinimumLength = 3)]
        public string Currency { get; set; }

        /// <summary>
        /// The card details used in this transaction
        /// </summary>
        [Required]
        public CardDetails CardDetails { get; set; }
    }
}
