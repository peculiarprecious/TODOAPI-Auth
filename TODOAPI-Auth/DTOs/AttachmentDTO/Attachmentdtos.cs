namespace TODOAPI_Auth.DTOs.AttachmentDTO
{
   
        public class AttachmentDto
        {
            public int Id { get; set; }
            public string FileName { get; set; } = string.Empty;
            public long FileSize { get; set; }
            public string ContentType { get; set; } = string.Empty;
            public DateTime UploadedAt { get; set; }
            public UserInfoDto UploadedBy { get; set; } = new UserInfoDto();
            public string DownloadUrl { get; set; } = string.Empty;
        }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

}
