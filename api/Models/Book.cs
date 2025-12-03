namespace api.Models;

public class Book
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime Published { get; set; }
    public string OwnerUsername { get; set; } = "";  // important for auth
}