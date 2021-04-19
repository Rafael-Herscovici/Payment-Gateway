using System.Threading.Tasks;
using BankEmulatorAPI.Services;
using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankEmulatorAPI.Controllers
{
    /// <summary>
    /// A controller responsible for transactions on an account
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;

        /// <inheritdoc cref="TransactionController"/>
        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tries to process a payment request for an account
        /// </summary>
        /// <param name="dbAccess">Access to database service</param>
        /// <param name="paymentRequest">A <see cref="PaymentRequest"/></param>
        /// <returns>A <see cref="PaymentStatus"/></returns>
        [HttpPost]
        public async Task<PaymentStatus> ProcessPayment(
            [FromServices] DbAccess dbAccess,
            PaymentRequest paymentRequest)
        {
            return await dbAccess.ProcessPaymentAsync(paymentRequest, HttpContext.RequestAborted);
        }
    }
}
