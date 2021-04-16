using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankEmulatorAPI.Controllers
{
    /// <summary>
    /// A Bank controller emulator
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly ILogger<BankController> _logger;

        /// <inheritdoc cref="BankController"/>
        public BankController(ILogger<BankController> logger)
        {
            _logger = logger;
        }
    }
}
