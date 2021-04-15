using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Services;
using System.Threading.Tasks;

namespace PaymentGatewayAPI.Controllers
{
    /// <summary>
    /// Responsible for receiving payment requests for a merchant
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        /// <inheritdoc />
        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;

            // Dev note: This will throw an exception anywhere down the pipeline if cancellation was requested.
            // We have the operation cancelled exception filter to handle those.
            HttpContext.RequestAborted.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Process a payment on a merchant behalf.
        /// </summary>
        /// <param name="dbAccess">The <see cref="DbAccess"/> service.</param>
        /// <param name="paymentRequest">a <see cref="PaymentRequestModel"/> model.</param>
        /// <returns>
        ///     <see cref="OkResult"/> when processed successfully
        ///     <see cref="BadRequestResult"/> result when validation fails
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> ProcessPaymentAsync(
            [FromServices] DbAccess dbAccess,
            [FromBody]     PaymentRequestModel paymentRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"An invalid model state was supplied to {nameof(ProcessPaymentAsync)}.");
                return BadRequest(ModelState);
            }

            await dbAccess.ProcessPaymentAsync(paymentRequest, HttpContext.RequestAborted);

            return Ok();
        }
    }
}
