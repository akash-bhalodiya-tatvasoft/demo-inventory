using System.Text.Json.Serialization;
using Inventory.Common.Helpers;

namespace Inventory.Models.Entities;

public abstract class BaseEntity
{
    [JsonIgnore]
    public int? CreatedBy { get; set; }
    public string? EncryptedCreatedBy
    {
        get => EncryptionHelper.EncryptId(CreatedBy.ToString());
    }
    [JsonIgnore]
    public int? ModifiedBy { get; set; }
    public string? EncryptedModifiedBy
    {
        get => EncryptionHelper.EncryptId(ModifiedBy.ToString());
    }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
}
