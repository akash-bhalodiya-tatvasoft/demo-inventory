namespace Inventory.Models.ErrorLog;

public class ErrorLogDeleteByDateRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
