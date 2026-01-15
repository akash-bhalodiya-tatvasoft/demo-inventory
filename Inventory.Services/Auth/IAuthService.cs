using Inventory.Models.Auth;

namespace Inventory.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    Task<LoginResponse?> VerifyOtpAsync(string email, string otp);
    Task<int?> UpdateUserStatusAsync(int userId, int modifiedBy, bool isActive);
}
