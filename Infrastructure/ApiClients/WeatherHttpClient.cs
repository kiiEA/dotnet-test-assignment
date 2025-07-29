using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain.Entities;
using WeatherMcpServer.Exceptions;
using WeatherMcpServer.Infrastructure.Http;
using WeatherMcpServer.Infrastructure.Http.Mappers;
using WeatherMcpServer.Infrastructure.Http.Models.Responses;

namespace WeatherMcpServer.Infrastructure.ApiClients;

public class WeatherHttpClient(
    HttpClient httpClient,
    IOptions<WeatherApiOptions> options,
    ILogger<WeatherHttpClient> logger)
    : IWeatherApiClient
{
    private readonly WeatherApiOptions _options = options.Value;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<WeatherInfo?> GetCurrentWeatherAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        var query = BuildLocationQuery(city, countryCode);
        var url = $"weather?q={query}&appid={_options.ApiKey}&units=metric";

        logger.LogInformation("Requesting current weather: {Url}", url);
        var response = await httpClient.GetAsync(url, cancellationToken);
        await HandleErrors(response);

        var data = await response.Content.ReadFromJsonAsync<OpenWeatherCurrentResponse>(_serializerOptions, cancellationToken);
        return data?.ToDomain();
    }

    public async Task<IReadOnlyCollection<WeatherInfo>> GetForecastAsync(string city, string? countryCode = null, int days = 3, CancellationToken cancellationToken = default)
    {
        var query = BuildLocationQuery(city, countryCode);
        var url = $"forecast?q={query}&appid={_options.ApiKey}&units=metric";

        logger.LogInformation("Requesting forecast: {Url}", url);
        var response = await httpClient.GetAsync(url, cancellationToken);
        await HandleErrors(response);

        var data = await response.Content.ReadFromJsonAsync<OpenWeatherForecastResponse>(_serializerOptions, cancellationToken);
        return data?.List.Select(x => x.ToDomain()).ToList() ?? [];
    }

    public async Task<string> GetAlertsAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        var coord = await GetCoordinatesAsync(city, countryCode, cancellationToken);
        if (coord is null)
            throw new CityNotFoundException($"City not found: {city}, {countryCode}");

        var url = $"onecall?lat={coord.Value.Lat}&lon={coord.Value.Lon}&appid={_options.ApiKey}&exclude=current,minutely,hourly,daily";

        logger.LogInformation("Requesting alerts: {Url}", url);
        var response = await httpClient.GetAsync(url, cancellationToken);
        await HandleErrors(response);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return json; // or parse if needed
    }

    private async Task<(double Lat, double Lon)?> GetCoordinatesAsync(string city, string? countryCode, CancellationToken cancellationToken)
    {
        var query = BuildLocationQuery(city, countryCode);
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={query}&limit=1&appid={_options.ApiKey}";

        logger.LogInformation("Requesting coordinates: {Url}", url);
        var response = await httpClient.GetAsync(url, cancellationToken);
        await HandleErrors(response);

        var coords = await response.Content.ReadFromJsonAsync<List<GeoResponse>>(_serializerOptions, cancellationToken);
        var first = coords?.FirstOrDefault();
        return first == null ? null : (first.Lat, first.Lon);
    }

    private async Task HandleErrors(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var content = await response.Content.ReadAsStringAsync();
        logger.LogError("API error: {StatusCode}, Content: {Content}", response.StatusCode, content);

        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new CityNotFoundException("City not found"),
            HttpStatusCode.Unauthorized => new InvalidTokenException("Invalid API token"),
            HttpStatusCode.TooManyRequests => new RateLimitExceededException("Rate limit exceeded"),
            _ => new ApiException($"API error: {response.StatusCode} - {content}")
        };
    }

    private static string BuildLocationQuery(string city, string? countryCode)
        => string.IsNullOrWhiteSpace(countryCode) ? city : $"{city},{countryCode}";}