using System.ComponentModel.DataAnnotations;

namespace TODOAPI_Auth.DTOs.UserDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
