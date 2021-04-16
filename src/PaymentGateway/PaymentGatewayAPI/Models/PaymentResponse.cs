using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaymentGatewayDB.Enums;

namespace PaymentGatewayAPI.Models
{
    /// <summary>
    /// A base interface for an historic payment (shared)
    /// Dev note: This is a response only model and does not require validation
    /// </summary>
    public interface IPaymentResponse
    {
        /// <summary>
        /// The payment id
        /// </summary>
        public Guid PaymentId { get; set; }
        /// <summary>
        /// The payment status
        /// </summary>
        public PaymentStatus Status { get; set; }
    }

    /// <inheritdoc cref="IPaymentResponse"/>
    public class PaymentResponse : IPaymentResponse
    {
        /// <inheritdoc cref="IPaymentResponse"/>
        public Guid PaymentId { get; set; }

        /// <inheritdoc cref="IPaymentResponse"/>
        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus Status { get; set; } = PaymentStatus.None;
    }
}
