using Microsoft.EntityFrameworkCore;
using TODOAPI_Auth.DatabaseContext;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _storageService;

        public FileService(ApplicationDbContext context, IFileStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<Attachments> UploadFileAsync(IFormFile file, int todoItemId, int uploadedBy)
        {
            // Verify the todo item exists and belongs to the user before uploading
            var todoExists = await _context.TodoItems
                .AnyAsync(t => t.Id == todoItemId && t.UserId == uploadedBy);

            if (!todoExists)
            {
                throw new UnauthorizedAccessException("You do not have permission to attach files to this task.");
            }

            // 1. Physically save the file bytes to disk
            var (storedFileName, filePath) = await _storageService.SaveFileAsync(file);

            // 2. Map data to Attachments model
            var attachment = new Attachments
            {
                FileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow,
                TodoItemId = todoItemId,
                UploadedBy = uploadedBy
            };

            // 3. Track record in SQL Server database
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<Attachments?> GetAttachmentAsync(int attachmentId, int userId)
        {
            // Only return the file if the parent task belongs to the user
            return await _context.Attachments
                .Include(a => a.TodoItem)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.TodoItem.UserId == userId);
        }

        public async Task<List<Attachments>> GetTodoAttachmentsAsync(int todoItemId, int userId)
        {
            // Verify todo item access before listing attachments
            var todoExists = await _context.TodoItems.AnyAsync(t => t.Id == todoItemId && t.UserId == userId);
            if (!todoExists)
            {
                throw new UnauthorizedAccessException("Access denied to this task.");
            }

            return await _context.Attachments
                .Include(a => a.User) // Include user so FirstName/LastName mapping functions correctly
                .Where(a => a.TodoItemId == todoItemId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAttachmentAsync(int attachmentId, int userId)
        {
            var attachment = await _context.Attachments
                .Include(a => a.TodoItem)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if (attachment == null) return false;

            // Security check
            if (attachment.TodoItem.UserId != userId)
            {
                throw new UnauthorizedAccessException("You don't own this attachment.");
            }

            // 1. Drop file from disk storage
            _storageService.DeleteFile(attachment.FilePath);

            // 2. Delete row metadata entry from SQL Server
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(int attachmentId, int userId)
        {
            var attachment = await GetAttachmentAsync(attachmentId, userId);
            if (attachment == null)
            {
                throw new FileNotFoundException("Access denied or attachment not found.");
            }

            if (!File.Exists(attachment.FilePath))
            {
                throw new FileNotFoundException("The physical file is missing from server storage.");
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(attachment.FilePath);
            return (fileBytes, attachment.ContentType, attachment.FileName);
        }
    }
}
