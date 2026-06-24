using System.Net;
using System.Text.Json;
using TODOAPI_Auth.DTOs.WeatherDTO;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

       private readonly IConfiguration _configuration;
        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherDto?> GetWeatherAsync(string city)
        {
            try
            {
                var apiKey = _configuration["ExternalApis:OpenWeather:ApiKey"];
                var apiUrl = _configuration["ExternalApis:OpenWeather:ApiUrl"];

                var url = $"{apiUrl}/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);

                // If the user inputs a city that doesn't exist, return null safely instead of throwing a 500 [INDEX]
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var weather = JsonSerializer.Deserialize<OpenWeatherResponse>(json, options);

                if (weather == null || weather.Weather.Count == 0) return null;

                return new WeatherDto
                {
                    City = weather.Name,
                    Temperature = weather.Main.Temp,
                    Description = weather.Weather[0].Description,
                    Humidity = weather.Main.Humidity,
                    WindSpeed = weather.Wind.Speed
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
