namespace TODOAPI_Auth.Exceptions
{
    // Base exception
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public ApiException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    // Specific exceptions
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message)
            : base(message, 404) { }
        public NotFoundException(string entity, object id)
            : base($"{entity} with ID {id} not found", 404) { }
    }

    public class ValidationException : ApiException
    {
        public Dictionary<string, string[]>? Errors { get; set; }
        public ValidationException(string message, Dictionary<string, string[]>? errors = null)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message = "Unauthorized access")
            : base(message, 401) { }
    }

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "Access forbidden")
            : base(message, 403) { }
    }

    public class ConflictException : ApiException
    {
        public ConflictException(string message)
            : base(message, 409) { }
    }
}
