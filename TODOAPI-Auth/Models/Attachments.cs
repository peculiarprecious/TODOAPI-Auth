using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TODOAPI_Auth.Models
{
    public class Attachments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; } = string.Empty; // Unique filename on disk

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty ;

        [Required]
        public long FileSize { get; set; } // In bytes

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int TodoItemId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UploadedBy { get; set; }

        // Navigation Properties
        public TodoItem TodoItem { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
