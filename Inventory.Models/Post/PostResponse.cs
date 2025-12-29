using System.Text.Json.Serialization;
using Inventory.Common.Helpers;

namespace Inventory.Models.Post;

public class APIPostResponse
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;

}

public class PostResponse
{
    [JsonIgnore]
    public int UserId { get; set; }
    public string EncryptedUserId
         => EncryptionHelper.EncryptId(UserId.ToString());

    [JsonIgnore]
    public int Id { get; set; }
    public string EncryptedId
         => EncryptionHelper.EncryptId(Id.ToString());
    public string Title { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;

}
