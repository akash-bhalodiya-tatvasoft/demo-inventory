using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Auth;

public class VerifyOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Invalid OTP.")]
    public string Otp { get; set; } = string.Empty;
}