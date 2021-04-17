using System.ComponentModel.DataAnnotations;
using Common.Attributes;

namespace Common.Models
{
    /// <summary>
    /// A debit/credit card details
    /// </summary>
    public class CardDetails
    {
        /// <summary>
        /// The card number
        /// </summary>
        [Required]
        [CreditCard]
        public string CardNumber { get; set; } = null!;

        /// <summary>
        /// The card expiry date, must be in MM/yy format
        /// </summary>
        [Required]
        [CardExpiryDateValidator]
        public string CardExpiryDate { get; set; } = null!;

        // Dev note: we should really not be saving this value on the Gateway,
        // See SECURITY BENEFITS on https://en.wikipedia.org/wiki/Card_security_code

        /// <summary>
        /// The CardSecurityCode of the card
        /// </summary>
        [Required]
        [StringLength(maximumLength: 3, MinimumLength = 3)]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "Must be a 3 digit code")]
        public string CardSecurityCode { get; set; } = null!;
    }
}
