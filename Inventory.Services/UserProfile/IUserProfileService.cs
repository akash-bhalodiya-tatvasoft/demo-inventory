using Inventory.Models.Entities;
using Inventory.Models.UserProfiles;

namespace Inventory.Services.Interfaces;

public interface IUserProfileService
{
    Task<IEnumerable<UserProfileResponse>> GetAllAsync(string search);
    Task<UserProfileResponse?> GetByIdAsync(int id);
    Task<int?> CreateAsync(UserProfileRequest request, int? userId);
    Task<bool> UpdateAsync(int id, UserProfileRequest request, int? userId);
    Task<bool> DeleteAsync(int id);
    Task<UserProfile?> GetByUserIdAsync(int userId);

}
