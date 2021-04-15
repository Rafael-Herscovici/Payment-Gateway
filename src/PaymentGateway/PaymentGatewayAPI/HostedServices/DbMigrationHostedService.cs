using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using PaymentGatewayDB;
using Microsoft.EntityFrameworkCore;

namespace PaymentGatewayAPI.HostedServices
{
    /// <summary>
    /// Apply database migrations (or create from scratch)
    /// </summary>
    public class DbMigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DbMigrationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var myDbContext = scope.ServiceProvider.GetRequiredService<PaymentGatewayDbContext>();
            await myDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
