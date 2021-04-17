using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    /// <summary>
    /// A payment request model
    /// </summary>
    public class PaymentRequest : PaymentBase
    {
        /// <summary>
        /// The card details used in this transaction
        /// </summary>
        [Required]
        public CardDetails CardDetails { get; set; } = null!;
    }
}
