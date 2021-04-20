using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Services;
using System;
using System.Threading;
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
        /// <summary>
        /// Get a payment by its id
        /// </summary>
        /// <param name="dbAccess">The <see cref="DbAccess"/> service.</param>
        /// <param name="paymentId">The payment id</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>
        ///     <see cref="PaymentHistoric"/> when processed successfully
        ///     <see cref="NotFoundResult"/> result when validation fails
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<PaymentHistoric>> GetPaymentByIdAsync(
            [FromServices] DbAccess dbAccess,
            Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            // Dev note: This endpoint allow to retrieve ANY paymentId, since we have no secure way to authenticate the merchant
            // We could implement authentication and then get the merchant id from the authenticated user and only provide payments
            // that were created by that specific merchant.

            var paymentHistoric = await dbAccess.GetPaymentByIdAsync(paymentId, cancellationToken);
            if (paymentHistoric == null)
                return NotFound(new ProblemDetails
                {
                    Detail = "The requested Payment id could not be found."
                });

            return Ok(paymentHistoric);
        }

        /// <summary>
        /// Process a payment on a merchant behalf.
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="dbAccess">The <see cref="DbAccess"/> service.</param>
        /// <param name="paymentRequest">a <see cref="PaymentRequest"/> model.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>
        ///     <see cref="PaymentResponse"/> when processed successfully
        ///     <see cref="BadRequestResult"/> result when validation fails
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> ProcessPaymentAsync(
            [FromServices] ILogger<PaymentController> logger,
            [FromServices] DbAccess dbAccess,
            [FromBody] PaymentRequest paymentRequest,
            CancellationToken cancellationToken = default)
        {
            // Validate currency
            if (!await dbAccess.IsValidCurrencyAsync(paymentRequest.Currency, cancellationToken))
            {
                var supportCurrencies = await dbAccess.GetSupportedCurrenciesAsync(cancellationToken);
                ModelState.AddModelError(
                    nameof(paymentRequest.Currency),
                    $"Invalid currency, supported currencies: {string.Join(",", supportCurrencies)}.");
            }

            if (!ModelState.IsValid)
            {
                logger.LogWarning($"An invalid model state was supplied to {nameof(ProcessPaymentAsync)}.");
                return BadRequest(ModelState);
            }

            return Ok(await dbAccess.ProcessPaymentAsync(paymentRequest, cancellationToken));
        }
    }
}
