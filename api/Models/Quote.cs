namespace api.Models;

public class Quote
{
    // Using camelCase to avoid serialization headaches with Cosmos. 
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string quote { get; set; } = "";
    public string author { get; set; } = "";
    public string userId { get; set; } = "";
}