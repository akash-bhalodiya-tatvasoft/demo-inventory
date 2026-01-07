namespace Inventory.Models.Entities;

public class Product : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountedPrice { get; set; }

    public DateOnly? DiscountEndOn { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public string? ProductImage { get; set; }
    public string? ProductImageUrl { get; set; }
}
