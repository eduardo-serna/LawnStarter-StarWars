using System.Text.Json;

public class MetricsProcessor : BackgroundService
{
    private readonly IQueueService _queueService;
    private readonly List<long> _executionTimes = new();
    private readonly object _lock = new();

    public MetricsProcessor(IQueueService queueService)
    {
        _queueService = queueService;
    }

    public Dictionary<string, object> GetMetrics()
    {
        lock (_lock)
        {
            if (_executionTimes.Count == 0)
                return new Dictionary<string, object> { { "average", 0 }, { "count", 0 } };

            var average = _executionTimes.Average();
            return new Dictionary<string, object>
            {
                { "average", average },
                { "count", _executionTimes.Count }
            };
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _queueService.DequeueAsync("timing_metrics_queue");
            if (message != null)
            {
                var metric = JsonSerializer.Deserialize<MetricInput>(message);
                if (metric != null)
                {
                    lock (_lock)
                    {
                        _executionTimes.Add(metric.ExecutionTime);
                        if (_executionTimes.Count > 1000) // Clean the list to avoid staling the data
                        {
                            _executionTimes.RemoveAt(0);
                        }
                    }
                }
            }
        }
    }

    public async Task ProcessMetricsAsync(CancellationToken cancellationToken)
    {
        await ExecuteAsync(cancellationToken);
    }
}