using System.Text.Json.Serialization;

namespace WeatherMcpServer.Infrastructure.Http.Models.Responses;

public abstract class OpenWeatherForecastResponse
{
    [JsonPropertyName("cod")]
    public string Cod { get; set; } = null!;

    [JsonPropertyName("message")]
    public int Message { get; set; }

    [JsonPropertyName("cnt")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public List<OpenWeatherForecastItem> List { get; set; } = [];

    [JsonPropertyName("city")]
    public CityInfo City { get; set; } = null!;
}

public abstract class CityInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = null!;

    [JsonPropertyName("coord")]
    public Coordinates Coord { get; set; } = null!;
}