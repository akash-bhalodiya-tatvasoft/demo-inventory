using Inventory.Models.Category;
using Inventory.Models.Entities;

namespace Inventory.Services.Interfaces;

public interface IMemoryCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}