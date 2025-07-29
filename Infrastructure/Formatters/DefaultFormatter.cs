using System.Text;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain.Entities;

namespace WeatherMcpServer.Infrastructure.Formatters;
public class PlainTextWeatherFormatter : IWeatherFormatter
{
    public string FormatCurrent(WeatherInfo current)
    {
        return $"🌡️ {current.TemperatureCelsius:F1}°C\n" +
               $"🌀 {string.Join(", ", current.Conditions)}\n" +
               $"🕒 {current.DateTime:yyyy-MM-dd HH:mm}";
    }

    public string FormatForecast(IEnumerable<WeatherInfo> forecast)
    {
        var sb = new StringBuilder("📅 Forecast:\n");
        foreach (var item in forecast)
        {
            sb.AppendLine($"{item.DateTime:dd.MM HH:mm} — {item.TemperatureCelsius:F1}°C, {string.Join(", ", item.Conditions)}");
        }
        return sb.ToString();
    }

    public string FormatAlerts(string alertsText)
    {
        return $"🚨 Alerts:\n{alertsText}";
    }
}
