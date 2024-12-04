namespace StarWars.People.API.Models {
    public class Character
    {
        public string? Name { get; set; }
        public List<string>? Films { get; set; }
        public required string Url { get; set; }
    }
}