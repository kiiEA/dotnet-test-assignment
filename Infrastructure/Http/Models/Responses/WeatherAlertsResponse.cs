using System.Text.Json.Serialization;

namespace WeatherMcpServer.Infrastructure.Http.Models.Responses;

public class WeatherAlertsResponse
{
    [JsonPropertyName("alerts")]
    public WeatherAlert[]? Alerts { get; set; }
}

public class WeatherAlert
{
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; set; }

    [JsonPropertyName("event")]
    public string? Event { get; set; }

    [JsonPropertyName("start")]
    public long StartUnix { get; set; }

    [JsonPropertyName("end")]
    public long EndUnix { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}