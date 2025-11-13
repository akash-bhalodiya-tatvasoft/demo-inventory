using Inventory.Models.Entities;

namespace Inventory.Models.Product;

public class ProductResponse : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountedPrice { get; set; }

    public DateTime? DiscountEndOn { get; set; }

    public string CategoryName { get; set; } = string.Empty;

}
