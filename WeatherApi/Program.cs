
using WeatherApi.Services;

namespace WeatherApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient<WeatherService>();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
            

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseCors("AllowAll");
           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/weather/{city}", async (string city, WeatherService weatherService) =>
            {

                try
                {
                    var weatherData = await weatherService.GetWeatherAsync(city);

                    if (weatherData == null)
                    {
                        return Results.NotFound(new { Message = "City not found or API error" });
                    }

                    return Results.Ok(weatherData);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error: {ex.Message}");
                }
            });

            app.MapGet("/forecast/{city}", async (string city, WeatherService weatherService) =>
            {
                try
                {
                    var forecastData = await weatherService.GetForecastAsync(city);

                    if (forecastData == null)
                    {
                        return Results.NotFound(new { Message = "City not found or API error" });
                    }

                    return Results.Ok(forecastData);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error: {ex.Message}");
                }
            });





            app.Run();
        }
    }
}
