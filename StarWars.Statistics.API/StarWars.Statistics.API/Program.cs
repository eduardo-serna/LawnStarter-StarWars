using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var redisOptions = new ConfigurationOptions
{
    EndPoints = { { "redis", 6379 } },
    AbortOnConnectFail = false,
    DefaultDatabase = 0,
    ConnectTimeout = 5000,
    SyncTimeout = 5000
};

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddSingleton<IQueueService, RedisQueueService>();
builder.Services.AddSingleton<MetricsProcessor>();
builder.Services.AddHostedService<MetricsBackgroundService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options => { options.WithOrigins("http://localhost:5004"); }); // Just in case we want to create a view in the frontend to get data

// Allow swagger for this API
app.UseSwagger();
app.UseSwaggerUI();



app.MapPost("/capture", async (IQueueService queueService, MetricInput input) =>
{
    var message = JsonSerializer.Serialize(input);
    await queueService.EnqueueAsync("timing_metrics_queue", message);

    return Results.Ok(new { Message = "Metric captured successfully" });
});

// Return count and average run time on ms
app.MapGet("/average", (MetricsProcessor processor) =>
{
    return Results.Ok(processor.GetMetrics());
});

app.Run();