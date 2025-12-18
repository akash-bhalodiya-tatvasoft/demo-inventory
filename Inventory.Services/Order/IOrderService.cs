using Inventory.Models.Entities;
using Inventory.Models.Order;

namespace Inventory.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllAsync();
    Task<OrderResponse?> GetByIdAsync(int id);
    Task<int?> CreateAsync(OrderRequest order, int userId);
}