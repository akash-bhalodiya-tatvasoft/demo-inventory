
namespace Inventory.Models.Entities;

public class Order : BaseEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Status { get; set; }

    public decimal TotalAmount { get; set; }

    public User User { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
