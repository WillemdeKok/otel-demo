using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OtelDemoApp;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry setup
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.AddConsoleExporter();
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        //OpenTelemetry.Instrumentation.AspNetCore
        metrics.AddAspNetCoreInstrumentation()
            //OpenTelemetry.Instrumentation.Http
            .AddHttpClientInstrumentation()
            //OpenTelemetry.Instrumentation.Runtime
            .AddRuntimeInstrumentation();
    })
    .WithTracing(tracing =>
    {
        tracing
            //OpenTelemetry.Instrumentation.AspNetCore
            .AddAspNetCoreInstrumentation()
            //OpenTelemetry.Instrumentation.Http
            .AddHttpClientInstrumentation();
    });

builder.Services.AddHttpClient();

builder.Services.AddTransient<SendHttpCallService>();

// Background service that sends out a periodic request to another instance. 
builder.Services.AddHostedService<PeriodicCallBackgroundService>();

var app = builder.Build();

app.MapGet("/call", async (ILogger<Program> logger, SendHttpCallService httpCallService, int counter) =>
{
    var maxCalls = int.Parse(Environment.GetEnvironmentVariable("MAX_CALLS") ?? "3");

    if (counter <= maxCalls)
    {
        await httpCallService.SendRequest(counter + 1);    
    }
    
    return Results.Ok($"Processed call with value {counter} at {DateTime.UtcNow}");
});

app.Urls.Add("https://*:8080");

app.Run();