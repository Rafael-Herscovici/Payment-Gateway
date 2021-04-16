using PaymentGatewayDB.Enums;
using System;

namespace PaymentGatewayAPI.Services
{
    /// <summary>
    /// An interface for the bank service
    /// </summary>
    public interface IBankService
    {
        /// <summary>
        /// Assuming that whatever bank service it is, it has a ProcessPayment method
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="amount">The amount</param>
        /// <param name="currency">The currency</param>
        /// <returns>A <see cref="PaymentStatus"/>, Please note, in a real world scenario, we probably should not reuse the enum.</returns>
        PaymentStatus ProcessPayment(Guid userId, decimal amount, string currency);
    }

    /// <inheritdoc cref="IBankService"/>
    public class BankService : IBankService
    {
        public PaymentStatus ProcessPayment(Guid userId, decimal amount, string currency)
        {
            return PaymentStatus.Success;
        }
    }
}
