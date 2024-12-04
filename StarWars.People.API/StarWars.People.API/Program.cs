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
builder.Services.AddHttpClient<CharacterService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options => { options.WithOrigins("http://localhost:5004"); });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/get", async (CharacterService characterService) =>
{
        try
        {
            var character = await characterService.GetCharactersAsync();
            return character;
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