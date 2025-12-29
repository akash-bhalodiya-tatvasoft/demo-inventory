using System.Text.Json.Serialization;
using Inventory.Common.Helpers;
using Inventory.Models.Entities;

namespace Inventory.Models.UserProfiles;

public class UserProfileResponse : BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }
    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? Dob { get; set; }

    public string Email { get; set; } = string.Empty;
}
