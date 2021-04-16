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
            services.AddDbContext<PaymentGatewayDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(nameof(PaymentGatewayDB)),
                    x => x.MigrationsAssembly(nameof(PaymentGatewayDB))));
            return services;
        }
    }
}
