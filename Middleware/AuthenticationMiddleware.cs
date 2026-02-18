namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for validating token-based authentication
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        // Valid tokens for testing/demo purposes
        private static readonly HashSet<string> ValidTokens = new()
        {
            "Bearer token-12345",
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9",
            "Bearer demo-token-xyz"
        };

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for health check and OpenAPI endpoints
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/openapi") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Check for Authorization header
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                _logger.LogWarning($"[AUTH] Missing authorization header for {context.Request.Method} {context.Request.Path}");
                await ReturnUnauthorizedResponse(context);
                return;
            }

            var token = authHeader.ToString();

            // Validate the token
            if (!ValidTokens.Contains(token))
            {
                _logger.LogWarning($"[AUTH] Invalid token provided for {context.Request.Method} {context.Request.Path}");
                await ReturnUnauthorizedResponse(context);
                return;
            }

            _logger.LogInformation($"[AUTH] Valid token authenticated for {context.Request.Method} {context.Request.Path}");
            await _next(context);
        }

        private static Task ReturnUnauthorizedResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var response = new
            {
                status = StatusCodes.Status401Unauthorized,
                message = "Unauthorized",
                detail = "Missing or invalid authorization token. Please provide a valid Bearer token."
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
