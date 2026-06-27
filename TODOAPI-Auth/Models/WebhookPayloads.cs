using System;
using System.Text.Json.Serialization;

namespace TODOAPI_Auth.Models
{
    // Payload shape for standard todo events
    public class WebhookPayload
    {
        public int UserId { get; set; }
        public int TodoId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    // Payload shape for incoming GitHub webhooks
    public class GitHubWebhookPayload
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("issue")]
        public GitHubWebhookIssue Issue { get; set; } = null!;
    }

    public class GitHubWebhookIssue
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; set; } // Nullable in case the description is empty
    }
}
