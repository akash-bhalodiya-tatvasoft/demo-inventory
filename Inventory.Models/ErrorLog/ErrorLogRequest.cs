using Inventory.Common.Requests;

namespace Inventory.Models.ErrorLog;

public class ErrorLogFilterRequest : PaginationRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
