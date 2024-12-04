using System.Text.Json.Serialization;

namespace StarWars.Film.API.Models {
    public class Film
    {
        [JsonPropertyName("title")]
        public string? Name { get; set; }

        [JsonPropertyName("opening_crawl")]
        public string? OpeningCrawl { get; set; }

        public List<string>? Characters { get; set; }
    }
}
