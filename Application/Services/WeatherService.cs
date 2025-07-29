using Microsoft.Extensions.Logging;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain.Entities;
using WeatherMcpServer.Exceptions;
using WeatherMcpServer.Infrastructure.Http.Clients;

namespace WeatherMcpServer.Application.Services;

public class WeatherService(WeatherClient client, ILogger<WeatherService> logger) : IWeatherService
{
    private readonly ILogger<WeatherService> _logger = logger;

    public async Task<WeatherInfo?> GetCurrentWeatherAsync(string city, string? countryCode = null)
    {
        var response = await client.GetCurrentWeatherAsync(city, countryCode);
        if (response == null) return null;

        return new WeatherInfo
        {
            DateTime = DateTimeOffset.FromUnixTimeSeconds(response.Dt).UtcDateTime,
            TemperatureCelsius = response.Main.Temp,
            Conditions = response.Weather.Select(w => w.Main).ToArray()
        };
    }

    public async Task<List<WeatherInfo>> GetForecastAsync(string city, string? countryCode = null, int days = 3)
    {
        var response = await client.GetForecastAsync(city, countryCode, days);
        if (response?.List == null) return [];

        return response.List.Select(item => new WeatherInfo
        {
            DateTime = DateTime.Parse(item.DtTxt ?? string.Empty),
            TemperatureCelsius = item.Main.Temp,
            Conditions = item.Weather.Select(w => w.Main).ToArray()
        }).ToList();
    }

    public async Task<string?> GetWeatherAlertsAsync(string city, string? countryCode = null)
    {
        var current = await client.GetCurrentWeatherAsync(city, countryCode);
        if (current?.Coord == null) return null;

        var alerts = await client.GetWeatherAlertsAsync(current.Coord.Lat, current.Coord.Lon);
        if (alerts?.Alerts == null || alerts.Alerts.Length == 0)
        {
            return "✅ No weather alerts.";
        }

        return string.Join("\n\n", alerts.Alerts.Select(alert =>
            $"🚨 {alert.Event}\nBy: {alert.SenderName}\n{alert.Description}"
        ));
    }
}