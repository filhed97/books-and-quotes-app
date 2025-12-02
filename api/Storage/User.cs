namespace api.Storage;

using Newtonsoft.Json;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!; // Partition key
    [JsonProperty("username")]
    public string Username { get; set; } = default!;
    [JsonProperty("passwordHash")]
    public string PasswordHash { get; set; } = default!;
}