using Inventory.Models.Entities;

namespace Inventory.Models.UserProfiles;

public class UserProfileResponse : BaseEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? Dob { get; set; }

    public string Email { get; set; } = string.Empty;
}
