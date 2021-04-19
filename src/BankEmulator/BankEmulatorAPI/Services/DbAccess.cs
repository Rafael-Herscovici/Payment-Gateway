using AutoMapper;
using BankEmulatorDB;
using Common;
using Common.Enums;
using Common.Models;
using CurrencyExchange;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BankEmulatorAPI.Services
{
    /// <summary>
    /// Provides database access for the bank emulator api
    /// </summary>
    /// <remarks>The default currency in the bank account is EUR</remarks>
    public class DbAccess
    {
        private readonly ILogger<DbAccess> _logger;
        private readonly BankEmulatorDbContext _bankDbContext;
        private readonly CurrencyExchangeDbContext _exchangeDbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Provides database access for the bank emulator api
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="bankDbContext">The db context</param>
        /// <param name="mapper">Mapper service</param>
        public DbAccess(
            ILogger<DbAccess> logger,
            BankEmulatorDbContext bankDbContext,
            CurrencyExchangeDbContext exchangeDbContext,
            IMapper mapper)
        {
            _logger = logger;
            _bankDbContext = bankDbContext;
            _exchangeDbContext = exchangeDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Processes a payment
        /// </summary>
        /// <param name="paymentRequest">A <see cref="PaymentRequest"/></param>
        /// <param name="cancellationToken">A cancellation token to abort the request</param>
        /// <returns>A <see cref="PaymentStatus"/></returns>
        public virtual async Task<PaymentStatus> ProcessPaymentAsync(
            PaymentRequest paymentRequest,
            CancellationToken cancellationToken = default)
        {
            var account = await _bankDbContext.Accounts.FindAsync(
                paymentRequest.CardDetails.CardNumber,
                paymentRequest.CardDetails.CardExpiryDate,
                paymentRequest.CardDetails.CardSecurityCode);

            var chargeAmount = paymentRequest.Amount;

            if (paymentRequest.Currency != Constants.DefaultCurrency)
                chargeAmount *= await GetCurrencyRate(paymentRequest.Currency);

            if (account.Balance < chargeAmount)
                return PaymentStatus.Failed;

            account.Balance -= chargeAmount;

            await _bankDbContext.SaveChangesAsync(cancellationToken);

            return PaymentStatus.Success;

            async Task<decimal> GetCurrencyRate(string currency)
            {
                var currencyEntity = await _exchangeDbContext.Currencies.FindAsync(currency);
                return currencyEntity.Rate;
            }
        }
    }
}
