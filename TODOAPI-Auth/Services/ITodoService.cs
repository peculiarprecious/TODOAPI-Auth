using TODOAPI_Auth.DTOs.TODODTO;

namespace TODOAPI_Auth.Services
{
    public interface ITodoService
    {
        Task<List<TodoResponseDto>> GetAllAsync(int userId, string? status = null);

        Task<object> GetTodosAsync(
            int userId,
            string? status,
            string? priority,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10);

        Task<object> GetStatisticsAsync(int userId);

        Task<List<TodoResponseDto>> SearchAsync(int userId, string q);

        Task<TodoResponseDto?> GetByIdAsync(int userId, int id);

        Task<TodoResponseDto> CreateAsync(int userId, CreateTodoDTO dto);

        Task<TodoResponseDto?> UpdateAsync(int userId, int id, UpdateTodoDTO dto);

        Task<bool> DeleteAsync(int userId, int id);
    
}
}
