using System.ComponentModel.DataAnnotations;
using Common.Models;

namespace PaymentGatewayAPI.Models
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
