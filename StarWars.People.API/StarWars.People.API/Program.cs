using StackExchange.Redis;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddHttpClient<CharacterService>();

// For Local Redis running
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