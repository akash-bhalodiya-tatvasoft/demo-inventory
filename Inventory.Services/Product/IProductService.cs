using Inventory.Models.Entities;
using Inventory.Models.Product;

namespace Inventory.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> CreateAsync(ProductRequest product, int? userId);
    Task<bool> UpdateAsync(int id, ProductRequest product, int? userId);
    Task<bool> DeleteAsync(int id);
}
