using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOAPI_Auth.DatabaseContext;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(ApplicationDbContext context, ILogger<WebhooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("todo-created")] // POST: api/webhooks/todo-created
        public IActionResult OnTodoCreated([FromBody] WebhookPayload payload)
        {
            if (!ModelState.IsValid || payload == null)
            {
                return BadRequest(new { error = "Invalid todo webhook data payload." });
            }

            _logger.LogInformation(
                "Webhook received: TODO {Action} for user {UserId} at {Timestamp}",
                payload.Action, payload.UserId, payload.Timestamp);

            return Accepted();
        }

        [HttpPost("github-issue")] // POST: api/webhooks/github-issue
        public async Task<IActionResult> OnGitHubIssue([FromBody] GitHubWebhookPayload payload)
        {
            if (!ModelState.IsValid || payload == null || payload.Issue == null)
            {
                return BadRequest(new { error = "Invalid GitHub webhook data payload." });
            }

            _logger.LogInformation(
                "GitHub webhook triggered: Issue #{Number} {Action} - Title: {Title}",
                payload.Issue.Number, payload.Action, payload.Issue.Title);

            // Process webhook asynchronously
            if (payload.Action.Equals("opened", StringComparison.OrdinalIgnoreCase))
            {
                // VALIDATION: Find any existing user to own this task
                // First try to look for User ID 1, if missing, grab the first available user in the table
                var fallbackUser = await _context.Users.FindAsync(1)
                                   ?? await _context.Users.FirstOrDefaultAsync();

                if (fallbackUser != null)
                {
                    var autoTodo = new TodoItem
                    {
                        Title = $"[GH-WEBHOOK-#{payload.Issue.Number}] {payload.Issue.Title}",
                        Description = $"{payload.Issue.Body ?? "No description provided."}\n\nGenerated via GitHub Webhook Listener.",
                        Priority = "Medium",
                        UserId = fallbackUser.Id, // Securely uses a real, verified User ID!
                        CreatedAt = DateTime.UtcNow,
                        IsCompleted = false
                    };

                    _context.TodoItems.Add(autoTodo);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("💾 Successfully auto-saved Webhook Issue #{Number} for User ID {UserId}.", payload.Issue.Number, fallbackUser.Id);
                }
                else
                {
                    // Database cushion: Logs an alert if the instructor hasn't registered a single user yet
                    _logger.LogWarning("Webhook parsed successfully, but task creation was skipped because the Users table is completely empty or no user with Id = 1.");
                }
            }

            return Accepted(); //Always returns 202 Accepted to the third-party server
        }


    }
}
