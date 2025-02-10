using WeatherApi.Models;
using System.Text.Json;

namespace WeatherApi.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error fetching weather data: {response.StatusCode} - {errorMessage}");

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception("City not found.");
                    }
                    else
                    {
                        throw new Exception("Weather API error. Please try again later.");
                    }
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string iconCode = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "01d";

                var weatherData = new WeatherResponse
                {
                    City = root.GetProperty("name").GetString() ?? "Unknown",
                    Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "Unknown",
                    Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                    FeelsLike = root.GetProperty("main").GetProperty("feels_like").GetDouble(),
                    WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    IconCode = iconCode 
                };

                Console.WriteLine($"Weather Data Fetched: {JsonSerializer.Serialize(weatherData)}"); // Debugging
                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing weather data: {ex.Message}");
                return null;
            }
        }

        public async Task<WeatherForecastResponse?> GetForecastAsync(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // ✅ Extract City Name
                string cityName = root.GetProperty("city").GetProperty("name").GetString() ?? "Unknown";

                // ✅ Extract Forecast Data (Next 5 Days, One Entry Per Day)
                var forecasts = root.GetProperty("list")
                    .EnumerateArray()
                    .Where((entry, index) => index % 8 == 0) // Selects one forecast per day (~every 24h)
                    .Select(entry => new ForecastEntry
                    {
                        Date = DateTime.Parse(entry.GetProperty("dt_txt").GetString() ?? ""),
                        Description = entry.GetProperty("weather")[0].GetProperty("description").GetString() ?? "Unknown",
                        Temperature = entry.GetProperty("main").GetProperty("temp").GetDouble(),
                        IconCode = entry.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "01d"
                    })
                    .ToList();

                return new WeatherForecastResponse
                {
                    City = cityName,
                    Forecasts = forecasts
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching forecast: {ex.Message}");
                return null;
            }
        }

    }
}
