using Inventory.Models.Auth;

namespace Inventory.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    Task<int?> UpdateUserStatusAsync(int userId, int modifiedBy, bool isActive);
}
