using StackExchange.Redis;

public class RedisQueueService : IQueueService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisQueueService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task EnqueueAsync(string queueName, string message)
    {
        var db = _redis.GetDatabase();
        await db.ListRightPushAsync(queueName, message);
    }

    public async Task<string?> DequeueAsync(string queueName)
    {
        var db = _redis.GetDatabase();
        return await db.ListLeftPopAsync(queueName);
    }
}
