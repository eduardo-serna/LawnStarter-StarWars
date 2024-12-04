using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var redisOptions = new ConfigurationOptions
{
    EndPoints = { { "redis", 6379 } },
    //EndPoints = { { "localhost", 5001 } }, // For Local Redis enviroment, code not needed if implement env variables
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

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp",
            builder => builder.WithOrigins("http://localhost:4200") // Angular app URL
                .AllowAnyHeader()
                .AllowAnyMethod());

        options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

var app = builder.Build();

app.UseCors("AllowAngularApp");
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }


app.MapPost("/capture", async (IQueueService queueService, MetricInput input) =>
{
    var message = JsonSerializer.Serialize(input);
    await queueService.EnqueueAsync("timing_metrics_queue", message);

    return Results.Ok(new { Message = "Metric captured successfully" });
});

app.MapGet("/average", (MetricsProcessor processor) =>
{
    return Results.Ok(processor.GetMetrics());
});

app.Run();