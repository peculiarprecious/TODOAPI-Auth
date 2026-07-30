using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TODOAPI_Auth.DTOs.AttachmentDTO;
using TODOAPI_Auth.Helpers;
using TODOAPI_Auth.Models;
using TODOAPI_Auth.Services;

namespace TODOAPI_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }
        // 1. UPLOAD ATTACHMENT ENDPOINT
        [HttpPost("{todoId}/attachments")]
        public async Task<ActionResult<AttachmentDto>> UploadAttachment(int todoId, IFormFile file)
        {
            var userId = GetCurrentUserId();

            // Perform file validation constraints check
            var validation = FileValidator.ValidateFile(file);
            if (!validation.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "File validation failed",
                    Details = validation.ErrorMessage,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var attachment = await _fileService.UploadFileAsync(file, todoId, userId);

                return CreatedAtAction(
                    nameof(GetAttachment),
                    new { id = attachment.Id },
                    MapToAttachmentDto(attachment));
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message); // 404 if user doesn’t own TODO
            }
        }
        // 2. GET ATTACHMENTS ENDPOINT
        [HttpGet("{todoId}/attachments")]
        public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetTodoAttachments(int todoId)
        {
            var userId = GetCurrentUserId();

            try
            {
                var attachments = await _fileService.GetTodoAttachmentsAsync(todoId, userId);
                return Ok(attachments.Select(MapToAttachmentDto));
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound(); // 404 if user doesn’t own TODO
            }
        }
        // Action helper routing lookup target
        [HttpGet("attachments/{id}", Name = "GetAttachment")]
        public async Task<ActionResult<AttachmentDto>> GetAttachment(int id)
        {
            var attachment = await _fileService.GetAttachmentAsync(id, GetCurrentUserId());
            if (attachment == null) return NotFound();

            return Ok(MapToAttachmentDto(attachment));
        }
        // 3. DOWNLOAD ATTACHMENT ENDPOINT
        [HttpGet("attachments/{id}/download")]
        public async Task<ActionResult> DownloadAttachment(int id)
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _fileService.DownloadFileAsync(id, GetCurrentUserId());
                return File(fileBytes, contentType, fileName);
            }
            catch (FileNotFoundException ex) when (ex.Message.Contains("denied") || ex.Message.Contains("Access"))
            {
                return Forbid(); // Returns 403 Forbidden
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message); // Returns 404 Not Found
            }
        }
        // 4. DELETE ATTACHMENT ENDPOINT
        [HttpDelete("attachments/{id}")]
        public async Task<ActionResult> DeleteAttachment(int id)
        {
            try
            {
                var deleted = await _fileService.DeleteAttachmentAsync(id, GetCurrentUserId());
                if (!deleted) return NotFound();

                return NoContent(); // Returns 204 No Content
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); // Returns 403 Forbidden
            }
        }


        // --- INTERNAL LOGIC HELPERS ---

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid authentication token context profile.");
            }
            return userId;
        }

        private AttachmentDto MapToAttachmentDto(Attachments a)
        {
            string uploaderName = a.User != null
                ? $"{a.User.firstName} {a.User.lastName}".Trim()
                : "Unknown User";

            return new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSize = a.FileSize,
                ContentType = a.ContentType,
                UploadedAt = a.UploadedAt,
                UploadedBy = new UserInfoDto
                {
                    Id = a.UploadedBy,
                    FullName = uploaderName
                },
                DownloadUrl = $"/api/todos/attachments/{a.Id}/download"
            };
        }


    }
}
