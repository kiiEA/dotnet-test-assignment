using WeatherMcpServer.Domain.Entities;

namespace WeatherMcpServer.Application.Interfaces;

public interface IWeatherFormatter
{
    string FormatCurrent(WeatherInfo current);
    string FormatForecast(IEnumerable<WeatherInfo> forecast);
    string FormatAlerts(string alertsText);
}