namespace WeatherMcpServer.Infrastructure.Http;

public class WeatherApiOptions
{
    public const string SectionName = "WeatherApi";

    public string BaseUrl { get; init; } = default!;
    public string ApiKey { get; init; } = default!;
    public string Units { get; init; } = "metric";
    public int TimeoutSeconds { get; init; } = 10;
}
