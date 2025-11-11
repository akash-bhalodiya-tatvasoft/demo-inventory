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

    public bool IsActive { get; set; } = true;
    public string Permissions { get; set; } = JsonSerializer.Serialize(new UserPermissions());
}

public class UserPermissions
{
    public bool Create { get; set; } = true;
    public bool Update { get; set; } = true;
    public bool Delete { get; set; } = true;
}
