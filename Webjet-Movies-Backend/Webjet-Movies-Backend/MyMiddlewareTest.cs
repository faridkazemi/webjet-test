//namespace Webjet_Movies_Backend
//{
//    using Microsoft.AspNetCore.Http;
//    using Microsoft.Extensions.Logging;
//    using System.Threading.Tasks;

//    public class RequestLoggingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<RequestLoggingMiddleware> _logger;

//        // Constructor to inject RequestDelegate and ILogger
//        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        // This method is called by the ASP.NET Core pipeline
//        public async Task InvokeAsync(HttpContext context)
//        {
//            _logger.LogInformation($"[{context.TraceIdentifier}] Incoming Request: {context.Request.Method} {context.Request.Path}");

//            // Log the Origin header specifically for CORS debugging
//            if (context.Request.Headers.ContainsKey("Origin"))
//            {
//                _logger.LogInformation($"[{context.TraceIdentifier}] Origin Header: {context.Request.Headers["Origin"]}");
//            }
//            else
//            {
//                _logger.LogInformation($"[{context.TraceIdentifier}] Origin Header: Not present in request.");
//            }

//            // You can also log other relevant headers if needed
//            // For example, to check the Host header that Nginx might be forwarding
//            // if (context.Request.Headers.ContainsKey("Host"))
//            // {
//            //     _logger.LogInformation($"[{context.TraceIdentifier}] Host Header: {context.Request.Headers["Host"]}");
//            // }
//            // if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
//            // {
//            //     _logger.LogInformation($"[{context.TraceIdentifier}] X-Forwarded-For Header: {context.Request.Headers["X-Forwarded-For"]}");
//            // }


//            // Call the next middleware in the pipeline
//            await _next(context);
//        }
//    }
//}
