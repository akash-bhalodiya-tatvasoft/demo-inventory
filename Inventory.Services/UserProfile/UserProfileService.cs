using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.UserProfiles;
using Inventory.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services.Implementations;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;

    public UserProfileService(AppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<IEnumerable<UserProfile>> GetAllAsync()
    {
        return await _context.UserProfiles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<UserProfile?> GetByIdAsync(int id)
    {
        return await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<UserProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<int?> CreateAsync(UserProfileRequest request, int? userId)
    {

        var user = await _userService.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return null;
        }

        var entity = new UserProfile
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = !string.IsNullOrWhiteSpace(request.Gender) ? request.Gender : "",
            PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.PhoneNumber : "",
            Dob = request.Dob.HasValue ? DateTime.SpecifyKind(request.Dob.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc) : null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.UserProfiles.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, UserProfileRequest request, int? userId)
    {
        var profile = await _context.UserProfiles.FindAsync(id);
        if (profile == null)
            return false;

        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.Gender))
            profile.Gender = request.Gender;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            profile.PhoneNumber = request.PhoneNumber;
        if (request.Dob.HasValue)
            profile.Dob = DateTime.SpecifyKind(request.Dob.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        profile.ModifiedAt = DateTime.UtcNow;
        profile.ModifiedBy = userId;

        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var profile = await _context.UserProfiles.FindAsync(id);
        if (profile == null)
            return false;

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        return true;
    }
}
