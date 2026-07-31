
using System.Net;
using System.Net.Mail;
using TODOAPI_Auth.Models;
namespace TODOAPI_Auth.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _password;
        private readonly ILogger<EmailService> _logger;


        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _fromEmail = configuration["Email:FromEmail"];
            _password = configuration["Email:Password"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_fromEmail, _password),
                    EnableSsl = true,
                    Timeout = 10000 // 10 second timeout requirement
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation(
                    "Email sent successfully to {Email} with subject: {Subject}",
                    toEmail,
                    subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
        public async Task SendTodoCreatedEmailAsync(User user, TodoItem todo)
        {
            string fullName = $"{user.firstName} {user.lastName}".Trim();
            var subject = "New TODO Created";
            var body = $@"
            <h2>New TODO Created</h2>
            <p>Hi {fullName},</p>
            <p>A new TODO has been created:</p>
            <ul>
                <li><strong>Title:</strong> {todo.Title}</li>
                <li><strong>Priority:</strong> {todo.Priority}</li>
                <li><strong>Due Date:</strong> {todo.DueDate?.ToString("yyyy-MM-dd") ?? "No due date"}</li>
            </ul>
            <p>Log in to your account to view more details.</p>";

            await SendEmailAsync(user.email, subject, body);
        }
        public async Task SendTodoCompletedEmailAsync(User user, TodoItem todo)
        {
            string fullName = $"{user.firstName} {user.lastName}".Trim();
            var subject = "TODO Completed";
            var body = $@"
            <h2>TODO Completed! 🎉</h2>
            <p>Hi {fullName},</p>
            <p>Congratulations on completing: <strong>{todo.Title}</strong></p>";

            await SendEmailAsync(user.email, subject, body);
        }

    }
}
