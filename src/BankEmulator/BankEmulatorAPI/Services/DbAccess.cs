using System.Threading.Tasks;
using AutoMapper;
using BankEmulatorDB;
using Microsoft.Extensions.Logging;

namespace BankEmulatorAPI.Services
{
    /// <summary>
    /// Provides database access for the bank emulator api
    /// </summary>
    public class DbAccess
    {
        private readonly ILogger<DbAccess> _logger;
        private readonly BankEmulatorDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Provides database access for the bank emulator api
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="dbContext">The db context</param>
        /// <param name="mapper">Mapper service</param>
        public DbAccess(
            ILogger<DbAccess> logger,
            BankEmulatorDbContext dbContext,
            IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public virtual async Task ProcessPaymentAsync()
        {

        }
    }
}
