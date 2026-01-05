using Inventory.Models.Dashboard;

namespace Inventory.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(DashboardRequest request);
}
