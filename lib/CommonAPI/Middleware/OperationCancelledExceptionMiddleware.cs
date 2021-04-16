using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CommonAPI.Middleware
{
#pragma warning disable CS1591
    public class OperationCancelledExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger<OperationCanceledException> _logger = null!;

        public OperationCancelledExceptionMiddleware(
            RequestDelegate next,
            ILogger<OperationCanceledException> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) when (ex is OperationCanceledException)
            {
                await HandleOperationCancelledExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleOperationCancelledExceptionAsync(HttpContext context, Exception exception)
        {
            const int status = (int) HttpStatusCode.BadRequest;
            var problem = new ProblemDetails
            {
                Detail = "The operation was aborted.",
                Status = status
            };
            _logger.LogInformation(exception, problem.Detail);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(problem));
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            const string msg = "Internal server error.";
            _logger.LogError(exception, msg);

            context.Response.ContentType = "plain/text";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(msg);
        }
    }
#pragma warning restore CS1591
}
