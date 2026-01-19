using Inventory.Models.Validation;

namespace Inventory.Models.Dashboard;

[DateRangeValidationAttribute]
public class DashboardRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}