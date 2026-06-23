using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TODOAPI_Auth.DTOs.UserDTO;
using TODOAPI_Auth.Services;

namespace TODOAPI_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;

        }

        // POST /api/auth/register
        [HttpPost("register")] // POST: api/auth/register
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerDto)
        {
            // 1. Manual check for Data Annotation failures
            if (!ModelState.IsValid)
            {
                return BadRequest(BuildErrorResponse(
                    400,
                    "Validation failed",
                    GetValidationErrors()
                ));
            }

            // 2. Delegate the registration work directly to the service layer
            var response = await _authService.RegisterAsync(registerDto);

            // 3. If response is null, it means the email was already taken
            if (response == null)
            {
                return BadRequest(BuildErrorResponse(
                    400,
                    "Email already registered"
                ));
            }

            // 4. Return standard HTTP 201 Created status for new resources
            return CreatedAtAction(
                nameof(Register),
                new { email = response.Email },
                response
            );
        }

        [HttpPost("login")] // POST: api/auth/login
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BuildErrorResponse(
                    400,
                    "Validation failed",
                    GetValidationErrors()
                ));
            }

            var response = await _authService.LoginAsync(loginDto);

            if (response == null)
            {
                return Unauthorized(BuildErrorResponse(
                    401,
                    "Invalid email or password"
                ));
            }

            return Ok(response);
        }

        private object BuildErrorResponse(int statusCode, string message, object? errors = null)
        {
            return new
            {
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                Timestamp = DateTime.UtcNow
            };
        }

        private object GetValidationErrors()
        {
            return ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
        }
    }
}
