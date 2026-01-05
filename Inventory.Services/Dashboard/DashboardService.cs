using Inventory.Context;
using Inventory.Common.Enums;
using Inventory.Models.Dashboard;
using Inventory.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> GetDashboardAsync(DashboardRequest request)
    {
        var orders = _context.Orders
            .Where(o => o.CreatedAt >= request.FromDate &&
                        o.CreatedAt <= request.ToDate);

        var users = _context.Users
            .Where(u => u.CreatedAt >= request.FromDate &&
                        u.CreatedAt <= request.ToDate);

        var orderItems = _context.OrderItems
            .Where(oi => oi.CreatedAt >= request.FromDate &&
                         oi.CreatedAt <= request.ToDate);

        return new DashboardResponse
        {
            TotalOrders = await orders.CountAsync(),
            TotalRevenue = await orders
                .Where(o => o.Status == (int)OrderStatus.Completed)
                .SumAsync(o => o.TotalAmount),
            TotalNewUsers = await users.CountAsync(),

            OrdersByStatus = await orders
                .GroupBy(o => (OrderStatus)o.Status)
                .Select(g => new OrderStatusSummary
                {
                    Status = (int)g.Key,
                    Count = g.Count()
                })
                .ToListAsync(),

            TopSellingProducts = await orderItems
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopSellingProductSummary
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync(),

            LowStockProducts = await _context.Products
                .Where(p => p.Quantity < 10)
                .Select(p => new LowStockProductSummary
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    AvailableQuantity = p.Quantity
                })
                .ToListAsync()
        };
    }
}
