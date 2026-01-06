using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
