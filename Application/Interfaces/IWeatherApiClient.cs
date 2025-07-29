using WeatherMcpServer.Domain.Entities;

namespace WeatherMcpServer.Application.Interfaces;

public interface IWeatherApiClient
{
    Task<WeatherInfo?> GetCurrentWeatherAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WeatherInfo>> GetForecastAsync(string city, string? countryCode = null, int days = 3, CancellationToken cancellationToken = default);
    Task<string> GetAlertsAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default);
}