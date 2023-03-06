using AccountManagementAPI.Exceptions;
using AccountManagementAPI.Models;
using Newtonsoft.Json;
using System.Net;

namespace AccountManagementAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var errorDetails = new ErrorDetails()
            {
                Message = ex.Message,
                Source = ex.Source
            };

            switch (ex)
            {
                case AccountManagementAPIException e:
                    errorDetails.StatusCode = e.StatusCode;
                    errorDetails.Message = e.Message;
                    break;
                
                case KeyNotFoundException e:
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case BadRequestException e:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    errorDetails.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.LogError($"ExceptionHandlingMiddleware : {errorDetails.Message} - statuscode: {errorDetails.StatusCode}");
            var response = httpContext.Response;
            response.ContentType = "application/json";

            await response.WriteAsync(JsonConvert.SerializeObject(errorDetails));
        }
    }
}
