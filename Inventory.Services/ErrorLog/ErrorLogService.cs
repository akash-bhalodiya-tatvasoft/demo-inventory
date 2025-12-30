using Inventory.Common.Requests;
using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.ErrorLog;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services;

public class ErrorLogService : IErrorLogService
{
    private readonly AppDbContext _context;

    public ErrorLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<ErrorLog>> GetAllAsync(ErrorLogFilterRequest request)
    {
        var query = _context.ErrorLogs.AsNoTracking();

        if (request.FromDate.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(request.FromDate.Value, DateTimeKind.Utc);
            query = query.Where(x => x.LogDate >= fromUtc);
        }

        if (request.ToDate.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(
                request.ToDate.Value.Date.AddDays(1).AddTicks(-1),
                DateTimeKind.Utc);

            query = query.Where(x => x.LogDate <= toUtc);
        }

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.LogDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<ErrorLog>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRecords = totalCount
        };
    }

    public async Task<ErrorLog?> GetByIdAsync(int id)
    {
        return await _context.ErrorLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
