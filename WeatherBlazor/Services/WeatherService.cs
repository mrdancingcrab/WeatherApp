using System.Net.Http.Json;
using WeatherBlazor.Models;

namespace WeatherBlazor.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(WeatherData? data, string? errorMessage)> GetWeatherAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"weather/{city}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    return (null, $"Error: {errorMsg}");
                }

                var weatherResponse = await response.Content.ReadFromJsonAsync<WeatherResponse>();

                if (weatherResponse == null)
                    return (null, "Invalid weather data received.");

                var weather = new WeatherData
                {
                    City = weatherResponse.City,
                    Description = weatherResponse.Description,
                    Temperature = weatherResponse.Temperature,
                    FeelsLike = weatherResponse.FeelsLike,
                    WindSpeed = weatherResponse.WindSpeed,
                    Humidity = weatherResponse.Humidity,
                    IconCode = weatherResponse.IconCode // ✅ Matches new API response
                };

                return (weather, null);
            }
            catch (Exception ex)
            {
                return (null, $"Error: {ex.Message}");
            }
        }

        public async Task<WeatherForecast?> GetForecastAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"forecast/{city}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<WeatherForecast>();
            }
            catch
            {
                return null;
            }
        }
    }
}
