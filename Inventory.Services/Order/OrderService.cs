using System.Data;
using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.Order;
using Inventory.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int?> CreateAsync(OrderRequest orderRequest, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.UtcNow;
            var orderItems = new List<OrderItem>();

            var products = await _context.Products
                            .Where(p => orderRequest.OrderItems.Select(x => x.ProductId).ToList().Contains(p.Id))
                            .ToListAsync();

            foreach (var item in orderRequest.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    return null;
                }

                product.Quantity = product.Quantity - item.Quantity;
                product.ModifiedAt = now;
                product.ModifiedBy = userId;
                _context.Products.Update(product);

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = item.Quantity * product.Price,
                    CreatedAt = now,
                    CreatedBy = userId,
                    ModifiedAt = null
                });
            }

            var order = new Order
            {
                UserId = (int)userId,
                Status = orderRequest.Status,
                CreatedAt = now,
                CreatedBy = userId,
                OrderItems = orderItems,
                ModifiedAt = null
            };

            order.TotalAmount = orderItems.Sum(oi => oi.TotalPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return order.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        return await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .AsNoTracking()
        .ProjectToType<OrderResponse>()
        .ToListAsync();
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .ProjectToType<OrderResponse>()
                .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        return order;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return false;

        var now = DateTime.UtcNow;

        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Quantity += item.Quantity;
                product.ModifiedAt = now;
                product.ModifiedBy = userId;
            }
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(int id, OrderRequest request, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            var now = DateTime.UtcNow;

            foreach (var oldItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(oldItem.ProductId);
                if (product != null)
                {
                    product.Quantity += oldItem.Quantity;
                    product.ModifiedAt = now;
                    product.ModifiedBy = userId;
                }
            }

            _context.OrderItems.RemoveRange(order.OrderItems);

            var productIds = request.OrderItems.Select(x => x.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var newItems = new List<OrderItem>();

            foreach (var item in request.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                product.Quantity -= item.Quantity;
                product.ModifiedAt = now;
                product.ModifiedBy = userId;

                newItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * item.Quantity,
                    CreatedAt = now,
                    CreatedBy = userId
                });
            }

            order.OrderItems = newItems;
            order.Status = request.Status;
            order.TotalAmount = newItems.Sum(x => x.TotalPrice);
            order.ModifiedAt = now;
            order.ModifiedBy = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}
