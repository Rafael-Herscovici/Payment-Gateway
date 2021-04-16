using System;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayDB;
using PaymentGatewayDB.Entities;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGatewayDB.Enums;

namespace PaymentGatewayAPI.Services
{
    // DEV NOTE: Since entity framework is built with the repository pattern,
    // There is not point in creating a repository, instead we can just create a service as below.

    /// <summary>
    /// Provides database access for the payment gateway api
    /// </summary>
    public class DbAccess
    {
        private readonly ILogger<DbAccess> _logger;
        private readonly PaymentGatewayDbContext _dbContext;
        private readonly Encryption _encryption;

        /// <summary>
        /// Provides database access for the payment gateway api
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="dbContext">The dbcontext</param>
        /// <param name="encryption">Encryption service</param>
        public DbAccess(
            ILogger<DbAccess> logger,
            PaymentGatewayDbContext dbContext,
            Encryption encryption)
        {
            _logger = logger;
            _dbContext = dbContext;
            _encryption = encryption;
        }

        /// <summary>
        /// Processes a payment request
        /// </summary>
        /// <param name="paymentRequestModel">a <see cref="PaymentRequest"/> model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        public virtual async Task<PaymentResponse> ProcessPaymentAsync(
            PaymentRequest paymentRequestModel,
            CancellationToken cancellationToken)
        {
            // Dev note: Model mappings should have a class of their own (or use Automapper)
            var paymentRequestEntity = new PaymentRequestEntity
            {
                MerchantId = paymentRequestModel.MerchantId,
                Amount = paymentRequestModel.Amount,
                Currency = paymentRequestModel.Currency,
                CardDetails = _encryption.Encrypt(JsonConvert.SerializeObject(paymentRequestModel.CardDetails)),
                PaymentStatus = PaymentStatus.Success
            };
            await _dbContext.AddAsync(paymentRequestEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new PaymentResponse
            {
                PaymentId = paymentRequestEntity.PaymentId,
                Status = PaymentStatus.Success
            };
        }

        /// <summary>
        /// Gets an historic payment by id
        /// </summary>
        /// <param name="paymentId">The payment id</param>
        /// <returns>A <see cref="PaymentHistoric"/> model.</returns>
        public virtual async Task<PaymentHistoric?> GetPaymentByIdAsync(Guid paymentId)
        {
            var paymentEntity = await _dbContext.PaymentRequests.FindAsync(paymentId);
            // Dev note: we could implement Null object pattern on paymentHistoric
            if (paymentEntity == null)
                return null;

            var cardDetails = JsonConvert.DeserializeObject<CardDetails>(
                _encryption.Decrypt(paymentEntity.CardDetails));
            return new PaymentHistoric
            {
                PaymentId = paymentEntity.PaymentId,
                Status = paymentEntity.PaymentStatus,
                MerchantId = paymentEntity.MerchantId,
                Amount = paymentEntity.Amount,
                Currency = paymentEntity.Currency,
                CardDetails = new CardDetails
                {
                    CardNumber = MaskValue(cardDetails.CardNumber, 12),
                    // Dev note: am not sure why we return these two, they could be ommited
                    CardExpiryDate = MaskExpiryDate(cardDetails.CardExpiryDate),
                    CardSecurityCode = MaskValue(cardDetails.CardSecurityCode, 3)
                }
            };

            string MaskExpiryDate(string str)
            {
                if (string.IsNullOrEmpty(str) || str.Length != 5 || !str.Contains('-'))
                    return str;
                var split = str.Split('-');
                return $"{MaskValue(split[0], 2)}-{MaskValue(split[1], 2)}";
            }

            static string MaskValue(string str, int maskCount) =>
                string.IsNullOrEmpty(str) || str.Length < maskCount
                    ? str
                    : string.Concat(new string('*', maskCount), str.Substring(maskCount));
        }
    }
}
