public class MetricsBackgroundService : BackgroundService
{
    private readonly MetricsProcessor _metricsProcessor;

    public MetricsBackgroundService(MetricsProcessor metricsProcessor)
    {
        _metricsProcessor = metricsProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken); // Delay for 10 seconds before running again
            await _metricsProcessor.ProcessMetricsAsync(stoppingToken);
        }
    }
}