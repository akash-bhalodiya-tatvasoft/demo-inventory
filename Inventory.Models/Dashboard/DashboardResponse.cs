namespace Inventory.Models.Dashboard;

public class DashboardResponse
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalNewUsers { get; set; }

    public List<OrderStatusSummary> OrdersByStatus { get; set; } = [];
    public List<TopSellingProductSummary> TopSellingProducts { get; set; } = [];
    public List<LowStockProductSummary> LowStockProducts { get; set; } = [];
}

public class OrderStatusSummary
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public class TopSellingProductSummary
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
}

public class LowStockProductSummary
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
}
