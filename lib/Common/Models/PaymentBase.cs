using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    /// <summary>
    /// A payment base model (shared)
    /// </summary>
    public class PaymentBase
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

        // Dev Note: in a real world app, we probably would have dictionary with the available currencies
        // and would validate the user input is a valid currency.

        /// <summary>
        /// The currency to charge in (3 letter ISO 4217 code)
        /// </summary>
        [Required, StringLength(maximumLength: 3, MinimumLength = 3)]
        public string Currency { get; set; } = null!;
    }
}
