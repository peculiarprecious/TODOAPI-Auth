using System.Text.Json.Serialization;

namespace TODOAPI_Auth.Models
{
    public class GoogleCalendarEventPayload
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public GoogleCalendarDateTime Start { get; set; } = null!;

        [JsonPropertyName("end")]
        public GoogleCalendarDateTime End { get; set; } = null!;
    }

    public class GoogleCalendarDateTime
    {
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; } = string.Empty; // Must be in ISO 8601 format (e.g., yyyy-MM-ddTHH:mm:ssZ)

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; } = "UTC";
    }
}
