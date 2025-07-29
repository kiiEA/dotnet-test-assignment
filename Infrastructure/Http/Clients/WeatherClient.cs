using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Infrastructure.Http.Models.Responses;

namespace WeatherMcpServer.Infrastructure.Http.Clients;

public abstract class WeatherClient(HttpClient httpClient, ILogger<WeatherClient> logger)
{
    public async Task<CurrentWeatherResponse?> GetCurrentWeatherAsync(string city, string? countryCode)
    {
        var query = string.IsNullOrWhiteSpace(countryCode) ? city : $"{city},{countryCode}";
        var endpoint = $"weather?q={query}&units=metric";

        try
        {
            return await httpClient.GetFromJsonAsync<CurrentWeatherResponse>(endpoint);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error fetching weather for {City}", query);
            return null;
        }
    }

    public async Task<ForecastResponse?> GetForecastAsync(string city, string? countryCode, int days = 3)
    {
        var query = string.IsNullOrWhiteSpace(countryCode) ? city : $"{city},{countryCode}";
        var cnt = days * 8; // 3-часовые шаги
        var endpoint = $"forecast?q={query}&units=metric&cnt={cnt}";

        try
        {
            return await httpClient.GetFromJsonAsync<ForecastResponse>(endpoint);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error fetching forecast for {City}", query);
            return null;
        }
    }

    public async Task<WeatherAlertsResponse?> GetWeatherAlertsAsync(double lat, double lon)
    {
        var endpoint = $"onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,daily,current&units=metric";

        try
        {
            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WeatherAlertsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching alerts for lat={Lat}, lon={Lon}", lat, lon);
            return null;
        }
    }
}