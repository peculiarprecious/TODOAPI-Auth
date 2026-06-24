using System.Text.Json.Serialization;

namespace TODOAPI_Auth.Models
{
    public class OpenWeatherResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public MainWeather Main { get; set; } = null!;

        [JsonPropertyName("weather")]
        public List<WeatherInfo> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public Wind Wind { get; set; } = null!;
    }

    public class MainWeather
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherInfo
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

    public class Wind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}