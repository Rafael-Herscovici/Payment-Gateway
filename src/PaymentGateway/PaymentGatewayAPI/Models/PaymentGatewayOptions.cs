namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// Payment gateway options
    /// </summary>
    public class PaymentGatewayOptions
    {
        /// <summary>
        /// The payment gateway db connection string
        /// </summary>
        public string ConnectionString { get; set; } = null!;
    }
}
