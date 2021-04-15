using System.ComponentModel.DataAnnotations;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A debit/credit card details
    /// </summary>
    public class CardDetails
    {
        /// <summary>
        /// The card number
        /// </summary>
        /// <remarks>There are alogritms to calculate if a card number is valid, not implemented.</remarks>
        [Required, StringLength(maximumLength: 16, MinimumLength = 16)]
        public string CardNumber { get; set; }

        /// <summary>
        /// The card expiry date, must be in MM/yy format
        /// </summary>
        [Required, StringLength(maximumLength: 5, MinimumLength = 5)]
        public string CardExpiryDate { get; set; }

        /// <summary>
        /// The CardSecurityCode of the card
        /// </summary>
        [Required, StringLength(maximumLength: 3, MinimumLength = 3)]
        public string CardSecurityCode { get; set; }
    }
}
