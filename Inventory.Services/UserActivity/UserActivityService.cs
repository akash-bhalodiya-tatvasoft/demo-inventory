using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Inventory.Services;

public class UserActivityService : IUserActivityService
{
    private readonly AppDbContext _context;

    public UserActivityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(UserActivityLog log)
    {
        log.CreatedAt = DateTime.UtcNow;

        _context.UserActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
