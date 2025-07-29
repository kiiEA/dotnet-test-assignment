using Microsoft.Extensions.Options;
using WeatherMcpServer.Configuration;

namespace WeatherMcpServer.Infrastructure.Handlers;

public class AuthHeaderHandler(IOptions<WeatherApiSettings> settings) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = settings.Value.ApiKey;
        request.Headers.Add("x-api-key", key);
        return base.SendAsync(request, cancellationToken);
    }
}
