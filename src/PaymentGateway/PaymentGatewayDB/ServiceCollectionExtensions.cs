using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGatewayDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(nameof(PaymentGatewayDB));
            services.AddDbContext<PaymentGatewayDbContext>(options =>
                options.UseSqlServer(connectionString, x => 
                    x.MigrationsAssembly(nameof(PaymentGatewayDB))));
            return services;
        }
    }
}
