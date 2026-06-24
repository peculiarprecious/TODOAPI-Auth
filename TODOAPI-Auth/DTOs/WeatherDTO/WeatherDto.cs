namespace TODOAPI_Auth.DTOs.WeatherDTO
{
    public class WeatherDto
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
