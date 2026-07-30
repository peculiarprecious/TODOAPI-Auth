namespace TODOAPI_Auth.Helpers
{
    public class FileValidator
    {
        // 1. Quota Checking: Set max allowed size (e.g., 5 MB)
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

        // 2. Extension Whitelist
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".txt" };

        // 3. Magic Numbers (File Signatures) for Content-Type validation
        private static readonly byte[][] AllowedSignatures = new byte[][]
        {
            new byte[] { 0xFF, 0xD8, 0xFF },                // JPEG/JPG
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, // PNG
            new byte[] { 0x25, 0x50, 0x44, 0x46 },          // PDF
            new byte[] { 0x50, 0x4B, 0x03, 0x04 },          // DOCX / ZIP
        };

        public static (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File is empty.");
            }

            // Check file size constraint
            if (file.Length > MaxFileSizeInBytes)
            {
                return (false, $"File size exceeds the limit of {MaxFileSizeInBytes / (1024 * 1024)} MB.");
            }

            // Check file extension
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return (false, $"The file extension '{extension}' is not permitted.");
            }

            // Performance optimization: plain text files skip byte matching signature checks
            if (extension == ".txt")
            {
                return (true, string.Empty);
            }

            // Check file signatures (Magic Numbers)
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                // Read enough bytes to match the longest signature (8 bytes for PNG)
                byte[] fileBytes = reader.ReadBytes(8);

                bool matchesSignature = AllowedSignatures.Any(signature =>
                    fileBytes.Take(signature.Length).SequenceEqual(signature));

                if (!matchesSignature)
                {
                    return (false, "File content signature verification failed (Spoofed file extension detected).");
                }
            }

            return (true, string.Empty);
        }
    }
}
