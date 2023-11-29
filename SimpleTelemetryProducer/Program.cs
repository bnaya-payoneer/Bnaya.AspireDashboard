using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Context.Propagation;



const string APP_NAME = "my-app"; 

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
builder.Environment.ApplicationName = APP_NAME;
                services.AddHostedService<Worker>();

var resource = ResourceBuilder.CreateDefault().AddService(APP_NAME);

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resource);

    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.AddConsoleExporter();
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource
                                        .AddService(APP_NAME)
                                        .AddService("bmaya.demo")
                                        )
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter());

var app = builder.Build();

await app.RunAsync();
