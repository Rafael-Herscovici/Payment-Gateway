using DapperWrapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DbAccess.Models;
using Microsoft.Extensions.Options;

namespace DbAccess
{
    public class DbAccess
    {
        private readonly ILogger<DbAccess> _logger;
        private readonly DapperAbstraction _dapperAbstraction;
        private readonly string _paymentGatewayConnectionString;

        public DbAccess(
            IOptionsMonitor<DbAccessOptions> dbAccessOptions,
            ILogger<DbAccess> logger,
            DapperAbstraction dapperAbstraction)
        {
            _logger = logger;
            _dapperAbstraction = dapperAbstraction;
            _paymentGatewayConnectionString = dbAccessOptions.CurrentValue.PaymentGatewayConnectionString;
        }

        
    }
}
