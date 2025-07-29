using WeatherMcpServer.Domain.Entities;
using WeatherMcpServer.Infrastructure.Http.Models.Responses;

namespace WeatherMcpServer.Infrastructure.Http.Mappers;

public static class WeatherMappers
{
    public static WeatherInfo ToDomain(this OpenWeatherCurrentResponse response)
    {
        return new WeatherInfo
        {
            DateTime = DateTimeOffset.FromUnixTimeSeconds(response.Dt).UtcDateTime,
            TemperatureCelsius = response.Main.Temp,
            Conditions = response.Weather.Select(w => w.Main).ToArray()
        };
    }

    public static WeatherInfo ToDomain(this OpenWeatherForecastItem item)
    {
        return new WeatherInfo
        {
            DateTime = DateTime.Parse(item.DtTxt ?? ""),
            TemperatureCelsius = item.Main.Temp,
            Conditions = item.Weather.Select(w => w.Main).ToArray()
        };
    }
}