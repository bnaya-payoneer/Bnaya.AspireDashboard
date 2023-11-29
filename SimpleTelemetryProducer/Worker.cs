using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using SimpleTelemetryProducer;

internal class Worker : IHostedLifecycleService
{
    private readonly ILogger<Worker> _logger;
    private readonly Meter _meter;
    private readonly Counter<int> _counter;
    private readonly ActivitySource _trace;
    private readonly CancellationTokenSource _cancellation =  new CancellationTokenSource();
    private Task? _task;
    private readonly Random _random = new Random();

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _meter = new Meter("bnaya.meters", "1.0.0");
        _counter = _meter.CreateCounter<int>("demo.count", description: "Counts the number of ...");

        // Custom ActivitySource for the application
        _trace = new ActivitySource("bmaya.demo");
    }
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start worker");

        _task = Task.Run(ExecAsync);

        return Task.CompletedTask;
    }

    private async Task ExecAsync()
    {
        int i = 0;
        while (!_cancellation.Token.IsCancellationRequested)
        {
            using var trc = _trace.StartActivity($"bnaya-{i++}");
            _counter.Add(1);
            Logs.Executing(_logger, i);
            await Task.Delay(_random.Next(1000, 3000));
            trc?.SetTag("completed", true);
        }
    }

    Task IHostedLifecycleService.StartedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker had started");
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker is starting");
        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop worker");
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StoppedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker had stopped");
        return Task.CompletedTask;
    }

    Task IHostedLifecycleService.StoppingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker is stopping");
        return Task.CompletedTask;
    }
}