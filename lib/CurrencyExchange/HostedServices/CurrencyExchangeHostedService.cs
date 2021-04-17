using CurrencyExchange.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange.HostedServices
{
    public class CurrencyExchangeHostedService<TDbContext> : IHostedService, IDisposable
        where TDbContext : CurrencyExchangeDbContext
    {
        private int _executionCount;
        private Timer _timer = null!;

        private const string ServiceName = nameof(CurrencyExchangeHostedService<TDbContext>);
        private readonly ILogger<CurrencyExchangeHostedService<TDbContext>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CurrencyExchangeHostedService(
            IServiceProvider serviceProvider,
            ILogger<CurrencyExchangeHostedService<TDbContext>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{ServiceName} Service running.");

            using var scope = _serviceProvider.CreateScope();
            var currencyExchangeDbContext =
                scope.ServiceProvider.GetRequiredService<TDbContext>();
            await currencyExchangeDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            if (!currencyExchangeDbContext.Currencies.Any())
            {
                _logger.LogInformation($"{ServiceName} Seeding base EUR Currency.");
                await currencyExchangeDbContext.AddAsync(new CurrencyEntity
                {
                    Currency = "EUR",
                    Rate = 1
                }, cancellationToken);
                await currencyExchangeDbContext.SaveChangesAsync(cancellationToken);
            }

            _timer = new Timer(FetchCurrencies, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));
        }

        private void FetchCurrencies(object? state)
        {
            var count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation(
                "{ServiceName} Service is working. Count: {Count}", ServiceName, count);


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{ServiceName} Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
