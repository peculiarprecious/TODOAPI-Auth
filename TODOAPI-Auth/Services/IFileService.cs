using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public interface IFileService
    {
        Task<Attachments> UploadFileAsync(IFormFile file, int todoItemId, int uploadedBy);
        Task<Attachments?> GetAttachmentAsync(int attachmentId, int userId);
        Task<List<Attachments>> GetTodoAttachmentsAsync(int todoItemId, int userId);
        Task<bool> DeleteAttachmentAsync(int attachmentId, int userId);
        Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(int attachmentId, int userId);
    }
}
