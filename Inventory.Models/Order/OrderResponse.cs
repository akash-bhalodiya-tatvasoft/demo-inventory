using System;
using System.Text.Json.Serialization;
using Inventory.Common.Helpers;

namespace Inventory.Models.Order;

public class OrderResponse
{
    [JsonIgnore]
    public int Id { get; set; }
    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }
    [JsonIgnore]
    public int UserId { get; set; }
    public string EncryptedUserId
    {
        get => EncryptionHelper.EncryptId(UserId.ToString());
    }
    public string UserName { get; set; } = string.Empty;
    public int Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<OrderItemResponse> OrderItems { get; set; } = new();
}

public class OrderItemResponse
{
    [JsonIgnore]
    public int Id { get; set; }
    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }
    [JsonIgnore]
    public int ProductId { get; set; }
    public string EncryptedProductId
    {
        get => EncryptionHelper.EncryptId(ProductId.ToString());
    }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
