using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankEmulator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly ILogger<BankController> _logger;

        public BankController(ILogger<BankController> logger)
        {
            _logger = logger;
        }
    }
}
