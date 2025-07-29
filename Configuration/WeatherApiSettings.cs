namespace WeatherMcpServer.Configuration;

public class WeatherApiSettings
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 10;
}
