using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Order;

public class OrderRequest
{
    [Required(ErrorMessage = "Status is required.")]
    [Range(1, 4, ErrorMessage = "Invalid order status.")]
    public int Status { get; set; } = 1;

    [Required(ErrorMessage = "Order must contain at least one item.")]
    [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
    public List<OrderItemRequest> OrderItems { get; set; } = new();
}

public class OrderItemRequest
{
    [Required(ErrorMessage = "ProductId is required.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}
