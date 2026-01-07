using System.Text.Json.Serialization;
using Inventory.Common.Helpers;
using Inventory.Models.Entities;

namespace Inventory.Models.Product;

public class ProductResponse : BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }
    public string EncryptedId
    {
        get => EncryptionHelper.EncryptId(Id.ToString());
    }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountedPrice { get; set; }

    public DateTime? DiscountEndOn { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [JsonIgnore]
    public string? ProductImageUrl { get; set; }
    public string? ProductImageBase64 { get; set; }


}
