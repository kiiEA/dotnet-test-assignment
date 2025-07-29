using System.ComponentModel;
using ModelContextProtocol.Server;
using WeatherMcpServer.Application.Interfaces;

namespace WeatherMcpServer.Presentation.Tools;

public class WeatherTools(IWeatherService weatherService,IWeatherFormatter formatter)
{
    [McpServerTool]
    [Description("Get current weather conditions")]
    public async Task<string> GetCurrentWeather(
        [Description("City name")] string city,
        [Description("Optional: country code")] string? countryCode = null)
    {
        var weather = await weatherService.GetCurrentWeatherAsync(city, countryCode);
        return weather == null ? "❌ Not found" : formatter.FormatCurrent(weather);
    }

    [McpServerTool]
    [Description("Get weather forecast")]
    public async Task<string> GetForecast(
        [Description("City name")] string city,
        [Description("Optional: country code")] string? countryCode = null)
    {
        var forecast = await weatherService.GetForecastAsync(city, countryCode);
        if (forecast.Count == 0) return "No forecast data found.";

        return string.Join("\n", forecast.Take(5).Select(f =>
            $"{f.DateTime:dd.MM HH:mm} — {f.TemperatureCelsius:F1}°C, {string.Join(", ", f.Conditions)}"));
    }

    [McpServerTool]
    [Description("Get weather alerts")]
    public async Task<string> GetAlerts(
        [Description("City name")] string city,
        [Description("Optional: country code")] string? countryCode = null)
    {
        return await weatherService.GetWeatherAlertsAsync(city, countryCode)
               ?? "No alerts.";
    }
}