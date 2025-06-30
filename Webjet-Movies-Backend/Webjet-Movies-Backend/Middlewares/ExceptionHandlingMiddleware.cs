
using System.Text.Json;

namespace Webjet_Movies_Backend.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate _next;
        private ILogger<ExceptionHandlingMiddleware> _logger;
        private IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env ) 
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError("Unhandle Exception");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    message = _env.IsDevelopment() ? ex.Message : "An unexpecte error happened.",
                    details = _env.IsDevelopment() ? ex.StackTrace : null
                };

                var errorJson = JsonSerializer.Serialize(error);

                await context.Response.WriteAsync(errorJson);
            }
            // TODO catch more exceptions types and add releted HttpStatus code with proper message
        }
    }
}
