using Inventory.Common.Helpers;

namespace Inventory.Models.Entities;

public class OrderItem : BaseEntity
{
    public int Id { get; set; }
    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }

    public int OrderId { get; set; }
    public string EncryptedOrderId
    {
        get => EncryptionHelper.EncryptId(OrderId.ToString());
    }

    public int ProductId { get; set; }
    public string EncryptedProductId
    {
        get => EncryptionHelper.EncryptId(ProductId.ToString());
    }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
