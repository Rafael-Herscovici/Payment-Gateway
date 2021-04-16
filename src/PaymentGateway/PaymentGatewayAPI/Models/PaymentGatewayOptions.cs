using Microsoft.OpenApi.Models;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// Payment gateway options
    /// </summary>
    public class PaymentGatewayOptions
    {
        /// <summary>
        /// The aes key used for encrypt/decrypt
        /// </summary>
        public string AesEncryptionKey { get; set; } = null!;
    }
}
