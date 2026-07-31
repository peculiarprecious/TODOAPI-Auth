using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendTodoCreatedEmailAsync(User user, TodoItem todo);
        Task SendTodoCompletedEmailAsync(User user, TodoItem todo);
    }
}
