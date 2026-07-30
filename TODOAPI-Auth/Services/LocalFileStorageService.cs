namespace TODOAPI_Auth.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment) { 
        _environment = environment;
        }

        public async Task<(string StoredFileName, string FilePath)> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Provided file asset is empty.");

            // Dynamically target the wwwroot/uploads folder
            string uploadsFolder = Path.Combine(_environment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads");

            // Automatically creates the uploads folder on your hard drive if missing
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // mask the original filename with a GUID
            string fileExtension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid().ToString("N")}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, storedFileName);

            // Stream and save the binary data directly to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return (storedFileName, filePath);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
