using Microsoft.AspNetCore.Identity.Data;
using TODOAPI_Auth.DTOs.UserDTO;

namespace TODOAPI_Auth.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(UserRegisterDTO registerDto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
    }
}
