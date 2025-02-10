namespace WeatherBlazor.Models
{
    public class WeatherForecast
    {
        public string City { get; set; } = "";
        public List<ForecastEntry> Forecasts { get; set; } = new();
    }

    public class ForecastEntry
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public double Temperature { get; set; }
        public string IconCode { get; set; } = "";

    }

}
