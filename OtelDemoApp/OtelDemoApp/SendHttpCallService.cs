using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace OtelDemoApp;

public class SendHttpCallService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SendHttpCallService> _logger;

    public SendHttpCallService(IHttpClientFactory httpClientFactory, ILogger<SendHttpCallService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    //Send out a new call every 20 seconds
    public async Task SendRequest(int counter)
    {
        var client = _httpClientFactory.CreateClient();
        
        var hostname = Environment.GetEnvironmentVariable("TARGET_HOSTNAME") ?? "localhost";
        
        var targetUrl = $"https://{hostname}:8080/call?counter={counter}";

        try
        {
            _logger.LogInformation("Sending call to {Url}", targetUrl);
            await client.GetAsync(targetUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during http call");
        }
    }
}