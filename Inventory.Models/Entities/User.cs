using System.Text.Json;

namespace Inventory.Models.Entities;

public class User : BaseEntity
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 1 = SuperAdmin, 2 = Admin, 3 = User
    /// </summary>
    public int Role { get; set; } = 3;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public bool IsActive { get; set; } = true;

    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public int OtpFailedAttempts { get; set; }
    public DateTime? LockoutUntil { get; set; }

    public string Permissions { get; set; } = JsonSerializer.Serialize(new UserPermissions());
    public DateTime? LastLogin { get; set; }

    public UserProfile UserProfile { get; set; } = null!;

}

public class UserPermissions
{
    public bool Create { get; set; } = true;
    public bool Update { get; set; } = true;
    public bool Delete { get; set; } = true;
}
