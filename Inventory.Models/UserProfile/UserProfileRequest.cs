using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.UserProfiles;

public class UserProfileRequest
{
    [Required(ErrorMessage = "UserId is required.")]
    public string EncryptedUserId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Gender cannot exceed 20 characters.")]
    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Invalid date format for Date of Birth.")]
    public DateOnly? Dob { get; set; }
}
