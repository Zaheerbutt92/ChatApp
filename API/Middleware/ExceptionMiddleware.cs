using System;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

                var response = _env.IsDevelopment() 
                    ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode,"Internal Server Error");

                var options = new JsonSerializerOptions{ PropertyNamingPolicy= JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response,options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}