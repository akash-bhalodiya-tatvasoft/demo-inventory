using System.Data;
using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.Order;
using Inventory.Services.Interfaces;
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
        .Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            Status = o.Status,
            UserName = o.User != null && o.User.UserProfile != null
                ? $"{o.User.UserProfile.FirstName} {o.User.UserProfile.LastName}"
                : String.Empty,
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,
            OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product != null ? oi.Product.Name : String.Empty,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        })
        .ToListAsync();
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product).Select(o => new OrderResponse
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Status = o.Status,
                    UserName = o.User != null && o.User.UserProfile != null
                ? $"{o.User.UserProfile.FirstName} {o.User.UserProfile.LastName}"
                : String.Empty,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product != null ? oi.Product.Name : String.Empty,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.TotalPrice
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        return order;
    }
}
