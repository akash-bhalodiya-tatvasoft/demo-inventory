namespace Inventory.Models.Post;

public class PostResponse
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;

}
