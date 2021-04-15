using Microsoft.Extensions.Logging;
using PaymentGatewayDB;

namespace PaymentGatewayAPI.Services
{
    /// <summary>
    /// Provides database access for the payment gateway api
    /// </summary>
    public class DbAccess
    {
        private readonly ILogger<DbAccess> _logger;

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

        }
    }
}
