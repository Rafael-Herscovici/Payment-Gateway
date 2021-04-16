using AutoMapper;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayDB;
using PaymentGatewayDB.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IMapper _mapper;

        /// <summary>
        /// Provides database access for the payment gateway api
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="dbContext">The db context</param>
        /// <param name="mapper">Mapper service</param>
        public DbAccess(
            ILogger<DbAccess> logger,
            PaymentGatewayDbContext dbContext,
            IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Processes a payment request
        /// </summary>
        /// <param name="paymentRequestModel">a <see cref="PaymentRequest"/> model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        public virtual async Task<PaymentResponse> ProcessPaymentAsync(
            PaymentRequest paymentRequestModel,
            CancellationToken cancellationToken = default)
        {
            var paymentRequestEntity = _mapper.Map<PaymentRequestEntity>(paymentRequestModel);
            await _dbContext.AddAsync(paymentRequestEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PaymentResponse>(paymentRequestEntity);
        }

        /// <summary>
        /// Gets an historic payment by id
        /// </summary>
        /// <param name="paymentId">The payment id</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="PaymentHistoric"/> model.</returns>
        public virtual async Task<PaymentHistoric?> GetPaymentByIdAsync(
            Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            var paymentEntity = await _dbContext.PaymentRequests.FindAsync(paymentId);
            // Dev note: we could implement Null object pattern on paymentHistoric
            if (paymentEntity == null)
                return null;

            return _mapper.Map<PaymentHistoric>(paymentEntity);
        }
    }
}
