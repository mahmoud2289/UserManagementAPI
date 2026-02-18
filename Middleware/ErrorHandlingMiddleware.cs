namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for handling unhandled exceptions and returning consistent error responses
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (Exception exception)
            {
                _logger.LogError($"An unhandled exception occurred: {exception.Message}");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                status = context.Response.StatusCode,
                message = "An error occurred while processing your request.",
                detail = exception.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
