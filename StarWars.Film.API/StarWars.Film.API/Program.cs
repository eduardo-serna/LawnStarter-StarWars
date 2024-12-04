using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisOptions = new ConfigurationOptions
{
    EndPoints = { { "redis", 6379 } },
    AbortOnConnectFail = false,
    DefaultDatabase = 0,
    ConnectTimeout = 5000,
    SyncTimeout = 5000
};

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddHttpClient<FilmService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options => { options.WithOrigins("http://localhost:5004"); });

// Load heavy request to cache when API init to reduce the response time in the front end.
using (var scope = app.Services.CreateScope())
{
    var filmService = scope.ServiceProvider.GetRequiredService<FilmService>();
    await filmService.GetFilmsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/get", async (FilmService filmService) =>
{
        try
        {
            var films = await filmService.GetFilmsAsync();
            return films;
        } 
        catch (HttpRequestException ex) 
        {
            return Results.NotFound($"Character with ID not found. Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Results.Problem($"An error occurred: {ex.Message}");
        }
})
.WithName("Get")
.WithOpenApi();

app.Run();
