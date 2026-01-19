using Inventory.Models.Validation;

namespace Inventory.Models.Post;

public class PostRequest
{
    public string EncryptedUserId { get; set; } = String.Empty;

    [AlphaNumericMinLengthAttribute(3)]
    public string Title { get; set; } = String.Empty;

    public string Body { get; set; } = String.Empty;
}
