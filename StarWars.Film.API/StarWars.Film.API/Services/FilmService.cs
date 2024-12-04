using System.Diagnostics;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using StarWars.Film.API.Models;

public class FilmService
{
    private readonly HttpClient _httpClient;
    private readonly IDatabase _redis;

    public FilmService(HttpClient httpClient, IConnectionMultiplexer muxer)
    {
        _httpClient = httpClient;
        _redis = muxer.GetDatabase();
    }

    public async Task<IResult> GetFilmsAsync()
    {
        var cacheKey = "allFilms";
        var films = new List<Film>();

        var cachedData = await _redis.StringGetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Deserialize cached data
            films = JsonSerializer.Deserialize<List<Film>>(cachedData!);
        }
        else
        {
            try
            {
               string nextUrl = "https://swapi.dev/api/films/";

                while (!string.IsNullOrEmpty(nextUrl))
                {
                    var sw = Stopwatch.StartNew(); // start watch
                    var response = await _httpClient.GetAsync(nextUrl);
                    response.EnsureSuccessStatusCode();

                    // Post http content
                    using StringContent jsonContent = new(
                        JsonSerializer.Serialize(new
                        {
                            executionTime = sw.ElapsedMilliseconds,
                            queryName = "GetPeople()"
                        }),
                        Encoding.UTF8,
                        "application/json");
                    await _httpClient.PostAsync("http://host.docker.internal:5005/capture", jsonContent);
                    // await _httpClient.PostAsync("http://localhost:5005/capture", jsonContent); // DEV Only
                    sw.Stop(); // stop watch

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var page = JsonSerializer.Deserialize<StarWarsPage<Film>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (page != null && page.Results != null)
                    {
                        films.AddRange(page.Results);
                        nextUrl = page.Next!;
                    }
                    else
                    {
                        break;
                    }
                }

                 // Serialize data and store it in Redis cache
                var serializedData = JsonSerializer.Serialize(films);
                await _redis.StringSetAsync(cacheKey, serializedData);

            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while fetching data: {ex.Message}");
            }
        }

        return Results.Ok(films);
    }
}
