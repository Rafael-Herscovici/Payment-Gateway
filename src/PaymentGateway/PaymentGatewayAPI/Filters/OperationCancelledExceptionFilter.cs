using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace PaymentGatewayAPI.Filters
{
    /// <summary>
    /// Handles <see cref="CancellationToken"/> Operation Cancelled exceptions
    /// </summary>
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<OperationCancelledExceptionFilter> _logger;

        /// <inheritdoc />
        public OperationCancelledExceptionFilter(ILogger<OperationCancelledExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is OperationCanceledException)) 
                return;

            _logger.LogInformation(context.Exception, "The operation was aborted.");
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(400);
        }
    }
}
