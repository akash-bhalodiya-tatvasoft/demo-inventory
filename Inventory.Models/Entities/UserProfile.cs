namespace Inventory.Models.Entities;

public class UserProfile : BaseEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? Dob { get; set; }
}
