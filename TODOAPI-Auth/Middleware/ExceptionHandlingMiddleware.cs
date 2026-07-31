using TODOAPI_Auth.Exceptions;

namespace TODOAPI_Auth.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
                // Logs exceptions with full details
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Evaluates exception types using pattern matching switch rules
            var response = exception switch
            {
                NotFoundException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                },
                ValidationException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Errors = ex.Errors,
                    Timestamp = DateTime.UtcNow
                },
                UnauthorizedException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                },
                ForbiddenException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                },
                ConflictException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                },
                ApiException ex => new GlobalErrorResponse
                {
                    StatusCode = ex.StatusCode,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                },
                _ => new GlobalErrorResponse
                {
                    StatusCode = 500,
                    Message = "An internal server error occurred",
                    Timestamp = DateTime.UtcNow
                }
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response);
        }

    }
    // Custom internal model layout naming prevents conflict with DTO layer classes
    public class GlobalErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public Dictionary<string, string[]>? Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
