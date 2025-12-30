using Inventory.Common.Requests;
using Inventory.Common.Responses;
using Inventory.Models.Entities;
using Inventory.Models.ErrorLog;

public interface IErrorLogService
{
    Task<PaginatedResponse<ErrorLog>> GetAllAsync(ErrorLogFilterRequest request);
    Task<ErrorLog?> GetByIdAsync(int id);
}
