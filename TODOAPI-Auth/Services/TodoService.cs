using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODOAPI_Auth.DatabaseContext;
using TODOAPI_Auth.DTOs;
using TODOAPI_Auth.DTOs.TODODTO;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;
        private readonly SlackService _slackService;

        public TodoService(ApplicationDbContext context, SlackService slackService)
        {
            _context = context;
            _slackService = slackService;
        }

        private static TodoResponseDto MapToResponse(TodoItem todo)
        {
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                DueDate = todo.DueDate,
                Priority = todo.Priority
            };
        }

        // 1. GET all — ToListAsync() with User security
        public async Task<List<TodoResponseDto>> GetAllAsync(int userId, string? status = null)
        {
            var query = _context.TodoItems.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "completed")
                {
                    query = query.Where(t => t.IsCompleted == true);
                }
                else if (status.ToLower() == "pending")
                {
                    query = query.Where(t => t.IsCompleted == false);
                }
            }

            var todos = await query.ToListAsync();
            return todos.Select(t => MapToResponse(t)).ToList();
        }

        // 2. Text Search locked to owner
        public async Task<List<TodoResponseDto>> SearchAsync(int userId, string q)
        {
            var todos = await _context.TodoItems
                .Where(t => t.UserId == userId &&
                           (t.Title.Contains(q) || (t.Description != null && t.Description.Contains(q))))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return todos.Select(t => MapToResponse(t)).ToList();
        }

        // 3. Paginated, Sorted, Filtered Query Envelope
        public async Task<object> GetTodosAsync(
            int userId,
            string? status,
            string? priority,
            string? sortBy,
            string? sortOrder,
            int page = 1,
            int pageSize = 10)
        {
            // Validate pagination safety boundaries
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // Prepare base query blueprint locked down to current user
            var query = _context.TodoItems.Where(t => t.UserId == userId);

            // Apply status filtering
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "completed")
                {
                    query = query.Where(t => t.IsCompleted == true);
                }
                else if (status.ToLower() == "pending")
                {
                    query = query.Where(t => t.IsCompleted == false);
                }
            }

            // Apply priority filtering
            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
            }

            // Get total count matching current user's filtered criteria
            var totalCount = await query.CountAsync();

            // Apply multi-criteria sorting configurations
            bool descending = sortOrder?.ToLower() == "desc";
            query = sortBy?.ToLower() switch
            {
                "title" => descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "duedate" => descending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "priority" => descending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "createdat" => descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                _ => query.OrderBy(t => t.Id)
            };

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;

            var todos = await query.Skip(skip).Take(pageSize).ToListAsync();
            var mappedData = todos.Select(t => MapToResponse(t)).ToList();

            return new
            {
                Data = mappedData,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount,
                    HasPrevious = page > 1,
                    HasNext = page < totalPages
                }
            };
        }

        // 4. Dashboard Statistics Aggregates
        public async Task<object> GetStatisticsAsync(int userId)
        {
            var userQuery = _context.TodoItems.Where(t => t.UserId == userId);

            var totalCount = await userQuery.CountAsync();
            var completedCount = await userQuery.CountAsync(t => t.IsCompleted);
            var pendingCount = totalCount - completedCount;

            var highPriorityCount = await userQuery.CountAsync(t => t.Priority == "High");
            var mediumPriorityCount = await userQuery.CountAsync(t => t.Priority == "Medium");
            var lowPriorityCount = await userQuery.CountAsync(t => t.Priority == "Low");

            return new
            {
                TotalCount = totalCount,
                CompletedCount = completedCount,
                PendingCount = pendingCount,
                Priorities = new
                {
                    High = highPriorityCount,
                    Medium = mediumPriorityCount,
                    Low = lowPriorityCount
                }
            };
        }

        // 5. Get Single Item By ID
        public async Task<TodoResponseDto?> GetByIdAsync(int userId, int id)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return todo == null ? null : MapToResponse(todo);
        }

        // 6. Create Todo
        public async Task<TodoResponseDto> CreateAsync(int userId, CreateTodoDTO dto)
        {
            // ✅ Check user exists before creating todo
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception($"User with id {userId} not found");

            var todo = new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                UserId = userId,
                CreatedAt = DateTime.Now 
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            _ = _slackService.NotifyTodoCreatedAsync(todo, user);

            return MapToResponse(todo);
        }

        // 7. Update Todo
        public async Task<TodoResponseDto?> UpdateAsync(int userId, int id, UpdateTodoDTO dto)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return null;

            // Track state change before updating
            bool wasCompleted = todo.IsCompleted;

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.DueDate = dto.DueDate;
            todo.Priority = dto.Priority;
            todo.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();

            if (!wasCompleted && todo.IsCompleted)
            {
                _ = _slackService.NotifyTodoCompletedAsync(todo); 
            }
            return MapToResponse(todo);
        }

        // 8. Delete Todo
        public async Task<bool> DeleteAsync(int userId, int id)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            var todoSnapshot = new TodoItem
            {
                Title = todo.Title,
                Priority = todo.Priority
            };

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();

            _ = _slackService.NotifyTodoDeletedAsync(todoSnapshot);
            return true;
        }
    }
}
