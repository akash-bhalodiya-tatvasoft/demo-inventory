using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Product;

public class ProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Discounted price must be greater than or equal to 0.")]
    public decimal DiscountedPrice { get; set; }

    public DateTime? DiscountEndOn { get; set; }

    [Required(ErrorMessage = "Category ID is required.")]
    public string EnvryptedCategoryId { get; set; }
}
