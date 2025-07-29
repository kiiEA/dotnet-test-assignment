using System.Text.Json.Serialization;

namespace WeatherMcpServer.Infrastructure.Http.Models.Responses;

public class OpenWeatherForecastItem
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public OpenWeatherMain Main { get; set; } = null!;

    [JsonPropertyName("weather")]
    public List<OpenWeatherCondition> Weather { get; set; } = [];

    [JsonPropertyName("clouds")]
    public OpenWeatherClouds Clouds { get; set; } = null!;

    [JsonPropertyName("wind")]
    public OpenWeatherWind Wind { get; set; } = null!;

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("pop")]
    public double Pop { get; set; } // probability of precipitation

    [JsonPropertyName("dt_txt")]
    public string? DtTxt { get; set; }
}