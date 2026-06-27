using System.Text.Json.Serialization;

namespace TODOAPI_Auth.Models
{
    public class GitHubIssue
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; set; } 

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonPropertyName("labels")]
        public List<GitHubLabel> Labels { get; set; } = new();
    }

    public class GitHubLabel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
