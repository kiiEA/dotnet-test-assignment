using Microsoft.Extensions.Logging;

namespace WeatherMcpServer.Infrastructure.Http.DelegatingHandlers;

public class WeatherLoggingHandler : DelegatingHandler
{
    private readonly ILogger<WeatherLoggingHandler> _logger;

    public WeatherLoggingHandler(ILogger<WeatherLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending request to {Url}", request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Request to {Url} failed with status code {StatusCode}. Response: {Content}",
                request.RequestUri, response.StatusCode, content);
        }

        return response;
    }
}