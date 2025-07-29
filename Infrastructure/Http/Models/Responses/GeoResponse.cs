using System.Text.Json.Serialization;

namespace WeatherMcpServer.Infrastructure.Http.Models.Responses;

public class GeoResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = null!;
}