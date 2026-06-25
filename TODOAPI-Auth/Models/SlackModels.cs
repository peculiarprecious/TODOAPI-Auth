using System.Text.Json.Serialization;

namespace TODOAPI_Auth.Models
{
    public class SlackModels
    {
        public class SlackMessage
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;

            [JsonPropertyName("attachments")]
            public SlackAttachment[] Attachments { get; set; } = System.Array.Empty<SlackAttachment>();
        }

        public class SlackAttachment
        {
            [JsonPropertyName("fallback")]
            public string Fallback { get; set; } = string.Empty;

            [JsonPropertyName("color")]
            public string Color { get; set; } = string.Empty;

            [JsonPropertyName("fields")]
            public SlackField[] Fields { get; set; } = System.Array.Empty<SlackField>();
        }

        public class SlackField
        {
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("short")]
            public bool IsShort { get; set; }
        }
    }
}
