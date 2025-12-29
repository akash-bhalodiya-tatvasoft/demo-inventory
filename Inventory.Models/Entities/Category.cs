using System.Text.Json.Serialization;
using Inventory.Common.Helpers;

namespace Inventory.Models.Entities;

public class Category : BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }

    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
