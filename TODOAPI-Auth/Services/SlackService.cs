using System.Text;
using System.Text.Json;
using TODOAPI_Auth.Models;
using static TODOAPI_Auth.Models.SlackModels;

namespace TODOAPI_Auth.Services
{
    public class SlackService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SlackService> _logger;

        public SlackService(HttpClient httpClient, IConfiguration configuration, ILogger<SlackService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task NotifyTodoCreatedAsync(TodoItem todo, User user)
        {
            var webhookUrl = _configuration["ExternalApis:Slack:WebhookUrl"];

            if (string.IsNullOrEmpty(webhookUrl) || webhookUrl.Contains("YOUR/REAL"))
            {
                _logger.LogWarning("Slack webhook URL not configured");
                return;
            }

            var message = new SlackMessage
            {
                Text = "New TODO Created",
                Attachments = new[]
                {
            new SlackAttachment
            {
                Color    = "#3498db",
                Fallback = $"New task created: {todo.Title}",
                Fields   = new[]
                {
                    new SlackField
                    {
                        Title   = "Title",
                        Value   = todo.Title,
                        IsShort = false
                    },
                    new SlackField
                    {
                        Title   = "Priority",
                        Value   = todo.Priority,
                        IsShort = true
                    },
                    new SlackField
                    {
                        Title   = "Created By",
                        Value   = $"{user.firstName} {user.lastName}".Trim(),
                        IsShort = true
                    },
                    new SlackField
                    {
                        Title   = "Due Date",
                        Value   = todo.DueDate?.ToString("yyyy-MM-dd")
                                  ?? "No due date",
                        IsShort = true
                    }
                }
            }
        }
            };

            await SendToSlackAsync(webhookUrl, message);
        }
        public async Task NotifyTodoCompletedAsync(TodoItem todo)
        {
            var webhookUrl = _configuration["ExternalApis:Slack:WebhookUrl"];
            if (string.IsNullOrEmpty(webhookUrl) || webhookUrl.Contains("YOUR/REAL")) return;

            var message = new SlackMessage
            {
                Text = "TODO Completed",
                Attachments = new[]
                {
            new SlackAttachment
            {
                Color = "#2ecc71",
                Fallback = $"Task completed: {todo.Title}", 
                Fields = new[]
                {
                    new SlackField { Title = "Title", Value = todo.Title, IsShort = false }
                }
            }
        }
            };

            await SendToSlackAsync(webhookUrl, message);
        }

        public async Task NotifyTodoDeletedAsync(TodoItem todo)
        {
            var webhookUrl = _configuration["ExternalApis:Slack:WebhookUrl"];
            if (string.IsNullOrEmpty(webhookUrl) || webhookUrl.Contains("YOUR/REAL")) return;

            var message = new SlackMessage
            {
                Text = "TODO Deleted",
                Attachments = new[]
                {
            new SlackAttachment
            {
                Color = "#e74c3c",
                Fields = new[]
                {
                    new SlackField { Title = "Title", Value = todo.Title, IsShort = false },
                    new SlackField { Title = "Priority", Value = todo.Priority, IsShort = true }
                }
            }
        }
            };

            await SendToSlackAsync(webhookUrl, message);
        }

        private async Task SendToSlackAsync(string webhookUrl, SlackMessage message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(webhookUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send Slack notification: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Slack notification");
            }
        }





    }
}
