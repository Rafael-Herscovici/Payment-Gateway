using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Services;
using System.Threading.Tasks;
using Common.Models;

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
        }

        /// <summary>
        /// Get a payment by its id
        /// </summary>
        /// <param name="dbAccess">The <see cref="DbAccess"/> service.</param>
        /// <param name="paymentId">The payment id</param>
        /// <returns>
        ///     <see cref="PaymentHistoric"/> when processed successfully
        ///     <see cref="NotFoundResult"/> result when validation fails
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<PaymentHistoric>> GetPaymentById(
            [FromServices] DbAccess dbAccess,
            Guid paymentId)
        {
            // Dev note: This endpoint allow to retrieve ANY paymentId, since we have no secure way to authenticate the merchant
            // We could implement authentication and then get the merchant id from the authenticated user and only provide payments
            // that were created by that specific merchant.

            var paymentHistoric = await dbAccess.GetPaymentByIdAsync(paymentId);
            if (paymentHistoric == null)
                return NotFound(new ProblemDetails
                {
                    Title = "The requested Payment id could not be found."
                });

            return Ok(paymentHistoric);
        }

        /// <summary>
        /// Process a payment on a merchant behalf.
        /// </summary>
        /// <param name="dbAccess">The <see cref="DbAccess"/> service.</param>
        /// <param name="paymentRequest">a <see cref="PaymentRequest"/> model.</param>
        /// <returns>
        ///     <see cref="PaymentResponse"/> when processed successfully
        ///     <see cref="BadRequestResult"/> result when validation fails
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> ProcessPaymentAsync(
            [FromServices] DbAccess dbAccess,
            [FromBody] PaymentRequest paymentRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"An invalid model state was supplied to {nameof(ProcessPaymentAsync)}.");
                return BadRequest(ModelState);
            }

            // Dev note: This will throw an exception anywhere down the pipeline if cancellation was requested.
            // We have the operation cancelled exception middleware to handle those.
            HttpContext.RequestAborted.ThrowIfCancellationRequested();

            return Ok(await dbAccess.ProcessPaymentAsync(paymentRequest, HttpContext.RequestAborted));
        }
    }
}
