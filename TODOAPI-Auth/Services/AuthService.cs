using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;
using TODOAPI_Auth.DatabaseContext;
using TODOAPI_Auth.DTOs.UserDTO;
using TODOAPI_Auth.Helpers;
using TODOAPI_Auth.Migrations;
using TODOAPI_Auth.Models;
using BC = BCrypt.Net.BCrypt; // Alias for BCrypt library

namespace TODOAPI_Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJWTHelper _jWTHelper;

        public AuthService(ApplicationDbContext context, IJWTHelper jWTHelper)
        {
            _context = context;
            _jWTHelper = jWTHelper;
        }

        public async Task<AuthResponseDTO?> RegisterAsync(UserRegisterDTO registerDto)
        {
            // 1. Check if email already exists in the database
            var emailExist = await _context.Users.AnyAsync(u => u.email == registerDto.email);
            if (emailExist) {
                throw new Exception("User with this email already exists.");
            }

            // 2. Hash the plain text password securely using BCrypt
            string passwordHash = BC.HashPassword(registerDto.password);

            // 3. Map the DTO data into User database model
            var user = new User
            {
                firstName = registerDto.firstName,
                lastName = registerDto.lastName,
                email = registerDto.email,
                passwordHash = passwordHash

            };
            // 4. Save the user data to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 5. Generate the JWT token badge
            var token = _jWTHelper.GenerateToken(user);
            // 6. Return the finalized payload structure back to the controller
            return new AuthResponseDTO
            {
                Token = token,
                UserId = user.Id,
                Email = user.email,
                Username = $"{user.firstName} {user.lastName}".Trim(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)


            };
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            // 1. Locate the user profile by their email address
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == loginDto.Email);
            if (user == null) return null;
            // 2. Verify if the incoming password matches the stored scrambled hash
            var isPasswordValid = BC.Verify(loginDto.Password, user.passwordHash);
            if (!isPasswordValid)
            { throw new Exception("Incorrect password."); 
            } 

            // 3. Generate the token badge for the validated session
            var token = _jWTHelper.GenerateToken(user);

            return new AuthResponseDTO
            {
                Token = token,
                Username = $"{user.firstName} {user.lastName}".Trim(),
                Email = user.email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}
