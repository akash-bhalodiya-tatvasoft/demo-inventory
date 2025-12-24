using Inventory.Models.Entities;
using Inventory.Models.Order;

namespace Inventory.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<OrderResponse?> GetByIdAsync(int id);
    Task<int?> CreateAsync(OrderRequest order, int userId);
    Task<bool> UpdateAsync(int id, OrderRequest order, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}