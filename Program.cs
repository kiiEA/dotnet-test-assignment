using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Application.Services;
using WeatherMcpServer.Configuration;
using WeatherMcpServer.Infrastructure.ApiClients;
using WeatherMcpServer.Infrastructure.Formatters;
using WeatherMcpServer.Infrastructure.Handlers;
using WeatherMcpServer.Infrastructure.Http.Policies;
using WeatherMcpServer.Presentation.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Конфигурация
builder.Services
    .AddOptions<WeatherApiSettings>()
    .Bind(builder.Configuration.GetSection("WeatherApi"))
    .ValidateDataAnnotations()
    .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "API key is required");

// HTTP-клиент с лучшими практиками (Polly, DelegatingHandler, Named client)
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient<WeatherHttpClient>("OpenWeather", (sp, client) =>
    {
        var cfg = sp.GetRequiredService<IOptions<WeatherApiSettings>>().Value;
        client.BaseAddress = new Uri(cfg.BaseUrl!);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "WeatherMcpClient");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>()
    .AddPolicyHandler(HttpPolicyBuilder.GetRetryPolicy())
    .AddPolicyHandler(HttpPolicyBuilder.GetTimeoutPolicy());

// DI: сервисы, форматтеры, адаптеры
builder.Services.AddScoped<IWeatherApiClient>(sp =>
    sp.GetRequiredService<WeatherHttpClient>()
);
builder.Services.AddScoped<IWeatherFormatter, PlainTextWeatherFormatter>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// MCP Tools
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

// Проверка конфигурации перед запуском
var tempProvider = builder.Services.BuildServiceProvider();
var settings = tempProvider.GetRequiredService<IOptions<WeatherApiSettings>>().Value;
if (string.IsNullOrWhiteSpace(settings.ApiKey))
{
    Console.Error.WriteLine("❌ Missing OpenWeather API key. Check appsettings.json or environment variables.");
    return;
}

// Запуск
await builder.Build().RunAsync();
