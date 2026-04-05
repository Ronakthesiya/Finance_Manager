using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Finance_Manager_Core.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception Occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>
            {
                Success = false,
                Message = GetMessage(ex)
            };

            context.Response.StatusCode = (int)GetStatusCode(ex);

            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }

        private HttpStatusCode GetStatusCode(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private string GetMessage(Exception ex)
        {
            return ex switch
            {
                ArgumentException => ex.Message,
                KeyNotFoundException => ex.Message,
                _ => "Something went wrong. Please try again later."
            };
        }
    }
}
