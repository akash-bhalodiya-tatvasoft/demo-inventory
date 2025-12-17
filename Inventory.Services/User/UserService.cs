using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
            return null;

        return user;
    }



    public async Task<int> MarkUserInactive(int inactiveMonths)
    {
        var cutoffDate = DateTimeOffset.UtcNow.AddMonths(-inactiveMonths);

        var affectedRows = await _context.Users
            .Where(u =>
                u.IsActive &&
                u.LastLogin != null &&
                u.LastLogin <= cutoffDate)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.IsActive, false)
            );

        return affectedRows;
    }
}