namespace Inventory.Models.Post;

public class PostRequest
{
    public int UserId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;

}
