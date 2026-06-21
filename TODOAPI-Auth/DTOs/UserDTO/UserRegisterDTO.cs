using System.ComponentModel.DataAnnotations;
namespace TODOAPI_Auth.DTOs.UserDTO
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage ="FirstName is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "FirstName must be at least 3 characters")]
        public string firstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "LastName is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "LastName must be at least 3 characters")]
        public string lastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters")]
        public string password { get; set; } = string.Empty;

    }
}
