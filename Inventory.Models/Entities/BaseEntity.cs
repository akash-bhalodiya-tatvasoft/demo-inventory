namespace Inventory.Models.Entities;

public abstract class BaseEntity
{
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
}
