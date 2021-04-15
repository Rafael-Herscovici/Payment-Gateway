﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGatewayAPI.Models;

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
        /// Process a payment on a merchant behalf.
        /// </summary>
        /// <param name="paymentRequest">a <see cref="PaymentRequest"/> model.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(PaymentRequest paymentRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
    }
}