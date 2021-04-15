using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayDB;
using PaymentGatewayDB.Entities;

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

        /// <summary>
        /// Provides database access for the payment gateway api
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbContext"></param>
        public DbAccess(
            ILogger<DbAccess> logger,
            PaymentGatewayDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Processes a payment request
        /// </summary>
        /// <param name="paymentRequestModel">a <see cref="PaymentRequestModel"/> model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        public virtual async Task ProcessPaymentAsync(
            PaymentRequestModel paymentRequestModel,
            CancellationToken   cancellationToken)
        {
            // Dev note: i normally use AutoMapper for mapping models to entities and vice versa,
            // Since not many entities/models are included in this task, i skipped it.
            var paymentRequestEntity = new PaymentRequestEntity
            {
                MerchantId = paymentRequestModel.MerchantId,
                Amount = paymentRequestModel.Amount,
                Currency =  paymentRequestModel.Currency,
                CardDetails = ""
            };
            await _dbContext.AddAsync(paymentRequestEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
