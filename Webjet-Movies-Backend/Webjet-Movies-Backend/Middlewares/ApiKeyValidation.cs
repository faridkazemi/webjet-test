using Microsoft.Extensions.Options;
using Webjet_Movies_Backend.ConfigOptions;

namespace Webjet_Movies_Backend.Middlewares
{
    public class ApiKeyValidationMiddleware
    {
        private RequestDelegate _next;
        private MoviesBackendApiConfigurationOption _configs;

        public ApiKeyValidationMiddleware(RequestDelegate next,
            IOptions<MoviesBackendApiConfigurationOption> options)
        {
            _next = next;
            _configs = options.Value;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var apikey = context.Request.Headers["x-api-key"];

            if (apikey != _configs.ApiKey || string.IsNullOrEmpty(apikey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }

            await _next.Invoke(context);

        }
    }
}
