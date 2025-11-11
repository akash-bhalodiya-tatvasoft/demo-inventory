using Inventory.Models.Category;
using Inventory.Models.Entities;

namespace Inventory.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> CreateAsync(CategoryRequest request, int? userId);
    Task<bool> UpdateAsync(int id, CategoryRequest request, int? userId);
    Task<bool> DeleteAsync(int id);
}
