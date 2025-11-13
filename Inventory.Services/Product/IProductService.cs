using Inventory.Models.Entities;
using Inventory.Models.Product;

namespace Inventory.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync(string search);
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<int> CreateAsync(ProductRequest product, int? userId);
    Task<bool> UpdateAsync(int id, ProductRequest product, int? userId);
    Task<bool> DeleteAsync(int id);
    Task AddOfferAsync(int id, ProductOfferRequest productOfferRequest, int? userId);

}
