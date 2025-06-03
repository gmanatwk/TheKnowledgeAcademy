namespace MyFirstWebApp.Middleware
{
    public class ProcessingTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProcessingTimeMiddleware> _logger;

        public ProcessingTimeMiddleware(RequestDelegate next, ILogger<ProcessingTimeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request information
            var requestInfo = $"Request: {context.Request.Method} {context.Request.Path} " +
                            $"from {context.Connection.RemoteIpAddress}";
            _logger.LogInformation(requestInfo);

           

            // Record start time
            var startTime = DateTime.UtcNow;
            var elapsed = DateTime.UtcNow - startTime;
             context.Response.Headers.Add("X-ElapsedTime", elapsed.TotalMilliseconds.ToString());
            // Call the next middleware
            await _next(context);

            // Log response information
            //var elapsed = DateTime.UtcNow - startTime;
            var responseInfo = $"Response: {context.Response.StatusCode} in {elapsed.TotalMilliseconds}ms";
            _logger.LogInformation(responseInfo);
        }
    }

    // Extension method to make it easy to use
    public static class ProcessingTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseProcessingTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProcessingTimeMiddleware>();
        }
    }
}