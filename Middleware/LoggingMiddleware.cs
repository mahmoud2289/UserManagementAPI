namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for logging incoming requests and outgoing responses
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request
            var request = context.Request;
            var logMessage = $"[REQUEST] {request.Method} {request.Path}";
            _logger.LogInformation(logMessage);

            // Store the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream to capture the response
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    // Call the next middleware
                    await _next(context);

                    // Log outgoing response
                    var statusCode = context.Response.StatusCode;
                    var responseLogMessage = $"[RESPONSE] {request.Method} {request.Path} - Status: {statusCode}";
                    _logger.LogInformation(responseLogMessage);

                    // Reset stream position to beginning before copying
                    responseBody.Position = 0;
                    
                    // Copy the response body back to the original stream
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }
    }
}
