using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TODOAPI_Auth.DTOs.TODODTO;
using TODOAPI_Auth.Services;

namespace TODOAPI_Auth.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")] // Routes requests to: api/todoitems
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoItemsController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet("all")] // GET: api/todoitems/all
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            try
            {
                int userId = GetUserIdFromClaims();
                var result = await _todoService.GetAllAsync(userId, status);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(BuildErrorResponse(401, ex.Message));
            }
        }

        [HttpGet] // GET: api/todoitems?status=completed&priority=high&page=1&pageSize=10
        public async Task<IActionResult> GetTodos(
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            int userId = GetUserIdFromClaims();
            var result = await _todoService.GetTodosAsync(userId, status, priority, sortBy, sortOrder, page, pageSize);
            return Ok(result);
        }

        [HttpGet("statistics")] // GET: api/todoitems/statistics
        public async Task<IActionResult> GetStatistics()
        {
            int userId = GetUserIdFromClaims();
            var result = await _todoService.GetStatisticsAsync(userId);
            return Ok(result);
        }

        [HttpGet("search")] // GET: api/todoitems/search?q=groceries
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            int userId = GetUserIdFromClaims();
            var result = await _todoService.SearchAsync(userId, q);
            return Ok(result);
        }

        [HttpGet("{id}")] // GET: api/todoitems/5
        public async Task<IActionResult> GetById(int id)
        {
            int userId = GetUserIdFromClaims();
            var result = await _todoService.GetByIdAsync(userId, id);

            if (result == null)
                return NotFound(BuildErrorResponse(404, "Todo item not found or you do not have permission to view it."));

            return Ok(result);
        }

        [HttpPost] // POST: api/todoitems
        public async Task<IActionResult> Create([FromBody] CreateTodoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BuildErrorResponse(400, "Validation failed", GetValidationErrors()));

            int userId = GetUserIdFromClaims();
            var result = await _todoService.CreateAsync(userId, dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")] // PUT: api/todoitems/5
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BuildErrorResponse(400, "Validation failed", GetValidationErrors()));

            int userId = GetUserIdFromClaims();
            var result = await _todoService.UpdateAsync(userId, id, dto);

            if (result == null)
                return NotFound(BuildErrorResponse(404, "Todo item not found or you do not have permission to edit it."));

            return Ok(result);
        }

        [HttpDelete("{id}")] // DELETE: api/todoitems/5
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserIdFromClaims();
            bool success = await _todoService.DeleteAsync(userId, id);

            if (!success)
                return NotFound(BuildErrorResponse(404, "Todo item not found or you do not have permission to delete it."));

            return NoContent(); 
        }



        //private int GetUserIdFromClaims()
        //{
        //    // Extracts the "NameIdentifier" claim embedded into the JWT token inside JwtHelper.cs
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        //    {
        //        throw new UnauthorizedAccessException("User identification token is missing or corrupted.");
        //    }

        //    return userId;
        //}
        private int GetUserIdFromClaims()
        {
            // 1. Check if the User identity context or claims exist at all
            if (User?.Identity?.IsAuthenticated != true)
            {
                // This stops execution safely and tells .NET to handle it
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // 2. Locate the specific NameIdentifier claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 3. Defensive Validation: Validate that it exists and can be parsed as a number
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User identification token is missing or invalid.");
            }

            return userId; // Returns safely if everything passes
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
