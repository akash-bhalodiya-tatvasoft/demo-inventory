namespace Inventory.Models.Entities;

public class UserActivityLog : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string? ActivityDescription { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int ActivityModule { get; set; }
}
