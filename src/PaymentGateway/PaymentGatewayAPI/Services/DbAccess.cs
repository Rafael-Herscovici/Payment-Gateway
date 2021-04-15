using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Models;
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
        /// <param name="dbAccessOptions"></param>
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
