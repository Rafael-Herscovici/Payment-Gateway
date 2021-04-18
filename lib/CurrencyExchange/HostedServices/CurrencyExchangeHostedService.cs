using CurrencyExchange.Entities;
using CurrencyExchange.HostedServices.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Internal;

namespace CurrencyExchange.HostedServices
{
    public class CurrencyExchangeHostedService : IHostedService, IDisposable
    {
        private int _executionCount;
        private Timer _timer = null!;

        // Dev note: While this should be in settings, we act as this service is a 3th party one, so its just inlined.
        private const string ExternalServiceUri = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private const string ServiceName = nameof(CurrencyExchangeHostedService);
        private const string DefaultCurrency = "EUR";
        private readonly ILogger<CurrencyExchangeHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CurrencyExchangeHostedService(
            IServiceProvider serviceProvider,
            ILogger<CurrencyExchangeHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{ServiceName} Service running.");

            using var scope = _serviceProvider.CreateScope();
            var currencyExchangeDbContext =
                scope.ServiceProvider.GetRequiredService<CurrencyExchangeDbContext>();
            await currencyExchangeDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

            if (await currencyExchangeDbContext.Currencies.FindAsync(DefaultCurrency) == null)
            {
                _logger.LogInformation($"{ServiceName} Service is seeding {DefaultCurrency} currency.");
                await currencyExchangeDbContext.AddAsync(new CurrencyEntity
                {
                    Currency = DefaultCurrency,
                    Rate = 1
                }, cancellationToken);
                await currencyExchangeDbContext.SaveChangesAsync(cancellationToken);
            }

            _timer = new Timer(obj => FetchCurrenciesAsync(obj, cancellationToken), null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10));
        }

        private async void FetchCurrenciesAsync(object? state, CancellationToken cancellationToken = default)
        {
            var count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation(
                "{ServiceName} Service is working. Count: {Count}", ServiceName, count);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            var response = client.GetAsync(ExternalServiceUri, cancellationToken).Result;

            response.EnsureSuccessStatusCode();

            var xmlSerializer = new XmlSerializer(typeof(Envelope));
            var stringResult = await response.Content.ReadAsStreamAsync();
            var xmlObj = (Envelope)xmlSerializer.Deserialize(stringResult);
            using var scope = _serviceProvider.CreateScope();
            var currencyExchangeDbContext =
                scope.ServiceProvider.GetRequiredService<CurrencyExchangeDbContext>();
            foreach (var entry in xmlObj.Cube.Cube1.Cube)
            {
                var existingCurrency = await currencyExchangeDbContext.Currencies.FindAsync(new object[] { entry.currency }, cancellationToken);
                if (existingCurrency == null)
                    currencyExchangeDbContext.Add(new CurrencyEntity
                    {
                        Currency = entry.currency,
                        Rate = entry.rate
                    });
                else
                    existingCurrency.Rate = entry.rate;
            }

            await currencyExchangeDbContext.SaveChangesAsync(cancellationToken);
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
