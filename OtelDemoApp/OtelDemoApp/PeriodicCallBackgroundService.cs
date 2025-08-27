using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OtelDemoApp;

public class PeriodicCallBackgroundService : BackgroundService
{
    private readonly SendHttpCallService _sendHttpCallService;
    private readonly ILogger<PeriodicCallBackgroundService> _logger;

    public PeriodicCallBackgroundService(
        SendHttpCallService sendHttpCallService, 
        ILogger<PeriodicCallBackgroundService> logger)
    {
        _sendHttpCallService = sendHttpCallService;
        _logger = logger;
    }

    //Send out a new call every 20 seconds
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _sendHttpCallService.SendRequest(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during periodic call");
            }

            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}