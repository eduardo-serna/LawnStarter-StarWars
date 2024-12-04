public interface IQueueService
{
    Task EnqueueAsync(string queueName, string message);

    Task<string?> DequeueAsync(string queueName);
}