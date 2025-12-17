using Inventory.Models.Entities;

namespace Inventory.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<int> MarkUserInactive(int userId);
}