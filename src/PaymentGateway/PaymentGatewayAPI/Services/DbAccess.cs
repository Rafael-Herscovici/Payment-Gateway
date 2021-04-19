using AutoMapper;
using Common.Entities;
using Common.Models;
using CurrencyExchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayDB;
using PaymentGatewayDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMapper _mapper;
        private readonly IBankAccess _bankAccess;
        private readonly PaymentGatewayDbContext _gatewayDbContext;
        private readonly CurrencyExchangeDbContext _exchangeDbContext;

        /// <summary>
        /// Provides database access for the payment gateway api
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="gatewayDbContext">The gateway db context</param>
        /// <param name="currencyExchangeDbContext">The exchange db context</param>
        /// <param name="mapper">Mapper service</param>
        /// <param name="bankAccess">Access to the <see cref="BankAccess"/> http client</param>
        public DbAccess(
            ILogger<DbAccess> logger,
            IMapper mapper,
            IBankAccess bankAccess,
            PaymentGatewayDbContext gatewayDbContext,
            CurrencyExchangeDbContext currencyExchangeDbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _bankAccess = bankAccess;
            _gatewayDbContext = gatewayDbContext;
            _exchangeDbContext = currencyExchangeDbContext;
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
            try
            {
                var paymentRequestEntity = _mapper.Map<PaymentRequestEntity>(paymentRequestModel);
                paymentRequestEntity.PaymentStatus = await _bankAccess.ProcessPaymentAsync(paymentRequestModel, cancellationToken);
                await _gatewayDbContext.AddAsync(paymentRequestEntity, cancellationToken);
                await _gatewayDbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<PaymentResponse>(paymentRequestEntity);
            }
            catch (Exception ex)
            {
                // Dev note: we just throw the exception and let it bubble up, we just log it so we are aware of it.
                _logger.LogError(ex, "An error occured while trying to process the transaction.");
                throw;
            }
        }

        /// <summary>
        /// Gets an historic payment by id
        /// </summary>
        /// <param name="paymentId">The payment id</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        /// <returns>A <see cref="PaymentHistoric"/> model.</returns>
        public virtual async Task<PaymentHistoric?> GetPaymentByIdAsync(
            Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            var paymentEntity = await _gatewayDbContext.PaymentRequests.FindAsync(paymentId);
            return paymentEntity == null
                ? null
                : _mapper.Map<PaymentHistoric>(paymentEntity);
        }

        /// <summary>
        /// Check if a currency 3 char code is valid
        /// </summary>
        /// <param name="currency">The currency 3 letter code to check</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        /// <returns>as <see cref="bool"/></returns>
        public virtual Task<bool> IsValidCurrencyAsync(
            string currency,
            CancellationToken cancellationToken = default)
        {
            return _exchangeDbContext.Currencies.AnyAsync(x =>
                x.Currency == currency,
                cancellationToken);
        }

        /// <summary>
        /// Get a List of supported currencies
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to abort the request.</param>
        /// <returns>a list of <see cref="CurrencyEntity"/></returns>
        public virtual Task<List<string>> GetSupportedCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            return _exchangeDbContext.Currencies.Select(x => x.Currency).ToListAsync(cancellationToken);
        }
    }
}
