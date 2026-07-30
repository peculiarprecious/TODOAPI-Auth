namespace TODOAPI_Auth.Services
{
    public interface IFileStorageService
    {
        // Saves an uploaded file payload securely to the local disk folder.
        
        Task<(string StoredFileName, string FilePath)> SaveFileAsync(IFormFile file);

       
        // Deletes a physical asset file from the disk storage system.
    
        void DeleteFile(string filePath);
    }
}
