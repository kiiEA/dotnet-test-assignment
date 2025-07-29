using WeatherMcpServer.Domain.Entities;

namespace WeatherMcpServer.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherInfo?> GetCurrentWeatherAsync(string city, string? countryCode = null);
    Task<List<WeatherInfo>> GetForecastAsync(string city, string? countryCode = null, int days = 3);
    Task<string?> GetWeatherAlertsAsync(string city, string? countryCode = null);
}