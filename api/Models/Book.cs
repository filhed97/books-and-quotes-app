namespace api.Models;

public class Book
{
    // Using camelCase to avoid serialization headaches. 
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string title { get; set; } = "";
    public string author { get; set; } = "";
    public DateTime published { get; set; }
    public string userId { get; set; } = "";
}