using System;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A card details model
    /// </summary>
    public class CardDetails
    {
        /// <summary>
        /// The card number
        /// </summary>
        public string Number { get; set; } = null!;

        /// <summary>
        /// The card expiry date
        /// </summary>
        public DateTime? ExpiryDate { get; set; } = null!;

        /// <summary>
        /// The CVV of the card
        /// </summary>
        public string CVV { get; set; } = null!;
    }
}
