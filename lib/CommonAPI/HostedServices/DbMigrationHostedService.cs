using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonAPI.HostedServices
{
    /// <inheritdoc />
    public class DbMigrationHostedService<TDbContext> : IHostedService
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Apply database migrations (or create from scratch)
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
            var myDbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await myDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
