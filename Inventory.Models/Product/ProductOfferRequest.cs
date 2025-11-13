using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Product;

public class ProductOfferRequest
{
    [Required(ErrorMessage = "Discount price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Discounted price must be greater than or equal to 0.")]
    public decimal DiscountedPrice { get; set; }

    [Required(ErrorMessage = "Discount end date is required.")]
    public DateOnly DiscountEndOn { get; set; }
}
