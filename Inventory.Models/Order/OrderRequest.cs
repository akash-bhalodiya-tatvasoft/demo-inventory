using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Order;

public class OrderRequest
{
    public int Status { get; set; } = 1;

    public List<OrderItemRequest> OrderItems { get; set; } = new();
}

public class OrderItemRequest
{
    public string ProductIdEnc { get; set; }

    public int Quantity { get; set; }
}
