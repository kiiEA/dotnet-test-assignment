namespace WeatherMcpServer.Domain.Entities;

public class WeatherInfo
{
    public DateTime DateTime { get; set; }
    public double TemperatureCelsius { get; set; }
    public string[] Conditions { get; set; } = [];
}