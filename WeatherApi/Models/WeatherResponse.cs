namespace WeatherApi.Models
{
    public class WeatherResponse
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double WindSpeed { get; set; }
        public int Humidity { get; set; }
        public string IconCode { get; set; }
    }
}
