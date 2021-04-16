using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayDB;
using PaymentGatewayDB.Entities;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        /// <param name="paymentRequestModel">a <see cref="PaymentRequestModel"/> model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        public virtual async Task ProcessPaymentAsync(
            PaymentRequestModel paymentRequestModel,
            CancellationToken cancellationToken)
        {
            // Dev note: Model mappings should have a class of their own (or use Automapper)
            var paymentRequestEntity = new PaymentRequestEntity
            {
                MerchantId = paymentRequestModel.MerchantId,
                Amount = paymentRequestModel.Amount,
                Currency = paymentRequestModel.Currency,
                CardDetails = _encryption.Encrypt(JsonConvert.SerializeObject(paymentRequestModel))
            };
            await _dbContext.AddAsync(paymentRequestEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
