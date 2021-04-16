using PaymentGatewayAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A payment response model
    /// </summary>
    public class PaymentResponse
    {
        /// <summary>
        /// The payment id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// The payment status
        /// </summary>
        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus Status { get; set; } = PaymentStatus.None;
    }
}
