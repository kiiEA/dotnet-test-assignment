using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using WeatherMcpServer.Infrastructure.Http.Clients;
using WeatherMcpServer.Infrastructure.Http.DelegatingHandlers;
using WeatherMcpServer.Infrastructure.Http.Policies;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace WeatherMcpServer.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherHttpClient(this IServiceCollection services)
    {
        services.AddTransient<WeatherLoggingHandler>();

        services.AddHttpClient<WeatherClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IConfiguration>().GetSection("OpenWeatherMap");
                var apiKey = options["ApiKey"];

                client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "WeatherMcpClient");
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }
            })
            .AddHttpMessageHandler<WeatherLoggingHandler>() // 👁️ логируем каждый запрос/ответ
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))     // ♻️ перезапускаем HttpMessageHandler
            .AddPolicyHandler(HttpPolicyBuilder.GetRetryPolicy())     // 🔁 retry (Polly)
            .AddPolicyHandler(HttpPolicyBuilder.GetTimeoutPolicy());  // ⏱️ timeout (Polly)

        return services;
    }
}